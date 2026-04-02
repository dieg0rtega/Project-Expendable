using System.Collections;
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
    [SerializeField] private Vector2 _facingDirection = Vector2.right;
    [SerializeField] private float _detectionAngle = 20f;

    private float _playerSeenTimer = 0f;
    private bool _isAttacking = false;
    private GameObject _player;
    private Vector3 _hitboxOriginalLocalPos;
    private Vector3 _hitboxOriginalLocalScale;
    private Vector3 _hitboxOriginalWorldPos;

    private void Awake()
    {
        _player = GameObject.FindWithTag("Player");
        _hitboxOriginalWorldPos = _hitbox.transform.position;
        _hitboxOriginalLocalPos = _hitbox.transform.localPosition;
        _hitboxOriginalLocalScale = _hitbox.transform.localScale;
    }

    private void Update()
    {
        bool canSee = CanSeePlayer();

        if (canSee)
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
        _isAttacking = true;

        Vector2 worldDir = (_player.transform.position - transform.position).normalized;

        float angle = Mathf.Atan2(worldDir.y, worldDir.x) * Mathf.Rad2Deg;
        _hitbox.transform.rotation = Quaternion.Euler(0f, 0f, angle);

        // Extend by scaling only
        float currentScale = 0f;
        while (currentScale < _maxExtendDistance)
        {
            currentScale = Mathf.MoveTowards(currentScale, _maxExtendDistance, _extendSpeed * Time.deltaTime);
            _hitbox.transform.localScale = new Vector3(currentScale, _hitboxOriginalLocalScale.y, 1f);
            _hitbox.transform.position = (Vector2)transform.position + worldDir * (currentScale / 2f);
            yield return null;
        }

        yield return new WaitForSeconds(_holdDuration);

        // Retract by scaling back down
        while (currentScale > 0f)
        {
            currentScale = Mathf.MoveTowards(currentScale, 0f, _retractSpeed * Time.deltaTime);
            _hitbox.transform.localScale = new Vector3(currentScale, _hitboxOriginalLocalScale.y, 1f);
            _hitbox.transform.position = (Vector2)transform.position + worldDir * (currentScale / 2f);
            yield return null;
        }

        // Reset
        _hitbox.transform.localPosition = _hitboxOriginalLocalPos;
        _hitbox.transform.localScale = _hitboxOriginalLocalScale;
        _hitbox.transform.localRotation = Quaternion.identity;

        _isAttacking = false;
    }

    private bool CanSeePlayer()
    {
        Vector2 directionToPlayer = (_player.transform.position - transform.position).normalized;

        float angle = Vector2.Angle(_facingDirection, directionToPlayer);
        if (angle > _detectionAngle) return false;

        Vector2 rayOrigin = (Vector2)transform.position + _facingDirection * 0.5f;

        RaycastHit2D[] hits = Physics2D.RaycastAll(rayOrigin, directionToPlayer, _visionRange);
        System.Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));

        foreach (RaycastHit2D hit in hits)
        {
            if (hit.collider.transform.IsChildOf(transform) || hit.collider.gameObject == gameObject) continue;
            if (hit.collider.name == "Edge2") continue;
            if (hit.collider.CompareTag("Player")) return true;
            return false;
        }

        return false;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, _visionRange);

        Vector3 leftBound = Quaternion.Euler(0, 0, _detectionAngle) * (Vector3)_facingDirection * _visionRange;
        Vector3 rightBound = Quaternion.Euler(0, 0, -_detectionAngle) * (Vector3)_facingDirection * _visionRange;
        Gizmos.color = Color.cyan;
        Gizmos.DrawRay(transform.position, leftBound);
        Gizmos.DrawRay(transform.position, rightBound);

        if (_player != null)
        {
            Gizmos.color = CanSeePlayer() ? Color.red : Color.white;
            Gizmos.DrawLine(transform.position, _player.transform.position);
        }
    }
}