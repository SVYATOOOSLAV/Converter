using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Converter_Ver3
{
    public  class Png : Converter
    {
        public static byte[] AsPng(byte[] data)
        {
            // MemoryStream - запоминающий поток, для быстрой работы с вводом/выводом.
            using (MemoryStream inStream = new MemoryStream(data))
            using (MemoryStream outStream = new MemoryStream())
            {
                Image imageStream = Image.FromStream(inStream);
                imageStream.Save(outStream, ImageFormat.Png);
                return outStream.ToArray();
            }
        }
    }
}
