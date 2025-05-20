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
using static System.Windows.Forms.AxHost;

namespace LabaKg3
{
    public partial class Form1 : Form
    {
        bool f = true;
        int k, l, m; // элементы матрицы сдвига
        float[,] kvSohran = new float[8, 4]; // Уменьшил размер, так как originalKv имеет 8 строк
        float[,] kv = new float[8, 4]; // матрица тела велосипеда (8 вершин)
        float[,] osi = new float[4, 3]; // матрица координат осей
        float[,] matr_sdv = new float[4, 4]; //матрица преобразования
        private double rotationAngle = 0;
        private double wheelRotationAngle = 0; // угол поворота колес
        private float[,] originalKv = new float[8, 4]; // Для хранения исходных координат (float)
        private const int wheelRadius = 48; // радиус колес
        private const int spokeCount = 8; // количество спиц в колесе
        private float scale = 1.0f;       // Масштаб
        private double rotationAngleY = 0; // Угол поворота вокруг оси Y в радианах
        private double rotationAngleX = 0; // Угол вращения вокруг оси X

        private double rotationAngleZ = 0; // Угол вращения вокруг оси Z
        public Form1()
        {
            InitializeComponent();
            Init_kvadrat(); // Инициализируем фигуру один раз при запуске
            Array.Copy(kv, originalKv, kv.Length); // Сохраняем исходное состояние
        }
        //обновление для осей
        private float[,] CalculateFunctionValues(int stepsX, int stepsY)
        {
            float[,] zValues = new float[stepsX, stepsY];
            float xMin = -3f, xMax = 3f;
            float yMin = -3f, yMax = 3f;

            for (int i = 0; i < stepsX; i++)
            {
                float x = xMin + (xMax - xMin) * i / (stepsX - 1);
                for (int j = 0; j < stepsY; j++)
                {
                    float y = yMin + (yMax - yMin) * j / (stepsY - 1);
                    zValues[i, j] = (float)Math.Exp(Math.Sin(y) - x * x);
                }
            }
            return zValues;
        }
        
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

       
        private void ClearDrawing()
        {
            pictureBox1.Image = null;
            pictureBox1.Refresh();
        }
        private void ResetFigure()
        {
            Array.Copy(originalKv, kv, originalKv.Length);
            Draw_Kv(); // Перерисовываем фигуру в исходном положении
        }
        private void Init_kvadrat()
        {

            kv[0, 0] = 0; kv[0, 1] = 0; kv[0, 2] = 0; kv[0, 3] = 1;
            kv[1, 0] = 100; kv[1, 1] = 0; kv[1, 2] = 0; kv[1, 3] = 1;
            kv[2, 0] = 100; kv[2, 1] = 100; kv[2, 2] = 0; kv[2, 3] = 1;
            kv[3, 0] = 0; kv[3, 1] = 100; kv[3, 2] = 0; kv[3, 3] = 1;
            //верх
            kv[4, 0] = 0; kv[4, 1] = 100; kv[4, 2] = 100; kv[4, 3] = 1;
            kv[5, 0] = 100; kv[5, 1] = 100; kv[5, 2] = 100; kv[5, 3] = 1;
            kv[6, 0] = 100; kv[6, 1] = 0; kv[6, 2] = 100; kv[6, 3] = 1;
            kv[7, 0] = 0; kv[7, 1] = 0; kv[7, 2] = 100; kv[7, 3] = 1;

            // Сохраняем исходные координаты
            Array.Copy(kv, originalKv, kv.Length);
        }
        private float rotationX = (float)(Math.PI / 2); // 90° в радианах
        private void Draw_Kv()
        {
            ClearDrawing();

            // Создаем матрицу поворота
            float[,] rotationX = CreateRotationXMatrix(rotationAngleX);
            float[,] rotationY = CreateRotationYMatrix(rotationAngleY);
            float[,] rotationZ = CreateRotationZMatrix(rotationAngleZ);

            // Комбинируем повороты (порядок важен!)
            float[,] rotationMatrix = Multiply_matr(rotationX, rotationY);
            rotationMatrix = Multiply_matr(rotationMatrix, rotationZ);

            // Применяем поворот к каждой точке функции
            float[,] zValues = CalculateFunctionValues(30, 30);
            int stepsX = zValues.GetLength(0);
            int stepsY = zValues.GetLength(1);

            int centerX = pictureBox1.Width / 2;
            int centerY = pictureBox1.Height / 2;

            using (Graphics g = Graphics.FromHwnd(pictureBox1.Handle))
            using (Pen pen = new Pen(Color.Blue, 1))
            {
                // Преобразуем и рисуем все точки
                PointF[,] transformedPoints = new PointF[stepsX, stepsY];

                for (int i = 0; i < stepsX; i++)
                {
                    for (int j = 0; j < stepsY; j++)
                    {
                        float x = -3f + 6f * i / (stepsX - 1);
                        float y = -3f + 6f * j / (stepsY - 1);
                        float z = zValues[i, j];

                        // Применяем поворот
                        float[] point = { x, y, z, 1 };
                        float[] rotatedPoint = new float[4];

                        for (int k = 0; k < 4; k++)
                        {
                            rotatedPoint[k] = 0;
                            for (int l = 0; l < 4; l++)
                            {
                                rotatedPoint[k] += point[l] * rotationMatrix[l, k];
                            }
                        }

                        // Проецируем на 2D с учетом масштаба и центра
                        transformedPoints[i, j] = new PointF(
                            centerX + rotatedPoint[0] * scale * 20,
                            centerY - rotatedPoint[1] * scale * 20
                        );
                    }
                }

                // Рисуем линии вдоль X (вертикальные линии)
                for (int j = 0; j < stepsY; j++)
                {
                    PointF[] linePoints = new PointF[stepsX];
                    for (int i = 0; i < stepsX; i++)
                    {
                        linePoints[i] = transformedPoints[i, j];
                    }
                    g.DrawLines(pen, linePoints);
                }

                // Рисуем линии вдоль Y (горизонтальные линии)
                for (int i = 0; i < stepsX; i++)
                {
                    PointF[] linePoints = new PointF[stepsY];
                    for (int j = 0; j < stepsY; j++)
                    {
                        linePoints[j] = transformedPoints[i, j];
                    }
                    g.DrawLines(pen, linePoints);
                }
            }

            
        }
        private float[,] ApplyTransformations(float[,] zValues)
        {
            int stepsX = zValues.GetLength(0);
            int stepsY = zValues.GetLength(1);
            float[,] transformed = new float[stepsX, stepsY];

            // Преобразуем 2D-массив в 3D-точки (x, y, z)
            float xMin = -3f, xMax = 3f;
            float yMin = -3f, yMax = 3f;

            for (int i = 0; i < stepsX; i++)
            {
                float x = xMin + (xMax - xMin) * i / (stepsX - 1);
                for (int j = 0; j < stepsY; j++)
                {
                    float y = yMin + (yMax - yMin) * j / (stepsY - 1);
                    float z = zValues[i, j];

                    // Применяем преобразования (поворот, сдвиг, масштаб)
                    float newX = x;
                    float newY = y;
                    float newZ = z;

                    // 1. Поворот вокруг осей (если нужно)
                    if (rotationAngle != 0)
                    {
                        //  поворот вокруг оси Y
                        double cosA = Math.Cos(rotationAngle);
                        double sinA = Math.Sin(rotationAngle);
                        newX = (float)(x * cosA - z * sinA);
                        newZ = (float)(x * sinA + z * cosA);
                    }

                    // 2. Сдвиг (k, l, m)
                    newX += k;
                    newY += l;
                    newZ += m;

                    // 3. Масштабирование (если нужно)
                    newX *= scale;
                    newY *= scale;
                    newZ *= scale;

                    transformed[i, j] = newZ; // Возвращаем только Z (X и Y используются для позиционирования)
                }
            }

            return transformed;
        }
        private void Init_matr_preob(int k1, int l1, int m1)
        {
            matr_sdv[0, 0] = 1; matr_sdv[0, 1] = 0; matr_sdv[0, 2] = 0; matr_sdv[0, 3] = 0;
            matr_sdv[1, 0] = 0; matr_sdv[1, 1] = 1; matr_sdv[1, 2] = 0; matr_sdv[1, 3] = 0;
            matr_sdv[2, 0] = 0; matr_sdv[2, 1] = 0; matr_sdv[2, 2] = 1; matr_sdv[2, 3] = 0;
            matr_sdv[3, 0] = k1; matr_sdv[3, 1] = l1; matr_sdv[3, 2] = m1; matr_sdv[3, 3] = 1;
        }

        private void Init_osi()
        {
            osi[0, 0] = -200; osi[0, 1] = 0; osi[0, 2] = 1;
            osi[1, 0] = 200; osi[1, 1] = 0; osi[1, 2] = 1;
            osi[2, 0] = 0; osi[2, 1] = 200; osi[2, 2] = 1;
            osi[3, 0] = 0; osi[3, 1] = -200; osi[3, 2] = 1;
        }
        private float[,] Multiply_matr(float[,] a, float[,] b)
        {
            int n = a.GetLength(0);
            int m = b.GetLength(1);
            int m_a = a.GetLength(1);
            if (m_a != b.GetLength(0)) throw new Exception("Матрицы нельзя перемножить!");
            float[,] r = new float[n, m];
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < m; j++)
                {
                    r[i, j] = 0;
                    for (int ii = 0; ii < m_a; ii++)
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
        private void button1_Click(object sender, EventArgs e)
        {
            k = pictureBox1.Width / 2;
            l = pictureBox1.Height / 2;
            DrawStaticAxes();
        }
        //Таемер для сдвига



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

        }
        //Поворот фигуры
       
        
        // Метод для умножения матриц 4x4
        private double[,] MultiplyMatrices(double[,] a, double[,] b)
        {
            double[,] result = new double[4, 4];
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    result[i, j] = 0;
                    for (int k = 0; k < 4; k++)
                    {
                        result[i, j] += a[i, k] * b[k, j];
                    }
                }
            }
            return result;

        }


        //Отражение фигуры относительно Х
        private void button11_Click(object sender, EventArgs e)
        {
           
            // Матрица отражения относительно оси X
            float[,] reflectX = new float[4, 4]
            {
                { 1, 0, 0, 0 },
                { 0, -1, 0, 0 },
                { 0, 0, 1, 0 },
                { 0, 0, 0, 1 }
            };

            // Применяем отражение к текущим координатам
            float[,] temp = Multiply_matr(kv, reflectX);

            // Обновляем текущие координаты фигуры
            for (int i = 0; i < 8; i++)
            {
                kv[i, 0] = temp[i, 0];
                kv[i, 1] = temp[i, 1];
                kv[i, 2] = temp[i, 2];
                kv[i, 3] = temp[i, 3];
            }

            // Рисуем отраженную фигуру с учетом текущего сдвига
            Draw_Kv();
        }

        private void button12_Click(object sender, EventArgs e)
        {
            
            // Матрица отражения относительно оси Y
            float[,] reflectY = new float[4, 4]
            {
                { -1, 0, 0, 0 },
                { 0, 1, 0, 0 },
                { 0, 0, 1, 0 },
                { 0, 0, 0, 1 }
            };

            // Применяем отражение к текущим координатам
            float[,] temp = Multiply_matr(kv, reflectY);

            // Обновляем текущие координаты фигуры
            for (int i = 0; i < 8; i++)
            {
                kv[i, 0] = temp[i, 0];
                kv[i, 1] = temp[i, 1];
                kv[i, 2] = temp[i, 2];
                kv[i, 3] = temp[i, 3];
            }

            // Рисуем отраженную фигуру с учетом текущего сдвига
            Draw_Kv();
        }

        private void button13_Click(object sender, EventArgs e)
        {
          
            // Матрица отражения относительно начала координат
            float[,] reflectOrigin = new float[4, 4]
            {
                { -1, 0, 0, 0 },
                { 0, -1, 0, 0 },
                { 0, 0, 1, 0 },
                { 0, 0, 0, 1 }
            };

            // Применяем отражение к текущим координатам
            float[,] temp = Multiply_matr(kv, reflectOrigin);

            // Обновляем текущие координаты фигуры
            for (int i = 0; i < 8; i++)
            {
                kv[i, 0] = temp[i, 0];
                kv[i, 1] = temp[i, 1];
                kv[i, 2] = temp[i, 2];
                kv[i, 3] = temp[i, 3];
            }

            // Рисуем отраженную фигуру с учетом текущего сдвига
            Draw_Kv();
        }

        //Очистка PictureBox1
        private void button3_Click(object sender, EventArgs e)
        {
            ClearDrawing();
        }

        private void button4_Click(object sender, EventArgs e) // Поворот вокруг X
        {
            rotationAngleX += 0.1;
            Draw_Kv();
        }

        private void button5_Click(object sender, EventArgs e) // Поворот вокруг Z
        {
            rotationAngleZ += 0.1;
            Draw_Kv();
        }

        private void button16_Click(object sender, EventArgs e) // Поворот вокруг Y (влево)
        {
            rotationAngleY -= 0.1;
            Draw_Kv();
        }

        private void button15_Click(object sender, EventArgs e) // Поворот вокруг Y (вправо)
        {
            rotationAngleY += 0.1;
            Draw_Kv();
        }


        private void button14_Click(object sender, EventArgs e)
        {
            k = pictureBox1.Width / 2;
            l = pictureBox1.Height / 2;
            ResetFigure(); // Возвращаем фигуру в исходное положение и центр
        }
        private float[,] CreateRotationXMatrix(double angle)
        {
            float cos = (float)Math.Cos(angle);
            float sin = (float)Math.Sin(angle);

            return new float[4, 4]
            {
        { 1, 0,    0,     0 },
        { 0, cos, -sin,   0 },
        { 0, sin,  cos,   0 },
        { 0, 0,    0,     1 }
            };
        }

        // Матрица поворота вокруг оси Y
        private float[,] CreateRotationYMatrix(double angle)
        {
            float cos = (float)Math.Cos(angle);
            float sin = (float)Math.Sin(angle);

            return new float[4, 4]
            {
        { cos,  0, sin, 0 },
        { 0,    1, 0,   0 },
        { -sin, 0, cos, 0 },
        { 0,    0, 0,   1 }
            };
        }

        // Матрица поворота вокруг оси Z
        private float[,] CreateRotationZMatrix(double angle)
        {
            float cos = (float)Math.Cos(angle);
            float sin = (float)Math.Sin(angle);

            return new float[4, 4]
            {
        { cos, -sin, 0, 0 },
        { sin,  cos, 0, 0 },
        { 0,    0,   1, 0 },
        { 0,    0,   0, 1 }
            };
        }
    }
}
