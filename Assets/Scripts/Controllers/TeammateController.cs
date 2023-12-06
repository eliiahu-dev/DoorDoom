using System.Collections.Generic;
using OLS_HyperCasual;
using UnityEngine;

public class TeammateController : PoolableController<TeammateModel, TeammateView>
{
    public override bool HasUpdate => true;
    public int Money;
    
    private List<TeammateModel> modelList = new();
    private GameEffectsController effectsController;
    private EnemyController enemyController;

    public TeammateController()
    {
        effectsController = BaseEntryPoint.Get<GameEffectsController>();
        
        var count = (int)TeammateType.Count;
        var resourcesController = BaseEntryPoint.Get<ResourcesController>();
        var soGameSettings = resourcesController.GetResource<GameSettingsSO>(GameResourceConstants.GameSettingsSO, false);
        var effectView = resourcesController.GetResource<GameEffectsView>(GameResourceConstants.Shoot, false);
        effectsController.PreInitPool("Shoot", effectView);
        
        for (var i = 1; i < count; i++)
        {
            var name = GetTeammateName((TeammateType)i);
            var path = GetResourcePath(name, soGameSettings);
            var prefab = resourcesController.GetResource<TeammateView>(path, false);
            PreInitPool(name, prefab);
        }
    }
    
    public TeammateModel ShowTeammate(string teammateType)
    {
        var newModel = true;
        var teammate = GetFromPool(teammateType);
        teammate.ShowTeammate(true);

        foreach (var model in modelList)
        {
            if (model == teammate)
            {
                newModel = false;
                break;
            }
        }

        if (newModel)
        {
            modelList.Add(teammate);
        }
        
        return teammate;
    }

    public void HideTeammate(TeammateModel model)
    {
        ReturnToPool(model);
        model.ShowTeammate(false);
    }

    public TeammateModel GetSelectedTeammate(RaycastHit hit)
    {
        foreach (var model in modelList)
        {
            if (model.CachedTransform == hit.transform)
            {
                return model;
            }
        }

        return null;
    }

    private string GetTeammateName(TeammateType type)
    {
        return type.ToString();
    }

    private string GetResourcePath(string key, GameSettingsSO soGameSettings)
    {
        var name = soGameSettings.GetValue(key);
        return $"Prefabs/Teammates/{name}";
    }

    private string GetEnemyEffectType(EnemyType type)
    {
        return $"{type}Death";
    }
    
    public override void Update(float dt)
    {
        foreach (var model in modelList)
        {
            if (model.InAttackPlace == false)
            {
                continue;
            }
            
            enemyController ??= BaseEntryPoint.Get<EnemyController>();
            model.SetTargetEnemy(enemyController.GetNearestEnemy(model.CachedTransform.position));

            if (model.TargetEnemy == null)
            {
                var direction = Vector3.forward;
                var rotation = Quaternion.LookRotation(direction);
                model.CachedTransform.rotation = Quaternion.Lerp( model.CachedTransform.rotation, rotation, 5f * Time.deltaTime);
                model.SetSpeedFire(0);
                continue;
            }

            model.UpdateDelay(dt);

            if (model.IsAttack())
            {
                model.OnTargetReceiveDamage();
                var pos = model.CachedTransform.position;
                var startPos = new Vector3(pos.x + 0.15f, pos.y, pos.z);
                effectsController.ShowEffect("Shoot", startPos, model.TargetEnemy.CachedTransform.position);
                
                if (model.TargetEnemy.IsLife() == false)
                {
                    var keyEffect = GetEnemyEffectType(model.TargetEnemy.Type);
                    effectsController.ShowEffect(keyEffect, model.TargetEnemy.CachedTransform.position, Vector3.zero);
                    enemyController.HideEnemy(model.TargetEnemy);
                    Money += model.TargetEnemy.Money;
                    
                    var allMoney = PlayerPrefs.GetInt("Money");
                    allMoney += model.TargetEnemy.Money;
                    PlayerPrefs.SetInt("Money", allMoney);
                    
                    model.RemoveTargetEnemy();
                }
            }
        }
    }
}