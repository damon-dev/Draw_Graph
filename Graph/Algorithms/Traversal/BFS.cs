using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Media;
using Graph.Objects;

namespace Graph.Algorithms.Traversal
{
    class BFS : Algorithm
    {
        private readonly SolidColorBrush mainColor,nextColor, tempColor;
        private Task algTask;

        public BFS(Color main, Color temp, Color next)
        {
            mainColor = new SolidColorBrush(main);
            nextColor = new SolidColorBrush(next);
            tempColor = new SolidColorBrush(temp);
        }

        public override async void Execute(Vertex start)
        {
            Initialize();
            algTask= Bfs(start);
            await algTask;
            base.Execute();
        }

        private async Task Bfs(Vertex start)
        {
            var queue = new Queue<Vertex>();
            queue.Enqueue(start);
            Visited[NodeDictionary[start]] = true;
            MainWindow.MessageTextBlock.Text += start.Content + " ";
            while (queue.Count > 0)
            {
                var node = queue.Dequeue();
                node.Body.Background = tempColor;
                foreach (var vertex in node.AdjacentNodes)
                {
                    var ind = NodeDictionary[vertex];
                    if (Visited[ind]) continue;
                    Visited[ind] = true;
                    vertex.Body.Background = nextColor;
                    queue.Enqueue(vertex);
                    MainWindow.MessageTextBlock.Text += vertex.Content + " ";
                    if (!IsAnimationSkiping) await Task.Delay(AnimationSpeed);
                    vertex.Body.Background = mainColor;
                }
                node.Body.Background= mainColor;
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
