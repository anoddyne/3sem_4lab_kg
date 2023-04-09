using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Tetraedr_task
{
    public partial class FormMain : Form
    {
        public static Tetraedr Platon;   // Класс платоновых тел
        bool drawing = false;
        static Graphics g;          // Класс, инкапсулирующий поверхность рисования
        public static bool flMove = false;
        MouseEventArgs e0;          // Класс, представляющий данные для событий Mouse*
        public FormMain()
        {
            InitializeComponent();
        }

        private void FormMain_Load(object sender, EventArgs e)
        {
            MouseWheel += new MouseEventHandler(FormMain_MouseWheel);
            g = CreateGraphics();
            Platon = new Tetraedr(ClientRectangle.Width, ClientRectangle.Height);
        }

        void FormMain_MouseWheel(object sender, MouseEventArgs e)
        {
            Platon.ChangeWindowXY(e.Delta);
            MyDraw();
        }
        public static void MyDraw()
        {
            Platon.Draw();
            g.DrawImage(Platon.bitmap, Program.formMain.ClientRectangle);
        }

        private void FormMain_Paint(object sender, PaintEventArgs e)
        {
            MyDraw();
        }

        private void FormMain_MouseDown(object sender, MouseEventArgs e)
        {
            drawing = true;
        }

        private void FormMain_MouseMove(object sender, MouseEventArgs e)
        {
            if (drawing)
            {
                if (e.Button == MouseButtons.Left)
                {
                    double x = e.X - ClientRectangle.Width / 2;
                    double y = e.Y - ClientRectangle.Height / 2;
                    Platon.SetAngle(x, y);
                }
                else
                    e0 = e;
                MyDraw();
            }
        }

        private void FormMain_MouseUp(object sender, MouseEventArgs e)
        {
            drawing = false;
        }

        private void FormMain_ResizeEnd(object sender, EventArgs e)
        {
            g = CreateGraphics();
            Platon = new Tetraedr(ClientRectangle.Width, ClientRectangle.Height);
            MyDraw();
        }
    }
}
