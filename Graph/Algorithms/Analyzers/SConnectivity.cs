using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Media;
using Graph.Objects;

namespace Graph.Algorithms.Analyzers
{
    class SConnectivity : Algorithm
    {
        private SolidColorBrush color;
        private readonly Random rand;
        private List<Vertex>[] transposedGraph;
        private Stack<Vertex> postorderStack; 

        public SConnectivity()
        {
            rand=new Random();
        }

        public override void Initialize()
        {
            base.Initialize();

            transposedGraph = new List<Vertex>[NumberOfVerticies];
            
            Parallel.For(0, NumberOfVerticies, i =>
            {
                transposedGraph[i]=new List<Vertex>();
            });

            postorderStack = new Stack<Vertex>();

            foreach (var vertex in MainWindow.VertexHandler.Verticies)
                foreach (var adjacentNode in vertex.AdjacentNodes)
                    transposedGraph[NodeDictionary[adjacentNode]].Add(vertex);
        }

        public override void Execute()
        {
            Initialize();
            int count = 0;
            for (int i = 0; i < NumberOfVerticies; i++)
                if (!Visited[i])
                    PostOrder(MainWindow.VertexHandler.Verticies[i]);
            while (postorderStack.Count > 0)
            {
                var vertex = postorderStack.Pop();
                if (Visited[NodeDictionary[vertex]])
                {
                    color = new SolidColorBrush(GenerateRandomColor());
                    count += 1;
                    Dfs(vertex);
                }
            }

            switch (count)
            {
                case 1:
                    MainWindow.MessageTextBlock.Text = "Graph is strongly connected!";
                    break;
                default:
                    MainWindow.MessageTextBlock.Text = string.Format("There are {0} strongly connected components!", count);
                    break;
            }

            base.Execute();
        }

        private void PostOrder(Vertex vertex)
        {
            Visited[NodeDictionary[vertex]] = true;
            foreach (var node in vertex.AdjacentNodes.Where(node => !Visited[NodeDictionary[node]]))
                PostOrder(node);
            postorderStack.Push(vertex);
        }

        private void Dfs(Vertex vertex)
        {
            Visited[NodeDictionary[vertex]] = false;
            vertex.Body.Background = color;
            foreach (var node in transposedGraph[NodeDictionary[vertex]].Where(node => Visited[NodeDictionary[node]]))
                Dfs(node);
        }

        private Color GenerateRandomColor()
        {
            var rgb = new byte[3];
            rand.NextBytes(rgb);

            return Color.FromRgb(rgb[0], rgb[1], rgb[2]);
        }
    }
}
