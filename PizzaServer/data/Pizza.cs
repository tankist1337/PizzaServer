namespace PizzaServer.data
{
    public class Pizza
    {
        private int _id;
        public int id
        {
            get { return _id; }
        }

        private string _name;
        public string name
        {
            get { return _name; }
            set { _name = value; }
        }

        private string _description;
        public string description
        {
            get { return _description; }
            set { _description = value; }
        }
        private double _price;
        public double price
        {
            get { return _price; }
            set { _price = value; }
        }

        public Pizza(int i, string n, string d, double p)
        {
            _id = i;
            _name = n;
            _description = d;
            _price = p;
        }
    }
}
