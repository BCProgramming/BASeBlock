using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace BASeCamp.BASeBlock
{
    public class DPIHelper
    {
        private static Dictionary<Form, PointF> ScaleMapping = new Dictionary<Form, PointF>(); 
        private const int PointsPerInch = 72;
        /// <summary>
        /// Scales the given Font size (in pixels) to the appropriate point size to get that size (approx) in the given DPI.
        /// </summary>
        /// <param name="pixelsDPI96">desired font size in pixels.</param>
        /// <param name="dpi">DPI to scale the desired pixel size.</param>
        /// <returns></returns>
        public static float ScaleFontSize(float pixelSize, float dpi)
        {
            //pixels * 72 / 96
            return pixelSize * PointsPerInch / dpi;
        }
        public static float ScaleFontSize(float pixelSize,Graphics g)
        {
            return ScaleFontSize(pixelSize, g.DpiY);
        }

        public static PointF GetDPIScaling(System.Windows.Forms.Form FormUse)
        {
            float dpiX, dpiY;
            Graphics graphics = FormUse.CreateGraphics();
            dpiX = graphics.DpiX;
            dpiY = graphics.DpiY;
            PointF BuildScale = new PointF((float)dpiX/96,(float)dpiY/96);
            if(!ScaleMapping.ContainsKey(FormUse))
            {
                ScaleMapping.Add(FormUse,BuildScale);
            }
            return BuildScale;
        }
        

    }
}
