using System.IO;
using System.Windows;
using Graph.Algorithms;
using Microsoft.Win32;

namespace Graph.Handlers
{
    public class IOMethods
    {
        private readonly MainWindow mainWindow;
        public string CurentOpenFile;

        public IOMethods(MainWindow window)
        {
            mainWindow = window;
        }

        public void LoadGraph()
        {
            var dialog = new OpenFileDialog
            {
                DefaultExt = ".graph",
                Filter = "GRAPH Files (*.graph)|*.graph"
            };

            var result = dialog.ShowDialog();

            if (result != true) return;

            while (mainWindow.VertexHandler.Verticies.Count > 0)
                mainWindow.VertexHandler.RemoveNode(mainWindow.VertexHandler.Verticies[0]);

            var reader = new StreamReader(dialog.FileName);
            mainWindow.Title = Path.GetFileNameWithoutExtension(dialog.FileName);

            ReadGraph(reader);

            CurentOpenFile = dialog.FileName;

            reader.Close();
        }

        public void RestoreGraph()
        {
            if (CurentOpenFile == null) return;

            while (mainWindow.VertexHandler.Verticies.Count > 0)
                mainWindow.VertexHandler.RemoveNode(mainWindow.VertexHandler.Verticies[0]);
            var reader = new StreamReader(CurentOpenFile);
            ReadGraph(reader);
            reader.Close();
        }

        private void ReadGraph(TextReader reader)
        {
            if (reader == null) return;
            var line = reader.ReadLine();
            if (line == null) return;

            Algorithm.IntegersOnlyChecked = false;
            mainWindow.IntegersOnlyCheckBox.IsChecked = false;
            var input = line.Split(' ');
            var n = int.Parse(input[0]);
            var m = int.Parse(input[1]);
            var oriented = int.Parse(input[2]);
            var weighted = int.Parse(input[3]);
            mainWindow.EdgeHandler.EdgesAreDirected = oriented == 1;
            mainWindow.EdgeHandler.EdgesAreWeighted = weighted == 1;
            mainWindow.MiDirected.IsChecked = mainWindow.EdgeHandler.EdgesAreDirected;
            mainWindow.MiWeightedEdges.IsChecked = mainWindow.EdgeHandler.EdgesAreWeighted;
            for (var i = 0; i < n; i++)
            {
                var readLine = reader.ReadLine();
                if (readLine != null) input = readLine.Split(' ');
                mainWindow.VertexHandler.CreateNode(new Point(double.Parse(input[0]), double.Parse(input[1])), input[2]);
            }

            for (var i = 0; i < m; ++i)
            {
                var readLine = reader.ReadLine();
                if (readLine != null) input = readLine.Split(' ');
                int indexA = int.Parse(input[0])-1, indexB = int.Parse(input[1])-1;
                mainWindow.EdgeHandler.CreateEdge(mainWindow.VertexHandler.Verticies[indexA], mainWindow.VertexHandler.Verticies[indexB]);
                if (mainWindow.EdgeHandler.EdgesAreWeighted)
                {
                    mainWindow.EdgeHandler.Edges[mainWindow.EdgeHandler.Edges.Count - 1].Weight = double.Parse(input[2]);
                    mainWindow.EdgeHandler.Edges[mainWindow.EdgeHandler.Edges.Count - 1].TbWeight.Text = input[2];
                }
            }
        }

        public void SaveGraph()
        {
            if (CurentOpenFile == null)
            {
                var dialog = new SaveFileDialog
                {
                    DefaultExt = ".graph",
                    Filter = "GRAPH files(*.graph)|*.graph",
                    RestoreDirectory = true
                };

                var result = dialog.ShowDialog();

                if (result != true) return;
                var writer = new StreamWriter(dialog.FileName);
                mainWindow.Title = Path.GetFileNameWithoutExtension(dialog.FileName);

                WriteGraph(writer);

                CurentOpenFile = dialog.FileName;

                writer.Close();
            }
            else
            {
                var writer = new StreamWriter(CurentOpenFile);

                WriteGraph(writer);

                writer.Close();
            }
        }

        public void SaveAsGraph()
        {
            var dialog = new SaveFileDialog
            {
                DefaultExt = ".graph",
                Filter = "GRAPH files(*.graph)|*.graph",
                RestoreDirectory = true
            };

            var result = dialog.ShowDialog();

            if (result != true) return;
            var writer = new StreamWriter(dialog.FileName);
            mainWindow.Title = Path.GetFileNameWithoutExtension(dialog.FileName);

            WriteGraph(writer);

            CurentOpenFile = dialog.FileName;

            writer.Close();
        }

        private void WriteGraph(TextWriter writer)
        {
            if (writer == null) return;

            writer.WriteLine("{0} {1} {2} {3}", mainWindow.VertexHandler.Verticies.Count, mainWindow.EdgeHandler.Edges.Count,
                mainWindow.EdgeHandler.EdgesAreDirected ? 1 : 0,
                mainWindow.EdgeHandler.EdgesAreWeighted ? 1 : 0);

            foreach (var t in mainWindow.VertexHandler.Verticies)
            {
                writer.WriteLine("{0} {1} {2}", t.Center.X, t.Center.Y, t.Content);
            }

            if (!mainWindow.EdgeHandler.EdgesAreWeighted)
            {
                foreach (var t in mainWindow.EdgeHandler.Edges)
                {
                    writer.WriteLine("{0} {1}", mainWindow.VertexHandler.Verticies.IndexOf(t.A)+1, mainWindow.VertexHandler.Verticies.IndexOf(t.B)+1);
                }
            }
            else
            {
                foreach (var t in mainWindow.EdgeHandler.Edges)
                {
                    writer.WriteLine("{0} {1} {2}", mainWindow.VertexHandler.Verticies.IndexOf(t.A)+1, mainWindow.VertexHandler.Verticies.IndexOf(t.B)+1, t.Weight);
                }
            }
        }
    }
}
