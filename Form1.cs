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

namespace Converter
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
            comboBox4.Visible = false;
            label5.Visible = false;
            label6.Visible = false;
            panel2.Visible = false;
        }

        string path = "";
        byte[] data = null;
        byte[] resultBytes = null;
        string formatOutputFile = null;

        static StringBuilder sbForRed = new StringBuilder();
        StringBuilder sbForGreen = new StringBuilder();
        StringBuilder sbForBlue = new StringBuilder();


        private void createResolution(string imageResolution)
        {

            switch (imageResolution)
            {
                case "Default":
                    break;
                case "HD (1280 x 720)":
                    resultBytes = Converter.Resize(resultBytes, 1280, 720);
                    break;
                case "Full HD (1920 x 1080)":
                    resultBytes = Converter.Resize(resultBytes, 1920, 1080);
                    break;
                case "Quad HD (2560 x 1440)":
                    resultBytes = Converter.Resize(resultBytes, 2560, 1440);
                    break;
                case "4K (3840 x 2160)":
                    resultBytes = Converter.Resize(resultBytes, 3840, 2160);
                    break;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            pictureBox2.BackColor = Color.Transparent;
            if (button3.Text == "Изменить путь" && textBox1.Text != "" && comboBox1.Text != "" && comboBox2.Text != "")
            {
                formatOutputFile = comboBox1.Text;
                string imageResolution = comboBox2.Text;

                switch (formatOutputFile)
                {
                    case "Jpeg":
                        resultBytes = Jpeg.AsJpeg(data);
                        break;
                    case "Png":
                        resultBytes = Png.AsPng(data);
                        break;
                    case "Gif":
                        resultBytes = Gif.AsGif(data);
                        break;
                }

                createResolution(imageResolution);

                if (formatOutputFile == "Jpeg")
                {
                    resultBytes = Jpeg.JPGCompress(resultBytes, 100 - trackBar1.Value);
                }

                if (formatOutputFile == "Png" && comboBox3.Text != "" && comboBox3.SelectedIndex == 0)
                {
                    Color color = getColorAndChangeBackColor();
                    try
                    {
                        PixelInfo pixelInfo = new PixelInfo(color,
                        new BorderInfo(int.Parse(textBox3.Text), int.Parse(textBox4.Text)),
                        new BorderInfo(int.Parse(textBox5.Text), int.Parse(textBox6.Text)),
                        new BorderInfo(int.Parse(textBox7.Text), int.Parse(textBox8.Text)));

                        resultBytes = Png.transparency(resultBytes, pixelInfo);
                        pictureBox2.BackColor = Color.White;
                    }
                    catch (FormatException)
                    {
                        MessageBox.Show(("Некорректные данные границ пикселя"), "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                }

                if (formatOutputFile == "Gif" && comboBox4.Text != "")
                {
                    resultBytes = Gif.colorDepth(resultBytes, int.Parse(comboBox4.Text));
                }


                label6.Visible = true;
                showPicture(pictureBox2, resultBytes);

                using (MemoryStream ms = new MemoryStream(resultBytes))
                {
                    Image image = Image.FromStream(ms);
                    Bitmap bm = new Bitmap(image);

                    label6.Text = $"Стало ({comboBox1.Text.ToLower()}, {image.Width}x{image.Height})";
                }
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
                pictureBox.Height = pictureBox.Width * image.Height / image.Width;
                pictureBox.Image = image;
            }
        }

        private void Form1_DragDrop(object sender, DragEventArgs e)
        {
            if (button3.Text != "Изменить путь")
            {
                string[] temp = (string[])e.Data.GetData(DataFormats.FileDrop);
                path = temp[0];
                textBox1.Text = path;
                updateInfoAboutImage();
            }
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
            label3.Visible = true;
            if (comboBox1.Text == "Jpeg")
            {
                comboBox4.Visible = false;
                panel2.Visible = false;
                comboBox3.Visible = false;
                trackBar1.Visible = true;
                textBox2.Visible = true;
                comboBox3.SelectedIndex = -1;
                comboBox4.SelectedIndex = -1;

                label3.Text = "Сжатие";

                trackBar1.Minimum = 0;
                trackBar1.Maximum = 100;
                trackBar1.Value = 0;
                textBox2.Text = trackBar1.Value.ToString();
            }
            else if (comboBox1.Text == "Png")
            {
                comboBox4.Visible = false;
                comboBox3.Visible = true;
                trackBar1.Visible = false;
                textBox2.Visible = false;
                comboBox4.SelectedIndex = -1;

                label3.Text = "Прозрачность";
            }
            else if (comboBox1.Text == "Gif")
            {
                comboBox3.SelectedIndex = -1;
                comboBox4.Visible = true;
                panel2.Visible = false;
                comboBox3.Visible = false;
                trackBar1.Visible = false;
                textBox2.Visible = false;

                label3.Text = "Глубина цвета";
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (resultBytes != null)
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

                save.InitialDirectory = @"C:\";

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

        private void trackBar1_Scroll(object sender, EventArgs e) => textBox2.Text = trackBar1.Value.ToString();


        private void button3_Click(object sender, EventArgs e)
        {
            if (button3.Text == "Сохранить путь")
            {
                updateInfoAboutImage();
            }
            else
            {
                pictureBox1.Image = null;
                pictureBox2.Image = null;
                label5.Text = "";
                label6.Text = "";
                panel1.BackColor = Color.Transparent;
                pictureBox2.BackColor = Color.Transparent;
                textBox1.ReadOnly = false;
                button3.Text = "Сохранить путь";
            }
        }

        private void updateInfoAboutImage()
        {
            if (File.Exists(textBox1.Text))
            {
                path = textBox1.Text;
                label5.Visible = true;
                data = File.ReadAllBytes(path);
                showPicture(pictureBox1, data);
                Bitmap image = new Bitmap(path);
                label5.Text = $"Было ({textBox1.Text.Substring(textBox1.Text.LastIndexOf('.') + 1)}, {image.Width}x{image.Height})";
                panel1.BackColor = image.GetPixel(0, 0);
                textBox1.ReadOnly = true;
                button3.Text = "Изменить путь";
            }
            else
            {
                MessageBox.Show("Изображения на этом пути не существует", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox3.SelectedIndex == 0)
            {
                panel2.Visible = true;
                getColorAndChangeBackColor();
            }
            else
            {
                panel2.Visible = false;
                numericUpDown1.Value = 0;
                numericUpDown2.Value = 0;
                numericUpDown3.Value = 0;
            }
        }

        private Color getColorAndChangeBackColor()
        {
            int pixelColorRed = (int)numericUpDown1.Value;
            int pixelColorGreen = (int)numericUpDown2.Value;
            int pixelColorBlue = (int)numericUpDown3.Value;

            Color color = Color.FromArgb(pixelColorRed, pixelColorGreen, pixelColorBlue);
            pictureBox3.BackColor = color;
            return color;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            getColorAndChangeBackColor();
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (textBox3.Text != "")
                {
                    String[] strings = textBox9.Text.Split('-');
                    int res = (int)numericUpDown1.Value - int.Parse(textBox3.Text);
                    strings[0] = (res < 0) ? "0" : res.ToString();
                    textBox9.Text = string.Join("-", strings);
                }
            }
            catch (FormatException)
            {
                MessageBox.Show("Неверное значение, возврат к значению по умолчанию", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                textBox3.Text = "0";
            }

        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (textBox4.Text != "")
                {
                    String[] strings = textBox9.Text.Split('-');
                    int res = (int)numericUpDown1.Value + int.Parse(textBox4.Text);
                    strings[1] = (res > 255) ? "255" : res.ToString();
                    textBox9.Text = string.Join("-", strings);
                }
            }
            catch (FormatException)
            {
                MessageBox.Show("Неверное значение, возврат к значению по умолчанию", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                textBox4.Text = "0";
            }

        }

        private void textBox5_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (textBox5.Text != "")
                {
                    String[] strings = textBox10.Text.Split('-');
                    int res = (int)numericUpDown2.Value - int.Parse(textBox5.Text);
                    strings[0] = (res < 0) ? "0" : res.ToString();
                    textBox10.Text = string.Join("-", strings);
                }
            }
            catch (FormatException)
            {
                MessageBox.Show("Неверное значение, возврат к значению по умолчанию", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                textBox5.Text = "0";
            }


        }

        private void textBox6_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (textBox6.Text != "")
                {
                    String[] strings = textBox10.Text.Split('-');
                    int res = (int)numericUpDown2.Value + int.Parse(textBox6.Text);
                    strings[1] = (res > 255) ? "255" : res.ToString();
                    textBox10.Text = string.Join("-", strings);
                }
            }
            catch (FormatException)
            {
                MessageBox.Show("Неверное значение, возврат к значению по умолчанию", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                textBox6.Text = "0";
            }


        }

        private void textBox7_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (textBox7.Text != "")
                {
                    String[] strings = textBox11.Text.Split('-');
                    int res = (int)numericUpDown3.Value - int.Parse(textBox7.Text);
                    strings[0] = (res < 0) ? "0" : res.ToString();
                    textBox11.Text = string.Join("-", strings);
                }
            }
            catch (FormatException)
            {
                MessageBox.Show("Неверное значение, возврат к значению по умолчанию", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                textBox7.Text = "0";
            }

        }

        private void textBox8_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (textBox8.Text != "")
                {
                    String[] strings = textBox11.Text.Split('-');
                    int res = (int)numericUpDown3.Value + int.Parse(textBox8.Text);
                    strings[1] = (res > 255) ? "255" : res.ToString();
                    textBox11.Text = string.Join("-", strings);
                }
            }
            catch (FormatException)
            {
                MessageBox.Show("Неверное значение, возврат к значению по умолчанию", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                textBox8.Text = "0";
            }

        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            textBox9.Text = $"{numericUpDown1.Value}-{numericUpDown1.Value}";
            textBox3_TextChanged(sender, e);
            textBox4_TextChanged(sender, e);
        }

        private void numericUpDown2_ValueChanged(object sender, EventArgs e)
        {
            textBox10.Text = $"{numericUpDown2.Value}-{numericUpDown2.Value}";
            textBox5_TextChanged(sender, e);
            textBox6_TextChanged(sender, e);
        }

        private void numericUpDown3_ValueChanged(object sender, EventArgs e)
        {
            textBox11.Text = $"{numericUpDown3.Value}-{numericUpDown3.Value}";
            textBox7_TextChanged(sender, e);
            textBox8_TextChanged(sender, e);
        }
    }
}


