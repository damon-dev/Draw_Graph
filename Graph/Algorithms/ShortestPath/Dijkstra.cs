using System.Globalization;
using System.Threading.Tasks;
using System.Windows.Media;
using Graph.Objects;

namespace Graph.Algorithms.ShortestPath
{
    class Dijkstra : Algorithm
    {
        private Vertex[] predecesor;
        private double[] distance;
        private readonly SolidColorBrush mainColor, tempColor, nextColor, searchColor;
        private Task algTask;

        public Dijkstra(Color main, Color temp, Color search, Color next)
        {
            mainColor=new SolidColorBrush(main);
            tempColor=new SolidColorBrush(temp);
            nextColor=new SolidColorBrush(next);
            searchColor=new SolidColorBrush(search);
        }

        public override void Initialize()
        {
            base.Initialize();
            distance = new double[NumberOfVerticies];
            predecesor = new Vertex[NumberOfVerticies];
        }

        public override async void Execute(Vertex start)
        {
            Initialize();
            await (algTask = dijkstra(start));
            base.Execute();
        }

        private async Task dijkstra(Vertex node)
        {
            Vertex bestOptionVertex = node;

            Parallel.For(0, NumberOfVerticies, i =>
            {
                distance[i] = double.PositiveInfinity;
                Visited[i] = false;
                predecesor[i] = node;
            });

            distance[NodeDictionary[node]] = 0;
            MainWindow.PathWindow.SetElement(node, MainWindow.VertexHandler.Verticies[NodeDictionary[node]], "0");
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
                        ColorPath(bestOptionVertex, searchColor, searchColor, searchColor, 3);
                        if (!IsAnimationSkiping) await Task.Delay(AnimationSpeed);
                        ColorPath(bestOptionVertex, nextColor, mainColor, new SolidColorBrush(Colors.Black), 1);
                    }
                }
                if (!double.IsInfinity(min))
                {
                    var currInd = NodeDictionary[bestOptionVertex];
                    Visited[currInd] = true;
                    ColorPath(bestOptionVertex, tempColor, tempColor, tempColor, 3);
                    if (!IsAnimationSkiping) await Task.Delay(AnimationSpeed);
                    foreach (var nextNode in bestOptionVertex.AdjacentNodes)
                    {
                        var weigth = MainWindow.VertexHandler.GetWeightBetween(bestOptionVertex, nextNode);
                        var nextInd = NodeDictionary[nextNode];
                        if (!Visited[nextInd] && distance[nextInd] > distance[currInd] + weigth)
                        {
                            distance[nextInd] = distance[currInd] + weigth;
                            MainWindow.PathWindow.SetElement(node, nextNode,
                                distance[nextInd].ToString(CultureInfo.InvariantCulture),true);
                            predecesor[nextInd] = bestOptionVertex;
                            nextNode.Body.Background = nextColor;
                            if (!IsAnimationSkiping) await Task.Delay(AnimationSpeed);
                        }
                    }
                    ColorPath(bestOptionVertex, mainColor, mainColor, mainColor, 3);
                }
                else
                {
                    ColorPath(bestOptionVertex, mainColor, mainColor, mainColor, 3);
                    break;
                }
            }
        }

        private void ColorPath(Vertex node, SolidColorBrush lastVertexColor, SolidColorBrush pathColor, SolidColorBrush lastEdgeColor,
            double lastStrokeThickness)
        {
            node.Body.Background = lastVertexColor;
            var temp = node;
            node = predecesor[NodeDictionary[node]];
            if (node == null) return;
            var edge = MainWindow.EdgeHandler.GetEdgeBetween(node, temp);
            edge.Body.Stroke = lastEdgeColor;
            edge.Body.StrokeThickness = lastStrokeThickness;
            node.Body.Background = pathColor;
            temp = node;
            node = predecesor[NodeDictionary[node]];
            while (node != null)
            {
                edge = MainWindow.EdgeHandler.GetEdgeBetween(node, temp);
                edge.Body.Stroke = pathColor;
                edge.Body.StrokeThickness = 3;
                node.Body.Background = pathColor;
                temp = node;
                node = predecesor[NodeDictionary[node]];
            }
        }

        public override void SkipAnimation()
        {
            IsAnimationSkiping = true;
        }

        public override async void StopAnimation()
        {
            SkipAnimation();
            await algTask;
            MainWindow.EnterNormalState();
        }
    }
}
