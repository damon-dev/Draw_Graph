using System;
using System.Globalization;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Graph.Algorithms.Trees
{
    class Kruskal : Algorithm
    {
        private readonly SolidColorBrush edgeGoodColor;
        private readonly SolidColorBrush edgeBadColor;
        private readonly SolidColorBrush edgeTempColor;
        private int[] nodesInForest;
        private Task algTask;
        private readonly Random rand;
        private double cost;

        public Kruskal(Color edgegood, Color edgebad, Color edgetemp)
        {
            edgeGoodColor = new SolidColorBrush(edgegood);
            edgeBadColor = new SolidColorBrush(edgebad);
            edgeTempColor=new SolidColorBrush(edgetemp);
            rand = new Random();
        }

        public override void Initialize()
        {
            base.Initialize();
            nodesInForest=new int[NumberOfVerticies];
            for (int i = 0; i < NumberOfVerticies; i++)
            {
                MainWindow.VertexHandler.Verticies[i].Body.Background=new SolidColorBrush(GenerateRandomColor());
                nodesInForest[i] = i;
            }
            MainWindow.EdgeHandler.Edges.Sort((a, b) => a.Weight > b.Weight ? 1 : -1);
        }

        public override async void Execute()
        {
            Initialize();
            await (algTask = KruskalFunc());
            base.Execute();
        }

        private async Task KruskalFunc()
        {
            foreach (var edge in MainWindow.EdgeHandler.Edges)
            {
                edge.Body.Stroke = edgeTempColor;
                edge.Body.StrokeThickness = 3;
                if (!IsAnimationSkiping) await Task.Delay(AnimationSpeed);

                int indA = NodeDictionary[edge.A];
                int indB = NodeDictionary[edge.B];
                if (nodesInForest[indA] != nodesInForest[indB])
                {
                    cost += edge.Weight;
                    edge.Body.Stroke = edgeGoodColor;
                    edge.Body.StrokeThickness = 3;
                    var exTree = nodesInForest[indB];
                    for (int i = 0; i < NumberOfVerticies; ++i)
                    {
                        if (nodesInForest[i] == exTree)
                        {
                            nodesInForest[i] = nodesInForest[indA];
                            MainWindow.VertexHandler.Verticies[i].Body.Background =
                                MainWindow.VertexHandler.Verticies[indA].Body.Background;
                        }
                    }
                }
                else
                {
                    edge.Body.Stroke = edgeBadColor;
                    edge.Body.StrokeThickness = 0.5;
                }
                if (!IsAnimationSkiping) await Task.Delay(AnimationSpeed);
            }
            MainWindow.MessageTextBlock.Text = cost.ToString(CultureInfo.InvariantCulture);
        }

        private Color GenerateRandomColor()
        {
            var rgb = new byte[3];
            rand.NextBytes(rgb);

            return Color.FromRgb(rgb[0], rgb[1], rgb[2]);
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
