using System.Linq;
using System.Threading.Tasks;
using System.Windows.Media;
using Graph.Objects;

namespace Graph.Algorithms.Traversal
{
    class DFS : Algorithm
    {
        private readonly SolidColorBrush mainColor;
        private readonly SolidColorBrush tempColor;
        private Task algTask;

        public DFS(Color main, Color temp)
        {
            mainColor = new SolidColorBrush(main);
            tempColor = new SolidColorBrush(temp);
        }

        public override async void Execute(Vertex start)
        {
            Initialize();
            algTask = Dfs(start);
            await algTask;
            base.Execute();
        }

        private async Task Dfs(Vertex node)
         {
             Visited[NodeDictionary[node]] = true;
            
             MainWindow.MessageTextBlock.Text += node.Content + " ";
             node.Body.Background = tempColor;
             if(!IsAnimationSkiping) await Task.Delay(AnimationSpeed);
             foreach (var vertex in from vertex in node.AdjacentNodes let ind = NodeDictionary[vertex] where !Visited[ind] select vertex)
             {
                 node.Body.Background = mainColor;
                 await Dfs(vertex);
                 node.Body.Background = tempColor;
                 if (!IsAnimationSkiping) await Task.Delay(AnimationSpeed);
             }
             node.Body.Background = mainColor;
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
