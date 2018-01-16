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
using FirstFloor.ModernUI.Presentation;

namespace Mufasa.BackEnd.Designer
{
    class Circle
    {
        public Circle(Canvas realCanvas)
        {
            this. canvas1 = realCanvas;
        }

        private Canvas canvas1 { get; set; }



        /* Creates an ellipse that accidentally is a circle ;)
 */
        Ellipse CreateEllipse(double width, double height, double desiredCenterX, double desiredCenterY)
        {
            Ellipse ellipse = new Ellipse { Width = width, Height = height };
            double left = desiredCenterX - (width / 2);
            double top = desiredCenterY - (height / 2);

            ellipse.Margin = new Thickness(left, top, 0, 0);
            ellipse.StrokeThickness = 2;
            ellipse.Stroke = Brushes.White;

            return ellipse;
        }

        /*
        Returns the coordinates of the point on the perimeter of the circle with the radius r, the center at the "center" point, value = 0/4 max is the right "side" of the circle inscribed in square (tangent point),
        value = 1 / 4max is the bottom of the circle, 1/2 - left 'side', 3/4 max - upper point of circumference of the circle
        */
        Point drawPoint(int r, Point center, double value, double max)
        {
            double tmp = value / max;
            double angle = ((Math.PI * 2) * tmp);
            Point point = new Point();
            point.X = center.X + r * Math.Cos(angle);
            point.Y = center.Y + r * Math.Sin(angle);

            return point;
        }

        /* Main circle method  
        */
        public void drawSeqCircle(Canvas canvas, Point center, int radius, int a_start, int a_size, int a_max, List<ArrowBox> arrows)
        {
            canvas.Children.Clear();

            Ellipse ellipse = CreateEllipse(2 * radius, 2 * radius, center.X, center.Y);
            canvas.Children.Add(ellipse);
            int maxPoint = 36;
            for (int i = 0; i < maxPoint; i++)
            {
                Point p = drawPoint(radius + 10, center, i, maxPoint);
                Line line = new Line();
                line.X1 = p.X;
                line.Y1 = p.Y;
                line.X2 = center.X;
                line.Y2 = center.Y;
                line.StrokeThickness = 2;
                line.Stroke = Brushes.White;
                canvas1.Children.Add(line);
            }

            Ellipse ellipse2 = CreateEllipse(2 * radius - 10, 2 * radius - 10, center.X, center.Y);
            ellipse2.StrokeThickness = 2;
            ellipse2.Stroke = Brushes.Orange;
            ellipse2.Fill = new SolidColorBrush(Color.FromRgb(0x25, 0x25, 0x25)); 
            canvas.Children.Add(ellipse2);

            //scale:
            int pointCounter = 0;
            int part = a_max / maxPoint;
            for (int i = 0; i < a_max; i++)
            {
                Point p = drawPoint(radius - 20, center, i, maxPoint);
                Line line = new Line();
                line.X1 = p.X;
                line.Y1 = p.Y;
                line.X2 = center.X;
                line.Y2 = center.Y;

                if (i % part == 0)
                {
                    p = drawPoint(radius, center, pointCounter * part, maxPoint);
                    pointCounter++;
                    line = new Line();
                    line.X1 = p.X;
                    line.Y1 = p.Y;
                    line.X2 = center.X;
                    line.Y2 = center.Y;
                    line.StrokeThickness = 1;
                    //line.Stroke = Brushes.Black;
                    canvas1.Children.Add(line); ;
                }
            }

            //red circular sector:
            if (a_size > 0)
            {
                for (int i = a_start; i < a_start + a_size; i++)
                {
                    Point p = drawPoint(radius - 20, center, i, a_max);
                    Line line = new Line();
                    line.X1 = p.X;
                    line.Y1 = p.Y;
                    line.X2 = center.X;
                    line.Y2 = center.Y;

                    line.StrokeThickness = 3;
                    line.Stroke = new SolidColorBrush(Color.FromRgb(AppearanceManager.Current.AccentColor.R, AppearanceManager.Current.AccentColor.G, AppearanceManager.Current.AccentColor.B));
                    canvas1.Children.Add(line);
                }
            }

            //middle circle
            ellipse2 = CreateEllipse(1.5 * radius, 1.5 * radius, center.X, center.Y);
            ellipse2.StrokeThickness = 2;
            ellipse2.Stroke = new SolidColorBrush(Color.FromRgb(0x25, 0x25, 0x25));
            ellipse2.Fill = new SolidColorBrush(Color.FromRgb(0x25, 0x25, 0x25));
            canvas.Children.Add(ellipse2);

            //arrows
            if (arrows != null)
            {
                foreach (ArrowBox box in arrows)
                {
                    int start = (int)box.start;
                    int stop = (int)box.stop;

                    int tmp1 = (int)(box.max * 0.02); //hides the start of the arrow - DO NOT TOUCH!!!
                    int tmp2 = (int)(box.max * 0.005); //arrowhead, do not touch

                    if (start < stop)
                    { //clockwise
                        for (int i = start; i < stop - tmp1; i++) //arrow body
                        {
                            Point p = drawPoint(box.radius, center, i, box.max);
                            Rectangle rec = CreateRec(10, 10, p.X, p.Y);
                            rec.StrokeThickness = 2;
                            rec.Stroke = box.color;
                            rec.Fill = box.color;
                            canvas.Children.Add(rec);
                        }

                        for (int i = start - 12; i < start; i++) //hides the start of the arrow
                        {
                            Point p = drawPoint(box.radius + 6, center, i, box.max);
                            Point p2 = drawPoint(box.radius - 6, center, i, box.max);
                            Line line = new Line();
                            line.X1 = p.X;
                            line.Y1 = p.Y;
                            line.X2 = p2.X;
                            line.Y2 = p2.Y;
                            line.StrokeThickness = 2;
                            line.Stroke = new SolidColorBrush(Color.FromRgb(0x25, 0x25, 0x25));
                            canvas1.Children.Add(line);
                        }

                        int counterSize = tmp2;
                        int tmp = 0;
                        for (int i = stop - tmp1; i < stop - (tmp1 - tmp2); i = i + 1) //arrowhead
                        {
                            Point p = drawPoint(box.radius, center, i + tmp, box.max);
                            Rectangle rec = CreateRec(counterSize, counterSize, p.X, p.Y);
                            rec.StrokeThickness = 2;
                            rec.Stroke = box.color;
                            rec.Fill = box.color;
                            canvas.Children.Add(rec);
                            counterSize -= 1;
                            if (counterSize < 0)
                                counterSize = 0;
                            tmp += (int)(box.max * 0.002);
                        }
                    }
                    else //inversely start > stop
                    { //anti-clockwise
                        for (int i = start; i > stop + 40; i--) //arrow body
                        {
                            Point p = drawPoint(box.radius, center, i, box.max);
                            Rectangle rec = CreateRec(10, 10, p.X, p.Y);
                            rec.StrokeThickness = 2;
                            rec.Stroke = box.color;
                            rec.Fill = box.color;
                            canvas.Children.Add(rec);
                        }


                        for (int i = start + 12; i > start; i--) //hides the start of the arrow
                        {
                            Point p = drawPoint(box.radius + 6, center, i, box.max);
                            Point p2 = drawPoint(box.radius - 6, center, i, box.max);
                            Line line = new Line();
                            line.X1 = p.X;
                            line.Y1 = p.Y;
                            line.X2 = p2.X;
                            line.Y2 = p2.Y;
                            line.StrokeThickness = 3;
                            line.Stroke = new SolidColorBrush(Color.FromRgb(0x25, 0x25, 0x25));
                            canvas1.Children.Add(line);
                        }

                        int counterSize = 10;
                        int tmp = 0;
                        for (int i = stop + tmp1; i > stop + (tmp1 - 10); i = i - 1) //arrowhead
                        {
                            Point p = drawPoint(box.radius, center, i - tmp, box.max);
                            Rectangle rec = CreateRec(counterSize, counterSize, p.X, p.Y);
                            rec.StrokeThickness = 2;
                            rec.Stroke = box.color;
                            rec.Fill = box.color;
                            canvas.Children.Add(rec);
                            counterSize -= 1;
                            if (counterSize < 0)
                                counterSize = 0;
                            tmp += (int)(box.max * 0.002);
                        }


                    }
                }
            }

            //captions:
            for (int i = 0; i < maxPoint; i++)
            {
                Point p = drawPoint(radius + 30, new Point(center.X - 8, center.Y - 5), i, maxPoint);
                TextBlock text = new TextBlock();
                double val = (double)i / (double)maxPoint;
                text.Text = "" + (int)(val * a_max);

                text.FontSize = 11;
                Canvas.SetLeft(text, p.X);
                Canvas.SetTop(text, p.Y);
                if (i < maxPoint)
                    canvas1.Children.Add(text);
            }
        }


        Rectangle CreateRec(double width, double height, double desiredCenterX, double desiredCenterY)
        {
            Rectangle rec = new Rectangle { Width = width, Height = height };
            double left = desiredCenterX - (width / 2);
            double top = desiredCenterY - (height / 2);

            rec.Margin = new Thickness(left, top, 0, 0);
            rec.StrokeThickness = 2;
            rec.Stroke = Brushes.Black;

            return rec;
        }
    }

}
