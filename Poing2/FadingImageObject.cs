using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;

namespace BASeBlock
{
    class FadingImageObject : GameObject 
    {

        private Image useDrawImage = null;
        private TimeSpan TTL = new TimeSpan(0, 0, 0, 1);
        private DateTime? FirstFrameTime;
        private RectangleF _DrawLocation;
        private Func<int, int, float> AlphaFunction = (per, total) => ((float)per) / (float)total;

        private static readonly Func<int, int, float> defaultAlphaFunction = (per, total) => ((float)per) / (float)total;




        //base constructor...
        public FadingImageObject(iImagable useImage, TimeSpan pTTL, Func<int, int, float> pAlphaFunction)
            :this(useImage.getImage(),useImage.getRectangle(), pTTL,pAlphaFunction)
        {



        }
        

        public FadingImageObject(Image useImage,RectangleF pDrawLocation, TimeSpan pTTL, Func<int, int, float> pAlphaFunction)
        {
            useDrawImage = useImage;
            _DrawLocation = pDrawLocation;
            TTL = pTTL;
            AlphaFunction = pAlphaFunction ?? defaultAlphaFunction;
                
            
        }
        ImageAttributes useattributes = new ImageAttributes();

        public override bool PerformFrame(BCBlockGameState gamestate)
        {
            if (FirstFrameTime == null) FirstFrameTime = DateTime.Now;
            
            double mslived = (DateTime.Now - FirstFrameTime).Value.TotalMilliseconds;

            double usealpha = AlphaFunction((int)mslived, (int)TTL.TotalMilliseconds);

            useattributes = new ImageAttributes();
            useattributes.SetColorMatrix(ColorMatrices.GetFader((int)(usealpha * 255)));



            return mslived > TTL.TotalMilliseconds;




        }



        public override void Draw(Graphics g)
        {

            g.DrawImage(useDrawImage,_DrawLocation.ToRectangle()
                ,0f,0f,(float)useDrawImage.Width,(float)useDrawImage.Height,
                GraphicsUnit.Pixel,useattributes);
            

            //throw new NotImplementedException();
        }
    }
}
