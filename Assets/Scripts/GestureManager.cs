using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This enum defines the states our hand can be in.
public enum Gesture { None, OpenHand, ClosedFist }

public class GestureManager : MonoBehaviour
{
    [Header("Game Objects")]
    public HandTracking handTracker; // Drag your HandTracking script here
    public GameObject fireballPrefab;
    public GameObject shieldPrefab;

    [Header("Spawn Points")]
    public Transform handPosition; // Drag your wrist (handPoints[0]) here
    public Transform fireballSpawnPoint; // Drag your middle knuckle (handPoints[9]) here

    [Header("Game Mechanics")]
    public float fireballSpeed = 20f;
    public float fireballCooldown = 0.75f;
    public float shieldCooldown = 2.0f;

    [Header("Debug")]
    public bool showDebugLogs = true;
    public bool showDebugGizmos = true;

    // --- Private Cooldown Timers ---
    private float timeToFire = 0f;
    private float timeToShield = 0f;
    private GameObject activeShield;

    // MediaPipe Hand Landmark indices:
    // FINGER TIPS: 8=Index, 12=Middle, 16=Ring, 20=Pinky
    // BASE KNUCKLES (MCP joints): 5=Index base, 9=Middle base, 13=Ring base, 17=Pinky base
    private int[] FINGER_TIPS = { 8, 12, 16, 20 };
    private int[] FINGER_BASE_KNUCKLES = { 5, 9, 13, 17 }; // Changed to base knuckles


    void Update()
    {
        // 1. Make sure the hand tracker is running
        if (handTracker == null || handTracker.handPoints == null || handTracker.handPoints.Length == 0)
        {
            if (showDebugLogs)
                Debug.LogWarning("Hand tracker not ready or no hand detected");
            return;
        }

        // 2. Detect the current gesture
        Gesture currentGesture = DetectGesture();

        if (showDebugLogs)
            Debug.Log($"Current Gesture: {currentGesture}");

        // 3. Act based on the gesture (This is your state machine!)
        switch (currentGesture)
        {
            case Gesture.OpenHand:
                HandleShield();
                break;

            case Gesture.ClosedFist:
                HandleFireball();
                break;

            case Gesture.None:
                // No gesture, so destroy any active shield
                if (activeShield != null)
                {
                    Destroy(activeShield);
                    activeShield = null;
                    timeToShield = Time.time + shieldCooldown; // Start cooldown
                }
                break;
        }
    }

    Gesture DetectGesture()
    {
        int straightFingers = 0;
        int bentFingers = 0;

        for (int i = 0; i < FINGER_TIPS.Length; i++)
        {
            // Compare fingertip to BASE knuckle (MCP joint)
            // This gives better detection for open vs closed hand
            float tipY = handTracker.handPoints[FINGER_TIPS[i]].transform.localPosition.y;
            float baseKnuckleY = handTracker.handPoints[FINGER_BASE_KNUCKLES[i]].transform.localPosition.y;

            // If tip is ABOVE base knuckle, finger is extended (open hand)
            if (tipY > baseKnuckleY)
            {
                straightFingers++;
            }
            // If tip is BELOW base knuckle, finger is curled (closed fist)
            else
            {
                bentFingers++;
            }
        }

        // --- GESTURE RULES ---
        
        // If 3 or more fingers are straight (tips above base), hand is OPEN
        if (straightFingers >= 3)
        {
            return Gesture.OpenHand;
        }

        // If 3 or more fingers are bent (tips below base), hand is CLOSED
        if (bentFingers >= 3)
        {
            return Gesture.ClosedFist;
        }

        return Gesture.None;
    }

    void HandleShield()
    {
        // Check if shield is on cooldown
        if (Time.time < timeToShield)
        {
            if (showDebugLogs)
                Debug.Log("Shield on cooldown!");
            return;
        }

        // If we don't have a shield, spawn one.
        if (activeShield == null)
        {
            // Spawn the shield at the hand's position
            activeShield = Instantiate(shieldPrefab, handPosition.position, Quaternion.identity);
            if (showDebugLogs)
                Debug.Log("Shield spawned!");
        }
        else
        {
            // If we already have a shield, just update its position
            activeShield.transform.position = handPosition.position;
        }
    }

    void HandleFireball()
    {
        // 1. Destroy any shield that's active
        if (activeShield != null)
        {
            Destroy(activeShield);
            activeShield = null;
            timeToShield = Time.time + shieldCooldown;
        }

        // 2. Check if we can fire (cooldown)
        if (Time.time < timeToFire)
        {
            if (showDebugLogs)
                Debug.Log("Fireball on cooldown!");
            return; // Can't fire yet
        }

        // 3. We can fire! Reset cooldown
        timeToFire = Time.time + fireballCooldown;

        // 4. CRITICAL FIX: Check if spawn points are assigned
        if (fireballSpawnPoint == null || handPosition == null)
        {
            Debug.LogError("Fireball spawn points not assigned in Inspector!");
            return;
        }

        // 5. Calculate the "forward" direction of the hand
        Vector3 palmBasePos = handTracker.handPoints[0].transform.position;
        Vector3 middleFingerPos = fireballSpawnPoint.position;
        Vector3 fireDirection = (middleFingerPos - palmBasePos).normalized;

        // CRITICAL FIX: If direction is too small (hand flat), use camera forward
        if (fireDirection.magnitude < 0.1f)
        {
            fireDirection = Camera.main.transform.forward;
            Debug.LogWarning("Hand direction unclear, using camera forward");
        }

        if (showDebugLogs)
        {
            Debug.Log($"Firing fireball! Direction: {fireDirection}");
            Debug.Log($"Spawn pos: {middleFingerPos}, Speed: {fireballSpeed}");
        }
        
        // 6. Spawn the fireball SLIGHTLY IN FRONT of the hand to avoid immediate collision
        Vector3 spawnPos = middleFingerPos + fireDirection * 0.3f;
        GameObject fireball = Instantiate(fireballPrefab, 
                                  spawnPos, 
                                  Quaternion.LookRotation(fireDirection));

        // 7. CRITICAL FIX: Make sure Rigidbody exists and add velocity
        Rigidbody rb = fireball.GetComponent<Rigidbody>();
        if (rb != null)
        {
            // Disable gravity initially so it flies straight
            rb.useGravity = false;
            
            // Use velocity instead of AddForce for more predictable behavior
            rb.linearVelocity = fireDirection * fireballSpeed;
            
            if (showDebugLogs)
                Debug.Log($"Fireball velocity set to: {rb.linearVelocity}");
        }
        else
        {
            Debug.LogError("Fireball prefab is missing Rigidbody component!");
        }
    }

    // Draw debug lines in Scene view
    void OnDrawGizmos()
    {
        if (!showDebugGizmos || handTracker == null || handTracker.handPoints == null)
            return;

        if (handTracker.handPoints.Length > 12 && fireballSpawnPoint != null && handPosition != null)
        {
            // Draw the fire direction line (red)
            Vector3 palmBasePos = handTracker.handPoints[0].transform.position;
            Vector3 middleFingerTip = handTracker.handPoints[12].transform.position; // Middle fingertip
            Vector3 fireDirection = (middleFingerTip - palmBasePos).normalized;
            
            // Red arrow showing shoot direction
            Gizmos.color = Color.red;
            Gizmos.DrawLine(fireballSpawnPoint.position, fireballSpawnPoint.position + fireDirection * 2f);
            
            // Yellow sphere at spawn point
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(fireballSpawnPoint.position, 0.1f);
            
            // NEW: Cyan line from wrist to middle fingertip (reference line)
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(palmBasePos, middleFingerTip);
            
            // Green sphere at middle fingertip
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(middleFingerTip, 0.08f);
        }
    }
}