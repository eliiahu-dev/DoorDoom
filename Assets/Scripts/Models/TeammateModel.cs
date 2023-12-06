using OLS_HyperCasual;
using UnityEngine;

public class TeammateModel : PoolableModel<TeammateView>
{
    public Transform CachedTransform { get; private set; }
    public EnemyModel TargetEnemy { get; private set; }
    public bool IsDrag { get; private set; }
    public bool InAttackPlace;

    private StatsControllerSO soStats;
    private float attackDelay;
    private float delay;

    private float attackSpeed;
    private float damage;
    
    public TeammateModel(TeammateView view)
    {
        View = view;
        CachedTransform = view.transform;
        
        var resourcesController = BaseEntryPoint.Get<ResourcesController>();
        soStats = resourcesController.GetResource<StatsControllerSO>(GameResourceConstants.StatsControllerSO, false);
    }

    public void ShowTeammate(bool state)
    {
        var indexDamage = PlayerPrefs.GetInt("Damage") - 1;
        var indexFireRate = PlayerPrefs.GetInt("FireRate") - 1;

        var addAttackSpeed = 0f;
        var addDamage = 0f;

        if (indexDamage >= 0)
        {
            addDamage = soStats.GetValue(StatsType.Damage).upgradeValue[indexDamage].Upgrate;
        }
        
        if (indexFireRate >= 0)
        {
            addAttackSpeed = soStats.GetValue(StatsType.FireRate).upgradeValue[indexFireRate].Upgrate;
        }
        
        CachedTransform.gameObject.SetActive(state);
        delay = 1 / (View.AttackSpeed + addAttackSpeed);
        attackSpeed = (View.AttackSpeed + addAttackSpeed) / 2;
        damage = View.AttackDamage + (View.AttackDamage * (addDamage / 100));
    }
    
    public void SetPlace(Vector3 place, BarrierModel barrier)
    {
        CachedTransform.position = place;
        View.Character.position = new Vector3(place.x, 0.06f, place.z);
        View.ChangeStateText(barrier == null);
        IsDrag = false;
        
        if (barrier != null)
        {
            var barrierIsLife = barrier.IsLife();
            View.ChangeStateGun(barrierIsLife);
            SetAnimation(barrierIsLife ? "isFire" : "isSit");
            InAttackPlace = barrierIsLife;
            SetSpeedFire(0);
            return;
        }
        
        SetAnimation("isIdle");
    }

    public void SetTargetEnemy(EnemyModel enemyModel)
    {
        TargetEnemy = enemyModel;
    }
    
    public bool IsAttack()
    {
        return attackDelay > delay;
    }
    
    public void UpdateDelay(float deltaTime)
    {
        attackDelay += deltaTime;
        
        var direction = TargetEnemy.CachedTransform.position - CachedTransform.position;
        var rotation = Quaternion.LookRotation(direction);
        CachedTransform.rotation = Quaternion.Lerp(CachedTransform.rotation, rotation, 5f * Time.deltaTime);
    }
    
    public void RemoveTargetEnemy()
    {
        TargetEnemy = null;
    }
    
    public void OnTargetReceiveDamage()
    {
        TargetEnemy.OnEnemyReceiveDamage(damage);
        SetSpeedFire(attackSpeed);
        attackDelay = 0;
    }
    
    public TeammateType GetTeammateType()
    {
        return View.Type;
    }

    public void SetLayer(int layer)
    {
        View.Mesh.gameObject.layer = layer;
    }
    
    public void VisualiseDrag()
    {
        var pos = CachedTransform.position;
        
        CachedTransform.rotation = new Quaternion(0f, 0f, 0f, 0f);
        View.Character.position = new Vector3(pos.x, pos.y + 0.5f, pos.z);
        InAttackPlace = false;
        IsDrag = true;

        View.ChangeStateGun(false);
        View.ChangeStateText(false);
        RemoveTargetEnemy();
        SetAnimation("isFly");
    }
    
    public void SetAnimation(string type)
    {
        View.ActionAnim.SetBool("isFly", false);
        View.ActionAnim.SetBool("isIdle", false);
        View.ActionAnim.SetBool("isFire", false);
        View.ActionAnim.SetBool("isSit", false);

        View.ActionAnim.SetBool(type, true);
    }
    
    public void SetSpeedFire(float value)
    {
        View.ActionAnim.SetFloat("SpeedFire", value);
    }
}