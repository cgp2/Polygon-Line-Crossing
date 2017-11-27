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

namespace Pol2
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private int N; //Количество точек
        private List<Lines> lines = new List<Lines>();
        private PointCollection points0 = new PointCollection();
        private PointCollection points = new PointCollection();
        private List<Point> crossing = new List<Point>();
        private double size;
        private double size0;
        private double coff;
        private bool isCrossed = false;

        public MainWindow()
        {
            InitializeComponent();

            size = Canv.Height; //определяем размеры окна
            size0 = size / 2;
        }

        private void bildPolygon_Click(object sender, RoutedEventArgs e)
        {
            int k = 0;
            double max = 0;
            Canv.Children.Clear();
            points.Clear();
            points0.Clear();
            lines.Clear();
            crossing.Clear();
            isCrossed = false;

            Lines X = new Lines(0, size0, size, size0);    // Координатные оси
            X.BuildLine(2, System.Windows.Media.Brushes.Black);
            Canv.Children.Add(X.ln);

            Lines Y = new Lines(size0, 0, size0, size);
            Y.BuildLine(2, System.Windows.Media.Brushes.Black);
            Canv.Children.Add(Y.ln);


            for (int i = 0; i <= size; i = i + (int)size / 10) // Засечки
            {
                Lines zX = new Lines(i, size0 - 6, i, size0 + 6);
                zX.BuildLine(2, System.Windows.Media.Brushes.Black);
                Canv.Children.Add(zX.ln);
            }

            for (int i = 0; i <= size; i = i + (int)size / 10)
            {
                Lines zY = new Lines(size0 - 6, i, size0 + 6, i);
                zY.BuildLine(2, System.Windows.Media.Brushes.Black);
                Canv.Children.Add(zY.ln);
            }

            string s = LineCoords.Text;

            if (s.Count(c => c == ';') == 0)
            {
                MessageBox.Show("Ошибка синтаксиса!");
                return;
            }

            try
            {
                for (int i = 0; i < 2; i++) //Считывание координат линии
                {
                    string s1 = s.Substring(0, s.IndexOf(";") + 1);
                    double x = double.Parse(s1.Substring(0, s1.IndexOf(",")));
                    s1 = s1.Remove(0, s1.IndexOf(",") + 1);
                    double y = double.Parse(s1.Substring(0, s1.IndexOf(";")));
                    s = s.Remove(0, s.IndexOf(";") + 1);

                    points.Add(new Point(x, y));
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Ошибка синтаксиса!");
                return;
            }

            s = PolygonCoords.Text;
            N = s.Count(c => c == ';');

            if (N == 0)
            {
                MessageBox.Show("Ошибка синтаксиса!");
                return;
            }

            try
            {
                for (int i = 0; i < N; i++) //Считывание координат многоугольника
                {

                    string s1 = s.Substring(0, s.IndexOf(";") + 1);
                    double x = double.Parse(s1.Substring(0, s1.IndexOf(",")));
                    s1 = s1.Remove(0, s1.IndexOf(",") + 1);
                    double y = double.Parse(s1.Substring(0, s1.IndexOf(";")));
                    s = s.Remove(0, s.IndexOf(";") + 1);

                    points.Add(new Point(x, y));
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Ошибка ситаксиса!");
                return;
            }

            for (int i = 0; i < N + 2; i++) //Поиск максимальной координаты
            {
                if (points[i].X > max)
                    max = points[i].X;
                if (points[i].Y > max)
                    max = points[i].Y;
            }

            coff = max;

            if (coff % 10 != 0)
                coff = 10 * Math.Ceiling(Math.Round(max) / 10); //Поиск коофициента мастштаба

            for (int i = 0; i < N + 2; i++)
            {
                double x = size0 + (size0 / coff) * points[i].X;  //Изменения координат согласно масштабу 
                double y = size0 - (size0 / coff) * points[i].Y;

                points0.Add(new Point(x, y));
            }

            Lines ln0 = new Lines(points0[0].X, points0[0].Y, points0[1].X, points0[1].Y); //Построение прямой
            if (ln0.start == ln0.end)
            {
                MessageBox.Show("Ошибка задания прямой");
                return;
            }
            ln0.BuildLine(3, System.Windows.Media.Brushes.Red);

            Lines last = new Lines(points0[2].X, points0[2].Y, points0[points0.Count - 1].X, points0[points0.Count - 1].Y); //Построение и проверка последней линии
            last.BuildLine(3, System.Windows.Media.Brushes.Black);
            Canv.Children.Add(last.ln);
            lines.Add(last);

            for (int i = 2; i < N + 1; i++) //Проверка прямых многоугольника на пересечение и построение многокгольника
            {
                Lines line = new Lines(points0[i].X, points0[i].Y, points0[i + 1].X, points0[i + 1].Y);
                line.BuildLine(3, System.Windows.Media.Brushes.Black);

                for (int j = 0; j < lines.Count - 1; j++)
                {
                    double s1 = (lines[j].end.X - lines[j].start.X) * (line.start.Y - lines[j].start.Y) - (lines[j].end.Y - lines[j].start.Y) * (line.start.X - lines[j].start.X);
                    double s2 = (lines[j].end.X - lines[j].start.X) * (line.end.Y - lines[j].start.Y) - (lines[j].end.Y - lines[j].start.Y) * (line.end.X - lines[j].start.X);
                    double s3 = (line.end.X - line.start.X) * (lines[j].start.Y - line.start.Y) - (line.end.Y - line.start.Y) * (lines[j].start.X - line.start.X);
                    double s4 = (line.end.X - line.start.X) * (lines[j].end.Y - line.start.Y) - (line.end.Y - line.start.Y) * (lines[j].end.X - line.start.X);
                    /// 120 120 120 280
                    if ((s1 * s2 < 0) && (s3 * s4 < 0))
                    {
                        isCrossed = true;
                        break;
                    }
                }
                lines.Add(line);
                Canv.Children.Add(line.ln);
            }

            //Lines last = new Lines(points0[2].X, points0[2].Y, points0[points0.Count - 1].X, points0[points0.Count - 1].Y); //Построение и проверка последней линии
            //last.BuildLine(3, System.Windows.Media.Brushes.Black);
            //for (int j = 0; j < lines.Count - 1; j++)
            //{
            //    double s1 = (lines[j].end.X - lines[j].start.X) * (last.start.Y - lines[j].start.Y) - (lines[j].end.Y - lines[j].start.Y) * (last.start.X - lines[j].start.X);
            //    double s2 = (lines[j].end.X - lines[j].start.X) * (last.end.Y - lines[j].start.Y) - (lines[j].end.Y - lines[j].start.Y) * (last.end.X - lines[j].start.X);
            //    double s3 = (last.end.X - last.start.X) * (lines[j].start.Y - last.start.Y) - (last.end.Y - last.start.Y) * (lines[j].start.X - last.start.X);
            //    double s4 = (last.end.X - last.start.X) * (lines[j].end.Y - last.start.Y) - (last.end.Y - last.start.Y) * (lines[j].end.X - last.start.X);

            //    if ((s1 * s2 < 0) && (s3 * s4 < 0))
            //    {
            //        isCrossed = true;
            //        break;
            //    }
            //}
            //Canv.Children.Add(last.ln);
            //lines.Add(last);


            /////////////////////////////////////////////////////////
            for (int i = 0; i<lines.Count; i++) //Поиск точек пересечения с секущей прямой
            {
                double x0, y0;

                if(lines[i].type == 1) //Вертикальные прямая
                {
                    if(ln0.type == 1 && ln0.c == lines[i].c) //Cовпадение прямых
                    {
                            x0 = lines[i].start.X;
                            y0 = ln0.start.Y > ln0.end.Y ? Math.Min(lines[i].start.Y, lines[i].end.Y) : Math.Max(lines[i].start.Y, lines[i].end.Y);
                    }
                    else
                    {
                        if (ln0.type == 1 && ln0.c != lines[i].c)
                            continue;

                        x0 = -lines[i].c;
                        y0 = Math.Round(-(ln0.a * x0 + ln0.c) / ln0.b);
                    }

                    if(lines[i].start.Y <= lines[i].end.X)//Возрастающая по y вертикальаня прямая
                    {
                        if ((y0 >= lines[i].start.Y) && (y0 <= lines[i].end.Y))
                        {
                            crossing.Add(new Point(x0, y0));
                        }
                        else
                            continue;
                    }
                    else//Убывающая по y вертикальная прямая
                    {
                        if((y0 <= lines[i].start.Y) && (y0 >= lines[i].end.Y))
                        {
                            crossing.Add(new Point(x0, y0));
                        }
                        else
                            continue;
                    }
                }
                if(lines[i].type == 2) //Горизонтальная прямая 
                {
                    if(ln0.type == 2 && ln0.c == lines[i].c) //Совпадения прямых
                    {
                        x0 = ln0.start.X > ln0.end.X ? Math.Min(lines[i].start.X, lines[i].end.X) : Math.Max(lines[i].start.X, lines[i].end.X);
                        y0 = lines[i].start.Y;
                    }
                    else
                    {
                        if (ln0.type == 2 && ln0.c != lines[i].c)
                            continue;

                        if(ln0.type == 1)
                        {
                            y0 = -lines[i].c;
                            x0 = -ln0.c;
                        }
                        else
                        {
                            y0 = -lines[i].c;
                            x0 = Math.Round(-(ln0.b * y0 + ln0.c ) / ln0.a);
                        }
                    }

                    if (lines[i].start.X < lines[i].end.X)//Возрастающая по X горизонтальная прямая
                    {
                        if ((x0 >= lines[i].start.X) && (lines[i].end.X >= x0))
                        {
                            crossing.Add(new Point(x0, y0));
                        }
                        else
                            continue;
                    }
                    else//Убывающая по X прямая
                    {
                        if((x0 <= lines[i].start.X) && (lines[i].end.X <= x0))
                        {
                            crossing.Add(new Point(x0, y0));
                        }
                        else
                            continue;
                    }
                }
                if(lines[i].type == 0) //Произвольнaя прямая
                {
                    if ((ln0.type == 0) && (ln0.a == lines[i].a) && (ln0.b == lines[i].b) && (ln0.c == lines[i].c))//Совпадение прямых
                    {
                        y0 = ln0.start.Y > ln0.end.Y ? Math.Min(lines[i].start.Y, lines[i].end.Y) : Math.Max(lines[i].start.Y, lines[i].end.Y);
                        x0 = ln0.start.X > ln0.end.X ? Math.Min(lines[i].start.X, lines[i].end.X) : Math.Max(lines[i].start.X, lines[i].end.X);
                    }
                    if(ln0.type == 1)
                    {
                        x0 = -ln0.c;
                        y0 = Math.Round((-lines[i].a * x0 - lines[i].c) / lines[i].b);
                    }
                    else
                    {
                        if (ln0.type == 2)
                        {
                            y0 = -ln0.c;
                            x0 = Math.Round(-(lines[i].b * y0 + lines[i].c) / lines[i].a);
                        }
                        else
                        {
                            x0 = Math.Round((ln0.c * lines[i].b - lines[i].c * ln0.b) / (lines[i].a * ln0.b - ln0.a * lines[i].b));
                            y0 = Math.Round(-(ln0.c + ln0.a * x0) / ln0.b);
                        }
                    }

                    if(lines[i].start.X < lines[i].end.X) //Возрастающая по X произвольная прямая
                    {
                        if ((x0 >= lines[i].start.X) && (lines[i].end.X >= x0))
                        {
                            crossing.Add(new Point(x0, y0));
                        }
                        else
                            continue;
                    }
                    else//Убывающая по X произвольная прямая
                    {
                        if ((x0 <= lines[i].start.X) && (lines[i].end.X <= x0))
                        {
                            crossing.Add(new Point(x0, y0));
                        }
                        else
                            continue;
                    }
                }
            }
            /////////////////////////////////////////
            
            crossing.Add(ln0.start);
            crossing.Add(ln0.end);

            if (ln0.type == 1) //Cортировка массива
                crossing.Sort((a, b) => a.Y.CompareTo(b.Y));
            else
                crossing.Sort((a, b) => a.X.CompareTo(b.X));

            for (int i = 0; i < crossing.Count-1; i++) //Устранение одинаковых точек
            {
                if (i == 0 || i == crossing.Count-2)
                {
                    if (crossing[i] == crossing[i + 1])
                    {
                        crossing.RemoveAt(i + 1);
                        k++;
                    }
                }
                else
                {
                    if (crossing[i] == crossing[i + 1])
                        crossing.RemoveAt(i + 1);
                }
            }

            for(int i = 0; i < crossing.Count; i++) //Подсчет точек справа или слева от начальной точки
            {
                if(ln0.type == 1) //Случай вертикальнйо прямой
                {
                    if(ln0.start.Y < ln0.end.Y) //Случай возрастания по Y вертикальной прямой
                    {
                        if (crossing[i].Y < ln0.start.Y)
                        {
                            k++;
                            crossing.RemoveAt(i);
                            i--;
                            continue;
                        }
                        if (crossing[i].Y > ln0.end.Y)
                        {
                            crossing.RemoveAt(i);
                            i--;
                            continue;
                        }
                    }
                    else //случай убывания по Y прямой
                    {
                        if (crossing[i].Y > ln0.start.Y)
                        {
                            crossing.RemoveAt(i);
                            i--;
                            continue;
                        }
                        if (crossing[i].Y < ln0.end.Y)
                        {
                            k++;
                            crossing.RemoveAt(i);
                            i--;
                            continue;
                        }
                    }
                }
                else //случай горизонтальной и произвольнйо прямой
                {
                    if (ln0.start.X < ln0.end.X) //Возрастание прямой по Х
                    {
                        if(crossing[i].X < ln0.start.X)
                        {
                            k++;
                            crossing.RemoveAt(i);
                            i--;
                            continue;
                        }
                        if(crossing[i].X > ln0.end.X)
                        {
                            crossing.RemoveAt(i);
                            i--;
                            continue;
                        }
                    }
                    else //Убывание прямой по Х
                    {
                        if (crossing[i].X > ln0.start.X)
                        {
                            crossing.RemoveAt(i);
                            i--;
                            continue;
                        }
                        if(crossing[i].X < ln0.end.X)
                        {
                            k++;
                            crossing.RemoveAt(i);
                            i--;
                            continue;
                        }
                    }
                }
            }

            if(k%2 == 0) //Построение прямой в зависимости от четности k
            {
                if(k==0)
                {
                    if(crossing.Count == 3)
                    {
                        Lines ln = new Lines(crossing[0].X, crossing[0].Y, crossing[2].X, crossing[2].Y);
                        ln.BuildLine(4, System.Windows.Media.Brushes.Red);
                        Canv.Children.Add(ln.ln);
                    }
                }
                for(int i = 0; i< crossing.Count -1 ; i++)
                {
                    if(i%2 == 0)
                    {
                        Lines ln = new Lines(crossing[i].X, crossing[i].Y, crossing[i + 1].X, crossing[i + 1].Y);
                        ln.BuildLine(4, System.Windows.Media.Brushes.Red);
                        Canv.Children.Add(ln.ln);
                    }
                    else
                    {
                        Lines ln = new Lines(crossing[i].X, crossing[i].Y, crossing[i + 1].X, crossing[i + 1].Y);
                        ln.BuildLine(4, System.Windows.Media.Brushes.Blue);
                        Canv.Children.Add(ln.ln);
                    }
                }
            }
            else
            {
                for (int i = 0; i < crossing.Count -1; i++)
                {
                    if (i % 2 == 0)
                    {
                        Lines ln = new Lines(crossing[i].X, crossing[i].Y, crossing[i + 1].X, crossing[i + 1].Y);
                        ln.BuildLine(4, System.Windows.Media.Brushes.Blue);
                        Canv.Children.Add(ln.ln);
                    }
                    else
                    {
                        Lines ln = new Lines(crossing[i].X, crossing[i].Y, crossing[i + 1].X, crossing[i + 1].Y);
                        ln.BuildLine(4, System.Windows.Media.Brushes.Red);
                        Canv.Children.Add(ln.ln);
                    }
                }
            }

            if (isCrossed == true)
            {
                MessageBox.Show("Ошибка задания многоугольника! Обнаруженно Пересечение");
            }

        }


    }
}
