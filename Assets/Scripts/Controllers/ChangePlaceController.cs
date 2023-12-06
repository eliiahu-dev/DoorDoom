using System.Collections.Generic;
using OLS_HyperCasual;
using UnityEngine;
using UnityEngine.EventSystems;

public class ChangePlaceController : BaseMonoController<ChangePlaceView, ChangePlaceModel>
{
    public override bool HasUpdate => true;

    private const int layerDefault = 0;
    private const int layerGround = 3;
    private const int layerVisible = 6;
    private const int layerPlace = 7;
    private const float endPos = 7;
    private const float spawnPos = 16;
    private const float addTime = 0.75f;
    
    private LevelController levelController;
    private EnemyController enemyController;
    private BarrierController barrierController;
    private TeammateController teammateController;
    private GameEffectsController gameEffectsController;
    private TeammateModel dragObject;
    private ChangePlaceModel lastPlace;
    private int mateEffects;
    private int index;
    private float timeScaleValue;
    private float maxTimeScaleValue;

    public ChangePlaceController()
    {
        gameEffectsController = BaseEntryPoint.Get<GameEffectsController>();
        teammateController = BaseEntryPoint.Get<TeammateController>();
        barrierController = BaseEntryPoint.Get<BarrierController>();
        enemyController = BaseEntryPoint.Get<EnemyController>();

        var resourcesController = BaseEntryPoint.Get<ResourcesController>();
        var prefabPoof = resourcesController.GetResource<GameEffectsView>(GameResourceConstants.Poof, false);
        var prefabMate = resourcesController.GetResource<GameEffectsView>(GameResourceConstants.NewMate, false);
        
        gameEffectsController.PreInitPool("Poof", prefabPoof);
        gameEffectsController.PreInitPool("NewMate", prefabMate);
    }
    
    public override ChangePlaceModel AddView(ChangePlaceView view)
    {
        BarrierModel barrierModel = null;
        
        if (view.Stash == false)
        {
            barrierModel = barrierController.GetBarrierModel();
        }

        index++;
        var model = new ChangePlaceModel(view, barrierModel, index);

        var typeSave = PlayerPrefs.GetInt($"Place_{index}");

        if (typeSave != 0)
        {
            CreateTeammate(model, (TeammateType)typeSave);
        }

        modelsList.Add(model);
        return model;
    }
    
    public bool CanSpawn()
    {
        var count = 0;
        
        foreach (var model in modelsList)
        {
            if (model.IsStash() == false)
            {
                continue;
            }
            
            if (model.Teammate != null)
            {
                count++;
            }
        }
        
        return count + mateEffects < 10;
    }
    
    public void SpawnTeammate()
    {
        if (CanSpawn() == false)
        {
            return;
        }
        
        gameEffectsController.ShowEffect("NewMate", new Vector3(0, 0, spawnPos), new Vector3(0, 0, endPos));
        mateEffects++;
    }
    
    public void CreateDefaultTeammate()
    {
        CreateTeammate(GetEmptyPlace(), (TeammateType)1);
        mateEffects--;
    }
    
    private void CreateTeammate(ChangePlaceModel model, TeammateType type)
    {
        var teammateModel = teammateController.ShowTeammate(GetTeammateName(type));
        teammateModel.SetLayer(layerDefault);
        model.SetTeammate(teammateModel);
        
        gameEffectsController.ShowEffect("Poof", teammateModel.CachedTransform.position, Vector3.zero);
    }
    
    private void TryDragObject()
    {
        var hit = GetRaycastHit(layerDefault);
        var raycastObject = OnRaycastCanvas();
        
        dragObject = teammateController.GetSelectedTeammate(hit);

        if (dragObject == null)
        {
            if (raycastObject != levelController.GetModel().OpenButtonText)
            {
                if (enemyController.StartGame)
                {
                    timeScaleValue += addTime;
                    
                    if (timeScaleValue > maxTimeScaleValue)
                    {
                        timeScaleValue = maxTimeScaleValue;
                    }
                }
            }
            
            return;
        }
        
        dragObject.SetLayer(layerVisible);
        dragObject.VisualiseDrag();
        
        foreach (var model in modelsList)
        {
            if (model.Teammate == dragObject)
            {
                lastPlace = model;
                lastPlace.SetTypeText("");
            }
        }
    }

    private void OnDragObject()
    {
        var hit = GetRaycastHit(layerGround);

        if (hit.transform == null)
        {
            OnDropObject();
            return;
        }
        
        dragObject.CachedTransform.position = hit.point;
    }
    
    private void OnDropObject()
    {
        if (dragObject == null)
        {
            return;
        }
        
        var hit = GetRaycastHit(layerPlace);
        
        foreach (var model in modelsList)
        {
            if (model.CachedTransform != hit.transform)
            {
                continue;
            }

            if (model.Teammate != null)
            {
                if (model.Teammate == dragObject)
                {
                    SetLastPlace();
                    return;
                }
                
                var type = dragObject.GetTeammateType();
                
                if (model.Teammate.GetTeammateType() == type && (int)type < (int)TeammateType.Count - 1 && model.IsStash())
                {
                    OnMerge(model);
                    return;
                }

                SetLastPlace();
                return;
            }
            
            SetNewPlace(model);
            return;
        }

        SetLastPlace();
    }

    private void SetNewPlace(ChangePlaceModel model)
    {
        lastPlace.ClearPlace();
        model.SetTeammate(dragObject);
        ClearData();
    }
    
    private void SetLastPlace()
    {
        lastPlace.SetTeammate(dragObject);
        ClearData();
    }

    private void OnMerge(ChangePlaceModel model)
    {
        teammateController.HideTeammate(model.Teammate);
        teammateController.HideTeammate(lastPlace.Teammate);
        
        var type = model.Teammate.GetTeammateType();
        
        model.ClearPlace();
        lastPlace.ClearPlace();
        ClearData();

        type = (TeammateType)((int)type + 1);
        CreateTeammate(model, type);
    }

    private string GetTeammateName(TeammateType type)
    {
        return type.ToString();
    }

    private ChangePlaceModel GetEmptyPlace()
    {
        foreach (var model in modelsList)
        {
            if (model.IsStash() == false)
            {
                continue;
            }
            
            if (model.Teammate == null)
            {
                return model;
            }
        }
        return null;
    }
    
    private RaycastHit GetRaycastHit(int layer)
    {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, 1 << layer))
        {
            return hit;
        }
        return hit;
    }

    private GameObject OnRaycastCanvas()
    {
        GameObject go = null;
        
        if (EventSystem.current.IsPointerOverGameObject())
        {
            PointerEventData pointerData = new PointerEventData(EventSystem.current)
            {
                pointerId = -1,
            };

            pointerData.position = Input.mousePosition;

            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(pointerData, results);

            if (results.Count == 0)
            {
                return null;
            }

            go = results[0].gameObject;
        }
    
        return go;
    }
    
    private void ClearData()
    {
        dragObject.SetLayer(layerDefault);
        dragObject = null;
        lastPlace = null;
    }

    private void UpdateTimeScale(float dt)
    {
        if (timeScaleValue > 0)
        {
            Time.timeScale = 2f;
            timeScaleValue -= dt / 2;
            levelController.GetModel().TimeSlider.value = timeScaleValue;
            return;
        }

        timeScaleValue = 0;
        Time.timeScale = 1f;
    }
    
    public override void Update(float dt)
    {
        if (enemyController.EndGame)
        {
            return;
        }

        if (levelController == null)
        {
            levelController = BaseEntryPoint.Get<LevelController>();
            maxTimeScaleValue = levelController.GetModel().TimeSlider.maxValue;
        }

        if (enemyController.LastWave == false)
        {
            var state = GetEmptyPlace() != null;
            levelController.GetModel().OpenButton.SetActive(state);
        }
        else
        {
            levelController.HideOpenButton();
        }
        
        UpdateTimeScale(dt);
        
        if (Input.GetMouseButtonDown(0))
        {
            TryDragObject();
        }

        if (dragObject != null)
        {
            OnDragObject();
        }

        if (Input.GetMouseButtonUp(0))
        {
            OnDropObject();
        }
    }
}