using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Shapes;


namespace Pol2
{
    class Lines
    {
        public System.Windows.Point start, end;
        public double a, b, c;
        public Line ln;
        public int type; //1-вертикальная, 2- горизонтальная, 3-произвольная

        public Lines(double x1,double y1,double x2,double y2)
        {
            
            start.X = x1;
            end.X = x2;
            start.Y = y1;
            end.Y = y2;

            if (x2 == x1) //Вертикальная
            {
                type = 1;
                a = 1;
                b = 0;
                c = -x1;
            }
            else
            {
                if (y2 == y1)
                {
                    type = 2; //горизонтальная
                    a = 0;
                    b = 1;
                    c = -y1;
                }
                else
                {
                    type = 0; //Произвольная
                    a = 1 / (x2 - x1);
                    b = 1 / (y1 - y2);
                    c = (y1 / (y2 - y1)) - (x1 / (x2 - x1));
                }
            }

        }


        //Lines last = new Lines(points0[2].X, points0[2].Y, points0[points0.Count - 1].X, points0[points0.Count - 1].Y); //Построение и проверка последней линии
        //last.BuildLine(3, System.Windows.Media.Brushes.Black);
        //if (isCrossed != true)
        //{
        //    for (int j = 0; j < lines.Count - 1; j++)
        //    {
        //         double s1 = (lines[j].end.X - lines[j].start.X) * (last.start.Y - lines[j].start.Y) - (lines[j].end.Y - lines[j].start.Y) * (last.start.X - lines[j].start.X);
        //        double s2 = (lines[j].end.X - lines[j].start.X) * (last.end.Y - lines[j].start.Y) - (lines[j].end.Y - lines[j].start.Y) * (last.end.X - lines[j].start.X);
        //        double s3 = (last.end.X - last.start.X) * (lines[j].start.Y - last.start.Y) - (last.end.Y - last.start.Y) * (lines[j].start.X - last.start.X);
        //        double s4 = (last.end.X - last.start.X) * (lines[j].end.Y - last.start.Y) - (last.end.Y - last.start.Y) * (lines[j].end.X - last.start.X);

        //        if ((s1 * s2 < 0) && (s3 * s4 < 0))
        //        {
        //            isCrossed = true;
        //            break;
        //        }
        //    }   
        //}
        //Canv.Children.Add(last.ln);
        //lines.Add(last);

        public Line BuildLine(double thick, System.Windows.Media.Brush br)
        {
            ln = new Line();
            ln.X1 = start.X;
            ln.X2 = end.X;
            ln.Y1 = start.Y;
            ln.Y2 = end.Y;
            ln.Stroke = br;
            ln.StrokeThickness = thick;

            return ln;
        }
    }
    
}
