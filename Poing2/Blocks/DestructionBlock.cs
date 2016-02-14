using System;
using System.Drawing;
using System.Runtime.Serialization;
using System.Xml.Linq;
using BASeCamp.Elementizer;

namespace BASeCamp.BASeBlock.Blocks
{
    [Serializable]
    [ImpactEffectBlockCategory]
    [BlockDescription("Explodes, destroying Blocks within the explosion radius in the process.")]
    public class DestructionBlock : ImageBlock
    {
        private float _EffectRadius = 64;
        private int _ComboCount = 0;
        public int ComboCount { get { return _ComboCount; } set { _ComboCount = value; } }
        public float EffectRadius { get { return _EffectRadius; } set { _EffectRadius = value; } }
        public DestructionBlock(RectangleF BlockRect, String Imagekey)
            : base(BlockRect, Imagekey)
        {


        }
        public DestructionBlock(RectangleF BlockRect)
            : this(BlockRect, "explosive")
        {

        }
        public DestructionBlock(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            _EffectRadius = info.GetSingle("EffectRadius");

        }
        public DestructionBlock(DestructionBlock clonethis)
            : base(clonethis)
        {
            _EffectRadius = clonethis.EffectRadius;
        }

        public override XElement GetXmlData(string pNodeName)
        {
            var result = base.GetXmlData(pNodeName);
            result.Add(new XAttribute("EffectRadius",_EffectRadius));
            return result;
        }

        public DestructionBlock(XElement Source)
        {
            _EffectRadius = Source.GetAttributeFloat("EffectRadius");
        }
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("EffectRadius", _EffectRadius);
        }
        public override object Clone()
        {
            return new DestructionBlock(this);
        }

        public override bool PerformBlockHit(BCBlockGameState parentstate, cBall ballhit)
        {


            parentstate.ComboCount++;

            parentstate.ComboFinishTime = DateTime.Now + new TimeSpan(0, 0, 0, 1);

            AddScore(parentstate, 10);
            BCBlockGameState.Soundman.PlaySound("explode");
            //create the explosioneffect.
            ExplosionEffect explosioneffectobj = new ExplosionEffect(CenterPoint(), _EffectRadius);
            explosioneffectobj.ComboCount = parentstate.ComboCount;
            explosioneffectobj.DamageBlocks = true;
            


            parentstate.GameObjects.AddLast(explosioneffectobj);
           

            return true;
        }

    }
}