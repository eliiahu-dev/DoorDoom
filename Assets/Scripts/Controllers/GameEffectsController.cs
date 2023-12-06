using OLS_HyperCasual;
using UnityEngine;

public class GameEffectsController : PoolableController<GameEffectsModel, GameEffectsView>
{
    public override bool HasUpdate => true;

    private ChangePlaceController changePlaceController;
    
    public void ShowEffect(string effectType, Vector3 startPosition, Vector3 endPosition)
    {
        var effect = GetFromPool(effectType);
        effect.ShowEffect(startPosition, endPosition);
    }
    
    private void ActionForType(GameEffectsModel model)
    {
        switch (model.Type)
        {
            case EffectType.Mate:
            {
                changePlaceController ??= BaseEntryPoint.Get<ChangePlaceController>();
                changePlaceController.CreateDefaultTeammate();
                    
                ShowEffect("Poof", model.CachedTransform.position, Vector3.zero);
                break;
            }
        }
    }
    
    public override void Update(float dt)
    {
        foreach (var kv in pooledModelsDict)
        {
            foreach (var kvModels in kv.Value)
            {
                var model = kvModels.Value;
                if (model.IsInPool)
                {
                    continue;
                }
                    
                if (model.IsAlive())
                {
                    model.UpdateLifeTime(dt);
                    continue;
                }

                ActionForType(model);
                ReturnToPool(model);
            }
        }
    }
}