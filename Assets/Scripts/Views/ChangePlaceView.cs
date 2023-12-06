using UnityEngine;
using TMPro;

public class ChangePlaceView : MonoBehaviour
{
    [field: SerializeField] public TextMeshPro TextType { get; private set; }
    [field: SerializeField] public bool Stash { get; private set; }
}