using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Design;
using System.Drawing.Drawing2D;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;
using bcHighScores;

//using System.Windows.Markup;
//using System.Windows.Media;
//using System.Xml.Serialization;

namespace BASeBlock
{
    
    [Serializable] 
    public class LevelSet : ISerializable
    {
        public static string DefaultSetName = "Default Set";
        private String _SetName = DefaultSetName;
        
        public String SetName { get { return _SetName; } set { _SetName = value; } }

            public List<Level> Levels = new List<Level>();
        public ObjectPathDataManager PathData = new ObjectPathDataManager();
        [Editor(typeof(BASeBlock.ObjectTypeEditor), typeof(UITypeEditor))]
        
        public bcHighScores.LocalHighScores HighScores {


            get { return BCBlockGameState.Scoreman[SetName]; }
            set { BCBlockGameState.Scoreman[SetName] = value; }
            
        }
        public GameStatistics Statistics
        {
            get { return BCBlockGameState.Statman[SetName]; }
            set { BCBlockGameState.Statman[SetName] = value; }

        }
        public string GetMD5Hash(string input)
        {
            System.Security.Cryptography.MD5CryptoServiceProvider x = new System.Security.Cryptography.MD5CryptoServiceProvider();
            byte[] bs = System.Text.Encoding.UTF8.GetBytes(input);
            bs = x.ComputeHash(bs);
            System.Text.StringBuilder s = new System.Text.StringBuilder();
            foreach (byte b in bs)
            {
                s.Append(b.ToString("x2").ToLower());
            }
            string password = s.ToString();
            return password;
        }
        public byte[] getserialbytes()
        {
            //simply serializes this object and returns the byte from that array.
            MemoryStream ms = new MemoryStream();
            IFormatter bf = BCBlockGameState.getFormatter<LevelSet>(BCBlockGameState.DataSaveFormats.Format_Binary);
            bf.Serialize(ms, this);
            //long locate = ms.Position;
            ms.Seek(0, SeekOrigin.Begin);
            byte[] returnit = new byte[(int)ms.Length];
            
            ms.Read(returnit, 0, (int)ms.Length);

                return returnit;

        }

        public String MD5Hash()
        {
            MD5CryptoServiceProvider MD5obj = new MD5CryptoServiceProvider();


            byte[] bytes = getserialbytes();
            byte[] bs = MD5obj.ComputeHash(bytes);
            StringBuilder sb = new StringBuilder();
            foreach (byte b in bs)
            {
                sb.Append(b.ToString("x2").ToLower());
            }

            return sb.ToString();


        }
        public static LevelSet FromBuilder(iLevelSetBuilder buildfrom,IWin32Window Owner)
        {
            return FromBuilder(buildfrom, new RectangleF(PointF.Empty, BCBlockGameState.DefaultLevelSize),Owner);

        }

        public static LevelSet FromBuilder(iLevelSetBuilder buildfrom,RectangleF targetrect,IWin32Window Owner)
        {
            
            return buildfrom.BuildLevelSet(targetrect,Owner);


        }
        
        public static List<iLevelSetBuilder> getLevelBuildersFromFolder(DirectoryInfo fromfolder,SearchOption soption)
        {
            List<iLevelSetBuilder> returnlist = new List<iLevelSetBuilder>();
            foreach (FileInfo loopfile in fromfolder.GetFiles("*.dll", soption))
            {
                try
                {
                    Assembly tryloadassembly = Assembly.LoadFrom(loopfile.FullName);
                    List<iLevelSetBuilder> tempreturnfrom = getLevelBuildersFromAssembly(tryloadassembly);
                    if (tempreturnfrom.Count > 0)
                        returnlist.AddRange(tempreturnfrom.ToArray());


                }
                catch (Exception e)
                {
                    //ignore, simply an incompatible type.


                }




            }

            return returnlist;


        }

        /// <summary>
        /// loads valid iLevelSetBuilder implementors from the given assembly.
        /// </summary>
        /// <param name="assemblyName"></param>
        /// <returns></returns>
        public static List<iLevelSetBuilder> getLevelBuildersFromAssembly(Assembly fromAssembly)
        {
            List<iLevelSetBuilder> listreturn = new List<iLevelSetBuilder>();
            foreach (Type looptype in fromAssembly.GetTypes())
            {

                if ((looptype.Attributes & TypeAttributes.Class) == TypeAttributes.Class)
                {
                    foreach (Type loopinterface in looptype.GetInterfaces())
                    {
                        if (loopinterface == typeof(iLevelSetBuilder))
                        {

                            listreturn.Add((iLevelSetBuilder)loopinterface);

                        }


                    }



                }


            }
            return listreturn;

        }
        public static List<iLevelSetBuilder> getLevelBuildersFromAssembly(String assemblyname)
        {

            return getLevelBuildersFromAssembly(Assembly.LoadFrom(assemblyname));

        }
        //the following are commented out: level saving and loading to/from files is done via the EditorSet now.
        /*
        public static LevelSet FromFile(String filename)
        {
            
            IFormatter binformatter = new BinaryFormatter();
         
                using (FileStream inputstream = new FileStream(filename, FileMode.Open))
                {
                    using (GZipStream gzstream = new GZipStream(inputstream, CompressionMode.Decompress))
                    {
                        LevelSet returnthis = (LevelSet) binformatter.Deserialize(gzstream);
                        return returnthis;
                    }
                }
            
           

        }
       

        public void Save(String filename)
        {
            FileStream outstream=null;
            IFormatter binformatter = new BinaryFormatter();
            try
            {
                
                outstream = new FileStream(filename, FileMode.OpenOrCreate);
                
                //create a GZip stream...
                using (GZipStream gzstream = new GZipStream(outstream, CompressionMode.Compress))
                {


                    Cursor.Current = Cursors.WaitCursor;
                    binformatter.Serialize(gzstream, this);
                    Cursor.Current = Cursors.Default;
                }


            }
            catch (Exception e)
            {
                Debug.Print("Exception in LevelSet.Save:" + e.Message);



            }
            finally
            {
                if(outstream!=null) outstream.Close();

            }








        }
         * */
        public LevelSet()
        {
            HighScores = new LocalHighScores(20);
            GraphicsPath testpath = new GraphicsPath();
            //testpath.AddString("Testing", new Font("Celestia Redux", 28), new Point(90, 32), StringFormat.GenericDefault);
            //testpath.Flatten();
            testpath.AddRectangle(new Rectangle(90,32,84,84));
            //PathData["testing path"] = new ObjectPathData(testpath);

            
        }

        public LevelSet(SerializationInfo info,StreamingContext ContextBoundObject):this()
            {
                SetName = info.GetString("Name");
                Levels = (List<Level>)info.GetValue("Levels", typeof(List<Level>));
                try { PathData = (ObjectPathDataManager)info.GetValue("PathData", typeof(ObjectPathDataManager)); }catch{PathData = new ObjectPathDataManager();}
                Debug.Print("Loaded " + PathData.Count() + " Paths...");
            //HighScores = (LocalHighScores)info.GetValue("HighScores", typeof(LocalHighScores));


            }

        #region ISerializable Members

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            //TODO: keep track of a single table of block images so that the file size doesn't bloat up like Oprah Winfrey every second month.
            info.AddValue("Name", SetName);
                info.AddValue("Levels",Levels);
                Debug.Print("Saving " + PathData.Count() + " Paths...");
                info.AddValue("PathData", PathData);
                
               // info.AddValue("HighScores", HighScores);



        }

        #endregion
    }
}
