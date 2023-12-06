using OLS_HyperCasual;
using UnityEngine;
using System;

public class BarrierModel : BaseModel<BarrierView>
{
    public Transform CachedTransform { get; private set; }
    public Action OnDestroyBarrier;

    private Vector3 MaxBoundsPos;
    private Vector3 MinBoundsPos;
    private float HP;
    
    public BarrierModel(BarrierView view)
    {
        View = view;
        CachedTransform = view.transform;

        var bounds = view.AttackZone.bounds;
        MaxBoundsPos = bounds.max;
        MinBoundsPos = bounds.min;
        
        var resourcesController = BaseEntryPoint.Get<ResourcesController>();
        var soStats = resourcesController.GetResource<StatsControllerSO>(GameResourceConstants.StatsControllerSO, false);
        
        var indexHealthBarrier = PlayerPrefs.GetInt("HealthBarrier") - 1;
        var addHealthBarrier = 0f;

        if (indexHealthBarrier >= 0)
        {
            addHealthBarrier = soStats.GetValue(StatsType.HealthBarrier).upgradeValue[indexHealthBarrier].Upgrate;
        }
        
        HP = view.HealthPoint + addHealthBarrier;
    }

    public bool IsLife()
    {
        return HP > 0;
    }

    public bool IsInsideZone(Vector3 position)
    {
        return position.x >= MinBoundsPos.x && position.x <= MaxBoundsPos.x &&
               position.z >= MinBoundsPos.z && position.z <= MaxBoundsPos.z;
    }

    public void OnBarrierReceiveDamage(float value)
    {
        HP -= value;

        if (HP > 0)
        {
            return;
        }
        
        HP = 0;
        
        CachedTransform.gameObject.SetActive(false);
        OnDestroyBarrier?.Invoke();
        OnDestroyBarrier = null;
    }
}