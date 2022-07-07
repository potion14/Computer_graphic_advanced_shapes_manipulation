using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Grafika7
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            points = new List<Point>();
            shapes = new List<List<Line>>();
            shapes.Add(new List<Line>());
        }

        public List<Point> points;
        public List<List<Line>> shapes;
        public bool isSelected = false, isPressed = false;
        public List<Line> currentShape;
        public bool rotate = false, move = true, scale = false;
        public Point startPoint;

        private void AddPoint(object sender, EventArgs e)
        {
            if (int.TryParse(XTextBox.Text, out int x) && int.TryParse(XTextBox.Text, out int y) && isSelected == false)
            {
                Point point = new Point
                {
                    X = Convert.ToInt32(XTextBox.Text),
                    Y = Convert.ToInt32(YTextBox.Text)
                };
                points.Add(point);
                if (points.Count >= 2)
                {
                    Line line = new Line
                    {
                        X1 = points[^2].X,
                        Y1 = points[^2].Y,
                        X2 = points[^1].X,
                        Y2 = points[^1].Y,
                        Stroke = Brushes.Black,
                        StrokeThickness = 3
                    };
                    shapes[^1].Add(line);
                    canvas.Children.Add(line);
                }
                XTextBox.Text = "0";
                YTextBox.Text = "0";
            }
        }

        private void CanvasLBD(object sender, MouseButtonEventArgs e)
        {
            Point point = new Point();
            if (e.OriginalSource is Canvas)
            {
                isSelected = false;
                point = Mouse.GetPosition(canvas);
                points.Add(point);
            }
            if (points.Count >= 2)
            {
                Line line = new Line
                {
                    X1 = points[points.Count - 2].X,
                    Y1 = points[points.Count - 2].Y,
                    X2 = points[points.Count - 1].X,
                    Y2 = points[points.Count - 1].Y,
                    Stroke = Brushes.Black,
                    StrokeThickness = 7
                };
                shapes[^1].Add(line);
                canvas.Children.Add(line);
            }
            else if(e.OriginalSource is Line && points.Count == 0)
            {
                startPoint = e.GetPosition(canvas);
                Line shape = (Line)e.OriginalSource;
                if (currentShape != null)
                {
                    foreach (var line in currentShape)
                    {
                        line.Stroke = Brushes.Black;
                    }
                }
                foreach (var s in shapes)
                {
                    if (s.Contains(shape))
                    {
                        currentShape = s;
                        foreach (var line in s)
                        {
                            line.Stroke = Brushes.Red;
                        }
                        break;
                    }
                }
                isSelected = true;
                isPressed = true;
            }
        }

        private void CanvasRBD(object sender, MouseButtonEventArgs e)
        {
            Line line = new Line
            {
                X1 = points[points.Count - 1].X,
                Y1 = points[points.Count - 1].Y,
                X2 = points[0].X,
                Y2 = points[0].Y,
                Stroke = Brushes.Black,
                StrokeThickness = 7
            };
            shapes[^1].Add(line);
            canvas.Children.Add(line);
            points.Clear();
            shapes.Add(new List<Line>());
        }

        private void AddVector(object sender, EventArgs e)
        {
            if (int.TryParse(XTextBox.Text, out int x) && int.TryParse(XTextBox.Text, out int y))
            {
                int VX = Convert.ToInt32(XTextBox.Text);
                int VY = Convert.ToInt32(YTextBox.Text);
                if (isSelected)
                {
                    foreach(Line line in currentShape)
                    {
                        line.X1 += VX;
                        line.X2 += VX;
                        line.Y1 += VY;
                        line.Y2 += VY;
                    }
                }
                XTextBox.Text = "0";
                YTextBox.Text = "0";
            }
        }

        private void Rotate_button(object sender, EventArgs e)
        {
            Rotate(currentShape, RadiusTextBox.Text);
            RadiusTextBox.Text = "0";
        }

        private void Rotate(List<Line> selectedShape, string radiusTB)
        {
            if (int.TryParse(RadiusTextBox.Text, out int x))
            {
                double radius = Convert.ToInt32(radiusTB) * Math.PI / 180;
                double SX = 0, SY = 0;
                if (isSelected)
                {
                    foreach (Line line in selectedShape)
                    {
                        SX += line.X1;
                        SY += line.Y1;
                    }
                    SX /= selectedShape.Count;
                    SY /= selectedShape.Count;
                    foreach (Line line in selectedShape)
                    {
                        double X1 = line.X1, X2 = line.X2, Y1 = line.Y1, Y2 = line.Y2;
                        line.X1 = SX + (X1 - SX) * Math.Cos(radius) - (Y1 - SY) * Math.Sin(radius);
                        line.X2 = SX + (X2 - SX) * Math.Cos(radius) - (Y2 - SY) * Math.Sin(radius);
                        line.Y1 = SY + (X1 - SX) * Math.Sin(radius) + (Y1 - SY) * Math.Cos(radius);
                        line.Y2 = SY + (X2 - SX) * Math.Sin(radius) + (Y2 - SY) * Math.Cos(radius);
                    }
                }
            }
        }

        private void Scale_button(object sender, EventArgs e)
        {
            if (int.TryParse(XTextBox.Text, out int k) && int.TryParse(XTextBox.Text, out int l) && double.TryParse(ScaleTextBox.Text, out double z))
            {
                double X = Convert.ToDouble(XTextBox.Text);
                double Y = Convert.ToDouble(YTextBox.Text);
                double scale = Convert.ToDouble(ScaleTextBox.Text);
                Scale(currentShape, X, Y, scale);
            }
        }

        private void Scale(List<Line> selectedShape, double x, double y, double Scale)
        {
            double X = x;
            double Y = y;
            double scale = Scale;

            if (isSelected)
            {
                foreach (Line line in selectedShape)
                {
                    double X1 = line.X1, X2 = line.X2, Y1 = line.Y1, Y2 = line.Y2;
                    line.X1 = X + (X1 - X) * scale;
                    line.X2 = X + (X2 - X) * scale;
                    line.Y1 = Y + (Y1 - Y) * scale;
                    line.Y2 = Y + (Y2 - Y) * scale;
                }
            }
            XTextBox.Text = "0";
            YTextBox.Text = "0";
            ScaleTextBox.Text = "1";
        }

        private void CanvasMM(object sender, MouseEventArgs e)
        {
            Point point = e.GetPosition(canvas);
            if(isSelected && isPressed)
            {
                if (move)
                {
                    foreach (Line line in currentShape)
                    {
                        double X1 = line.X1, X2 = line.X2, Y1 = line.Y1, Y2 = line.Y2;
                        line.X1 += (point.X - startPoint.X);
                        line.X2 += (point.X - startPoint.X);
                        line.Y1 += (point.Y - startPoint.Y);
                        line.Y2 += (point.Y - startPoint.Y);
                    }
                    startPoint = point;
                }
                else if (rotate)
                {
                    Rotate(currentShape, Convert.ToString(point.X - startPoint.X));
                    startPoint = point;
                }
                else if (scale)
                {
                    double SX = 0, SY = 0;
                    foreach (Line line in currentShape)
                    {
                        SX += line.X1;
                        SY += line.Y1;
                    }
                    SX /= currentShape.Count;
                    SY /= currentShape.Count;
                    Scale(currentShape, SX, SY, 1 + (point.X - startPoint.X)/100);
                    startPoint = point;
                }
            }
        }

        private void Button_Clear(object sender, RoutedEventArgs e)
        {
            canvas.Children.Clear();
            //shapes.Clear();
            //points.Clear();
            //currentShape.Clear();
            isPressed = false;
            isSelected = false;
        }

        private void Button_Move(object sender, RoutedEventArgs e)
        {
            move = true;
            rotate = false;
            scale = false;
            current_option.Content = "current: przemieszczanie";
        }

        private void Button_Rotate(object sender, RoutedEventArgs e)
        {
            move = false;
            rotate = true;
            scale = false;
            current_option.Content = "current: rotacja";
        }

        private void Button_Scale(object sender, RoutedEventArgs e)
        {
            move = false;
            rotate = false;
            scale = true;
            current_option.Content = "current: skalowanie";
        }

        private void CanvasLBU(object sender, MouseButtonEventArgs e)
        {
            isPressed = false;
        }
    }
}
