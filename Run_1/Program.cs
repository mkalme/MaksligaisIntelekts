using System.Text;

namespace Run_1
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Table table = ParseFromFile(File.ReadAllLines("data.txt"));
            while (table.RowHeaders.Length > 1) 
            {
                table = DoIteration(table);
            }

            Console.ReadLine();
        }

        private static Table ParseFromFile(string[] lines) 
        {
            Table output = new Table();
            int width = lines[0].Split(',').Length - 1;

            output.Values = new double[lines.Length, width];
            output.RowHeaders = new Cluster[lines.Length];

            for (int i = 0; i < lines.Length; i++) 
            {
                string[] values = lines[i].Split(',');
                output.RowHeaders[i] = new Cluster();
                output.RowHeaders[i].Values.Add(values[0]);

                for (int j = 1; j < values.Length; j++) 
                {
                    output.Values[i, j - 1] = double.Parse(values[j].Replace('.', ','));
                }
            }

            return output;
        }

        private static Table DoIteration(Table table) 
        {
            double[,] distances = new double[table.RowHeaders.Length, table.RowHeaders.Length];
            for (int y = 0; y < distances.GetLength(0); y++)
            {
                for (int x = 0; x < distances.GetLength(1); x++)
                {
                    distances[y, x] = GetDistance(table, y, x);
                }
            }

            FindSmallestPair(distances, out int rowA, out int rowB, out double d);
            int l = table.RowHeaders[rowA].Values.Count;
            Table output = Combine(table, rowA, rowB);

            StringBuilder builder = new("(");

            for (int i = 0; i < output.RowHeaders[rowA].Values.Count; i++) 
            {
                builder.Append(output.RowHeaders[rowA].Values[i]);

                if (i == l - 1) builder.Append(')');
                if (i < output.RowHeaders[rowA].Values.Count - 1) builder.Append(" ");
                if (i == l - 1) builder.Append('(');
            }

            builder.Append(')');

            Console.WriteLine($"{builder} = {d}");

            return output;
        }
        private static double GetDistance(Table table, int rowA, int rowB) 
        {
            double sum = 0;

            for (int i = 0; i < table.Values.GetLength(1); i++) 
            {
                sum += Math.Pow(table.Values[rowA, i] - table.Values[rowB, i], 2);
            }

            return Math.Sqrt(sum);
        }
        private static void FindSmallestPair(double[,] distances, out int rowA, out int columnB, out double distance) 
        {
            double smallest = double.MaxValue;
            rowA = 0;
            columnB = 0;

            for (int i = 0; i < distances.GetLength(0); i++) 
            {
                for (int j = i + 1; j < distances.GetLength(1); j++)
                {
                    double value = distances[i, j];
                    if (value < smallest)
                    {
                        smallest = value;
                        rowA = i;
                        columnB = j;
                    }
                }
            }

            distance = smallest;
        }
        private static Table Combine(Table input, int headerA, int headerB) 
        {
            Table output = new();
            output.RowHeaders = new Cluster[input.RowHeaders.Length - 1];
            output.Values = new double[input.RowHeaders.Length - 1, input.Values.GetLength(1)];

            int row = 0;
            for (int i = 0; i < output.RowHeaders.Length; i++) 
            {
                if (i == headerB) row++;

                for (int j = 0; j < output.Values.GetLength(1); j++) 
                {
                    output.Values[i, j] = input.Values[row, j];
                }

                output.RowHeaders[i] = input.RowHeaders[row];

                row++;
            }

            for (int i = 0; i < output.Values.GetLength(1); i++)
            {
                output.Values[headerA, i] = (input.Values[headerA, i] + input.Values[headerB, i]) / 2;
            }

            output.RowHeaders[headerA].Values.AddRange(input.RowHeaders[headerB].Values);

            return output;
        }
    }
}