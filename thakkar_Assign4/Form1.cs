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
        Pen myPen = new Pen(Color.Black);
        Point start_Point = new Point(0, 0);
        Point end_Point = new Point(0, 0);
        bool mouseIsDown = false;

        public Form1()
        {
            InitializeComponent();
            Pencil_Button.FlatStyle = FlatStyle.Flat;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
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

        private void Pencil_Button_Click(object sender, EventArgs e)
        {
            Brush_Button.FlatStyle = FlatStyle.Standard;
            Pencil_Button.FlatStyle = FlatStyle.Flat;
        }

        private void Brush_Button_Click(object sender, EventArgs e)
        {
            Pencil_Button.FlatStyle = FlatStyle.Standard;
            Brush_Button.FlatStyle = FlatStyle.Flat;
        }

        private void PictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            start_Point = e.Location;
            if (e.Button == MouseButtons.Left)
                mouseIsDown = true;
        }

        private void PictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            if (mouseIsDown)
            {
                if (Pencil_Button.FlatStyle == FlatStyle.Flat)
                {
                    myPen.Width = (float)Pencil_NumericUpDown.Value;
                }
                else if(Brush_Button.FlatStyle == FlatStyle.Flat)
                {
                    myPen.Width = (float)Brush_NumericUpDown.Value;
                }
                end_Point = e.Location;
                g = PictureBox1.CreateGraphics();
                myPen.Color = CurrentColorButton.BackColor;
                g.DrawLine(myPen, end_Point, start_Point);
            }
            start_Point = end_Point;
        }

        private void PictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            mouseIsDown = false;
        }

        private void OpenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openFileDialog1.ShowDialog();
        }

        private void SaveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            saveFileDialog1.ShowDialog();
        }
    }
}
