using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;

namespace ChatServer
{
    class ClientObject
    {
        protected internal string Id { get; private set; }
        protected internal NetworkStream Stream { get; private set; }
        string userName;
        TcpClient client;
        ServerObject server; // объект сервера

        public ClientObject(TcpClient tcpClient, ServerObject serverObject)
        {
            Id = Guid.NewGuid().ToString();
            client = tcpClient;
            server = serverObject;
            serverObject.AddConnection(this);
        }

        public void Process()
        {
            try
            {
                Stream = client.GetStream();
                // получаем имя пользователя
                string message = GetMessage();
                int number;
                userName = message;

                message = userName + " entered the chat";
                // посылаем сообщение о входе в чат всем подключенным пользователям
                server.BroadcastMessage(message, this.Id);
                Console.WriteLine($"SERVER:{message}");
                // в бесконечном цикле получаем сообщения от клиента
                while (true)
                {
                    try
                    {
                        number = GetMessageINT();
                        if (number>=0 && number<=10)
                        {
                            if(number==server.value)
                            {
                                Console.WriteLine($"SERVER:{userName} : {number} is winning number ");
                                server.BroadcastMessageAll($"{userName} : {number} is winning number \n Starting new round", this.Id);
                                server.GenerateRandomValue();
                            }
                            else if(number < server.value)
                            {
                                Console.WriteLine($"SERVER:{userName} : {number} is less winning number");
                                server.BroadcastMessageAll($"{userName} : {number} is less winning number", this.Id);
                            }
                            else if (number > server.value)
                            {
                                Console.WriteLine($"SERVER:{userName} : {number} is more winning number");
                                server.BroadcastMessageAll($"{userName} : {number} is more winning number", this.Id);
                            }

                        }
                        else
                        {
                            server.MessageToUser("You entered the wrong format. You need to enter a number from 0 to 10 without spaces and other symbols. Example:8", this.Id);
                        }
                        
                    }
                    catch
                    {
                        message = String.Format("{0}: left chat", userName);
                        Console.WriteLine($"SERVER:{message}");
                        server.BroadcastMessage(message, this.Id);
                        break;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            finally
            {
                // в случае выхода из цикла закрываем ресурсы
                server.RemoveConnection(this.Id);
                Close();
            }
        }

        // чтение входящего сообщения и преобразование в строку
        private string GetMessage()
        {
            byte[] data = new byte[64]; // буфер для получаемых данных
            StringBuilder builder = new StringBuilder();
            int bytes = 0;
            do
            {
                bytes = Stream.Read(data, 0, data.Length);
                builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
            }
            while (Stream.DataAvailable);

            return builder.ToString();
        }
        private int GetMessageINT()
        {
            byte[] data = new byte[64]; // буфер для получаемых данных
            StringBuilder builder = new StringBuilder();
            int bytes = 0;
            do
            {
                bytes = Stream.Read(data, 0, data.Length);
                builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
            }
            while (Stream.DataAvailable);
            try
            {
                return Convert.ToInt32(builder.ToString());
            }
            catch(Exception ex)
            {
                Console.WriteLine($"Server:{userName}: " + ex.Message);
                return -1;
            }
            
        }

        // закрытие подключения
        protected internal void Close()
        {
            if (Stream != null)
                Stream.Close();
            if (client != null)
                client.Close();
        }
    }
}
