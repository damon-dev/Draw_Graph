using Graph.Objects;

namespace Graph.Algorithms.Actions
{
    class Isolate : Algorithm
    {
        public override void Execute(Vertex start)
        {
            while (start.IncidentEdges.Count > 0)
                MainWindow.EdgeHandler.RemoveEdge(start.IncidentEdges[0]);
            MainWindow.MessageTextBlock.Text = "";
            base.Execute();
        }
    }
}
