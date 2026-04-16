using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collectible : MonoBehaviour
{
    public List<DialogueLine> dialogueLines;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            CollectibleUI ui = FindObjectOfType<CollectibleUI>();

            if (ui != null)
            {
                ui.Show(dialogueLines);
            }

            
       
        }
        }
    

}