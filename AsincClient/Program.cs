using System;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            try
            {
                TcpClient client = new TcpClient();
                await client.ConnectAsync("127.0.0.1", 7000);
                Console.WriteLine("Client connected");
                await ProcessClientAsync(client);
            }
            catch (Exception e)
            {
                Console.WriteLine("An error occurred: " + e.Message);
            }
            Console.ReadLine();
        }

        static async Task ProcessClientAsync(TcpClient client)
        {
            using (client)
            {
                NetworkStream stream = client.GetStream();

                while (true)
                {
                    Console.Write("Enter your request (type 'exit' to quit): ");
                    string request = Console.ReadLine();
                    if (string.Equals(request, "exit", StringComparison.OrdinalIgnoreCase))
                    {
                        break;
                    }

                    byte[] requestBytes = Encoding.ASCII.GetBytes(request);
                    await stream.WriteAsync(requestBytes, 0, requestBytes.Length);

                    byte[] responseBytes = new byte[256];
                    int length = await stream.ReadAsync(responseBytes, 0, responseBytes.Length);
                    string response = Encoding.ASCII.GetString(responseBytes, 0, length);
                    Console.WriteLine("Server says: " + response);
                }
            }
            Console.WriteLine("Client disconnected");
        }
    }
}
