using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using BASeCamp.BASeBlock.Blocks;

namespace BASeCamp.BASeBlock.Blocks
{
    [Serializable]
    [StandardBlockCategory]
    class WoodBlock:ImageBlock,IDamageableBlock
    {
        private int _Damage, _Health=5;
        public int Damage { get { return _Damage; } set { _Damage = value; hasChanged = true; } }
        public int Health { get { return _Health; } set { _Health = value; hasChanged = true; } }
        
        public WoodBlock(WoodBlock clonethis)
            : base(clonethis)
        {

        }
        public WoodBlock(RectangleF blockrect)
            : base(blockrect, "WOOD")
        {


        }
        public WoodBlock(SerializationInfo info, StreamingContext context):base(info,context)
        {
            Health = info.GetInt32("Health");
            Damage = info.GetInt32("Damage");

        }
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("Health", Health);
            info.AddValue("Damage", Damage);

        }
        public override bool PerformBlockHit(BCBlockGameState parentstate, cBall ballhit)
        {

            var ballrel = getBallRelative(ballhit);

            //we only take damage if we are hit on one of the sides...
            if (ballrel == BallRelativeConstants.Relative_Left || ballrel == BallRelativeConstants.Relative_Right)
            {
                Damage++;
                base.PerformBlockHit(parentstate, ballhit);
                return Health < Damage;
            }
            else
            {
                base.PerformBlockHit(parentstate, ballhit);
                return false;

            }



            
        }
        public override void Draw(Graphics g)
        {
            base.Draw(g);
            DrawDamage(g, this);
        }
        

    }
}
