using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;

public class LasherObstacle : MonoBehaviour
{
    public bool isPlayerDetected;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerStay2D(Collider2D collider)
    {
        if (collider.CompareTag("Player"))
        {
            isPlayerDetected = true;
            Debug.Log("Player Detected");
        } else
        {
            isPlayerDetected = false;
            Debug.Log("Player Gone");
        }
    }
}
