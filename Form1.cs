using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Paint2._0
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            this.Width = 900;
            this.Height = 700;
            bm = new Bitmap(this.Width, this.Height);
            g = Graphics.FromImage(bm);
            g.Clear(Color.White);
            pic.Image = bm;
            p.StartCap = System.Drawing.Drawing2D.LineCap.Round;//сглаживание линии
            p.EndCap = System.Drawing.Drawing2D.LineCap.Round;// сглаживание линии
        }
        Bitmap bm;
        Graphics g;
        bool paint = false;
        Point px, py;
        Pen p = new Pen(Color.Black, 1);
        Pen erase = new Pen(Color.White, 40);
        int index;
        int x, y, sY, sX, Cy, cX;

        ColorDialog cd = new ColorDialog();
        Color new_color;

        private void btn_fill_Click(object sender, EventArgs e)// кнопка заполнения цветом
        {
            index = 7;
        }

        private void btn_ellipse_Click(object sender, EventArgs e)// кнопка вставления окружности
        {
            index = 3;
        }

        private void btn_rect_Click(object sender, EventArgs e)// добавление прямоугольника
        {
            index = 4;
        }

        private void btn_line_Click(object sender, EventArgs e)// добавление линии
        {
            index = 5;
        }

        private void pic_Paint(object sender, PaintEventArgs e)//реализация отрисовки фигур(линии, окружности, прямоугольника) при добавлении  до отпускании клавиши
        {
            Graphics g =e.Graphics;
            if(paint)
            {
                if (index == 3)
                {
                    g.DrawEllipse(p, cX, Cy, sX, sY);
                }
                if (index == 4)
                {
                    g.DrawRectangle(p, cX, Cy, sX, sY);
                }
                if (index == 5)
                {
                    g.DrawLine(p, cX, Cy, x, y);
                }
            }
        }

        private void btn_clear_Click(object sender, EventArgs e) // очистка
        {
            g.Clear(Color.White);
            pic.Image = bm;
            index = 0;
        }

        private void pic_MouseUp(object sender, MouseEventArgs e)// реализации добавлении фигур(прямоугольника, окружности, линии)
        {
            paint = false;

            sX = x - cX;
            sY = y - Cy;

            if(index ==3)
            {
                g.DrawEllipse(p,cX,Cy,sX,sY);
            }
            if (index == 4)
            {
                g.DrawRectangle(p, cX, Cy, sX, sY);
            }
            if (index == 5)
            {
                g.DrawLine(p, cX, Cy, x, y);
            }

        }

        private void color_picker_MouseClick(object sender, MouseEventArgs e)// выбор цвета из палитры путем получения цвета пикселей из изображения палитры
        {
            Point point = set_point(color_picker, e.Location);
            pic_color.BackColor = ((Bitmap)color_picker.Image).GetPixel(point.X, point.Y);
            new_color = pic_color.BackColor;
            p.Color = pic_color.BackColor;
        }

        private void pic_MouseMove(object sender, MouseEventArgs e) // отрисовка при ведении мыши
        {
            if(paint)
            {

                if(index ==1)
                {
                    px = e.Location;
                    g.DrawLine(p, px, py);
                    py = px;
                }
                if (index == 2)
                {
                    px = e.Location;
                    g.DrawLine(erase, px, py);
                    py = px;
                }
            }
            pic.Refresh();

            x = e.X;
            y = e.Y;
            sX = e.X - cX;
            sY = e.Y - Cy;

        }

        private void pic_MouseDown(object sender, MouseEventArgs e)// отработка нажатия
        {
            paint = true;
            py = e.Location;

            cX = e.X;
            Cy= e.Y;
        }

        private void pic_MouseClick(object sender, MouseEventArgs e)// заполнение цветом 
        {
            if(index==7)
            {
                Point point = set_point(pic, e.Location);
                Fill(bm,point.X, point.Y, new_color);
            }
        }

        private void btn_save_click(object sender, EventArgs e)// сохранение 
        {
            if (pic.Image != null) //если в pictureBox естьизображение
            {
                SaveFileDialog savedialog = new SaveFileDialog();
                savedialog.Title = "Сохранитькартинкукак...";
                savedialog.OverwritePrompt = true;
                savedialog.CheckPathExists = true;
                savedialog.Filter = "jpg files (*.jpg)|*.jpg|png files (*.png)|*.png|bmp files (*.bmp)|*.bmp";
                savedialog.ShowHelp = true;
                if (savedialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        //pictureBox1.Image.Save(savedialog.FileName, System.Drawing.Imaging.ImageFormat.Jpeg);
                        pic.Image.Save(savedialog.FileName);
                    }
                    catch
                    {
                        MessageBox.Show("Невозможно сохранить изображение", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void trackBar1_ValueChanged(object sender, EventArgs e) // изменение толщины через трэкбар
        {
            p.Width = trackBar1.Value;
        }

        private void btn_pencil_Click(object sender, EventArgs e)// выбор ручки
        {
            index = 1;
        }

        private void btn_eraser_Click(object sender, EventArgs e)// выбор стерки
        {
            index = 2;
        }

        private void btn_color_Click(object sender, EventArgs e)// выборо цвета через встроенную форму
        {
            cd.ShowDialog();
            new_color = cd.Color;
            pic_color.BackColor = cd.Color;
            p.Color = cd.Color;


        }

        static Point set_point(PictureBox pb,Point pt)// получение крайних точек для заполнения
        {
            float px = 1f * pb.Image.Width / pb.Width;
            float py = 1f *pb.Image.Height / pb.Height;
            return new Point((int)(pt.X*px),(int)(pt.Y*py));

        }
        private void validate(Bitmap bm,Stack<Point>sp,int x, int y, Color old_color,Color new_color)
        {
            Color cx=bm.GetPixel(x,y);
            if(cx == old_color) 
            {
                sp.Push(new Point(x,y));
                bm.SetPixel(x, y, new_color);
            }

        }
        public void Fill(Bitmap bm,int x,int y,Color new_clr)// заполнение
        {
            Color old_color = bm.GetPixel(x,y);
            Stack<Point> pixel = new Stack<Point>();
            pixel.Push(new Point(x,y));
            bm.SetPixel(x, y, new_clr);
            if (old_color == new_clr) return;
            while(pixel.Count > 0) 
            { 
                Point pt = (Point)pixel.Pop();
                if(pt.X>0 && pt.Y>0 && pt.X<bm.Width-1 && pt.Y<bm.Height-1) 
                {
                    validate(bm, pixel, pt.X - 1, pt.Y , old_color, new_clr);
                    validate(bm, pixel, pt.X, pt.Y - 1, old_color, new_clr);
                    validate(bm, pixel, pt.X + 1, pt.Y , old_color, new_clr);
                    validate(bm, pixel, pt.X , pt.Y + 1, old_color, new_clr);



                }
            }
        }
    }
}
