
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing.Imaging;
using AForge;
using AForge.Imaging.Filters;
using AForge.Imaging;
using System.IO;
using Tesseract;

namespace pfe3
{
    
    public partial class Form1 : Form
    {
        string fname1;
        Bitmap img1;
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            pictureBox1.Visible = false;

        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            label1.Text = "";
            openFileDialog1.FileName = "";
            openFileDialog1.Title = "Images";
            openFileDialog1.Filter = "All Images|*.jpg; *.bmp; *.png";
            openFileDialog1.ShowDialog();
            if (openFileDialog1.FileName.ToString() != "")
            {
                fname1 = openFileDialog1.FileName.ToString();
                img1 = new Bitmap(fname1);
                pictureBox1.Visible = true;
                pictureBox1.Image = img1;
            }
        }
        private void button1_Click(object sender, EventArgs e)
        {

            double width = Math.Floor(img1.Width / img1.HorizontalResolution);
            double height = Math.Floor(img1.Height / img1.VerticalResolution);
            label1.Text = img1.HorizontalResolution + " " + img1.VerticalResolution;
            img1 = Grayscale.CommonAlgorithms.BT709.Apply(img1);
            Threshold filter = new Threshold(150);
            filter.ApplyInPlace(img1);
            DocumentSkewChecker skewChecker = new DocumentSkewChecker();
            double angle = skewChecker.GetSkewAngle(img1);
            RotateBilinear rotationFilter = new RotateBilinear(-angle);
            rotationFilter.FillColor = Color.White;
            img1 = rotationFilter.Apply(img1);
            pictureBox1.Image = img1;
            TesseractEngine engine = new TesseractEngine(@"C:\Users\moham\source\repos\pfe3\tessdata\", "fra",EngineMode.Default);
            if (((width == 3) && (height == 2)) || (width == 2 && height == 3))
            {
                if((width == 2 && height == 3))
                {
                    rotationFilter = new RotateBilinear(-90);
                    rotationFilter.FillColor = Color.White;
                    img1 = rotationFilter.Apply(img1);
                    Console.WriteLine("if1");
                    
                }
                string ocrText = engine.Process(img1, new Rect(700, 450, 150, 100)).GetText();
                engine.Dispose();
                ocrText = ocrText.Replace(" "," ");
                Console.WriteLine(ocrText+" "+ocrText.Length);
                if (ocrText.Length <= 9)
                label1.Text = "carte national " + ocrText;
                else
                {
                    rotationFilter = new RotateBilinear(180);
                    rotationFilter.FillColor = Color.White;
                    img1 = rotationFilter.Apply(img1);
                    engine = new TesseractEngine(@"C:\Users\moham\source\repos\pfe3\tessdata\","fra", EngineMode.Default);
                    ocrText = engine.Process(img1, new Rect(700, 450, 150, 100)).GetText();
                    engine.Dispose();
                    ocrText = ocrText.Replace(" ", "");
                }
                label1.Text = "carte national " + ocrText;
                pictureBox1.Image = img1;
                engine.Dispose();
            }
            else if ((width == 8 && height == 11) || (width == 11 && height == 8))
            {

            }
            else
            {

            }


        }
    }
}
