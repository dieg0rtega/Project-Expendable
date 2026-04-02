using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.UI;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneController : MonoBehaviour
{
    public static SceneController instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void NextLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1, LoadSceneMode.Single);
    }

    public void LoadLevel(string levelName)
    {
        SceneManager.LoadScene(levelName, LoadSceneMode.Single);
    }

    public IEnumerator LevelFade(Image fadeImage, float target, float duration, Action whenDone)
    {
        Color startColor = fadeImage.color;
        float startAlpha = startColor.a;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime; // over time, visit each alpha point between 0-1 and attach it to the sprite
            float newAlpha = Mathf.Lerp(startAlpha, target, elapsedTime / duration); // Lerp -> finds a value at a specific point between two endpoints on a straight line
            fadeImage.color = new Color(startColor.r, startColor.g, startColor.b, newAlpha);
            yield return null;
        }
        fadeImage.color = new Color(startColor.r, startColor.g, startColor.b, target); // assign the target alpha value after animation ends
        whenDone?.Invoke();
    }
}
