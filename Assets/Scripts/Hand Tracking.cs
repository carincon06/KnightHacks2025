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

        // --- FIX 1: CHECK IF DATA IS VALID ---
        // If data is null, empty, or too short to have brackets, 
        // stop executing this Update frame.
        if (string.IsNullOrEmpty(data) || data.Length < 2)
        {
            return; // Exit and wait for the next frame
        }

        data = data.Remove(0, 1);
        data = data.Remove(data.Length - 1, 1);
        //print(data);

        string[] points = data.Split(',');

        // --- FIX 2: CHECK FOR ENOUGH DATA POINTS ---
        // Your loop needs 21 * 3 = 63 data points.
        // If the array isn't that long, stop.
        if (points.Length < 63)
        {
            return; // Not enough data yet, exit
        }
        
        //print(points[0]);

        for (int i = 0; i < 21; i++)
        {
            float x = 7 - float.Parse(points[i * 3]) / 100;
            float y = float.Parse(points[i * 3 + 1]) / 100;
            float z = float.Parse(points[i * 3 + 2]) / 100;

            handPoints[i].transform.localPosition = new Vector3(x, y, z);
        }
    }
}