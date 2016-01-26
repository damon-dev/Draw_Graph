using System.Linq;
using System.Threading.Tasks;
using System.Windows.Media;
using Graph.Objects;

namespace Graph.Algorithms.Traversal
{
    class PostOrder : Algorithm
    {
        private readonly SolidColorBrush mainColor;
        private readonly SolidColorBrush tempColor;
        private readonly SolidColorBrush currColor;
        private Task algTask;

        public PostOrder(Color main, Color temp, Color current)
        {
            mainColor = new SolidColorBrush(main);
            tempColor = new SolidColorBrush(temp);
            currColor = new SolidColorBrush(current);
        }

        public override async void Execute(Vertex start)
        {
            Initialize();
            algTask = Postorder(start);
            await algTask;
            base.Execute();
        }

        private async Task Postorder(Vertex node)
         {
             Visited[NodeDictionary[node]] = true;
             node.Body.Background = currColor;
             if (!IsAnimationSkiping) await Task.Delay(AnimationSpeed);

             foreach (var vertex in from vertex in node.AdjacentNodes let ind = NodeDictionary[vertex] where !Visited[ind] select vertex)
             {
                 node.Body.Background = tempColor;
                 await Postorder(vertex);
                 node.Body.Background = currColor;
                 if (!IsAnimationSkiping) await Task.Delay(AnimationSpeed);
             }

             MainWindow.MessageTextBlock.Text += node.Content + " ";
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
