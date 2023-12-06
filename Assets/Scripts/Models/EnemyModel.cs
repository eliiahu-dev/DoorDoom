using OLS_HyperCasual;
using UnityEngine;

public class EnemyModel : PoolableModel<EnemyView>
{
    public EnemyType Type { get; private set; }
    public BarrierModel Barrier { get; private set; }
    public Transform CachedTransform { get; private set; }
    public int Money { get; private set; }
    public float ReactActiveDelay;
    public bool React;
    
    private static float reactActiveKD = 2f;
    private static float firstHitFix = 1.7f;
    private static float emissionKD = 0.15f;
    private static float reactKD = 0.8f;
    
    private float emissionDelay;
    private float attackDelay;
    private float reactDelay;
    private float damage;
    private float delay;
    private float HP;
    
    public EnemyModel(EnemyView view)
    {
        View = view;
        view.Agent.speed = view.Speed;
        CachedTransform = view.transform;
        damage = view.AttackDamage;
        Money = view.Reward;
        delay = GetClipLength("Hit") / 2;
        Type = view.Type;
        view.SliderHP.maxValue = view.HealthPoint;
    }
    
    public bool IsLife()
    {
        return HP > 0;
    }
    
    public void ShowEnemy(bool state)
    {
        if (state)
        {
            attackDelay = 0;
            HP = View.HealthPoint;
            View.ChangeStateCanvas(false);
            View.VisualizeDamage(false);
            CachedTransform.rotation = new Quaternion(0f, 180f, 0f, 0f);
        }
        
        CachedTransform.gameObject.SetActive(state);
    }

    public void SetPlace(Vector3 place)
    {
        CachedTransform.position = place;
    }
    
    public void SetTarget(BarrierModel barrierModel)
    {
        if (IsLife() == false)
        {
            return;
        }
        
        attackDelay = delay / firstHitFix;
        Barrier = barrierModel;
        SetDestination(Barrier.CachedTransform.position);
        SetAnimation(Barrier.IsInsideZone(CachedTransform.position) ? "isHit" : "isWalk");
    }

    public bool InZoneAttack()
    {
        var inZone = Barrier.IsInsideZone(CachedTransform.position);
        
        if (inZone && IsLife())
        {
            SetDestination(CachedTransform.position);
            SetAnimation("isHit");
        }
        
        return inZone;
    }

    public bool IsAttack()
    {
        return attackDelay > delay;
    }

    public void UpdateEmissionDelay(float deltaTime)
    {
        emissionDelay += deltaTime;

        if (emissionDelay > emissionKD)
        {
            View.VisualizeDamage(false);
        }
    }
    
    public void UpdateReactDelay(float deltaTime)
    {
        reactDelay += deltaTime;

        if (reactDelay > reactKD)
        {
            reactDelay = 0;
            React = false;
            SetAnimation("isWalk");
            View.Agent.speed = View.Speed;
        }
    }
    
    public void UpdateDelay(float deltaTime)
    {
        attackDelay += deltaTime;
    }
    
    public void OnTargetReceiveDamage()
    {
        if (IsLife() == false)
        {
            return;
        }
        
        Barrier.OnBarrierReceiveDamage(damage);
        CachedTransform.rotation = new Quaternion(0f, 180f, 0f, 0f);
        attackDelay = 0;
    }

    public void OnEnemyReceiveDamage(float value)
    {
        if (ReactActiveDelay > reactActiveKD)
        {
            OnReact();
        }
        
        emissionDelay = 0;
        HP -= value;
        View.VisualizeDamage(true);
        View.VisualizeHP(HP);

        if (HP > 0)
        {
            return;
        }
        
        HP = 0;
    }

    private void OnReact()
    {
        React = true;
        SetAnimation("isReact");
        View.Agent.speed = 0;
        ReactActiveDelay = 0;
    }
    
    private void SetDestination(Vector3 position)
    {
        View.Agent.destination = position;
    }
    
    private void SetAnimation(string type)
    {
        View.ActionAnim.SetBool("isWalk", false);
        View.ActionAnim.SetBool("isHit", false);
        View.ActionAnim.SetBool("isReact", false);

        View.ActionAnim.SetBool(type, true);
    }
    
    private float GetClipLength(string clipName)
    {
        var clips = View.ActionAnim.runtimeAnimatorController.animationClips;
        
        foreach (var clip in clips)
        {
            if (clip.name == clipName)
            {
                return clip.length;
            }
        }
        return 0;
    }
}