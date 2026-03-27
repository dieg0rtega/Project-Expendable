using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerRespawn : MonoBehaviour
{
    [SerializeField] private Vector2 _respawnPoint = Vector2.zero;
    [SerializeField] private float _respawnDelay = 1f;

    private PlayerMovement _controller;
    private Rigidbody2D _rb;
    private bool _isDead;

    private void Awake()
    {
        _controller = GetComponent<PlayerMovement>();
        _rb = GetComponent<Rigidbody2D>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (_isDead) return;

        if (other.CompareTag("Hazard"))
        {
            StartCoroutine(Respawn());
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (_isDead) return;

        if (other.collider.CompareTag("Hazard"))
        {
            StartCoroutine(Respawn());
        }
    }

    private IEnumerator Respawn()
    {
        _isDead = true;

        _rb.velocity = Vector2.zero;
        _controller.enabled = false;

        
        GetComponent<SpriteRenderer>().enabled = false;

        yield return new WaitForSeconds(_respawnDelay);

        transform.position = _respawnPoint;
        _rb.velocity = Vector2.zero;
        _controller.enabled = true;

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        GetComponent<SpriteRenderer>().enabled = true;

        _isDead = false;
    }
}
