using UnityEngine;

public class LookAtHPView : MonoBehaviour
{
    [SerializeField] private Transform Character;
    
    void Update()
    {
        var pos = Character.position;
        transform.LookAt(new Vector3(pos.x, pos.y + 5f, pos.z - 2f));
    }
}
