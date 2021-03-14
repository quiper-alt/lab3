using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Project1
{
    public partial class Form1 : Form
    {
        private List<FileInfo> _imageList;
        private int _imageIndex;
        float b;
        bool gray;
        Image original;
        bool noise;
        public Form1()
        {
            InitializeComponent();
            _imageList = new List<FileInfo>();
            gray = false;
            noise = false;
        }


    private void Button2_Click(object sender, EventArgs e)
        {
            var fb = new FolderBrowserDialog();
            if (fb.ShowDialog() == DialogResult.OK)
            {
                var folder = new DirectoryInfo(fb.SelectedPath);
                _imageList = folder.GetFiles()
                        .Where(c => c.Extension == ".jpg"
                        || c.Extension == ".png")
                        .ToList();

                //_imageList.Clear();
                //_imageList.AddRange(images);

                listBox1.Items.Clear();
                foreach (var item in _imageList)
                {
                    listBox1.Items.Add(item.Name);
                }
                DisplayImage();
            }
        }
        private void DisplayImage()
        {
            var image = _imageList.ElementAtOrDefault(_imageIndex);
            if (image != null)
            {
                pictureBox1.Image?.Dispose();
                pictureBox1.Image = Image.FromFile(image.FullName);
                original = Image.FromFile(image.FullName);
            }
        }

        private void ButtonImageChange(object sender, EventArgs e)
        {
            var button = sender as Button;
            switch (button.Name)
            {
                case "button1":
                    _imageIndex++;
                    break;
                case "LeftButton":
                    _imageIndex--;
                    break;
            }
        }
        private void ListBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            var listbox = sender as ListBox;
            _imageIndex = listbox.SelectedIndex;
            DisplayImage();
        }


        private void Discolor_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox checkBox = (CheckBox)sender; 
            if (checkBox.Checked == true)
            {
                gray = true;
                ChangePictures();
            }
            else
            {
                gray = false;
                ChangePictures();
            }
        }

        private void Noise_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox checkBox = (CheckBox)sender; 
            if (checkBox.Checked == true)
            {
                noise = true;
                ChangePictures();
            }
            else
            {
                noise = false;
                ChangePictures();
            }
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            b = (float)trackBar1.Value / 100;
            ChangePictures();

        }

        private void ChangePictures()
        {


            float[][] matrix = {
                    new float[] { 1, 0, 0, 0, 0 } ,
                    new float[] { 0, 1, 0, 0, 0 } ,
                    new float[] { 0, 0, 1, 0, 0 } ,
                    new float[] { 0, 0, 0, 1, 0 } ,
                    new float[] { b, b, b, 0, 1 }
                 };
            float[][] matrix2 = {
                    new float[] { 0.299f, 0.299f, 0.299f, 0, 0 },
                    new float[] { 0.587f, 0.587f, 0.587f, 0, 0 },
                    new float[] { 0.114f, 0.114f, 0.114f, 0, 0 },
                    new float[] { 0, 0, 0, 1, 0 },
                    new float[] { b, b, b, 0, 1 },
              };
            var image = new Bitmap(original.Width, original.Height);

            ImageAttributes attr = new ImageAttributes();
            if (gray)
            {
                attr.SetColorMatrix(new ColorMatrix(matrix2));
            }
            else
                attr.SetColorMatrix(new ColorMatrix(matrix));
            if (pictureBox1.Image != null)
            {
                using (Graphics g = Graphics.FromImage(image))
                {
                    Rectangle rect = new Rectangle(0, 0, image.Width, image.Height);
                    g.DrawImage(original, rect, 0, 0, original.Width, original.Height, GraphicsUnit.Pixel, attr);
                }
                if (noise)
                {
                    pictureBox1.Image = CreateNoise(image);
                    return;
                }
                pictureBox1.Image = image;
            }
        }

        
        private Bitmap CreateNoise(Bitmap imageNoise)
        {
            Random rnd = new Random();


            for (int i = 0; i < imageNoise.Width; i++)
            {
                for (int j = 0; j < imageNoise.Height; j++)
                {
                    Color color = imageNoise.GetPixel(i, j);
                    byte r = (byte)(rnd.Next(0, 2) == 1 ? color.R : 255);
                    byte b = (byte)(rnd.Next(0, 2) == 1 ? color.B : 255);
                    byte g = (byte)(rnd.Next(0, 2) == 1 ? color.G : 255);

                    imageNoise.SetPixel(i, j, Color.FromArgb(255, r, g, b));
                }
            }
            return imageNoise;
        }
    }
}
