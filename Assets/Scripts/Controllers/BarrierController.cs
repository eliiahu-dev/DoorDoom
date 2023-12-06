using OLS_HyperCasual;
using UnityEngine;

public class BarrierController : BaseMonoController<BarrierView, BarrierModel>
{
    private int attachmentIndex;
    
    public override BarrierModel AddView(BarrierView view)
    {
        var model = new BarrierModel(view);
        modelsList.Add(model);
        return model;
    }

    public BarrierModel GetNearestBarrier(Transform enemy)
    {
        BarrierModel nearestModel = null;
        var distance = float.PositiveInfinity;
        
        foreach (var model in modelsList)
        {
            if (model.IsLife() == false)
            {
                continue;
            }
            
            var dist = Vector3.Distance(enemy.position, model.CachedTransform.position);
            
            if (dist > distance)
            {
                continue;
            }
            
            nearestModel = model;
            distance = dist;
        }

        return nearestModel;
    }

    public BarrierModel GetBarrierModel()
    {
        var model = modelsList[attachmentIndex];
        attachmentIndex++;
        return model;
    }
}