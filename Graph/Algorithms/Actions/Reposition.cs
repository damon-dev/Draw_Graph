using Graph.Objects;

namespace Graph.Algorithms.Actions
{
    class Reposition : Algorithm
    {
        public override void Execute(Vertex start)
        {
            foreach (var vertex in MainWindow.VertexHandler.Verticies)
                vertex.Center = start.Center;

            foreach (var edge in MainWindow.EdgeHandler.Edges)
                edge.Update();

            base.Execute();
        }
    }
}
