using OLS_HyperCasual;
using UnityEngine;

public class PreLevelController : BaseMonoController<PreLevelView, PreLevelModel>
{ 
    public override PreLevelModel AddView(PreLevelView view)
    {
        var model = new PreLevelModel(view);
        modelsList.Add(model);
        return model;
    }
}
