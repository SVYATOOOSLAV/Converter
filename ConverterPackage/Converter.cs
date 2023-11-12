using System;
using System.Collections.Generic;
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
    public class Converter
    {

        public static ImageFormat getImageFormat(byte[] data)
        {
            using (MemoryStream inStream = new MemoryStream(data))
            {
                Image image = Image.FromStream(inStream);
                return image.RawFormat;
            }
        }

        public static byte[] Resize(byte[] data, int width, int height)
        {
            ImageFormat format = getImageFormat(data);

            using (MemoryStream stream = new MemoryStream(data))
            {
                Image image = Image.FromStream(stream);

                Image thumbnail = image.GetThumbnailImage(width, height, null, IntPtr.Zero);

                using (MemoryStream thumbnailStream = new MemoryStream())
                {
                    thumbnail.Save(thumbnailStream, format);
                    return thumbnailStream.ToArray();
                }
            }
        }

        //ImageFormat - формат файла
        //ImageCodecInfo - содержит члены, которые выдают инфу об установленных кодеках

        private static ImageCodecInfo GetEncoder(ImageFormat format)
        {
            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageDecoders();
            foreach (ImageCodecInfo codec in codecs)
            {
                if (codec.FormatID == format.Guid)
                {
                    return codec;
                }
            }
            return null;
        }

        public static byte[] JPGCompress(byte[] data, long value)
        {
            ImageFormat format = getImageFormat(data);
            ImageCodecInfo pictureEncoder = GetEncoder(format);

            using (MemoryStream inStream = new MemoryStream(data))
            using (MemoryStream outStream = new MemoryStream())
            {
                Image image = Image.FromStream(inStream);

                var qualityEncoder = Encoder.Quality;

                EncoderParameters encoderParameters = new EncoderParameters(1);
                encoderParameters.Param[0] = new EncoderParameter(qualityEncoder, value);
                image.Save(outStream, pictureEncoder, encoderParameters);

                return outStream.ToArray();
            }
        }

        public static byte[] colorDepth(byte[] data, int newColorDepth)
        {
            Image image;
            using(MemoryStream inStream = new MemoryStream(data))
            {
                image = Image.FromStream(inStream);
            }
            Bitmap bitmap = new Bitmap(image);

            Bitmap newBitmap = null;
            switch (newColorDepth)
            {
                case 1:
                    newBitmap = bitmap.Clone(new Rectangle(0, 0, bitmap.Width, bitmap.Height), PixelFormat.Format1bppIndexed);
                    break;
                case 4:
                    newBitmap = bitmap.Clone(new Rectangle(0, 0, bitmap.Width, bitmap.Height), PixelFormat.Format4bppIndexed);
                    break;
                case 8:
                    newBitmap = bitmap.Clone(new Rectangle(0, 0, bitmap.Width, bitmap.Height), PixelFormat.Format8bppIndexed);
                    break;
                case 24:
                    newBitmap = bitmap.Clone(new Rectangle(0, 0, bitmap.Width, bitmap.Height), PixelFormat.Format24bppRgb);
                    break;
            }
              
            byte[] imageBytes;
            using (MemoryStream ms = new MemoryStream())
            {
                newBitmap.Save(ms, ImageFormat.Gif);
                imageBytes = ms.ToArray();
            }

            return imageBytes;
        }
    
    

        public static byte[] transparency(byte[] data, PixelInfo pixelInfo)
        {
            Image pict = null;
            using (MemoryStream inStream = new MemoryStream(data))
            {
                pict = Image.FromStream(inStream);
            }

            // Загрузка изображения в формате PNG
            using (Bitmap image = new Bitmap(pict))
            {
                // Создание нового изображения с альфа-каналом (прозрачностью)
                using (Bitmap newImage = new Bitmap(image.Width, image.Height, PixelFormat.Format32bppArgb))
                {
                    // Получение объекта Graphics, чтобы рисовать на новом изображении
                    using (Graphics g = Graphics.FromImage(newImage))
                    {
                        // Установка режима с альфа-каналом
                        g.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceOver;

                        // Копирование исходного изображения в новое с сохранением альфа-канала
                        g.DrawImage(image, 0, 0);

                        // Изменение значения альфа-канала для каждого пикселя
                        for (int y = 0; y < newImage.Height; y++)
                        {
                            for (int x = 0; x < newImage.Width; x++)
                            {
                                Color pixelColor = newImage.GetPixel(x, y);

                                // Например, установка полной прозрачности (A=0) для пикселей с определенным условием
                                if ((pixelColor.R >= pixelInfo.pixelColor.R - pixelInfo.redBorder.leftBorder) && (pixelColor.R <= pixelInfo.pixelColor.R + pixelInfo.redBorder.rightBorder) &&
                                    (pixelColor.G >= pixelInfo.pixelColor.G - pixelInfo.greenBorder.leftBorder) && (pixelColor.G <= pixelInfo.pixelColor.G + pixelInfo.greenBorder.rightBorder) &&
                                    (pixelColor.B >= pixelInfo.pixelColor.B - pixelInfo.blueBorder.leftBorder) && (pixelColor.B <= pixelInfo.pixelColor.B + pixelInfo.blueBorder.rightBorder)) // Ваше условие для изменения прозрачности
                                {
                                    pixelColor = Color.FromArgb(0, pixelColor);
                                }

                                // Установка нового значения альфа-канала для пикселя
                                newImage.SetPixel(x, y, pixelColor);
                            }
                        }
                    }

                    byte[] imageBytes;
                    using (MemoryStream ms = new MemoryStream())
                    {
                        newImage.Save(ms, ImageFormat.Png);
                        imageBytes = ms.ToArray();
                    }

                    return imageBytes;
                }
            }
        }
    }
}