using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.Serialization;
using System.Xml.Linq;
using BASeCamp.Elementizer;

namespace BASeCamp.BASeBlock.Blocks
{
    [BlockDescription("Attracts or Repels Balls.")]
    [Serializable]

    public class BlackHoleBlock : ImageBlock
    {

        //first, a simple class that is used to store a few variables within the ball (specifically, the mass that is to be rendered unto the ball)
        [Serializable]
        public class GravityBlockBallBehaviour : BaseBehaviour, ISerializable
        {
            public double BallMass { get; set; }
            public GravityBlockBallBehaviour(double pBallMass)
            {
                BallMass = pBallMass;

            }
            public GravityBlockBallBehaviour(GravityBlockBallBehaviour clonethis)
            {
                BallMass = clonethis.BallMass;

            }
            public GravityBlockBallBehaviour(XElement Source, Object pPersistenceData) :base(Source,pPersistenceData)
            {
                BallMass = Source.GetAttributeDouble("BallMass", 1);
            }
            public override XElement GetXmlData(String pNodeName,Object pPersistenceData)
            {

                return new XElement(pNodeName, new XAttribute("BallMass", BallMass));
            }
            public override object Clone()
            {
                return new GravityBlockBallBehaviour(this);
            }

            #region iBallBehaviour Members

            private static double Distance(PointF PointA, PointF PointB)
            {

                return Math.Sqrt(Math.Pow(Math.Abs(PointB.X - PointA.X), 2) + Math.Pow(Math.Abs(PointB.Y - PointA.Y), 2));

            }
            private bool isBlackHole(Block testblock)
            {
                if (testblock is BlackHoleBlock) return true;

                if (testblock is AnimatedBlock)
                {
                    return (testblock as AnimatedBlock).baseBlock is BlackHoleBlock;


                }

                return false;

            }
            private Block getbhole(Block fromblock)
            {
                if (fromblock is BlackHoleBlock) return fromblock;
                if (fromblock is AnimatedBlock)
                {
                    return (fromblock as AnimatedBlock).baseBlock;

                }
                return null;
            }

            public override List<Block> PerformFrame(cBall ballobject, BCBlockGameState ParentGameState, ref List<cBall> ballsadded, ref List<cBall> ballsremove, out bool removethis)
            {
                //this is the "miracle"; rather then the black hole block being
                //responsible for looping through all balls every frame and adjusting their trajectories, instead it is done by a ball behaviour, in the Opposite direction (that is, it acquires all the black hole blocks and performs the appropriate calculations)
                removethis = false;
                List<Block> blackholeblocks = (from y in ParentGameState.Blocks where (isBlackHole(y)) select getbhole(y)).ToList();
                blackholeblocks.AddRange((from y in ParentGameState.Blocks where (y is BoundedMovingBlock && ((BoundedMovingBlock)y).baseBlock is BlackHoleBlock) select ((BoundedMovingBlock)y).baseBlock).ToList());
                if (blackholeblocks.Count == 0) return null;
                //perform velocity "correction" between this ball and each of the acquired blocks.

                //attract to the block with a force proportional to the product of their masses, and inversely proportional to the distance
                //between them.
                //uses simple newtonian physics.
                const double G = 6.673E-8;

                foreach (Block loopblock in blackholeblocks)
                {
                    BlackHoleBlock casted = (BlackHoleBlock)loopblock;
                    double distance = Distance(loopblock.CenterPoint(), ballobject.Location);
                    if(!casted.Interactive)
                    {
                        if(distance < casted.EventHorizon)
                        {
                            casted.PerformBlockHit(ParentGameState, ballobject);
                        }
                    }
                    // double Force = G * BallMass * casted.Mass / ((distance * distance));
                    double Force = BallMass * casted.Mass / ((distance * distance));
                    Force /= -3;
                    //now that we have the force being exerted, we'll just use it as the amount to change the velocity by;
                    //create the appropriate vector of that magnitude at the angle between the ball and the blocks center, then add that
                    //to the balls current velocity to arrive at the new velocity.
                    double angleforce = GetAngle(loopblock.CenterPoint(), ballobject.Location);

                    PointF veldelta = new PointF((float)(Math.Cos(angleforce) * Force), (float)(Math.Sin(angleforce) * Force));

                    //and lastly, add the veldelta to the balls velocity
                    ballobject.Velocity = new PointF(ballobject.Velocity.X + veldelta.X, ballobject.Velocity.Y + veldelta.Y);





                }
                //ta da!
                return null;

                /*
                 * Force = Gravitational constant x mass of 1st object x mass of 2nd object / distance squared.

F=Gm1m2 / d2

Where G=6.672 x10-11 Nm2/kg2

Read more: http://wiki.answers.com/Q/The_amount_of_gravitational_force_between_objects_depends_on_their#ixzz1CEEcfss3

                 * */



                //throw new NotImplementedException();

            }
            

            #endregion

            #region ISerializable Members

            public override void GetObjectData(SerializationInfo info, StreamingContext context)
            {
                info.AddValue("Mass", BallMass);
            }
            public GravityBlockBallBehaviour(SerializationInfo info, StreamingContext context)
            {
                BallMass = info.GetSingle("Mass");
            }

         
            #endregion
        }


        public double Mass { get; set; }
        /// <summary>
        /// whether this block will be drawn and interact as a block.
        /// if false, it will not appear at all, and effectively be invisible.
        /// </summary>
        public bool Interactive { get; set; }
        private int _EventHorizon = 3;
        public int EventHorizon { get { return _EventHorizon;} set {_EventHorizon = value;} }
        private bool firstframecalled = false;

        public BlackHoleBlock(RectangleF blockrect)
            : base(blockrect, "BLACKHOLE")
        {
            Mass = 1000;

        }
        public BlackHoleBlock(ImageBlock cloneme)
            : base(cloneme.BlockRectangle, cloneme.BlockImageKey)
        {
            Mass = ((BlackHoleBlock)cloneme).Mass;
        }
        public BlackHoleBlock(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            Mass = info.GetSingle("Mass");
            Interactive = info.GetBoolean("Interactive");
            EventHorizon = info.GetInt32("EventHorizon");

        }
        public BlackHoleBlock(XElement Source,Object pPersistenceData):base(Source,pPersistenceData)
        {
            Mass = Source.GetAttributeDouble("Mass");
            
        }

        public override XElement GetXmlData(String pNodeName,Object pPersistenceData)
        {
            var Result = base.GetXmlData(pNodeName,pPersistenceData);
            Result.Add(new XAttribute("Mass",Mass));
            Result.Add(new XAttribute("Interactive",Interactive));
            Result.Add(new XAttribute("EventHorizon",EventHorizon));
            return Result;
        }

        public override bool PerformBlockHit(BCBlockGameState parentstate, cBall ballhit)
        {
            //if our mass is positive....
            if (Mass > 0)
            {
                if(!Interactive) return false;
                //we are a "black hole" block; thus any balls that hit us are swallowed up.
                if (parentstate.Balls.Count((w) => !w.isTempBall) > 1)
                    parentstate.RemoveBalls.Add(ballhit);

            }
            else
            {
                //ballsadded.Add(new cBall(new PointF(CenterPoint().X, BlockRectangle.Bottom), new PointF(0, -2)));
            }
            return false;
        }
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("Mass", Mass);
            info.AddValue("Interactive",Interactive);
            info.AddValue("EventHorizon",EventHorizon);
        }
        public override object Clone()
        {
            return new BlackHoleBlock(this);
        }
        public override bool MustDestroy()
        {
            return false;
        }

    }
}