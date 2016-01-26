using System;
using System.Globalization;
using System.Linq;
using System.Windows.Media;
using Graph.Objects;

namespace Graph.Algorithms.Analyzers
{
    internal class Connectivity : Algorithm
    {
        private SolidColorBrush color;
        private Random rand;
        private double[] weightsBackup;

        public Connectivity()
        {
            rand=new Random();
        }

        public override void Execute()
        {
            Initialize();
            int count = 0;
            bool switched = false;
            if (MainWindow.EdgeHandler.EdgesAreDirected)
            {
                if (MainWindow.EdgeHandler.EdgesAreWeighted)
                {
                    weightsBackup = new double[MainWindow.EdgeHandler.Edges.Count];
                    for (int i = 0; i < MainWindow.EdgeHandler.Edges.Count; i++)
                    {
                        var edge = MainWindow.EdgeHandler.Edges[i];
                        weightsBackup[i] = edge.Weight;
                    }
                }
                MainWindow.EdgeHandler.SwitchTypeDirected();
                switched = true;
            }
            for (int i = 0; i < NumberOfVerticies; ++i)
            {
                var vertex = MainWindow.VertexHandler.Verticies[i];
                if (!Visited[NodeDictionary[vertex]])
                {
                    count += 1;
                    color = new SolidColorBrush(GenerateRandomColor());
  
                    Dfs(vertex);
                }
            }

            if (switched)
            {
                MainWindow.EdgeHandler.SwitchTypeDirected();
                if (MainWindow.EdgeHandler.EdgesAreWeighted)
                {
                    for (int i = 0; i < MainWindow.EdgeHandler.Edges.Count; i++)
                    {
                        var edge = MainWindow.EdgeHandler.Edges[i];
                        edge.Weight = weightsBackup[i];
                        edge.TbWeight.Text = weightsBackup[i].ToString(CultureInfo.InvariantCulture);
                    }
                }
            }

            switch (count)
            {
                case 1:
                    MainWindow.MessageTextBlock.Text = "Graph is connected!";
                    break;
                default:
                    MainWindow.MessageTextBlock.Text = string.Format("There are {0} connected components!", count);
                    break;
            }
            base.Execute();
        }

        private Color GenerateRandomColor()
        {
            var rgb = new byte[3];
            rand.NextBytes(rgb);

            return Color.FromRgb(rgb[0], rgb[1], rgb[2]);
        }

        private void Dfs(Vertex node)
        {
            Visited[NodeDictionary[node]] = true;
            node.Body.Background = color;
            foreach (
                var vertex in
                    from vertex in node.AdjacentNodes let ind = NodeDictionary[vertex] where !Visited[ind] select vertex
                )
                Dfs(vertex);
        }
    }
}
