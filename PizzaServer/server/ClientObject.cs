using PizzaServer.data;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;

namespace PizzaServer.server
{
    class ClientObject
    {
        public TcpClient client;
        static DbContext db = new DbContext();
        NetworkStream stream;
        public ClientObject(TcpClient tcpClient)
        {
            client = tcpClient;
        }

        public void Process()
        {
            try
            {
                stream = client.GetStream();
                byte[] data = new byte[64]; // буфер для получаемых данных

                // проверочное слово, что клиент подключился к нужному серверу
                string check = "pizzaTime";
                data = Encoding.UTF8.GetBytes(check);
                stream.Write(data, 0, data.Length);

                Console.WriteLine($"Подключился: { Convert.ToString(((System.Net.IPEndPoint)client.Client.RemoteEndPoint).Address)}");
                data = new byte[1024];
                while (true)
                {
                    // получаем сообщение
                    
                    StringBuilder builder = new StringBuilder();
                    int bytes = 0;
                    do
                    {
                        bytes = stream.Read(data, 0, data.Length);
                        builder.Append(Encoding.UTF8.GetString(data, 0, bytes));
                    }
                    while (stream.DataAvailable);

                    string message = builder.ToString();

                    if (message == "") break;

                    getCommand(message);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                if (stream != null)
                    stream.Close();
                if (client != null)
                    client.Close();
            }
        }

        private void getCommand(string message)
        {
            Console.WriteLine($"Получена команда: {message}");
            string command = message.Split(new char[] { '%' })[0];
            switch (command)
            {
                case "getMenu":
                    getMenu();
                    break;

                case "addOrder":
                    addOrder(message); // addOrder%name%address%number%nameItem/count/cost%nameItem2/count/cost%...
                    break;

                default:
                    Console.WriteLine("Неизвестная команда");
                    break;
            }
        }

        // Я получаю что-то типо:   addOrder%name%number%address%nameItem/count/cost%nameItem2/count/cost%...
        // где черз '%' разделены все элементы(имя, адрес, номер, список заказов
        // 0 - команда(addOrder), 1 - имя заказчика, 2 - номер телефона, 3 - адрес заказчика, 4 и больше - список заказаных товаров
        // в списке заказов значения разделены '/'
        private void addOrder(string message)
        {
            string name = "";
            string address = "";
            string number = "";
            List<Ordered> ordereds = new List<Ordered>();

            string[] vs = message.Split(new char[] { '%' });


            if (vs.Length < 5) return;

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"Новый заказ: ");
            Console.ResetColor();
            for (int i = 0; i < vs.Length; i++)
            {
                switch (i)
                {
                    case 0:
                            break;
                    case 1:
                        name = vs[1];
                        Console.WriteLine($"Имя: {name}");
                        break;
                    case 2:
                        number = vs[2];
                        Console.WriteLine($"Мобильный номер: {number}");
                        break;
                    case 3:
                        address = vs[3];
                        Console.WriteLine($"Имя: {address}");
                        break;
                    default:
                        string[] items = vs[i].Split(new char[] { '/' });
                        double price = db.getPizzaByName(items[0]).price * int.Parse(items[1]);
                        ordereds.Add(new Ordered(items[0],int.Parse(items[1]),price));
                        Console.WriteLine($"Пицца: {items[0]}, {items[1]} шт., {price} BYN");
                        break;
                }
            }

            if (name == "" || address == "" || number == "") return; // проверка, есть ли не затронутые поля

            Ordered[] ordereds1 = ordereds.ToArray();

            db.addOrder(name, address, number, ordereds1);

        }

        private void getMenu()
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Запросили меню.");
            Console.ResetColor();

            Pizza[] pizzas = db.getPizzas();

            string message = "";

            for (int i = 0; i < pizzas.Length; i++)
            {
                if (i != pizzas.Length - 1)
                {
                    message += $"{pizzas[i].id}[cut]{pizzas[i].name}[cut]{pizzas[i].description}[cut]{pizzas[i].price}%";
                }
                else
                {
                    message += $"{pizzas[i].id}[cut]{pizzas[i].name}[cut]{pizzas[i].description}[cut]{pizzas[i].price}";
                }
                
            }

            byte[] data = Encoding.UTF8.GetBytes(message);
            stream.Write(data, 0, data.Length);
        }


    }
}
