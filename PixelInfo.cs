using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Converter_Ver3
{
    public class PixelInfo
    {
        public Color pixelColor;
        public BorderInfo redBorder;
        public BorderInfo greenBorder;
        public BorderInfo blueBorder;

        public PixelInfo (Color pixelColor, BorderInfo redBorder, BorderInfo greenBorder, BorderInfo blueBorder)
        {
            this.pixelColor = pixelColor;
            this.redBorder = redBorder;
            this.greenBorder = greenBorder;
            this.blueBorder = blueBorder;
        }
    }
}
