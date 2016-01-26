using System.Globalization;
using System.Threading.Tasks;
using System.Windows.Media;
using Graph.Objects;

namespace Graph.Algorithms.ShortestPath
{
    class ApathB : Algorithm
    {
        private double[] distance;
        private Vertex[] predecesor;
        private Vertex start;

        public ApathB(Vertex a)
        {
            start = a;
        }

        public override void Initialize()
        {
            base.Initialize();

            distance=new double[NumberOfVerticies];
            predecesor = new Vertex[NumberOfVerticies];
        }

        public override void Execute(Vertex finish)
        {
            Initialize();
            Dijkstra(start);
            if (!double.IsPositiveInfinity(distance[NodeDictionary[finish]]))
            {
                ColorPath(finish, new SolidColorBrush(Colors.Red), 3);
                MainWindow.MessageTextBlock.Text =
                    distance[NodeDictionary[finish]].ToString(CultureInfo.InvariantCulture);
            }
            else
            {
                finish.Body.Background= new SolidColorBrush(Colors.Red);
                MainWindow.MessageTextBlock.Text = "It's impossible to reach target!";
            }
            base.Execute();
        }

        private void Dijkstra(Vertex node)
        {
            Vertex bestOptionVertex = node;

            Parallel.For(0, NumberOfVerticies, i =>
            {
                distance[i] = double.PositiveInfinity;
                Visited[i] = false;
                predecesor[i] = node;
            });

            distance[NodeDictionary[node]] = 0;
            predecesor[NodeDictionary[node]] = null;

            while (true)
            {
                var min = double.PositiveInfinity;
                for (var i = 0; i < NumberOfVerticies; ++i)
                {
                    int index = i;
                    var ind = NodeDictionary[MainWindow.VertexHandler.Verticies[index]];
                    if (!Visited[ind] && min > distance[ind])
                    {
                        min = distance[ind];
                        bestOptionVertex = MainWindow.VertexHandler.Verticies[index];
                    }
                }
                if (!double.IsInfinity(min))
                {
                    var currInd = NodeDictionary[bestOptionVertex];
                    Visited[currInd] = true;

                    foreach (var nextNode in bestOptionVertex.AdjacentNodes)
                    {
                        var weigth = MainWindow.VertexHandler.GetWeightBetween(bestOptionVertex, nextNode);
                        var nextInd = NodeDictionary[nextNode];
                        if (!Visited[nextInd] && distance[nextInd] > distance[currInd] + weigth)
                        {
                            distance[nextInd] = distance[currInd] + weigth;
                            predecesor[nextInd] = bestOptionVertex;
                        }
                    }
                }
                else break;
            } 
        }

        private void ColorPath(Vertex node, SolidColorBrush color, double strokeThickness)
        {
            node.Body.Background = color;
            var temp = node;
            node = predecesor[NodeDictionary[node]];
            while (node != null)
            {
                var edge = MainWindow.EdgeHandler.GetEdgeBetween(node, temp);
                edge.Body.Stroke = color;
                edge.Body.StrokeThickness = strokeThickness;
                node.Body.Background = color;
                temp = node;
                node = predecesor[NodeDictionary[node]];
            }
        }
    }
}
