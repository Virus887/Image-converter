using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GkProject3.Separators
{
    public abstract class Separator
    {
        protected DirectBitmap mainBitmap;
        protected int w, h;
        public DirectBitmap bitmap1;
        public DirectBitmap bitmap2;
        public DirectBitmap bitmap3;
        public Separator(DirectBitmap mainBitmap) 
        {
            this.mainBitmap = mainBitmap;
            w = mainBitmap.Width;
            h = mainBitmap.Height;
        }
    }
}
