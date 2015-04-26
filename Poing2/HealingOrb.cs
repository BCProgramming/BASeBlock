using System.Drawing;
using BASeBlock.GameObjects.Orbs;

namespace BASeBlock.GameObjects.Orbs
{
    public class HealingOrb : CollectibleOrb
    {
        public HealingOrb(PointF pLocation):this(pLocation,DefaultSize)
        {

        }
        public HealingOrb(PointF pLocation, SizeF usesize)
            : base(pLocation, usesize,new Image[]{BCBlockGameState.Imageman.getLoadedImage("HEART")})
        {


        }
        protected override CollectibleTypeConstants getCollectibleType()
        {
            return base.getCollectibleType();
        }
        protected override bool TouchCharacter(BCBlockGameState gstate, GameCharacter gchar)
        {
            return true;
        }
        protected override bool TouchPaddle(BCBlockGameState gstate, Paddle pchar)
        {
            pchar.HP++;
            return true;
        }
    }
}