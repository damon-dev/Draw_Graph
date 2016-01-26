using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using Graph.Algorithms;
using Graph.Handlers;
using Graph.Objects;

namespace Graph
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>

    public enum States
    {
        NormalState,
        EdgeDragging,
        NodeDragging,
        WaitingForNodeSelection,
        AnimationRunning
    }

    public partial class MainWindow
    {
        public MatrixWindow AdjacencyWindow;
        public MatrixWindow PathWindow;

        public States ApplicationState;

        public VertexMethods VertexHandler;
        public EdgeMethods EdgeHandler;
        public IOMethods IOHandler;

        private string selectedAlgorithm;

        private readonly Algorithm algorithm;
        private Dictionary<string, List<ListBoxItem>> algorithmTypeDictionary;
        private Vertex draggedVertex;
        private Edge draggedEdge;

        public MainWindow()
        {
            InitializeComponent();

            algorithm = new Algorithm(this);
            VertexHandler = new VertexMethods(this);
            EdgeHandler = new EdgeMethods(this);
            IOHandler = new IOMethods(this);
            ApplicationState = new States();

            InitialiseAlgorithmDictionary();

            AdjacencyWindow = new MatrixWindow(VertexHandler.Verticies, "Adjacencies", this);
            PathWindow = new MatrixWindow(VertexHandler.Verticies, "Shortest Paths", this);
        }

        #region MouseEvents

        private void canvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            
            Keyboard.ClearFocus();
            switch (ApplicationState)
            {
                case States.NormalState:
                    if (LockGraphCheckBox.IsChecked == true) return;
                    if (!VertexHandler.IsVertex(e.Source))
                    {
                        if (VertexHandler.Verticies.Count == 99) return;
                        var numOfVerticies = VertexHandler.Verticies.Count;

                        VertexHandler.CreateNode(Mouse.GetPosition(MainCanvas),
                            (numOfVerticies + 1).ToString(CultureInfo.InvariantCulture));

                        EnterNodeDraggingState(VertexHandler.Verticies[numOfVerticies]);
                    }
                    else
                    {
                        var vertex = VertexHandler.GetSelectedVertex(e.Source);

                        if (e.ClickCount == 2)
                            vertex.TbIndex.Focus();
                        else
                            EnterNodeDraggingState(vertex);
                    }
                    break;
                case States.WaitingForNodeSelection:
                    if (VertexHandler.IsVertex(e.Source))
                        algorithm.ApplyAlgorithm(selectedAlgorithm, VertexHandler.GetSelectedVertex(e.Source));
                    break;
                case States.AnimationRunning:
                    algorithm.SkipAnimation();
                    break;
            }
        }

        private void canvas_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (ApplicationState != States.NodeDragging) return;
            draggedVertex.Body.ReleaseMouseCapture();
            if (draggedVertex.IsOutside(MainCanvas))
                VertexHandler.RemoveNode(draggedVertex);
            EnterNormalState();
        }

        private void canvas_MouseMove(object sender, MouseEventArgs e)
        {
            switch (ApplicationState)
            {
                case States.EdgeDragging:
                {
                    var mousePosition = Mouse.GetPosition(MainCanvas);
                    if (EdgeHandler.EdgesAreDirected)
                    {
                        var temp = (ArrowLine) draggedEdge.Body;
                        temp.X2 = mousePosition.X;
                        temp.Y2 = mousePosition.Y;
                    }
                    else
                    {
                        var temp = (Line) draggedEdge.Body;
                        temp.X2 = mousePosition.X;
                        temp.Y2 = mousePosition.Y;
                    }

                    break;
                }
                case States.NodeDragging:
                {
                    var mousePosition = Mouse.GetPosition(MainCanvas);
                    draggedVertex.Center = mousePosition;

                    if (draggedVertex.IncidentEdges.Count == 0) return;
                    foreach (var t in draggedVertex.IncidentEdges)
                        t.Update();
                    break;
                }
            }
        }

        private void canvas_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            Keyboard.ClearFocus();
            if (LockGraphCheckBox.IsChecked != true &&
                VertexHandler.IsVertex(e.Source))
            {
                EnterEdgeDraggingState(VertexHandler.GetSelectedVertex(e.Source));
            }
        }

        private void canvas_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (ApplicationState != States.EdgeDragging) return;
            MainCanvas.ReleaseMouseCapture();

            if (!VertexHandler.IsVertex(e.Source))
                EdgeHandler.RemoveEdge(draggedEdge);
            else
            {
                var selectedVertex = VertexHandler.GetSelectedVertex(e.Source);
                if (draggedEdge.A.Equals(selectedVertex))
                    EdgeHandler.RemoveEdge(draggedEdge);
                else if (draggedEdge.A.AdjacentNodes.Contains(selectedVertex))
                {
                    EdgeHandler.RemoveEdge(EdgeHandler.GetEdgeBetween(draggedEdge.A, selectedVertex));
                    EdgeHandler.RemoveEdge(draggedEdge);
                }
                else
                    EdgeHandler.FinalizeEdgeCreation(selectedVertex, draggedEdge);
            }
            EnterNormalState();
        }

        #endregion

        #region ButtonEvents
  
        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (ApplicationState != States.NormalState) return;
                IOHandler.SaveGraph();
        }

        private void Restore_Click(object sender, RoutedEventArgs e)
        {
            if (ApplicationState != States.NormalState) return;
            IOHandler.RestoreGraph();
        }

        private void Clear_Click(object sender, RoutedEventArgs e)
        {
            if (ApplicationState != States.NormalState) return;
            Clear();
        }

        private void Execute_Click(object sender, RoutedEventArgs e)
        {
            Execute();
        }

        private void LbAlgorithms_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            Execute();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            if (ApplicationState == States.EdgeDragging || ApplicationState == States.NodeDragging) return;
            foreach (var node in VertexHandler.Verticies)
                node.Body.Background = VertexHandler.StdColorBrush;
            foreach (var edge in EdgeHandler.Edges)
            {
                edge.Body.Stroke = new SolidColorBrush(Colors.Black);
                edge.Body.StrokeThickness = 1;
            }

            PathWindow.ResetPathMatrix();
            EnterNormalState();
        }

        #endregion

        #region MenuEvents

        #region FILE
        public void Open_Click(object sender, RoutedEventArgs e)
        {
            if (ApplicationState != States.NormalState) return;

            IOHandler.LoadGraph();
            UpdateTableMenus();
        }

        public void SaveAs_Click(object sender, RoutedEventArgs e)
        {
            if (ApplicationState != States.NormalState) return;
            IOHandler.SaveAsGraph();
        }

        public void Exit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        #endregion

        #region OPTIONS
        public void Directed_Click(object sender, RoutedEventArgs e)
        {
            if (ApplicationState != States.NormalState)
            {
                MiDirected.IsChecked = !MiDirected.IsChecked;
                return;
            }
            EdgeHandler.SwitchTypeDirected();
        }

        public void Weighted_Edges_Click(object sender, RoutedEventArgs e)
        {
            if (ApplicationState != States.NormalState)
            {
                MiWeightedEdges.IsChecked = !MiWeightedEdges.IsChecked;
                return;
            }
            EdgeHandler.SwitchTypeWeighted();
            if (!PathWindow.IsVisible) return;
            if(EdgeHandler.EdgesAreWeighted) 
                PathWindow.ResetPathMatrix();
            else
                PathWindow.Hide();
        }

        private void IntegersOnly_Click(object sender, RoutedEventArgs e)
        {
            if (ApplicationState != States.NormalState)
            {
                IntegersOnlyCheckBox.IsChecked = !IntegersOnlyCheckBox.IsChecked;
                return;
            }

            if (IntegersOnlyCheckBox.IsChecked != null)
                Algorithm.IntegersOnlyChecked = (bool)IntegersOnlyCheckBox.IsChecked;

            foreach (var vertex in VertexHandler.Verticies)
                vertex.Content = vertex.Content;
        }

        private void AnimationSpeed_Click(object sender, RoutedEventArgs e)
        {
            var clickedItem = (MenuItem)e.Source;

            switch (clickedItem.Header.ToString())
            {
                case "Very Slow":
                    Algorithm.AnimationSpeed = 1250;
                    break;
                case "Slow":
                    Algorithm.AnimationSpeed = 1000;
                    break;
                case "Normal":
                    Algorithm.AnimationSpeed = 500;
                    break;
                case "Fast":
                    Algorithm.AnimationSpeed = 250;
                    break;
            }
        }

        private void SkipAnimations_Click(object sender, RoutedEventArgs e)
        {
            Algorithm.SkipAnimationsChecked = MiAnimations.IsChecked;
            if (ApplicationState == States.AnimationRunning && Algorithm.SkipAnimationsChecked)
                algorithm.SkipAnimation();
        }

        #endregion

        #region VIEW

        private void AdjacencyMatrix_Click(object sender, RoutedEventArgs e)
        {
            if (!AdjacencyWindow.IsVisible)
            {
                AdjacencyWindow.Show();
                AdjacencyWindow.ResetAdjacencyMatrix();
            }
            else
                AdjacencyWindow.Hide();
        }

        private void PathMatrix_Click(object sender, RoutedEventArgs e)
        {
            if (!PathWindow.IsVisible)
            {
                PathWindow.Show();
                PathWindow.ResetPathMatrix();
            }
            else
                PathWindow.Hide();
        }

        private void MiAlgorithms_Click(object sender, RoutedEventArgs e)
        {
            var clickedItem = (MenuItem) e.Source;

            foreach (var listBoxItem in algorithmTypeDictionary[clickedItem.Header.ToString()])
                listBoxItem.Visibility = clickedItem.IsChecked ? Visibility.Visible : Visibility.Collapsed;
        }

        #endregion

        #endregion

        #region Methods

        private void InitialiseAlgorithmDictionary()
        {
            algorithmTypeDictionary = new Dictionary<string, List<ListBoxItem>>
            {
                {
                    "Actions",
                    new List<ListBoxItem>(new[]
                    {CompleteBoxItem, IsolateBoxItem, BipartizationBoxItem,RepositionBoxItem})
                },
                {
                    "Analyzers",
                    new List<ListBoxItem>(new[] {ConnectivityBoxItem, SConnectivityBoxItem})
                },
                {
                    "Shortest Path",
                    new List<ListBoxItem>(new[] {BellmanBoxItem, DijkstraBoxItem, FloydBoxItem, ApathBoxItem})
                },
                {
                    "Traversal",
                    new List<ListBoxItem>(new[] {BfsBoxItem, DFSBoxItem, PostOrderBoxItem})
                },
                {
                    "Trees",
                    new List<ListBoxItem>(new[] {DoMaxHeapBoxItem, DoMinHeapBoxItem, KruskalBoxItem, PrimBoxItem})
                }
            };
        }

        private void Clear()
        {
            if (ApplicationState != States.NormalState) return;
            while (VertexHandler.Verticies.Count > 0)
                VertexHandler.RemoveNode(VertexHandler.Verticies[0]);
        }

        public void UpdateTables()
        {
            AdjacencyWindow.ResetAdjacencyMatrix();
            if (EdgeHandler.EdgesAreWeighted) PathWindow.ResetPathMatrix();
        }

        public void UpdateTableMenus()
        {
            MiPath.IsEnabled = EdgeHandler.EdgesAreWeighted;
            MiPath.IsChecked = PathWindow.IsVisible;
            MiAdjacency.IsChecked = AdjacencyWindow.IsVisible;
        }

        private void EnterNodeDraggingState(Vertex target)
        {
            if (ApplicationState != States.NormalState) return;
            draggedVertex = target;
            draggedVertex.Body.CaptureMouse();
            ApplicationState = States.NodeDragging;
        }

        private void EnterEdgeDraggingState(Vertex start)
        {
            if (ApplicationState != States.NormalState) return;
            ApplicationState = States.EdgeDragging;
            EdgeHandler.CreateEdge(start);
            draggedEdge = EdgeHandler.Edges[EdgeHandler.Edges.Count - 1];
            Mouse.Capture(MainCanvas, CaptureMode.SubTree);
        }

        public void EnterNormalState()
        {
            if(ApplicationState==States.AnimationRunning) algorithm.StopAnimation();

            MessageTextBlock.Text = "";
            draggedVertex = null;
            draggedEdge = null;
            ApplicationState = States.NormalState;
        }

        private void Execute()
        {
            switch (ApplicationState)
            {
                case States.AnimationRunning:
                    algorithm.SkipAnimation();
                    break;
                case States.WaitingForNodeSelection:
                case States.NormalState:
                    EnterNormalState();
                    foreach (var node in VertexHandler.Verticies)
                        node.Body.Background = VertexHandler.StdColorBrush;
                    foreach (var edge in EdgeHandler.Edges)
                    {
                        edge.Body.Stroke = new SolidColorBrush(Colors.Black);
                        edge.Body.StrokeThickness = 1;
                    }

                    PathWindow.ResetPathMatrix();
                    selectedAlgorithm = ((ListBoxItem) LbAlgorithms.SelectedValue).Content.ToString();
                    algorithm.ApplyAlgorithm(selectedAlgorithm);
                    break;
            }
        }

        #endregion

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}
