using System;
using System.Threading.Tasks;
using System.Windows.Media;
using Graph.Objects;

namespace Graph.Algorithms.Trees
{
    class DoMaxHeap : Algorithm
    {
        private Vertex[] heap;
        private Task algTask;
        private Edge[] edgeBackup;
        private string[] indexBackup;
        private int backupInd;

        public override void Initialize()
        {
            base.Initialize();

            heap = new Vertex[NumberOfVerticies+1];
            edgeBackup = new Edge[MainWindow.EdgeHandler.Edges.Count];
            indexBackup=new string[NumberOfVerticies];
            while (backupInd < MainWindow.EdgeHandler.Edges.Count)
            {
                edgeBackup[backupInd] = MainWindow.EdgeHandler.Edges[backupInd];
                backupInd += 1;
            }
            for (var i = 1; i <= NumberOfVerticies; i++)
            {
                heap[i] = MainWindow.VertexHandler.Verticies[i - 1];
                indexBackup[i - 1] = MainWindow.VertexHandler.Verticies[i - 1].Content;
                MainWindow.VertexHandler.Isolate(heap[i]);
            }
        }

        public override async void Execute()
        {
            Initialize();
            await (algTask = MaxHeap());
            base.Execute();
        }

        private async Task MaxHeap()
        {
            for (int i = NumberOfVerticies/2; i > 0; i--)
            {
                MainWindow.EdgeHandler.CreateEdge(heap[i], heap[2*i]);
                if (2*i + 1 <= NumberOfVerticies) MainWindow.EdgeHandler.CreateEdge(heap[i], heap[2*i + 1]);
                heap[i].Body.Background = new SolidColorBrush(Colors.Cyan);
                if (!IsAnimationSkiping) await Task.Delay(AnimationSpeed);
                heap[i].Body.Background = new SolidColorBrush(Colors.Chartreuse);
                await CombHeap(i);
            }
        }

        private async Task CombHeap(int parent)
        {
            int child = parent*2;
            while (child <= NumberOfVerticies)
            {
                bool tmp;
                if (child + 1 <= NumberOfVerticies)
                {
                    if (!IntegersOnlyChecked) tmp = String.Compare(heap[child + 1].Content, heap[child].Content, StringComparison.Ordinal) > 0;
                    else tmp = int.Parse(heap[child + 1].Content) > int.Parse(heap[child].Content);
                    if (tmp)
                        child = child + 1;
                }
                if (!IntegersOnlyChecked) tmp = String.Compare(heap[child].Content, heap[parent].Content, StringComparison.Ordinal) > 0;
                else tmp = int.Parse(heap[child].Content) > int.Parse(heap[parent].Content);
                if (tmp)
                {
                    string aux = heap[child].Content;
                    heap[child].Content = heap[parent].Content;
                    heap[parent].Content = aux;
                    heap[child].Body.Background=new SolidColorBrush(Colors.Cyan);
                    parent = child;
                    child = parent*2;
                    if (!IsAnimationSkiping) await Task.Delay(AnimationSpeed);
                    heap[parent].Body.Background = new SolidColorBrush(Colors.Chartreuse);
                }
                else break;
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
            for (int i = 0; i < MainWindow.VertexHandler.Verticies.Count; i++)
            {
                var vertex = MainWindow.VertexHandler.Verticies[i];
                vertex.Content = indexBackup[i];
                MainWindow.VertexHandler.Isolate(vertex);
            }
            for (int i = 0; i < backupInd; i++)
            {
                MainWindow.EdgeHandler.CreateEdge(edgeBackup[i].A, edgeBackup[i].B);
                MainWindow.EdgeHandler.Edges[i].Weight = edgeBackup[i].Weight;
            }
            MainWindow.EnterNormalState();
        }
    }
}
