using UnityEngine.UI;
using UnityEngine.AI;
using UnityEngine;
using TMPro;

public class EnemyView : MonoBehaviour
{
    [field: SerializeField] public EnemyType Type { get; private set; }
    [field: SerializeField] public NavMeshAgent Agent { get; private set; }
    [field: SerializeField] public Animator ActionAnim { get; private set; }
    [field: SerializeField] public SkinnedMeshRenderer Mesh { get; private set; }
    [field: SerializeField] public TextMeshProUGUI TextHP { get; private set; }
    [field: SerializeField] public Slider SliderHP { get; private set; }
    [field: SerializeField] public GameObject Canvas { get; private set; }
    [field: SerializeField] public int Reward { get; private set; }
    [field: SerializeField] public float HealthPoint { get; private set; }
    [field: SerializeField] public float Speed { get; private set; }
    [field: SerializeField] public float AttackDamage { get; private set; }

    public void VisualizeDamage(bool state)
    {
        Mesh.materials[0].SetFloat("_MCIALO", state ? 1f : 0f);
    }
    
    public void VisualizeHP(float HP)
    {
        ChangeStateCanvas(true);
        SliderHP.value = HP;
        TextHP.text = $"{(int)HP}";
    }

    public void ChangeStateCanvas(bool state)
    {
        Canvas.SetActive(state);
    }
}