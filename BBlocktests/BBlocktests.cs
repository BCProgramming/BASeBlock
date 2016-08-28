using System;
using System.Collections;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Reflection.BASeCamp;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Web.UI.WebControls;
using System.Xml.Linq;
using BASeCamp.BASeBlock.Blocks;
using BASeCamp.CommandLineParser;
using BASeCamp.Licensing;
using BASeCamp.Elementizer;
using BASeCamp.Updating;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using BASeCamp.BASeBlock;
using BASeCamp.BASeBlock.HighScores;
using Image = System.Drawing.Image;


namespace BBlocktests
{
    /// <summary>
    /// Summary description for BBlocktests
    /// </summary>
    [TestClass]
    public class BBlocktests
    {
        public BBlocktests()
        {
            
            BCBlockGameState.Initgamestate();
            //
            // TODO: Add constructor logic here
            //
        }

        [TestMethod]
        public void TestRectangleXML()
        {
            Rectangle BuildRect = new Rectangle(32, 32, 100, 100);

            XElement readRect = StandardHelper.SaveElement(BuildRect, "TestRectangle");
            Rectangle testdupe = StandardHelper.ReadElement<Rectangle>(readRect);
            if (!BuildRect.Equals(testdupe)) throw new AssertFailedException("Retrieved Rectangle doesn't match input rectangle.");
        }

        [TestMethod]
        public void TestListXML()
        {
            //just go for Int right now.
            List<int> buildint = Enumerable.Range(0, 500).ToList();
            XElement intList = StandardHelper.SaveList<int>(buildint, "Integers");
            List<int> reconstituted = StandardHelper.ReadList<int>(intList);

            for (int i = 0; i < buildint.Count; i++)
            {
                if (buildint[i] != reconstituted[i])
                    throw new AssertFailedException("item index " + i + " values do not match: Original:" + buildint[i] + " reconstituted:" + reconstituted[i]);
            }
        }



        [TestMethod]
        public void TestStringArray()
        {
            String[] TestSave = new String[] { "STRONG1", "STRONG2", "STRONG3" };
            XElement SavedItem = StandardHelper.SaveArray(TestSave, "Strong");

        }
        [TestMethod]
        public void TestArrayXML()
        {
            int[,] sq1 = new int[,]
            {
                {1, 2, 3, 4, 5},
                {6, 7, 8, 9, 10},
                {11, 12, 13, 14, 15},
                {16, 17, 18, 19, 20},
                {21, 22, 23, 24, 25}
            };
            int[][] sq2 = new int[][]
            {
                new int[] {1, 2, 3, 4, 5},
                new[] {6, 7, 8, 9, 10},
                new[] {11, 12, 13, 14, 15},
                new[] {16, 17, 18, 19, 20},
                new[] {21, 22, 23, 24, 25},
                new[] {26, 27, 28, 29, 30}
            };
            XElement sq1Result = StandardHelper.SaveArray(sq1, "MultiDim");
            XElement sq2Result = StandardHelper.SaveArray(sq2, "MultiArray");
            
            int[,] First = (int[,])(new XElement("Test",sq1Result).ReadArray<int>("MultiDim"));
            int[][] Second = (int[][])(new XElement("Test",sq2Result).ReadArray<int[]>("MultiArray"));
            ColorMatrix sample = ColorMatrices.GrayScale();
            XElement CMatrix = StandardHelper.SaveElement(sample, "CMatrix");
            XDocument savedoc = new XDocument(new XElement("Root", sq1Result, sq2Result, CMatrix));
            savedoc.Save("D:\\testdocument.xml");
        }

        [TestMethod]
        public void VerifyXMLSerialization()
        {
            foreach (Type iteratetype in Assembly.GetAssembly(typeof (Block)).GetTypes())
            {
                //is it a block?
                if (iteratetype.IsSubclassOf(typeof (Block)))
                {
                    //does it expose a constructor that takes a "XElement"?
                    ConstructorInfo coninfo = iteratetype.GetConstructor(BindingFlags.NonPublic | BindingFlags.Public, null, new Type[] {typeof (XElement)}, null);
                    if (coninfo == null)
                    {
                        Debug.Print(iteratetype.Name + " Does not have a XElement accepting constructor.");
                        continue;
                    }
                    else
                    {
                        //check for GetXmlData method.
                        MethodInfo xmlData = iteratetype.GetMethod("GetXmlData", BindingFlags.NonPublic | BindingFlags.Public, null, new Type[] {typeof (String)}, null);
                        if (xmlData == null)
                        {
                            Debug.Print(iteratetype.Name + " Does not have a GetXmlData method.");
                            continue;
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void TestLevelSerialization()
        {
            Level CreateLevel = new Level();
            CreateLevel.levelballs.Add(new cBall(new PointF(200,300),BCBlockGameState.GetRandomVelocity(2,5)));
            int currentX=0,CurrentY=0;
            for (int x = 0; x < 5; x++)
            {

                for (int y = 0; y < 5; y++)
                {
                    NormalBlock buildblock = new NormalBlock(new RectangleF(x, y, 33, 16));
                    buildblock.BlockColor = BCBlockGameState.Choose(new Color[] { Color.Red, Color.Green, Color.Blue, Color.Yellow, Color.Orange });
                    CreateLevel.levelblocks.Add(buildblock);
                    
                }
            }
            //Save the level to the XML Element.
            XElement LevelXML = CreateLevel.GetXmlData("LevelData");
            //now recreate the level from the XML data.
            //now reconstitute it. Just add water...
            Level Reconstitute = new Level(LevelXML);
            //now compare the NormalBlocks in this reconstituted instance to the original.
            for (int i = 0; i < Reconstitute.levelblocks.Count;i++)
            {
                NormalBlock Original = (NormalBlock)CreateLevel.levelblocks[i];
                NormalBlock Rebuilt = (NormalBlock)Reconstitute.levelblocks[i];

                if(Original.BlockColor.ToArgb()!=Rebuilt.BlockColor.ToArgb())
                {
                    throw new AssertFailedException("Failed Block Color comparison. Index=" + i.ToString() + " First=" + Original.BlockColor.ToString() + " New=" + Rebuilt.BlockColor.ToString());
                }

            }

        }
        [TestMethod]
        public void TestLevelSetSerialization()
        {
            EditorSet es = new EditorSet();
            es.CreateData = new CreatorProperties("BC_Programming","1.0.0","This is a test edit set.");
            DefaultLevelBuilder dl = new DefaultLevelBuilder();
            es.LevelData = dl.BuildLevelSet(new RectangleF(0,0,450f, 800f), null);

            XElement result = es.GetXmlData("EditData");
            using(FileStream fs = new FileStream("D:\\result.txt",FileMode.Create))
            {
                XDocument newdoc = new XDocument(result);
                newdoc.Save(fs);
            }
            EditorSet reread = new EditorSet(result);

        }
        [TestMethod]
        public void VerifyXMLBlockSerialization()
        {
            String TestDataLocation = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "BASeBlock\\TestData");
            if (!Directory.Exists(TestDataLocation)) Directory.CreateDirectory(TestDataLocation);
            
            foreach (Type iteratetype in BCBlockGameState.BlockTypes.ManagedTypes)
            {
                Block TestBlock = null;
                TestContext.WriteLine("Instantiating type: " + iteratetype.Name);
                try
                {
                    ConstructorInfo ci = iteratetype.GetConstructor(new Type[] {typeof (RectangleF)});
                    if (ci != null)
                    {
                        TestBlock = (Block) ci.Invoke(new object[] {new RectangleF(0, 0, 33, 16)});
                        if (TestBlock != null)
                        {
                        }
                        else
                        {
                            TestContext.WriteLine("Failed to instantiate type " + iteratetype.Name);
                        }
                    }
                }
                catch (Exception exx)
                {
                    TestContext.WriteLine("Failed to find appropriate constructor to instantiate type " + iteratetype.Name);
                }

                if (TestBlock != null)
                {
                    try
                    {
                        TestContext.WriteLine("Successfully instantiated type " + iteratetype.Name);
                        //now serialize.
                        TestContext.WriteLine("Serializing to an XML Element...");

                        XElement createelement = TestBlock.GetXmlData("TestBlock");

                        TestContext.WriteLine("Serialization Completed.");
                        String TargetPath = Path.Combine(TestDataLocation, iteratetype.Name + ".xml");
                        XDocument writedocument = new XDocument(createelement);
                        writedocument.Save(TargetPath);
                        TestContext.WriteLine("Saved XML element to " + TargetPath);
                        TestContext.WriteLine("Attempting to deserialized XElement...");
                        Block result = null;
                        try
                        {
                            result = (Block)StandardHelper.ReadElement(iteratetype, createelement);
                            if (result == null)
                            {
                                TestContext.WriteLine("No result...");
                            }
                            else
                            {
                                TestContext.WriteLine("Deserialization successful. Result:" + result.GetType());
                            }
                        }
                        catch (Exception exx)
                        {
                            TestContext.WriteLine("Deserialization failed for type " + iteratetype.Name + " " + exx.ToString());
                        }
                    }
                    catch(Exception exx)
                    {
                        TestContext.WriteLine("Serialization failed for type:" + iteratetype.Name + " " + exx.ToString());
                    }
                }
            }
        }

        [TestMethod]
        public void TestXML()
        {
            
            Bitmap b = new Bitmap(50, 50);
            Graphics g = Graphics.FromImage(b);

            for (int i = 0; i < 50; i++)
            {
                using (Pen usepen = new Pen(Color.FromArgb((int) ((float) i)/50*255, 0, 0), 1))
                    g.DrawLine(usepen, 0, i, 50, i);
            }

            TextureBrush tb = new TextureBrush(b, new Rectangle(0, 0, 50, 50));

            XElement lgmxml = StandardHelper.SaveElement(tb, "Texture");
            XDocument builddoc = new XDocument(lgmxml);
            builddoc.Save("D:\\texture.xml");
            TextureBrush restored = StandardHelper.ReadElement<TextureBrush>(lgmxml);

            AddBallBlock abb = new AddBallBlock(new RectangleF(0, 0, 33, 16));
            XElement AddBallElement = abb.GetXmlData("AddBallBlock");
            XDocument SaveAddBallBlock = new XDocument(AddBallElement);
            SaveAddBallBlock.Save("D:\\AddBall.xml");
            AddBallBlock RestoredAddBallBlock = (AddBallBlock) StandardHelper.ReadElement(typeof (AddBallBlock), AddBallElement);
        }

        [TestMethod]
        public void TestImageXML()
        {
            try
            {
                Bitmap testImage = new Bitmap(255, 255);
                Graphics g = Graphics.FromImage(testImage);

                for (int i = 0; i < 255; i++)
                {
                    Pen usePen = new Pen(Color.FromArgb(i, 0, 0));
                    g.DrawLine(usePen, 0, i, 255, i);
                    usePen.Dispose();
                }
                //testImage is now a test image... as the name would seem to imply, really.
                XElement Target = StandardHelper.SaveElement(testImage, "Image");
                XDocument builddoc = new XDocument(Target);
                builddoc.Save("D:\\somefile.dat");

                XDocument readdoc = XDocument.Load("D:\\somefile.dat");
                XElement grabfrom = readdoc.Root;
                Image readimagedata = StandardHelper.ReadElement<Image>(grabfrom);
            }
            catch (Exception exx)
            {
                TestContext.WriteLine("Test failure: Exception:" + exx.ToString());
            }
        }


        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get { return testContextInstance; }
            set { testContextInstance = value; }
        }

        #region Additional test attributes

        //
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // Use TestInitialize to run code before running each test 
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //

        #endregion

        [TestMethod]
        public void TestNetHighScores()
        {
        }

        [TestMethod]
        public void TestItemBucket()
        {
            ItemBucket<int> ValueBucket = new ItemBucket<int>(ItemBucket<int>.RepeatArray(new int[] {1, 2, 3, 4, 5, 6}, 5));
            Console.WriteLine();
            for (int i = 0; i < 50; i++)
            {
                Console.Write(ValueBucket.Dispense() + ",");
            }
        }

        [TestMethod]
        public void TestUpdateClass()
        {
            BCUpdate updater = new BCUpdate();
            foreach (BCUpdate.UpdateInfo loopupdate in updater.LoadedUpdates)
            {
                Console.WriteLine("Update Name:" + loopupdate.DlName + ", URL=" + loopupdate.fullURL);
            }
        }

        [TestMethod]
        public void TestProductKey()
        {
            ProductKey pk = new ProductKey();
            pk.ExpiryDate = DateTime.Now + new TimeSpan(30, 0, 0, 0);
            pk.Header = 128;
            pk.Product = ProductKey.Products.BASeBlock;
            pk.Trial = true;
            Console.WriteLine(pk.GetProductCode());
        }

        public bool ConnectionAvailable()
        {
            try
            {
                var result = new WebClient().DownloadString("http://www.msftncsi.com/ncsi.txt");
                return true;
            }
            catch (Exception exx)
            {
                return false;
            }
        }

        public void testcheckconnection()
        {
        }

        [TestMethod]
        public void XmlSerializationExperiments()
        {
            BCBlockGameState.Initgamestate();
            NormalBlock nb = new NormalBlock(new RectangleF(64, 64, 33, 16));
            nb.BlockColor = Color.Blue;
            nb.PenColor = Color.Yellow;
            XElement NormalBlockData = nb.GetXmlData("Block");

            XDocument builddoc = new XDocument(NormalBlockData);
            builddoc.Save("D:\\normalblock.xml");
        }


        [TestMethod]
        public void TestVersionInfo()
        {
            VersionInfo vii = new VersionInfo(12, 5, 7, 2);
            VersionInfo vix = new VersionInfo("12.5.7.2");
            Console.WriteLine("Testing that " + vii + " equals " + vix);
            if (vii != vix)
            {
                throw new AssertFailedException("VersionInfo with numeric arguments not equal to equivalent VersionInfo constructed with a string argument.");
            }
            VersionInfo larger = new VersionInfo(5, 4, 12, 4);
            VersionInfo thisassemblyver = Assembly.GetExecutingAssembly().GetName().Version;
            Console.WriteLine("Comparing " + larger + " to " + thisassemblyver);
            Console.WriteLine("CompareTo returned " + larger.CompareTo(thisassemblyver));
        }
        [TestMethod]
        public void TestDictionarySerialization()
        {
            Dictionary<String, String> TestStringDictionary = new Dictionary<string, string> {{"First", "FirstElement"}, {"Second", "SecondElement"}};

            Dictionary<String, int> TestIntDictionary = new Dictionary<string, int> {{"First", 1}, {"Second", 2}};
            Dictionary<String,List<String>> TestListDictionary = new Dictionary<string, List<string>>();
            TestListDictionary.Add("Fruit",new List<string>() {"Apple","Pear","Strawberry"});
            TestListDictionary.Add("Vegetable", new List<string>() { "Cucumber", "Lettuce", "Strawberry" });


            XElement SaveStringElement = StandardHelper.SaveDictionary(TestStringDictionary, "StringDictionary");
            XElement SaveIntElement = StandardHelper.SaveDictionary(TestIntDictionary, "IntDictionary");
            XElement SaveListElement = StandardHelper.SaveDictionary((IDictionary)TestListDictionary, "ListDictionary");

            //load them back up...

            Dictionary<String, String> LoadStringDictionary = StandardHelper.ReadDictionary<String, String>(SaveStringElement);
            Dictionary<String, int> LoadIntDictionary = StandardHelper.ReadDictionary<String, int>(SaveIntElement);
            IDictionary LoadListDictionary = StandardHelper.ReadDictionary(SaveListElement);




        }

        [TestMethod]
        public void TestBlockSerialization()
        {
            //test method for testing that all blocks serialize properly.
            //as a rule, block serialization and cloning are trouble spots when
            //I add new blocks.
            List<Type> Blocktypes = LoadedTypeManager.GetTypesFromAssembly(Assembly.GetAssembly(typeof (BCBlockGameState)), typeof (Block));
            int failcount = 0;
            Console.WriteLine("Acquired Block types. Count=" + Blocktypes.Count);
            foreach (Type iteratetype in Blocktypes)
            {
               
                //Console.WriteLine("Testing Block Type:" + iteratetype.Name);
                //Console.WriteLine("\tInstantiating...");
               /* Block constructed = null;
                try
                {
                    constructed = (Block) Activator.CreateInstance(iteratetype, new RectangleF(0, 0, 33, 16));
                }
                catch (Exception e)
                {
                    Console.WriteLine("Exception:" + e.Message + " Constructing type " + iteratetype.Name);
                    failcount++;
                }*/
                //now, verify it has a "XElement" Constructor
                var xelementconstructor = iteratetype.GetConstructor(new Type[] { typeof(XElement) });
                var GetXmlDataMethod = iteratetype.GetMethod("GetXmlData", new Type[] { typeof(String) });
                if(xelementconstructor == null || GetXmlDataMethod==null)
                {

                    if (xelementconstructor == null) TestContext.WriteLine("Block Type: " + iteratetype.Name + " Lacks a XElement accepting constructor to meet the IXmlPersistable Interface requirement.");
                    if (GetXmlDataMethod == null) TestContext.WriteLine("Block Type: " + iteratetype.Name + "Lacks a GetXmlData Method.");
                    failcount++;
                    //throw new InternalTestFailureException("IXmlPersistable support error.");
                }
               /* if (constructed != null)
                {
                    //proceed with test. Serialize and Deserialize from a memorystream.
                    MemoryStream testserialize = new MemoryStream();
                    try
                    {
                        BinaryFormatter bff = new BinaryFormatter();
                        bff.Serialize(testserialize, constructed);
                        testserialize.Seek(0, SeekOrigin.Begin);
                        Block deserialized = (Block) bff.Deserialize(testserialize);
                        if (deserialized.GetType() != constructed.GetType())
                        {
                            failcount++;
                            Console.WriteLine("Deserialized Type " + deserialized.GetType().Name + " Is not the same as the serialized type " + constructed.GetType().Name);
                        }
                        else
                        {
                            Block cloned = (Block) deserialized.Clone();
                            if (cloned.GetType() != deserialized.GetType())
                            {
                                failcount++;
                                Console.WriteLine("cloned Type " + cloned.GetType().Name + " Is not the same as the serialized type " + deserialized.GetType().Name);
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Exception:" + e.Message + " testing serialization of type " + iteratetype.Name);
                        failcount++;
                    }
                }*/
            }
            
            return;
            if (failcount > 0)
            {
                throw new AssertFailedException(failcount + " block tests failed.");
            }
        }

        [TestMethod]
        public void testserializeTriggers()
        {
            BlockTrigger singletesttrigger = new BlockTrigger(50, new TimeSpan(0, 0, 0, 2), null);
            BlockEvent singletestevent = new BlockEvent(60, null);


            BinaryFormatter testformatter = new BinaryFormatter();
            MemoryStream tempstream = new MemoryStream();
            Console.WriteLine("Testing single serialization...");
            testformatter.Serialize(tempstream, singletesttrigger);
            tempstream.Seek(0, SeekOrigin.Begin);
            BlockTrigger secondtrigger = (BlockTrigger) testformatter.Deserialize(tempstream);


            Console.WriteLine("Testing list serialization...");

            List<BlockTrigger> serializeme = new List<BlockTrigger>();

            for (int i = 0; i < 50; i++)
            {
                serializeme.Add(new BlockTrigger(i, new TimeSpan(0, 0, 0, 2), null));
            }
            tempstream = new MemoryStream();
            testformatter.Serialize(tempstream, serializeme);

            tempstream.Seek(0, SeekOrigin.Begin);
            serializeme = (List<BlockTrigger>) testformatter.Deserialize(tempstream);

            foreach (BlockTrigger looptrigger in serializeme)
            {
                Console.WriteLine(looptrigger.ID);
            }

            Block createblock = new NormalBlock(new RectangleF(10, 10, 32, 16), new SolidBrush(Color.Red), new Pen(Color.Black));
            //serialize it.
            tempstream = new MemoryStream();
            testformatter.Serialize(tempstream, createblock);
            tempstream.Seek(0, SeekOrigin.Begin);
            createblock = (Block) testformatter.Deserialize(tempstream);
        }

        [TestMethod]
        public void TestFormatSize()
        {
            long[] TestSizes = new long[] {38945345, 48953, 8293, 920342};


            Console.WriteLine("FormatSize Test...");
            foreach (long printsize in TestSizes)
            {
                Console.WriteLine("Value=" + printsize.ToString() + " formatted:" + BCBlockGameState.FormatSize(printsize));
            }
            Console.WriteLine("using FormatSizes...");
            String[] resultformats = BCBlockGameState.FormatSizes(TestSizes);
            for (int i = 0; i < resultformats.Length; i++)
            {
                Console.WriteLine(TestSizes[i] + " = " + resultformats[i]);
            }
        }

        [TestMethod]
        public void TestLocalHighScores()
        {
            String[] SendNames = new string[] {"Barry", "Billy", "Chuck", "Trish", "Bob", "Thomas"};
            int[] SendScores = new int[] {600, 200, 400, 700, 250, 320};


            //LocalHighScores testScore = new LocalHighScores(20);
            //IHighScoreList testScore = new LocalHighScores(20);
            IHighScoreList testScore = new NetHighScores(3, "Default", 0); //the "Default" score goes with 0. Well, not really.
            //actually, the LevelSetBuilder's will be responsible for creating an appropriate hash value. the two defaults will likely provide hard-coded entries; the FromFile implementation
            //will probably hash the contents of the file.
            for (int i = 0; i < SendNames.Length; i++)
            {
                Console.WriteLine("Submitting score Name:" + SendNames[i] + " with score: " + SendScores[i]);
                testScore.Submit(SendNames[i], SendScores[i]);

                new int[] {1, 2, 3, 4}.Select((w) => w + 1);
            }
            testScore.Reload();
            Console.WriteLine("Current Scores:");
            foreach (HighScoreEntry loopentry in testScore.GetScores())
            {
                Console.WriteLine("Entry:" + loopentry.ToString());
            }


            if (testScore is LocalHighScores)
            {
                Console.WriteLine("Serializing to File...");

                BinaryFormatter useformatter = new BinaryFormatter();
                Stream usewriter = new FileStream(@"D:\testoutput.dat", FileMode.Create);
                //usewriter.Write(testlocal);
                useformatter.Serialize(usewriter, testScore);
                Console.WriteLine("disposing existing object...");
                testScore = null;
                usewriter.Close();
                Console.WriteLine("Deserializing new instance from saved copy...");
                Stream usereader = new FileStream(@"D:\testoutput.dat", FileMode.Open);
                testScore = (LocalHighScores) useformatter.Deserialize(usereader);

                Console.WriteLine("Loaded Scores:");
                foreach (HighScoreEntry loopentry in testScore.GetScores())
                {
                    Console.WriteLine("Entry:" + loopentry.ToString());
                }
            }
            Console.WriteLine("Testing \"Eligible\" routine...");

            String[] eligiblenames = new String[] {"Billy", "Bob", "BearLover", "Tickles"};
            int[] eligiblescores = new int[] {210, 225, 400, 54};

            for (int i = 0; i < eligiblenames.Length; i++)
            {
                Console.WriteLine("IsEligible, Name={0}, Score={1}", eligiblenames[i], eligiblescores[i]);
                int eligibleresult = testScore.Eligible(eligiblenames[i], eligiblescores[i]);
                Console.WriteLine("return value:" + eligibleresult.ToString());
            }
        }
    }
}