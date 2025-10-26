using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandTracking : MonoBehaviour
{
    public UDPReceive udpReceive;
    public GameObject[] handPoints;

    void Start()
    {
        // Your Start code
    }

    void Update()
    {
        string data = udpReceive.data;

        // Check if data is valid
        if (string.IsNullOrEmpty(data) || data.Length < 2)
        {
            return; // Exit and wait for the next frame
        }

        data = data.Remove(0, 1);
        data = data.Remove(data.Length - 1, 1);

        string[] points = data.Split(',');

        // Check for enough data points (21 landmarks * 3 coordinates = 63)
        if (points.Length < 63)
        {
            return; // Not enough data yet, exit
        }

        // First, we need to find the center Z position to mirror around
        float centerZ = 0f;
        for (int i = 0; i < 21; i++)
        {
            centerZ += float.Parse(points[i * 3 + 2]);
        }
        centerZ = centerZ / 21f / 100f; // Average Z position

        for (int i = 0; i < 21; i++)
        {
            // Parse raw coordinates
            float x = float.Parse(points[i * 3]) / 100;
            float y = float.Parse(points[i * 3 + 1]) / 100;
            float z = float.Parse(points[i * 3 + 2]) / 100;

            // THE KEY FIX: Mirror Z around the center point
            // This flips the hand inside-out so palm becomes back and vice versa
            float mirroredZ = centerZ - (z - centerZ); // or: 2 * centerZ - z
            
            handPoints[i].transform.localPosition = new Vector3(
                12.8f - x,      // Mirror X axis (1280/100 = 12.8)
                7.2f - y,       // Flip Y axis (720/100 = 7.2)
                mirroredZ       // Mirror Z to flip palm/back
            );
        }
    }
}