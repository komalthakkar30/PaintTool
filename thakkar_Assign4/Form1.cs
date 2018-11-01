using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Forms;

namespace thakkar_Assign4
{
    public partial class Form1 : Form
    {
        Bitmap bmp;
        Graphics g;
        Image imageFile;
        Pen myPen = new Pen(Color.Black);
        Point start_Point = new Point(0, 0);
        Point end_Point = new Point(0, 0);
        bool mouseIsDown = false;
        String FileName = "";
        private Stack<Image> _undoStack = new Stack<Image>();
        private Stack<Image> _redoStack = new Stack<Image>();

        public Form1()
        {
            InitializeComponent();
            Pencil_Button.FlatStyle = FlatStyle.Flat;
            CurrentColorButton.BackColor = Color.Black;
            bmp = new Bitmap(PictureBox1.Width, PictureBox1.Height);
            //g = PictureBox1.CreateGraphics();
            g = Graphics.FromImage(bmp);

            if (_undoStack.Count == 0)
            {
                UndoToolStripMenuItem.Enabled = false;
                _undoStack.Push(PictureBox1.Image);
            }
            if (_redoStack.Count == 0)
            {
                RedoToolStripMenuItem.Enabled = false;
            }
            //SaveSnapshot();
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
                    PictureBox1.Image = bmp;
                }
                start_Point = end_Point;
            }
        }

        private void PictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            var width = 3;
            mouseIsDown = false;
            myPen.Color = CurrentColorButton.BackColor;

            if (Line_Button.FlatStyle == FlatStyle.Flat)
            {
                width = 3;
                myPen.Width = width;
                g.DrawLine(myPen, start_Point, end_Point);
            }
            else if (start_Point == end_Point && Erase_Button.FlatStyle != FlatStyle.Flat)
            {
                width = (Pencil_Button.FlatStyle == FlatStyle.Flat) ? (int)Pencil_NumericUpDown.Value : (int)Brush_NumericUpDown.Value;
                SolidBrush brush = new SolidBrush(CurrentColorButton.BackColor);
                g.FillEllipse(brush, start_Point.X - 4, start_Point.Y - 4, width, width);
            }

            SaveSnapshot();
        }

        /*******************    File Management Features    ******************/

        private void NewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (PictureBox1.Image != null)
            {
                string message = "You did not save the existing filename. Save this file?";
                string caption = "Alert box";
                MessageBoxButtons buttons = MessageBoxButtons.YesNo;
                DialogResult result;

                // Displays the MessageBox.

                result = MessageBox.Show(message, caption, buttons);

                if (result == DialogResult.Yes)
                {

                    Bitmap bmp = new Bitmap(PictureBox1.Width, PictureBox1.Height);
                    Rectangle bounds = new Rectangle(Left, Top, Width, Height);
                    PictureBox1.DrawToBitmap(bmp, PictureBox1.ClientRectangle);

                    saveFileDialog1.ShowDialog();
                    bmp.Save(saveFileDialog1.FileName, ImageFormat.Png);
                    PictureBox1.Image.Dispose();
                    PictureBox1.Image = null;

                }
                if (result == DialogResult.No)
                {
                    PictureBox1.Image.Dispose();
                    PictureBox1.Image = null;
                }
            }
        }

        private void OpenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                using (imageFile = Image.FromFile(openFileDialog1.FileName))
                {
                    FileName = openFileDialog1.FileName;
                    PictureBox1.Image = imageFile;
                    PictureBox1.Refresh();
                    PictureBox1.DrawToBitmap(bmp, PictureBox1.ClientRectangle);

                    _undoStack.Clear();
                    UndoToolStripMenuItem.Enabled = false;
                    _undoStack.Push(PictureBox1.Image);
                    _redoStack.Clear();
                    RedoToolStripMenuItem.Enabled = false;
                }
            }
        }

        private void SaveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (FileName == "")
            {
                SaveAsToolStripMenuItem_Click(sender, e);
            }
            else
            {
                bmp.Save(FileName, ImageFormat.Png);
            }
        }

        private void SaveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Bitmap bmp = new Bitmap(PictureBox1.Width, PictureBox1.Height);
            PictureBox1.DrawToBitmap(bmp, PictureBox1.ClientRectangle);

            saveFileDialog1.RestoreDirectory = true;
            saveFileDialog1.ShowDialog();
            bmp.Save(saveFileDialog1.FileName, ImageFormat.Png);
            FileName = saveFileDialog1.FileName;
        }

        private void ExitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void UndoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _redoStack.Push(_undoStack.Pop());
            RedoToolStripMenuItem.Enabled = true;

            if (_undoStack.Count > 0)
            {
                PictureBox1.Image = _undoStack.Peek();
                PictureBox1.Refresh();
                //bmp = new Bitmap(PictureBox1.Image);
            }
            else
            {
                PictureBox1.Image = null;
                PictureBox1.Refresh();
                UndoToolStripMenuItem.Enabled = false;
            }
            //PictureBox1.DrawToBitmap(bmp, PictureBox1.ClientRectangle);
        }

        private void RedoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _undoStack.Push(_redoStack.Pop());
            UndoToolStripMenuItem.Enabled = true;
            //OnRedo();
            PictureBox1.Image = _undoStack.Peek();
            PictureBox1.Refresh();

            if(_redoStack.Count <= 0)
            {
                RedoToolStripMenuItem.Enabled = false;
            }
            PictureBox1.DrawToBitmap(bmp, PictureBox1.ClientRectangle);
        }
        private void SaveSnapshot()
        {
            //assign bmp to picturebox image to update
            PictureBox1.Image = bmp;
            PictureBox1.Update();

            if (_undoStack.Count == 0)
            {
                _redoStack.Clear();
                RedoToolStripMenuItem.Enabled = false;
            }
            _undoStack.Push(PictureBox1.Image);
            PictureBox1.DrawToBitmap(bmp, PictureBox1.ClientRectangle);
            UndoToolStripMenuItem.Enabled = true;
        }
    }
}
