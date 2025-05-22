namespace Run
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Tree tree = ParseFromFile(File.ReadAllLines("data.txt"));

            for (int i = 1; i < tree.Middle.Count; i++)
            {
                Node node = tree.Middle[i];

                double result = 0;
                for (int j = 0; j < node.Back.Count; j++)
                {
                    Node? parent = node.Back[j].From;
                    if (parent is null) continue;
                    result += parent.Vector * node.Back[j].Weight;
                }

                node.Shift = Sigmoid(result);

                Console.WriteLine($"WS{i} = {result}");
                Console.WriteLine($"Z{i} = {Sigmoid(result)}");
            }

            double y = 0;
            for (int i = 0; i < tree.End.Back.Count; i++)
            {
                Node? parent = tree.End.Back[i].From;
                if (parent is null) continue;
                y += parent.Shift * tree.End.Back[i].Weight;
            }

            Console.WriteLine($"WS{tree.Middle.Count} = {y}");
            y = Sigmoid(y);
            Console.WriteLine($"Z{tree.Middle.Count} = {y}");

            double error1 = y * (1 - y) * (tree.EndValue - y);
            Console.WriteLine($"Error1 = {error1}");

            for (int i = 0; i < tree.End.Back.Count; i++)
            {
                Node? parent = tree.End.Back[i].From;
                if (parent is null) continue;

                tree.End.Back[i].Weight += tree.Lambda * error1 * parent.Shift;
                Console.WriteLine($"{tree.End.Back[i].Name} = {tree.End.Back[i].Weight}");
            }

            for (int i = 0; i < tree.End.Back.Count; i++)
            {
                Node? node = tree.End.Back[i].From;
                if (node is null || node.Back.Count == 0) continue;

                double error2 = node.Shift * (1 - node.Shift) * error1 * tree.End.Back[i].Weight;
                Console.WriteLine($"Error2({node.Name}) = {error2}");

                for (int j = 0; j < node.Back.Count; j++)
                {
                    Node? parent = node.Back[j].From;
                    if (parent is null) continue;

                    node.Back[j].Weight += tree.Lambda * error2 * parent.Vector;
                    Console.WriteLine($"{node.Back[j].Name} = {node.Back[j].Weight}");
                }
            }

            Console.ReadLine();
        }

        private static Tree ParseFromFile(string[] lines) 
        {
            IDictionary<string, Node> nodes = new Dictionary<string, Node>();
            IDictionary<string, Connection> connections = new Dictionary<string, Connection>();
            IList<TempConnection> tempConnections = new List<TempConnection>();

            Tree output = new Tree();

            int category = 0;
            foreach (string line in lines)
            {
                if (line.StartsWith("#"))
                {
                    category++;
                    continue;
                }

                if (string.IsNullOrEmpty(line)) continue;

                if (category == 1)
                {
                    ParseLine(line, nodes, tempConnections, output.Start);
                }
                else if (category == 2)
                {
                    ParseLine(line, nodes, tempConnections, output.Middle);
                }
                else if (category == 3)
                {
                    Node node = new()
                    {
                        Name = line
                    };

                    nodes.Add(node.Name, node);
                    output.End = node;
                }
                else if (category == 4)
                {
                    string[] shifts = line.Split(',');

                    for (int i = 0; i < shifts.Length; i++)
                    {
                        string[] info = shifts[i].Split("=");
                        Node node = nodes[info[0]];
                        node.Vector = double.Parse(info[1].Replace('.', ','));
                    }
                }
                else if (category == 5)
                {
                    output.EndValue = double.Parse(line.Replace('.', ','));
                }
                else if (category == 6)
                {
                    output.Lambda = double.Parse(line.Replace('.', ','));
                }
                else if (category == 7) 
                {
                    string[] shifts = line.Split(',');

                    for (int i = 0; i < shifts.Length; i++)
                    {
                        string[] info = shifts[i].Split("=");
                        Node node = nodes[info[0]];
                        node.Shift = double.Parse(info[1].Replace('.', ','));
                    }
                }
            }

            foreach (TempConnection connection in tempConnections)
            {
                Connection c = new()
                {
                    From = nodes[connection.From],
                    To = nodes[connection.To],
                    Name = connection.Name,
                    Weight = connection.Weight
                };

                c.From.Forwards.Add(c);
                c.To.Back.Add(c);

                connections.Add(c.Name, c);
            }

            return output;
        }

        private static void ParseLine(string line, IDictionary<string, Node> nodes, IList<TempConnection> tempConnections, IList<Node>? output) 
        {
            string[] split = line.Split(':');
            string nodeName = split[0];

            string[] nodeConnections = split[1].Split(",");
            foreach (string nodeConnection in nodeConnections)
            {
                string[] connectionInfo = nodeConnection.Split("=");

                TempConnection connection = new()
                {
                    From = nodeName,
                    To = connectionInfo[0],
                    Name = connectionInfo[1],
                    Weight = double.Parse(connectionInfo[2].Replace('.', ','))
                };

                tempConnections.Add(connection);
            }

            Node node = new()
            {
                Name = nodeName,
            };

            nodes.Add(node.Name, node);
            output?.Add(node);
        }

        private static double Sigmoid(double x) 
        {
            return 1 / (1 + Math.Exp(-x));
        }
    }
}