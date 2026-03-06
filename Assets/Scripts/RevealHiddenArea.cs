using System.Collections;
using System.Collections.Generic;
using UnityEditor.ShaderGraph;
using UnityEngine;

public class RevealHiddenArea : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Color c = spriteRenderer.color;
            // make the area transparent while Player makes contact with tunnel
            c.a = 0.3f;
            spriteRenderer.color = c;
        }
    }
}

