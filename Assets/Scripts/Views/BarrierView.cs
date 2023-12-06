using UnityEngine;

public class BarrierView : MonoBehaviour
{
    [field: SerializeField] public Collider AttackZone { get; private set; }
    [field: SerializeField] public float HealthPoint { get; private set; }
}