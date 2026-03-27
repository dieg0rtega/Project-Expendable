using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collectible : MonoBehaviour
{
    public string flavorText;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            try
        {
            Debug.Log(flavorText);
        } 
        catch (KeyNotFoundException)
        {
            Debug.Log("Text not found.");
        }
        }
    }

}