using UnityEngine;

public class GameEffectsView : MonoBehaviour
{
    [field: SerializeField] public EffectType EffectType { get; private set; }
    [field: SerializeField] public float MoveSpeed{ get; private set; }
    [field: SerializeField] public float PlaybackTime { get; private set; }
}