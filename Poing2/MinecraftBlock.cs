using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Text;
using BASeCamp.BASeBlock.Blocks;
using Ionic.Zip;

namespace BASeCamp.BASeBlock
{
    public class MinecraftBlockAttribute : BlockCategoryAttribute
    {
        /// <summary>
        /// returns the short name of this attribute.
        /// </summary>
        /// <returns></returns>
        public override string GetName()
        {
            return "Minecraft Blocks";
        }

        /// <summary>
        /// returns the description for this Block category.
        /// </summary>
        /// <returns></returns>
        public override string GetDescription()
        {
            return "Set of blocks loaded from your Minecraft.jar";
        }

        public override Image CategoryImage()
        {
            return null;
            //throw new NotImplementedException();
        }
        public override bool ShowCategory()
        {
            return MinecraftBlock.grabterrain != null; //will be null of the image wasn't found.
        }
    }
    //MinecraftBlock: a silly implementation of Block that will load images from minecraft.jar if minecraft is installed.
    [ManyToOneBlock]
    [MinecraftBlock]
    [Serializable]
    public class MinecraftBlock : ImageBlock ,IGameInitializer
    {

        private class MinecraftBlockInformation
        {
            public String Name { get; set; }
            public Image BlockImage { get; set; }


            public MinecraftBlockInformation(String pName, Image pBlockImage)
            {
                Name = pName;
                BlockImage = pBlockImage;

            }
            /// <summary>
            /// 
            /// </summary>
            /// <param name="pName"></param>
            /// <param name="Offset">BLOCK location of offset (x16)</param>
            /// <param name="largerpic"></param>
            public MinecraftBlockInformation(String pName, Point Offset, Image largerpic)
            {
                //all MC blocks are 16x16...
                //we need to draw the 16x16 block starting at Offset.
                Name = pName;
                Point GrabOffset = new Point(Offset.X * 16, Offset.Y * 16);
                Bitmap createimage = new Bitmap(16, 16);
                Graphics createcanvas = Graphics.FromImage(createimage);
                createcanvas.Clear(Color.Transparent);
                createcanvas.DrawImage(largerpic, new Rectangle(0, 0, 16, 16), 
                    new Rectangle(GrabOffset, new Size(16, 16)), GraphicsUnit.Pixel);

                //createimage is the image...
                BlockImage = createimage;
               // createimage.Save(Path.Combine("D:\\",pName + ".png"));


            }
        }
        private class NameOffsetData
        {
            public String Name { get; set; }
            public Point Offset { get; set; }
            public NameOffsetData(String pName, Point puseoffset)
            {
                Name = pName;
                Offset = puseoffset;


            }

        }
        //list if static Name=>Block offsets...
        private static NameOffsetData[] NameData = new NameOffsetData[]
        {
            //0,15 through 10,15 is  the cracks...
            new NameOffsetData("CRACK1",new Point(0,15)),
            new NameOffsetData("CRACK2",new Point(1,15)),
            new NameOffsetData("CRACK3",new Point(3,15)),
            new NameOffsetData("CRACK4",new Point(4,15)),
            new NameOffsetData("CRACK5",new Point(5,15)),
            new NameOffsetData("CRACK6",new Point(6,15)),
            new NameOffsetData("CRACK7",new Point(7,15)),
            new NameOffsetData("CRACK8",new Point(8,15)),
            new NameOffsetData("CRACK9",new Point(9,15)),
            new NameOffsetData("Grass",new Point(0,0)),
            new NameOffsetData("Stone",new Point(1,0)),
            new NameOffsetData("Dirt",new Point(2,0)),
            new NameOffsetData("SideGrass",new Point(3,0)),
            new NameOffsetData("Planks",new Point(4,0)),
            new NameOffsetData("Slabs",new Point(5,0)),
            new NameOffsetData("SlabTop",new Point(6,0)),
            new NameOffsetData("Brick",new Point(7,0)),
            new NameOffsetData("TNT",new Point(8,0)),
            new NameOffsetData("Cobweb",new Point(11,0)),
            new NameOffsetData("Cobblestone",new Point(0,1)),
            new NameOffsetData("Bedrock",new Point(1,1)),
            new NameOffsetData("Sand",new Point(2,1)),
            new NameOffsetData("Gravel",new Point(3,1)),
            new NameOffsetData("Wood",new Point(4,1)),
            new NameOffsetData("Iron Block",new Point(6,1)),
            new NameOffsetData("Gold Block",new Point(7,1)),
            new NameOffsetData("Diamond Block", new Point(8,1)),
            new NameOffsetData("Mossy Cobblestone",new Point(4,2))
            /*
             * 

4,1 Wood
6,1 IronBlock
7,1 GoldBlock
8,1 DiamondBlock
0,2 Gold Ore
1,2 Iron Ore
2,2 Coal
3,3 Bookcase
4,2 MossStone

*/


        };
        private static Dictionary<String, MinecraftBlockInformation> MCBlocks = new Dictionary<string, MinecraftBlockInformation>();
        

        //BlockType.GetMethod("Instantiate", new Type[]{typeof(ManyToOneBlockData),typeof(RectangleF)});
        public static Block Instantiate(ManyToOneBlockData data, RectangleF blockrect)
        {
            //data.DisplayText is the MC name.
            //we can use that as an index into the BlockInformation static dictionary.

            return new MinecraftBlock(blockrect, "MC_" + data.DisplayText.ToUpper());

            return null;
        }
        public static Image grabterrain = null;
        private static Dictionary<String, Image> MCImages; 
        public static void GameInitialize(iManagerCallback datahook)
        {
            //minecraft.jar/title/splashes
            loadMCSplashes();
            ReadBlocks();

            if (grabterrain != null)
            {
                foreach (var iterate in NameData)
                {
                    MinecraftBlockInformation newinfo = new MinecraftBlockInformation(iterate.Name, iterate.Offset, grabterrain);
                    BCBlockGameState.Imageman.AddImage("MC_" + iterate.Name.ToUpper(), newinfo.BlockImage);
                    MCBlocks.Add(newinfo.Name, newinfo);
                }
            }


        }
        private static void ReadBlocks()
        {
            MCImages = new Dictionary<string, Image>();
            //read blocks from jar.
            // from /textures/blocks
            String sourcefile = getMCjar();
            ZipFile zf = new ZipFile(sourcefile);
            foreach (var entry in zf.Entries)
            {
                if (entry.FileName.ToLower().StartsWith("textures/blocks/") && entry.FileName.ToLower().EndsWith(".png"))
                {
                    int lastslash= entry.FileName.LastIndexOf("/");
                    //String spngname = entry.FileName.Substring(lastslash+1,)
                    int endfilename = entry.FileName.LastIndexOf(".png", StringComparison.OrdinalIgnoreCase)-1;
                    //grab  the filename part, use the name as the index.
                    String usefname = entry.FileName.Substring(lastslash+1, endfilename - lastslash);
                    String usetemppath = Path.GetTempFileName();
                    Image blockimage = null;
                    using(FileStream fs = new FileStream(usetemppath,FileMode.Create,FileAccess.ReadWrite,FileShare.Delete,1024*8,FileOptions.DeleteOnClose))
                    {
                        entry.Extract(fs);
                        //seek to start.
                        fs.Seek(0, SeekOrigin.Begin);
                        //read image.
                        blockimage = Image.FromStream(fs);
                    }
                    //read
                    MinecraftBlockInformation mbi = new MinecraftBlockInformation(usefname, blockimage);
                    BCBlockGameState.Imageman.AddImage("MC_" + usefname.ToUpper(), blockimage);
                    MCBlocks.Add("MC_" + usefname, mbi);
                    //That sorts the job it does...
                }


            }



        }
        public MinecraftBlock(RectangleF rectmake):base(rectmake)
        {

            //needed for the game load...
        }
        public MinecraftBlock(MinecraftBlock clonethis)
            : base(clonethis)
        {


        }
        public override void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
        {
            //we only need the key...

            base.GetObjectData(info, context);
        }
        public override object Clone()
        {
            return new MinecraftBlock(this);
        }
        //we need to override draw to provide the proper "feel" of MC blocks.
        public override void Draw(Graphics g)
        {
            var temp = g.InterpolationMode;
            g.InterpolationMode = InterpolationMode.NearestNeighbor;
            base.Draw(g);
            g.InterpolationMode = temp;
        }
        private MinecraftBlock(RectangleF rectmake, String MCImagekey):base(rectmake,MCImagekey)
        {
            


        }
        public static ManyToOneBlockData[] GetManyToOneData()
        {
            List<ManyToOneBlockData> iteratedata= new List<ManyToOneBlockData>();
            foreach (var iterate in MCBlocks)
            {
                ManyToOneBlockData bdata = new ManyToOneBlockData(typeof(MinecraftBlock), iterate.Key, iterate.Value.BlockImage);

                iteratedata.Add(bdata);
                

            }

            return iteratedata.ToArray();

        }
        //  MethodInfo acquiremethod = fortype.GetMethod("GetManyToOneData");
                //AcquiredData = (ManyToOneBlockData[])
        //acquiremethod.Invoke(null, BindingFlags.Static, null, null, Thread.CurrentThread.CurrentCulture);
        
        private static string getMCjar()
        {
            String appdatafolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            return Path.Combine(appdatafolder,".minecraft\\bin\\minecraft.jar");




        }
        public static void loadMCSplashes()
        {
            //reads the minecraft splash screen information from the jar, if found, and adds them to our own list.
            String mcjar = getMCjar();
            if (File.Exists(mcjar))
            {

                var readjar = new ZipFile(mcjar);
                foreach (var iterate in readjar)
                {
                    if (iterate.FileName.Equals("title\\splashes.txt", StringComparison.OrdinalIgnoreCase))
                    {
                        MemoryStream readtomemstream = new MemoryStream();
                        iterate.Extract(readtomemstream);
                        readtomemstream.Seek(0, SeekOrigin.Begin);
                        StreamReader sr = new StreamReader(readtomemstream);
                        String[] appendtext = sr.ReadToEnd().Split(new char[] { '\n' });
                        sr.Close();
                        List<String> newintros = new List<string>(BCBlockGameState.IntroStrings);
                        newintros.AddRange(appendtext);
                        BCBlockGameState.IntroStrings = newintros.ToArray();
                        return;

                    }

                }



            }



        }
       

    }
}
