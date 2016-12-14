using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace GameServer
{
    public static class TCPClientExtension
    {
        public static void WriteMessage(this TcpClient client, string message)
        {
            lock (client)
            {
                if (!client.Connected)
                    return;

                StreamWriter writer = new StreamWriter(client.GetStream(),
                                           Encoding.UTF8);
                writer.Write(message);
                //Write Prompt;

                writer.Flush();
            }
        }
        public static  string ReadCommand(this TcpClient client)
        {
            byte[] message = new byte[4096];
            var encoder = Encoding.UTF8;
            int bytesRead;
            string finalMessage = "";
            var stream = client.GetStream();
           
            while (!finalMessage.EndsWith("\r\n"))
            {
                bytesRead = 0;

                //try reading from the client stream
                try
                {
                    bytesRead =stream.Read(message, 0, 4096);
                    finalMessage += encoder.GetString(message, 0, bytesRead);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
            }

            if (finalMessage.Contains("\b"))
            {
                do
                {
                    if (finalMessage.IndexOf("\b") == 0)
                    {
                        finalMessage = finalMessage.Remove(finalMessage.IndexOf("\b"), 1);
                    }
                    else
                    {
                        finalMessage = finalMessage.Remove(finalMessage.IndexOf("\b") - 1, 2);
                    }
                } while (finalMessage.Contains("\b"));

            }
          
            return finalMessage.TrimEnd('\r', '\n');
        }
    }
}
