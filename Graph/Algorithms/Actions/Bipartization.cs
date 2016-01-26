using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Media;
using Graph.Objects;

namespace Graph.Algorithms.Actions
{
    class Bipartitzation : Algorithm
    {
        private readonly SolidColorBrush[] colors;
        private readonly SolidColorBrush tempColor;
        private bool[] colorUsed;
        private Task algTask;
        private Stack<Edge> edgeStack;

        public Bipartitzation(Color first, Color seccond, Color temp)
        {
            colors = new SolidColorBrush[2];
            colors[0] = new SolidColorBrush(first);
            colors[1] = new SolidColorBrush(seccond);
            tempColor = new SolidColorBrush(temp);
        }

        public override void Initialize()
        {
            base.Initialize();
            colorUsed = new bool[NumberOfVerticies];
            edgeStack=new Stack<Edge>();
        }

        public override async void Execute()
        {
            Initialize();
            for (int i = 0; i < NumberOfVerticies; i++)
            {
                if (Visited[i]) continue;
                Visited[i] = true;
                algTask = Bipartite(MainWindow.VertexHandler.Verticies[i]);
                await algTask;
            }

            base.Execute();
        }

        private async Task Bipartite(Vertex node)
        {
            node.Body.Background = tempColor;
            if (!IsAnimationSkiping) await Task.Delay(AnimationSpeed);
            var ind = NodeDictionary[node];
            var currColorInd = colorUsed[ind] ? 1 : 0;
            for (int i = 0; i < node.AdjacentNodes.Count; i++)
            {
                var vertex = node.AdjacentNodes[i];
                var nextInd = NodeDictionary[vertex];
                var nextColor = !colorUsed[ind];
                if (!Visited[nextInd])
                {
                    node.Body.Background = colors[currColorInd];
                    colorUsed[nextInd] = nextColor;
                    Visited[nextInd] = true;
                    await Bipartite(vertex);
                    node.Body.Background = tempColor;
                    if (!IsAnimationSkiping) await Task.Delay(AnimationSpeed);
                }
                else
                {
                    if (colorUsed[nextInd] != nextColor)
                    {
                        var edge = MainWindow.EdgeHandler.GetEdgeBetween(node, vertex);
                        edgeStack.Push(edge);
                        MainWindow.EdgeHandler.RemoveEdge(edge);
                        i -= 1;
                        if (!IsAnimationSkiping) await Task.Delay(AnimationSpeed);
                    }
                }
            }
            node.Body.Background = colors[currColorInd];
        }

        public override void SkipAnimation()
        {
            IsAnimationSkiping = true;
        }

        public override async void StopAnimation()
        {
            SkipAnimation();
            await algTask;
            while (edgeStack.Count > 0)
            {
                var edge = edgeStack.Pop();
                MainWindow.EdgeHandler.CreateEdge(edge.A,edge.B);
                MainWindow.EdgeHandler.Edges[MainWindow.EdgeHandler.Edges.Count - 1].Weight = edge.Weight;
            }
            MainWindow.EnterNormalState();
        }
    }
}
