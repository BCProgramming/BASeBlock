using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;

namespace BASeCamp.BASeBlock
{

    public abstract class CharacterAnimator
    {
        protected PointF startpos;
        protected PointF endpos;
        protected PointF currentposition;
        protected TimeSpan totaltime;
        protected DateTime StartTime;
        public char usecharacter;
        protected bool isinited = false;
        public Font useFont = new Font(BCBlockGameState.GetMonospaceFont(), 24);
        public Brush fillbrush = new SolidBrush(Color.Red);
        public Pen strokepen = new Pen(Color.Black);
        public CharacterAnimator(PointF pStartpos, PointF pendpos, TimeSpan ptotaltime)
        {

            startpos = pStartpos;
            endpos = pendpos;
            totaltime = ptotaltime;

        }
        public abstract void PerformFrame();
       

        public void DoPerformFrame()
        {
            if(!isinited)
            {
                isinited = true;
                StartTime = DateTime.Now;
                currentposition = startpos;
            }
            else
            {

                PerformFrame();
              


            }

        }

        public virtual void Draw(Graphics g)
        {
            GraphicsPath usepath = new GraphicsPath();
            usepath.AddString(usecharacter.ToString(), useFont.FontFamily, (int)useFont.Style, useFont.Size, currentposition,StringFormat.GenericDefault);
            g.FillPath(fillbrush, usepath);
            g.DrawPath(strokepen, usepath);


        }




    }
    public class LinearCharacterAnimator : CharacterAnimator
    {

        public LinearCharacterAnimator(Char pusecharacter,PointF pStartpos, PointF pendpos, TimeSpan ptotaltime)
            : base(pStartpos, pendpos, ptotaltime)
        {
            usecharacter = pusecharacter;
        }

        public override void PerformFrame()
        {
            TimeSpan TimeDiff = DateTime.Now - StartTime;
            //find out what "percent" we are to "finishing"...
            float percentage = ((float)TimeDiff.TotalMilliseconds) / (float)(totaltime.TotalMilliseconds);
            if (percentage >= 1) percentage = 1;
            //get the "difference" between start and end positions...
            PointF diffposition = new PointF(endpos.X - startpos.X, endpos.Y - startpos.Y);
            //now, we use the percentage we have to create a new point between the two positions...

            PointF newpos = new PointF(startpos.X + (diffposition.X * percentage), startpos.Y + (diffposition.Y * percentage));
            currentposition = newpos;
        }


    }
    public class TextAnimationManager
    {
        public delegate CharacterAnimator GetCharacterAnimator(PointF initialpos,String forstring, int charindex);

        public List<CharacterAnimator> Characters = new List<CharacterAnimator>();

        
        public TextAnimationManager(String forString, GetCharacterAnimator getchardelegate,PointF startpos)
        {
            for (int i = 0; i < forString.Length; i++)
            {
                //use the delegate to get an instance...
                CharacterAnimator gotanimator = getchardelegate(startpos,forString, i);
                Characters.Add(gotanimator);

            }

        }
        public void PerformFrame()
        {
            foreach (CharacterAnimator loopanimator in Characters)
            {
                Debug.Print("performing frame... letter=" + loopanimator.usecharacter);
                loopanimator.PerformFrame();

            }


        }
        public void Draw(Graphics g)
        {

            foreach (CharacterAnimator loopanimator in Characters)
            {
                loopanimator.Draw(g);

            }


        }

    }
}
