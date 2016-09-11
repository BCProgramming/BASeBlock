using System;
using System.Drawing;
using System.Runtime.Serialization;
using System.Xml.Linq;

namespace BASeCamp.BASeBlock.Blocks
{
    [Serializable]
    //[BBEditorInvisible]
    public class TexturedBlock : ImageBlock, ISerializable
    {
        TextureBrush tb = null;
        Pen mPen = new Pen(Color.Transparent);
        public TexturedBlock(RectangleF blockrect, String blockimagekey)
            : base(blockrect, blockimagekey)
        {
            tb = new TextureBrush(BCBlockGameState.Imageman.getLoadedImage(blockimagekey));

        }
        private void refreshtexture()
        {
            try
            {
                tb = new TextureBrush(BCBlockGameState.Imageman.getLoadedImage(BlockImageKey));
            }
            catch (InvalidOperationException eex)
            {
                tb = new TextureBrush((Image)BCBlockGameState.Imageman.getLoadedImage(BlockImageKey).Clone());

            }
        }

        public TexturedBlock(RectangleF blockrect, String blockimagekey, Pen PenUse)
            : this(blockrect, blockimagekey)
        {
            mPen = PenUse;


        }
        public TexturedBlock(TexturedBlock clonethis)
            : base(clonethis)
        {
            BlockImageKey = clonethis.BlockImageKey;
            refreshtexture();

        }
        public TexturedBlock(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            refreshtexture();

        }
        public TexturedBlock(XElement source, Object pPersistenceData) :base(source,pPersistenceData)
        {
            refreshtexture();
        }
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("PenColor",mPen.Color);
            
        }

        public override object Clone()
        {
            return new TexturedBlock(this);
        }
        public TexturedBlock(RectangleF blockrect, String blockimagekey, Color PenColorUse)
            : this(blockrect, blockimagekey, new Pen(PenColorUse))
        {


        }

        public override void Draw(Graphics g)
        {
            //base.Draw(g);
            g.FillRectangle(tb, BlockRectangle);
            g.DrawRectangle(mPen, BlockRectangle.Left, BlockRectangle.Top, BlockRectangle.Width, BlockRectangle.Height);
        }
    }
}