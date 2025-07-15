using System;
using System.Threading.Tasks;

namespace SAGE.Framework.Core.Network.LocalServer
{
    using System.Net;
    using System.Net.Sockets;
    using System.Threading;
    using UnityEngine;

    public class TCPServer : MonoBehaviour
    {
        private TcpListener server;
        private volatile bool isRunning = false; // Use "volatile" for thread safety

        void Start()
        {
            int port = 8080;
            server = new TcpListener(IPAddress.Any, port);
            server.Start();
            isRunning = true;
            Debug.Log("Server started on 127.0.0.1:" + port);
            new Thread(ListenForClientsAsync).Start();
        }

        private async void ListenForClientsAsync()
        {
            try
            {
                while (isRunning)
                {
                    // Use async polling instead of blocking indefinitely
                    TcpClient client = await server.AcceptTcpClientAsync();
                    Debug.Log("Client connected!");
                    HandleClientAsync(client);
                }
            }
            catch (SocketException ex)
            {
                if (isRunning) Debug.LogError("Server error: " + ex.Message);
            }
        }

        private async void HandleClientAsync(TcpClient client)
        {
            try
            {
                NetworkStream stream = client.GetStream();
                byte[] buffer = new byte[1024];
                while (isRunning)
                {
                    if (stream.DataAvailable)
                    {
                        int bytesRead = stream.Read(buffer, 0, buffer.Length);
                        string data = System.Text.Encoding.UTF8.GetString(buffer, 0, bytesRead);
                        Debug.Log("Received: " + data);
                    }

                    await Task.Delay(100); // Poll every 100ms
                }
            }
            catch (Exception ex)
            {
                Debug.LogError("Client error: " + ex.Message);
            }
            finally
            {
                client.Close();
            }
        }

        void OnApplicationQuit()
        {
            isRunning = false; // Signal threads to exit
            if (server != null)
            {
                server.Stop(); // This will unblock AcceptTcpClient()
                Debug.Log("Server stopped.");
            }
        }
    }
}