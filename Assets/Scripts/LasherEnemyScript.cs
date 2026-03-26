using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LasherEnemyScript : MonoBehaviour
{
    [SerializeField] private float _visionRange = 5f;
    [SerializeField] private float _attackDelay = 1.5f;
    [SerializeField] private LayerMask _playerLayer;
    [SerializeField] private LayerMask _visionBlockingLayers;
    [SerializeField] private GameObject _hitbox;
    [SerializeField] private Vector2 _hitboxExtendedPos;
    [SerializeField] private Vector2 _hitboxRetractedPos;

    private float _playerSeenTimer = 0f;
    private bool _isAttacking = false;
    private GameObject _player;

    private void Awake()
    {
        _player = GameObject.FindWithTag("Player");
    }

    private void Update()
    {
        if (CanSeePlayer())
        {
            _playerSeenTimer += Time.deltaTime;

            if (_playerSeenTimer >= _attackDelay && !_isAttacking)
                ExtendHitbox();
        }
        else
        {
            _playerSeenTimer = 0f;
            RetractHitbox();
        }
    }

    private bool CanSeePlayer()
    {
        Vector2 directionToPlayer = (_player.transform.position - transform.position).normalized;

        RaycastHit2D hit = Physics2D.Raycast(
            transform.position,
            directionToPlayer,
            _visionRange,
            _playerLayer | _visionBlockingLayers
        );

        return hit.collider != null && hit.collider.CompareTag("Player");
    }

    private void ExtendHitbox()
    {
        _isAttacking = true;
        _hitbox.transform.localPosition = _hitboxExtendedPos;
    }

    private void RetractHitbox()
    {
        _isAttacking = false;
        _hitbox.transform.localPosition = _hitboxRetractedPos;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, _visionRange);

        if (_player != null)
        {
            Gizmos.color = CanSeePlayer() ? Color.red : Color.white;
            Gizmos.DrawLine(transform.position, _player.transform.position);
        }
    }
}
