using System;
using System.Drawing;
using System.Reflection;
using System.Threading;

namespace BASeBlock.Blocks
{
    /// <summary>
    /// hold data about blocks; used by BlockDataManager.
    /// </summary>
    public struct BlockData
    {
        public Type BlockType;
        public Image UseBlockImage; //rendered to 128x64 buffer
        public ManyToOneBlockData CreateData;
        /// <summary>
        /// retrievs the ManyToOneAttributeData associated with this Block Type- or null, if the type doesn't have the attribute set.
        /// </summary>
        /// <returns></returns>
        public ManyToOneAttributeData GetManyToOneAttributeData()
        {


            if (BCBlockGameState.HasAttribute(BlockType, typeof(ManyToOneBlockAttribute)))
            {

                return new ManyToOneAttributeData(BlockType);


            }


            return null;
        }
        public override string ToString()
        {
            return BlockType.Name;
        }
        /// <summary>
        /// Instantiates the block. This is required instead of directly calling Activator.CreateInstance() because
        /// Blocks with the ManyToOne essentially act as "templates".
        /// </summary>
        /// <param name="blockrect"></param>
        /// <returns></returns>
        public Block Instantiate(RectangleF blockrect)
        {
            if (CreateData == null)
                return (Block)Activator.CreateInstance(BlockType, new object[] { blockrect });
            else
            {
                //need to call static 'Instantiate' method of "BlockType".

                MethodInfo callit = BlockType.GetMethod("Instantiate", new Type[]{typeof(ManyToOneBlockData),typeof(RectangleF)});
                if(callit!=null)
                {
                    Block result = (Block)callit.Invoke(null, BindingFlags.Static, null, new object[] { CreateData,blockrect }, Thread.CurrentThread.CurrentCulture);
                    return result;
                }


                
                return null;
            }



        }

        public Image useBlockImage { 
            get 
            {

                if (UseBlockImage == null)
                {
                    Bitmap drawBitmap = new Bitmap(128, 64);
                    Graphics DrawBuffer = Graphics.FromImage(drawBitmap);
                    DrawBuffer.Clear(Color.Transparent);
                    try
                    {
                        Block instantiated = (Block)Activator.CreateInstance(BlockType, new object[] { new RectangleF(0, 0, 127, 63) });
                        //draw it...
                        instantiated.Draw(DrawBuffer);
                        UseBlockImage = (Image)drawBitmap.Clone();

                    }
                    catch (Exception ex)
                    {
                        return null;

                    }


                }

                return UseBlockImage;
            } 
            set { UseBlockImage=value; } }

        public String Usename;
    }
}