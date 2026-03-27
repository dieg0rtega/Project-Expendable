using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collectible : MonoBehaviour
{
    // create a hash map of collectibles
    // when player grabs an item, display the flavor text of the corresponding item
    // start by displaying sample text when player grabs the item
    // make the item disappear from the scene and add it to an "inventory"
    void Start()
    {
        
    }

    
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Debug.Log("Flavor text: you got a collectible!");

        }
    }
}