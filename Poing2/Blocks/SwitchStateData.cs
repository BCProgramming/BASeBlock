using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace BASeBlock.Blocks
{
    [Serializable]
    public class SwitchStateData : ISerializable
    {
        public Image StateImage;
        public Color StateColor = Color.Red;
        public String HitSound;
        public int InvokeID;
        public SwitchStateData()
        {

        }
        public SwitchStateData(SwitchBlockMulti ParentBlock, int pInvokeID, String pHitSound, Color pStateColor)
        {
            StateColor = pStateColor;
            HitSound = pHitSound;
            InvokeID = pInvokeID;
            RebuildImage(ParentBlock);
        }
        public void RebuildImage(SwitchBlockMulti Source)
        {

            Image tActiveImage = new Bitmap((int)Source.BlockRectangle.Width, (int)Source.BlockRectangle.Height);
            Image tInactiveImage = new Bitmap((int)Source.BlockRectangle.Width, (int)Source.BlockRectangle.Height);
            using (Graphics A = Graphics.FromImage(tActiveImage))
            {
                using (Graphics I = Graphics.FromImage(tInactiveImage))
                {

                    Image gummyActive = BCBlockGameState.GetGummyImage(StateColor, tActiveImage.Size);

                    Image SwitchOverlay = BCBlockGameState.Imageman.getLoadedImage("SwitchOverlay");

                    A.DrawImageUnscaled(gummyActive, 0, 0);

                    A.DrawImage(SwitchOverlay, 0, 0, tActiveImage.Width, tActiveImage.Height);


                    StateImage = tActiveImage;

                }
            }

        }
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("InvokeID", InvokeID);
            info.AddValue("HitSound", HitSound);
            info.AddValue("StateColor", StateColor);

        }
        public SwitchStateData(SerializationInfo info, StreamingContext context)
        {
            try { InvokeID = info.GetInt32("InvokeID"); }
            catch { }
            try { HitSound = info.GetString("HitSound"); }
            catch { }


        }


    }
}
