using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.Serialization;
using System.Xml.Linq;
using BASeCamp.Elementizer;

namespace BASeCamp.BASeBlock.Blocks
{
    [Serializable]
    [BlockDescription("Grabs balls that come close in a perpendicular direction and spins them a bit before releasing them.")]
    public class TetherBlock : ImageBlock, ISerializable
    {



        private float _TetherRadius = 64;
        /// <summary>
        /// class used to store information on newly detached balls.
        /// this is used to prevent them from being immediately "tethered" again.
        /// </summary>
        private class DetachedBallData
        {
            public cBall DetachedBall;
            public DateTime DetachTime;


        }
        private class AttachedBallData
        {
            public cBall theBall;
            public double Radius;
            public double InitialAngle;
            public double PreviousAngle;
            public bool doRemove = false;
            public ProxyBallBehaviour proxyBehaviour;
            public AttachedBallData(cBall ptheBall, double pRadius, double pInitialAngle)
            {
                theBall = ptheBall;
                Radius = pRadius;
                InitialAngle = pInitialAngle;

            }

        }
        private Queue<DetachedBallData> DetachmentQueue = new Queue<DetachedBallData>();

        private Dictionary<cBall, AttachedBallData> attachedBalls = new Dictionary<cBall, AttachedBallData>();

        public float TetherRadius { get { return _TetherRadius; } set { _TetherRadius = value; } }

        public TetherBlock(RectangleF Blockrect)
            : base(Blockrect, "TETHER")
        {


        }
        public override object Clone()
        {
            return new TetherBlock(this);
        }
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("TetherRadius", _TetherRadius);
        }
        public TetherBlock(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            _TetherRadius = info.GetSingle("TetherRadius");

        }
        public TetherBlock(XElement Source)
        {
            TetherRadius = Source.GetAttributeFloat("TetherRadius");
        }

        public override XElement GetXmlData(string pNodeName)
        {
            var result = base.GetXmlData(pNodeName);
            result.Add(new XAttribute("TetherRadius",TetherRadius));
            return result;
        }

        public TetherBlock(TetherBlock clonethis)
            : base(clonethis)
        {
            TetherRadius = clonethis.TetherRadius;

        }
        public override bool RequiresPerformFrame()
        {
            return true;
        }
        public override bool PerformFrame(BCBlockGameState gamestate)
        {
            //return base.PerformFrame(gamestate);


            foreach (cBall CheckBall in (from p in gamestate.Balls where BCBlockGameState.Distance(CenterPoint().X, CenterPoint().Y, p.Location.X, p.Location.Y) < _TetherRadius select p))
            {
                //is it perpendicular?
                Debug.Print("Checking Ball at location: " + CheckBall.Location.X + "," + CheckBall.Location.Y);
                if (IsPerpendicular(CheckBall))
                {
                    if (!attachedBalls.ContainsKey(CheckBall))
                    {
                        AttachBall(gamestate,CheckBall);

                    }

                }

            }
            return true;

        }

        public override bool MustDestroy()
        {
            return false;
        }
        public override bool PerformBlockHit(BCBlockGameState parentstate, cBall ballhit)
        {
            return false;
        }
        //bool doremove = false;
        public List<Block> Ball_PerformFrame(cBall ballobject, BCBlockGameState ParentGameState, ref List<cBall> ballsadded, ref List<cBall> ballsremove, out bool removethis)
        {
            //delegate used with proxy object of balls that are tethered.
            //first, we need to get the AttachedBall data for this ball.
            //because we can't change the parameters to the delegate (duh) we have that all cached in the AttachedBalls dictionary, indexed by the ball itself, which we
            //are kindly provided in the parameter list.
            if (!attachedBalls.ContainsKey(ballobject))
            {
                removethis = true;
                return null;


            }
            AttachedBallData ballattachdata = attachedBalls[ballobject];
            PointF currentLocation = ballobject.Location;

            //calculate the next location.
            float gotmult = ParentGameState.GetMultiplier();
            PointF nextLocation = new PointF(ballobject.Location.X + ballobject.Velocity.X+gotmult, ballobject.Location.Y + ballobject.Velocity.Y*gotmult);

            //get difference the angle between the current location and the next positions angle, from the blocks center point.

            double CurrentAngle = BCBlockGameState.GetAngle(CenterPoint(), ballobject.Location);
            double NextAngle = BCBlockGameState.GetAngle(CenterPoint(), nextLocation);

            double difference = NextAngle - CurrentAngle;

            double arclength = ballobject.TotalSpeed;
            //pretend that distance is an "arc", and get the angle...
            //arc length = angle times radius

            double anglegot = arclength / ballattachdata.Radius;
            anglegot = difference;
            double accumangle = BCBlockGameState.GetAngle(CenterPoint(), ballobject.Location) + anglegot;

            //calculate the new location of the ball.

            PointF newlocation = new PointF((float)(Math.Cos(accumangle) * ballattachdata.Radius), (float)(Math.Sin(accumangle) * ballattachdata.Radius));

            //we want to keep going at the same speed, but in the new direction.
            //so, get the angle between the calculated location and the old location...
            PointF newlocabs = new PointF(CenterPoint().X + newlocation.X, CenterPoint().Y + newlocation.Y); ;
            double calcspeedangle = BCBlockGameState.GetAngle(ballobject.Location, newlocabs);

            //calc new  speed based on that angle (arclength is the speed to use).
            PointF calcnewspeed = new PointF((float)(Math.Cos(calcspeedangle) * arclength), (float)(Math.Sin(calcspeedangle) * arclength));
            //new speed needs to take the multiplier into account.
            
            // calcnewspeed = new PointF(calcnewspeed.X * gotmult, calcnewspeed.Y * gotmult);

            ballobject.Location = newlocabs;

            //get the difference between this location and the balls current location.

            //PointF newspeed = new PointF(newlocation.X - currentLocation.X, newlocation.Y - currentLocation.Y);
            //this is the new speed of the ball.
            ballobject.Velocity = calcnewspeed;

            //if the accumulated angle is more than PI  away (180 degrees) from the initial location, detach it

            
            /* doremove |= (Math.Abs(accumangle - ballattachdata.InitialAngle) > Math.PI);

            if (ballattachdata.PreviousAngle != 0 && Math.Sign(accumangle) != Math.Sign(ballattachdata.PreviousAngle))
            {
                doremove = true;


            }
            * */
            //balls are never held for more than a second.
            //doremove = (DateTime.Now - ballattachdata.AttachTime) > new TimeSpan(0, 0, 0, 0, 1000);

            if (ballattachdata.doRemove)
            {
                //Debug.Print("Detach");
                DetachBall(ballobject);
                removethis = true;
                return null;
            }
            else
            {
                ballattachdata.PreviousAngle = accumangle;
                //Debug.Print(String.Format("accumangle is:{0:0.00} Initial is {1:0.00}", accumangle, ballattachdata.InitialAngle));
            }



            removethis = false;
            return null;






        }
        public void Ball_Draw(cBall balldraw, Graphics g)
        {
            //g.DrawLine(new Pen(Color.Black, 2), CenterPoint(), balldraw.Location);
            g.DrawLineRandom(CenterPoint(), balldraw.Location, new Pen(Color.OrangeRed, 1));

        }

        /// <summary>
        /// "Attach" the given ball to this tether block.
        /// </summary>
        /// <param name="ballattach"></param>
        protected void AttachBall(BCBlockGameState gstate,cBall ballattach)
        {
            if ((from p in ballattach.Behaviours where (p is ProxyBallBehaviour && ((ProxyBallBehaviour)p).Tag.Equals("tether")) select p).Count() > 0) return;

            bool noattach = false;
            //further test; check recently detached balls.
            if (DetachmentQueue.Any())
            {
                var QueueTop = DetachmentQueue.Peek();
                TimeSpan onesecond = new TimeSpan(0, 0, 0, 1);
                while (DateTime.Now - QueueTop.DetachTime > onesecond)
                {

                    QueueTop = DetachmentQueue.Dequeue();
                    if (QueueTop.DetachedBall == ballattach) noattach = true;

                }
                if (noattach) return;
            }


            double angleuse = BCBlockGameState.GetAngle(CenterPoint(), ballattach.Location);

            AttachedBallData attachdata = new AttachedBallData(ballattach, BCBlockGameState.Distance(CenterPoint().X, CenterPoint().Y, ballattach.Location.X, ballattach.Location.Y), angleuse);
            ballattach.BallImpact += new cBall.ballimpactproc(ballattach_BallImpact); //hook ballimpact.
            
            
            //change: use delayinvoke to determine when to detach.
            gstate.DelayInvoke(new TimeSpan(0, 0, 0, 1), attachballlimit, new object[]{ballattach});
            //attachdata.AttachTime = DateTime.Now;



            attachedBalls.Add(ballattach, attachdata);
            //here we would also create a "proxy" ball behaviour, so that we can make adjustments to the ball's speed each frame.
            //this is done using a proxyBehaviourObject... (naturally).
            ProxyBallBehaviour proxied = new ProxyBallBehaviour("tether", null, Ball_PerformFrame, null, null, null, Ball_Draw, null, null);
            //assign the initial angle to the attached Data
            proxied.ownerball = ballattach;

            attachdata.proxyBehaviour = proxied;
            ballattach.Behaviours.Add(proxied);



        }
        private void attachballlimit(object[] parameters)
        {
            //this is called after one second of game time elapses. (thanks to the delayinvoke code).
            cBall converted = parameters[0] as cBall;
            if(converted!=null && attachedBalls.ContainsKey(converted))
                attachedBalls[converted].doRemove = true;
            Debug.Print("attachballlimit");
            


        }
        protected void DetachBall(cBall balldetach)
        {
            //remove the dictionary element.
            if (attachedBalls.ContainsKey(balldetach))
            {
                AttachedBallData attacheddata = attachedBalls[balldetach];
                //balldetach.Behaviours.Remove(attacheddata.proxyBehaviour); 
                //bugfix: we cannot, obviously remove the behaviour since we are executing (usually) within the context of a foreach() running through all the Behaviours.
                //so we'll just have it set the flag instead (obviously).

                //also: add line particles between the block and ball, to show the tether "breaking"


                attachedBalls.Remove(balldetach);
            }
            //remove the behaviour as well.



        }

        void ballattach_BallImpact(cBall ballimpact)
        {
            DetachBall(ballimpact);
            //detach the ball!
            //throw new NotImplementedException();
            
        }

        protected bool IsPerpendicular(cBall testball)
        {
            const double tolerance = 16;
            PointF BallSpeed = testball.Velocity;
            PointF diff = new PointF(testball.Location.X - CenterPoint().X, testball.Location.Y - CenterPoint().Y);

            double dotproduct = BallSpeed.X * diff.X + BallSpeed.Y * diff.Y;

            //Debug.Print("Dot product was " + dotproduct);

            if (Math.Abs(dotproduct) < tolerance) return true; else return false;

            //x1*x2 + y1*y2




        }




    }
}