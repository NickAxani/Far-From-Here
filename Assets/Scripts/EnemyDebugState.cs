using UnityEditor.Animations;
using UnityEngine;

public class EnemyDebugState : MonoBehaviour
{
    public Transform player;

    void Update()
    {
        transform.LookAt(player);
    }
}
