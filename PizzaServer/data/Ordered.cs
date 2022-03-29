namespace PizzaServer.data
{
    public class Ordered
    {
        private int _id;
        public int id
        {
            get { return _id; }
        }

        private int _idOrder;
        public int idOrder
        {
            get { return _idOrder; }
        }

        private string _name;
        public string name
        {
            get { return _name; }
        }

        private int _count;
        public int count
        {
            get { return _count; }
        }

        private double _cost;
        public double cost
        {
            get { return _cost; }
        }

        public Ordered(int i, int io, string n, int cnt, double cst)
        {
            _id = i;
            _idOrder = io;
            _name = n;
            _count = cnt;
            _cost = cst;
        }

        public Ordered(int io, string n, int cnt, double cst)
        {
            _idOrder = io;
            _name = n;
            _count = cnt;
            _cost = cst;
        }

        public Ordered(string n, int cnt, double cst)
        {
            _name = n;
            _count = cnt;
            _cost = cst;
        }
    }
}
