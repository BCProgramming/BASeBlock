using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.Serialization;

namespace BASeBlock.Blocks
{
    [Serializable()]
    [StandardBlockCategory]
    [BlockDescription("Shoots temporary Balls in various directions when hit.")]
    public class BombBlock : ImageBlock
    {
        [Flags]
        public enum BombBlockShotDirections
        {
            BBS_LEFT = 2,
            BBS_UP = 4,
            BBS_RIGHT = 8,
            BBS_DOWN = 16,
            BBS_UPLEFT = 32,
            BBS_UPRIGHT = 64,
            BBS_DOWNLEFT = 128,
            BBS_DOWNRIGHT = 256



        }
       




        private BombBlockShotDirections mShotDirections = BombBlockShotDirections.BBS_LEFT | BombBlockShotDirections.BBS_RIGHT |
                                                          BombBlockShotDirections.BBS_UP | BombBlockShotDirections.BBS_DOWN;
        //accepts a BombBlockShotDirections enumeration, and returns a list of the appropriate
        //set of balls that would be "exploded" from the bomb block when hit by impactball.
        private List<cBall> GetBallsForDirections(BombBlockShotDirections bbdirection, cBall impactBall)
        {
            PointF usepoint = new PointF(BlockRectangle.Left + BlockRectangle.Width / 2, BlockRectangle.Top + BlockRectangle.Height / 2);
            cBall ballhit = impactBall;
            cBall LeftShoot = new cBall(usepoint, new PointF(-ballhit.getMagnitude(), 0)) { Radius = 3, isTempBall = true, DrawColor = Color.Gray };
            cBall RightShoot = new cBall(usepoint, new PointF(ballhit.getMagnitude(), 0)) { Radius = 3, isTempBall = true, DrawColor = Color.Gray };
            cBall TopShoot = new cBall(usepoint, new PointF(0, -ballhit.getMagnitude())) { Radius = 3, isTempBall = true, DrawColor = Color.Gray };
            cBall BottomShoot = new cBall(usepoint, new PointF(0, ballhit.getMagnitude())) { Radius = 3, isTempBall = true, DrawColor = Color.Gray };
            float usevel = (float)Math.Sqrt(Math.Pow(ballhit.getMagnitude(), 2) * 2);

            cBall UpLeftShoot = new cBall(usepoint, new PointF(-usevel, -usevel)) { Radius = 3, isTempBall = true, DrawColor = Color.Gray };
            cBall UpRightShoot = new cBall(usepoint, new PointF(usevel, -usevel)) { Radius = 3, isTempBall = true, DrawColor = Color.Gray };
            cBall DownLeftShoot = new cBall(usepoint, new PointF(-usevel, usevel)) { Radius = 3, isTempBall = true, DrawColor = Color.Gray };
            cBall DownRightShoot = new cBall(usepoint, new PointF(usevel, usevel)) { Radius = 3, isTempBall = true, DrawColor = Color.Gray };
            List<cBall> returnballs = new List<cBall>();
            List<cBall> ballsadded = new List<cBall>();
            ballsadded.AddRange(new cBall[] { LeftShoot, RightShoot, TopShoot, BottomShoot, UpLeftShoot, UpRightShoot, DownLeftShoot, DownRightShoot });
            for (int i = 0; i < ballsadded.Count; i++)
            {
                BombBlockShotDirections checkit = ((BombBlockShotDirections)(Math.Pow(2, i + 1)));
                if (((bbdirection & ((BombBlockShotDirections)checkit)) == checkit))
                    returnballs.Add(ballsadded[i]);





            }
            return returnballs;



        }

        public BombBlock(RectangleF blockrect)
            : base(blockrect, "BOMB")
        {



        }
        public BombBlock(BombBlock clonethis)
            : base(clonethis.BlockRectangle, "BOMB")
        {



        }
        public BombBlock(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {

            BlockImageKey = "BOMB";
            mShotDirections = (BombBlockShotDirections)(info.GetValue("ShotDirection", typeof(BombBlockShotDirections)));

        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("ShotDirection", mShotDirections, typeof(BombBlockShotDirections));


        }
        public override bool PerformBlockHit(BCBlockGameState parentstate, cBall ballhit)
        {
            parentstate.GameScore += 50;
            BCBlockGameState.Soundman.PlaySound("BOMB", 0.9f);
            //Brush usebrush = new SolidBrush(Color.Gray);
            PointF usepoint = new PointF(BlockRectangle.Left + BlockRectangle.Width / 2, BlockRectangle.Top + BlockRectangle.Height / 2);
            /*
                    cBall LeftShoot = new cBall(usepoint, new PointF(-ballhit.getMagnitude(), 0)) { Radius = 3,isTempBall=true,DrawColor=Color.Gray};
                    cBall RightShoot = new cBall(usepoint, new PointF(ballhit.getMagnitude(), 0)) { Radius = 3, isTempBall = true, DrawColor=Color.Gray };



                    cBall TopShoot = new cBall(usepoint, new PointF(0, -ballhit.getMagnitude())) { Radius = 3, isTempBall = true, DrawColor=Color.Gray};
                    cBall BottomShoot = new cBall(usepoint, new PointF(0, ballhit.getMagnitude())) { Radius = 3, isTempBall = true, DrawColor = Color.Gray};
                    ballsadded.AddRange(new cBall[] { LeftShoot, RightShoot, TopShoot, BottomShoot });*/
            parentstate.NextFrameCalls.Enqueue(new BCBlockGameState.NextFrameStartup(() =>

                                                                                     GetBallsForDirections(mShotDirections, ballhit)));
            //                    parentstate.Balls.AddRangeAfter(new cBall[] { LeftShoot, RightShoot, TopShoot, BottomShoot });
            //parentstate.Balls.AddRange(new cBall[] { LeftShoot, RightShoot, TopShoot, BottomShoot });
            //parentstate.Balls.Add
            StandardSpray(parentstate, ballhit);
            return true;
        }
        public override object Clone()
        {
            return new BombBlock(this);
        }

    }
}