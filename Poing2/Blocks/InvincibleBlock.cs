using System;
using System.Drawing;
using System.Runtime.Serialization;
using System.Xml.Linq;
using BASeCamp.Elementizer;

namespace BASeCamp.BASeBlock.Blocks
{
    [Serializable]
    [StandardBlockCategory]
    [BlockDescription("An Invincible Block that can only be destroyed under very special circumstances.")]
    public class InvincibleBlock : ImageBlock
    {
        public bool Silent { get; set; }
        public InvincibleBlock(RectangleF blockrect)
            : base(blockrect, "Invincible")
        {


        }
        public InvincibleBlock(InvincibleBlock cloneme)
            : base(cloneme)
        {
            Silent = cloneme.Silent;
            //TriggerID = cloneme.TriggerID;
        }
        public InvincibleBlock(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            try { Silent = info.GetBoolean("Silent"); }
            catch { Silent = false; }


        }
        public InvincibleBlock(XElement Source):base(Source)
        {
            Silent = Source.GetAttributeBool("Silent", false);
        }
        public override XElement GetXmlData(string pNodeName)
        {
            XElement result = base.GetXmlData(pNodeName);
            result.Add(new XAttribute("Silent",Silent));
            return result;
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("Silent", Silent);
            
        }
        public override object Clone()
        {
            return new InvincibleBlock(this);
        }
        public override bool MustDestroy()
        {
            return false;
        }
        public override bool PerformBlockHit(BCBlockGameState parentstate, cBall ballhit)
        {
            bool nodef = false;
            DefaultHitSound = "";
           
            if(!Silent) PlayBlockSound(ballhit, "METAL");
            RaiseBlockHit(parentstate, ballhit, ref nodef);

            //temporary code for testing powerup:
            //invincible blocks will now "emerge" a mushroom.
            //MushroomPower mp = new MushroomPower(this);
            //parentstate.GameObjects.AddLast(mp);

            return false;
        }


    }
}