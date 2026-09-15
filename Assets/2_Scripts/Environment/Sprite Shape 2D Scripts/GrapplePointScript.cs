using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrapplePoint : MonoBehaviour
{
    [SerializeField] private float _detectionRange = 20f;
    [SerializeField] private LayerMask _playerLayer;

    public bool IsPlayerInRange(Vector2 playerPos)
    {
        return Vector2.Distance(transform.position, playerPos) <= _detectionRange;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, _detectionRange);
    }
}
