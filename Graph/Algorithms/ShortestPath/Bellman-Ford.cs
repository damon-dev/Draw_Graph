using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using System.Windows.Media;
using Graph.Objects;

namespace Graph.Algorithms.ShortestPath
{
    class BellmanFord : Algorithm
    {
        private readonly SolidColorBrush mainColor, tempColor, nextColor;
        private double[] distance;
        private int[] numberOfVisitations;
        private Task algTask;
        private Queue<Vertex> q;

        public BellmanFord(Color main, Color temp, Color next)
        {
            mainColor = new SolidColorBrush(main);
            tempColor = new SolidColorBrush(temp);
            nextColor = new SolidColorBrush(next);
        }

        public override void Initialize()
        {
            base.Initialize();
            distance = new double[NumberOfVerticies];
            numberOfVisitations = new int[NumberOfVerticies];
            q = new Queue<Vertex>();

        }

        public override async void Execute(Vertex start)
        {
            Initialize();
            await(algTask = Bellman(start));

            base.Execute();
        }

        private async Task Bellman(Vertex node)
        {
            Parallel.For(0, NumberOfVerticies, i =>
            {
                distance[i] = double.PositiveInfinity;
            });

            distance[NodeDictionary[node]] = 0;
            numberOfVisitations[NodeDictionary[node]] = 1;
            MainWindow.PathWindow.SetElement(node, MainWindow.VertexHandler.Verticies[NodeDictionary[node]], "0");

            q.Enqueue(node);

            while (q.Count > 0)
            {
                var vert = q.Dequeue();
                vert.Body.Background = tempColor;
                if (!IsAnimationSkiping) await Task.Delay(AnimationSpeed);

                foreach (var nextNode in vert.AdjacentNodes)
                {
                    var newDist = distance[NodeDictionary[vert]] + MainWindow.VertexHandler.GetWeightBetween(vert, nextNode);
                    if (distance[NodeDictionary[nextNode]] > newDist)
                    {
                        distance[NodeDictionary[nextNode]] = newDist;
                        MainWindow.PathWindow.SetElement(node, nextNode,
                                distance[NodeDictionary[nextNode]].ToString(CultureInfo.InvariantCulture),true);
                        numberOfVisitations[NodeDictionary[nextNode]] += 1;
                        if (numberOfVisitations[NodeDictionary[nextNode]] == NumberOfVerticies)
                        {
                            MainWindow.MessageTextBlock.Text = "Graph has negative cost circuit!";
                            return;
                        }
                        q.Enqueue(nextNode);
                        nextNode.Body.Background = nextColor;
                        if (!IsAnimationSkiping) await Task.Delay(AnimationSpeed);
                    }
                }
                vert.Body.Background = mainColor;
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
