using OLS_HyperCasual;
using UnityEngine;

public class GameEffectsModel : PoolableModel<GameEffectsView>
{
    public Transform CachedTransform { get; private set; }
    public EffectType Type { get; private set; }

    private Vector3 endPos;
    private float playbackTime;
    private float lifeTime;
    
    public GameEffectsModel(GameEffectsView view)
    {
        View = view;
        Type = view.EffectType;
        playbackTime = view.PlaybackTime;
        CachedTransform = view.transform;
    }

    public void ShowEffect(Vector3 startPosition, Vector3 endPosition)
    {
        endPos = endPosition;
        CachedTransform.position = startPosition;
        CachedTransform.gameObject.SetActive(true);
        lifeTime = 0;
    }
        
    public bool IsAlive()
    {
        return lifeTime < playbackTime;
    }

    public void UpdateLifeTime(float deltaTime)
    {
        lifeTime += deltaTime;

        if (Type is EffectType.Mate or EffectType.Shoot)
        {
            CachedTransform.position = Vector3.MoveTowards(
                CachedTransform.position, 
                endPos, 
                View.MoveSpeed * deltaTime);
        }
        
        if (lifeTime >= playbackTime)
        {
            View.gameObject.SetActive(false);
        }
    }
}