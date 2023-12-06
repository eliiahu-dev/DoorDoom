using System.Collections;
using UnityEngine;

public class CoroutineView : MonoBehaviour
{
    public void AddCoroutine(IEnumerator enumerator)
    {
        StartCoroutine(enumerator);
    }
}