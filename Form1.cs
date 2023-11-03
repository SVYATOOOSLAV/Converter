using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Encoder = System.Drawing.Imaging.Encoder;

namespace Converter_Ver3
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            label3.Visible = false;
            trackBar1.Visible = false;
            textBox2.Visible = false;
            comboBox3.Visible = false;
            label5.Visible = false;
            label6.Visible = false;
        }

        string path = "";
        byte[] resultBytes = null;
        int trackBarValue = -1;
        string formatOutputFile = null;

        private void button1_Click(object sender, EventArgs e)
        {
            if(textBox1.Text != "" && comboBox1.Text != "" && comboBox2.Text != "" && (comboBox3.Text != "" || trackBarValue != -1))
            {
                byte[] data = File.ReadAllBytes(path);
                formatOutputFile = comboBox1.Text;
                string imageResolution = comboBox2.Text;

                switch (formatOutputFile)
                {
                    case "Jpeg":
                        resultBytes = Jpeg.AsJpeg(data);
                        switch (imageResolution)
                        {
                            case "Default":
                                break;
                            case "HD (1280 x 720)":
                                resultBytes = Converter.Resize(resultBytes, 1280);
                                break;
                            case "Full HD (1920 x 1080)":
                                resultBytes = Converter.Resize(resultBytes, 1920);
                                break;
                            case "Quad HD (2560 x 1440)":
                                resultBytes = Converter.Resize(resultBytes, 2560);
                                break;
                            case "4K (3840 x 2160)":
                                resultBytes = Converter.Resize(resultBytes, 3840);
                                break;
                        }
                        break;
                    case "Png":
                        resultBytes = Png.AsPng(data);
                        switch (imageResolution)
                        {
                            case "Default":
                                break;
                            case "HD (1280 x 720)":
                                resultBytes = Converter.Resize(resultBytes, 1280);
                                break;
                            case "Full HD (1920 x 1080)":
                                resultBytes = Converter.Resize(resultBytes, 1920);
                                break;
                            case "Quad HD (2560 x 1440)":
                                resultBytes = Converter.Resize(resultBytes, 2560);
                                break;
                            case "4K (3840 x 2160)":
                                resultBytes = Converter.Resize(resultBytes, 3840);
                                break;
                        }
                        break;
                    case "Gif":
                        resultBytes = Gif.AsGif(data);
                        switch (imageResolution)
                        {
                            case "Default":
                                break;
                            case "HD (1280 x 720)":
                                resultBytes = Converter.Resize(resultBytes, 1280);
                                break;
                            case "Full HD (1920 x 1080)":
                                resultBytes = Converter.Resize(resultBytes, 1920);
                                break;
                            case "Quad HD (2560 x 1440)":
                                resultBytes = Converter.Resize(resultBytes, 2560);
                                break;
                            case "4K (3840 x 2160)":
                                resultBytes = Converter.Resize(resultBytes, 3840);
                                break;
                        }
                        break;
                }

                if (formatOutputFile == "Jpeg")
                {
                    resultBytes = Jpeg.JPGCompress(resultBytes, trackBarValue);
                }

                if (formatOutputFile == "Png" && comboBox3.SelectedIndex == 0)
                {
                    resultBytes = Png.transparency(resultBytes);
                }

                if (formatOutputFile == "Gif" && comboBox3.SelectedIndex == 0)
                {
                    resultBytes = Gif.LZWCompress(resultBytes);
                }

                label5.Visible = true;
                label6.Visible = true;

                showPicture(pictureBox1, data);
                showPicture(pictureBox2, resultBytes);
            }
            else
            {
                MessageBox.Show("Не все поля заполнены", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void showPicture(PictureBox pictureBox, byte[] data)
        {
            using (MemoryStream inStream = new MemoryStream(data))
            {
                Image image = Image.FromStream(inStream);

                pictureBox.Image = image;
            }
        }

        private void Form1_DragDrop(object sender, DragEventArgs e)
        {
            string[] temp = (string[])e.Data.GetData(DataFormats.FileDrop);
            path = temp[0];
            textBox1.Text = path;
        }

        private void Form1_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effect = DragDropEffects.Copy;
            }
        }

        private void Form1_DragLeave(object sender, EventArgs e)
        {

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox1.Text == "Jpeg")
            {
                comboBox3.Visible = false;
                label3.Visible = true;
                trackBar1.Visible = true;
                textBox2.Visible = true;

                label3.Text = "Сжатие";
                trackBar1.Minimum = 0;
                trackBar1.Maximum = 75;
                textBox2.Text = trackBar1.Value.ToString();
                trackBarValue = 0;
            }
            else if (comboBox1.Text == "Png")
            {
                label3.Visible = true;
                comboBox3.Visible = true;
                trackBar1.Visible = false;
                textBox2.Visible = false;

                label3.Text = "Прозрачность";
            }
            else if (comboBox1.Text == "Gif")
            {
                label3.Visible = true;
                comboBox3.Visible = true;
                trackBar1.Visible = false;
                textBox2.Visible = false;

                label3.Text = "Cжатие";
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if(resultBytes != null)
            {
                SaveFileDialog save = new SaveFileDialog();
                switch (formatOutputFile)
                {
                    case "Png":
                        save.Filter = "Изображение (*.png))|*.png";
                        break;
                    case "Jpeg":
                        save.Filter = "Изображение (*.jpg))|*.jpg";
                        break;
                    case "Gif":
                        save.Filter = "Изображение (*.gif))|*.gif";
                        break;
                }

                save.InitialDirectory = @"D:\";

                if (save.ShowDialog() == DialogResult.OK)
                {
                    File.WriteAllBytes(save.FileName, resultBytes);
                }
            }
            else
            {
                MessageBox.Show("Нет картинки", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            textBox2.Text = trackBar1.Value.ToString();
            trackBarValue = trackBar1.Value;
        }
    }
}


