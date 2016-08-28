using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Design;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Windows.Forms;
using System.Xml.Linq;
using BASeCamp.BASeBlock.Blocks;
using BASeCamp.BASeBlock.PaddleBehaviours;
using BASeCamp.BASeBlock.Particles;
using BASeCamp.BASeBlock.Projectiles;
using BASeCamp.Elementizer;

//using System.Xml.Serialization;

namespace BASeCamp.BASeBlock
{



    #region ballbehaviours

    public enum HitWallReturnConstants
    {
        HitBall_Default, //default; ball is destroyed at the bottom 
        HitBall_Destroy, //Destroy the ball.
        HitBall_Preserve, //Ball remains alive.


    }

    public interface iBallDrawHandler : ISerializable
    {

        void Draw(cBall ball, Graphics g);


    }

    public class BallDrawHandler_Standard : iBallDrawHandler
    {
        public BallDrawHandler_Standard()
        {


        }

        public BallDrawHandler_Standard(SerializationInfo info, StreamingContext context)
        {
            //nothing...

        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            //nothing

        }
        private RectangleF getrect(PointF CenterPoint, float Radius)
        {

            return new RectangleF(CenterPoint.X - Radius, CenterPoint.Y - Radius, Radius*2, Radius*2);

        }
        public void Draw(cBall ball, Graphics g)
        {
            
            RectangleF ballrect = ball.getballrect();
            //perform default drawing

            try
            {
                g.FillEllipse(ball.DrawBrush, ballrect);
                g.DrawEllipse(ball.DrawPen, ballrect);

                foreach (cBall.TrailItemData tid in ball.TrailData)
                {

                    ballrect = getrect(tid.Location, tid.Radius);
                    g.FillEllipse(ball.DrawBrush, ballrect);
                    g.DrawEllipse(ball.DrawPen, ballrect);
                }

            }
            catch (OverflowException e)
            {
                Debug.Print("Caught overflow exception...");
            }


        }



    }

    public class BallDrawHandler_Arrow : iBallDrawHandler
    {

        //draws an "arrow", whose arrowhead is centered on our location.
        private static PointF[] getTrianglePoints(float Radius,float Angle)
        {
            PointF[] returnthis = new PointF[3];
            returnthis[0] = new PointF((float)Math.Cos(Angle)*Radius,(float)Math.Sin(Angle)*Radius);
            double pi7 = Math.PI*0.7+Angle;
            double pi2 = Math.PI*1.3+Angle;
            returnthis[1] = new PointF((float)Math.Cos(pi7)*Radius,(float)Math.Sin(pi7)*Radius);
            returnthis[2] = new PointF((float)Math.Cos(pi2)*Radius,(float)Math.Sin(pi2)*Radius);

            return returnthis;
            



        }
       

        


        #region iBallDrawHandler Members

        public void Draw(cBall ball, Graphics g)
        {
            const double Arrowlength = 25;
            double movementangle = BCBlockGameState.GetAngle(PointF.Empty,ball.Velocity);
            double obverse = (movementangle+(Math.PI)) % Math.PI; //180 degrees from movementangle.
            PointF[] arrowhead = getTrianglePoints(ball.Radius,(float)movementangle);

            GraphicsPath arrowheadpath = new GraphicsPath();
            arrowheadpath.AddPolygon(arrowhead);
            //add the line for the arrow shaft...
            g.DrawPath(ball.DrawPen,arrowheadpath);
            arrowheadpath.Dispose();

            PointF arrowfletchpos = new PointF((float)(Math.Cos(obverse)* Arrowlength)+ball.Location.X,
                (float)(Math.Sin(obverse)*Arrowlength)+ball.Location.Y);
            //draw the arrow shaft...
            g.DrawLine(ball.DrawPen,ball.Location,arrowfletchpos);


        }

        #endregion

        #region ISerializable Members

        public BallDrawHandler_Arrow(SerializationInfo info,StreamingContext context)
        {

            //nothing here either..

        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
           //nothing to deserialize...
        }

        #endregion
    }
    public static class iBallBehaviourExtension
{
        public static ISerializable Clone(this ISerializable copyit)
        {
            MemoryStream ms = new MemoryStream();
            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(ms, copyit);
            ms.Seek(0, SeekOrigin.Begin);
            return (ISerializable)bf.Deserialize(ms);



        }


}

public interface iBallBehaviour : ISerializable,IXmlPersistable
    {
        //similar in concept to PaddleBehaviours
        //each ball will have a list of them; as an example, I could make a "fire" block, which, when hit, adds a "FireBallBehavior" to the behaviour list.
        //the idea is that this will allow a ball to have a varied set of "powers". Of course, the appropriate measures should be taken so that 
        //powers that don't make sense at the same time, such as an Ice and a Fire behaviour, aren't there simultaneously.

      
        /// <summary>
        /// Called when the ball hits a block.
        /// </summary>
        /// <param name="blockhit">The block that was hit.</param>
        /// <returns>true to allow for standard ball velocity changes (bounce off the block) false otherwise.</returns>
        bool HitBlock(BCBlockGameState currentstate,cBall ballobject, Block blockhit);
        /// <summary>
        /// Called Each frame.
        /// </summary>
        /// <param name="ballobject">The ball</param>
        /// <param name="ParentGameState">GameState</param>
        /// <param name="ballsadded">Balls to add</param>
        /// <param name="ballsremove">Balls to remove</param>
        /// <param name="removethis">whether to remove this behaviour when the method returns.</param>
        /// <returns></returns>
        List<Block> PerformFrame(cBall ballobject, BCBlockGameState ParentGameState, ref List<cBall> ballsadded, ref List<cBall> ballsremove,out bool removethis);
    
        HitWallReturnConstants HitWall(BCBlockGameState currentstate, cBall ballobject); //return true to destroy this ball...
        void BehaviourAdded(cBall onBall, BCBlockGameState currstate); //called when the behaviour is added to a ball.
        void BehaviourRemoved(cBall onBall, BCBlockGameState currstate);
        
        void Draw(Graphics g);
        //returns the blocks that were affected by the frame (changed or moved).
        String PaddleBallSound();
        String BlockBallSound();
        

    }
/*
public class FrustratorBallBehaviour : TimedBallBehaviour
{
    private cBall _ownerBall = null;

    public cBall OwnerBall { get { return _ownerBall; } set { 
        
        
        _ownerBall = value; } }


    public FrustratorBallBehaviour(SerializationInfo info, StreamingContext context):base(info,context)
    {


    }
    public FrustratorBallBehaviour(TimeSpan FrustrationLength):base(FrustrationLength)
    {
        

    }

    public FrustratorBallBehaviour(FrustratorBallBehaviour Clonethis)
        : base(Clonethis)
    {
        _ownerBall =  Clonethis.OwnerBall;

    }
    public override object Clone()
    {
        return new FrustratorBallBehaviour(this);
    }


    public override List<Block> FrameTick(cBall ballobject, BCBlockGameState ParentGameState, ref List<cBall> ballsadded, ref List<cBall> ballsremove, out bool removethis)
    {
        //same as PerformFrame for BaseBallBehaviour.
        //the behaviour doesn't actually break or damage blocks

        removethis = false;
        //throw new NotImplementedException();
        return new List<Block>();
    }

}
    */

    /// <summary>
    /// abstract Class used for BallBehaviours that act on a timer.
    /// </summary>
    [Serializable]
public abstract class TimedBallBehaviour : BaseBehaviour
{
    private TimeSpan _TimeLength;
    private bool Destroyself = false;
    public TimeSpan TimeLength { get { return _TimeLength; } set { _TimeLength = value; } }

    protected TimedBallBehaviour(SerializationInfo info, StreamingContext context)
        : base(info, context)
    {
        _TimeLength = (TimeSpan)info.GetValue("TimeLength", typeof(TimeSpan));


    }
    protected TimedBallBehaviour(TimeSpan pTimeLength)
    {
        _TimeLength = pTimeLength;

    }
    protected TimedBallBehaviour(TimedBallBehaviour clonethis)
    {

        _TimeLength = clonethis.TimeLength;
    }
    
    public override void GetObjectData(SerializationInfo info, StreamingContext context)
    {
        base.GetObjectData(info, context);
        info.AddValue("TimeLength", _TimeLength);
    }


    public void DelayRoutine(Object[] parameters)
    {
        Destroyself = true;

    }
    public override void BehaviourAdded(cBall onBall, BCBlockGameState currstate)
    {
        base.BehaviourAdded(onBall, currstate);

        //start timer...
        currstate.DelayInvoke(_TimeLength, DelayRoutine, null);


    }
    public abstract List<Block> FrameTick(cBall ballobject, BCBlockGameState ParentGameState, ref List<cBall> ballsadded, ref List<cBall> ballsremove, out bool removethis);




    public override sealed List<Block> PerformFrame(cBall ballobject, BCBlockGameState ParentGameState, ref List<cBall> ballsadded, ref List<cBall> ballsremove, out bool removethis)
    {

        List<Block> result = FrameTick(ballobject, ParentGameState, ref ballsadded, ref ballsremove, out removethis);
        removethis = Destroyself || removethis;
        return result;
}
}

    //when added, this behaviour will destroy the ball when it impacts anything.
    //public class TempBallBehaviour : iBallBehaviour
    //{


    //}
    //proxy behaviour, used in a few cases where we cannot use the standard Proxy object.
    //it's a long story.


    [Serializable]
    public abstract class BaseBehaviour : iBallBehaviour,ICloneable 
    {
        private bool mInitialized=false;
        BCBlockGameState prevstate=null;
        cBall useball = null;
        protected BaseBehaviour(SerializationInfo info, StreamingContext context)
        {

        }
        protected BaseBehaviour(XElement Source)
        {

        }
        public abstract XElement GetXmlData(String pName);
        protected BaseBehaviour()
        {


        }
        ~BaseBehaviour()
        {
            BehaviourRemoved(useball, prevstate);


        }



        #region iBallBehaviour Members
       
        public virtual void BehaviourAdded(cBall onBall, BCBlockGameState currstate)
        {


        }
        public virtual void BehaviourRemoved(cBall onBall, BCBlockGameState currstate)
        {


        }

        public virtual bool HitBlock(BCBlockGameState currentstate, cBall ballobject, Block blockhit)
        {
            //throw new NotImplementedException();
            return true;
        }
   
        public virtual List<Block> PerformFrame(cBall ballobject, BCBlockGameState ParentGameState, ref List<cBall> ballsadded, ref List<cBall> ballsremove,out bool removethis)
        {
            removethis=false;
            if(!mInitialized)
            {
                useball = ballobject;
                prevstate = ParentGameState;
                BehaviourAdded(ballobject, ParentGameState);
                mInitialized=true;
                    
            }
            //throw new NotImplementedException();
            return null;
        }

        public virtual HitWallReturnConstants HitWall(BCBlockGameState currentstate, cBall ballobject)
        {
           // throw new NotImplementedException();
            return HitWallReturnConstants.HitBall_Default;
        }

        public virtual void Draw(Graphics g)
        {
            //do nothing
        }
        public static String DefaultBlockBallSound = "BBOUNCE";
        public virtual String BlockBallSound()
        {
            return DefaultBlockBallSound;  

        }
        public static String DefaultPaddleBallSound = "BOUNCE";
        public virtual String PaddleBallSound()
        {
            return DefaultPaddleBallSound; //default ball sound for paddle impact.

        }

        #endregion

        #region ISerializable Members

        public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            //
        }

        #endregion

        #region ICloneable Members

        public abstract Object Clone();

        #endregion
    }


    /// <summary>
    /// class used to make a ball emit particles
    /// </summary>
    [Serializable]
    [Description("Particle Emitter")]
    public class ParticleEmitterBehaviour : BaseBehaviour
    {
        public delegate void EmitParticle(cBall onball,BCBlockGameState gamestate);
        private cBall ourball;
        private Type _ParticleType = typeof(DustParticle);

        [Editor(typeof(ItemTypeEditor<Particle>), typeof(UITypeEditor))]
        public Type ParticleType { get { return _ParticleType; } set { _ParticleType = value; } }


        private EmitParticle EmitParticleProc = null;
        public override void BehaviourAdded(cBall onBall, BCBlockGameState currstate)
        {
            base.BehaviourAdded(onBall, currstate);
            ourball=onBall;    
        }
        private void defaultemitter(cBall onball,BCBlockGameState gamestate)
        {
            Particle addparticle = new DustParticle(onball);


        }
        public override List<Block> PerformFrame(cBall ballobject, BCBlockGameState ParentGameState, ref List<cBall> ballsadded, ref List<cBall> ballsremove, out bool removethis)
        {
            EmitParticleProc(ballobject, ParentGameState);
            return base.PerformFrame(ballobject, ParentGameState, ref ballsadded, ref ballsremove, out removethis);
        }
        public ParticleEmitterBehaviour():this((EmitParticle)null)
        {
            EmitParticleProc = defaultemitter;

        }
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("ParticleType",_ParticleType.Name);
        }
        public override XElement GetXmlData(String pNodeName)
        {
            return new XElement(pNodeName,new XAttribute("ParticleType",_ParticleType.Name));
        }
        public ParticleEmitterBehaviour(XElement Source):this()
        {
            String sParticleType = Source.GetAttributeString("ParticleType","DustParticle");
            _ParticleType = BCBlockGameState.FindClass(sParticleType);
            if (_ParticleType == null) _ParticleType = typeof(DustParticle);
        }
        public ParticleEmitterBehaviour(SerializationInfo info, StreamingContext context):base(info,context)
        {

            _ParticleType = BCBlockGameState.FindClass(_ParticleType.Name);


        }
        public ParticleEmitterBehaviour(EmitParticle emissionRoutine)
        {

            EmitParticleProc = emissionRoutine;

        }

        public override object Clone()
        {
            return new ParticleEmitterBehaviour(EmitParticleProc);
        }
    }
    //a water ball spews out water particles (todo) and makes splash sounds.
    //usually paired with Linear Gravity Ball Behaviour.
    public class WaterBallBehaviour : BaseBehaviour
    {
        public override object Clone()
        {
            return new WaterBallBehaviour();
        }
        public WaterBallBehaviour(XElement Source):base(Source)
        {

        }

        public override XElement GetXmlData(string pNodeName)
        {
            return new XElement(pNodeName);
        }

        public WaterBallBehaviour()
        {

        }
        public override List<Block> PerformFrame(cBall ballobject, BCBlockGameState ParentGameState, ref List<cBall> ballsadded, ref List<cBall> ballsremove, out bool removethis)
        {

            //spew out 3 waterparticles at random velocities (<=2)
            const int WaterBallSpewCount = 1;
            for (int i = 1; i < WaterBallSpewCount; i++)
            {
                //calculate the random velocities.
                float xvel = ((float)BCBlockGameState.rgen.NextDouble() * 2) - 1f;
                float yvel = ((float)BCBlockGameState.rgen.NextDouble() * 2) - 1f;
                //add current ball speeds by a factor of .3.
                xvel+= (ballobject.Velocity.X/3);
                yvel+=(ballobject.Velocity.Y/3); 
                PointF pointvelocity = new PointF(xvel,yvel);
                //create and add the new particle.
               // ParentGameState.Particles.Add(new WaterParticle(ballobject.Location, pointvelocity));
            }


            return base.PerformFrame(ballobject, ParentGameState, ref ballsadded, ref ballsremove,out removethis);
            
        }

        public override string PaddleBallSound()
        {
            return "SPLASH";
        }

    }

    [Serializable]
    [Description("Non-reboundable")]
    public class NonReboundableBallBehaviour : BaseBehaviour
    {

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
        }

        public NonReboundableBallBehaviour(XElement Source)
        {

        }
        public override XElement GetXmlData(String pNodeName)
        {
            return new XElement(pNodeName);
        }
        public NonReboundableBallBehaviour(SerializationInfo info, StreamingContext context)
        {
            //nothing here either...


        }
        public NonReboundableBallBehaviour(NonReboundableBallBehaviour cloneit)
        {
            //nothing to copy...


        }
        public NonReboundableBallBehaviour()
        {


        }

        public override object Clone()
        {
            return new NonReboundableBallBehaviour(this);
        }
    }


    [Serializable]
    [Description("Linear Gravity")]
    public class LinearGravityBallBehaviour : BaseBehaviour
    {
        public double Acceleration { get; set; }
        public override object Clone()
        {
            return new LinearGravityBallBehaviour(this);
        }
        public LinearGravityBallBehaviour()
        {


        }
        public LinearGravityBallBehaviour(XElement Source)
        {
            Acceleration = Source.GetAttributeDouble("Acceleration", 2);
        }
        public override XElement GetXmlData(String pNodeName)
        {
            return new XElement(pNodeName,new XAttribute("Acceleration",Acceleration));
        }
        public LinearGravityBallBehaviour(LinearGravityBallBehaviour clonethis)
        {
            Acceleration = clonethis.Acceleration;


        }

        public LinearGravityBallBehaviour(double pAcceleration)
        {
            Acceleration=Acceleration;

        }
        public LinearGravityBallBehaviour(SerializationInfo info, StreamingContext context):base(info,context)
        {
            Acceleration = info.GetDouble("Acceleration");

        }
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("Acceleration", Acceleration);

        }
        public override List<Block> PerformFrame(cBall ballobject, BCBlockGameState ParentGameState, ref List<cBall> ballsadded, ref List<cBall> ballsremove, out bool removethis)
        {
            //add mAcceleration to the Y velocity of the ball.
            
            ballobject.Velocity = new PointF(ballobject.Velocity.X,(float)(ballobject.Velocity.Y+Acceleration));
            //Debug.Print("GravityBehaviour accelerated ball by " + mAcceleration.ToString() + "resulting speed is " + ballobject.Velocity.ToString());
            return base.PerformFrame(ballobject, ParentGameState, ref ballsadded, ref ballsremove,out removethis);
            
        }

    }
    /// <summary>
    /// Adds a given behaviour and then removes it after a specified gametime has elapsed.
    /// </summary>
    [Description("Timed Behaviour")]
    public class TimedBehaviour : BaseBehaviour
    {
        private TimeSpan _Span = new TimeSpan(0, 0, 0, 1);
        private Type _useBehaviour = null;
        /// <summary>
        /// Behaviour to add (and subsequently remove).
        /// </summary>
        [Editor(typeof(ItemTypeEditor<BaseBehaviour>), typeof(UITypeEditor))]
        public Type useBehaviour { get { return _useBehaviour; } set { _useBehaviour = value; } }
        /// <summary>
        /// Time before this behaviour will be removed.
        /// </summary>
        public TimeSpan Span { get { return _Span; } set { _Span = value; } }
        public TimedBehaviour()
        {
            _useBehaviour = null;
            
        }
        public TimedBehaviour(XElement Source)
        {
            Span = Source.ReadElement("Span", new TimeSpan(0, 0, 0, 1));
            String grabtype = Source.GetAttributeString("useBehaviour");
            useBehaviour = BCBlockGameState.FindClass(grabtype);
        }

        public override XElement GetXmlData(string pName)
        {
            String typeuse = useBehaviour.Name;
            return new XElement(pName,StandardHelper.SaveElement(Span,"Span"),new XAttribute("useBehaviour",typeuse));
        }

        public TimedBehaviour(Type usetype, TimeSpan usespan)
        {
            _Span = usespan;
            _useBehaviour = usetype;


        }
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("TypeName", useBehaviour.Name);
            info.AddValue("Span", _Span);
        }
        public TimedBehaviour(SerializationInfo info, StreamingContext context)
        {
            useBehaviour = BCBlockGameState.FindClass(info.GetString("TypeName"));
            _Span = (TimeSpan)info.GetValue("Span", typeof(TimeSpan));

        }
        public override object Clone()
        {
            return new TimedBehaviour(_useBehaviour, _Span);
        }
        private void delayroutine(object[] parms)
    {
        cBall removefrom = parms[0] as cBall;
        BaseBehaviour removebeh = parms[1] as BaseBehaviour;
        if(removefrom!=null && removebeh!=null)
            if(removefrom.Behaviours.Contains(removebeh))
                removefrom.Behaviours.Remove(removebeh);


    }
   public override void  BehaviourAdded(cBall onBall, BCBlockGameState currstate)
        {
 	            base.BehaviourAdded(onBall, currstate);
                    BaseBehaviour AddedBehaviour = (BaseBehaviour)Activator.CreateInstance(useBehaviour);

                    currstate.NextFrameCalls.Enqueue(new BCBlockGameState.NextFrameStartup(() => onBall.Behaviours.Add(AddedBehaviour)));

                    currstate.DelayInvoke(_Span,delayroutine,new object[]{onBall,AddedBehaviour});

        }
    }


    /// <summary>
    /// LeapFrogExploderBehaviour:
    /// explodes a certain number of times. After that, this behaviour is r emoved and a tempballbehaviour is added.
    /// 
    /// 
    /// 
    /// 
    /// 
    /// </summary>
    [Serializable]
    
    [Description("LeapFrog Exploder")]
    public class LeapFrogExploderBehaviour : BaseBehaviour
    {
        public const int LeapFrogExplosionCount_Default = 3;

        protected int _TimesExploded = 0; //number if times we have exploded.
        protected int _NumExplosions = LeapFrogExplosionCount_Default; //maximum number of explosions before we are "spent"
        protected float _MaxRadius = 16f;

        public int TimesExploded { get { return _TimesExploded; } set { _TimesExploded = value; } }
        public int NumExplosions { get { return _NumExplosions; } set { _NumExplosions = value; } }
        public float MaxRadius { get { return _MaxRadius; } set { _MaxRadius = value; } }
            public LeapFrogExploderBehaviour(LeapFrogExploderBehaviour cloneme)
        {
            _TimesExploded = cloneme.TimesExploded;
            _NumExplosions = cloneme.NumExplosions;
            _MaxRadius = cloneme.NumExplosions;

        }
        public LeapFrogExploderBehaviour()
        {

           


        }
        private float GetExplosionRadius()
        {
            //return percentage that TimesExploded is of NumExplosions.
            return (((float)TimesExploded) / ((float)NumExplosions))*MaxRadius;
        }
        private bool PerformFrameProxyRemove(ProxyObject sourceobject,BCBlockGameState gstate)
        {
            RemoveFromBall.Behaviours.Remove(this);
            RemoveFromBall.Behaviours.Add(new TempBallBehaviour());
            return true;

        }
        private cBall RemoveFromBall;
        public override bool HitBlock(BCBlockGameState currentstate, cBall ballobject, Block blockhit)
        {
            //increment NumExplosions:

            _TimesExploded++;

            float useradius = GetExplosionRadius();
            ExplosionEffect ee = new ExplosionEffect(ballobject.Location, useradius);
            currentstate.GameObjects.AddLast(ee);

            //if we reached the max, replace this behaviour with a temporary ball behaviour.
            if (_TimesExploded == MaxRadius)
            {

                //in order to remove a behaviour from the ball here, we need to set a proxy object...
                RemoveFromBall=ballobject;
                ProxyObject po = new ProxyObject(PerformFrameProxyRemove, null);
                //add it...
                currentstate.GameObjects.AddLast(po);
                
                

            }
            //if we also havea linear gravity behaviour and are going down, make us go up...

            if (ballobject.Behaviours.Any((w) => w.GetType() == typeof(LinearGravityBallBehaviour)))
            {
                ballobject.Velocity = new PointF(ballobject.Velocity.X,-Math.Abs(ballobject.Velocity.Y));

            }


            return base.HitBlock(currentstate, ballobject, blockhit);
        }

        public override XElement GetXmlData(string pName)
        {
            return new XElement(pName,new XAttribute("TimesExploded",_TimesExploded),new XAttribute("NumExplosions",_NumExplosions),new XAttribute("MaxRadius",MaxRadius));
        }
        public LeapFrogExploderBehaviour(XElement Source)
        {
            _TimesExploded = Source.GetAttributeInt("TimesExploded", 0);
            _NumExplosions = Source.GetAttributeInt("NumExplosions", 3);
            _MaxRadius = Source.GetAttributeFloat("MaxRadius", 15);
        }
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("TimesExploded", _TimesExploded);
            info.AddValue("NumExplosions", _NumExplosions);
            info.AddValue("MaxRadius", _MaxRadius);
        }
            public override object Clone()
            {
                return new LeapFrogExploderBehaviour(this);
            }
        public LeapFrogExploderBehaviour(SerializationInfo info, StreamingContext context):base(info,context)
        {
            _TimesExploded = info.GetInt32("TimesExploded");
            _NumExplosions = info.GetInt32("NumExplosions");
            _MaxRadius = info.GetInt32("MaxRadius");

        }



    }



    /// <summary>
    /// pauses a ball for a given amount of time, will show a countdown as well.
    /// </summary>
    [Serializable]
    [StickyNonRemovable]
    [AssignmentNonremovable]
    public class PausedBallBehaviour : BaseBehaviour
    {
        protected TimeSpan TimeLeft {get { return (waitTime -TimeElapsed); }}
            protected TimeSpan TimeElapsed { get { return DateTime.Now - startWaitTime; } }
            public TimeSpan waitTime { get; set; }
        public DateTime startWaitTime{get;set; }
        private bool Waiting=false;
        private PointF _cachedSpeed;
        private cBall ourball;
        private bool doresetspeed = true; //if false, we don't reset from the cached speed.
        private BCBlockGameState ourstate;

        public PointF cachedSpeed
        {
            get { return _cachedSpeed; }
            set {

                if (value.X == 0 && value.Y == 0)
                    Debug.Print("Value passed was 0,0");
                _cachedSpeed = value;
                
            
            }
        }

            public override object Clone()
        {
            return new PausedBallBehaviour(waitTime);
        }
        public PausedBallBehaviour(TimeSpan pwaitTime)
        {
            waitTime = pwaitTime;
            

        }
        public PausedBallBehaviour(TimeSpan pwaitTime, bool nowait):this(pwaitTime)
        {
            Waiting=!nowait;


        }

        public override void BehaviourAdded(cBall onBall, BCBlockGameState currstate)
        {
            base.BehaviourAdded(onBall, currstate);
            
            ourball = onBall;
            ourstate=currstate;
            
        }
        public override void BehaviourRemoved(cBall onBall, BCBlockGameState currstate)
        {
            Debug.Print("PausedBallBehaviour Removed.");
        }
        public void Forcerevert()
        {
        if(ourball!=null)
            ourball.Velocity = cachedSpeed;

        }
        public void SetSpeed()
        {
            if(ourball!=null) cachedSpeed=ourball.Velocity;


        }

        private List<iBallBehaviour> prevbehaviours = new List<iBallBehaviour>();
        public override List<Block> PerformFrame(cBall ballobject, BCBlockGameState ParentGameState, ref List<cBall> ballsadded, ref List<cBall> ballsremove, out bool removethis)
        {
            
            List<Block> returnval = base.PerformFrame(ballobject, ParentGameState, ref ballsadded, ref ballsremove, out removethis);
            if (!Waiting)
            {
                //if we aren't "waiting", then set waiting to true, cache the current speed of the ball, and initialize startWaitTime.
              

                cachedSpeed = ballobject.Velocity;
                ballobject.Velocity = PointF.Empty; //set speed to 0
                Waiting=true;
                startWaitTime=DateTime.Now;
                prevbehaviours=ballobject.Behaviours;
                ballobject.Behaviours = new List<iBallBehaviour>();
                ballobject.Behaviours.Add(this);


            }
            else if(Waiting)
            {
                //if we <ARE> waiting, then check the times.
                if (TimeElapsed > waitTime)
                {

                    //waiting is over!
                   ballobject.Velocity=cachedSpeed;
                   ballobject.Behaviours = prevbehaviours;
                    //and remove ourselves..
                    removethis=true;


                }
            }

            return returnval;
        }
        private static Color defaultfirstcolor = Color.FromArgb(128,Color.Gray);
        private static Color defaultsecondcolor = Color.FromArgb(128,Color.Black);
        //private Brush surroundbrush = new SolidBrush(Color.FromArgb(128, Color.Gray));
        private Brush surroundbrush = new TextureBrush(ImageManager.
            CreateImageFromMatrix<bool>(
            new bool[][]{new bool[]{true,true},new bool[]{false,false}},(a,b,c)=>a?defaultfirstcolor:defaultsecondcolor)); 
        public static TimeSpan GetDefaultTimeout = new TimeSpan(0, 0, 5);

        public override void Draw(Graphics g)
        {
            if (ourball == null) return;
            //base.Draw(g);
            //draw a "darkened" ellipse around the ball; three times the size of the radius.
            GraphicsPath SurroundPath = new GraphicsPath();
            GraphicsPath TextPath = new GraphicsPath();
            



            RectangleF overlayrect = new RectangleF(ourball.Location.X - (ourball.Radius * 3), ourball.Location.Y - (ourball.Radius * 3), ourball.Radius * 6, ourball.Radius * 6);

            SurroundPath.AddEllipse(overlayrect);
            //g.FillEllipse(new SolidBrush(Color.FromArgb(128, Color.Gray)), overlayrect);
            //g.DrawString(TimeElapsed.Seconds.ToString() + "." + TimeElapsed.Milliseconds.ToString().Substring(0, 2),new Font(BCBlockGameState.GetMonospaceFont(),12,FontStyle.Bold,;


            TextPath.AddString(TimeLeft.Seconds.ToString(),new FontFamily(BCBlockGameState.GetMonospaceFont()),(int)FontStyle.Bold,18,new PointF(overlayrect.Left,overlayrect.Top),StringFormat.GenericDefault);
            //g.DrawString(TimeElapsed.Seconds.ToString(), new Font(BCBlockGameState.GetMonospaceFont(), 18,FontStyle.Bold), new SolidBrush(Color.Black), overlayrect);

            var oldsmooth = g.SmoothingMode;
            g.SmoothingMode = SmoothingMode.HighQuality;
            g.FillPath(surroundbrush, SurroundPath);
            g.FillPath(new SolidBrush(Color.Red), TextPath);
            g.DrawPath(new Pen(Color.Black), TextPath);

            g.SmoothingMode = oldsmooth;

        }
        #region Serialization

        public PausedBallBehaviour(XElement Source):base(Source)
        {
            waitTime = Source.ReadElement("waitTime", new TimeSpan(0, 0, 3));
        }
        public override XElement GetXmlData(String pNodeName)
        {
            return new XElement(pNodeName,new XAttribute("waitTime",waitTime));
        }
        public PausedBallBehaviour(SerializationInfo info, StreamingContext context)
        {
            waitTime = (TimeSpan)(info.GetValue("waitTime", typeof(TimeSpan)));
            //startWaitTime = (DateTime)(info.GetValue("startWaitTime", typeof(DateTime)));
            

        }
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("waitTime", waitTime);
           // info.AddValue("startWaitTime", startWaitTime);
        }
        #endregion

    }

    




    [Serializable]
    [StickyNonRemovable]
    [Description("Laser Spin")]
    public class LaserSpinBehaviour : BaseBehaviour
    {
        private int Framecount = 0;
        private TimeSpan _Interval;
        private Pen[] UseLaserPens = DefaultLaserRainbow();
        private LaserShot.LaserShotFrameFunction _EventFunction;

        public LaserShot.LaserShotFrameFunction EventFunction
        {
            get { return _EventFunction; }
            set { _EventFunction = value; }
        }

        private static Pen[] DefaultLaserRainbow()
        {

            return new Pen[] {new Pen(Color.Red,3),
                new Pen(Color.OrangeRed,3),
                new Pen(Color.Orange,3),
            new Pen(Color.Yellow,3),
            new Pen(Color.YellowGreen,3),
            new Pen(Color.Green,3),
            new Pen(Color.Blue,3),
            new Pen(Color.Indigo,3),
            new Pen(Color.Violet,3)};

        }
        private Pen ChooseLaserPen()
        {

            int useindex = BCBlockGameState.rgen.Next(0, UseLaserPens.Length);
            return UseLaserPens[useindex];



        }

        public TimeSpan Interval
        {
            get { return _Interval; }
            set { _Interval = value; }
        }
        
        public override object Clone()
        {
            return new LaserSpinBehaviour();
        }
        public LaserSpinBehaviour():this(new TimeSpan(0,0,0,0,100))
        {


        }
        public LaserSpinBehaviour(TimeSpan CoolDownTime)
        {
            _Interval = CoolDownTime;

        }
        public LaserSpinBehaviour(XElement Source):base(Source)
        {

        }
        public override XElement GetXmlData(String pNodeName)
        {
            return new XElement(pNodeName);
        }
        public LaserSpinBehaviour(SerializationInfo info, StreamingContext context)
        {


        }
        
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            //
        }
        DateTime lastfire = DateTime.Now;
        

        public override List<Block> PerformFrame(cBall ballobject, BCBlockGameState ParentGameState, ref List<cBall> ballsadded, ref List<cBall> ballsremove, out bool removethis)
        {

            if ((DateTime.Now - lastfire) > _Interval)
            {

                BCBlockGameState.Soundman.PlaySound("ELECTRICLASER");
                LaserShot makeshot = new LaserShot(ballobject.Location, (float)(Math.PI * 2 * BCBlockGameState.rgen.NextDouble()), 4, 9, ChooseLaserPen());
                //hook the event, if desired.
                makeshot.Weak = true;
                if (_EventFunction != null)
                    makeshot.LaserShotFrame += _EventFunction;
                ParentGameState.GameObjects.AddLast(makeshot);
                

                lastfire = DateTime.Now;
            }


            
            return base.PerformFrame(ballobject, ParentGameState, ref ballsadded, ref ballsremove, out removethis);
        }
    }





    [Serializable]
    [StickyNonRemovable]
    public class CrazyBallBehaviour : BaseBehaviour
    {
        private class AfterImage
        {
            int _currentAlpha = 255;
            public int currentAlpha
            {
                get
                {
                    if (_currentAlpha > 255) _currentAlpha = 255;
                    if (_currentAlpha < 0) _currentAlpha = 0;
                    return _currentAlpha;
                }
                set { _currentAlpha = value; }
            }
                RectangleF ourrect;

            public AfterImage(RectangleF userect)
            {
                ourrect = userect;


            }
            public PointF Location
            {
                get { return new PointF(ourrect.Left + (ourrect.Width / 2), ourrect.Top + (ourrect.Height / 2)); }
                set {
                    ourrect = new RectangleF(value.X-(ourrect.Width/2),value.Y-(ourrect.Height/2),ourrect.Width,ourrect.Height);
                
                
                }


            }
                public bool PerformFrame()
            {
                currentAlpha-=20;
                return currentAlpha <= 0;


            }
                public Color GetColour()
                {
                    return Color.FromArgb(currentAlpha, Color.Black);

                }

            public void Draw(Graphics g)
            {
                if(currentAlpha>0)
                    g.FillEllipse(new SolidBrush(this.GetColour()), ourrect);


            }



        }
        private WeakReference OwnerBall;
        private const int TimeToLive = 500; //5000 frames...
        private int LiveTime = 0;
        private Queue<AfterImage> AfterImages= new Queue<AfterImage>();
        public override object Clone()
        {
            return new CrazyBallBehaviour();
        }
        public CrazyBallBehaviour()
        {


        }
        public CrazyBallBehaviour(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            

        }
        public CrazyBallBehaviour(XElement Source):base(Source)
        {

        }
        public override XElement GetXmlData(String pNodeName)
        {
            return new XElement(pNodeName);
        }
        public override void BehaviourAdded(cBall onBall, BCBlockGameState currstate)
        {
            onBall.Velocity = new PointF(onBall.Velocity.X*2,onBall.Velocity.Y*2);
            OwnerBall = new WeakReference(onBall);
        }
        public override List<Block> PerformFrame(cBall ballobject, BCBlockGameState ParentGameState, ref List<cBall> ballsadded, ref List<cBall> ballsremove, out bool removethis)
        {
            removethis=false;
            LiveTime++;
            //List<AfterImage> removeimages=new List<AfterImage>();
            foreach(AfterImage loopafterimage in AfterImages)
            {
                loopafterimage.PerformFrame();
            


            }
            
            //if ((LiveTime % 3)==0)
            //{
                //add another "image" of the ball. we just store location and radius; the drawing is handled, well, by the draw override, obviously
                //remove the last one, but only if the count is over 15.
                if (AfterImages.Count >= 15) AfterImages.Dequeue();


                AfterImages.Enqueue(new AfterImage(ballobject.getRect()));
                //tada....


            //}
                if (LiveTime >= TimeToLive)
                {
                    removethis=true;


                }


            bool notreturned=false;
            return base.PerformFrame(ballobject, ParentGameState, ref ballsadded, ref ballsremove,out notreturned);
            //return base.PerformFrame(ballobject, ParentGameState, ref ballsadded, ref ballsremove);
        }
        public override HitWallReturnConstants HitWall(BCBlockGameState currentstate, cBall ballobject)
        {
            //return true to destroy the ball.
            return HitWallReturnConstants.HitBall_Preserve; 
            //return base.HitWall(currentstate, ballobject);
        }
        private Pen GetAfterImagePen(AfterImage foraimage)
        {
            Pen AfterImagePen = new Pen(foraimage.GetColour(), ((cBall)(OwnerBall.Target)).Radius * 2);
            return AfterImagePen;
        }

        public override void Draw(Graphics g)
        {
            AfterImage previousimage=null;
            
            foreach (AfterImage loopafter in AfterImages)
            {
                loopafter.Draw(g);
                if (previousimage != null)
                {
                    Pen usepen = GetAfterImagePen(loopafter);
                    g.DrawLine(usepen, previousimage.Location, loopafter.Location);


                }
                previousimage=loopafter;
            }


            base.Draw(g);
        }
        


    }


    [Serializable]
    public class PowerBallBehaviour : BaseBehaviour 
    {

        public override object Clone()
        {
            return new PowerBallBehaviour();
        }

        #region iBallBehaviour Members
        public PowerBallBehaviour()
        {
        }

        public PowerBallBehaviour(XElement Source):base(Source)
        {

        }

        public override XElement GetXmlData(string pName)
        {
            return new XElement(pName);
        }

        public PowerBallBehaviour(SerializationInfo info, StreamingContext context):base(info,context)
        {
            //nothing...


        }
        
        public override bool HitBlock(BCBlockGameState currentstate,cBall ballobject, Block blockhit)
        {
            
            return !(blockhit.MustDestroy());
        }

       #endregion

    }
    [Serializable] 
    [AssignmentNonremovableAttribute]
    public class TempBallBehaviour : BaseBehaviour
    {
        private int _NumBounces = 1; //number of bounces left.

        public int NumBounces { get { return _NumBounces; } set { _NumBounces = value; } }

        public override object Clone()
        {
            return new TempBallBehaviour();
        }
        public TempBallBehaviour()
        {


        }
        public TempBallBehaviour(XElement Source)
        {

        }

        public override XElement GetXmlData(string pName)
        {
            return new XElement(pName);
        }

        public TempBallBehaviour(int numBounces)
        {
            _NumBounces = numBounces;
        }
        public TempBallBehaviour(SerializationInfo info, StreamingContext context):base(info,context)
        {


        }

        public override HitWallReturnConstants HitWall(BCBlockGameState currentstate, cBall ballobject)
        {
            _NumBounces--;
            if(_NumBounces <= 0) return HitWallReturnConstants.HitBall_Destroy; //is destroyed regardless.
            return HitWallReturnConstants.HitBall_Default; 
        }
        public override bool HitBlock(BCBlockGameState currentstate, cBall ballobject, Block blockhit)
        {
            //currentstate.Balls.Remove(ballobject);
            return true;
        }
        public override List<Block> PerformFrame(cBall ballobject, BCBlockGameState ParentGameState, ref List<cBall> ballsadded, ref List<cBall> ballsremove, out bool removethis)
        {
            //if (ballobject.Behaviours.Count <= 2)
            //{   //only if we are the only behaviour...
                for (int i = 0; i < (int) (7*BCBlockGameState.ParticleGenerationFactor); i++)
                {
                    ParentGameState.Particles.Add(new FireParticle(ballobject));

                }
            //}
            return base.PerformFrame(ballobject, ParentGameState, ref ballsadded, ref ballsremove,out removethis);
        }


    }
    //for the temp behaviour:

    //   for(int i =0;i < (int)(10 * BCBlockGameState.ParticleGenerationFactor);i++)
    //                {
    //                    wholeref.Particles.Add(new FireParticle(this));

    //                }



    #endregion

    public class AssignmentNonremovableAttribute : Attribute
    {
        



    }
    
    public class FrustratorBall : cBall
    {


        public FrustratorBall()
        {
            base.DrawColor = Color.Blue;

        }
        public FrustratorBall(FrustratorBall clonethis)
            : base(clonethis)
        {


        }
        public FrustratorBall(PointF pLocation, PointF pVelocity)
            : base(pLocation, pVelocity)
        {
            


        }
        public FrustratorBall(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {


        }
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);

        }
        public override bool HitBlock(BCBlockGameState currentstate, Block BlockHit)
        {
            return false; //we cannot hit blocks.
        }
        public override void Draw(Graphics g)
        {
            
            Image gotimage = BCBlockGameState.GetSphereImage(Color.Blue);
            gotimage = BCBlockGameState.FadeImage(gotimage, null, 128);
            g.DrawImage(gotimage, Location.X - 10, Location.Y - 10, 20, 20);
            base.Draw(g);
            
        }
        protected override List<Block> CheckBlocks(BCBlockGameState ParentGameState, ref List<cBall> ballsadded, BCBlockGameState wholeref)
        {
            var RespawnCandidates = FrustratorBlock.getRespawnCandidates(ParentGameState);
            //check for collisions
            foreach (var iterateblock in RespawnCandidates)
            {

                if (iterateblock.DoesBallTouch(this))
                {
                    //revive it...
                    Block respawnblock = (Block)iterateblock.Clone();
                    ParentGameState.Blocks.AddLast(respawnblock);
                    //TODO: funky effect when respawning block.
                    FrustratorBlock.spawnpoof(ParentGameState, respawnblock);
                    ParentGameState.Forcerefresh = true; //to redraw the new block...
                    //bounce the ball now, as normal.
                    Block.BallRelativeConstants brc = iterateblock.getBallRelative(this);
                    //move us appropriately; and bounce off. We don't actually fire the blockhit event of blocks.
                    if ((brc & Block.BallRelativeConstants.Relative_Left) == Block.BallRelativeConstants.Relative_Left)
                    {
                        Location = new PointF(iterateblock.BlockRectangle.Left - Radius, Location.Y);
                        Velocity = new PointF(-Math.Abs(Velocity.X), Velocity.Y);

                    }
                    else if ((brc & Block.BallRelativeConstants.Relative_Right) == Block.BallRelativeConstants.Relative_Right)
                    {
                        Location = new PointF(iterateblock.BlockRectangle.Right + Radius, Location.Y);
                        Velocity = new PointF(Math.Abs(Velocity.X), Velocity.Y);
                    }
                    if ((brc & Block.BallRelativeConstants.Relative_Up) == Block.BallRelativeConstants.Relative_Up)
                    {
                        Location = new PointF(Location.X, iterateblock.BlockRectangle.Top - Radius);
                        Velocity = new PointF(Velocity.X, -Math.Abs(Velocity.Y));


                    }
                    else if ((brc & Block.BallRelativeConstants.Relative_Down) == Block.BallRelativeConstants.Relative_Down)
                    {
                        Location = new PointF(Location.X, iterateblock.BlockRectangle.Bottom + Radius);
                        Velocity = new PointF(Velocity.X, Math.Abs(Velocity.Y));


                    }
                }


            }
            return new List<Block>();
        }
        int mm = 0;
        int usehue = 0;
        public override List<Block> PerformFrame(BCBlockGameState ParentGameState, ref List<cBall> ballsadded, ref List<cBall> ballsremove)
        {
            /*
            PreviousVelocity = Velocity;
            //returns the blocks that were affected by the frame (changed or moved).
            PointF NextPosition = new PointF(Location.X + Velocity.X, Location.Y + Velocity.Y);
            RectangleF NextPositionBorders = new RectangleF(NextPosition.X - Radius, NextPosition.Y - Radius, Radius, Radius);
            NormalizeSpeed();
            if (ballsadded == null) ballsadded = new List<cBall>();
            RectangleF rectangleborders = new RectangleF(Location.X - Radius, Location.Y - Radius, Radius, Radius);
            PrevLocation = Location;
            Location = NextPosition;
            bool doemit=false;
            CheckWallCollision(ParentGameState.GameArea, getRect(), ref doemit);
            if (ParentGameState.PlayerPaddle != null)
                ParentGameState.PlayerPaddle.CheckImpact(ParentGameState, this,false);
            //FrustratorBalls only hit Destroyed Blocks.
             * */


            mm = ((mm + 1) % 2);
            usehue += 2;
            usehue %= 240;
            //spawn a lightorb in a random colour.
            if (mm == 0)
            {
                Color drawcolor = new HSLColor((float)usehue, 240, 120);
                LightOrb lo = new LightOrb(Location, drawcolor, 12);
                lo.TTL = 30;
                ParentGameState.Particles.Add(lo);
            }
            return base.PerformFrame(ParentGameState, ref ballsadded, ref ballsremove);
            


        }


    }
    /// <summary>
    /// Exactly the same as cBall, but always has a temporary ball behaviour.
    /// </summary>
    [Serializable]
    public class ProjectileBall : cBall
    {
        public ProjectileBall(ProjectileBall clonethis)
            : base(clonethis)
        {
            if (!Behaviours.Any((w) => w is TempBallBehaviour))
                Behaviours.Add(new TempBallBehaviour());
        }
        public ProjectileBall(PointF pLocation, PointF pVelocity)
            : base(pLocation, pVelocity)
        {
            Behaviours.Add(new TempBallBehaviour());

        }
        public ProjectileBall()
        {
            Behaviours.Add(new TempBallBehaviour());
            this.DrawColor = Color.Yellow;
            Radius = 2;
        }
        public ProjectileBall(SerializationInfo info, StreamingContext context):base(info,context)
        {

            if (!Behaviours.Any((w) => w is TempBallBehaviour))
                Behaviours.Add(new TempBallBehaviour());

        }
        public override object Clone()
        {
            return new ProjectileBall(this);
        }

    }

    [Serializable]
    public class cBall: ExpandableObjectConverter, ICloneable,ISerializable,iLocatable ,iProjectile ,IExplodable,IXmlPersistable
    {
        internal struct TrailItemData
        {
            public PointF Location;
            public float Radius;
            public TrailItemData(PointF pLocation, float pRadius)
            {
                Location = pLocation;
                Radius = pRadius;


            }
        }
        public PointF PreviousVelocity = new PointF(0, 0);
        private int _Trails = 1;
        private bool _Visible = true;
        public bool Visible { get { return _Visible; } set { _Visible = value; } }
        internal Queue<TrailItemData> TrailData = new Queue<TrailItemData>();
        public int Trails { get { return _Trails; } set { _Trails = value; } }

       // private WeakReference ParentGameState; //holds a weak reference to the parent "BCBlockGameState" object that created it.
        //Important: this weakreference will be Null in the editor (probably)
        public bool isTempBall
        {
            get {
                return ((from q in Behaviours where q is TempBallBehaviour select q).Count() > 0);
                
            }
            set {
                if (!isTempBall)
                    Behaviours.Add(new TempBallBehaviour());
            }
        }
        public float Mass = 3.0f;
        private int mnumImpacts = 0;
        public delegate void ballimpactproc(cBall ballimpact);
        public event ballimpactproc BeforeBallImpact;
        public event ballimpactproc BallImpact;
        public Particle emitparticletemplate=null;
        //DrawHandlers: default to one: the standard handler.
       public List<iBallDrawHandler> DrawHandlers = new List<iBallDrawHandler>() { new BallDrawHandler_Standard()};  
        public List<iBallBehaviour> Behaviours = new List<iBallBehaviour>();
//        public ObservableCollection<iBallBehaviour> Behaviours = new ObservableCollection<iBallBehaviour>();
        /// <summary>
        /// given a Type, determines if any of the behaviours of this ball (iBallBehaviours) is that type.
        /// </summary>
        /// <param name="behaviourtype">Type of behaviour to check for.</param>
        /// <returns></returns>
        public bool hasBehaviour(Type behaviourtype)
        {
            foreach (iBallBehaviour loopbehaviour in Behaviours)
            {
                if(loopbehaviour.GetType()==behaviourtype)
                    return true;



            }
            
            return false;

        }
        public static void CollideBalls(cBall BallA, cBall BallB)
        {
            double RatioA = BallB.Mass / (BallA.Mass + BallB.Mass);
            double RatioB = BallA.Mass / (BallA.Mass + BallB.Mass);
            
            //Get the velocities of each ball.
            PointF VelR = new PointF(BallB.Velocity.X-BallA.Velocity.X,BallB.Velocity.Y-BallA.Velocity.Y);
            //I = (1+e)*N*(Vr • N)

//Va = -I*(Mb / (Ma+Mb))
//Vb = +I*(Ma / (Ma+Mb))

            //angle between the two balls.
            double angle = BCBlockGameState.GetAngle(BallA.Location,BallB.Location);
            //create a vector of magnitude 1 of that angle.
            PointF N = new PointF((float)Math.Cos(angle),(float)Math.Sin(angle));


         



        }

        /*
        public bool isTempBall
        {
            get {
                foreach (iBallBehaviour loopbeh in Behaviours)
                    if (loopbeh.GetType() == typeof(TempBallBehaviour)) return true;
            
                return false;
            }
            set {
                if (isTempBall == value)
                    return; //no processing if newvalue==oldvalue.
                //if we are a temp ball and the new value says we aren't...
                if (isTempBall && (!value))
                {
                    List<iBallBehaviour> removethese= new List<iBallBehaviour>();
                    foreach (iBallBehaviour loopbeh in Behaviours)
                    {
                        if (loopbeh.GetType() == typeof (TempBallBehaviour))
                            removethese.Add(loopbeh);
                    }


                    foreach (iBallBehaviour removeit in removethese)
                    {
                        Behaviours.Remove(removeit);


                    }



                }
                else if (!isTempBall && value)
                {
                    Behaviours.Add(new TempBallBehaviour());



                }



            }
        */
        
        #region ballbehaviour invocations
        private bool InvokeHitBlock(BCBlockGameState currentstate, cBall ballobject, Block blockhit)
        {
            /*
            bool anyfalse = false;
            foreach (iBallBehaviour loopbeh in Behaviours)
            {
                if (!loopbeh.HitBlock(currentstate, this, blockhit))
                    anyfalse = true;


            }
            if (anyfalse) return false; else return true;    
             * */
            return HitBlock(currentstate, blockhit);
        }
        public delegate List<Block> BallPerformFrameRoutine(cBall ballobject, BCBlockGameState ParentGameState, ref List<cBall> ballsadded, ref List<cBall> ballsremove);
        public event BallPerformFrameRoutine FrameEvent;


        //calls the FrameEvents, coalesces any affected blocks, and returns that result.
        private List<Block> InvokeFrameRoutine(cBall ballobject, BCBlockGameState ParentGameState, ref List<cBall> ballsadded, ref List<cBall> ballsremove)
        {
            List<Block> buildlisting= new List<Block>();
            var copied = FrameEvent;
            if (copied == null) return new List<Block>();
            foreach (BallPerformFrameRoutine iterate in FrameEvent.GetInvocationList())
            {
                List<Block> currentresult = iterate(ballobject, ParentGameState, ref ballsadded, ref ballsremove);
                buildlisting.AddRange(currentresult);

            }
            return buildlisting;
        }

        private List<Block> InvokePerformFrame(cBall ballobject, BCBlockGameState ParentGameState, ref List<cBall> ballsadded,ref List<cBall> ballsremove)
        {
            List<Block> listaggregate = InvokeFrameRoutine(ballobject, ParentGameState, ref ballsadded, ref ballsremove);
            List<iBallBehaviour> removethese = new List<iBallBehaviour>();
           
            foreach (iBallBehaviour loopbeh in Behaviours)
            {
             
                bool removethis=false;
                List<Block> tempreturn = loopbeh.PerformFrame(ballobject, ParentGameState,ref ballsadded,ref ballsremove,out removethis);
                if (removethis)
                {
                    removethese.Add(loopbeh);


                }
                if (tempreturn != null)
                    listaggregate.AddRange(tempreturn);


            }
            foreach (iBallBehaviour removeBehaviour in removethese)
            {
                Behaviours.Remove(removeBehaviour);

            }
            return listaggregate;
        }

        private HitWallReturnConstants invokeHitWall(BCBlockGameState currentstate, cBall ballobject)
        {
            

            //troublesome; some behaviours will want to preserve, others will want to destroy the ball.
            //the behaviour here is a Preserve return value adds one to our value, a destroy subtracts one, and we return
            //default if the result is 0, to preserve if it's over 0, and to destroy if it's less than 0.

            //
            int runningtotal = 0;

            foreach (iBallBehaviour loopbeh in Behaviours)
            {
                switch (loopbeh.HitWall(currentstate, this))
                {
                    case HitWallReturnConstants.HitBall_Destroy:
                        runningtotal--;
                        break;
                    case HitWallReturnConstants.HitBall_Preserve:
                        runningtotal++;
                        break;
                }


            }
            if (runningtotal > 0) return HitWallReturnConstants.HitBall_Preserve;
            if (runningtotal < 0) return HitWallReturnConstants.HitBall_Destroy;
            return HitWallReturnConstants.HitBall_Default;

        } //return true to destroy this ball...

        private void invokeDraw(Graphics g)
        {
            foreach (iBallBehaviour loopbeh in Behaviours)
                loopbeh.Draw(g);

        }

        #endregion 

        public static PointF getRandomVelocity(float magnitude)
        {
            //choose a random angle..

            double randomangle = (2 * Math.PI)*BCBlockGameState.rgen.NextDouble();
            //create a new vector out of this angle of the given magnitude...
            return new PointF((float)(Math.Sin(randomangle) * magnitude),(float)(Math.Cos(randomangle)*magnitude));





        }
        public RectangleF getRect()
        {
            return new RectangleF(Location.X-Radius,Location.Y-Radius,Radius*2,Radius*2);


        }

        public String getSpecificSound(Block hitblock)
        {
            //used by derived "Ball" classes to change the default sounds of a block, or something similar.
            return "";

        }
        /// <summary>
        /// called by block implementation. return true to allow for standard block-based velocity changes; false otherwise.
        /// </summary>
        /// <param name="BlockHit"></param>
        /// <returns></returns>
        public virtual bool HitBlock(BCBlockGameState currentstate,Block BlockHit)
        {
            bool anyfalse=false;
            foreach (iBallBehaviour loopbeh in Behaviours)
            {
                if(!loopbeh.HitBlock(currentstate, this, BlockHit))
                    anyfalse=true;


            }
            if(anyfalse) return false; else return true;    

        }

        public void invokeballimpact(cBall withball)
        {
            bool doignore = false;
            ballimpactproc tempcopy = BallImpact;
            if (tempcopy != null)
                tempcopy.Invoke(withball);



            
        }
        public void invokebeforeblockimpact(cBall withball)
        {
            ballimpactproc tempcopy = BeforeBallImpact;
            if (tempcopy != null) tempcopy.Invoke(withball);


        }

        public bool hasBallImpact()
        {

            return BallImpact != null;

        }

        public int numImpacts
        {
            get { return mnumImpacts; }

            
            set
            {
                mnumImpacts=value;
                invokeballimpact(this);

            }

        }//number if times this ball has hit a block or wall.
        internal const int NumPolyPoints = 8;
        private float _Radius;
        public float Radius { 
            get {return _Radius;}
            set {
                _Radius=value;
                regenpoints();
            }

        }
        private List<PointF> PolyPoints = new List<PointF>();
        private Polygon BallPoly=null;
        private void regenpoints()
        {
            PolyPoints = new List<PointF>();
            double currangle = 0, angleincrement = Math.PI / (NumPolyPoints * 2);

            for (int i = 0; i < NumPolyPoints; i++)
            {
                PolyPoints.Add(new PointF((float)(Math.Cos(currangle)*Radius),(float)(Math.Sin(currangle)*Radius)));


                currangle+=angleincrement;


            }
            BallPoly = new Polygon(PolyPoints.ToArray());
           //done...

        }
        public List<PointF> getPolyPoints()
        {
            if (PolyPoints == null || PolyPoints.Count == 0) regenpoints();
            return PolyPoints;
            


        }
        public Polygon GetBallPoly()
        {
            Polygon cloned = (Polygon)BallPoly.Clone();


            cloned.Offset(Location.X, Location.Y);





            return cloned;
        }

        [TypeConverter(typeof(FloatFConverter))]
        public PointF Location { get; set; }
        [TypeConverter(typeof(FloatFConverter))]
        public PointF Velocity { get; set; }

        public double TotalSpeed
        {
            get { return Math.Sqrt(Velocity.X * Velocity.X + Velocity.Y * Velocity.Y); }


        }

            [Browsable(false)]
        public PointF PrevLocation { get;  set; }
        [Browsable(false)]
        internal Brush DrawBrush = new SolidBrush(Color.Red);

        internal Color _DrawColor= Color.Red;

        public Color DrawColor { get { return _DrawColor; }
            set {
            _DrawColor=value;
            DrawBrush = new SolidBrush(_DrawColor);
            
            }
        }
        [Browsable(false)]
        internal Pen DrawPen = new Pen(Color.Black);
        public float getMagnitude()
        {
            return (float)Math.Sqrt(Math.Pow(Velocity.X, 2) + Math.Pow(Velocity.Y, 2));



        }

        public cBall()
        {
            Radius = 3;
            Behaviours.Add(new BlackHoleBlock.GravityBlockBallBehaviour(2));
            //Behaviours.Add(new CrazyBallBehaviour());
            DrawColor=Color.Red;
            
        }
        public cBall(cBall clonethis)
        {
            Radius = clonethis.Radius;
            Location = clonethis.Location;
            Velocity = clonethis.Velocity;
            DrawBrush = (Brush)clonethis.DrawBrush.Clone();
            DrawPen = (Pen)clonethis.DrawPen.Clone();
            //Behaviours = clonethis.Behaviours;
            //Don't Set Behaviours directly. go through each.
            foreach (var loopbehave in clonethis.Behaviours)
            {
                Behaviours.Add(loopbehave);


            }


            DrawColor = clonethis.DrawColor;


        }

        //add more constructors for defining location, radius, speed, etc.
        
        public cBall(PointF pLocation, PointF pVelocity):this()
        {
            Location =pLocation;
            Velocity = pVelocity;


        }
        public cBall(SerializationInfo info, StreamingContext context)
        {

            DrawColor = (Color)info.GetValue("Color", typeof(Color));
            DrawBrush = new SolidBrush(DrawColor);
            /*
             *             info.AddValue("Radius", Radius);
            info.AddValue("Location", Location);
            info.AddValue("Velocity", Velocity);
            info.AddValue("Behaviours", Behaviours);*/
            Radius = info.GetInt32("Radius");
            Location = (PointF)info.GetValue("Location", typeof(PointF));
            Velocity = (PointF)info.GetValue("Velocity", typeof(PointF));
            Behaviours = (List<iBallBehaviour>)info.GetValue("Behaviours", typeof(List<iBallBehaviour>));
            try { Visible = info.GetBoolean("Visible"); }
            catch { Visible = true; }
        }


        internal RectangleF getballrect()
        {
            return new RectangleF(Location.X-Radius,Location.Y-Radius,Radius*2,Radius*2);



        }

        public virtual void Draw(Graphics g)
        {
            if (!Visible) return; //if invisible, no drawing occurs at all.
            RectangleF ballrect = getballrect();
            //call the draw handlers...
            foreach(iBallDrawHandler dh in DrawHandlers)
            {

                dh.Draw(this,g);

            }

            //also any "special" drawing by behaviours. This might seem redundant but having them separate makes sure that
            //draws by behaviours (such as the pausedballbehaviour) always appear on top of graphics drawn by the DrawHandlers. 


            foreach (iBallBehaviour loopbehave in Behaviours)
            {
                loopbehave.Draw(g);


            }
            

        }
        protected void NormalizeSpeed()
        {
            if (getMagnitude() > 25)
            {
                double useangle = Block.GetAngle(new PointF(0, 0), Velocity);
                Velocity = new PointF((float)Math.Cos(useangle) * 20, (float)Math.Sin(useangle) * 20);
 


            }



        }

        public virtual List<Block> PerformFrame(BCBlockGameState ParentGameState,ref List<cBall> ballsadded,ref List<cBall> ballsremove)
        {
            PreviousVelocity = Velocity;
            //returns the blocks that were affected by the frame (changed or moved).
            //PointF NextPosition = new PointF(Location.X + Velocity.X, Location.Y + Velocity.Y);
            PointF NextPosition = Location;
            BCBlockGameState.IncrementLocation(ParentGameState, ref NextPosition, Velocity);
            //RectangleF NextPositionBorders = new RectangleF(NextPosition.X - Radius, NextPosition.Y - Radius, Radius, Radius);
            NormalizeSpeed();
            if(ballsadded==null)            ballsadded = new List<cBall>();
            RectangleF rectangleborders = new RectangleF(Location.X-Radius,Location.Y-Radius,Radius*2,Radius*2);
            PrevLocation=Location;
            //Location = new PointF(Location.X+Velocity.X,Location.Y+Velocity.Y);
            Location = NextPosition;

            TrailData.Enqueue(new TrailItemData(Location,Radius));
            while (TrailData.Count > _Trails)
                TrailData.Dequeue();
            
            BCBlockGameState wholeref = ParentGameState;

            List<Block> affectedblocks = InvokePerformFrame(this, ParentGameState, ref  ballsadded,ref ballsremove);

            if(affectedblocks==null) affectedblocks = new List<Block>();
            if (wholeref != null)
            {

                /*if (isTempBall)
                {

                    for (int i = 0; i < (int)(10 * BCBlockGameState.ParticleGenerationFactor); i++)
                    {
                        ParentGameState.Particles.Add(new FireParticle(this));

                    }

                }*/



                //check for collision with the walls...
                Rectangle clientrect = wholeref.GameArea;
                bool DoEmitSound=false;
                bool didhit=false;
                didhit = CheckWallCollision(rectangleborders, clientrect, ref DoEmitSound);


                //CheckBall.Y + CheckBall.radius >= Paddle.Y And CheckBall.Y - _
                //CheckBall.radius < Paddle.Y + Paddle.PHeight Or (((CheckBall.Y - _
                //CheckBall.Yspeed + CheckBall.radius) >= Paddle.Y And CheckBall.Y - _
                //CheckBall.Yspeed - CheckBall.radius < Paddle.Y + Paddle.PHeight) And _
                //Sgn(CheckBall.Yspeed) = 1)
                if (wholeref.PlayerPaddle != null)
                {
                    /*
                    if (rectangleborders.IntersectsWith(wholeref.PlayerPaddle.Getrect()) && Math.Sign(Velocity.Y) == 1)
                        // Paddle paddle = wholeref.PlayerPaddle;
                        //if((Location.Y + Radius >= paddle.Position.Y) && (Location.Y - 
                        //    Radius < paddle.Position.X+paddle.PaddleSize.Width  ) || (((Location.Y - Velocity.Y + Radius) >= paddle.Position.Y &&
                        //    Location.Y - Velocity.Y - Radius < paddle.Position.Y+paddle.PaddleSize.Height) && Math.Sign(Velocity.Y)==1))

                        // if (Location.Y + Radius >= paddle.Position.Y && (Location.Y - Radius <= (paddle.Position.X + paddle.PaddleSize.Width)))
                    {
                        //the X coordinate checks out here...


                        {

                            wholeref.PlayerPaddle.Impact(wholeref, this);

                        }
                    }
                     * */
                    wholeref.PlayerPaddle.CheckImpact(wholeref, this, true);


                }
                bool HitBottom=false;
                if (rectangleborders.Bottom > clientrect.Bottom)
                {

                    didhit = true;
                    HitBottom=true;
                    DoEmitSound=false;

                }
                if (didhit)
                {
                    numImpacts++;
                    switch (invokeHitWall(wholeref, this))
                    {
                        case HitWallReturnConstants.HitBall_Default:
                            if (HitBottom)
                            {
                                ParentGameState.invokeballhitbottom(this);
                                ParentGameState.Defer(() => ParentGameState.Balls.Remove(this));
                            }
                            //
                            
                            break;
                        case HitWallReturnConstants.HitBall_Destroy:


                            ParentGameState.invokeballhitbottom(this);
                            ParentGameState.Defer(() => ParentGameState.Balls.Remove(this));
                            break;
                        case HitWallReturnConstants.HitBall_Preserve:
                            //the ball is preserved...
                            if (HitBottom)
                            {
                                Velocity = new PointF(Velocity.X,-Velocity.Y);
                                Location = new PointF(Location.X,ParentGameState.GameArea.Bottom-Radius);
                            }
                            break;




                    }
                }






                // BCBlockGameState.Soundman.PlaySound("bounce", 0.9f);
                    if(DoEmitSound) BCBlockGameState.Soundman.PlaySound(GetWallHitSound(), 0.9f);
                



                affectedblocks.AddRange(CheckBlocks(ParentGameState, ref ballsadded,  wholeref));
            }
          




            return affectedblocks;
        }

        protected virtual List<Block> CheckBlocks(BCBlockGameState ParentGameState, ref List<cBall> ballsadded, 
                                           BCBlockGameState wholeref)
        {
            List<Block> affectedblocks = new List<Block>();
//if (HitBottom) ParentGameState.BallHitBottom(this);
            List<Block> destroyed = new List<Block>();
            foreach (Block loopblock in wholeref.Blocks)
            {
                bool was_hit = false;
                bool returnimpact = false;
                //if (isTempBall) 
                if ((returnimpact = loopblock.CheckImpact(wholeref, this, out was_hit, ref ballsadded)))
                {
                    Velocity = new PointF(Velocity.X + 0.01f, Velocity.Y + 0.01f);
                    InvokeHitBlock(wholeref, this, loopblock);
                }
                if (was_hit || returnimpact)
                {
                    affectedblocks.Add(loopblock);
                    if (isTempBall) ParentGameState.RemoveBalls.Add(this);
                }

                if (was_hit)
                {
                    Debug.Print("destroying block");
                    destroyed.Add(loopblock);
                }
            }

            lock (wholeref.Blocks)
            {
                foreach (Block removeblock in destroyed)
                {
                    bool nodef = false;
                    //removeblock.RaiseBlockDestroy(ParentGameState, this, ref ballsadded, ref nodef);
                    wholeref.Blocks.Remove(removeblock);
                    affectedblocks.Add(removeblock);
                }
            }
            //  foreach (cBall removeball in ParentGameState.removeballs)
            //  {
            //      ParentGameState.Balls.Remove(removeball);


            //}
            //ParentGameState.removeballs.Clear();
            //check of we destroyed the last destructible block...
            if (destroyed.Count > 0)
            {
                //if (wholeref.Blocks.Count((q)=>q.MustDestroy()) == 0)
                if (wholeref.Blocks.Count((x) => x.MustDestroy()) == 0)
                {
                    wholeref.invokeLevelComplete();
                }
            }
            return affectedblocks;
        }

        private static double[] SineTable = null;
        private static double[] CosTable = null;


        internal PointF[] getCollisionPointOffsets()
        {
            const int numcollisionpoints = 6;
            float Anglebetween = (float)((Math.PI * 2) / numcollisionpoints);
            PointF[] offsets = new PointF[numcollisionpoints];
            if (CosTable == null)
            {
                float angleaccum = 0;
                CosTable = new double[numcollisionpoints];
                SineTable = new double[numcollisionpoints];
                for (int i = 0; i < numcollisionpoints; i++)
                {
                    CosTable[i] = Math.Cos(angleaccum);
                    SineTable[i] = Math.Sin(angleaccum);
                    
                    angleaccum += Anglebetween;
                }


            }
            for (int i = 0; i < numcollisionpoints; i++)
            {
                offsets[i] = new PointF((float)(CosTable[i] * Radius), (float)(SineTable[i] * Radius));

            }

            return offsets;
        }
        public PointF[] getcollisioncheckpoints()
        {
            PointF[] useoffsets = getCollisionPointOffsets();
            PointF[] returnpoints = new PointF[useoffsets.Length];
            for (int i = 0; i < returnpoints.Length; i++)
            {
                returnpoints[i] = new PointF(Location.X+useoffsets[i].X,Location.Y+useoffsets[i].Y);


            }


            return returnpoints;

        }
        private PointF? GetTouchPoint(RectangleF checkrectangle)
        {
            PointF LinePointA=PrevLocation;
            PointF LinePointB=Location;
            PointF[] TopLine = new PointF[] { new PointF(checkrectangle.Left, checkrectangle.Top), new PointF(checkrectangle.Right, checkrectangle.Top) };
            PointF[] LeftLine = new PointF[] { new PointF(checkrectangle.Left, checkrectangle.Top), new PointF(checkrectangle.Left, checkrectangle.Bottom) };
            PointF[] BottomLine = new PointF[] { new PointF(checkrectangle.Left, checkrectangle.Bottom), new PointF(checkrectangle.Right, checkrectangle.Bottom) };
            PointF[] RightLine = new PointF[] { new PointF(checkrectangle.Right, checkrectangle.Top), new PointF(checkrectangle.Right, checkrectangle.Bottom) };
            int closestIndex = 0;
            List<PointF> touchpoints = new List<PointF>();
            List<PointF> balllocations = new List<PointF>();
            foreach (PointF[] checkline in new PointF[][] { TopLine, LeftLine, BottomLine, RightLine })
            {
                PointF? ballloc;
                PointF? result = GetWallIntersection(checkline[0], checkline[1], out ballloc);
                if (result != null)
                {
                    touchpoints.Add(result.Value);
                    balllocations.Add(ballloc.Value);
                }


            }
            //acquire closest point within balllocations..
            return BCBlockGameState.getClosestPoint(PrevLocation, balllocations.ToArray());




        }

        private PointF? GetWallIntersection(PointF LineAspot, PointF LineBSpot)
        {
            PointF? pointlessref;
            return GetWallIntersection(LineAspot, LineBSpot,out pointlessref);

        }

        private PointF? GetWallIntersection(PointF LineAspot, PointF LineBSpot,out PointF? ballLocation)
        {
            PointF[] collisioncheckpointsoff = getCollisionPointOffsets();

            ballLocation=null;
            PointF[] prevs, curr;
            List<PointF> isPoints= new List<PointF>();
            prevs = new PointF[collisioncheckpointsoff.Length];
            curr=new PointF[collisioncheckpointsoff.Length];
            for (int i = 0; i < collisioncheckpointsoff.Length; i++)
            {

                prevs[i] = new PointF(PrevLocation.X+collisioncheckpointsoff[i].X,PrevLocation.Y+collisioncheckpointsoff[i].Y);
                curr[i] = new PointF(Location.X + collisioncheckpointsoff[i].X,PrevLocation.Y + collisioncheckpointsoff[i].Y);

                
                PointF? intersection = BCBlockGameState.findIntersection(LineAspot, LineBSpot, prevs[i], curr[i]);
                if (intersection != null)
                    isPoints.Add(intersection.Value);



            }
            if (isPoints.Count == 0)
            {
                ballLocation = null;
                return null;
            }

            else
            {
                int indexintersect = 0;
                PointF? result = BCBlockGameState.getClosestPoint(LineAspot, isPoints.ToArray(), out indexintersect);
                if (result != null)
                {
                    ballLocation = new PointF(Location.X + collisioncheckpointsoff[indexintersect].X,
                                              Location.Y + collisioncheckpointsoff[indexintersect].Y);
                    return result;
                }
               

            }
            return null;
        }

        protected bool CheckWallCollision(RectangleF rectangleborders, RectangleF clientrect, ref bool DoEmitSound)
        {

            bool didhit=false;

           /* PointF[] results = BCBlockGameState.LiangBarsky(clientrect, PrevLocation, Location);
            if (results != null)
            {
                Location = results[0];


                return true;
            }
            */
            //use BCBlockGameState.getintersection to check
            //for intersections with each wall.
           
            
            /*
            if (!clientrect.Contains(rectangleborders))
            {
                Debug.Print("rect intersection");
                didhit = true;
                //knowns: ball (rectangleborders) has collided with the bounding box of clientrect.

                PointF balltouch = GetTouchPoint(rectangleborders).Value;
                Location=balltouch;
                DoEmitSound=true;



            }
            */
             float exitpercent = 0;
            if (rectangleborders.Left < clientrect.Left || (rectangleborders.Right> clientrect.Right))
            {
                didhit=true;

                    
                DoEmitSound = true;
                if (Velocity.X > 0)
                {
                    if (rectangleborders.Left < clientrect.Left)
                    {
                        exitpercent = Location.X/Velocity.X;
                        if (float.IsNaN(exitpercent)) exitpercent = 0;
                        Location = new PointF(clientrect.Left + Radius, Location.Y + (Velocity.Y*exitpercent));

                    }
                    else
                    {
                        exitpercent = (float) Math.Abs(clientrect.Right - Location.X)/Velocity.X;
                        if (float.IsNaN(exitpercent)) exitpercent = 0;
                        Location = new PointF(clientrect.Right-Radius, Location.Y + (Velocity.Y*exitpercent));
                    }
                }
                else
                {
                    if (rectangleborders.Left < clientrect.Left)
                    {
                        Location = new PointF(clientrect.Left + Radius, Location.Y);
                    }
                    else if (rectangleborders.Right > clientrect.Right)
                    {
                        Location = new PointF(clientrect.Right-Radius, Location.Y);
                    }
                    DoEmitSound=true;
                }

                if (rectangleborders.Left < clientrect.Left)
                {
                    
                    Velocity = new PointF(Math.Abs(Velocity.X), Velocity.Y);
                }
                else
                {
                    Velocity = new PointF(-Math.Abs(Velocity.X), Velocity.Y);
                }


            }
            if (rectangleborders.Top < clientrect.Top )
            {
                didhit=true;
                exitpercent = (float)(clientrect.Top-Location.Y)/Velocity.Y;
                if (float.IsNaN(exitpercent)) exitpercent = 1;
                Location = new PointF(Location.X, clientrect.Top + Radius);
                Velocity = new PointF(Velocity.X,Math.Abs(Velocity.Y));
                    
                DoEmitSound=true;
                   
                
                       
                    
            }
            
            return didhit;
        }

        public string GetWallHitSound()
        {
            foreach (var loopbehaviour in Behaviours)
            {
                string getsound = loopbehaviour.PaddleBallSound();
                if(getsound!=null&&getsound!=BaseBehaviour.DefaultPaddleBallSound &&getsound!="")
                    return getsound;
            }
            return BaseBehaviour.DefaultPaddleBallSound;



        }
        public String GetBlockHitSound()
        {
            string getsound = "";
            foreach (var loopbehaviour in Behaviours)
            {
                getsound = loopbehaviour.BlockBallSound();
                if (getsound!=null&&getsound != BaseBehaviour.DefaultBlockBallSound && getsound!="")
                    return getsound;


            }
            return BaseBehaviour.DefaultBlockBallSound;


        }

        //represents a ball in the Poing game world.

        #region ICloneable Members

        public virtual object Clone()
        {
            return new cBall(this);
        }

        #endregion

        
        public XElement GetXmlData(String pNodeName)
        {
            XElement BuildResult = new XElement(pNodeName);
            BuildResult.Add(StandardHelper.SaveElement(DrawColor,"Color"));
            BuildResult.Add(new XAttribute("Radius",Radius));
            BuildResult.Add(StandardHelper.SaveElement(Location,"Location"));
            BuildResult.Add(StandardHelper.SaveElement(Velocity, "Velocity"));
            BuildResult.Add(new XAttribute("Visible",Visible));
            BuildResult.Add(StandardHelper.SaveList<iBallBehaviour>(Behaviours, "Behaviours", true));
            return BuildResult;
        }
        public cBall(XElement Source)
        {
            Radius = Source.GetAttributeFloat("Radius");
            Visible = Source.GetAttributeBool("Visible");
            DrawColor = Source.ReadElement<Color>("Color", Color.Red);
            Location = Source.ReadElement<PointF>("Location", new PointF(25, 25));
            Velocity = Source.ReadElement<PointF>("Velocity", new PointF(3, 3));
            Behaviours = Source.ReadList<iBallBehaviour>("Behaviours", new List<iBallBehaviour>());
        }
        #region ISerializable Members

        public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            //info.AddValue("Brush", new SerializeBrush(DrawBrush));
            info.AddValue("Color", DrawColor);
            info.AddValue("Radius", Radius);
            info.AddValue("Location", Location);
            info.AddValue("Velocity", Velocity);
            info.AddValue("Visible", Visible);
            info.AddValue("Behaviours", Behaviours);
        }

        #endregion

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        public void ExplosionInteract(object sender, PointF Origin, PointF Vector)
        {
            double Strength = Math.Sqrt(Vector.X * Vector.X + Vector.Y * Vector.Y);
            //get the angle between the center point and ourselves.
            double angletest = BCBlockGameState.GetAngle(Origin, Location);
            PointF svector = new PointF((float)(Math.Sin(angletest) * Strength/25), (float)(Math.Cos(angletest) * Strength/25));
            Velocity = new PointF(Velocity.X + svector.X, Velocity.Y + svector.Y);


        }
    }


  
}
