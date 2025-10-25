using UnityEngine;

public class FaceCamera : MonoBehaviour
{
    private Transform mainCameraTransform;
    
    [Header("Shield Settings")]
    public bool billboardEffect = true; // Always face camera
    public Vector3 scaleMultiplier = Vector3.one; // Adjust shield size

    void Start()
    {
        // Find the main camera once at the start
        mainCameraTransform = Camera.main.transform;
        
        // Apply scale if needed
        transform.localScale = Vector3.Scale(transform.localScale, scaleMultiplier);
    }

    void LateUpdate() // Use LateUpdate to ensure it happens after all position updates
    {
        if (mainCameraTransform == null)
        {
            mainCameraTransform = Camera.main.transform;
            return;
        }

        if (billboardEffect)
        {
            // Make the shield face the camera
            // This ensures the front of the shield is always visible
            transform.rotation = Quaternion.LookRotation(transform.position - mainCameraTransform.position);
        }
    }
}