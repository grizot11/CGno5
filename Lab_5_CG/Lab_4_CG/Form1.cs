using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Lab_4_CG
{
    public partial class Form1 : Form
    {

        public int counter;
        public List<Point> ListInputPoints = new List<Point>();
        Graphics gr;
        public int leftX, rightX, topY, bottomY;
        public List<Point> ListOutputPoints = new List<Point>();
        Bitmap background, backgroundTemp;
        private void ClearButton_Click(object sender, EventArgs e)
        {
            Clear();
        }
        private void OrthogonalWindow()
        {
            Pen pen = new Pen(Color.DarkBlue, 0.5f);
            gr = Graphics.FromImage(background);
            gr.DrawLine(pen, leftX, topY, rightX, topY);
            gr.DrawLine(pen, leftX, topY, leftX, bottomY);
            gr.DrawLine(pen, rightX, bottomY, rightX, topY);
            gr.DrawLine(pen, rightX, bottomY, leftX, bottomY);
            this.BackgroundImage = background;
            this.Refresh();
        }
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            leftX = 130;
            rightX = 650;
            topY = 140;
            bottomY = 560;
            counter = -1;
            gr = this.CreateGraphics();
            background = new Bitmap(this.ClientSize.Width, this.ClientSize.Height, gr);
            backgroundTemp = new Bitmap(this.ClientSize.Width, this.ClientSize.Height, gr);
            OrthogonalWindow();
            buttonStart.Visible = false;
        }
        private bool insidePolygon(Point p)
        {
            if ((p.X < leftX) || (p.X > rightX) || (p.Y > bottomY) || (p.Y < topY)) return false;
            else return true;
        }
        private void Satherland_Hodgman()
        {
            for (int p = 0;p<ListInputPoints.Count-1;p++)
            {
                if (insidePolygon(ListInputPoints[p]))
                {
                    if (insidePolygon(ListInputPoints[p+1]))
                    {
                        ListOutputPoints.Add(ListInputPoints[p + 1]);
                    }
                    else
                    {
                        ListOutputPoints.Add(Satherland_Coen(ListInputPoints[p+1], ListInputPoints[p]));
                    }
                }
                else if (insidePolygon(ListInputPoints[p + 1]))
                {
                    ListOutputPoints.Add(Satherland_Coen(ListInputPoints[p], ListInputPoints[p + 1]));
                    ListOutputPoints.Add(ListInputPoints[p + 1]);
                }        
            }
            if (insidePolygon(ListInputPoints.Last()))
            {
                if (insidePolygon(ListInputPoints[0]))
                {
                    ListOutputPoints.Add(ListInputPoints[0]);
                }
                else
                {
                    ListOutputPoints.Add(Satherland_Coen(ListInputPoints[0], ListInputPoints.Last()));
                }
            }
            else if (insidePolygon(ListInputPoints[0]))
            {
                ListOutputPoints.Add(Satherland_Coen(ListInputPoints.Last(), ListInputPoints[0]));
            }

            Pen pen = new Pen(Color.Orange, 0.5f);
            gr = Graphics.FromImage(background);
            for (int p = 0; p < ListOutputPoints.Count; p++)
            {
                if (p != ListOutputPoints.Count - 1)
                    gr.DrawLine(pen, ListOutputPoints[p], ListOutputPoints[p+1]);
                else gr.DrawLine(pen, ListOutputPoints[p], ListOutputPoints[0]);
            }
            this.BackgroundImage = background;
            this.Refresh();

        }
        private Point Satherland_Coen(Point FirstPoint, Point SecondPoint)
        {
            Point ResPoint = FirstPoint;
            if (ResPoint.X < leftX)
            {
                ResPoint.Y += (FirstPoint.Y - SecondPoint.Y) * (leftX - ResPoint.X) / (FirstPoint.X - SecondPoint.X);
                ResPoint.X = leftX;
            }
            else if (ResPoint.X > rightX)
            {
                ResPoint.Y += (FirstPoint.Y - SecondPoint.Y) * (rightX - ResPoint.X) / (FirstPoint.X - SecondPoint.X);
                ResPoint.X = rightX;
            }
            else if (ResPoint.Y < topY)
            {
                ResPoint.X += (FirstPoint.X - SecondPoint.X) * (topY - ResPoint.Y) / (FirstPoint.Y - SecondPoint.Y);
                ResPoint.Y = topY;
            }
            else if (ResPoint.Y > bottomY)
            {
                ResPoint.X += (FirstPoint.X - SecondPoint.X) * (bottomY - ResPoint.Y) / (FirstPoint.Y - SecondPoint.Y);
                ResPoint.Y = bottomY;
            }
            return ResPoint;

        }
        private void buttonStart_Click(object sender, EventArgs e)
        {
            Satherland_Hodgman();
            buttonStart.Visible = false;

        }
        private bool PolygonIsValid(Point p0, Point p1)
        {
            //Изменения координат
            int dx = (p1.X > p0.X) ? (p1.X - p0.X) : (p0.X - p1.X);
            int dy = (p1.Y > p0.Y) ? (p1.Y - p0.Y) : (p0.Y - p1.Y);
            //Направление приращения
            int sx = (p1.X >= p0.X) ? (1) : (-1);
            int sy = (p1.Y >= p0.Y) ? (1) : (-1);

            if (dy < dx)
            {
                int d = (dy << 1) - dx;
                int d1 = dy << 1;
                int d2 = (dy - dx) << 1;
                background.SetPixel(p0.X, p0.Y, Color.Black);
               
                int x = p0.X + sx;
                int y = p0.Y;
                for (int i = 1; i <= dx; i++)
                {
                    if (d > 0)
                    {
                        d += d2;
                        y += sy;
                    }
                    else
                        d += d1;
                    if ("ff000000"==background.GetPixel(x,y).Name && new Point(x, y) != p1) return false;
                    background.SetPixel(x, y, Color.Black);
                    
                    x += sx;
                }
            }
            else
            {
                int d = (dx << 1) - dy;
                int d1 = dx << 1;
                int d2 = (dx - dy) << 1;
                background.SetPixel(p0.X, p0.Y, Color.Black);
                
                int x = p0.X;
                int y = p0.Y + sy;
                for (int i = 1; i <= dy; i++)
                {
                    if (d > 0)
                    {
                        d += d2;
                        x += sx;
                    }
                    else
                        d += d1;
                    if ("ff000000" == background.GetPixel(x, y).Name && new Point(x,y) != p1) return false;
                    background.SetPixel(x, y, Color.Black);
                   
                    y += sy;
                }
            }
            return true;
        }

        private void Form1_MouseClick(object sender, MouseEventArgs e)
        {
            counter++;
            ListInputPoints.Add(new Point(e.X, e.Y));
            if (counter >= 1)
            {
                if (!PolygonIsValid(ListInputPoints[counter-1], ListInputPoints[counter]))
                {
                     Clear();
                    //ListInputPoints.RemoveAt(counter);
                    //counter--;
                    MessageBox.Show("Отмеченные вами точки не позволяют построить многоугольник с непересекающимися ребрами!");
                }
                else
                {
                    this.BackgroundImage = background;
                    this.Refresh();
                }
            }
        }
        private void Draw(Point f, Point s, Color c)
        {
            Pen pen = new Pen(c, 0.5f);
            gr = Graphics.FromImage(background);
            gr.DrawLine(pen, f, s);
            this.BackgroundImage = background;
            this.Refresh();
        }

        private void EndButton_Click(object sender, EventArgs e)
        {
            if (counter >= 2)
            {
                if (!PolygonIsValid(ListInputPoints[counter], ListInputPoints[0]))
                {
                     Clear();
                  
                    MessageBox.Show("Отмеченные вами точки не позволяют построить многоугольник с непересекающимися ребрами!");
                }
                else
                {
                    this.BackgroundImage = background;
                    this.Refresh();
                }
                buttonStart.Visible = true;
                counter = -1;
            }
            else MessageBox.Show("Добавьте хотя бы одну точку");
        }

        private void Clear()
        {
            counter = -1;
            ListInputPoints.Clear();
            background = new Bitmap(backgroundTemp);
            OrthogonalWindow();
        }



    }
}
