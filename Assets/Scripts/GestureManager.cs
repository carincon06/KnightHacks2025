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
    public float shieldDistance = 0.5f; // Distance in front of hand

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

        // --- NEW LOGIC ---
        // We need to calculate the palm's "normal" vector (the direction it's facing).
        // We get three points on the palm: wrist, index base, pinky base.
        Vector3 palmBasePos = handTracker.handPoints[0].transform.position;
        Vector3 indexKnucklePos = handTracker.handPoints[5].transform.position;
        Vector3 pinkyKnucklePos = handTracker.handPoints[17].transform.position;

        // Create two vectors that lie flat on the palm
        Vector3 v1 = indexKnucklePos - palmBasePos; // Vector from wrist to index
        Vector3 v2 = pinkyKnucklePos - palmBasePos; // Vector from wrist to pinky

        // The Cross Product gives us a vector perpendicular to v1 and v2.
        // This is our new "handDirection" (the palm normal).
        Vector3 handDirection = Vector3.Cross(v2, v1).normalized;
        
        // ---
        // *** IMPORTANT ***
        // Because your HandTracking.cs mirrors the coordinates, the cross product
        // might point *into* your palm instead of *out*.
        //
        // If the shield still appears behind your hand, UNCOMMENT the line below:
        // handDirection = -handDirection;
        // ---

        if (handDirection.magnitude < 0.1f)
        {
            // Fallback if the hand is clenched or points are aligned
            handDirection = Camera.main.transform.forward;
        }

        // --- BETTER POSITIONING ---
        // Position the shield in front of the *center* of the palm (middle knuckle),
        // not the wrist (handPosition). This will feel more natural.
        Vector3 palmCenterPos = handTracker.handPoints[9].transform.position; // Middle base knuckle
        Vector3 shieldPosition = palmCenterPos + handDirection * shieldDistance;

        // If we don't have a shield, spawn one.
        if (activeShield == null)
        {
            activeShield = Instantiate(shieldPrefab, shieldPosition, Quaternion.LookRotation(handDirection));
            if (showDebugLogs)
                Debug.Log("Shield spawned!");
        }
        else
        {
            // If we already have a shield, update its position and rotation
            activeShield.transform.position = shieldPosition;
            activeShield.transform.rotation = Quaternion.LookRotation(handDirection);
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

        // 4. Check if spawn points are assigned
        if (fireballSpawnPoint == null || handPosition == null)
        {
            Debug.LogError("Fireball spawn points not assigned in Inspector!");
            return;
        }

        // 5. Calculate the "forward" direction of the hand (SAME AS RED DEBUG LINE)
        Vector3 palmBasePos = handTracker.handPoints[0].transform.position;
        Vector3 middleFingerTip = handTracker.handPoints[12].transform.position; // Middle fingertip
        Vector3 fireDirection = (middleFingerTip - palmBasePos).normalized;

        // If direction is too small (hand flat), use camera forward
        if (fireDirection.magnitude < 0.1f)
        {
            fireDirection = Camera.main.transform.forward;
            Debug.LogWarning("Hand direction unclear, using camera forward");
        }

        if (showDebugLogs)
        {
            Debug.Log($"Firing fireball! Direction: {fireDirection}");
            Debug.Log($"Spawn pos: {fireballSpawnPoint.position}, Speed: {fireballSpeed}");
        }
        
        // 6. Spawn the fireball at the middle knuckle (fireballSpawnPoint)
        Vector3 spawnPos = fireballSpawnPoint.position + fireDirection * 0.3f;
        GameObject fireball = Instantiate(fireballPrefab, 
                                  spawnPos, 
                                  Quaternion.LookRotation(fireDirection));

        // 7. Make sure Rigidbody exists and add velocity
        Rigidbody rb = fireball.GetComponent<Rigidbody>();
        if (rb != null)
        {
            // Disable gravity initially so it flies straight
            rb.useGravity = false;
            
            // Use velocity for predictable behavior
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
            // Draw the fire direction line (red) - THIS IS THE ACTUAL SHOOT DIRECTION
            Vector3 palmBasePos = handTracker.handPoints[0].transform.position;
            Vector3 middleFingerTip = handTracker.handPoints[12].transform.position; // Middle fingertip
            Vector3 fireDirection = (middleFingerTip - palmBasePos).normalized;
            
            // Red arrow showing shoot direction (from spawn point)
            Gizmos.color = Color.red;
            Gizmos.DrawLine(fireballSpawnPoint.position, fireballSpawnPoint.position + fireDirection * 2f);
            
            // Yellow sphere at spawn point
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(fireballSpawnPoint.position, 0.1f);
            
            // Cyan line from wrist to middle fingertip (reference line)
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(palmBasePos, middleFingerTip);
            
            // Green sphere at middle fingertip
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(middleFingerTip, 0.08f);

            // Magenta line showing shield position
            Gizmos.color = Color.magenta;
            Vector3 shieldPos = handPosition.position + fireDirection * shieldDistance;
            Gizmos.DrawLine(handPosition.position, shieldPos);
            Gizmos.DrawWireSphere(shieldPos, 0.15f);
        }
    }
}