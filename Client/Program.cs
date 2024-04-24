using System;
using System.Diagnostics;
using System.Net.Sockets;
using System.Text;

namespace Client
{
    internal class Program
    {
        static void Main(string[] args)
        {
            try
            {
                TcpClient client = new TcpClient("127.0.0.1", 7000);
                Console.WriteLine("Client connected");
                NetworkStream stream = client.GetStream();


                for (int i = 0; i < 3; i++)
                {
                    Console.Write("Enter your request: ");
                    string request = Console.ReadLine();
                    if (request.Equals("exit", StringComparison.OrdinalIgnoreCase))
                    {
                        break;  
                    }
                    byte[] bytesWrite = Encoding.ASCII.GetBytes(request);
                    stream.Write(bytesWrite, 0, bytesWrite.Length);
                    Console.WriteLine("Client sent the request");

                    byte[] bytesRead = new byte[256];
                    int length = stream.Read(bytesRead, 0, bytesRead.Length);
                    string answer = Encoding.ASCII.GetString(bytesRead, 0, length);
                    Console.WriteLine("Server says: " + answer);
                }

                client.Close();
                Console.WriteLine("Client disconnected");

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            Console.ReadLine();
        }
    }
}
