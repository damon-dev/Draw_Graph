using System.Globalization;
using System.Threading.Tasks;
using System.Windows.Media;
using Graph.Objects;

namespace Graph.Algorithms.Trees
{
    class Prim : Algorithm
    {
        private readonly SolidColorBrush initColor, edgeFinalColor, edgeTempColor, nodeFinalColor;
        private double[] distance;
        private Vertex[] predecesor;
        private Task algTask;
        private double cost;
        Edge edge;

        public Prim(Color init, Color edgefinal, Color edgetemp, Color nodefinal)
        {
            initColor = new SolidColorBrush(init);
            edgeFinalColor = new SolidColorBrush(edgefinal);
            edgeTempColor = new SolidColorBrush(edgetemp);
            nodeFinalColor = new SolidColorBrush(nodefinal);
        }

        public override void Initialize()
        {
            base.Initialize();

            foreach (var vertex in MainWindow.VertexHandler.Verticies)
                vertex.Body.Background = initColor;
            distance = new double[NumberOfVerticies];
            predecesor = new Vertex[NumberOfVerticies];
        }

        public override async void Execute(Vertex start)
        {
            Initialize();
            await (algTask = PrimFunc(start));
            base.Execute();
        }

        private async Task PrimFunc(Vertex node)
        {
            Vertex bestOptionVertex = node;

            Parallel.For((long) 0, NumberOfVerticies, i =>
            {
                distance[i] = double.PositiveInfinity;
                Visited[i] = false;
                predecesor[i] = node;
            });

            distance[NodeDictionary[node]] = 0;
            node.Body.Background = nodeFinalColor;
            predecesor[NodeDictionary[node]] = null;

            while (true)
            {
                var min = double.PositiveInfinity;
                for (var i = 0; i < NumberOfVerticies; ++i)
                {
                    int index = i;
                    var ind = NodeDictionary[MainWindow.VertexHandler.Verticies[index]];
                    if (!Visited[ind])
                    {
                        if(min > distance[ind])
                        {
                            min = distance[ind];
                            bestOptionVertex = MainWindow.VertexHandler.Verticies[index];
                        }
                    }
                }
                if (double.IsInfinity(min)) break;
                var currInd = NodeDictionary[bestOptionVertex];
                bestOptionVertex.Body.Background = nodeFinalColor;
                if (!bestOptionVertex.Equals(node))
                {
                    edge = MainWindow.EdgeHandler.GetEdgeBetween(bestOptionVertex, predecesor[currInd]);
                    edge.Body.Stroke = edgeFinalColor;
                }
                Visited[currInd] = true;
                cost += distance[currInd];
               
                foreach (var nextNode in bestOptionVertex.AdjacentNodes)
                {
                    var weigth = MainWindow.VertexHandler.GetWeightBetween(bestOptionVertex, nextNode);
                    var nextInd = NodeDictionary[nextNode];
                    if (!Visited[nextInd])
                    {
                        if (distance[nextInd] > weigth)
                        {
                            predecesor[nextInd] = bestOptionVertex;
                            distance[nextInd] = weigth;
                        }
                        edge=MainWindow.EdgeHandler.GetEdgeBetween(nextNode, bestOptionVertex);
                        edge.Body.Stroke= edgeTempColor;
                        edge.Body.StrokeThickness = 3;
                    }
                    else
                    {
                        edge = MainWindow.EdgeHandler.GetEdgeBetween(nextNode, bestOptionVertex);
                        if (edge.Body.Stroke.Equals(edgeFinalColor)) continue;
                        edge.Body.Stroke = initColor;
                        edge.Body.StrokeThickness = 1;
                    }
                }
                if (!IsAnimationSkiping) await Task.Delay(AnimationSpeed);
            }
            MainWindow.MessageTextBlock.Text = cost.ToString(CultureInfo.InvariantCulture);
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
