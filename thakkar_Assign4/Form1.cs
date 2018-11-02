using System;
using System.Collections.Generic;
using System.Drawing;
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
        bool isNewButtonClicked = false;
        private Stack<Image> _undoStack = new Stack<Image>();
        private Stack<Image> _redoStack = new Stack<Image>();
        private List<String> _recentFiles = new List<string>();


        /*******************    Defined Initial State and Behaviour    ******************/
        public Form1()
        {
            InitializeComponent();
            
            Pencil_Button.FlatStyle = FlatStyle.Flat;
            CurrentColorButton.BackColor = Color.Black;
            bmp = new Bitmap(PictureBox1.Width, PictureBox1.Height);
            g = Graphics.FromImage(bmp);

            if (_undoStack.Count == 0)
            {
                Undo_Button.Enabled = false;
                _undoStack.Push(new Bitmap(bmp));
            }
            if (_redoStack.Count == 0)
            {
                Redo_Button.Enabled = false;
            }

            using (StreamReader readFile = new StreamReader("..\\..\\RecentFiles.txt"))
            {
                String line = readFile.ReadLine();
                while (line != null)
                {
                    _recentFiles.Add(line);
                    line = readFile.ReadLine();
                    RecentFilesToolStripMenuItem.DropDownItems.Add(_recentFiles[_recentFiles.Count - 1]).Click += RecentFiles_MenuItem_Click;
                }
                if (_recentFiles.Count > 0)
                {
                    RecentFilesToolStripMenuItem.Enabled = true;
                }
            }
        }

        /*******************    Color Selection    ******************/
        #region color selection
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
        #endregion


        /*******************    Paint Tool Selection    ******************/
        #region paint tool
        private void Draw_Button_Click(object sender, EventArgs e)
        {
            Button featureButton = sender as Button;
            Brush_Button.FlatStyle = FlatStyle.Standard;
            Pencil_Button.FlatStyle = FlatStyle.Standard;
            Line_Button.FlatStyle = FlatStyle.Standard;
            Erase_Button.FlatStyle = FlatStyle.Standard;

            featureButton.FlatStyle = FlatStyle.Flat;
        }
        #endregion


        /*******************    Picturebox Mouse Events    ******************/
        #region Picturebox Mouse Events
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
        #endregion


        /*******************    File Management Features    ******************/
        #region File Management Features
        private void NewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (PictureBox1.Image != null)
            {
                string message = "You did not save the existing filename. Save this file?";
                string caption = "Alert box";
                MessageBoxButtons buttons = MessageBoxButtons.YesNo;

                // Displays the MessageBox

                isNewButtonClicked = true;
                if (MessageBox.Show(message, caption, buttons) == DialogResult.Yes)
                {
                    SaveToolStripMenuItem_Click(sender, e);
                }
                else
                {
                    ResetPictureBox();
                }
            }
        }

        private void OpenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                OpenFile(sender, e, false);
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
                if (isNewButtonClicked)
                {
                    ResetPictureBox();
                }
                SaveRecentFilesToolStripMenuItem(sender,e);
            }
        }

        private void SaveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult result = saveFileDialog1.ShowDialog();
            
            if (result == DialogResult.OK)
            {
                bmp.Save(saveFileDialog1.FileName, ImageFormat.Png);
                FileName = saveFileDialog1.FileName;
                if (isNewButtonClicked)
                {
                    ResetPictureBox();
                }
                SaveRecentFilesToolStripMenuItem(sender, e);
            }
        }

        private void ExitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (StreamWriter stringToWrite = new StreamWriter("..\\..\\RecentFiles.txt"))
            {
                for (int i = _recentFiles.Count - 1; i >= 0; i--)
                {
                    stringToWrite.WriteLine(_recentFiles[i]);
                }
                stringToWrite.Flush(); //write stream to file
            }
            Environment.Exit(0);
        }

        #endregion


        /*******************    Redo Undo Features    ******************/
        #region Redo Undo
        private void Undo_Button_Click(object sender, EventArgs e)
        {
            _redoStack.Push(_undoStack.Pop());
            Redo_Button.Enabled = true;

            bmp = new Bitmap(_undoStack.Peek());
            g = Graphics.FromImage(bmp);
            PictureBox1.Image = bmp;
            PictureBox1.Refresh();

            Undo_Button.Enabled = !(_undoStack.Count == 1);
        }

        private void Redo_Button_Click(object sender, EventArgs e)
        {
            _undoStack.Push(_redoStack.Pop());
            Undo_Button.Enabled = true;

            bmp = new Bitmap(_undoStack.Peek());
            g = Graphics.FromImage(bmp);
            PictureBox1.Image = bmp;
            PictureBox1.Refresh();

            Redo_Button.Enabled = !(_redoStack.Count == 0);
        }

        private void SaveSnapshot()
        {
            //assign bmp to picturebox image to update
            PictureBox1.Image = bmp;
            PictureBox1.Update();

            if (_redoStack.Count > 0)
            {
                _redoStack.Clear();
                Redo_Button.Enabled = false;
            }
            _undoStack.Push(new Bitmap(bmp));
            Undo_Button.Enabled = true;
        }

        private void ResetPictureBox()
        {
            FileName = "";
            bmp = new Bitmap(PictureBox1.Width, PictureBox1.Height);
            g = Graphics.FromImage(bmp);
            PictureBox1.Image = bmp;
            PictureBox1.Refresh();

            _undoStack.Clear();
            _redoStack.Clear();
            Undo_Button.Enabled = false;
            Redo_Button.Enabled = false;
            isNewButtonClicked = false;

            _undoStack.Push(new Bitmap(bmp));
        }
        #endregion


        /*******************    Recent Files Feature    ******************/
        #region Recent File Operations
        private void SaveRecentFilesToolStripMenuItem(object sender, EventArgs e)
        {
            if (_recentFiles.Contains(FileName)) //prevent duplication on recent list
            {
                _recentFiles.Remove(FileName);
            }
            if (_recentFiles.Count == 5)
            {
                _recentFiles.RemoveAt(0);
            }
            _recentFiles.Add(FileName);
            RecentFilesToolStripMenuItem.DropDownItems.Clear();

            for (int i = _recentFiles.Count - 1; i >= 0; i--)
            {
                RecentFilesToolStripMenuItem.DropDownItems.Add(_recentFiles[i]).Click += RecentFiles_MenuItem_Click;
            }
            RecentFilesToolStripMenuItem.Enabled = true;
        }

        private void RecentFiles_MenuItem_Click(object sender, EventArgs e)
        {
            ToolStripItem obj = sender as ToolStripItem;
            FileName = obj.Text;
            OpenFile(sender, e, true);
        }

        private void OpenFile(object sender, EventArgs e, bool isRecentFileClicked)
        {
            if (!isRecentFileClicked)
            {
                FileName = openFileDialog1.FileName;
            }
            using (imageFile = Image.FromFile(FileName))
            {
                _undoStack.Clear();
                _redoStack.Clear();
                bmp = new Bitmap(imageFile);
                g = Graphics.FromImage(bmp);
                PictureBox1.Image = bmp;
                PictureBox1.Refresh();
                _undoStack.Push(new Bitmap(bmp));

                Undo_Button.Enabled = false;
                Redo_Button.Enabled = false;
                SaveRecentFilesToolStripMenuItem(sender, e);
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            ExitToolStripMenuItem_Click(sender, e);
            Environment.Exit(0);
        }
        #endregion
    }
}
