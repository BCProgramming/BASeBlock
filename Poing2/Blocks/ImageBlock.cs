using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Drawing.Imaging;
using System.Runtime.Serialization;
using System.Xml.Linq;
using BASeCamp.BASeBlock.Particles;
using BASeCamp.Elementizer;

namespace BASeCamp.BASeBlock.Blocks
{
    [Serializable()]
    [BlockDescription("Commonly used base class for blocks that are depicted using Image Data.")]
    public class ImageBlock : Block
    {
        private String _BlockImageKey = "";
        [Editor(typeof(ImageKeyEditor), typeof(UITypeEditor))]
        public String BlockImageKey { get { return _BlockImageKey; }

            set
            {
                String oldval =_BlockImageKey;
                _BlockImageKey = value;
                OnImageKeySet(oldval, _BlockImageKey);

            }
        }
        public ImageAttributes DrawAttributes { get; set; }
        public Image BlockImage
        {
            get
            {
                if (BCBlockGameState.Imageman.Exists(BlockImageKey.ToUpper()))
                {
                    return BCBlockGameState.Imageman[BlockImageKey.ToUpper()];
                }
                else
                {
                    return null;


                }


            }


        }

        protected virtual void OnImageKeySet(String oldkey,String newkey)
        {


        }


        public ImageBlock()
            : this(new RectangleF(0, 0, 32, 16), null)
        {

        }
        public ImageBlock(RectangleF blockrect)
        {
            BlockRectangle = blockrect;
            BlockImageKey = "bomb";
        }

        public ImageBlock(RectangleF blockrect, String blockimagekey)
        {
            BlockRectangle = blockrect;

            BlockImageKey = blockimagekey;



        }

        public ImageBlock(ImageBlock clonethis)
            : base(clonethis)
        {

            BlockImageKey = clonethis.BlockImageKey;
            DrawAttributes = clonethis.DrawAttributes;
            BlockRectangle = clonethis.BlockRectangle;



        }
        protected ImageBlock(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {

            //custom added code here...
            BlockImageKey = info.GetString("BlockImageKey");
            // DrawAttributes = (ImageAttributes)info.GetValue("BlockImageAttributes", typeof(ImageAttributes));


        }
        public ImageBlock(XElement Source):base(Source)
        {
            BlockImageKey = Source.GetAttributeString("BlockImageKey");
        }

        public override XElement GetXmlData(string pNodeName)
        {
            XElement result = base.GetXmlData(pNodeName);
            result.Add(new XAttribute("BlockImageKey",BlockImageKey));
            return result;
        }

        public override bool PerformBlockHit(BCBlockGameState parentstate, cBall ballhit)
        {
            
            
            return base.PerformBlockHit(parentstate, ballhit);
        }
        public override object Clone()
        {
            return new ImageBlock(this);
        }
        protected override void CreateOrbs(PointF Location, BCBlockGameState gstate)
        {
            //base.CreateOrbs(Location, gstate);
        }
        protected override Particle AddStandardSprayParticle(BCBlockGameState parentstate, cBall ballhit)
        {
            
            PointF middlespot;
            if (ballhit != null)
                middlespot = ballhit.Location;
            else
            {
                middlespot = CenterPoint();
            }
            //return base.AddStandardSprayParticle(parentstate, ballhit);

            float useradius = Math.Min(BlockRectangle.Width  , BlockRectangle.Height )/4;


            if (ballhit == null)
                return new PolyDebris(middlespot, 3, BlockImage,useradius-2,useradius+2,3,7);
            else
            {
                return new PolyDebris(ballhit.Location,3, BlockImage, useradius - 2, useradius + 2, 3, 7);
            }
        }
        public override void Draw(Graphics g)
        {

            if (Single.IsNaN(BlockRectangle.X) || Single.IsNaN(BlockRectangle.Y))
                BlockRectangle = new RectangleF(0, 0, BlockRectangle.Width, BlockRectangle.Height);
         

            base.Draw(g);
            if (BlockImage != null)
            {
                bool error = true;
                while (error)
                {
                    try
                    {

                        using (Image usedrawimage = (Image)BlockImage.Clone())
                        {
                            if (DrawAttributes != null)
                            {
                                // g.DrawImage(BlockImage,BlockRectangle,
                                g.DrawImage(usedrawimage, new Rectangle((int)BlockRectangle.Left, (int)BlockRectangle.Top, (int)BlockRectangle.Width, (int)BlockRectangle.Height), 0f, 0f, BlockImage.Width, BlockImage.Height, GraphicsUnit.Pixel, DrawAttributes);
                            }
                            else
                            {
                                g.DrawImage(usedrawimage, BlockRectangle);
                            }
                        }
                        error = false;
                    }
                    catch
                    {
                        error = true;

                    }
                    
                    //g.DrawString("test", new Font("Comic Sans MS", 8), new SolidBrush(Color.Black), BlockRectangle);
                }

            }
            
        }
        


        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            //save the base class info... (rectangle... and whatever else)
            base.GetObjectData(info, context);
            //now, we save our fun stuff.
            info.AddValue("BlockImageKey", BlockImageKey);

            //ta DA!

        }
    }
}