using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Graph.Objects
{
    public class Edge
    {
        public Shape Body;
        public Vertex A, B;
        private bool oriented;
        public bool weighted;
        public TextBox TbWeight;
        private double weight;
        private Point centerPoint;
        private Point weightPosition;
        private double offsetX = 12.47/2, offsetY = 17.96/2;

        public double Weight
        {
            get
            {
                return weight;
            }
            set
            {
                weight = value;
                TbWeight.Text = value.ToString(CultureInfo.InvariantCulture);
            }
        }

        public Edge(Vertex nodeA, bool isOriented, bool isWeighted)
        {
            A = nodeA;
            B = nodeA;
            oriented = isOriented;
            weighted = isWeighted;
            if (oriented)
                CreateArrow();
            else
                CreateEdge();
        }

        public Edge(Vertex nodeA, Vertex nodeB, bool isOriented, bool isWeighted)
        {
            A = nodeA;
            B = nodeB;
            oriented = isOriented;
            weighted = isWeighted;
            centerPoint = new Point((A.Center.X + B.Center.X) / 2, (A.Center.Y + B.Center.Y) / 2);

            if(weighted)
                CreateWeightBlock();
            if (oriented)
                CreateArrow();
            else
                CreateEdge();
        }

        private void CreateEdge()
        {
            Body = new Line
            {
                X1 = A.Center.X,
                Y1 = A.Center.Y,
                X2 = B.Center.X,
                Y2 = B.Center.Y,
                StrokeThickness = 1,
                Stroke = Brushes.Black,
                IsHitTestVisible = false
            };
        }

        private void CreateArrow()
        {
            Body = new ArrowLine();

            var temp = (ArrowLine) Body;

            temp.X1 = A.Center.X;
            temp.Y1 = A.Center.Y;
            temp.X2 = B.Center.X;
            temp.Y2 = B.Center.Y;
            temp.Stroke = Brushes.Black;
            temp.StrokeThickness = 1;
            temp.IsHitTestVisible = false;
        }

        public void CreateWeightBlock()
        {
            TbWeight = new TextBox {Text = weight.ToString(CultureInfo.InvariantCulture)};
            TbWeight.KeyDown += TbWeight_OnKeyPressed;
            TbWeight.LostKeyboardFocus += TbWeight_LostKeyboardFocus;
            TbWeight.GotKeyboardFocus += (sender, args) =>
            {
                if (Mouse.LeftButton != MouseButtonState.Pressed)
                    TbWeight.SelectAll();
            };
        }

        public void Update()
        {
            centerPoint = new Point((A.Center.X + B.Center.X) / 2, (A.Center.Y + B.Center.Y) / 2);

            if (oriented)
            {
                var temp = (ArrowLine) Body;
                var a = new Vector(A.Center.X, A.Center.Y);
                var b = new Vector(B.Center.X, B.Center.Y);
                var dir = Vector.Subtract(a, b);
                dir.Normalize();
                var c = Vector.Add(b, Vector.Multiply(dir, B.Radius));
                temp.X1 = A.Center.X;
                temp.Y1 = A.Center.Y;
                temp.X2 = c.X;
                temp.Y2 = c.Y;
                if (!weighted) return;
                c = Vector.Add(b, Vector.Multiply(dir, Vector.Subtract(a, b).Length*0.4));
                weightPosition = new Point(c.X - offsetX, c.Y - offsetY);
                Canvas.SetLeft(TbWeight, weightPosition.X);
                Canvas.SetTop(TbWeight, weightPosition.Y);
            }
            else
            {
                var temp = (Line) Body;
                temp.X1 = A.Center.X;
                temp.Y1 = A.Center.Y;
                temp.X2 = B.Center.X;
                temp.Y2 = B.Center.Y;
                if(!weighted) return;
                weightPosition = new Point(centerPoint.X - offsetX, centerPoint.Y - offsetY);
                Canvas.SetLeft(TbWeight, weightPosition.X);
                Canvas.SetTop(TbWeight, weightPosition.Y);
            }
        }

        void TbWeight_OnKeyPressed(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Return) return;
            e.Handled = true;
            Keyboard.ClearFocus();
        }

        private void TbWeight_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            double input;
            double.TryParse(TbWeight.Text, out input);
            Weight = input;
            offsetX = TbWeight.ActualWidth / 2;
            offsetY = TbWeight.ActualHeight / 2;
        }
    }
}
