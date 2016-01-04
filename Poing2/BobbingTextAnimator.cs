using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;

namespace BASeCamp.BASeBlock
{
    public interface Animatable
    {
        void Draw(Graphics g, int x, int y);
        void PerformFrame();
        




    }
    class FallingTextAnimator : Animatable
    {
        //this is a tad more involved then bobbing text. Separates the letters as needed, making them fall and bounce to their final location.
        private class FallingCharacterAnimator
        {

            //stores the data for a single falling character.
            
            public Bitmap DrawBitmap; //created in constructor.
            public Graphics DrawCanvas; //again- created in constructor and drawn on there as well.
            public GraphicsPath drawletterpath;
            public PointF Position;
            public PointF Velocity;
            public RectangleF usebounds;
            public FallingCharacterAnimator(char Character, Font fontuse,PointF pPosition,PointF pVelocity,Brush UseBrush,Pen usePen,RectangleF pbounds)
            {
                Position=pPosition;
                Velocity = pVelocity;
                usebounds = pbounds;
                SizeF LetterSize = BCBlockGameState.MeasureString(Character.ToString(), fontuse);

                //create letter bitmap...
                DrawBitmap = new Bitmap((int)LetterSize.Width,(int)LetterSize.Height);
                //create the canvas, and draw the character on it.
                DrawCanvas = Graphics.FromImage(DrawBitmap);
                //draw; use a path.
                drawletterpath = new GraphicsPath();
                drawletterpath.AddString(Character.ToString(), fontuse.FontFamily, (int)fontuse.Style, 
                    fontuse.Size, new Point(0, 0), StringFormat.GenericDefault);
                //paint that graphics path onto the bitmap...
                DrawCanvas.FillPath(UseBrush, drawletterpath);
                DrawCanvas.DrawPath(usePen, drawletterpath);


            }
            public void PerformFrame()
            {
                if (!usebounds.Contains(new RectangleF(Position.X, Position.Y, DrawBitmap.Width, DrawBitmap.Height)))
                {
                    Velocity = new PointF(0, 0);
                }
                //Position = new PointF(Position.X+Velocity.X,Position.Y+Velocity.Y);


            }
            public void Draw(Graphics g)
            {
                //x and y unused atm...

                g.DrawImage(DrawBitmap, Position);


            }

        }
            private List<FallingCharacterAnimator> FallingCharacters = new List<FallingCharacterAnimator>();
            private PointF Gravity = new PointF(0,0.98f);
            private RectangleF useBounds;
            private Random rgen = new Random();
            public FallingTextAnimator(String DisplayString, Font fontuse, PointF Position, Brush useBrush, Pen usePen,PointF pGravity,RectangleF pBounds)
            {
                useBounds = pBounds;
                Gravity = pGravity;
                String BuildString = "";
                float currtextwidth = 0;
                PointF currentPosition = Position;
                foreach (Char loopchar in DisplayString)
                {
                    PointF useVelocity = new PointF(0, (float) rgen.NextDouble()*3);
                    FallingCharacterAnimator Addme = new FallingCharacterAnimator(loopchar, fontuse, currentPosition,
                                                                                  useVelocity, useBrush, usePen,
                                                                                  useBounds);
                    FallingCharacters.Add(Addme);
                    currentPosition = new PointF(Position.X+currtextwidth, Position.Y);
                    BuildString+=loopchar;
                    int currlength = BuildString.Length;
                    currtextwidth +=(BCBlockGameState.MeasureString(loopchar.ToString(),fontuse).Width/currlength);



                }

            }

        public void  PerformFrame()
                {
                    foreach(FallingCharacterAnimator loopanimator in FallingCharacters)
                    {
                        loopanimator.Velocity = new PointF(loopanimator.Velocity.X + Gravity.X, loopanimator.Velocity.Y + Gravity.Y);
                        loopanimator.PerformFrame();



                    }




                }
        public void Draw(Graphics g,int x,int y)
        {
            //x and y are unused...
            foreach (FallingCharacterAnimator loopcharacter in FallingCharacters)
            {

                loopcharacter.Draw(g);


            }



        }







    }




    

    /// <summary>
    /// A "simple" class that animates a piece of text to shrink and grow in a specified position.
    /// </summary>
    class BobbingTextAnimator:Animatable 
    {
        Bitmap textbitmap;
        Graphics textcanvas;
        Font mfont;
        String mtext;
        float mScale = 1.0f;
        float MinScale = 0.75f;
        float MaxScale = 1.75f;
        float scalestep = .05f;
        float useangle = 35;
        bool drawShadow=true;
        PointF ShadowOffset = new PointF(8, 8);
        ImageAttributes drawshadowattributes = null;
        ulong frame = 0;
        Brush currentBrush = new SolidBrush(Color.Red);
        public String DrawString
    {
        get
        {
            return mtext;
        }
    }
        public Font DrawFont
        {
            get { return mfont;}
        }
        public static float GetTextScaleFactor(String textuse,Font fontuse)
        {
            //Bitmap usebitmap = new Bitmap(1, 1);
            //Graphics guse = Graphics.FromImage(usebitmap);
            //SizeF Fullsize = guse.MeasureString(textuse, fontuse);
            SizeF Fullsize = BCBlockGameState.MeasureString(textuse, fontuse);
            return 1 / (Math.Max(Fullsize.Width, Fullsize.Height) / 384);






        }
        public BobbingTextAnimator(String textuse, double angle, Font usefont, float pminscale, float pmaxscale, float pscalestep, float pangle)
        {
            textbitmap = new Bitmap(1, 1);
            textcanvas = Graphics.FromImage(textbitmap);
            SizeF textsize = textcanvas.MeasureString(textuse, usefont);
            LinearGradientBrush pathbrush = new LinearGradientBrush(new PointF(0, 0), new PointF(textsize.Width, textsize.Height), Color.Yellow, Color.Green);
            Init(usefont, textuse, pminscale, pmaxscale, pscalestep, pangle, pathbrush);

        }

        public BobbingTextAnimator(String textuse, double angle, Font usefont,float pminscale,float pmaxscale,float pscalestep,float pangle,Brush usebrush)
        {
            //task: initialize our bitmap to be the appropriate size given the various options.
            //step one: create a bitmap so we can measure the string...
            Init(usefont, textuse, pminscale, pmaxscale, pscalestep, pangle,usebrush);
        }

        private void Init(Font usefont, string textuse, float pminscale, float pmaxscale, float pscalestep, float pangle,Brush usebrush)
        {
            mfont = usefont;
            mtext = textuse;
            float textscale = GetTextScaleFactor(textuse, usefont);
            textbitmap = new Bitmap(1, 1);
            textcanvas = Graphics.FromImage(textbitmap);
            textcanvas.SmoothingMode = SmoothingMode.HighQuality;
            textcanvas.CompositingQuality = CompositingQuality.HighQuality; 
            SizeF textsize = textcanvas.MeasureString(textuse, usefont);
            MinScale=pminscale*textscale;
            MaxScale=pmaxscale*textscale;
            scalestep=pscalestep;
            useangle=pangle;
            mScale = ((MaxScale-MinScale)/2)+MinScale;
            //now we know the height and width of the text, but we need to know
            //how much rectangular area it will consume after being rotated. but we just cheat and make both the height and width the larger of the two. (wtf, why not).
            //double usecoord = Math.Max(textsize.Width, textsize.Height);
            //SizeF newsize = new SizeF((float)usecoord, (float)usecoord);
            //recreate bitmap..
            textbitmap = new Bitmap((int)textsize.Width, (int)textsize.Height);
            textcanvas = Graphics.FromImage(textbitmap);
            //textcanvas.FillRectangle(new SolidBrush(Color.Red), 0, 0, textbitmap.Width, textbitmap.Height);
            GraphicsPath usepath = new GraphicsPath();



            //LinearGradientBrush pathbrush = new LinearGradientBrush(new PointF(0, 0), new PointF(textbitmap.Width, textbitmap.Height), Color.Yellow, Color.Green);
            Brush pathbrush = usebrush;
            Pen pathpen = new Pen(new SolidBrush(Color.Black), 3);
            //usepath.AddString(textuse, usefont, currentBrush, 0, 0);
            usepath.AddString(textuse, usefont.FontFamily, (int)usefont.Style, usefont.Size, new Point(textbitmap.Width/2, 0), new StringFormat { Alignment = StringAlignment.Center });
            textcanvas.FillPath(pathbrush, usepath);
            textcanvas.DrawPath(pathpen, usepath);


            drawshadowattributes = new ImageAttributes();
            ColorMatrix bwmatrix = new ColorMatrix(
      new float[][]
      {
         new float[] {0, 0, 0, 0, 0},
         new float[] {0, 0, 0, 0, 0},
         new float[] {0, 0, 0, 0, 0},
         new float[] {0, 0, 0, 1, 0},
         new float[] {0.25f, 0, 0, 0, 0.5f}
      });
            bwmatrix.Matrix33 = 0.8f;
            drawshadowattributes.SetColorMatrix(bwmatrix);

        }


        public void PerformFrame()
        {
            frame++;

            //Debug.Print("BobbingTextAnimator::PerformFrame()");
            //mScale+=scalestep;
            mScale = (float)Math.Sin(frame) * (MaxScale - MinScale) + (MinScale*2);
            //Debug.Print("Scale:" + mScale);
            //if ((mScale > MaxScale) || (mScale < MinScale))
            //    scalestep *= -1;
            //textcanvas.Clear(Color.Transparent);
            //textcanvas.DrawString(mtext, mfont, currentBrush, 0, 0);

            //new "sine" version.
            //(Sin(frame*speed)*extent)+minimumscale

            




        }
        
        public void Draw(Graphics g, int x, int y)
        {
            g.ResetTransform();
            g.TranslateTransform(x, y);
            g.RotateTransform(useangle);
           // Debug.Print("Drawing at scale" + mScale);
            g.ScaleTransform(mScale, mScale);
            g.TranslateTransform(-textbitmap.Width / 2, -textbitmap.Height / 2);
            //should we draw a shadow?
            if (drawShadow)
            {
                //if so, translate to the shadow offset.
                g.TranslateTransform(ShadowOffset.X, ShadowOffset.Y);
                //draw the shadow

          
                g.DrawImage(textbitmap, new Rectangle(0, 0, (int)textbitmap.Width, (int)textbitmap.Height), 0, 0, textbitmap.Width, textbitmap.Height, GraphicsUnit.Pixel, drawshadowattributes);


                //go back...
                g.ResetTransform();
                g.TranslateTransform(x, y);
                g.RotateTransform(useangle);
                // Debug.Print("Drawing at scale" + mScale);
                g.ScaleTransform(mScale, mScale);
                g.TranslateTransform(-textbitmap.Width / 2, -textbitmap.Height / 2);


            }

            g.DrawImage(textbitmap, 0, 0);
            
            //g.RotateTransform(30);

        }



    }
}