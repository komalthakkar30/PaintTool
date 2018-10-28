using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace thakkar_Assign4
{
    public partial class Form1 : Form
    {
        Graphics g;
        Image File;
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
                g.DrawLine(myPen, start_Point, end_Point);
            }
            else if(start_Point == end_Point && Erase_Button.FlatStyle != FlatStyle.Flat)
            {
                var width = (Pencil_Button.FlatStyle == FlatStyle.Flat) ? (float)Pencil_NumericUpDown.Value : (float)Brush_NumericUpDown.Value;
                SolidBrush brush = new SolidBrush(CurrentColorButton.BackColor);
                g.FillEllipse(brush, start_Point.X - 4, start_Point.Y - 4, width, width);
            }
        }

        private void OpenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                File = Image.FromFile(openFileDialog1.FileName);
                PictureBox1.Image = File;
            }
        }

        private void SaveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            saveFileDialog1.ShowDialog();
        }

        private void NewToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void ExitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void UndoToolStripMenuItem_Click(object sender, EventArgs e)
        {
        }
    }
}
