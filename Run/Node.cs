namespace Run
{
    public class Node
    {
        public IList<Connection> Back { get; set; } = new List<Connection>();
        public IList<Connection> Forwards { get; set; } = new List<Connection>();
        public string Name { get; set; } = string.Empty;
        public double Shift { get; set; } = 0;
        public double Vector { get; set; } = 0;
    }
}
