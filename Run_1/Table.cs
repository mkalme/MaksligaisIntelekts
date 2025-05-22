namespace Run_1
{
    public class Table
    {
        public double[,] Values { get; set; } 
        public Cluster[] RowHeaders { get; set; } = Array.Empty<Cluster>();
    }
}
