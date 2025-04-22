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
        bool f = true;
        int k, l; // элементы матрицы сдвига
        int[,] kv = new int[5, 3]; // матрица тела
        int[,] osi = new int[4, 3]; // матрица координат осей
        int[,] matr_sdv = new int[3, 3]; //матрица преобразования
        private double rotationAngle = 0;
        private int[,] originalKv = new int[5, 3]; // Для хранения исходных координат
        public Form1()
        {
            InitializeComponent();
        }
        //обновление для осей
        private void DrawStaticAxes()
        {
            // Оси будут рисоваться относительно центра pictureBox
            int centerX = pictureBox1.Width / 2;
            int centerY = pictureBox1.Height / 2;

            Pen axisPen = new Pen(Color.Red, 1);
            Graphics g = Graphics.FromHwnd(pictureBox1.Handle);

            // Рисуем горизонтальную ось X
            g.DrawLine(axisPen, 0, centerY, pictureBox1.Width, centerY);

            // Рисуем вертикальную ось Y
            g.DrawLine(axisPen, centerX, 0, centerX, pictureBox1.Height);

            g.Dispose();
            axisPen.Dispose();
        }

        private void DrawFigureAndStaticAxes()
        {
            ClearDrawing(); // Очищаем поле
            DrawStaticAxes(); // Рисуем статичные оси поверх фигуры
        }
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
            //Матрица имеет размер 4×3(4 вершины, по 3 координаты на каждую):
            //Первый индекс[i, ] -номер вершины(0-3)
            //Второй индекс[, j] -координаты:
            //j=0 - координата X
            //j = 1 - координата Y
            //j = 2 - однородная координата(всегда 1)
            //kv[0, 0] = -50; kv[0, 1] = 0; kv[0, 2] = 1;
            //kv[1, 0] = 0; kv[1, 1] = 50; kv[1, 2] = 1;
            //kv[2, 0] = 50; kv[2, 1] = 0; kv[2, 2] = 1;
            //kv[3, 0] = 0; kv[3, 1] = -50; kv[3, 2] = 1;
            //kv[4, 0] = 50; kv[4, 1]= -50; kv[4, 2] = 1;
            //

            kv[0, 0] = 0; kv[0, 1] = 0; kv[0, 2] = 1;
            kv[1, 0] = 50; kv[1, 1] = -50; kv[1, 2] = 1;
            kv[2, 0] = -45; kv[2, 1] = -75; kv[2, 2] = 1;
            kv[3, 0] = -45; kv[3, 1] = 100; kv[3, 2] = 1;
            kv[4, 0] = 50; kv[4, 1] = 50; kv[4, 2] = 1;

            // Сохраняем исходные координаты
            Array.Copy(kv, originalKv, kv.Length);
        }

        private void Draw_Kv()
        {


            Init_kvadrat(); //инициализация матрицы тела
            Init_matr_preob(k, l); //инициализация матрицы преобразования
            int[,] kv1 = Multiply_matr(kv, matr_sdv); //перемножение матриц

            Pen myPen = new Pen(Color.Blue, 2);// цвет линии и ширина

            //создаем новый объект Graphics (поверхность рисования) из pictureBox
            Graphics g = Graphics.FromHwnd(pictureBox1.Handle);
            g.DrawLine(myPen, kv1[0, 0], kv1[0, 1], kv1[1, 0], kv1[1, 1]);
            g.DrawLine(myPen, kv1[1, 0], kv1[1, 1], kv1[2, 0], kv1[2, 1]);
            g.DrawLine(myPen, kv1[2, 0], kv1[2, 1], kv1[3, 0], kv1[3, 1]);
            g.DrawLine(myPen, kv1[3, 0], kv1[3, 1], kv1[4, 0], kv1[4, 1]);
            g.DrawLine(myPen, kv1[4, 0], kv1[4, 1], kv1[0, 0], kv1[0, 1]);

            //// рисуем 1 сторону квадрата
            //g.DrawLine(myPen, kv1[0, 0], kv1[0, 1], kv1[1, 0], kv1[1, 1]);
            //// рисуем 2 сторону квадрата
            //g.DrawLine(myPen, kv1[1, 0], kv1[1, 1], kv1[2, 0], kv1[2, 1]);
            //// рисуем 3 сторону квадрата
            //g.DrawLine(myPen, kv1[2, 0], kv1[2, 1], kv1[3, 0], kv1[3, 1]);
            //// рисуем 4 сторону квадрата
            //g.DrawLine(myPen, kv1[3, 0], kv1[3, 1], kv1[0, 0], kv1[0, 1]);

            //g.DrawLine(myPen, kv1[4, 0], kv1[4, 1], kv1[0, 0], kv1[1, 1]);
            g.Dispose();// освобождаем все ресурсы, связанные с отрисовкой
            myPen.Dispose(); //освобождаем ресурсы, связанные с Pen
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
            Draw_Kv();

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

        private void button5_Click(object sender, EventArgs e)
        {
            ClearDrawing();
            DrawStaticAxes();
            k -= 5; //изменение соответствующего элемента матрицы сдвига
            Draw_Kv(); //вывод квадратика
         
        }
        private void button4_Click(object sender, EventArgs e)
        {
           
            ClearDrawing();
            DrawStaticAxes();
            k += 5; //изменение соответствующего элемента матрицы сдвига
            Draw_Kv(); //вывод квадратика

        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            k++;
            ClearDrawing();
            DrawStaticAxes();
            Draw_Kv();
            Thread.Sleep(100);
        }

        private void button6_Click(object sender, EventArgs e)
        {
            ClearDrawing();
            DrawStaticAxes();
            l += 5; //изменение соответствующего элемента матрицы сдвига
            Draw_Kv(); //вывод квадратика
         
            
        }

        private void button7_Click(object sender, EventArgs e)
        {
            ClearDrawing();
            DrawStaticAxes();
            l -= 5; //изменение соответствующего элемента матрицы сдвига
            Draw_Kv(); //вывод квадратика
         
        }
        //Масштабирование  фигуры на плоскости
        private void button9_Click(object sender, EventArgs e)
        {
            DrawFigureAndStaticAxes();
            if (textBox1.Text.Length == 0)
            {
                MessageBox.Show("Введите число!", "Внимание");

            }
            else
            {
                if (double.Parse(textBox1.Text.ToString()) < 0)
                {
                    MessageBox.Show("Число не может быть меньше 0", "");
                    textBox1.Clear();
                }
                else
                {
                    double n = double.Parse(textBox1.Text);
                    double[,] scaleMatrix = new double[3, 3] {
                        { n, 0, 0 },
                        { 0, n, 0 },
                        { 0, 0, 1 }
                    };

                    // Применяем масштабирование к текущим координатам
                    double[,] temp = new double[5, 3];
                    for (int i = 0; i < 5; i++)
                    {
                        for (int j = 0; j < 3; j++)
                        {
                            temp[i, j] = 0;
                            for (int k = 0; k < 3; k++)
                            {
                                temp[i, j] += kv[i, k] * scaleMatrix[k, j];
                            }
                        }
                    }

                    // Поменять для 3 графики!!!
                    for (int i = 0; i < 5; i++)
                    {
                        kv[i, 0] = (int)Math.Round(temp[i, 0]);
                        kv[i, 1] = (int)Math.Round(temp[i, 1]);
                        kv[i, 2] = 1;
                    }

                    // Отрисовка с текущими координатами
                    DrawFigureWithCurrentState();

                    textBox1.Clear();
                }
            }
        }

private void DrawFigureWithCurrentState()
        {
            ClearDrawing();
            DrawStaticAxes();

            // Применяем текущий сдвиг
            int[,] displayedKv = new int[5, 3];
            for (int i = 0; i < 5; i++)
            {
                displayedKv[i, 0] = kv[i, 0] + k;
                displayedKv[i, 1] = kv[i, 1] + l;
                displayedKv[i, 2] = 1;
            }

            Pen myPen = new Pen(Color.Blue, 2);
            Graphics g = Graphics.FromHwnd(pictureBox1.Handle);
            g.DrawLine(myPen, displayedKv[0, 0], displayedKv[0, 1], displayedKv[1, 0], displayedKv[1, 1]);
            g.DrawLine(myPen, displayedKv[1, 0], displayedKv[1, 1], displayedKv[2, 0], displayedKv[2, 1]);
            g.DrawLine(myPen, displayedKv[2, 0], displayedKv[2, 1], displayedKv[3, 0], displayedKv[3, 1]);
            g.DrawLine(myPen, displayedKv[3, 0], displayedKv[3, 1], displayedKv[4, 0], displayedKv[4, 1]);
            g.DrawLine(myPen, displayedKv[4, 0], displayedKv[4, 1], displayedKv[0, 0], displayedKv[0, 1]);
            g.Dispose();
            myPen.Dispose();
        }
        
    
        
            
        
        //Поворот фигуры 
        private void button10_Click(object sender, EventArgs e)
        {
                if (!double.TryParse(textBox2.Text, out double userAngle))
                {
                    MessageBox.Show("Введите число");
                    return;
                }

                DrawFigureAndStaticAxes();

                // Увеличиваем угол поворота на введенное значение (циклически)
                rotationAngle = (rotationAngle + userAngle) % 360;

                // Преобразуем угол в радианы для математических функций
                double angleInRadians = rotationAngle * Math.PI / 180;

                // Создаем матрицу поворота для произвольного угла
                double cosA = Math.Cos(angleInRadians);
                double sinA = Math.Sin(angleInRadians);

                double[,] rotationMatrix = new double[3, 3]
                {
                  { cosA, sinA, 0 },
                  { -sinA,  cosA, 0 },
                  {    0,     0, 1 }
                };

                // Инициализируем фигуру
                Init_kvadrat();

                // Временный массив для хранения повернутых координат (double)
                double[,] temp = new double[5, 3];

                // Применяем поворот к каждой вершине
                for (int i = 0; i < 5; i++)
                {
                    for (int j = 0; j < 3; j++)
                    {
                        temp[i, j] = 0;
                        for (int k = 0; k < 3; k++)
                        {
                            temp[i, j] += kv[i, k] * rotationMatrix[k, j];
                        }
                    }
                }

                // Применяем сдвиг (преобразуем double -> int)
                int[,] rotatedKv = new int[5, 3];
                for (int i = 0; i < 5; i++)
                {
                    rotatedKv[i, 0] = (int)Math.Round(temp[i, 0] + this.k);
                    rotatedKv[i, 1] = (int)Math.Round(temp[i, 1] + this.l);
                    rotatedKv[i, 2] = 1;
                }

                // Отрисовка повернутой фигуры
                Pen myPen = new Pen(Color.Blue, 2);
                Graphics g = Graphics.FromHwnd(pictureBox1.Handle);
                g.DrawLine(myPen, rotatedKv[0, 0], rotatedKv[0, 1], rotatedKv[1, 0], rotatedKv[1, 1]);
                g.DrawLine(myPen, rotatedKv[1, 0], rotatedKv[1, 1], rotatedKv[2, 0], rotatedKv[2, 1]);
                g.DrawLine(myPen, rotatedKv[2, 0], rotatedKv[2, 1], rotatedKv[3, 0], rotatedKv[3, 1]);
                g.DrawLine(myPen, rotatedKv[3, 0], rotatedKv[3, 1], rotatedKv[4, 0], rotatedKv[4, 1]);
                g.DrawLine(myPen, rotatedKv[4, 0], rotatedKv[4, 1], rotatedKv[0, 0], rotatedKv[0, 1]);

                g.Dispose();
                myPen.Dispose();
            }
            
        

        //Отражение фигуры относительно Х
        private void button11_Click(object sender, EventArgs e)
        {
            DrawFigureAndStaticAxes();
            // 3. Матрица отражения относительно оси Y
            int[,] reflectY = new int[3, 3]
            {
               { 1, 0, 0 },
               { 0, -1, 0 },
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

        private void button12_Click(object sender, EventArgs e)
        {
            DrawFigureAndStaticAxes();
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

        private void button13_Click(object sender, EventArgs e)
        {
            DrawFigureAndStaticAxes();
            // 3. Матрица отражения относительно оси Y
            int[,] reflectY = new int[3, 3]
            {
               { 0, 1, 0 },
               { 1, 0, 0 },
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


            //Старый варик Колес
            //// Переднее колесо (окружность, представленная 8 точками)
            //for (int i = 0; i < 8; i++)
            //{
            //    double angle = i * Math.PI / 4;
            //    kv[4 + i, 0] = frontWheelX + (int)(wheelRadius * Math.Cos(angle));
            //    kv[4 + i, 1] = wheelY + (int)(wheelRadius * Math.Sin(angle));
            //    kv[4 + i, 2] = 1;
            //}

            //// Заднее колесо (окружность, представленная 8 точками)
            //for (int i = 0; i < 8; i++)
            //{
            //    double angle = i * Math.PI / 4;
            //    kv[12 + i, 0] = backWheelX + (int)(wheelRadius * Math.Cos(angle));
            //    kv[12 + i, 1] = wheelY + (int)(wheelRadius * Math.Sin(angle));
            //    kv[12 + i, 2] = 1;
            //}

            //// Сохраняем оригинальные координаты
            //originalKv = new int[20, 3];
            //Array.Copy(kv, originalKv, kv.Length);
        }

        private void button14_Click(object sender, EventArgs e)
        {
            k = pictureBox1.Width / 2;
            l = pictureBox1.Height / 2;
            ClearDrawing();
            Draw_Bicycle();
            Draw_osi();
        }

        private void Draw_Bicycle()
        {
            Init_Bicycle(); // Инициализация матрицы велосипеда
            Init_matr_preob(k, l); // Инициализация матрицы преобразования
            int[,] bicycle = Multiply_matr(kv, matr_sdv); // Перемножение матриц

            Pen framePen = new Pen(Color.Blue, 3); // Перо для рамы
            Pen wheelPen = new Pen(Color.Black, 2); // Перо для колес

            Graphics z = Graphics.FromHwnd(pictureBox1.Handle);
            Graphics g = Graphics.FromHwnd(pictureBox1.Handle);
            Graphics p = Graphics.FromHwnd(pictureBox1.Handle);


            // Рисуем раму (треугольник)
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

            z.DrawEllipse(new Pen(Color.Blue, 2),260,245, pictureBox1.Width/11, pictureBox1.Height/11);
            z.DrawEllipse(new Pen(Color.Blue, 2), 360, 245, pictureBox1.Width/11, pictureBox1.Height/11);
            z.DrawEllipse(new Pen(Color.Blue, 2), 90, 245, pictureBox1.Width/11, pictureBox1.Height/11);
            z.DrawEllipse(new Pen(Color.Blue, 2), 205, 245, pictureBox1.Width/11, pictureBox1.Height/11);

            p.DrawLine(framePen, bicycle[13, 0], bicycle[13, 1], bicycle[14, 0], bicycle[14, 1]);
            p.DrawLine(framePen, bicycle[14, 0], bicycle[14, 1], bicycle[15, 0], bicycle[15, 1]);
            p.DrawLine(framePen, bicycle[15, 0], bicycle[15, 1], bicycle[16, 0], bicycle[16, 1]);
            p.DrawLine(framePen, bicycle[16, 0], bicycle[16, 1], bicycle[13, 0], bicycle[13, 1]);

            p.DrawLine(framePen, bicycle[17, 0], bicycle[17, 1], bicycle[18, 0], bicycle[18, 1]);
            p.DrawLine(framePen, bicycle[18, 0], bicycle[18, 1], bicycle[19, 0], bicycle[19, 1]);
            p.DrawLine(framePen, bicycle[19, 0], bicycle[19, 1], bicycle[20, 0], bicycle[20, 1]);
            p.DrawLine(framePen, bicycle[20, 0], bicycle[20, 1], bicycle[17, 0], bicycle[17, 1]);
            //// Рисуем спицы колес
            //Pen spokePen = new Pen(Color.Gray, 1);
            //int frontCenterX = bicycle[4, 0] + (bicycle[8, 0] - bicycle[4, 0]) / 2;
            //int frontCenterY = bicycle[4, 1] + (bicycle[8, 1] - bicycle[4, 1]) / 2;

            //int backCenterX = bicycle[12, 0] + (bicycle[16, 0] - bicycle[12, 0]) / 2;
            //int backCenterY = bicycle[12, 1] + (bicycle[16, 1] - bicycle[12, 1]) / 2;

            //for (int i = 0; i < 8; i++)
            //{
            //    g.DrawLine(spokePen, frontCenterX, frontCenterY, bicycle[4 + i, 0], bicycle[4 + i, 1]);
            //    g.DrawLine(spokePen, backCenterX, backCenterY, bicycle[12 + i, 0], bicycle[12 + i, 1]);
            //}

            //g.Dispose();
            //framePen.Dispose();
            //wheelPen.Dispose();
            //spokePen.Dispose();
        }
    }
}
