using System;
using System.Drawing;
using System.Runtime.Serialization;

namespace BASeBlock.Blocks
{
    [Serializable]
    [BlockDescription("Street1 has become a Silent Keyboard.  http://www.legacy.com/obituaries/savannah/obituary.aspx?page=lifestory&pid=139870551")]
    public class StreetBlock : ImageBlock
    {
        public StreetBlock(RectangleF blockrectangle)
            : base(blockrectangle, "streetblock")
        {


        }
        public StreetBlock(StreetBlock otherblock)
            : base(otherblock)
        {



        }

        public StreetBlock(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {

            //

        }
        public override object Clone()
        {
            return new StreetBlock(this);
        }

        public override bool PerformBlockHit(BCBlockGameState parentstate, cBall ballhit)
        {
            BCBlockGameState.Soundman.PlaySound("street", 1.0f);
            PopupText(parentstate, "R.I.P", new Font("Arial Black", 12));
            return true;
        }
    }
}