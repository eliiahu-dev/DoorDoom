using System.Collections.Generic;
using OLS_HyperCasual;
using UnityEngine;

public class EnemyController : PoolableController<EnemyModel, EnemyView>
{
    public override bool HasUpdate => true;
    public Vector3 MaxBoundsPos;
    public Vector3 MinBoundsPos;
    public bool StartGame;
    public bool EndGame;
    public bool LastWave;

    private const float posBehindDoorZ = 10f;
    private BarrierController barrierController;
    private GameEffectsController gameEffectsController;
    private LevelController levelController;
    private List<EnemyModel> modelList = new();
    private LevelControllerSO soLevels;
    private int waveIndex;
    private int level;

    public EnemyController()
    {
        level = PlayerPrefs.GetInt("Level");
        barrierController = BaseEntryPoint.Get<BarrierController>();
        gameEffectsController = BaseEntryPoint.Get<GameEffectsController>();
        
        var resourcesController = BaseEntryPoint.Get<ResourcesController>();
        soLevels = resourcesController.GetResource<LevelControllerSO>(GameResourceConstants.LevelControllerSO, false);
        
        var enemyTypes = soLevels.GetValue(level);
        var soGameSettings = resourcesController.GetResource<GameSettingsSO>(GameResourceConstants.GameSettingsSO, false);
        
        List<EnemyType> initEnemies = new();

        foreach (var enemyValue in enemyTypes)
        {
            var nextStep = false;
            
            foreach (var enemyType in initEnemies)
            {
                if (enemyValue.enemy == enemyType)
                {
                    nextStep = true;
                    break;
                }
            }

            if (nextStep)
            {
                continue;
            }
            
            initEnemies.Add(enemyValue.enemy);
        }

        foreach (var enemyType in initEnemies)
        {
            var name = GetEnemyName(enemyType);
            var path = GetResourcePath(name, soGameSettings);
            var prefab = resourcesController.GetResource<EnemyView>(path, false);
            PreInitPool(name, prefab);
            
            var deathPrefab = resourcesController.GetResource<GameEffectsView>(GetDeathEffectPath(enemyType), false);
            gameEffectsController.PreInitPool($"{enemyType}Death", deathPrefab);
        }
    }

    public void StartWaveEnemies()
    {
        if (LastWave)
        {
            return;
        }
        
        var wave = soLevels.GetValue(level)[waveIndex];
        
        for (var i = 0; i < wave.count; i++)
        {
            ShowEnemy(GetEnemyName(wave.enemy), GetRandomPosition());
        }
        
        waveIndex++;

        if (soLevels.GetValue(level).Length <= waveIndex)
        {
            LastWave = true;
        }
    }

    public EnemyModel GetNearestEnemy(Vector3 position)
    {
        EnemyModel nearestModel = null;
        var distance = float.PositiveInfinity;
        
        foreach (var model in modelList)
        {
            if (model.CachedTransform.position.z > posBehindDoorZ)
            {
                continue;
            }

            if (model.IsLife() == false)
            {
                continue;
            }
            
            var dist = Vector3.Distance(position, model.CachedTransform.position);
            
            if (dist > distance)
            {
                continue;
            }
            
            nearestModel = model;
            distance = dist;
        }
        
        return nearestModel;
    }

    private bool EnemyOutDoor()
    {
        foreach (var model in modelList)
        {
            if (model.IsLife() == false)
            {
                continue;
            }

            if (model.CachedTransform.position.z > posBehindDoorZ)
            {
                return true;
            }
        }
        
        return false;
    }
    
    private void ShowEnemy(string enemyType, Vector3 position)
    {
        var newModel = true;
        var enemy = GetFromPool(enemyType);
        
        enemy.ShowEnemy(true);
        enemy.SetPlace(position);
        TryUpdateNearestTarget(enemy);

        foreach (var model in modelList)
        {
            if (model == enemy)
            {
                newModel = false;
                break;
            }
        }

        if (newModel)
        {
            modelList.Add(enemy);
        }
        
        //levelController.OnOpenDoors(true);
    }

    public void HideEnemy(EnemyModel model)
    {
        ReturnToPool(model);
        model.ShowEnemy(false);
    }
    
    private Vector3 GetRandomPosition()
    {
        var x = Random.Range(MaxBoundsPos.x, MinBoundsPos.x);
        var z = Random.Range(MaxBoundsPos.z, MinBoundsPos.z);;
        
        return new Vector3(x, 0, z);
    }
    private string GetEnemyName(EnemyType type)
    {
        return type.ToString();
    }

    private void TryUpdateNearestTarget(EnemyModel model)
    {
        var targetModel = barrierController.GetNearestBarrier(model.CachedTransform);

        if (targetModel == null)
        {
            var levelController = BaseEntryPoint.Get<LevelController>();
            levelController.ShowLosePopUp();
            EndGame = true;
            return;
        }
        
        model.SetTarget(targetModel);
    }
    
    private string GetResourcePath(string key, GameSettingsSO soGameSettings)
    {
        var name = soGameSettings.GetValue(key);
        return $"Prefabs/Enemies/{name}";
    }
    
    private string GetDeathEffectPath(EnemyType type)
    {
        return $"Prefabs/Effects/Deaths/{type}DeathEffect";
    }

    public override void Update(float dt)
    {
        var allDead = true;
        
        foreach (var model in modelList)
        {
            model.ReactActiveDelay += dt;
            model.UpdateEmissionDelay(dt);
            
            if (model.IsLife())
            {
                allDead = false;
            }
            
            if (model.React)
            {
                model.UpdateReactDelay(dt);
                continue;
            }
            
            if (model.Barrier.IsLife() == false)
            {
                TryUpdateNearestTarget(model);
            }
            
            if (model.InZoneAttack())
            {
                model.UpdateDelay(dt);

                if (model.IsAttack())
                {
                    model.OnTargetReceiveDamage();
                }
            }
        }

        levelController ??= BaseEntryPoint.Get<LevelController>();
        levelController.OnOpenDoors(EnemyOutDoor());
        
        if (allDead == false)
        {
            return;
        }

        if (LastWave)
        {
            levelController.ShowWinPopUp();
            EndGame = true;
        }
    }
}
