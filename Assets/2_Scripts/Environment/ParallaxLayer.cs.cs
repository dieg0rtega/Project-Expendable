using Unity.VisualScripting;
using UnityEngine;

[ExecuteInEditMode]
public class ParallaxLayer : MonoBehaviour
{
    public float parallaxFactor;
    public bool followY = false;

    private void Update()
    {
        if (followY)
        {
            Vector3 newPos = transform.localPosition;
            newPos.y = GameObject.Find("Main Camera").transform.position.y;
            transform.localPosition = newPos;
        }
    }

    public void Move(float delta)
    {
        Vector3 newPos = transform.localPosition;
        newPos.x -= delta * parallaxFactor;

        transform.localPosition = newPos;
    }

}
