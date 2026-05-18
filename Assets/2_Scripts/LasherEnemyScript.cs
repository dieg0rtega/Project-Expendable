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
    [SerializeField] private GameObject _TileMesh;
    [SerializeField] private Vector2 _facingDirection = Vector2.right;
    [SerializeField] private float _detectionAngle = 20f;

    private Animator _hitboxAnimator;
    private float _playerSeenTimer = 0f;
    private bool _isAttacking = false;
    private GameObject _player;
    private Vector3 _hitboxOriginalLocalPos;
    private Vector3 _hitboxOriginalLocalScale;
    private Vector3 _hitboxOriginalWorldPos;
    private SpriteRenderer _hitboxRenderer;
    private BoxCollider2D _hitboxCollider;
    private bool _hasDetectedPlayer = false;

    //SFX
    AudioManager audioManager;

    private void Awake()
    {
        _player = GameObject.FindWithTag("Player");
         audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();
        _hitboxOriginalWorldPos = _hitbox.transform.position;
        _hitboxOriginalLocalPos = _hitbox.transform.localPosition;
        _hitboxOriginalLocalScale = _hitbox.transform.localScale;
        _hitboxCollider = _hitbox.GetComponent<BoxCollider2D>();
        _hitboxRenderer = _TileMesh.GetComponent<SpriteRenderer>();
        _hitboxAnimator = _TileMesh.GetComponent<Animator>();
        _hitboxAnimator.enabled = false;

    }

    private void Update()
    {
        bool canSee = CanSeePlayer();

        if (canSee && !_hasDetectedPlayer)
        {
            _hasDetectedPlayer = true;
            audioManager.PlaySFX(audioManager.lasherDetection);
            Debug.Log("Detect Trigger");
        }

        if (!canSee)
        {
            _hasDetectedPlayer = false;
            _playerSeenTimer = 0f;
            return;
        }

        _playerSeenTimer += Time.deltaTime;

        if (_playerSeenTimer >= _attackDelay && !_isAttacking)
        {
            StartCoroutine(TentacleAttack());
            StartCoroutine(PlayStrikeSoundDelayed()); 
        }
    }


    IEnumerator AttackWithDelay()
    {
        _isAttacking = true;

        // Wait until detection sound finishes
        while (audioManager.IsPlaying())
            yield return null;

        audioManager.PlaySFX(audioManager.lasherStrike);

        yield return StartCoroutine(TentacleAttack());
    }

    IEnumerator PlayStrikeSoundDelayed()
    {
        yield return new WaitForSeconds(0.1f); // tweak this
        audioManager.PlaySFX(audioManager.lasherStrike);
    }


    private IEnumerator TentacleAttack()
    {
        _isAttacking = true;
        _hitboxAnimator.enabled = true;
        _hitboxAnimator.SetTrigger("Attack");

        Vector2 worldDir = (_player.transform.position - transform.position).normalized;
        float angle = Mathf.Atan2(worldDir.y, worldDir.x) * Mathf.Rad2Deg;
        _hitbox.transform.rotation = Quaternion.Euler(0f, 0f, angle);
        _hitbox.transform.position = transform.position;

        // Extend
        float elapsed = 0f;
        float duration = _maxExtendDistance / _extendSpeed;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            float currentSize = Mathf.Lerp(0f, _maxExtendDistance, t);
            _hitboxRenderer.size = new Vector2(currentSize, _hitboxRenderer.size.y);
            _hitboxCollider.size = new Vector2(currentSize, _hitboxCollider.size.y);
            _hitboxCollider.offset = new Vector2(currentSize / 2f, 0f);
            yield return null;
        }

        yield return new WaitForSeconds(_holdDuration);

        // Retract
        _hitboxAnimator.SetTrigger("Retract");

        elapsed = 0f;
        duration = _maxExtendDistance / _retractSpeed;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            float currentSize = Mathf.Lerp(_maxExtendDistance, 0f, t);
            _hitboxRenderer.size = new Vector2(currentSize, _hitboxRenderer.size.y);
            _hitboxCollider.size = new Vector2(currentSize, _hitboxCollider.size.y);
            _hitboxCollider.offset = new Vector2(currentSize / 2f, 0f);
            yield return null;
        }

        // Reset
        _hitbox.transform.localPosition = _hitboxOriginalLocalPos;
        _hitbox.transform.localScale = _hitboxOriginalLocalScale;
        _hitbox.transform.localRotation = Quaternion.identity;
        _hitboxRenderer.size = new Vector2(0f, _hitboxRenderer.size.y);
        _hitboxCollider.size = new Vector2(0f, _hitboxCollider.size.y);
        _hitboxCollider.offset = Vector2.zero;
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
            if (hit.collider.gameObject == _player)

                return true;

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