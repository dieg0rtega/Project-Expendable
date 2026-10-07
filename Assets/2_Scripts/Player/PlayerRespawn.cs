using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerRespawn : MonoBehaviour
{
    [SerializeField] private Vector2 _respawnPoint = Vector2.zero;
    [SerializeField] private float _respawnDelay = 10f;

    private Animator _animator;
    private SpriteRenderer _renderer;
    private PlayerMovement _controller;
    private Rigidbody2D _rb;
    private bool _isDead;

    AudioManager audioManager;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _renderer =  GetComponent<SpriteRenderer>();
        _controller = GetComponent<PlayerMovement>();
        _rb = GetComponent<Rigidbody2D>();
        audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (_isDead) return;

        if (other.CompareTag("Hazard"))
        {
            StartCoroutine(Respawn());
            if (other.name.Contains("Spike"))
            {
                audioManager.PlaySFX(audioManager.spike);
            } else if (other.name.Contains("IcePool"))
            {
                audioManager.PlaySFX(audioManager.icePool);
            }

        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (_isDead) return;

        if (other.collider.CompareTag("Hazard"))
        {
            StartCoroutine(Respawn());
            if (other.collider.name.Contains("Spike"))
            {
                audioManager.PlaySFX(audioManager.spike);
            }
            else if (other.collider.name.Contains("IcePool"))
            {
                audioManager.PlaySFX(audioManager.icePool);
            }
        }
    }

    private IEnumerator Respawn()
    {
        _isDead = true;

        _rb.velocity = Vector2.zero;
        _rb.gravityScale = 0;
        _controller.enabled = false;
        
        _animator.SetBool("IsDead", _isDead);

        yield return new WaitForSeconds(_respawnDelay);
        
        _renderer.enabled = false;
        
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        transform.position = _respawnPoint;
        
        _isDead = false;
    }
}
