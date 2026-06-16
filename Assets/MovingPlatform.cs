using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    public float speed;         // speed of the platform
    public int startingPoint;   // starting index (position of the platform)
    public Transform[] points;  // an array of transform points (positions where the platform needs to move)

    private int i; // index of the array

    void Start()
    {
        transform.position = points[startingPoint].position; // setting the position to
                                                             // the position of one of the points uding index "startingPoint"
    }

    void Update()
    {
        // checking the distance of the platform and the point
        if (Vector2.Distance(transform.position, points[i].position) < 0.02f)
        {
            i++;
            if (i == points.Length)
            {
                i = 0;   
            }
        }

        // moving the platform to the point position with the index "i"
        transform.position = Vector2.MoveTowards(transform.position, points[i].position, speed * Time.deltaTime);

    }
}
