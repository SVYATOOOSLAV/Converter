using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Converter_Ver3
{
    public class BorderInfo
    {
        public int leftBorder { get; set; }
        public int rightBorder { get; set; }

        public BorderInfo(int leftBorder, int rightBorder)
        {
            this.leftBorder = leftBorder;
            this.rightBorder = rightBorder;
        }
    }
}
