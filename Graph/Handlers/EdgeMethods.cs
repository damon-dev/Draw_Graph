using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Input;
using Graph.Objects;

namespace Graph.Handlers
{
    public class EdgeMethods
    {
        private readonly MainWindow mainWindow;
        public List<Edge> Edges;
        public bool EdgesAreDirected;
        public bool EdgesAreWeighted;
        public EdgeMethods(MainWindow window)
        {
            mainWindow = window;
            Edges = new List<Edge>();
        }
        public void CreateEdge(Vertex source)
        {
            var newEdge = new Edge(source, EdgesAreDirected, EdgesAreWeighted);
            Edges.Add(newEdge);
            Panel.SetZIndex(newEdge.Body, -1);
            mainWindow.MainCanvas.Children.Add(newEdge.Body);
        }

        public void FinalizeEdgeCreation(Vertex finish, Edge draggedEdge)
        {
            draggedEdge.B = finish;
            finish.IncidentEdges.Add(draggedEdge);
            draggedEdge.A.IncidentEdges.Add(draggedEdge);

            if (EdgesAreWeighted)
            {
                draggedEdge.CreateWeightBlock();
                mainWindow.MainCanvas.Children.Add(draggedEdge.TbWeight);
                Keyboard.Focus(draggedEdge.TbWeight);
            }

            if (EdgesAreDirected)
            {
                draggedEdge.A.AdjacentNodes.Add(finish);
                mainWindow.AdjacencyWindow.SetElement(draggedEdge.A, finish, "1");
            }
            else
            {
                finish.AdjacentNodes.Add(draggedEdge.A);
                draggedEdge.A.AdjacentNodes.Add(finish);
                mainWindow.AdjacencyWindow.SetElement(draggedEdge.A, finish, "1");
                mainWindow.AdjacencyWindow.SetElement(finish, draggedEdge.A, "1");
            }

            draggedEdge.Update();
        }

        public void CreateEdge(Vertex start, Vertex finish)
        {
            var edge = new Edge(start, finish, EdgesAreDirected, EdgesAreWeighted);

            if (EdgesAreDirected)
            {
                if (start.AdjacentNodes.Contains(finish)) return;
                start.AdjacentNodes.Add(finish);
                mainWindow.AdjacencyWindow.SetElement(start,finish,"1");
            }
            else
            {
                if (start.AdjacentNodes.Contains(finish)||finish.AdjacentNodes.Contains(start)) return;
                start.AdjacentNodes.Add(finish);
                finish.AdjacentNodes.Add(start);
                mainWindow.AdjacencyWindow.SetElement(start, finish, "1");
                mainWindow.AdjacencyWindow.SetElement(finish, start, "1");
            }

            start.IncidentEdges.Add(edge);
            finish.IncidentEdges.Add(edge);

            Edges.Add(edge);
            Panel.SetZIndex(edge.Body, -1);
            mainWindow.MainCanvas.Children.Add(edge.Body);
            
            if (EdgesAreWeighted)
                mainWindow.MainCanvas.Children.Add(edge.TbWeight);

            edge.Update();
        }

        public void RemoveEdge(Edge edge)
        {
            Edges.Remove(edge);
            edge.A.IncidentEdges.Remove(edge);
            edge.B.IncidentEdges.Remove(edge);
            if (EdgesAreDirected)
            {
                edge.A.AdjacentNodes.Remove(edge.B);
                mainWindow.AdjacencyWindow.SetElement(edge.A, edge.B, "0");
            }
            else
            {
                edge.A.AdjacentNodes.Remove(edge.B);
                edge.B.AdjacentNodes.Remove(edge.A);
                mainWindow.AdjacencyWindow.SetElement(edge.A, edge.B, "0");
                mainWindow.AdjacencyWindow.SetElement(edge.B, edge.A, "0");
            }
            mainWindow.MainCanvas.Children.Remove(edge.Body);
            if (EdgesAreWeighted)
                mainWindow.MainCanvas.Children.Remove(edge.TbWeight);
        }

        public Edge GetEdgeBetween(Vertex a, Vertex b)
        {
            return EdgesAreDirected ? a.IncidentEdges.FirstOrDefault(edge => edge.B.Equals(b)) : a.IncidentEdges.FirstOrDefault(edge => edge.B.Equals(b) || edge.A.Equals(b));
        }

        public void SwitchTypeDirected()
        {
            var l = Edges.Count;
            var cEdges = new int[l, 2];

            for (int index = 0; index < l; index++)
            {
                var edge = Edges[index];
                cEdges[index, 0] = mainWindow.VertexHandler.Verticies.IndexOf(edge.A);
                cEdges[index, 1] = mainWindow.VertexHandler.Verticies.IndexOf(edge.B);
            }

            while (Edges.Count > 0)
                RemoveEdge(Edges[0]);

            EdgesAreDirected = !EdgesAreDirected;
            for (int index = 0; index < l; index++)
                CreateEdge(mainWindow.VertexHandler.Verticies[cEdges[index, 0]],
                    mainWindow.VertexHandler.Verticies[cEdges[index, 1]]);

            mainWindow.MiDirected.IsChecked = EdgesAreDirected;
        }

        public void SwitchTypeWeighted()
        {
            EdgesAreWeighted = !EdgesAreWeighted;

            foreach (var edge in Edges)
            {
                edge.weighted = EdgesAreWeighted;

                if (EdgesAreWeighted)
                {
                    edge.CreateWeightBlock();
                    mainWindow.MainCanvas.Children.Add(edge.TbWeight);
                    edge.Update();
                }
                else
                    mainWindow.MainCanvas.Children.Remove(edge.TbWeight);
            }

            mainWindow.MiWeightedEdges.IsChecked = EdgesAreWeighted;
            mainWindow.MiPath.IsEnabled = EdgesAreWeighted;
        }

        
    }
}
