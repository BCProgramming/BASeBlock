using System;
using System.Drawing;
using System.Runtime.Serialization;
using System.Xml.Linq;

namespace BASeCamp.BASeBlock.Blocks
{
    [Serializable()]
    public abstract class GenericImageBlock : ImageBlock
    {
        protected String ImageKey = "";

        protected GenericImageBlock(RectangleF blockrect, String imagemankey)
            : base(blockrect, imagemankey)
        {


        }
        protected GenericImageBlock(XElement source, Object pPersistenceData) :base(source,pPersistenceData)
        {

        }
        protected GenericImageBlock(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {

            // ImageKey = info.GetString("BlockImageKey");

        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);

        }

        public override XElement GetXmlData(String pNodeName,Object pPersistenceData)
        {
            return base.GetXmlData(pNodeName,pPersistenceData);
        }

        public abstract override object Clone();
        public abstract override bool PerformBlockHit(BCBlockGameState parentstate, cBall ballhit);


    }
}