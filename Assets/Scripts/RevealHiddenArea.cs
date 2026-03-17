using System.Collections;
using System.Collections.Generic;
using UnityEditor.ShaderGraph;
using UnityEngine;

public class RevealHiddenArea : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;
    public float duration = 1.0f;
    public float targetAlpha = 0.3f; // the desired alpha (opacity) value from 0-1

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            StopAllCoroutines(); // Prevents multiple fades from overlapping
            StartCoroutine(FadeToAlpha(targetAlpha)); // begin fade animation
        }
    }

    IEnumerator FadeToAlpha(float target)
    {
        Color startColor = spriteRenderer.color;
        float startAlpha = startColor.a;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime; // over time, visit each alpha point between 0-1 and attach it to the sprite
            float newAlpha = Mathf.Lerp(startAlpha, target, elapsedTime / duration); // Lerp -> finds a value at a specific point between two endpoints on a straight line
            spriteRenderer.color = new Color(startColor.r, startColor.g, startColor.b, newAlpha);
            yield return null;
        }
        spriteRenderer.color = new Color(startColor.r, startColor.g, startColor.b, target); // assign the target alpha value after animation ends
    }

}

