namespace SAGE.Framework.Core.Network.LocalServer
{
    using System.Net.Sockets;
    using System.Text;
    using UnityEngine;

    public class TCPClient : MonoBehaviour
    {
        private TcpClient client;
        private NetworkStream stream;

        void Start()
        {
            string serverIP = "192.168.1.187";
            int port = 8080;
            client = new TcpClient(serverIP, port);
            stream = client.GetStream();
            Debug.Log("Connected to server!");
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                SendData("Hello from client!");
            }
        }

        private void SendData(string message)
        {
            byte[] data = Encoding.UTF8.GetBytes(message);
            stream.Write(data, 0, data.Length);
            Debug.Log("Sent: " + message);
        }

        void OnApplicationQuit()
        {
            stream.Close();
            client.Close();
        }
    }
}