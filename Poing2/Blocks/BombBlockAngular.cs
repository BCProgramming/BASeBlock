using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.Serialization;

namespace BASeBlock.Blocks
{
    /// <summary>
    /// BombBlock that explodes balls like the BombBlock, but at given angular increments.
    /// </summary>
    [Serializable]
    [StandardBlockCategory]
    [BlockDescription("Works similarly to the BombBlock, but  is more customizable.")]
    public class BombBlockAngular : ImageBlock
    {
        private float _StartAngle = 0;
        private float _EmissionCount = 4;
        private float _SpeedMultiplier = 1.4f;
        public float StartAngle { get { return _StartAngle; } set { _StartAngle = value; } }
        public float StartAngle_Degree { get { return (float)(StartAngle * (180 / Math.PI)); } set { StartAngle = (float)(value / (180 / Math.PI)); } }
        public float SpeedMultiplier { get { return _SpeedMultiplier; } set { _SpeedMultiplier = value; } }
        public float EmissionCount { get { return _EmissionCount; } set { _EmissionCount = value; } }

        public List<iBallBehaviour> PassBehaviours = new List<iBallBehaviour>() { new TempBallBehaviour() };


        public BombBlockAngular(BombBlockAngular clonethis)
            : base(clonethis)
        {
            _StartAngle = clonethis.StartAngle;
            _EmissionCount = clonethis.EmissionCount;
            SpeedMultiplier = clonethis.SpeedMultiplier;
            PassBehaviours = new List<iBallBehaviour>(from p in clonethis.PassBehaviours select (iBallBehaviour)p.Clone());



        }
        public override object Clone()
        {

            return new BombBlockAngular(this);    
                
            
        }
        public BombBlockAngular(RectangleF blockrect)
            : base(blockrect, "BOMB")
        {
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("PassBehaviours", PassBehaviours);
            info.AddValue("StartAngle", _StartAngle);
            info.AddValue("EmissionCount", _EmissionCount);
            base.GetObjectData(info, context);
        }
        public BombBlockAngular(SerializationInfo info, StreamingContext context):base(info,context)
        {
            PassBehaviours = (List<iBallBehaviour>)info.GetValue("PassBehaviours", typeof(List<iBallBehaviour>));
            _StartAngle = info.GetSingle("StartAngle");
            _EmissionCount = info.GetSingle("EmissionCount");

        }
        public override bool PerformBlockHit(BCBlockGameState parentstate, cBall ballhit)
        {
            //spawn the appropriate number of new items, giving each a clone of the items in PassBehaviours.
            float impactmagnitude = ballhit.getMagnitude()*SpeedMultiplier;
            double angleincrement = Math.PI * 2 / _EmissionCount;
            for (double currangle = _StartAngle; currangle < _StartAngle + Math.PI * 2; currangle += angleincrement)
            {
                float useX = (float)(Math.Cos(currangle) * impactmagnitude);
                float useY = (float)(Math.Sin(currangle) * impactmagnitude);

                PointF usespeed = new PointF(useX, useY);
                cBall addball = new cBall(CenterPoint(), usespeed);
                addball.Behaviours.AddRange(from beh in PassBehaviours select (iBallBehaviour)(beh.Clone()));

                //parentstate.NextFrameCalls.Enqueue(new BCBlockGameState.NextFrameStartup(() => parentstate.Balls.AddLast(addball)));
                parentstate.NextFrameCalls.Enqueue(new BCBlockGameState.NextFrameStartup(() => parentstate.Balls.AddLast(addball)));


            }



            base.PerformBlockHit(parentstate, ballhit);
            return true;
        }

    }
}