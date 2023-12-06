using System;
using UnityEngine;
using TMPro;

public class TeammateView : MonoBehaviour
{
    [field: SerializeField] public TeammateType Type { get; private set; }
    [field: SerializeField] public Transform Character { get; private set; }
    [field: SerializeField] public Transform Mesh { get; private set; }
    [field: SerializeField] public Animator ActionAnim { get; private set; }
    [field: SerializeField] public TextMeshPro Text { get; private set; }
    [field: SerializeField] public GameObject Gun { get; private set; }
    [field: SerializeField] public float AttackSpeed { get; private set; }
    [field: SerializeField] public float AttackDamage { get; private set; }

    private void Start()
    {
        Text.text = $"{(int)Type}";
    }

    public void ChangeStateText(bool state)
    {
        Text.gameObject.SetActive(state);
    }

    public void ChangeStateGun(bool state)
    {
        Gun.gameObject.SetActive(state);
    }
}