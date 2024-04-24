using System;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Threading;

namespace Laba2_oop
{
    internal class Program
    {
        static void Main(string[] args)
        {
            TcpListener serverSocket = new TcpListener(IPAddress.Any, 7000);
            Console.WriteLine(JsonConvert.SerializeObject(new { Message = "Server started" }));
            serverSocket.Start();

            try
            {
                while (true)
                {
                    TcpClient clientSocket = serverSocket.AcceptTcpClient();
                    Console.WriteLine(JsonConvert.SerializeObject(new { Message = "Client connected" }));

                    Thread clientThread = new Thread(new ParameterizedThreadStart(HandleClient));
                    clientThread.Start(clientSocket);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(JsonConvert.SerializeObject(new { Error = e.Message }));
            }
            finally
            {
                serverSocket.Stop();
                Console.WriteLine(JsonConvert.SerializeObject(new { Message = "Server stopped" }));
            }
        }

        private static void HandleClient(object client)
        {
            TcpClient clientSocket = (TcpClient)client;
            NetworkStream stream = clientSocket.GetStream();
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            try
            {
                while (clientSocket.Connected)
                {
                    byte[] bytes = new byte[256];
                    int length = stream.Read(bytes, 0, bytes.Length);
                    if (length == 0) break;

                    string request = Encoding.ASCII.GetString(bytes, 0, length);
                    Console.WriteLine(JsonConvert.SerializeObject(new { Request = request }));

                    if (request.Equals("exit", StringComparison.OrdinalIgnoreCase))
                    {
                        Console.WriteLine(JsonConvert.SerializeObject(new { Message = "Client requested to disconnect." }));
                        break;
                    }

                    string message = $"Length of your request: {request.Length}";
                    Console.WriteLine(JsonConvert.SerializeObject(new { Response = message }));
                    bytes = Encoding.ASCII.GetBytes(message);
                    stream.Write(bytes, 0, bytes.Length);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(JsonConvert.SerializeObject(new { Error = e.Message }));
            }
            finally
            {
                clientSocket.Close();
                stopwatch.Stop();
                Console.WriteLine(JsonConvert.SerializeObject(new { Message = "Client disconnected", TimeElapsed = stopwatch.Elapsed.TotalSeconds }));
            }
        }
    }
}