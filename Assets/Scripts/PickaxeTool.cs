using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickaxeHook : MonoBehaviour
{
    [Header("Hook Settings")]
    public float hookRange = 1.5f;         
    public float boostForce = 8f;          
    public float wallCheckDistance = 0.6f; 
    public float ledgeClearHeight = 0.5f;  
    public LayerMask wallLayer;

    [Header("References")]
    public Transform playerCenter;

    private Rigidbody2D rb;
    private bool isHooked = false;
    private float hookCooldown = 0f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (hookCooldown > 0) hookCooldown -= Time.deltaTime;

        if (Input.GetButtonDown("Fire1") && hookCooldown <= 0)
        {
            TryHook();
        }

        if (isHooked && rb.velocity.y < -0.1f)
        {
            isHooked = false;
        }
    }

    void TryHook()
    {
        // Determine direction based on player facing or movement
        Vector2[] directions = { Vector2.right, Vector2.left };

        foreach (Vector2 dir in directions)
        {
            RaycastHit2D hit = Physics2D.Raycast(playerCenter.position, dir, hookRange, wallLayer);

            if (hit.collider != null)
            {
                // Check if there's a ledge
                Vector2 ledgeCheckOrigin = hit.point + Vector2.up * ledgeClearHeight;
                RaycastHit2D ledgeClear = Physics2D.Raycast(ledgeCheckOrigin, dir, wallCheckDistance, wallLayer);

                if (ledgeClear.collider == null) 
                {
                    Hook(dir);
                    return;
                }
            }
        }
    }

    void Hook(Vector2 wallDirection)
    {
        isHooked = true;
        hookCooldown = 0.5f;

        rb.velocity = new Vector2(rb.velocity.x, 0f);
        rb.AddForce(new Vector2(-wallDirection.x * 2f, boostForce), ForceMode2D.Impulse);
    }

    void OnDrawGizmosSelected()
    {
        if (playerCenter == null) return;
        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(playerCenter.position, playerCenter.position + Vector3.right * hookRange);
        Gizmos.DrawLine(playerCenter.position, playerCenter.position + Vector3.left * hookRange);
    }
}