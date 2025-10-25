using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

public class UDPReceive : MonoBehaviour
{
    Thread receiveThread;
    UdpClient client;
    public int port = 5052;
    public string data;

    void Start()
    {
        receiveThread = new Thread(new ThreadStart(ReceiveData));
        receiveThread.IsBackground = true;
        receiveThread.Start();
    }

    private void ReceiveData()
    {
        client = new UdpClient(port);
        
        while (true)
        {
            try
            {
                IPEndPoint anyIP = new IPEndPoint(IPAddress.Any, 0);
                byte[] dataByte = client.Receive(ref anyIP);
                data = Encoding.UTF8.GetString(dataByte);
            }
            catch (SocketException e)
            {
                // Socket was closed, thread is stopping
                Debug.Log("UDP Socket closed: " + e.Message);
                break;
            }
            catch (ThreadAbortException)
            {
                // Thread is being stopped, exit gracefully
                break;
            }
            catch (Exception e)
            {
                Debug.LogError("UDP Receive error: " + e.Message);
            }
        }
    }

    void OnDestroy()
    {
        // Clean shutdown
        if (receiveThread != null)
        {
            receiveThread.Abort();
        }
        
        if (client != null)
        {
            client.Close();
        }
    }
}