using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutsceneTrigger : MonoBehaviour
{
    void Update()
    {
        // add button that will continue through dialogue until cutscene is done
        // when dialogue finished, proceed to EndCutscene()
        if (Input.GetKeyDown(KeyCode.Q)) // sample keycode test
        {
            EndCutscene();
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            StartCutscene();
        }
    }

    public void StartCutscene()
    {
        // add dialogue/flashback UI panel here

        Time.timeScale = 0f;
        Debug.Log("Cutscene started...");
    }

    public void EndCutscene()
    {
        // close dialogue/flashback UI panel here

        Time.timeScale = 1f;
        Debug.Log("Cutscene ended...");
    }
    
}