using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Forms;

namespace thakkar_Assign4
{
    public partial class Form1 : Form
    {
        Graphics g;
        Image imageFile;
        Pen myPen = new Pen(Color.Black);
        Point start_Point = new Point(0, 0);
        Point end_Point = new Point(0, 0);
        bool mouseIsDown = false;

        public Form1()
        {
            InitializeComponent();
            Pencil_Button.FlatStyle = FlatStyle.Flat;
            CurrentColorButton.BackColor = Color.Black;
        }

        private void Color_button_Click(object sender, EventArgs e)
        {
            Button myButton = sender as Button;
            CurrentColorButton.BackColor = myButton.BackColor;
        }

        private void Custom_Color_button_Click(object sender, EventArgs e)
        {
            if (colorDialog1.ShowDialog() != DialogResult.Cancel)
            {
                CurrentColorButton.BackColor = colorDialog1.Color;
            }
        }

        private void Draw_Button_Click(object sender, EventArgs e)
        {
            Button featureButton = sender as Button;
            Brush_Button.FlatStyle = FlatStyle.Standard;
            Pencil_Button.FlatStyle = FlatStyle.Standard;
            Line_Button.FlatStyle = FlatStyle.Standard;
            Erase_Button.FlatStyle = FlatStyle.Standard;

            featureButton.FlatStyle = FlatStyle.Flat;
        }

        private void PictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            start_Point = e.Location;
            if (e.Button == MouseButtons.Left)
            {
                mouseIsDown = true;
            }
        }

        private void PictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            end_Point = e.Location;
            if (Line_Button.FlatStyle != FlatStyle.Flat)
            {
                if (mouseIsDown)
                {
                    g = PictureBox1.CreateGraphics();
                    if (Pencil_Button.FlatStyle == FlatStyle.Flat)
                    {
                        myPen.Width = (float)Pencil_NumericUpDown.Value;
                        myPen.Color = CurrentColorButton.BackColor;
                    }
                    else if (Brush_Button.FlatStyle == FlatStyle.Flat)
                    {
                        myPen.Width = (float)Brush_NumericUpDown.Value;
                        myPen.Color = CurrentColorButton.BackColor;
                    }
                    else if (Erase_Button.FlatStyle == FlatStyle.Flat)
                    {
                        myPen.Width = 5;
                        myPen.Color = PictureBox1.BackColor;
                    }
                    g.DrawLine(myPen, start_Point, end_Point);
                }
                start_Point = end_Point;
            }
        }

        private void PictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            mouseIsDown = false;
            g = PictureBox1.CreateGraphics();
            myPen.Color = CurrentColorButton.BackColor;

            if (Line_Button.FlatStyle == FlatStyle.Flat)
            {
                myPen.Width = 3;
                g.DrawLine(myPen, start_Point, end_Point);
            }
            else if(start_Point == end_Point && Erase_Button.FlatStyle != FlatStyle.Flat)
            {
                var width = (Pencil_Button.FlatStyle == FlatStyle.Flat) ? (float)Pencil_NumericUpDown.Value : (float)Brush_NumericUpDown.Value;
                SolidBrush brush = new SolidBrush(CurrentColorButton.BackColor);
                g.FillEllipse(brush, start_Point.X - 4, start_Point.Y - 4, width, width);
            }
        }

        /*******************    File Management Features    ******************/

        private void NewToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void OpenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                imageFile = Image.FromFile(openFileDialog1.FileName);
                PictureBox1.Image = imageFile;
            }
        }

        private void SaveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            saveFileDialog1.ShowDialog();
            if (saveFileDialog1.FileName != "")
            {
                using (FileStream fs = (FileStream)saveFileDialog1.OpenFile())
                {
                    switch (saveFileDialog1.FilterIndex)
                    {
                        case 1:
                            PictureBox1.Image.Save(fs, ImageFormat.Jpeg);
                            break;

                        case 2:
                            PictureBox1.Image.Save(fs, ImageFormat.Bmp);
                            break;

                        case 3:
                            PictureBox1.Image.Save(fs, ImageFormat.Png);
                            break;
                    }
                }
            }
        }

        private void ExitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void UndoToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void RedoToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }
    }
}
