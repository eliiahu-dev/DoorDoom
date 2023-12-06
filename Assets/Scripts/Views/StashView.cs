using System.Collections.Generic;
using OLS_HyperCasual;
using UnityEngine;

public class StashView : MonoBehaviour
{
    [field: SerializeField] public List<ChangePlaceView> StashViews { get; private set; }
    [field: SerializeField] public List<BarrierView> BarrierViews { get; private set; }
    
    private void Start()
    {
        var entry = BaseEntryPoint.GetInstance();
        
        foreach (var view in BarrierViews)
        {
            entry.SubscribeOnBaseControllersInit(() =>
            {
                entry.GetController<BarrierController>().AddView(view);
            });
        }
        
        foreach (var view in StashViews)
        {
            entry.SubscribeOnBaseControllersInit(() =>
            {
                entry.GetController<ChangePlaceController>().AddView(view);
            });
        }
    }
}