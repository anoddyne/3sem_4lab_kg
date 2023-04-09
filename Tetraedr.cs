using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Tetraedr_task
{
    public class Tetraedr
    {
        public double Xmin;    // прямоугольник
        public double Xmax;
        public double Ymin;
        public double Ymax;
        public int I2;
        public int J2;
        public bool fTypeFace = false; // true - Line, false - Face
        public double Alf;
        public double Bet;
        public static bool visibleOXYZ = false;
        public static bool flRotate = false;
        public static byte flEdge = 0;
        public Bitmap bitmap;
        public SolidBrush myBrush;
        public Font myFont = new Font("Courier", 10);
        public static Body body;
        public static Body body0;

        public Tetraedr(int VW, int VH)
        {
            double scale = (double)VW / VH;
            Xmax = Math.Round(2 * scale);
            Xmin = -Xmax;
            Ymin = -2;
            Ymax = 2;
            Alf = 4.31;
            Bet = 4.92;
            fTypeFace = false;
            I2 = VW;
            J2 = VH;
            bitmap = new Bitmap(VW, VH);
            myBrush = new SolidBrush(Color.White);
            body = new Body(0);
            body0 = new Body(1);
        }

        double[] ToVector(double x, double y, double z)
        {
            double[] result = new double[4];
            result[0] = x;
            result[1] = y;
            result[2] = z;
            result[3] = 1;
            return result;
        }

        double[] VM_Mult(double[] A, double[][] B)
        {
            double[] result = new double[4];
            for (int j = 0; j < 4; j++)
            {
                result[j] = A[0] * B[0][j];
                for (int k = 1; k < 4; k++)
                    result[j] += A[k] * B[k][j];
            }
            if (result[3] != 0)
                for (int j = 0; j < 3; j++)
                    result[j] /= result[3];
            result[3] = 1;
            return result;
        }

        public double[] Rotate(double[] V, int k, double fi) //поворот
        {
            double[][] M = new double[4][];

            for (int i = 0; i < 4; i++)
                M[i] = new double[4];

            for (int i = 0; i < 4; i++)
            {
                M[3][i] = 0;
                M[i][3] = 0;
            }
            M[3][3] = 1;
            switch (k)
            {
                case 0:
                    M[0][0] = 1; M[0][1] = 0; M[0][2] = 0;
                    M[1][0] = 0; M[1][1] = Math.Cos(fi); M[1][2] = Math.Sin(fi);
                    M[2][0] = 0; M[2][1] = -Math.Sin(fi); M[2][2] = Math.Cos(fi);
                    break;
                case 1:
                    M[0][0] = Math.Cos(fi); M[0][1] = 0; M[0][2] = -Math.Sin(fi);
                    M[1][0] = 0; M[1][1] = 1; M[1][2] = 0;
                    M[2][0] = Math.Sin(fi); M[2][1] = 0; M[2][2] = Math.Cos(fi);
                    break;
                case 2:
                    M[0][0] = Math.Cos(fi); M[0][1] = Math.Sin(fi); M[0][2] = 0;
                    M[1][0] = -Math.Sin(fi); M[1][1] = Math.Cos(fi); M[1][2] = 0;
                    M[2][0] = 0; M[2][1] = 0; M[2][2] = 1;
                    break;
            }
            return VM_Mult(V, M);
        }

        int II(double x)
        {
            return (int)Math.Round((x - Xmin) * I2 / (Xmax - Xmin));
        }
        int JJ(double y)
        {
            return (int)Math.Round((y - Ymax) * J2 / (Ymin - Ymax));
        }

        Point IJ(double[] Vt)
        {
            Point result;
            Vt = Rotate(Vt, 0, Alf);
            Vt = Rotate(Vt, 1, Bet);
            result = new Point(II(Vt[0]), JJ(Vt[1]));
            return result;
        }
        
        double[] Norm(double[] V1, double[] V2, double[] V3)
        {
            double[] Result = new double[4];
            double[] A = new double[4];
            double[] B = new double[4];
            A[0] = V2[0] - V1[0]; A[1] = V2[1] - V1[1]; A[2] = V2[2] - V1[2];
            B[0] = V3[0] - V1[0]; B[1] = V3[1] - V1[1]; B[2] = V3[2] - V1[2];
            double u = A[1] * B[2] - A[2] * B[1];
            double v = -A[0] * B[2] + A[2] * B[0];
            double w = A[0] * B[1] - A[1] * B[0];

            double d = Math.Sqrt(u * u + v * v + w * w);
            if (d != 0)
            {
                Result[0] = u / d;
                Result[1] = v / d;
                Result[2] = w / d;
            }
            else
            {
                Result[0] = 0;
                Result[1] = 0;
                Result[2] = 0;
            }
            return Result;
        }

        void OXYZ(Graphics g) //видимость ребер
        {
            Point P0, P1;
            P0 = IJ(ToVector(-3, 0, 0));
            P1 = IJ(ToVector(3, 0, 0));
            g.DrawLine(Pens.Silver, P0.X, P0.Y, P1.X, P1.Y);
            g.DrawString("X", myFont, Brushes.Black, P1);
            P0 = IJ(ToVector(0, -3, 0));
            P1 = IJ(ToVector(0, 3, 0));
            g.DrawLine(Pens.Silver, P0.X, P0.Y, P1.X, P1.Y);
            g.DrawString("Y", myFont, Brushes.Black, P1);
            P0 = IJ(ToVector(0, 0, -3));
            P1 = IJ(ToVector(0, 0, 3));
            g.DrawLine(Pens.Silver, P0.X, P0.Y, P1.X, P1.Y);
            g.DrawString("Z", myFont, Brushes.Black, P1);
        }
        
        public void DrawBody(Graphics g)
        {
            double[][] Wn = new double[3][];
            int L1 = body.Faces.Length;
            int L0 = body.Faces[0].p.Length;
            Point[] w = new Point[L0];
            for (int i = 0; i < L1; i++)
            {
                for (int j = 0; j < L0; j++)
                {
                    double[] Vt = body.Vertexs[body.Faces[i].p[j]];
                    Vt = Rotate(Vt, 0, Alf);
                    Vt = Rotate(Vt, 1, Bet);
                    if (j <= 2) Wn[j] = Vt;
                    w[j].X = II(Vt[0]);
                    w[j].Y = JJ(Vt[1]);
                }
                double[] NN = Norm(Wn[0], Wn[1], Wn[2]);
                Pen pb = new Pen(Color.Black, 2);
                Pen pg = new Pen(Color.Gray);
                if (NN[2] < 0)
                    g.DrawPolygon(pb, w);
                else
                    g.DrawPolygon(pg, w);
            }
        }

        public void Draw()
        {
            I2 = bitmap.Width;
            J2 = bitmap.Height;
            using (Graphics g = Graphics.FromImage(bitmap))
            {
                Color cl;
                if (flEdge == 2)
                    cl = Color.FromArgb(0, 0, 0);
                else
                    cl = Color.FromArgb(255, 255, 255);
                g.Clear(cl);
                g.SmoothingMode = SmoothingMode.HighQuality;

                if (visibleOXYZ)
                    OXYZ(g);
                DrawBody(g);
            }
        }

        public void ChangeWindowXY(int Delta)
        {
            double coeff;
            if (Delta < 0)
                coeff = 1.03;
            else
                coeff = 0.97;
            Xmin *= coeff;
            Xmax *= coeff;
            Ymin *= coeff;
            Ymax *= coeff;
        }

        public void SetAngle(double x, double y)
        {
            Alf = Math.Atan2(y, x);
            Bet = Math.Sqrt((x / 10) * (x / 10) + (y / 10) * (y / 10));
        }

    }
    // ========== Body =================================================
    public struct Face
    {
        public int[] p;            // номера вершин
        public double A, B, C, D;  // коэффициенты уравнения плоскости
        public double[] N;         // вектор нормали к грани
    }
    public struct TEdge
    {
        public int p1, p2;         // номера вершин
    }

    public class Body
    {
        public double SizeBody = 0.3;
        public double[][] Vertexs;   // массив вершин всех тел
        public double[][] VertexsT;  // массив вершин всех тел
        public TEdge[] Edges;        // массив ребер всех тел
        public Face[] Faces;        // массив граней всех тел
        public Body(byte fl)
        {
            Tetraedr(SizeBody);
        }

        private void Tetraedr(double Size)
        {
            // создание двумерного массива вершин 
            Vertexs = new double[4][];
            VertexsT = new double[4][];
            for (int i = 0; i < 4; i++)
            {
                Vertexs[i] = new double[4];
                VertexsT[i] = new double[4];
            }
            // заполнение массива вершин (задание однородных координат вершин)
            Vertexs[0][0] = Size; Vertexs[0][1] = -Size; Vertexs[0][2] = -Size;
            Vertexs[1][0] = Size; Vertexs[1][1] = Size; Vertexs[1][2] = Size;
            Vertexs[2][0] = -Size; Vertexs[2][1] = -Size; Vertexs[2][2] = Size;
            Vertexs[3][0] = -Size; Vertexs[3][1] = Size; Vertexs[3][2] = -Size;
            for (int i = 0; i < 4; i++)
                Vertexs[i][3] = 1;
            // создание массива ребер (задаются номера вершин, соединяемых ребром)
            Edges = new TEdge[6];
            Edges[0].p1 = 0; Edges[0].p2 = 1;
            Edges[1].p1 = 0; Edges[1].p2 = 2;
            Edges[2].p1 = 1; Edges[2].p2 = 2;
            Edges[3].p1 = 3; Edges[3].p2 = 0;
            Edges[4].p1 = 3; Edges[4].p2 = 1;
            Edges[5].p1 = 3; Edges[5].p2 = 2;
            // создание массива граней (задаются номера вершин, инцидентных грани)
            Faces = new Face[4];
            for (int i = 0; i < 4; i++)
                Faces[i].p = new int[3];
            Faces[0].p[0] = 0; Faces[0].p[1] = 1; Faces[0].p[2] = 2;
            Faces[1].p[0] = 1; Faces[1].p[1] = 3; Faces[1].p[2] = 2;
            Faces[2].p[0] = 0; Faces[2].p[1] = 2; Faces[2].p[2] = 3;
            Faces[3].p[0] = 0; Faces[3].p[1] = 3; Faces[3].p[2] = 1;
        }
    }
}
