using OLS_HyperCasual;

public class MonoEntryPoint : BaseEntryPoint
{
    protected override bool IsAllInited()
    {
        return true;
    }

    protected override void InitControllers()
    {
        AddController(new CoroutineController());
        AddController(new ResourcesController());
        AddController(new PreLevelController());
        AddController(new PoolController());
        AddController(new GameEffectsController());
        AddController(new TeammateController());
        AddController(new BarrierController());
        AddController(new EnemyController());
        AddController(new ChangePlaceController());
        AddController(new LevelController());
        
        base.InitControllers();
    }

    protected override void InitPostControllers()
    {
        base.InitPostControllers();
    }
}