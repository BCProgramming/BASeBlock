using System;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.Serialization;
using BASeBlock.Particles;

namespace BASeBlock.Blocks
{
    /// <summary>
    /// ImageClipBlock: shows a piece of a given image.
    /// 
    /// </summary>
    [Serializable]
    [BlockDescription("ImageBlock that clips to a given image.")]
    public class ImageClipBlock : ImageBlock
    {

        private Image CachedClipImage = null;
        
        private RectangleF _cliprect;
        [TypeConverter(typeof(FloatFConverter))]
        public RectangleF cliprect { get { return _cliprect; } set { _cliprect = value; RefreshCachedClip(); } }

        protected override void OnImageKeySet(string oldkey, string newkey)
        {
            base.OnImageKeySet(oldkey, newkey);
            RefreshCachedClip();
        }

        void RefreshCachedClip()
        {
            if (_cliprect.IsEmpty) return;
            if (BlockImage == null) return;
            if (CachedClipImage != null) CachedClipImage.Dispose();
            Bitmap buildbitmap = new Bitmap((int)_cliprect.Width, (int)_cliprect.Height);
            Graphics grabclip = Graphics.FromImage(buildbitmap);
            grabclip.DrawImage(BlockImage, 0, 0, _cliprect, GraphicsUnit.Pixel);

            CachedClipImage = buildbitmap;
        }

        public ImageClipBlock(RectangleF blockrect)
            : base(blockrect)
        {
            BlockImageKey = "Generic_2";
            Size getsize = BCBlockGameState.Imageman.getLoadedImage(BlockImageKey).Size;
            cliprect = new RectangleF(0, 0, getsize.Width, getsize.Height);


        }

        public ImageClipBlock(RectangleF blockrect, String blockimagekey, RectangleF pcliprect)
            : base(blockrect, blockimagekey)
        {
            cliprect = pcliprect;



        }
        public ImageClipBlock(ImageClipBlock clonethis)
            : base(clonethis)
        {
            cliprect = clonethis.cliprect;
            
        }

        public ImageClipBlock(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            cliprect = (RectangleF)info.GetValue("ClipRect", typeof(RectangleF));


        }
        public override object Clone()
        {
            return new ImageClipBlock(this);
        }
        public override void Draw(Graphics g)
        {
            //draw the appropriate piece.
            //Image drawimage = BCBlockGameState.Imageman.getLoadedImage(BlockImageKey);
            //g.DrawImage(drawimage, BlockRectangle, cliprect, GraphicsUnit.Pixel);
            g.DrawImage(CachedClipImage, BlockRectangle);



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

            float useradius = Math.Min(BlockRectangle.Width, BlockRectangle.Height) / 4;
            Point topleft = new Point((int)cliprect.Left,(int)cliprect.Top);
            Size clipsize = new Size((int)cliprect.Width,(int)cliprect.Height);

            if (ballhit == null)
                return new PolyDebris(middlespot, 3, BlockImage, useradius - 2, useradius + 2, 3, 7,topleft,clipsize);
            else
            {
                return new PolyDebris(ballhit.Location, 3, BlockImage, useradius - 2, useradius + 2, 3, 7,topleft,clipsize);
            }
        }
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("ClipRect", cliprect);
        }
        public override bool PerformBlockHit(BCBlockGameState parentstate, cBall ballhit)
        {
            AddScore(parentstate, 30);
            PlayBlockSound(ballhit, "BOUNCE");
            return base.PerformBlockHit(parentstate, ballhit);
        }

    }
}