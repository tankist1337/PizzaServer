using System;
using System.Collections.Generic;
using System.Data.SQLite;

namespace PizzaServer.data
{
    public class DbContext
    {
        public Pizza[] getPizzas()
        {
            List<Pizza> pizzasList = new List<Pizza>();

            using var connection = new SQLiteConnection(@"Data source = cafe.db");
            connection.Open();

            using var command = new SQLiteCommand(connection);
            command.CommandText = "SELECT * FROM pizza";

            using SQLiteDataReader reader = command.ExecuteReader();

            Pizza pizza;

            while (reader.Read())
            {
                pizza = new Pizza(reader.GetInt32(0), reader.GetString(1), reader.GetString(2), reader.GetDouble(3));
                pizzasList.Add(pizza);
            }

            reader.Close();
            connection.Close();

            Pizza[] pizzasArray = pizzasList.ToArray();

            return pizzasArray;
        }

        public void addPizza(string name, string description, double price)
        {
            using var connection = new SQLiteConnection(@"Data source = cafe.db");
            connection.Open();

            using var command = new SQLiteCommand(connection);
            command.CommandText = "INSERT INTO pizza(name, description, price) VALUES(@name, @description, @price)";

            command.Parameters.AddWithValue("@name", name);
            command.Parameters.AddWithValue("@description", description);
            command.Parameters.AddWithValue("@price", price);
            command.Prepare();

            command.ExecuteNonQuery();
            connection.Close();
        }

        // don't works
        public void removePizza(int id)
        {
            using var connection = new SQLiteConnection(@"Data source = cafe.db");
            connection.Open();

            using var command = new SQLiteCommand(connection);
            command.CommandText = "DELETE FROM pizza WHERE id = @id";

            command.Parameters.AddWithValue("@id", id);
            command.Prepare();

            Console.WriteLine(command.ExecuteNonQuery());
            connection.Close();
        }


        public void updatePizza(Pizza pizza)
        {
            using var connection = new SQLiteConnection(@"Data source = cafe.db");
            connection.Open();

            using var command = new SQLiteCommand(connection);
            command.CommandText = "UPDATE pizza SET name = @name, description = @description, price = @price WHERE id = @id";

            command.Parameters.AddWithValue("@id", pizza.id);
            command.Parameters.AddWithValue("@name", pizza.name);
            command.Parameters.AddWithValue("@description", pizza.description);
            command.Parameters.AddWithValue("@price", pizza.price);
            command.Prepare();

            command.ExecuteNonQuery();
            connection.Close();
        }

        public void addOrder(string name, string address, string number, Ordered[] ordereds)
        {
            using var connection = new SQLiteConnection(@"Data source = cafe.db");
            connection.Open();

            using var command = new SQLiteCommand(connection);

            // Добавляю строку в таблицу orders, в которой дата заказа, имя заказчика, мобильный номер и адрес
            command.CommandText = "INSERT INTO orders(data, username, number, address) VALUES(@data, @username, @number, @address)";
            command.Parameters.AddWithValue("@data", DateTime.Now.ToString());
            command.Parameters.AddWithValue("@username", name);
            command.Parameters.AddWithValue("@number", number);
            command.Parameters.AddWithValue("@address", address);
            command.Prepare();
            command.ExecuteNonQuery();

            // Получаю id добавленной строки
            command.CommandText = "SELECT MAX(id) FROM orders";
            using SQLiteDataReader reader = command.ExecuteReader();
            reader.Read();
            int idOrder = reader.GetInt32(0);

            // Закрываю все подключения
            reader.Close();
            connection.Close();

            // Вызываю метод, который записывает в таблицу ordered список заказанных пицц
            addOrdered(ordereds, idOrder);
        }

        //метод, который записывает в таблицу ordered список заказанных пицц
        private void addOrdered(Ordered[] ordereds, int id)
        {
            using var connection = new SQLiteConnection(@"Data source = cafe.db");
            connection.Open();

            using var command = new SQLiteCommand(connection);

            for (int i = 0; i < ordereds.Length; i++)
            {
                command.CommandText = "INSERT INTO ordered(id_order, item_name, count, cost) VALUES(@id, @name, @count, @cost)";

                command.Parameters.AddWithValue("@id", id);
                command.Parameters.AddWithValue("@name", ordereds[i].name);
                command.Parameters.AddWithValue("@count", ordereds[i].count);
                command.Parameters.AddWithValue("@cost", ordereds[i].cost);
                command.Prepare();

                command.ExecuteNonQuery();
            }

            connection.Close();
        }

        // получить информацию о пицце по его названию
        public Pizza getPizzaByName(string name)
        {
            using var connection = new SQLiteConnection(@"Data source = cafe.db");
            connection.Open();

            using var command = new SQLiteCommand(connection);
            command.CommandText = "SELECT * FROM pizza WHERE name = @name";
            command.Parameters.AddWithValue("@name",name);
            command.Prepare();

            using SQLiteDataReader reader = command.ExecuteReader();

            reader.Read();

            Pizza pizza = new Pizza(reader.GetInt32(0),reader.GetString(1),reader.GetString(2),reader.GetDouble(3));

            reader.Close();
            connection.Close();

            return pizza;
        }
    }
}
