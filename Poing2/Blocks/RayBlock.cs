using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Runtime.Serialization;
using System.Xml.Linq;
using BASeCamp.BASeBlock.Particles;
using BASeCamp.Elementizer;

namespace BASeCamp.BASeBlock.Blocks
{
    [Serializable()]
    [ImpactEffectBlockCategory]
    [BlockDescription("Shoots a temporary ball in a given direction each time it is hit.")]
    public class RayBlock : ImageBlock
    {
        public enum RayFireDirection
        {
            Fire_Left,
            Fire_Up,
            Fire_Right,
            Fire_Down,


        }

        protected Type _ShootProjectileType = typeof(ProjectileBall);
        private String _ShootProjectileTypeString
        {
            get { return _ShootProjectileType.Name; }
            set { _ShootProjectileType = BCBlockGameState.FindClass(value); }
        }
        [Editor(typeof(ItemTypeEditor<iProjectile>),typeof(UITypeEditor))]
        public Type ShootProjectileType { get { return _ShootProjectileType; } set { _ShootProjectileType = value; } }

        public RayFireDirection mFireDirection = RayFireDirection.Fire_Up;
        private float LaunchVelocity = 3;
        /// <summary>
        /// Position from which to launch projectiles. relative to the block. defaults to top-center.
        /// </summary>
        private PointF LaunchPosition;

        public RayBlock(RectangleF blockrect, PointF LaunchVelocity, PointF LaunchPosition)
            : base(blockrect, "rayblock")
        {


        }
        public RayBlock(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {

            //custom added code here...
            //info.AddValue("LaunchVelocity", LaunchVelocity);
            //info.AddValue("LaunchPosition", LaunchPosition);
            mFireDirection = (RayFireDirection)info.GetValue("FireDirection", typeof(RayFireDirection));
            LaunchPosition = (PointF)info.GetValue("LaunchPosition", typeof(PointF));
            LaunchVelocity = info.GetSingle("LaunchVelocity");
            try { _ShootProjectileTypeString = info.GetString("ShootProjectileType"); }
            catch { _ShootProjectileType = typeof(ProjectileBall); }

        }
        public RayBlock(XElement Source, Object pPersistenceData) :base(Source,pPersistenceData)
        {
            mFireDirection = (RayFireDirection) Source.GetAttributeInt("FireDirection");
            LaunchVelocity = Source.GetAttributeFloat("LaunchVelocity");
            LaunchPosition = (PointF)Source.ReadElement<PointF>("LaunchPosition");
            try { _ShootProjectileTypeString = Source.GetAttributeString("ShootProjectileType");
            }
            catch(Exception exx)
            {
                _ShootProjectileType = typeof(ProjectileBall);
            }
            
        }

        public override XElement GetXmlData(String pNodeName,Object pPersistenceData)
        {
            var result = base.GetXmlData(pNodeName,pPersistenceData);
            result.Add(new XAttribute("FireDirection",(int) mFireDirection));
            result.Add(StandardHelper.SaveElement(LaunchPosition,"LaunchPosition",pPersistenceData));
            result.Add(new XAttribute("LaunchVelocity",LaunchVelocity));
            result.Add(new XAttribute("ShootProjectileType",_ShootProjectileType.Name));
            return result;

        }

        public RayBlock(RectangleF blockrect)
            : this(blockrect, new PointF(0, -3), new PointF(blockrect.Width / 2, 0))
        {
        }
        protected RayBlock(ImageBlock clonethis)
            : base(clonethis)
        {




        }


        public override string GetToolTipInfo(IEditorClient Client)
        {
            return base.GetToolTipInfo(Client) + "\n" + "Shoots:" + _ShootProjectileTypeString;
        }
        public override object Clone()
        {
            return new RayBlock(this);
        }
        public void ShootProjectile(BCBlockGameState parentstate)
        {
            ShootProjectile(parentstate, 2);

        }
        public void ShootProjectile(BCBlockGameState parentstate,float Speed)
        {
            //first, determine appropriate velocity and starting locations.
            float usespeed = Speed;
            float useradius = 2f;
            PointF VelocityUse = new PointF();
            PointF InitialLocation = new PointF();
            float CenterX = BlockRectangle.Left + (BlockRectangle.Width / 2);
            float CenterY = BlockRectangle.Top + (BlockRectangle.Height / 2);
            switch (mFireDirection)
            {
                case RayFireDirection.Fire_Up:
                    VelocityUse = new PointF(0, -usespeed);
                    InitialLocation = new PointF(CenterX, BlockRectangle.Top - useradius);
                    break;
                case RayFireDirection.Fire_Down:
                    VelocityUse = new PointF(0, usespeed);
                    InitialLocation = new PointF(CenterX, BlockRectangle.Bottom + useradius);
                    break;
                case RayFireDirection.Fire_Left:
                    VelocityUse = new PointF(-usespeed, 0);
                    InitialLocation = new PointF(BlockRectangle.Left - useradius, CenterY);
                    break;
                case RayFireDirection.Fire_Right:
                    VelocityUse = new PointF(usespeed, 0);
                    InitialLocation = new PointF(BlockRectangle.Right + useradius, CenterY);
                    break;
            }

            if (parentstate.GameArea.Contains(new Point((int)InitialLocation.X + (int)VelocityUse.X, (int)InitialLocation.Y + (int)VelocityUse.Y)))
            {

                //Debug.Print("using velocity:" + VelocityUse.ToString());
                iProjectile newprojectile = (iProjectile)Activator.CreateInstance(_ShootProjectileType);
                newprojectile.Location = InitialLocation;
                newprojectile.Velocity = VelocityUse;

                if (newprojectile is cBall)
                {
                    parentstate.NextFrameCalls.Enqueue(new BCBlockGameState.NextFrameStartup(() => parentstate.Balls.AddLast((cBall)newprojectile)));
                }
                else if (newprojectile is GameObject)
                {
                    parentstate.NextFrameCalls.Enqueue(new BCBlockGameState.NextFrameStartup(() => 
                                                                                             parentstate.GameObjects.AddLast((GameObject)newprojectile)));

                }

                //parentstate.NextFrameCalls.Enqueue(new BCBlockGameState.NextFrameStartup(()=>parentstate.GameObjects.AddLast(newprojectile));
                /*cBall newball = new cBall(InitialLocation, VelocityUse);
                //newball.isTempBall=true;
                newball.Behaviours.Clear();
                newball.Behaviours.Add(new TempBallBehaviour());
                newball.Radius = useradius;
                ballsadded.Add(newball);*/
            }

        }

        public override bool PerformBlockHit(BCBlockGameState parentstate, cBall ballhit)
        {
            BCBlockGameState.Soundman.PlaySound("ray", 0.9f);
            //parentstate.GameScore += 65;
            //AddScore(parentstate, 10);
            ShootProjectile(parentstate,ballhit.getMagnitude());

            /*if (BlockRectangle.Top > 10)
            {
                cBall newball =
                    new cBall(
                        new PointF(BlockRectangle.Left + (float) (BlockRectangle.Width/2f), BlockRectangle.Top - 5),
                        new PointF(0, -4));
                newball.isTempBall = true;


                ballsadded.Add(newball);
            } */
            //ray blocks cannot be destroyed.
            return false;
        }
        protected override Particle AddStandardSprayParticle(BCBlockGameState parentstate, cBall ballhit)
        {
            //return base.AddStandardSprayParticle(parentstate, ballhit);
            return AddSprayParticle_Default(parentstate, ballhit);
        }
        public override bool MustDestroy()
        {
            return false;
        }
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("LaunchVelocity", LaunchVelocity);
            info.AddValue("LaunchPosition", LaunchPosition);
            info.AddValue("FireDirection", mFireDirection);
            info.AddValue("ShootProjectileType", _ShootProjectileTypeString);
            
        }
    }
}