using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelSwitch : MonoBehaviour
{
    // Get black screen image
    [SerializeField] private GameObject fadeImage;
    private Image imageComponent;

    // How long the fade happens and the alpha transparency it reaches
    [SerializeField] public float fadeDuration = 0.5f;
    [SerializeField] public float targetAlphaStart = 0.0f;
    [SerializeField] public float targetAlphaEnd = 1.0f;

    // Start is called before the first frame update
    void Start()
    {
        imageComponent = fadeImage.GetComponent<Image>();
        StartCoroutine(SceneController.instance.SetTransparency(imageComponent, targetAlphaEnd));
        StartCoroutine(SceneController.instance.LevelFade(imageComponent, targetAlphaStart, fadeDuration, Update));
    }

    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            StopAllCoroutines(); // Prevents multiple fades from overlapping
            StartCoroutine(SceneController.instance.LevelFade(imageComponent, targetAlphaEnd, fadeDuration, SceneController.instance.NextLevel)); // begin fade animation
        }
    }
}
