using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using BASeBlock.PaddleBehaviours;

namespace BASeBlock.Powerups
{
    public class BallSplitterPowerup : GamePowerUp
    {
        private int SplitCount = 3;
        protected static Dictionary<int, Image> SplitterPics = new Dictionary<int, Image>();
        public static float PowerupChance()
        {

            return 2f;
        }
        private bool BallSplit(BCBlockGameState gs)
        {
            List<cBall> removethese = new List<cBall>();
            List<cBall> addballs = new List<cBall>();
            if (gs.PlayerPaddle != null)
            {
                //check for Sticky...
                if (gs.PlayerPaddle.Behaviours.Any((w) => w is StickyBehaviour))
                {
                    foreach (var unstick in gs.PlayerPaddle.Behaviours.Where((w) => w is StickyBehaviour))
                        ((StickyBehaviour)unstick).ReleaseAllBalls();


                }


            }
            //            foreach(var splitball in (from m in gs.Balls where !m.hasBehaviour(typeof(TempBallBehaviour)) select m))
            foreach (var splitball in gs.Balls)
            {
                //split splitball into SplitCount new balls.
                //first, take note of the absolute speed of the ball.
                double magnitude = splitball.TotalSpeed;
                //how big will the angle be between them?
                double anglediff = (Math.PI * 2) / SplitCount;
                double startangle = BCBlockGameState.rgen.NextDouble() * Math.PI * 2;

                for (int i = 0; i < SplitCount; i++)
                {
                    double fireangle = startangle + (anglediff * i);


                    PointF usevelocity = new PointF((float)(Math.Cos(fireangle) * magnitude), (float)(Math.Sin(fireangle) * magnitude));

                    //clone the ball...
                    //use the proper type of the object, it could be a subclass!
                    cBall ballcreate = (cBall)Activator.CreateInstance(splitball.GetType(), splitball);



                    
                    //set the velocity of the clone to the calculated value.

                    ballcreate.Velocity = usevelocity;
                    addballs.Add(ballcreate);


                }

                removethese.Add(splitball);
            }
            foreach (var addit in addballs) gs.Balls.AddLast(addit);

            foreach (cBall removeit in removethese)
            {
                gs.Balls.Remove(removeit);


            }
            BCBlockGameState.Soundman.PlaySound("ELECTRICLASER", false);
            return true;
        }
        protected static Image GetSplitterImage(int count)
        {
            if (!SplitterPics.ContainsKey(count))
            {
                //if the key is not present, create the image and add it.
                Image drawimage = new Bitmap(128, 64);
                Graphics g = Graphics.FromImage(drawimage);
                g.FillRectangle(new SolidBrush(Color.Green), 0, 0, 128, 64);
                g.DrawRectangle(new Pen(Color.Red, 4), 0, 0, 128, 64);
                //paint the text.
                Font drawfont = BCBlockGameState.GetScaledFont(new Font("Arial", 64), 64);
                SizeF stringsize = g.MeasureString(count.ToString(), drawfont);
                g.DrawString(count.ToString(), drawfont, new SolidBrush(Color.Black), 128 - (stringsize.Width / 2), 2);
                g.Dispose();
                //add the image 
                SplitterPics.Add(count, drawimage);


            }


            return SplitterPics[count];

        }
        public BallSplitterPowerup(PointF Location, SizeF ObjectSize)
            : this(Location, ObjectSize, 3)
        {

        }

        public BallSplitterPowerup(PointF Location, SizeF ObjectSize, int SplitCount)
            : base(Location, ObjectSize, GetSplitterImage(SplitCount), null)
        {
            base.usefunction = BallSplit;

        }



    }
}