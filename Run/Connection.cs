namespace Run
{
    public class Connection
    {
        public Node? From { get; set; }
        public Node? To { get; set; }
    
        public double Weight { get; set; }
        public string Name { get; set; } = string.Empty;
    }
}
