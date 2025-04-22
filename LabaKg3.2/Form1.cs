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
        private void button12_Click(object sender, EventArgs e)
        {

            // 3. Матрица отражения относительно оси Y
            int[,] reflectY = new int[3, 3]
            {
               { -1, 0, 0 },
               { 0, 1, 0 },
                 { 0, 0, 1 }
            };

            Init_kvadrat();
            Init_matr_preob(k, l);

            // Применяем отражение, затем сдвиг
            int[,] temp = Multiply_matr(kv, reflectY);
            int[,] kv1 = Multiply_matr(temp, matr_sdv);

            // Рисуем отраженную фигуру
            Pen myPen = new Pen(Color.Blue, 2);
            Graphics g = Graphics.FromHwnd(pictureBox1.Handle);
            g.DrawLine(myPen, kv1[0, 0], kv1[0, 1], kv1[1, 0], kv1[1, 1]);
            g.DrawLine(myPen, kv1[1, 0], kv1[1, 1], kv1[2, 0], kv1[2, 1]);
            g.DrawLine(myPen, kv1[2, 0], kv1[2, 1], kv1[3, 0], kv1[3, 1]);
            g.DrawLine(myPen, kv1[3, 0], kv1[3, 1], kv1[4, 0], kv1[4, 1]);
            g.DrawLine(myPen, kv1[4, 0], kv1[4, 1], kv1[0, 0], kv1[0, 1]);
            g.Dispose();
            myPen.Dispose();
        }

      
        //Очистка PictureBox1
        private void button3_Click(object sender, EventArgs e)
        {
            pictureBox1.Image = null;
        }
        private void Init_Bicycle()
        {
            kv = new int[90, 3]; // Увеличиваем количество точек для велосипеда

            // Рама велосипеда
            //kv[0, 0] = -33; kv[0, 1] = -7; kv[0, 2] = 1;
            // Поезд
            kv[0, 0] = 0; kv[0, 1] = 0; kv[0, 2] = 1;
            kv[1, 0]= 150; kv[1, 1] = 0; kv[1, 2] = 1;
            kv[2, 0] = 160; kv[2, 1] = -30; kv[2, 2] = 1;
            kv[3, 0] = 150; kv[3, 1] = -60; kv[3, 2] =1;
            kv[4, 0] = 110; kv[4, 1] = -60; kv[4, 2] = 1;
            kv[5, 0] = 130; kv[5, 1] = -90; kv[5, 2] = 1;
            kv[6, 0] = 92; kv[6, 1] = -90; kv[6, 2] = 1;
            kv[7, 0] = 100; kv[7, 1] = -60; kv[7, 2] =1;
            kv[8, 0] = 60; kv[8, 1] = -60; kv[8, 2] =1;
            kv[9, 0] = 60; kv[9, 1] = -90; kv[9, 2] = 1;
            kv[10, 0] = 0; kv[10, 1] = -90; kv[10, 2] = 1;
            //окно
            kv[11, 0] = 50; kv[11, 1] = -70; kv[11, 2] = 1;
            kv[12, 0] = 10; kv[12, 1] =-70; kv[12, 2] = 1;

            //голубой вагон
            kv[13, 0] = -10; kv[13, 1] = 0; kv[13, 2] = 1;
            kv[14, 0] = -170; kv[14, 1] = 0; kv[14, 2] = 1;
            kv[15, 0] = -170; kv[15, 1] = -90; kv[15, 2] = 1;
            kv[16, 0] = -10; kv[16, 1] = -90; kv[16, 2] = 1;


            kv[17, 0] = -30; kv[17, 1] = -20; kv[17, 2] = 1;
            kv[18, 0] = -70; kv[18, 1] = -20; kv[18, 2] = 1;
            kv[19, 0] = -70; kv[19, 1] = -50; kv[19, 2] = 1;
            kv[20, 0] = -30; kv[20, 1] = -50; kv[20, 2] = 1;
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
            Init_Bicycle();
            Init_matr_preob(k, l);
            int[,] bicycle = Multiply_matr(kv, matr_sdv);

            Pen framePen = new Pen(Color.Blue, 3);
            Pen wheelPen = new Pen(Color.Black, 2);
            Pen railPen = new Pen(Color.DarkGray, 4);
            Pen sleeperPen = new Pen(Color.Brown, 3);
            Pen connectingRodPen = new Pen(Color.Red, 3); // Перо для шатуна

            Graphics z = Graphics.FromHwnd(pictureBox1.Handle);
            Graphics g = Graphics.FromHwnd(pictureBox1.Handle);
            Graphics p = Graphics.FromHwnd(pictureBox1.Handle);

            // Рисуем рельсы с учетом смещения
            int railY = bicycle[10, 1] + 136;
            int railLeft = bicycle[14, 0] - 12000;
            int railRight = bicycle[1, 0] + 12000;

            // Основные рельсы (смещаем на railOffset)
            z.DrawLine(railPen, railLeft + railOffset, railY, railRight + railOffset, railY);
            z.DrawLine(railPen, railLeft + railOffset, railY + 15, railRight + railOffset, railY + 15);

            // Шпалы (также смещаем на railOffset)
            for (int x = railLeft; x <= railRight; x +=60)
            {
                z.DrawLine(sleeperPen,
                          x - 5 + railOffset, railY - 5,
                          x + 5 + railOffset, railY + 20);
            }

            // Рисуем паровоз (ваш существующий код)
            g.DrawLine(framePen, bicycle[0, 0], bicycle[0, 1], bicycle[1, 0], bicycle[1, 1]);
            g.DrawLine(framePen, bicycle[1, 0], bicycle[1, 1], bicycle[2, 0], bicycle[2, 1]);
            g.DrawLine(framePen, bicycle[2, 0], bicycle[2, 1], bicycle[3, 0], bicycle[3, 1]);
            g.DrawLine(framePen, bicycle[3, 0], bicycle[3, 1], bicycle[4, 0], bicycle[4, 1]);
            g.DrawLine(framePen, bicycle[4, 0], bicycle[4, 1], bicycle[5, 0], bicycle[5, 1]);
            g.DrawLine(framePen, bicycle[5, 0], bicycle[5, 1], bicycle[6, 0], bicycle[6, 1]);
            g.DrawLine(framePen, bicycle[6, 0], bicycle[6, 1], bicycle[7, 0], bicycle[7, 1]);
            g.DrawLine(framePen, bicycle[7, 0], bicycle[7, 1], bicycle[8, 0], bicycle[8, 1]);
            g.DrawLine(framePen, bicycle[8, 0], bicycle[8, 1], bicycle[9, 0], bicycle[9, 1]);
            g.DrawLine(framePen, bicycle[9, 0], bicycle[9, 1], bicycle[10, 0], bicycle[10, 1]);
            g.DrawLine(framePen, bicycle[10, 0], bicycle[10, 1], bicycle[0, 0], bicycle[0, 1]);

            // Окно паровозика
            z.DrawLine(framePen, bicycle[11, 0], bicycle[11, 1], bicycle[12, 0], bicycle[12, 1]);

            // Колеса
            int wheelDiameter1 = pictureBox1.Width / 11;
            int wheelDiameter2 = pictureBox1.Width / 11;
            int wheelDiameter3 = pictureBox1.Width / 11;
            int wheelDiameter4 = pictureBox1.Width / 11;

            int wheelX1 = 260;
            int wheelY1 = 245;
            int wheelX2 = 360;
            int wheelY2 = 245;
            int wheelX3 = 90;
            int wheelY3 = 245;
            int wheelX4 = 205;
            int wheelY4 = 245;

            z.DrawEllipse(new Pen(Color.Blue, 2), wheelX1, wheelY1, wheelDiameter1, wheelDiameter1);
            z.DrawEllipse(new Pen(Color.Blue, 2), wheelX2, wheelY2, wheelDiameter2, wheelDiameter2);
            z.DrawEllipse(new Pen(Color.Blue, 2), wheelX3, wheelY3, wheelDiameter3, wheelDiameter3);
            z.DrawEllipse(new Pen(Color.Blue, 2), wheelX4, wheelY4, wheelDiameter4, wheelDiameter4);

            // Рисуем спицы для каждого колеса
            DrawWheelSpokes(z, wheelX1 + wheelDiameter1 / 2, wheelY1 + wheelDiameter1 / 2, wheelDiameter1 / 2, 6, Color.Black);
            DrawWheelSpokes(z, wheelX2 + wheelDiameter2 / 2, wheelY2 + wheelDiameter2 / 2, wheelDiameter2 / 2, 6, Color.Black);
            DrawWheelSpokes(z, wheelX3 + wheelDiameter3 / 2, wheelY3 + wheelDiameter3 / 2, wheelDiameter3 / 2, 6, Color.Black);
            DrawWheelSpokes(z, wheelX4 + wheelDiameter4 / 2, wheelY4 + wheelDiameter4 / 2, wheelDiameter4 / 2, 6, Color.Black);

            // Добавляем шатун между колесами
            // Точка крепления шатуна к первому колесу (с учетом вращения)
            float rodStartX = wheelX1 + wheelDiameter1 / 2;
            float rodStartY = wheelY1 + wheelDiameter1 / 2;

            // Точка крепления шатуна ко второму колесу (смещена на 180 градусов)
            float rodEndX = wheelX2 + wheelDiameter2 / 2;
            float rodEndY = wheelY2 + wheelDiameter2 / 2;

            // Рисуем шатун (красная линия)
            z.DrawLine(connectingRodPen, rodStartX, rodStartY, rodEndX, rodEndY);

            // Рисуем кривошип (короткие линии от центра колеса к шатуну)
            float crankLength = wheelDiameter1 / 3;
            float crankStartX1 = rodStartX + crankLength * (float)Math.Cos(rotationAngle);
            float crankStartY1 = rodStartY + crankLength * (float)Math.Sin(rotationAngle);
            float crankStartX2 = rodEndX - crankLength * (float)Math.Cos(rotationAngle);
            float crankStartY2 = rodEndY - crankLength * (float)Math.Sin(rotationAngle);

            z.DrawLine(connectingRodPen, rodStartX, rodStartY, crankStartX1, crankStartY1);
            z.DrawLine(connectingRodPen, rodEndX, rodEndY, crankStartX2, crankStartY2);

            // Обновляем угол вращения для анимации
            rotationAngle += 0.1;
            if (rotationAngle > 2 * Math.PI)
            {
                rotationAngle = 0;
            }

            // Вагон
            p.DrawLine(framePen, bicycle[13, 0], bicycle[13, 1], bicycle[14, 0], bicycle[14, 1]);
            p.DrawLine(framePen, bicycle[14, 0], bicycle[14, 1], bicycle[15, 0], bicycle[15, 1]);
            p.DrawLine(framePen, bicycle[15, 0], bicycle[15, 1], bicycle[16, 0], bicycle[16, 1]);
            p.DrawLine(framePen, bicycle[16, 0], bicycle[16, 1], bicycle[13, 0], bicycle[13, 1]);

            // Окно вагона
            p.DrawLine(framePen, bicycle[17, 0], bicycle[17, 1], bicycle[18, 0], bicycle[18, 1]);
            p.DrawLine(framePen, bicycle[18, 0], bicycle[18, 1], bicycle[19, 0], bicycle[19, 1]);
            p.DrawLine(framePen, bicycle[19, 0], bicycle[19, 1], bicycle[20, 0], bicycle[20, 1]);
            p.DrawLine(framePen, bicycle[20, 0], bicycle[20, 1], bicycle[17, 0], bicycle[17, 1]);
        }

        private void DrawWheelSpokes(Graphics g, float centerX, float centerY, float radius, int numSpokes, Color color)
        {
            Pen spokePen = new Pen(color, 1); // Толщина спиц

            for (int i = 0; i < numSpokes; i++)
            {
                // Угол каждой спицы
                double angle = 2 * Math.PI * i / numSpokes;

                // Координаты конца спицы
                float endX = centerX + radius * (float)Math.Cos(angle);
                float endY = centerY + radius * (float)Math.Sin(angle);

                // Рисуем спицу
                g.DrawLine(spokePen, centerX, centerY, endX, endY);
            }

            spokePen.Dispose();
        }
    }
}