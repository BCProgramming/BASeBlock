using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
/*
using Jurassic;
using Jurassic.Compiler;
using Jurassic.Library;

namespace BASeCamp.BASeBlock
{

    public class JSMultiData : ManyToOneBlockData
    {

        //public JintEngine je;
        public Jurassic.ScriptEngine je;
        public String ScriptCode;
        public JavascriptBlock.InstantiationData instancedata;
        public JSMultiData(Type ownerblock, String displaytext, Image drawimage, Jurassic.ScriptEngine pje, String pscriptcode, JavascriptBlock.InstantiationData _instance)
            : base(ownerblock, displaytext, drawimage)
        {
            je = pje;
            ScriptCode = pscriptcode;
            instancedata = _instance;

        }


    }
    public class JavaScriptedBlockAttribute : BlockCategoryAttribute
    {



        public override string GetName()
        {
            return "JavaScript Blocks";
        }

        public override string GetDescription()
        {
            return "JavaScript blocks";
        }

        public override Image CategoryImage()
        {
            return BCBlockGameState.Imageman.getLoadedImage("jsicon");
        }
    }
    //invisible for now..
    //[BBEditorInvisible] 
    [ManyToOneBlock]
    [Serializable]
    [JavaScriptedBlock]
    
    public class JavascriptBlock : Block,IGameInitializer
    {
        public class InstantiationData:ICloneable 
        {
            public Jurassic.ScriptEngine JSEngine;
            public String ScriptCode;

            public InstantiationData(String pscriptcode, Jurassic.ScriptEngine jengine)
            {
                ScriptCode = pscriptcode;
                JSEngine = jengine;

            }
            public object Clone()
            {
                Jurassic.ScriptEngine newje = new Jurassic.ScriptEngine();
                newje.Execute(ScriptCode);
                try
                {
                    newje.CallGlobalFunction("scriptload", new Nullcallback());
                   // newje.CallFunction("scriptload", new Nullcallback());
                }
                catch(Exception er)
                {

                }
                return new InstantiationData(ScriptCode,newje);


            }


        }
        public InstantiationData ourinstancedata = null;
        private static List<InstantiationData> scriptdata = new List<InstantiationData>();
        public static Jurassic.ScriptEngine Createjint()
        {
            Jurassic.ScriptEngine je = new Jurassic.ScriptEngine();
            
            
            return je;
        }
        public static void GameInitialize(iManagerCallback datahook)
        {
            //TODO: load scripts here.
            String ScriptsFolder = Path.Combine(BCBlockGameState.AppDataFolder, "Scripts");
            datahook.ShowMessage("JS: searching for Javascript files in " + ScriptsFolder);

            DirectoryInfo grabdir = new DirectoryInfo(ScriptsFolder);
            foreach (FileInfo jsFile in grabdir.GetFiles("*.js"))
            {
                datahook.ShowMessage("Found javascript File:" + jsFile.Name);

                //load in the script from disk.
                try
                {
                    using (FileStream readscript = new FileStream(jsFile.FullName,FileMode.Open))
                    {

                        StreamReader sr = new StreamReader(readscript);
                        String script = sr.ReadToEnd();
                        Jurassic.ScriptEngine je = Createjint();
                        je.Execute(script);
                        try
                        {
                            datahook.ShowMessage("Running JS scriptload function...");
                            je.CallGlobalFunction("scriptload", datahook);
                        }
                        catch (Exception jscall)
                        {


                        }
                        scriptdata.Add(new InstantiationData(script,je));
                    }


                }
                catch (IOException ee)
                {

                    datahook.ShowMessage("error reading script file " + jsFile.FullName);

                }
            }


        }
        public static ManyToOneBlockData[] GetManyToOneData()
        {
           //
           //use data structures initialized in GameInitialize() to create the array.
            //For example, the js code loaded could have functions for DisplayName and stuff.
            List<ManyToOneBlockData> createlist = new List<ManyToOneBlockData>();
            foreach (InstantiationData iterateval in scriptdata)
            {
                try
                {
                    String useText = (String)iterateval.JSEngine.CallGlobalFunction("GetName");
                    createlist.Add(new JSMultiData(typeof(JavascriptBlock), useText, null,iterateval.JSEngine,iterateval.ScriptCode,iterateval));
                }
                    catch(Exception ee)
                    {

                    }

            }

            return createlist.ToArray();
            
        }
        public static Block Instantiate(ManyToOneBlockData Fromdata, RectangleF userectangle)
        {
            return new JavascriptBlock(((JSMultiData)Fromdata).instancedata,userectangle);
            

            return null;

        }
        public JavascriptBlock(RectangleF br)
        {
            BlockRectangle = br;

        }
       
        private bool _Redraw = true;
        public bool Redraw { get { return _Redraw; } set { _Redraw = value; } }
        //DoDraw is set to false when we draw. the script will have
        //to set it if the Draw routine is to be called again.
        //this is mostly for the editor.
        public JavascriptBlock(InstantiationData id,RectangleF userectangle):this(userectangle)
        {

            ourinstancedata = (InstantiationData)id.Clone();

            ourinstancedata.JSEngine = Createjint();
            ourinstancedata.JSEngine.SetGlobalValue("Self", this);
            ourinstancedata.JSEngine.Execute(ourinstancedata.ScriptCode);
  
            
            

        }
        public JavascriptBlock(SerializationInfo info, StreamingContext context)
        {
            String savedcode = info.GetString("Code");
            ourinstancedata.JSEngine = Createjint();
            ourinstancedata.JSEngine.SetGlobalValue("Self", this);
            

            ourinstancedata.JSEngine.CallGlobalFunction("blockinit", this);
        }
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            //save
            info.AddValue("Code", ourinstancedata.ScriptCode);


        }
        public JavascriptBlock(JavascriptBlock copyblock):base(copyblock)
        {
            ourinstancedata = (InstantiationData)copyblock.ourinstancedata.Clone();
            ourinstancedata.JSEngine.CallGlobalFunction("blockinit", this);

        }

        public override void Draw(Graphics g)
        {
           // if (!Redraw) return;
            //call javascript...
            ourinstancedata.JSEngine.SetGlobalValue("self", this);
            ourinstancedata.JSEngine.CallGlobalFunction("draw", g);
            
            //g.DrawRectangle(new System.Drawing.Pen(System.Drawing.Color.Yellow), this.BlockRectangle.Left,this.BlockRectangle.Top,this.BlockRectangle.Width,this.BlockRectangle.Height);
           // Redraw = false; //set so we don't constantly call it. (again,mostly for the editor...
            
            
            

        }

        public override bool PerformBlockHit(BCBlockGameState parentstate, cBall ballhit, ref List<cBall> ballsadded)
        {
            ourinstancedata.JSEngine.SetGlobalValue("self", this);
            bool returnvalue = (bool)ourinstancedata.JSEngine.CallGlobalFunction("PerformBlockHit", parentstate, ballhit, ballsadded);


            return returnvalue;
        }

        public override bool MustDestroy()
        {
            
            return (bool)ourinstancedata.JSEngine.CallGlobalFunction("MustDestroy",new object[0]);


        }

        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>
        /// A new object that is a copy of this instance.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public override object Clone()
        {
            return new JavascriptBlock(this);
        }
    }
}
*/