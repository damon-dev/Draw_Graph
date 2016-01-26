using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Graph.Objects;

namespace Graph
{
    /// <summary>
    /// Interaction logic for MatrixWindow.xaml
    /// </summary>
    public partial class MatrixWindow
    {
        private readonly List<Vertex> verticies;
        private TextBlock[,] elements;
        private readonly Dictionary<Vertex, int> nodeDictionary;
        private readonly MainWindow mainWindow;
        private const int MinimumAllowed=15;

        public MatrixWindow(List<Vertex> verticies, string name, MainWindow window)
        {
            InitializeComponent();

            this.verticies = verticies;
            Title = name;
            nodeDictionary=new Dictionary<Vertex, int>();
            mainWindow = window;
            GenerateEmptyMatrix();
        }

        public void GenerateEmptyMatrix()
        {
            if (!IsVisible) return;
            int n = Math.Min(verticies.Count, MinimumAllowed); 
            elements = new TextBlock[n + 1, n + 1];
            MatrixGrid.RowDefinitions.Clear();
            MatrixGrid.ColumnDefinitions.Clear();
            MatrixGrid.Children.Clear();
            nodeDictionary.Clear();
            for (int i = 0; i < n; i++)
                nodeDictionary.Add(verticies[i], i + 1);

            for (int i = 0; i <= n; ++i)
            {
                MatrixGrid.RowDefinitions.Add(new RowDefinition {SharedSizeGroup = "A"});
                MatrixGrid.ColumnDefinitions.Add(new ColumnDefinition {SharedSizeGroup = "A" });
                for (int j = 0; j <= n; ++j)
                {
                    var lab = new TextBlock
                    {
                        HorizontalAlignment=HorizontalAlignment.Center,
                        VerticalAlignment=VerticalAlignment.Center,
                        TextAlignment = TextAlignment.Center,
                        Margin = new Thickness(0,5,0,5),
                        Text="0"
                    };
                    elements[i, j] = lab;

                    var brd = new Border
                    {
                        BorderThickness = new Thickness(0.5),
                        BorderBrush = new SolidColorBrush(Colors.Black),
                        Child = lab
                    };

                    if(i==0||j==0) brd.Background=new SolidColorBrush(Color.FromRgb(168, 255, 249));
                    Grid.SetRow(brd, i);
                    Grid.SetColumn(brd, j);
                    MatrixGrid.Children.Add(brd);
                }
            }

            elements[0, 0].Text = "Nr.";

            for (int i = 1; i <= n; ++i)
                elements[0, i].Text = elements[i, 0].Text = verticies[i - 1].Content;
        }

        public void ResetAdjacencyMatrix()
        {
            if (!IsVisible) return;
            GenerateEmptyMatrix();

            var n = Math.Min(verticies.Count, MinimumAllowed);

            for (var i = 0; i < n; i++)
                foreach (
                    var j in
                        from vertex in verticies[i].AdjacentNodes
                        where nodeDictionary.ContainsKey(vertex)
                        select nodeDictionary[vertex])
                    elements[i + 1, j].Text = "1";
        }

        public void ResetPathMatrix()
        {
            if (!IsVisible) return;
            GenerateEmptyMatrix();

            var n = Math.Min(verticies.Count, MinimumAllowed);

            for (int i = 0; i < n; i++)
                for (int j = 0; j < n; j++)
                    elements[i + 1, j + 1].Text = j == i ? "0" : "∞";
        }

        public void SetElement(Vertex i, Vertex j, string content, bool changeColor=false)
        {
            if (!IsVisible) return;
            if (!(nodeDictionary.ContainsKey(i) && nodeDictionary.ContainsKey(j))) return;
            if (changeColor)
            {
                for (int k = 1; k <= verticies.Count; k++)
                    elements[nodeDictionary[i], k].Foreground = new SolidColorBrush(Colors.Black);
                elements[nodeDictionary[i], nodeDictionary[j]].Foreground = new SolidColorBrush(Colors.Red);
            }
            elements[nodeDictionary[i], nodeDictionary[j]].Text = content;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;

            Hide();

            mainWindow.UpdateTableMenus();
        }
    }
}
