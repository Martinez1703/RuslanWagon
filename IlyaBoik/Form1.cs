using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Schema;

namespace LabaKg3
{
    public partial class Form1 : Form
    {
        private int railOffset = 0;
        bool f = true;
        int k, l; // элементы матрицы сдвига
        int[,] kv = new int[90, 3]; // матрица тела
        int[,] osi = new int[4, 3]; // матрица координат осей
        int[,] matr_sdv = new int[3, 3]; //матрица преобразования
        private double rotationAngle = 0;
        private int[,] originalKv = new int[90, 3]; // Для хранения исходных координат
        public Form1()
        {
            InitializeComponent();
        }
        //обновление для осей



        private void ClearDrawing()
        {
            pictureBox1.Image = null;
            pictureBox1.Refresh();
        }
        private void ResetFigure()
        {
            Array.Copy(originalKv, kv, originalKv.Length);
        }
        private void Init_kvadrat()
        {

            kv[0, 0] = 0; kv[0, 1] = 0; kv[0, 2] = 1;
            kv[1, 0] = 50; kv[1, 1] = -50; kv[1, 2] = 1;
            kv[2, 0] = -45; kv[2, 1] = -75; kv[2, 2] = 1;
            kv[3, 0] = -45; kv[3, 1] = 100; kv[3, 2] = 1;
            kv[4, 0] = 50; kv[4, 1] = 50; kv[4, 2] = 1;

            // Сохраняем исходные координаты
            Array.Copy(kv, originalKv, kv.Length);
        }
        private void Init_matr_preob(int k1, int l1)
        {
            matr_sdv[0, 0]=1; matr_sdv[0, 1]=0; matr_sdv[0, 2]=0;
            matr_sdv[1, 0]=0; matr_sdv[1, 1]=1; matr_sdv[1, 2]=0;
            matr_sdv[2, 0]=k1; matr_sdv[2, 1]=l1; matr_sdv[2, 2]=1;
        }

        private void Init_osi()
        {
            osi[0, 0] = -200; osi[0, 1] = 0; osi[0, 2] = 1;
            osi[1, 0] = 200; osi[1, 1] = 0; osi[1, 2] = 1;
            osi[2, 0] = 0; osi[2, 1] = 200; osi[2, 2] = 1;
            osi[3, 0] = 0; osi[3, 1] = -200; osi[3, 2] = 1;

        }
        private int[,] Multiply_matr(int[,] a, int[,] b)
        {
            int n = a.GetLength(0);
            int m = a.GetLength(1);

            int[,] r = new int[n, m];
            for (int i = 0; i<n; i++)
            {
                for (int j = 0; j < m; j++)
                {
                    r[i, j] = 0;
                    for (int ii = 0; ii < m; ii++)
                    {
                        r[i, j] += a[i, ii] * b[ii, j];
                    }
                }
            }
            return r;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            //середина pictureBox
            k = pictureBox1.Width / 2;
            l = pictureBox1.Height / 2;


        }
        private void Draw_osi()
        {
            Init_osi();
            Init_matr_preob(k, l);
            int[,] osi1 = Multiply_matr(osi, matr_sdv);

            Pen myPen = new Pen(Color.Red, 1);
            Graphics g = Graphics.FromHwnd(pictureBox1.Handle);

            g.DrawLine(myPen, osi1[0, 0], osi1[0, 1], osi1[1, 0], osi1[1, 1]);
            g.DrawLine(myPen, osi1[2, 0], osi1[2, 1], osi1[3, 0], osi1[3, 1]);

            g.Dispose();
            myPen.Dispose();
        }
        //Отрисовкка осей
        private void button1_Click(object sender, EventArgs e)
        {
            k = pictureBox1.Width / 2;
            l = pictureBox1.Height / 2;
            Draw_osi();
        }
        //Таемер для сдвига
        private void button8_Click(object sender, EventArgs e)
        {
            //ClearDrawing();
            timer1.Interval = 100;

            button8.Text = "Стоп";
            if (f == true)
            {
                timer1.Start();

            }
            else
            {
                timer1.Stop();
                button8.Text = "Старт";
            }
            f = !f;

        }
        private void timer1_Tick(object sender, EventArgs e)
        {
            // Увеличиваем смещение рельсов (движение влево)
            railOffset -= 5;

            // Очищаем и перерисовываем сцену
            ClearDrawing();

            Draw_Bicycle();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            ClearDrawing();
            ;
            l += 5; //изменение соответствующего элемента матрицы сдвига
        }
       
        //Очистка PictureBox1
        private void button3_Click(object sender, EventArgs e)
        {
            pictureBox1.Image = null;
        }
        private void Init_Bicycle()
        {
            kv = new int[90, 3]; // Увеличиваем количество точек для велосипеда
            int z = 130;
            int p = 50;
            int k = 60;
            // Рама велосипеда
            //kv[0, 0] = -33; kv[0, 1] = -7; kv[0, 2] = 1;
            // Поезд
            kv[0, 0] = -150; kv[0, 1] = 0+z; kv[0, 2] = 1;
            kv[1, 0] =10; kv[1, 1] =0+z; kv[1, 2] =1;
            kv[2, 0] =50; kv[2, 1] =-60+z; kv[2, 2] =1;
            kv[3, 0] =-190; kv[3, 1] =-60+z; kv[3, 2] =1;
            kv[4, 0] =-90; kv[4, 1] =-60+z; kv[4, 2] =1;
            kv[5, 0] =-90; kv[5, 1] =-170+z; kv[5, 2] =1;
            kv[6, 0] = -20; kv[6, 1] = -130+z; kv[6, 2] = 1;
            kv[7, 0]= -90; kv[7, 1] = -60+z; kv[7, 2] =1;

            //чайки распиздяйки
            kv[8, 0] =-90; kv[8, 1] =-190; kv[8, 2] =1;
            kv[9, 0] = -80; kv[9, 1] = -160; kv[9, 2] = 1;
            kv[10, 0]= -70; kv[10, 1] = -190; kv[10, 2] =1;

            kv[11, 0] =-90+p; kv[11, 1] =-190; kv[11, 2] =1;
            kv[12, 0] = -80+p; kv[12, 1] = -160; kv[12, 2] = 1;
            kv[13, 0]= -70+p; kv[13, 1] = -190; kv[13, 2] =1;

            kv[14, 0] =-90+10; kv[14, 1] =-190+k; kv[14,2] =1;
            kv[15, 0] = -80+10; kv[15, 1] = -160+k; kv[15, 2] = 1;
            kv[16, 0]= -70+10; kv[16, 1] = -190+k; kv[16, 2] =1;

        }

        private void button14_Click(object sender, EventArgs e)
        {
            k = pictureBox1.Width / 2;
            l = pictureBox1.Height / 2;
            ClearDrawing();
            Draw_Bicycle();
            //Draw_osi();
        }

        private void Draw_Bicycle()
        {
            int wheelX1 = 400;
            int wheelY1 = 220;
            int wheelDiameter1 = pictureBox1.Width / 7;

            Init_Bicycle();
            Init_matr_preob(k, l);
            int[,] bicycle = Multiply_matr(kv, matr_sdv);

            Pen framePen = new Pen(Color.Red, 4);
            Pen wheelPen = new Pen(Color.Black, 2);
            Pen railPen = new Pen(Color.DarkGray, 4);
            Pen sleeperPen = new Pen(Color.Brown, 3);
            Pen wavePen = new Pen(Color.Blue, 2);

            Graphics z = Graphics.FromHwnd(pictureBox1.Handle);
            Graphics g = Graphics.FromHwnd(pictureBox1.Handle);
            Graphics p = Graphics.FromHwnd(pictureBox1.Handle);

            // Рисуем паровоз
            g.DrawLine(framePen, bicycle[0, 0], bicycle[0, 1], bicycle[1, 0], bicycle[1, 1]);
            g.DrawLine(framePen, bicycle[1, 0], bicycle[1, 1], bicycle[2, 0], bicycle[2, 1]);
            g.DrawLine(framePen, bicycle[2, 0], bicycle[2, 1], bicycle[3, 0], bicycle[3, 1]);
            g.DrawLine(framePen, bicycle[3, 0], bicycle[3, 1], bicycle[0, 0], bicycle[0, 1]);
            g.DrawLine(framePen, bicycle[4, 0], bicycle[4, 1], bicycle[5, 0], bicycle[5, 1]);
            g.DrawLine(framePen, bicycle[5, 0], bicycle[5, 1], bicycle[6, 0], bicycle[6, 1]);
            g.DrawLine(framePen, bicycle[6, 0], bicycle[6, 1], bicycle[7, 0], bicycle[7, 1]);

            // Солнышко 
            z.DrawEllipse(new Pen(Color.Orange, 100), wheelX1, wheelY1, wheelDiameter1, wheelDiameter1);

            // Чайки
            z.DrawLine(wheelPen, bicycle[8, 0], bicycle[8, 1], bicycle[9, 0], bicycle[9, 1]);
            z.DrawLine(wheelPen, bicycle[9, 0], bicycle[9, 1], bicycle[10, 0], bicycle[10, 1]);

            z.DrawLine(wheelPen, bicycle[11, 0], bicycle[11, 1], bicycle[12, 0], bicycle[12, 1]);
            z.DrawLine(wheelPen, bicycle[12, 0], bicycle[12, 1], bicycle[13, 0], bicycle[13, 1]);

            z.DrawLine(wheelPen, bicycle[14, 0], bicycle[14, 1], bicycle[15, 0], bicycle[15, 1]);
            z.DrawLine(wheelPen, bicycle[15, 0], bicycle[15, 1], bicycle[16, 0], bicycle[16, 1]);

            // Параметры волн
            int waveHeight1 = 20; // Высота основной волны
            int waveHeight2 = 10; // Высота вторичных волн
            int waveLength = 30;  // Длина волны
            int seaLevel = pictureBox1.Height - 100; // Уровень моря

            // Определяем контур корабля
            List<LineSegment> shipSegments = new List<LineSegment>
    {
        new LineSegment(new Point(bicycle[0, 0], bicycle[0, 1]), new Point(bicycle[1, 0], bicycle[1, 1])),
        new LineSegment(new Point(bicycle[1, 0], bicycle[1, 1]), new Point(bicycle[2, 0], bicycle[2, 1])),
        new LineSegment(new Point(bicycle[2, 0], bicycle[2, 1]), new Point(bicycle[3, 0], bicycle[3, 1])),
        new LineSegment(new Point(bicycle[3, 0], bicycle[3, 1]), new Point(bicycle[0, 0], bicycle[0, 1])),
        new LineSegment(new Point(bicycle[4, 0], bicycle[4, 1]), new Point(bicycle[5, 0], bicycle[5, 1])),
        new LineSegment(new Point(bicycle[5, 0], bicycle[5, 1]), new Point(bicycle[6, 0], bicycle[6, 1])),
        new LineSegment(new Point(bicycle[6, 0], bicycle[6, 1]), new Point(bicycle[7, 0], bicycle[7, 1]))
    };

            // Рисуем волны с обтеканием корабля
      
            DrawWave(g, wavePen, seaLevel, waveHeight2, waveLength, railOffset + 15, bicycle, -50);
            DrawWave(g, wavePen, seaLevel, waveHeight2, waveLength, railOffset + 15, bicycle, 20);
            DrawWave(g, wavePen, seaLevel, waveHeight2, waveLength, railOffset + 15, bicycle, 70);

            // Освобождаем ресурсы
            framePen.Dispose();
            wheelPen.Dispose();
            railPen.Dispose();
            sleeperPen.Dispose();
            wavePen.Dispose();
            z.Dispose();
            g.Dispose();
            p.Dispose();
        }

        // Метод для рисования одной волны с обтеканием корабля
        private void DrawWave(Graphics g, Pen wavePen, int seaLevel, int waveHeight, int waveLength, int offset, int[,] bicycle, int yOffset)
        {
            List<Point> wavePoints = new List<Point>();
            bool wasInsideShip = false;
            Point[] shipOutline = GetShipOutline(bicycle);

            for (int x = 0; x < pictureBox1.Width; x++)
            {
                int y = seaLevel + yOffset + (int)(waveHeight * Math.Sin((x + offset) * Math.PI / waveLength));
                Point currentPoint = new Point(x, y);

                bool isInsideShip = IsPointInPolygon(currentPoint, shipOutline);

                if (!isInsideShip)
                {
                    if (wasInsideShip)
                    {
                        // Если вышли из корабля, начинаем новую линию
                        if (wavePoints.Count > 0)
                        {
                            g.DrawLines(wavePen, wavePoints.ToArray());
                        }
                        wavePoints.Clear();
                    }
                    wavePoints.Add(currentPoint);
                    wasInsideShip = false;
                }
                else
                {
                    wasInsideShip = true;
                }
            }

            if (wavePoints.Count > 0)
            {
                g.DrawLines(wavePen, wavePoints.ToArray());
            }
        }

        private bool IsPointInPolygon(Point point, Point[] polygon)
        {
            int n = polygon.Length;
            bool inside = false;
            for (int i = 0, j = n - 1; i < n; j = i++)
            {
                if (((polygon[i].Y > point.Y) != (polygon[j].Y > point.Y)) &&
                    (point.X < (polygon[j].X - polygon[i].X) * (point.Y - polygon[i].Y) / (polygon[j].Y - polygon[i].Y) + polygon[i].X))
                {
                    inside = !inside;
                }
            }
            return inside;
        }

        private Point[] GetShipOutline(int[,] bicycle)
        {
            return new Point[]
            {
        new Point(bicycle[0, 0], bicycle[0, 1]),
        new Point(bicycle[1, 0], bicycle[1, 1]),
        new Point(bicycle[2, 0], bicycle[2, 1]),
        new Point(bicycle[3, 0], bicycle[3, 1]),
        new Point(bicycle[0, 0], bicycle[0, 1])
            };
        }

        private struct LineSegment
        {
            public Point Start;
            public Point End;

            public LineSegment(Point start, Point end)
            {
                Start = start;
                End = end;
            }
        }
    }
}