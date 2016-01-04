using System;
using System.Drawing;
using System.Runtime.Serialization;
using BASeCamp.BASeBlock.Particles;

namespace BASeCamp.BASeBlock.Blocks
{
    /// <summary>
    /// Creates a "Strong" block. this block resists several hits before being destroyed.
    /// </summary>
    /// 
    [Serializable]
    [StandardBlockCategory]
    [BlockDescription("Resists several hits before being destroyed. Total Health is customizable.")]
    public class StrongBlock : ImageBlock, ISerializable,IDamageableBlock 
    {
        protected int numhits = 0; //number of times hit.
        protected int totalstrength = 5;

        public int Health { get { return totalstrength; } set { totalstrength = value; hasChanged = true; } }



        protected String[] ourImages;
        /// <summary>
        /// The total "power" of this block. Generally translates to the number of times a ball needs to hit it. Note that
        /// balls with the "PowerBallBehaviour" will go right through it and tend to spam score as they do so.
        /// </summary>
        public int Strength
        {
            get
            {
                return totalstrength;
            }
            set
            {
                totalstrength = value;
                hasChanged = true;
            }
        }

        public int Damage
        {
            get { return numhits; }
            set { numhits = value; hasChanged = true; }


        }


        public StrongBlock(RectangleF blockrect)
            : this(blockrect, BCBlockGameState.Imageman.getImageFramesString("STRONG"))
        {


        }
        public StrongBlock(StrongBlock duplicatethis)
            : base(duplicatethis)
        {
            ourImages = duplicatethis.ourImages;
        }

        public StrongBlock(RectangleF blockrect, String[] strengthsteps)
            : base(blockrect, strengthsteps[0])
        {
            totalstrength = strengthsteps.Length - 1;
            ourImages = strengthsteps;

        }
        public StrongBlock(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            ourImages = (String[])info.GetValue("ourImages", typeof(String[]));
            numhits = info.GetInt32("numhits");
            totalstrength = ourImages.Length - 1;


        }
        protected override Particle AddStandardSprayParticle(BCBlockGameState parentstate, cBall ballhit)
        {
            if (numhits < totalstrength)
            {
                return AddSprayParticle_Default(parentstate, ballhit);
            }
            else
            {
                return base.AddStandardSprayParticle(parentstate, ballhit);
            }

        }
        /// <summary>
        /// override added Dec. 06 2010: growblock wasn't working properly... turns out the base call was simply calling the Imageblock
        /// draw routine which uses a image last set by the strongblock when it was hit.
        /// fix was to make StrongBlock::Draw change the image before calling the base routine.
        /// </summary>
        /// <param name="g"></param>
        public override void Draw(Graphics g)
        {
            //get the percentage of our total damage...
            //float currpercent = (float)numhits / (float)totalstrength;


            //currpercent can be used to set the amount of black to mix (or red, or something).
            //also use it to choose a block index.
            /*
            int indexuse = (int)(((float)ourImages.Length) * currpercent);



            if (indexuse >= ourImages.Length) indexuse = ourImages.Length - 1;

            //old code, before messing about with the "crack" images...
            //BlockImageKey = ourImages[indexuse];

            BlockImageKey = ourImages[0]; //always use the "clean" image...
            */


            //DrawAttributes = new ImageAttributes();
            //DrawAttributes.SetColorMatrix(ColorMatrices.GetColourizer(-currpercent, -currpercent, -currpercent,1));


            base.Draw(g);

            DrawDamage(g, numhits, totalstrength);
            //now draw the cracks. get the crack image...
            /* String oldkey = BlockImageKey;
            BlockImageKey = "CRACK" + indexuse;
            base.Draw(g);
            BlockImageKey = oldkey;
            */


        }
        public override bool PerformBlockHit(BCBlockGameState parentstate, cBall ballhit)
        {
            base.PerformBlockHit(parentstate, ballhit);
            //BCBlockGameState.Soundman.PlaySound("BBounce",1.0f);
            base.PlayBlockSound(ballhit, "METAL2");
            AddScore(parentstate, 20 * (numhits));
            numhits++;
            int indexuse = numhits;
            if (indexuse > ourImages.Length - 1) indexuse = ourImages.Length - 1;
            BlockImageKey = ourImages[indexuse];
            hasChanged = true;
            if (totalstrength < numhits)
            {


                return true;



            }
            return false;


        }
        public override object Clone()
        {
            //return base.Clone();
            return new StrongBlock(this);
        }


        #region ISerializable Members

        void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("ourImages", ourImages);
            info.AddValue("numhits", numhits);
        }

        #endregion
    }
}