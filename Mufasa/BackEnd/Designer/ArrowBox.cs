using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Mufasa.BackEnd.Designer
{
    class ArrowBox
    {
        public int radius;
        public double start;
        public double stop;
        public double max;
        public SolidColorBrush color;

        public ArrowBox(int v1, int v2, int v3, int v4, SolidColorBrush red)
        {
            this.radius = v1;
            this.start = v2;
            this.stop = v3;
            this.max = v4;
            this.color = red;
        }
    }
}
