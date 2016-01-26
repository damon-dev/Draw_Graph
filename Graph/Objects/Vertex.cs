using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Graph.Algorithms;

namespace Graph.Objects
{
    public class Vertex
    {
        public TextBox TbIndex;
        private string content;
        public Border Body;
        private Point center;
        public readonly int Radius = 20;
        public List<Edge> IncidentEdges;
        public List<Vertex> AdjacentNodes;
        private readonly SolidColorBrush stdColor;

        public string Content
        {
            get
            {
                return content;
            }
            set
            {
                if (value != string.Empty)
                    content = FixIndex(value);
                TbIndex.Text = content;
            }
        }

        public Point Center
        {
            get
            {
                return center;
            }
            set
            {
                center.X = value.X;
                center.Y = value.Y;
                Canvas.SetTop(Body, center.Y - Radius / 2);
                Canvas.SetLeft(Body, center.X - Radius / 2);
            }
        }

        public Vertex(string index)
        {
            TbIndex = new TextBox
            {
                TextAlignment = TextAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                BorderThickness = new Thickness(0),
                Background=Brushes.Transparent,
                IsHitTestVisible = false
            };

            TbIndex.GotKeyboardFocus += (o, args) => { TbIndex.Text = ""; };

            TbIndex.LostKeyboardFocus += (o, args) =>
            {
                Content = TbIndex.Text;
                TbIndex.TextAlignment = TextAlignment.Center;
                TbIndex.HorizontalAlignment = HorizontalAlignment.Center;
                TbIndex.VerticalAlignment = VerticalAlignment.Center;
            };

            TbIndex.KeyDown += (o, args) =>
            {
                if (args.Key != Key.Return) return;
                args.Handled = true;
                Keyboard.ClearFocus();
            };

            Content = index;
            IncidentEdges = new List<Edge>();
            AdjacentNodes = new List<Vertex>();
            stdColor = new SolidColorBrush(Colors.Chartreuse);

            CreateVertex();   
        }

        private void CreateVertex()
        {
            Body = new Border
            {
                Width = Radius,
                Height = Radius,
                CornerRadius = new CornerRadius(50),
                Background = stdColor,
                BorderBrush = Brushes.Black,
                BorderThickness = new Thickness(1),
                Child = TbIndex
            };
        }

        public bool IsOutside(Canvas refCanvas)
        {
            if (center.X < 0 || center.X > refCanvas.ActualWidth) return true;
            return center.Y < 0 || center.Y > refCanvas.ActualHeight;
        }

        private string FixIndex(string index)
        {
            index = Regex.Replace(index, " ", "");
            if (index.Length > 2) 
                index=index.Remove(2);
            if (Algorithm.IntegersOnlyChecked)
            {
                int temp;
                int.TryParse(index, out temp);
                index = temp.ToString(CultureInfo.InvariantCulture);
            }
            return index;
        }
    }
}
