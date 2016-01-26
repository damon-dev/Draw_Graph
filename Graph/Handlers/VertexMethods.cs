using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Graph.Objects;

namespace Graph.Handlers
{
    public class VertexMethods
    {
        private readonly MainWindow mainWindow;
        public List<Vertex> Verticies;
        public SolidColorBrush StdColorBrush;

        public VertexMethods(MainWindow window)
        {
            mainWindow = window;
            Verticies = new List<Vertex>();
            StdColorBrush = new SolidColorBrush(Colors.Chartreuse);
        }

        public void CreateNode(Point position, string index)
        {
            var node = new Vertex(index);

            Verticies.Add(node);
            node.Center = position;
            
            mainWindow.MainCanvas.Children.Add(node.Body);
            mainWindow.UpdateTables();
        }

        public void RemoveNode(Vertex poorVertex)
        {
            Isolate(poorVertex);

            Verticies.Remove(poorVertex);

            mainWindow.MainCanvas.Children.Remove(poorVertex.Body);
            mainWindow.UpdateTables();
        }

        public Vertex GetSelectedVertex(object source)
        {
            var brd = (Border)source;
            return Verticies.First(t => t.Body.Equals(brd));
        }

        public bool IsVertex(object source)
        {
            return source.GetType() == typeof (Border);
        }

        public void Isolate(Vertex target)
        {
            while (target.IncidentEdges.Count > 0)
                mainWindow.EdgeHandler.RemoveEdge(target.IncidentEdges[0]);   
        }

        public double GetWeightBetween(Vertex a, Vertex b)
        {
            if (!mainWindow.EdgeHandler.EdgesAreDirected)
                foreach (var edge in a.IncidentEdges.Where(edge => edge.B.Equals(b) || edge.A.Equals(b)))
                    return edge.Weight;
            else
                foreach (var edge in a.IncidentEdges.Where(edge => edge.B.Equals(b)))
                    return edge.Weight;
            return double.PositiveInfinity;
        }
    }
}
