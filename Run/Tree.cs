namespace Run
{
    public class Tree
    {
        public IList<Node> Start { get; set; } = new List<Node>();
        public IList<Node> Middle { get; set; } = new List<Node>();
        public Node End { get; set; }

        public double EndValue { get; set; }
        public double Lambda { get; set; }
    }
}
