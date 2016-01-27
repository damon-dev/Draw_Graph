using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;
using Graph.Algorithms.Actions;
using Graph.Algorithms.Analyzers;
using Graph.Algorithms.ShortestPath;
using Graph.Algorithms.Traversal;
using Graph.Algorithms.Trees;
using Graph.Objects;

namespace Graph.Algorithms
{
    public class Algorithm
    {
        protected Dictionary<Vertex, int> NodeDictionary;
        protected bool[] Visited;
        protected int NumberOfVerticies;
        private readonly string[] requiresNode;
        private readonly string[] requires2Nodes;
        private readonly string[] requiresWeight;
        private readonly string[] requiresUndirected;
        public static bool SkipAnimationsChecked;
        public static bool IntegersOnlyChecked;
        public static int AnimationSpeed = 500;
        protected static MainWindow MainWindow;
        private Vertex startVertex;
        protected bool IsAnimationSkiping;

        private Algorithm alg;

        public Algorithm(MainWindow window)
        {
            MainWindow = window;
            requiresNode = new[]
            {"BFS", "DFS", "Dijkstra", "Bellman-Ford", "Isolate", "A-path-B", "Post-order", "Prim", "Reposition"};
            requires2Nodes = new[] {"A-path-B"};
            requiresWeight = new[] { "Dijkstra", "Bellman-Ford", "Floyd-Warshall", "A-path-B", "Prim", "Kruskal"};
            requiresUndirected = new[] {"Bipartization", "Do MaxHeap", "Do MinHeap", "Prim", "Kruskal"};
        }

        protected Algorithm()
        {
             
        }

        public virtual void Initialize()
        {
            NumberOfVerticies = MainWindow.VertexHandler.Verticies.Count;
            NodeDictionary = new Dictionary<Vertex, int>();
            for (var i = 0; i < NumberOfVerticies; i++)
            {
                NodeDictionary.Add(MainWindow.VertexHandler.Verticies[i], i);
                MainWindow.VertexHandler.Verticies[i].AdjacentNodes.Sort(
                    (a, b) =>
                    {
                        if (!IntegersOnlyChecked)
                            return String.Compare(a.Content, b.Content, StringComparison.Ordinal);
                        return (int.Parse(a.Content) > int.Parse(b.Content)) ? 1 : -1;
                    });
            }
            Visited = new bool[NumberOfVerticies];
            MainWindow.MessageTextBlock.Text = "";
            IsAnimationSkiping = SkipAnimationsChecked;
        }

        public void ApplyAlgorithm(string algName, Vertex selectedVertex)
        {
            if (requires2Nodes.Contains(algName) && startVertex == null)
            {
                MainWindow.MessageTextBlock.Text = "Please choose the second node!";
                startVertex = selectedVertex;
                startVertex.Body.Background=new SolidColorBrush(Colors.Red);
                return;
            }

            alg = null;

            switch (algName)
            {
                case "BFS":
                    alg = new BFS(Colors.Cyan, Colors.DarkCyan, Colors.LightCyan);
                    break;
                case "DFS":
                    alg = new DFS(Colors.Blue, Colors.Aqua);
                    break;
                case "Post-order":
                    alg = new PostOrder(Colors.Blue, Colors.DodgerBlue, Colors.Aqua);
                    break;
                case "Dijkstra":
                    alg = new Dijkstra(Colors.DarkViolet, Colors.Red, Colors.Gold, Colors.Violet);
                    break;
                case "Bellman-Ford":
                    alg = new BellmanFord(Colors.Firebrick, Colors.Magenta, Colors.OrangeRed);
                    break;
                case "Isolate":
                    alg = new Isolate();
                    break;
                case "A-path-B":
                    alg = new ApathB(startVertex);
                    break;
                case "Prim":
                    alg = new Prim(Colors.DarkGray, Colors.Sienna, Colors.Gold, Colors.ForestGreen);
                    break;
                case "Reposition":
                    alg=new Reposition();
                    break;
            }

            if (alg == null) return;
            MainWindow.ApplicationState = States.AnimationRunning;
            alg.Execute(selectedVertex);
        }

        public void ApplyAlgorithm(string algName)
        {
            if (MainWindow.VertexHandler.Verticies.Count == 0)
            {
                MainWindow.MessageTextBlock.Text = "Graph is missing!";
                return;
            }

            startVertex = null;
            if (!MainWindow.EdgeHandler.EdgesAreWeighted)
                if (requiresWeight.Contains(algName))
                {
                    MainWindow.MessageTextBlock.Text = "Graph shall be weighted!";
                    return;
                }
            if (MainWindow.EdgeHandler.EdgesAreDirected)
                if (requiresUndirected.Contains(algName))
                {
                    MainWindow.MessageTextBlock.Text = "Graph shall be undirected!";
                    return;
                }
            if (requiresNode.Contains(algName))
            {
                MainWindow.ApplicationState = States.WaitingForNodeSelection;
                MainWindow.MessageTextBlock.Text = "Please choose a node!";
                return;
            }

            alg = null;

            switch (algName)
            {
                case "Bipartization":
                    alg = new Bipartitzation(Colors.DarkOrange, Colors.DarkOrchid, Colors.Aqua);
                    break;
                case "Do MaxHeap":
                    alg=new DoMaxHeap();
                    break;
                case "Do MinHeap":
                    alg = new DoMinHeap();
                    break;
                case "Complete":
                    alg = new Complete();
                    break;
                case "Connectivity":
                    alg = new Connectivity();
                    break;
                case "S Connectivity" :
                    alg = new SConnectivity();
                    break;
                case "Floyd-Warshall":
                    alg = new FloydWarshall();
                    break;
                case "Kruskal":
                    alg = new Kruskal(Colors.Sienna, Colors.DarkGray, Colors.Red);
                    break;
            }

            if (alg == null) return;
            MainWindow.ApplicationState=States.AnimationRunning;
            alg.Execute();
        }

        public virtual void Execute(Vertex start)
        {
        }

        public virtual void Execute()
        {
            MainWindow.ApplicationState=States.NormalState;
        }

        public virtual void SkipAnimation()
        {
            if (alg == null) return;
            alg.SkipAnimation();
        }

        public virtual void StopAnimation()
        {
            if (alg == null) return;
            alg.StopAnimation();
        }
    }
}
