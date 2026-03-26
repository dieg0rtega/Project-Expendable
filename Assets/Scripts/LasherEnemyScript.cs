using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LasherEnemyScript : MonoBehaviour
{
    [SerializeField] private float _visionRange = 5f;
    [SerializeField] private float _attackDelay = 1.5f;
    [SerializeField] private float _extendSpeed = 5f;
    [SerializeField] private float _retractSpeed = 8f;
    [SerializeField] private float _maxExtendDistance = 3f;
    [SerializeField] private float _holdDuration = 0.5f;
    [SerializeField] private LayerMask _playerLayer;
    [SerializeField] private LayerMask _visionBlockingLayers;
    [SerializeField] private GameObject _hitbox;

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
                StartCoroutine(TentacleAttack());
        }
        else
        {
            _playerSeenTimer = 0f;
        }
    }

    private IEnumerator TentacleAttack()
    {
        Debug.Log("Attack started");
        _isAttacking = true;

        Vector2 worldDir = (_player.transform.position - transform.position).normalized;
        Vector2 localDir = transform.InverseTransformDirection(worldDir);

        Vector2 retractedPos = Vector2.zero;
        Vector2 extendedPos = localDir * _maxExtendDistance;

        // Rotate hitbox to face player
        float angle = Mathf.Atan2(worldDir.y, worldDir.x) * Mathf.Rad2Deg;
        _hitbox.transform.localRotation = Quaternion.Euler(0f, 0f, angle);

        // Extend outward
        while (Vector2.Distance(_hitbox.transform.localPosition, extendedPos) > 0.05f)
        {
            _hitbox.transform.localPosition = Vector2.MoveTowards(
                _hitbox.transform.localPosition,
                extendedPos,
                _extendSpeed * Time.deltaTime
            );
            float progress = Vector2.Distance(retractedPos, _hitbox.transform.localPosition) / _maxExtendDistance;
            _hitbox.transform.localScale = new Vector3(progress * _maxExtendDistance, 0.2f, 1f);
            yield return null;
        }

        // Hold at full extension
        yield return new WaitForSeconds(_holdDuration);

        // Retract back
        while (Vector2.Distance(_hitbox.transform.localPosition, retractedPos) > 0.05f)
        {
            _hitbox.transform.localPosition = Vector2.MoveTowards(
                _hitbox.transform.localPosition,
                retractedPos,
                _retractSpeed * Time.deltaTime
            );
            yield return null;
        }

        _hitbox.transform.localPosition = retractedPos;
        _isAttacking = false;
    }

    private bool CanSeePlayer()
    {
        Vector2 rayOrigin = new Vector2(transform.position.x, transform.position.y + 0.5f);
        Vector2 playerCenter = new Vector2(_player.transform.position.x, _player.transform.position.y + 0.5f);
        Vector2 directionToPlayer = (playerCenter - rayOrigin).normalized;
        float distanceToPlayer = Vector2.Distance(rayOrigin, playerCenter);

        RaycastHit2D[] hits = Physics2D.RaycastAll(rayOrigin, directionToPlayer, _visionRange);

        foreach (RaycastHit2D hit in hits)
        {
            // Skip anything that is part of this enemy
            if (hit.collider.transform.IsChildOf(transform) || hit.collider.gameObject == gameObject) continue;

            // Skip Edge2
            if (hit.collider.name == "Edge2") continue;

            // If we hit the player first we can see them
            if (hit.collider.CompareTag("Player")) return true;

            // Something else blocked vision
            return false;
        }

        return false;
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