
/*
 * BASeCamp BASeBlock
Copyright (c) 2011, Michael Burgwin
All rights reserved.

Redistribution and use in source and binary forms, with or without modification, are permitted provided that the following conditions are met:

    Redistributions of source code must retain the above copyright notice, this list of conditions and the following disclaimer.
    Redistributions in binary form must reproduce the above copyright notice, this list of conditions and the following disclaimer in the documentation and/or other materials provided with the distribution.
    Neither the name of BASeCamp Corporation nor the names of its contributors may be used to endorse or promote products derived from this software without specific prior written permission.

THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
 * */

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Design;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.BASeCamp;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization.Formatters.Soap;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using System.Windows;
using BASeBlock.Blocks;
using BASeBlock.Events;
using BASeBlock.GameObjects.Orbs;
using BASeBlock.PaddleBehaviours;
using BASeBlock.Particles;
using BASeCamp.Configuration;
using BASeBlock.Templates;
using Img;
using bcHighScores;
using Ionic.Zip;
using Ionic.Zlib;
using BASeCamp.Licensing;
using Microsoft.JScript;
using Microsoft.WindowsAPICodePack.Shell.PropertySystem;
using Block = BASeBlock.Blocks.Block;
using Convert = System.Convert;
using Point = System.Drawing.Point;
using Rectangle = System.Drawing.Rectangle;

namespace BASeBlock
{


    
    


    public interface IMovingObject
    {
        PointF Velocity { get; set; }


    }

    //GameInitializer: Used to indicate presence, in the class, of a static routine "GameInitialize()" 
    //which is called at the tail of the BCBlockGameState initGameState(). this is necessary because
    //static constructors won't have access to BCBlockGameState's static methods since it will have to have had the initialize routine called first; this makes 
    //sure it is valid and those functions are called.
    /// <summary>
    /// Used to indicate presence of Static "GameInitialize(iManagerCallback datahook)" routine to be called at the tail of BCBlockGameState.initGameState().
    /// </summary>
    public interface IGameInitializer
    {

    }
    //used to indicate presence of static 
    public interface IBindingExtension
    {

    }

    public static class ColorMatrices
    {

        private static float[][] _colorMatrixElements = { 
   new float[] {1,  0,  0,  0, 0},        // red scaling factor of 2
   new float[] {0,  1,  0,  0, 0},        // green scaling factor of 1
   new float[] {0,  0,  2,  0, 0},        // blue scaling factor of 1
   new float[] {0,  0,  0,  1, 0},        // alpha scaling factor of 1
   new float[] {-0.5f, -0.5f, .8f, 0, 1}};    // three translations of 0.2

        public static ColorMatrix GetFader(float alpha)
        {

            return new ColorMatrix(new float[][] {
                new float[]{1,0,0,0,0},
                new float[]{0,1,0,0,0},
                new float[]{0,0,1,0,0},
                new float[]{0,0,0,alpha,0},
                new float[]{0,0,0,0,1}});



        }

        public static ColorMatrix GetColourizer(Color fromcolor)
        {
            return GetColourizer(fromcolor.R, fromcolor.G, fromcolor.B,fromcolor.A);
            
            

        }
        public static ColorMatrix GetColourizer(float red, float green, float blue)
        {
            return GetColourizer(red, green, blue, 1);

        }
        public static void AddColourizer(ImageAttributes toia,Color usecolor)
        {
            ColorMatrix grayscaler = GrayScale();
            toia.SetColorMatrices(GetColourizer(usecolor), grayscaler);



        }


        public static ColorMatrix GrayScale()
        {
            return new ColorMatrix(
      new float[][]
      {
         new float[] {.3f, .3f, .3f, 0, 0},
         new float[] {.59f, .59f, .59f, 0, 0},
         new float[] {.11f, .11f, .11f, 0, 0},
         new float[] {0, 0, 0, 1, 0},
         new float[] {0, 0, 0, 0, 1}
      });

        }
        public static float[][] GetIdentity()
        {
            return new float[][]
      {
         new float[] {1, 0, 0, 0, 0},
         new float[] {0, 1, 0, 0, 0},
         new float[] {0, 0, 1, 0, 0},
         new float[] {0, 0, 0, 1, 0},
         new float[] {0, 0, 0, 0, 1}
      };

        }
        public static ColorMatrix GetRedColourizer(float red, float green, float blue, float alpha)
        {
            float[][] matElement =  { 
   new float[] {red,  0,  0,  0, 0},        //red scaling factor
   new float[] {0,  green,  0,  0, 0},        // green scaling factor of 1
   new float[] {0,  0,  blue,  0, 0},        // blue scaling factor of 1
   new float[] {0,  0,  0,  alpha, 0},        // alpha scaling factor of 1
   new float[] {0, 0, 0, 0, 1}};    // three translations of 0.2
            //change the appropriate elements to match....
            matElement[0][0] = red;
            matElement[1][1] = green;
            matElement[2][2] = blue;
            matElement[3][3] = alpha;
            matElement[4][4] = 1;
            return new ColorMatrix(matElement);


        }

        public static ColorMatrix GetColourizer(float red, float green, float blue,float alpha)
        {


            


            /*
            float[][] MatElement =  { 
   new float[] {red,  0,  0,  0, 0},        //red scaling factor
   new float[] {0,  green,  0,  0, 0},        // green scaling factor of 1
   new float[] {0,  0,  blue,  0, 0},        // blue scaling factor of 1
   new float[] {0,  0,  0,  alpha, 0},        // alpha scaling factor of 1
   new float[] {-0.5f, -0.5f, .8f, 0, alpha}};    // three translations of 0.2
             */
            float[][] matElement =  { 
   new float[] {red,  0,  0,  0, 0},        //red scaling factor
   new float[] {0,  green,  0,  0, 0},        // green scaling factor of 1
   new float[] {0,  0,  blue,  0, 0},        // blue scaling factor of 1
   new float[] {0,  0,  0,  alpha, 0},        // alpha scaling factor of 1
   new float[] {0, 0, 0, 0, 1}};    // three translations of 0.2
            //change the appropriate elements to match....
            matElement[0][0] = red;
            matElement[1][1] = green;
            matElement[2][2] = blue;
            matElement[3][3]=alpha;
            matElement[4][4] = 1;
            return new ColorMatrix(matElement);
            
        }


    }
#region HSLColor
    /// <summary>
    /// Class used to convert to and from Hue,Saturation, and Luminousity.
    /// </summary>
     public class HSLColor
     {
         // Private data members below are on scale 0-1
         // They are scaled for use externally based on scale
         private double _hue = 1.0;
         private double _saturation = 1.0;
         private double _luminosity = 1.0;
  
         private const double Scale = 240.0;
  
         public double Hue
         {
             get { return _hue * Scale; }
             set { _hue = CheckRange(value / Scale); }
         }
         public double Saturation
         {
             get { return _saturation * Scale; }
             set { _saturation = CheckRange(value / Scale); }
         }
         public double Luminosity
         {
             get { return _luminosity * Scale; }
             set { _luminosity = CheckRange(value / Scale); }
         }
  
         private double CheckRange(double value)
         {
             if (value < 0.0)
                 value = 0.0;
             else if (value > 1.0)
                 value = 1.0;
             return value;
         }
  
         public override string ToString()
         {
             return String.Format("H: {0:#0.##} S: {1:#0.##} L: {2:#0.##}",   Hue, Saturation, Luminosity);
         }
  
         public string ToRGBString()
         {
             Color color = (Color)this;
             return String.Format("R: {0:#0.##} G: {1:#0.##} B: {2:#0.##}", color.R, color.G, color.B);
         }
  
         #region Casts to/from System.Drawing.Color
         public static implicit operator Color(HSLColor hslColor)
         {
             double r = 0, g = 0, b = 0;
             if (hslColor._luminosity != 0)
             {
                 if (hslColor._saturation == 0)
                     r = g = b = hslColor._luminosity;
                 else
                 {
                     double temp2 = GetTemp2(hslColor);
                     double temp1 = 2.0 * hslColor._luminosity - temp2;
  
                     r = GetColorComponent(temp1, temp2, hslColor._hue + 1.0 / 3.0);
                     g = GetColorComponent(temp1, temp2, hslColor._hue);
                     b = GetColorComponent(temp1, temp2, hslColor._hue - 1.0 / 3.0);
                 }
             }
             return Color.FromArgb((int)(255 * r), (int)(255 * g), (int)(255 * b));
         }
  
         private static double GetColorComponent(double temp1, double temp2, double temp3)
         {
             temp3 = MoveIntoRange(temp3);
             if (temp3 < 1.0 / 6.0)
                 return temp1 + (temp2 - temp1) * 6.0 * temp3;
             else if (temp3 < 0.5)
                 return temp2;
             else if (temp3 < 2.0 / 3.0)
                 return temp1 + ((temp2 - temp1) * ((2.0 / 3.0) - temp3) * 6.0);
             else
                 return temp1;
         }
         private static double MoveIntoRange(double temp3)
         {
             if (temp3 < 0.0)
                 temp3 += 1.0;
             else if (temp3 > 1.0)
                 temp3 -= 1.0;
             return temp3;
         }
         private static double GetTemp2(HSLColor hslColor)
         {
             double temp2;
             if (hslColor._luminosity < 0.5)  //<=??
                 temp2 = hslColor._luminosity * (1.0 + hslColor._saturation);
             else
                 temp2 = hslColor._luminosity + hslColor._saturation - (hslColor._luminosity * hslColor._saturation);
             return temp2;
         }
  
         public static implicit operator HSLColor(Color color)
         {
             HSLColor hslColor = new HSLColor();
             hslColor._hue = color.GetHue() / 360.0; // we store hue as 0-1 as opposed to 0-360 
             hslColor._luminosity = color.GetBrightness();
             hslColor._saturation = color.GetSaturation();
             return hslColor;
         }
         #endregion
  
         public void SetRGB(int red, int green, int blue)
         {
             HSLColor hslColor = (HSLColor)Color.FromArgb(red, green, blue);
             this._hue = hslColor._hue;
             this._saturation = hslColor._saturation;
             this._luminosity = hslColor._luminosity;
         }
  
         public HSLColor() { }
         public HSLColor(Color color)
         {
             SetRGB(color.R, color.G, color.B);
         }
         public HSLColor(int red, int green, int blue)
         {
             SetRGB(red, green, blue);
         }
         public HSLColor(double hue, double saturation, double luminosity)
         {
             this.Hue = hue;
             this.Saturation = saturation;
             this.Luminosity = luminosity;
         }


        public static Color RandomHue(double useSat, double uselum)
        {
            return new HSLColor(new Random().NextDouble() * 240, useSat, uselum);
        }
     }
#endregion



    /// <summary>
    /// Used to retrieve key state info
    /// </summary>
    public class KeyboardInfo
    {
        private KeyboardInfo() { }

        [DllImport("user32.dll")]
        public static extern Int16 GetAsyncKeyState(int vKey);

        public static bool IsPressed(Keys key)
        {
            return GetAsyncKeyState((int)key) < 0;

        }

        [DllImport("user32")]
        private static extern short GetKeyState(int vKey);
        public static KeyStateInfo GetKeyState(Keys key)
        {
            short keyState = GetKeyState((int)key);
            byte[] bits = BitConverter.GetBytes(keyState);
            bool toggled = bits[0] == 1, pressed = bits[1] == 1;
            return new KeyStateInfo(key, pressed, toggled);
        }
    }


    public struct KeyStateInfo
    {
        Keys _key;
        bool _isPressed,
            _isToggled;
        public KeyStateInfo(Keys key,
                        bool ispressed,
                        bool istoggled)
        {
            _key = key;
            _isPressed = ispressed;
            _isToggled = istoggled;
        }
        public static KeyStateInfo Default
        {
            get
            {
                return new KeyStateInfo(Keys.None,
                                            false,
                                            false);
            }
        }
        public Keys Key
        {
            get { return _key; }
        }
        public bool IsPressed
        {
            get { return _isPressed; }
        }
        public bool IsToggled
        {
            get { return _isToggled; }
        }
    }

    public static class StandardMatrices
    {
        public static ColorMatrix InvertImage()
        {
            ColorMatrix cmPicture = new System.Drawing.Imaging.ColorMatrix();

            // Change the elements

            cmPicture.Matrix00 = -1;

            cmPicture.Matrix11 = -1;

            cmPicture.Matrix22 = -1;
            return cmPicture;

        }




    }
    /// <summary>
    /// Holds attribute data for Blocks that have the ManyToOne attribute set.
    /// </summary>
    public class ManyToOneAttributeData
    {
        public ManyToOneBlockData[] AcquiredData = null;
        public ManyToOneAttributeData(Type fortype)
        {

            //call static method. GetManyToOneData()...
            try
            {
                MethodInfo acquiremethod = fortype.GetMethod("GetManyToOneData");
                AcquiredData = (ManyToOneBlockData[])acquiremethod.Invoke(null, BindingFlags.Static, null, null, Thread.CurrentThread.CurrentCulture);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Type " + fortype.Name + " has ManyToOne Attribute but does not implement \"GetManyToOneData\" Static function", ex);

            }



        }



    }


    public class BCBlockGameState  
    {
        public static ProductKey ProductRegistration = null;
        public class NextFrameStartup
        {
            private class BasicWrapper
            {
                private BasicFrameFunction basicfunction = null;
                public BasicWrapper(BasicFrameFunction pbasicfunction)
                {
                    basicfunction = pbasicfunction;

                }
                public bool BasicFrameFunctionWrapper(NextFrameStartup nfs, BCBlockGameState currstate)
                {
                    basicfunction();
                    return false; //assume it always get's removed.


                }


            }
            public delegate bool NextFrameFunction(NextFrameStartup nfs, BCBlockGameState currstate);
            public delegate void BasicFrameFunction(); 
            private NextFrameFunction _nextFrame = null;
            public Object _Tag = null;
           
            public NextFrameFunction NextFrame { get { return _nextFrame; } set { _nextFrame = value; } }
            public Object Tag { get { return _Tag; } set { _Tag = value; } }
            
            public NextFrameStartup(BasicFrameFunction bframe):this(new BasicWrapper(bframe).BasicFrameFunctionWrapper)
            {



            }
            public NextFrameStartup(NextFrameFunction nff):this(nff,null)
            {


            }
            public NextFrameStartup(NextFrameFunction nff, Object objtag)
            {
                _nextFrame = nff;
                _Tag = objtag;


            }


        }


        public Queue<NextFrameStartup> NextFrameCalls = new Queue<NextFrameStartup>();

        public void Defer(TimeSpan delay,Action callthis)
        {
            DelayInvoke(delay, (obj) => callthis());

        }
        public void Defer(Action callthis)
        {
            NextFrameCalls.Enqueue(new NextFrameStartup(()=>callthis()));

        }

        public void QueueFrameEvent(NextFrameStartup.NextFrameFunction nff, Object pTag)
        {
            NextFrameCalls.Enqueue(new NextFrameStartup(nff, pTag));


        }



        public delegate void DeferredLevelLoadProc<T>(T readfor);
        //public static MemoryStream CurrentLevelStream = null;
        public static EditorSet SetFromEditor = null;


        public delegate void GameSoundStopEventFunction(BCBlockGameState gstate, iSoundSourceObject soundSource);
        public event GameSoundStopEventFunction GameSoundStop;

        //public static string[] ComboNames = new string[] { "Triple", "Super", "Hyper", "Brutal", "Master", "Awesome", "Blaster", "Monster", "King", "Killer", "Ultra" };
        public static string[] ComboNames = new string[] { "Squirrel", "Chipmunk", "Badger", "Penguin", "Ostrich", "Kangaroo", "Platypus", "Koala", "Panda", "Possum" };
        public static Size DefaultLevelSize = new Size(493, 395);
        public static List<String> LevelFolders=null;
        private static Color[] _comboFillColour;
        protected void InvokeSoundStop(BCBlockGameState gstate,iSoundSourceObject SoundSource)
        {
            var copied = GameSoundStop;
            if(copied!=null)
                copied.Invoke(gstate,SoundSource);


        }
      

        public static Color[] ComboPenColour
        {
            get {
                return ComboFillColour.Reverse().ToArray();
            
            }



        }
            public static Color[] ComboFillColour { get


            {
                return _comboFillColour ?? (_comboFillColour = new Color[]
                                                                   {
                                                                       Color.YellowGreen, Color.Blue, Color.Purple,
                                                                       Color.Turquoise, Color.DodgerBlue,
                                                                       Color.Maroon, Color.LemonChiffon, Color.IndianRed
                                                                       , Color.Bisque,
                                                                       Color.Red
                                                                   });
            }
            }
            public DateTime? ComboFinishTime=null;
            //public int ComboCountDown = 0;
        public int ComboCount = 0;


        public static String[] IntroStrings = new String[]{"Possum\nBe\nPraised!","BEARS!","Donuts","OoT FTW","METROID","ARKANOID",
                             "Wear Flowers\nIn your Hair","And the \nwinner is...","It's so BAD.","Big-lipped\nAlligator!","The Future\n Soon!",
                             "Put the rock\nin the house!","It'll soft \nrock you!","The Show \nMust Go On.","That's a \nPaddlin'",
                             "Whatcha Talkin'\n bout?","I pity the fool!","Anybody got \nany cheese?","To Go\n Boldly","Directive\n39436175880932/B",
         ";IS THE \nMONKEY ACTIVE?","Go, \nCHARMANDER!","Contains \n\"Free\"\nEditor!","Not From\nConcentrate!","Classic\nFormula.","Possum\nfree","Swing Low, \nSweet Bear",
        "Written\nin\nC#","Not a\nJava Program","Object Oriented!","This message is\n an error","Refreshing","3 Hits\nOn Youtube!",
        "Come \nGet Some!","Groovy!","20% Cooler","Does Death\nWear Blue?","In this town\n I am the law","My Circuit's\n Slow","Only Nu\ncan prevent\nForest fires",
        "Mess with\nThe Best\nDie Like\n the rest","Watch out\nfor Acid Burn!","They don't \ncall her \nRainbow Dash\n for nothing!","Guaranteed\nNot to\nCause Mumps","TROLL \nHARD",
        "forgetting that \nyou forgot\ncan be bliss.","easy\n as \nquicksand","not designed\nfor sad onions","Booleans:\nTrue,\nFalse\nFILE_NOT_FOUND","Fully\nScriptable!",
        "Smile,Smile,Smile","Beam,\nBeam,Beam!","Grin,\nGrin,Grin","Rainbows\nAre\nSPICY","Batteries\n Not Included","That Spells\n DNA","Buy Cosmo now!","Easy to Crack!",
        "GOD is REAL\n(unless declared INTEGER)","This text\nis really long\nand might not fit\n or be visible!","Hail Celestia","Friendship\nis\nMagic","Tomorrow\nIs\n%weekday+1%",
        "Hello,\n%username%!","%usedmem%MB \nREADY.","./make\n./configure\n./make install","Try\nBCSearch!","Not a \nSubstitute\n For Exercise",
        "Colliding Polys","Don't Mess\nWith\nMagnetman"};

        public static string[] ExpandIntroStrings()
        {

            List<String> buildlisting = new List<string>();
            String availableram = Math.Round(new PerformanceCounter("Memory", "Available Bytes").NextValue() / 1024 / 1024, 2).ToString();
            foreach (String intro in IntroStrings)
            {
                //replace "speshul" variables.
                
                String addintro = intro.Replace("%weekday%", DateTime.Now.DayOfWeek.ToString());
                addintro = addintro.Replace("%weekday+1%", DateTime.Now.AddDays(1f).DayOfWeek.ToString());
                addintro = addintro.Replace("%username%", CurrentUserName);
                addintro = addintro.Replace("%usedmem%", availableram);
                buildlisting.Add(addintro);


            }
            return buildlisting.ToArray();


        }
        internal static List<DeletionHelper> pendingdeletions = new List<DeletionHelper>();

        public static void AddDeleter(String filename)
        {

            pendingdeletions.Add(new DeletionHelper(filename));

        }
        public static void Cleanup()
        {
            Debug.Print("Performing final cleanup (BCBlockGameState::Cleanup)");
            Soundman.Dispose(); //dispose of the sound manager, which should cleanup all the files that are referenced therein.
            Imageman=null;
            pendingdeletions=null;



        }
        public static Point ClampToGrid(Point position, Size gridSize,Point gridOffset)
        {


            //larger\smaller*gridsize...

            int calcX = (((position.X+gridOffset.X)/ gridSize.Width)*gridSize.Width)+gridOffset.X;
            int calcY = (((position.Y+gridOffset.Y)/ gridSize.Height)*gridSize.Height)+gridOffset.Y;

            return new Point(calcX, calcY);


        }


        public PointF Translation = new PointF(0, 0); //controls scrolling.
        //represents the "poing game state"... this includes things like score, the active balls, the blocks, the loaded levelset, etc.
        //this could make for something very interesting once I implement persistence. Or rather IF I do.

        //public class BallBounce
        public static int GameID = 21; //This is the game ID in the bc-programming download database. It is used by the updater.
        public Control TargetObject;
        public List<cBall> ShootBalls = new List<cBall>();
        public LinkedList<cBall> Balls = new LinkedList<cBall>();
        public LinkedList<Block> Blocks = new LinkedList<Block>();
        public LinkedList<GameObject> GameObjects = new LinkedList<GameObject>();
        public List<cBall> RemoveBalls = new List<cBall>();

        

        public List<Particle> Particles = new List<Particle>();
        public Rectangle GameArea;
        public static bool Verbose=false;
        public static float _ParticleGenerationFactor = 0.5f;
        public static bool ShowDebugInfo = true;
        public static bool WaterBlockAnimations=true;
        public static int MaxParticles = 1500;
        public static float ParticleGenerationFactor {

            get { return _ParticleGenerationFactor;}


            set { _ParticleGenerationFactor=value;}


        }
        private static string _datfolder=null;
        public static String AppDataFolder {
            get {
                if(_datfolder!=null) return _datfolder;
                
                //check for commandline...
                var allargs = System.Environment.GetCommandLineArgs().ToList();
                int usefoundlocation=-1;
                String joined = String.Join(" ", System.Environment.GetCommandLineArgs());

                int hyphenfind = joined.IndexOf("-datafolder:",StringComparison.OrdinalIgnoreCase);
                int slashfind = joined.IndexOf("/datafolder:",StringComparison.OrdinalIgnoreCase);
                if (hyphenfind > -1) usefoundlocation = hyphenfind; else if (slashfind > -1) usefoundlocation = slashfind;
               

                if(usefoundlocation>-1)
                {
                    int firstchar = usefoundlocation+12;
                    int endchar=-1;
                    //if the first character is a quote, find the next quote; increment firstchar to avoid capturing that quote later, as well.

                    if (joined[firstchar] == '\'')
                    {
                        endchar = joined.IndexOf('\'', firstchar + 1);
                        firstchar++;
                    }
                    else
                    {
                        endchar = joined.IndexOf(' ', firstchar + 1);
                        if (endchar == -1) endchar = joined.Length;
                    }

                    _datfolder = joined.Substring(firstchar,endchar-firstchar);
                    return _datfolder;

                }
                else
                {
                    if (!PortableMode)
                    {
                        _datfolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                                            "BASeBlock");
                    }
                    else
                    {
                        //get the executable path.
                        String exepath = (new FileInfo(Application.ExecutablePath).DirectoryName);
                        //append APPDATA to that exe path.
                        _datfolder = Path.Combine(exepath, "APPDATA");

                    }
                    return _datfolder;
                }
            } 
        }

            public static INIFile GameSettings;
        public static MRULists MRU;
        public static string INIFileName = "";

        private static int _mtickgen;
        
        private static ThreadLocal<Random> _rgen;
        public static Random rgen
        {
            get
            {
                if(_rgen==null) _rgen = new ThreadLocal<Random>(()=>new Random());
                return _rgen.Value;
            }
            set
            {
                if (_rgen == null) _rgen = new ThreadLocal<Random>(() => new Random());
                _rgen.Value = value;
            }
        }
        //public static SoundManager Soundman;
        public static cNewSoundManager Soundman;
        public static ImageManager ToolbarImages;
        public static ImageManager Imageman;
        private static TemplateManager _Templateman=null;
        public static TemplateManager Templateman
        {
            get
            {
                if (_Templateman == null)
                    _Templateman = new TemplateManager(usetemplatefolders, new Nullcallback());
                return _Templateman;
            }
            set {
                _Templateman = value;


            }

        }

    


    
        public static LevelLockData LockData;
        //May 27th 2011 8:14
        //finally got sick and tired of sitting at the BASeBlock splash screen for about 30 seconds while testing. Considering I have a quad core that is,
        //bloody ridiculous. Main problem is that the various LoadedTypeManagers all reiterated over the same assemblies. Refactored a bit so there is only one loop (in the multitypemanager) that
        //handles it all.
        public static MultiTypeManager MTypeManager;



        public static LoadedTypeManager Levelman { get { return MTypeManager[typeof(iLevelSetBuilder)]; } }

            //public static LoadedTypeManager Levelman;
        //public static LoadedTypeManager PluginManager;
        public static LoadedTypeManager PluginManager { get { return MTypeManager[typeof(iBBPlugin)]; } }
        public static LoadedTypeManager BallBehaviourManager { get { return MTypeManager[typeof(iBallBehaviour)]; } }
        public static LoadedTypeManager SoundDriverManager { get { return MTypeManager[typeof(iSoundEngineDriver)]; } }
        public static LoadedTypeManager FilterPluginManager { get { return MTypeManager[typeof(iEditBlockFilter)]; } }
        public static LoadedTypeManager BlockTriggerTypes { get { return MTypeManager[typeof(BlockTrigger)]; } }
        public static LoadedTypeManager BlockEventTypes { get { return MTypeManager[typeof(BlockEvent)]; } }
        public static LoadedTypeManager EventTypes { get { return MTypeManager[typeof(TriggerEvent)]; } }
        public static LoadedTypeManager TriggerTypes { get { return MTypeManager[typeof(Trigger)]; } }
            public static LoadedTypeManager BlockTypes { get { return MTypeManager[typeof(Block)]; } }
            public static LoadedTypeManager EnemyTriggerTypes { get { return MTypeManager[typeof(EnemyTrigger)]; } }
            public static LoadedTypeManager BlockCategoryManager { get { return MTypeManager[typeof(BlockCategoryAttribute)]; } }
            //public static LoadedTypeManager BallBehaviourManager;
        //public static LoadedTypeManager SoundDriverManager;
        //public static LoadedTypeManager FilterPluginManager;
        //public static LoadedTypeManager BlockTriggerTypes;
        //public static LoadedTypeManager EventTypes;
        //public static LoadedTypeManager BlockTypes;
        
        public static HighScoreManager Scoreman;
        public static StatisticsManager Statman;
        public static Dictionary<String, Type> SoundPluginTypes;
        public static List<iBBPlugin> LoadedPlugins;
        //public static List<LUAScript> LUAScriptFilenames;
        public static BBSettings Settings;
        [DllImport("Advapi32.dll", EntryPoint="GetUserName", 
        ExactSpelling=false, SetLastError=true)]
        static extern bool GetUserName(
    [MarshalAs(UnmanagedType.LPArray)] byte[] lpBuffer,
    [MarshalAs(UnmanagedType.LPArray)] Int32[] nSize);






        #region User Message handling (status messages)

        public class UserMessageData
        {
            String MessageString; //string to display...
            Stopwatch Startered; //Stopwatch started when we were added.
            public TimeSpan ShowDuration;
            private Font _DrawFont = new Font(BCBlockGameState.GetMonospaceFont(), 14, FontStyle.Bold);
            private Brush _DrawBrush = new SolidBrush(Color.Black);
            private Pen _DrawPen = new Pen(Color.White);

            public Font DrawFont { get { return _DrawFont; } set { _DrawFont = value; Recreatebitmap(); } }
            public Brush DrawBrush { get { return _DrawBrush; } set { _DrawBrush = value; Recreatebitmap(); } }
            public Pen DrawPen { get { return _DrawPen; } set { _DrawPen = value; Recreatebitmap(); } }


            public TimeSpan AliveTime { get { return Startered.Elapsed; } }
            public TimeSpan TimeToLive { get { return ShowDuration - AliveTime; } }
            //used for caching of drawn bitmap...
            private Bitmap DrawnMessage;
            private Graphics MessageDraw;

            private float CalcAlpha()
            {
                double alivems = AliveTime.TotalMilliseconds;
                double showdurationms = ShowDuration.TotalMilliseconds;

                if (alivems < (showdurationms / 2)) return 255;


                return (float)((showdurationms / 2) / ((alivems - (showdurationms / 2))));




            }
            
            private void Recreatebitmap()
            {
                //redraw the bitmap.
                if (MessageDraw != null)
                    MessageDraw.Dispose();
                if (DrawnMessage != null)
                    DrawnMessage.Dispose();

                SizeF ssize = CalcSize();

                //create a new bitmap of that size.
                DrawnMessage = new Bitmap((int)ssize.Width, (int)ssize.Height);
                MessageDraw = Graphics.FromImage(DrawnMessage);
                MessageDraw.CompositingQuality = CompositingQuality.HighQuality;
                MessageDraw.SmoothingMode = SmoothingMode.HighQuality;
                //Draw to it... using a GraphicsPath.
                GraphicsPath usepath = new GraphicsPath();
                usepath.AddString(MessageString, DrawFont.FontFamily, (int)DrawFont.Style, DrawFont.Size, new Point(0, 0), StringFormat.GenericDefault);
                MessageDraw.DrawPath(DrawPen, usepath);
                MessageDraw.FillPath(DrawBrush, usepath);

                //tada...



            }
            /*
            public T CopyObject<T>(T clonethis) where T : ISerializable
            {
                MemoryStream savetostream = new MemoryStream();
                BinaryFormatter bff = BCBlockGameState.getFormatter();



            }
            */
            public UserMessageData(String Message)
            {
                MessageString = Message;
                Startered = new Stopwatch();
                Startered.Start();
                ShowDuration = new TimeSpan(0, 0, 0, 3);
                Recreatebitmap();

            }
          

            public SizeF CalcSize()
            {

                //returns the size 
                Graphics measureg = getmeasureg();
                return measureg.MeasureString(MessageString, DrawFont);



            }


            public void Draw(int OriginX, int OriginY, Graphics g)
            {

                ImageAttributes usecolorattributes = new ImageAttributes();
                usecolorattributes.SetColorMatrix(ColorMatrices.GetColourizer(1, 1, 1, CalcAlpha()));
                //g.DrawString(MessageString, DrawFont, DrawBrush, OriginX, OriginY);
                //g.DrawImageUnscaled(DrawnMessage, OriginX, OriginY);
                //g.DrawImage(DrawnMessage,new Rectangle(OriginX,OriginY,DrawnMessage.Width,DrawnMessage.Height),new Rectangle(0,0,DrawnMessage.Width,DrawnMessage.Height),GraphicsUnit.Pixel,usecolorattributes);            

                g.DrawImage(DrawnMessage, new Rectangle(OriginX, OriginY, DrawnMessage.Width, DrawnMessage.Height), 0, 0, DrawnMessage.Width, DrawnMessage.Height, GraphicsUnit.Pixel, usecolorattributes);

                //g.DrawImage(drawoverlayimage, new Rectangle(PicEditor.Width - drawoverlayimage.Width, PicEditor.Height - percentheight,
                //drawoverlayimage.Width, drawoverlayimage.Height), 0, 0, drawoverlayimage.Width, drawoverlayimage.Height, GraphicsUnit.Pixel,Drawattributes);

            }
        }
        public delegate void DelayedInvokeRoutine(Object[] parameters);


        public class DelayInvokeData
         {
            public TimeSpan waitdelay;
            public DelayedInvokeRoutine routine;
            
            public object[] callparameters;
            public TimeSpan recordedStartTime;
            public String Identifier = Guid.NewGuid().ToString();
            public DelayInvokeData(TimeSpan precordedStart,TimeSpan pwaitdelay, DelayedInvokeRoutine proutine, object[] callparams)
            {
                recordedStartTime = precordedStart;
                waitdelay = pwaitdelay;
                routine = proutine;
                callparameters = callparams;

            }
          
         

        }
        private Queue<DelayInvokeData> DelayData = new Queue<DelayInvokeData>();


        public void HandleDelayInvoke(TimeSpan CurrentLevelTime)
        {
            lock (DelayData)
            {
                if (DelayData.Any())
                {
                    DelayInvokeData peekit = DelayData.Peek();
                    if ((CurrentLevelTime - peekit.recordedStartTime) > peekit.waitdelay)
                    {
                        DelayData.Dequeue();
                        //if (peekit.LiveTime > CurrentLevelTime)
                        peekit.routine(peekit.callparameters);
                        //peekit.callroutines(peekit.callparameters);
                    }
                }
            }
        }
        

        public String DelayInvoke(TimeSpan totaltime, DelayedInvokeRoutine routinefunc)
        {
            return DelayInvoke(totaltime, routinefunc, null);

        }

        /// <summary>
        /// Givne a routine and a delay, will invoke that routine in the given gametime.
        /// This function is necessary (as opposed to using say DateTime and a TimeSpan or whatever)
        /// because gameobjects and whatnot need to essentially "ignore" time that the game isn't running or is paused or whatever.
        /// </summary>
        /// <param name="Totaltime"></param>
        /// <param name="routinefunc"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public String DelayInvoke(TimeSpan Totaltime, DelayedInvokeRoutine routinefunc, object[] parameters)
        {
            TimeSpan CurrentTime = ClientObject.GetLevelTime();
            SortedDictionary<TimeSpan, DelayInvokeData> createlisting = new SortedDictionary<TimeSpan, DelayInvokeData>();
            lock (DelayData)
            {
                foreach (var iterate in DelayData)
                {

                    if (createlisting.ContainsKey(iterate.waitdelay))
                    {
                        while (createlisting.ContainsKey(iterate.waitdelay))
                        {
                            iterate.waitdelay = iterate.waitdelay + new TimeSpan(1);
                        }

                    }

                    createlisting.Add(iterate.waitdelay, iterate);




                }

                DelayInvokeData newdata = new DelayInvokeData(CurrentTime, Totaltime, routinefunc, parameters);

                //make sure the waitdelay is unique. add a tick until there aren't any.
                if (createlisting.ContainsKey(newdata.waitdelay))
                {
                    while (createlisting.ContainsKey(newdata.waitdelay))
                        newdata.waitdelay = newdata.waitdelay + new TimeSpan(1);

                }
                //createlisting[newdata.waitdelay].AddRoutine(newdata.

                createlisting.Add(newdata.waitdelay, newdata);


                DelayData = new Queue<DelayInvokeData>(createlisting.Values);
                return newdata.Identifier;
            }
        }
        public DelayInvokeData GetDelayData(String identifier)
        {
            List<DelayInvokeData> listdata = new List<DelayInvokeData>(DelayData);
            return listdata.FirstOrDefault((w) => w.Identifier == identifier);



        }
        /// <summary>
        /// changes the delay, or function, of an already set DelayInvoke.
        /// </summary>
        /// <param name="Identifier">String that was returned by DelayInvoke, identifying the element.</param>
        /// <param name="newdelay">New Delay Value. use null to keep current delay.</param>
        /// <param name="newfunction">new DelayInvokeRoutine; pass null to keep current.</param>
        /// <returns>true if changes were made to a delay; false otherwise. false will be immediately returned if both parameters are null.</returns>
        public bool ChangeDelayData(String Identifier, TimeSpan? newdelay, DelayedInvokeRoutine newfunction,bool resetstart)
        {
            if (newdelay == null && newfunction == null)
                return false;
            //find the given identifier...
            List<DelayInvokeData> listdata = new List<DelayInvokeData>(DelayData);
            if (listdata.Any((w) => w.Identifier == Identifier))
            {
                DelayInvokeData grabitem = listdata.First((w) => w.Identifier == Identifier);
                if (newdelay != null)
                {
                    grabitem.waitdelay = newdelay.Value;
                    if(resetstart) grabitem.recordedStartTime = ClientObject.GetLevelTime();


                }
                if (newfunction != null)
                    grabitem.routine = newfunction;
                return true;
            }
            else
            {
                //no element; return false.
                return false;
            }


        }

        public static void DelayInvoke_Direct(TimeSpan waittime, Action routine)
        {
            Thread usethread = new Thread(DelayInvokeThread);
            Stopwatch StopWatcher = new Stopwatch();
            StopWatcher.Start();
            usethread.Start(new Object[] { usethread, routine,waittime,StopWatcher });

        }
        private static void DelayInvokeThread(Object parameters)
        {
            Object[] acquireparam = (Object[])parameters;
            
            Action useaction = acquireparam[1] as Action;

            TimeSpan waittime = (TimeSpan)acquireparam[2];
            Stopwatch startdelay = (Stopwatch)acquireparam[3];
            while (startdelay.Elapsed < waittime)
            {
                Thread.Sleep(5);
            }

            useaction();
            


        }
        private Queue<UserMessageData> MessageQueue = new Queue<UserMessageData>();

        /// <summary>
        /// routine called from gameproc, which "processes" messages; that is, it processes a frame.
        /// </summary>
        public void ProcessMessages()
        {
            //peek at the next one in line.
            if (MessageQueue.Count > 0)
            {
                UserMessageData Nextup = MessageQueue.Peek();
                if (Nextup.AliveTime > Nextup.ShowDuration)
                {
                    //dequeue/remove it...
                    MessageQueue.Dequeue();


                }
            }
           



        }
        public void PaintMessages(Graphics g,RectangleF AvailableClientArea)
        {
            //the first items to be dequeued are the "older" items.
            SmoothingMode cachedsmooth = g.SmoothingMode;
            g.SmoothingMode = SmoothingMode.HighQuality;
            //step one, iterate through each and find the cumulative height, as well as the maximum width...
            float maxwidth = 0, heightaccum = 0;
            foreach (var loopmessage in MessageQueue)
            {
                SizeF getsize = loopmessage.CalcSize();
                if (getsize.Width > maxwidth) maxwidth = getsize.Width;
                heightaccum += getsize.Height;


            }
            //maxwidth: width of longest message
            //heightaccum: accumulated height.
            float CurrX = AvailableClientArea.Width - maxwidth;
            float CurrY = AvailableClientArea.Height - heightaccum;

            var cachedt = g.CompositingQuality;
            g.CompositingQuality=CompositingQuality.HighQuality;
            foreach (var loopmessage in MessageQueue)
            {

                //paint this message at CurrX/CurrY...
                loopmessage.Draw((int)CurrX, (int)CurrY, g);
                //increment CurrY by the height of this message.
                SizeF messagesize = loopmessage.CalcSize();
                CurrY += messagesize.Height;


            }

            g.CompositingQuality=cachedt;
            g.SmoothingMode = cachedsmooth;



        }

        public UserMessageData EnqueueMessage(String Message)
        {
            UserMessageData createdata = new UserMessageData(Message);
            //Enqueue it...
            MessageQueue.Enqueue(createdata);
            return createdata;



        }

        #endregion







    public static T ClampValue<T> (T Value, T min, T max)  where T:IComparable
    {
            //cast to IComparable
        IComparable cvalue = (IComparable)Value;
        IComparable cmin = (IComparable)min;
        IComparable cmax = (IComparable)max;

            //return (T)(cvalue.CompareTo(cmin)< 0 ?cmin:cvalue.CompareTo(cmax)>0?max:Value);
        if (cvalue.CompareTo(cmin) < 0)
        {
            return min;
        }
        else if (cvalue.CompareTo(cmax) > 0)
        {
            return max;
        }
            return Value;

    }

        public static String CurrentUserName
        {
            get {
                byte[] str = new byte[256];
                Int32[] len = new Int32[1];
                len[0] = 256;
                GetUserName(str, len);
                return System.Text.Encoding.ASCII.GetString(str).Replace('\0',' ').Trim();

            
            
            }


        }
        
        public static PointF GoTowardsPoint(PointF OriginPoint, PointF DestinationPoint, float speed)
        {
            double anglebetween = GetAngle(OriginPoint, DestinationPoint);
            return new PointF((float)(Math.Cos(anglebetween) * speed), (float)(Math.Sin(anglebetween) * speed));


        }




        /// <summary>
        /// determines if DWM (the desktop window manager) is available and active on the current system.
        /// </summary>
        /// <returns>returns true if the system is Vista or later and has the Desktop compositor enabled. False otherwise.</returns>


        public static bool hasDWM()
        {

            return DWM.IsDWMEnabled();

        }

        public static PointF? IntersectLine(PointF LinePointA, PointF LinePointB,RectangleF checkrectangle)
        {

            //return !(LiangBarsky(BlockRectangle, LinePointA, LinePointB) == null);
            PointF[] TopLine = new PointF[] { new PointF(checkrectangle.Left, checkrectangle.Top), new PointF(checkrectangle.Right, checkrectangle.Top) };
            PointF[] LeftLine = new PointF[] { new PointF(checkrectangle.Left, checkrectangle.Top), new PointF(checkrectangle.Left, checkrectangle.Bottom) };
            PointF[] BottomLine = new PointF[] { new PointF(checkrectangle.Left, checkrectangle.Bottom), new PointF(checkrectangle.Right, checkrectangle.Bottom) };
            PointF[] RightLine = new PointF[] { new PointF(checkrectangle.Right, checkrectangle.Top), new PointF(checkrectangle.Right, checkrectangle.Bottom) };
            PointF?[] nresults = (from x in new PointF[][] { TopLine, LeftLine, BottomLine, RightLine } select BCBlockGameState.findIntersection(LinePointA, LinePointB, x[0], x[1])).ToArray();

            List<PointF> genresults = (from iterate in nresults where iterate != null select iterate.Value).ToList();
            if (genresults.Count == 0) return null;


            PointF closepoint = BCBlockGameState.getClosestPoint(LinePointA, genresults.ToArray());

            //PointF closepoint = getClosestPoint(LinePointA, results);

            return closepoint;
        }
        public static PointF getClosestPoint(PointF ToPoint,PointF[] checkpoints)
        {
            int index;
            return getClosestPoint(ToPoint, checkpoints, out index);


        }

        public static PointF getFarthestPoint(PointF ToPoint, PointF[] checkpoints, out int index)
        {
            float currmax = float.MaxValue;
            int maxindex = 0;
            float[] distances = new float[checkpoints.Length];
            for (int i = 0; i < distances.Length; i++)
            {

                distances[i] = Distance(ToPoint, checkpoints[i]);
                if (distances[i] > currmax)
                {
                    currmax = distances[i];
                    maxindex = i;

                }
            }
            index = maxindex;
            return checkpoints[maxindex];

        }
        public static PointF getClosestPoint(PointF ToPoint, PointF[] checkpoints,out int index)
        {
            float currmin = 0;
            int minindex = 0;
            float[] distances = new float[checkpoints.Length];
            for (int i = 0; i < distances.Length; i++)
            {

                distances[i] = Distance(ToPoint, checkpoints[i]);
                if (distances[i] < currmin)
                {
                    currmin = distances[i];
                    minindex = i;

                }
            }
            index=minindex;
            return checkpoints[minindex];

        }
        // calculates intersection and checks for parallel lines.  
        // also checks that the intersection point is actually on  
        // the line segment p1-p2  
        public static PointF? findIntersection(PointF p1, PointF p2,
          PointF p3, PointF p4)
        {
            float xD1, yD1, xD2, yD2, xD3, yD3;
            float dot, deg, len1, len2;
            float segmentLen1, segmentLen2;
            float ua, ub, div;

            // calculate differences  
            xD1 = p2.X - p1.X;
            xD2 = p4.X - p3.X;
            yD1 = p2.Y - p1.Y;
            yD2 = p4.Y - p3.Y;
            xD3 = p1.X - p3.X;
            yD3 = p1.Y - p3.Y;

            // calculate the lengths of the two lines  
            len1 = (float)Math.Sqrt(xD1 * xD1 + yD1 * yD1);
            len2 = (float)Math.Sqrt(xD2 * xD2 + yD2 * yD2);

            // calculate angle between the two lines.  
            dot = (xD1 * xD2 + yD1 * yD2); // dot product  
            deg = dot / (len1 * len2);

            // if Math.Abs(angle)==1 then the lines are parallell,  
            // so no intersection is possible  
            if (Math.Abs(deg) == 1) return null;

            // find intersection Pt between two lines  
            PointF pt = new Point(0, 0);
            div = yD2 * xD1 - xD2 * yD1;
            ua = (xD2 * yD3 - yD2 * xD3) / div;
            ub = (xD1 * yD3 - yD1 * xD3) / div;
            pt.X = p1.X + ua * xD1;
            pt.Y = p1.Y + ua * yD1;

            // calculate the combined length of the two segments  
            // between Pt-p1 and Pt-p2  
            xD1 = pt.X - p1.X;
            xD2 = pt.X - p2.X;
            yD1 = pt.Y - p1.Y;
            yD2 = pt.Y - p2.Y;
            segmentLen1 = (float)(Math.Sqrt(xD1 * xD1 + yD1 * yD1) + Math.Sqrt(xD2 * xD2 + yD2 * yD2));

            // calculate the combined length of the two segments  
            // between Pt-p3 and Pt-p4  
            xD1 = pt.X - p3.X;
            xD2 = pt.X - p4.X;
            yD1 = pt.Y - p3.Y;
            yD2 = pt.Y - p4.Y;
            segmentLen2 = (float)(Math.Sqrt(xD1 * xD1 + yD1 * yD1) + Math.Sqrt(xD2 * xD2 + yD2 * yD2));

            // if the lengths of both sets of segments are the same as  
            // the lenghts of the two lines the point is actually  
            // on the line segment.  

            // if the point isn’t on the line, return null  
            if (Math.Abs(len1 - segmentLen1) > 0.01 || Math.Abs(len2 - segmentLen2) > 0.01)
                return null;

            // return the valid intersection  
            return pt;
        }
        public static PointF VaryVelocity(PointF velocity, double anglevariance)
        {

            double currentangle = GetAngle(new PointF(0, 0), velocity);
            double radius = Distance(new PointF(0, 0), velocity);
            double varyangle = (rgen.NextDouble() * anglevariance) - (anglevariance / 2);
            double useangle = currentangle + varyangle;
            PointF returnvel = new PointF((float)(Math.Cos(useangle) * radius), (float)(Math.Sin(useangle) * radius));

            return returnvel;


        }

        public static PointF[] LiangBarsky(RectangleF WindowRect, PointF PointA, PointF PointB)
        {
            
            float gd, gm;
            float x1, y1, x2, y2;
            float wxmin, wymin, wxmax, wymax;
            float u1 = 0.0f, u2 = 1.0f;
            float p1, q1, p2, q2, p3, q3, p4, q4;
            float r1, r2, r3, r4;
            float x11, y11, x22, y22;
         
            wxmin = WindowRect.Left;
            wymin = WindowRect.Top;
            wxmax = WindowRect.Right;
            wymax = WindowRect.Bottom;
            //now the line...
            x1 = PointA.X;
            y1 = PointA.Y;
            x2 = PointB.X;
            y2 = PointB.Y;

            p1 = -(x2 - x1); q1 = x1 - wxmin;
            p2 = (x2 - x1); q2 = wxmax - x1;
            p3 = -(y2 - y1); q3 = y1 - wymin;
            p4 = (y2 - y1); q4 = wymax - y1;
        
            if (((p1 == 0.0) && (q1 < 0.0)) ||
            ((p2 == 0.0) && (q2 < 0.0)) ||
            ((p3 == 0.0) && (q3 < 0.0)) ||
            ((p4 == 0.0) && (q4 < 0.0)))
            {
         
                return null; //no intersection... (line rejected)
            }
            else
            {
                if (p1 != 0.0)
                {
                    r1 = (float)q1 / p1;
                    if (p1 < 0)
                        u1 = Math.Max(r1, u1);
                    else
                        u2 = Math.Min(r1, u2);
                }
                if (p2 != 0.0)
                {
                    r2 = (float)q2 / p2;
                    if (p2 < 0)
                        u1 = Math.Max(r2, u1);
                    else
                        u2 = Math.Min(r2, u2);

                }
                if (p3 != 0.0)
                {
                    r3 = (float)q3 / p3;
                    if (p3 < 0)
                        u1 = Math.Max(r3, u1);
                    else
                        u2 = Math.Min(r3, u2);
                }
                if (p4 != 0.0)
                {
                    r4 = (float)q4 / p4;
                    if (p4 < 0)
                        u1 = Math.Max(r4, u1);
                    else
                        u2 = Math.Min(r4, u2);
                }

                if (u1 > u2)
                    return null;
                else
                {
                    x11 = x1 + u1 * (x2 - x1);
                    y11 = y1 + u1 * (y2 - y1);

                    x22 = x1 + u2 * (x2 - x1);
                    y22 = y1 + u2 * (y2 - y1);
                   
                    return new PointF[] { new PointF(x11, y11), new PointF(x22, y22) };
                }
            }
            
        }

        [Flags]
        public enum BBlockCheatMode
        {
            Cheats_None,
            Cheats_Ballability ,
            Cheats_kerploder

        }
        public Type KerploderFind, KerploderReplace;

            public static PointF closestpointonline(PointF EndPoint1,PointF EndPoint2,PointF TestPoint) 
         { 

            float lx1 = EndPoint1.X;
            float ly1 = EndPoint1.Y;
            float lx2 = EndPoint2.X;
            float ly2 = EndPoint2.Y;
            float x0 = TestPoint.X;
            float y0 = TestPoint.Y;

            float A1 = ly2 - ly1; 
            float B1 = lx1 - lx2; 
            double C1 = (ly2 - ly1)*lx1 + (lx1 - lx2)*ly1; 
            double C2 = -B1*x0 + A1*y0; 
            double det = A1*A1 - -B1*B1; 
            double cx = 0; 
            double cy = 0; 
            if(det != 0)
            { 
                cx = (float)((A1*C1 - B1*C2)/det); 
                cy = (float)((A1*C2 - -B1*C1)/det); 
            }
            else
            { 
                cx = x0; 
                cy = y0; 
            } 
            return new PointF((float)cx,(float) cy); 
    }




        public double Magnitude(PointF VectorTest)
        {
            return Math.Sqrt(Math.Pow(VectorTest.X, 2) + Math.Pow(VectorTest.Y, 2));


            
        }
        public PointF VectorNormal(PointF ofVector)
        {
            double useangle = GetAngle(new PointF(0, 0), ofVector);
            return new PointF((float)Math.Cos(useangle), (float)Math.Sin(useangle));



        }
        public static PointF CenterPoint(RectangleF ofrect)
        {
            return new PointF(ofrect.Left + (ofrect.Width / 2), ofrect.Top + (ofrect.Height / 2));

        }

        public static Rectangle CenterRect(RectangleF largerect, Size middlesize)
        {
            return new Rectangle((int)((largerect.Width/2)-((float)middlesize.Width/2)),(int)((largerect.Height/2)-((float)middlesize.Height/2)),middlesize.Width,middlesize.Height);



        }
        public static Rectangle CenterRect(RectangleF largerect, SizeF middlesize)
        {
            return new Rectangle((int)((largerect.Width / 2) - ((float)middlesize.Width / 2)), (int)((largerect.Height / 2) - ((float)middlesize.Height / 2)), (int)middlesize.Width, (int)middlesize.Height);



        }
        
        public double DotProduct(PointF VectorA, PointF VectorB)
        {

            //Which says that the dot product of two vectors is equal to the magnitude 
            //of each multiplied together all multiplied by the cosine of the angle between them

            double AngleA = GetAngle(new PointF(), VectorA);
            double AngleB = GetAngle(new PointF(), VectorB);
            double MagA = Distance(0, 0, VectorA.X, VectorA.Y);
            double MagB = Distance(0, 0 ,VectorB.X, VectorB.Y);
            return (MagA*MagB)*Math.Cos(AngleB-AngleA);






        }

        public static RectangleF GetBlockRect(List<Block> ofblocks)
           {
               float leftside = ofblocks.Min((w) => w.BlockRectangle.Left);
               float topside = ofblocks.Min((w) => w.BlockRectangle.Top);
               float rightside = ofblocks.Max((w) => w.BlockRectangle.Right);
               float bottomside = ofblocks.Max((w) => w.BlockRectangle.Bottom);
               return new RectangleF(leftside,topside,rightside-leftside,bottomside-topside);




           }

        public static BBlockCheatMode CheatMode;
        public static bool CheatSet(BBlockCheatMode checkcheat)
        {
            return ((CheatMode & checkcheat) == checkcheat);



        }
        public static bool IsType(Type testtype, Type testfor)
        {
            return (from p in testtype.GetInterfaces() where p == testfor select p).Any() ||
                testtype.IsSubclassOf(testfor);



        }
        public static Type FindClass(String classname)
        {
            Type[] returnvalue = FindClasses(classname);
            if (returnvalue.Length > 0) return returnvalue[0]; else return null;


        }
        private static Assembly[] domainassemblies = AppDomain.CurrentDomain.GetAssemblies();

        public static Type[] FindClasses(String classname)
        {
            List<Assembly> removethese = new List<Assembly>();
            Type[] result = FindClasses(domainassemblies, classname,(u)=>removethese.Add(u));

            domainassemblies = (from d in domainassemblies where !removethese.Contains(d) select d).ToArray();

            return result;
        }

        public static Type[] FindClasses(Assembly inAssembly, String classname)
        {
            return FindClasses(new Assembly[] { inAssembly }, classname);


        }
        public static String GetFileFilter()
        {

            return "XML Levelset Files (*.xblf)|*.xblf|BASeBlock LevelSet Files (*.blf)|*.BLF|All Files (*.*)|*.*";

        }
        public static String GetTempPath()
        {
            String tpath = Path.GetTempPath();
            tpath = Path.Combine(tpath, "BASeBlock");
            if (!Directory.Exists(tpath)) Directory.CreateDirectory(tpath);
            return tpath;

        }
        public static String GetTempFile(String useextension)
        {
            String tpath = GetTempPath();
            if (!useextension.StartsWith(".")) useextension = "." + useextension;
            //GetTempPath(1023,tpath);
            tpath = tpath.Replace('\0', ' ').Trim();
            String destfilename = Guid.NewGuid().ToString() + useextension;
            return Path.Combine(tpath, destfilename);





        }
        /// <summary>
        /// determines if the given Assembly is compatible with our current run mode.
        /// </summary>
        /// <param name="testassembly"></param>
        /// <returns></returns>
        private static bool isCompatible(Assembly testassembly)
        {
            Assembly us = Assembly.GetExecutingAssembly();
            ProcessorArchitecture pa = testassembly.GetName().ProcessorArchitecture;
            if (pa == ProcessorArchitecture.MSIL)
                return true; //was compiled for AnyCPU.
            else if (pa == ProcessorArchitecture.Amd64)
                return Isx64();
            else if (pa == ProcessorArchitecture.X86)
                return !Isx64();
            
                

            else
            {
                return false;
            }
            

            



        }
        public static Type[] FindClasses(IEnumerable<Assembly> inAssemblies, String classname)
        {
            return FindClasses(inAssemblies, classname, null);

        }
        public static Type[] FindClasses(IEnumerable<Assembly> inAssemblies, String classname,Action<Assembly> OnError )
        {
            List<Type> returnvalues = new List<Type>(); 
            foreach(Assembly inAssembly in inAssemblies)
            {
                if (isCompatible(inAssembly))
                {
                    try
                    {
                        returnvalues.AddRange(inAssembly.GetTypes().Where(looptype => looptype.Name.Equals(classname, StringComparison.OrdinalIgnoreCase) || looptype.FullName.Equals(classname,StringComparison.OrdinalIgnoreCase)));
                    }
                    catch (ReflectionTypeLoadException rtle)
                    {

                        Debug.Print(rtle.ToString());
                        foreach (Exception inner in rtle.LoaderExceptions)
                            Debug.Print("<<LoaderException:>>" + inner.ToString());
                        if (OnError != null) OnError(inAssembly);
                    }
                    catch (Exception ex)
                    {
                        Debug.Print(ex.ToString());
                        if (OnError != null) OnError(inAssembly);
                    }
                }
            }


            return returnvalues.ToArray();

        }

    
        public static String GetAssemblyFile()
        {

            string codeBase = Assembly.GetExecutingAssembly().CodeBase;
            UriBuilder uri = new UriBuilder(codeBase);
            string path = Uri.UnescapeDataString(uri.Path);
            return path;



        }
       

        public Bitmap lastframeimage = null;
        public Graphics Gamebackground;
        public Paddle PlayerPaddle;
        public Level PlayingLevel { get; set; }
        public BackgroundDrawer BackgroundDraw = null;
        private long mGameScore = 0;


        public static int isqrt(int x)
        {
            int x1;
            int s, g0, g1;

            if (x <= 1) return x;
            s = 1;
            x1 = x - 1;
            if (x1 > 65535) { s = s + 8; x1 = x1 >> 16; }
            if (x1 > 255) { s = s + 4; x1 = x1 >> 8; }
            if (x1 > 15) { s = s + 2; x1 = x1 >> 4; }
            if (x1 > 3) { s = s + 1; }

            g0 = 1 << s;                // g0 = 2**s. 
            g1 = (g0 + (x >> s)) >> 1;  // g1 = (g0 + x/g0)/2. 

            while (g1 < g0)
            {           // Do while approximations 
                g0 = g1;                 // strictly decrease. 
                g1 = (g0 + (x / g0)) >> 1;
            }
            return g0;
        }

      




        public long GameScore { 
            get {
            return mGameScore;
            } 
            set {
                
                long oldscore = mGameScore;
                

                mGameScore=value;
                invokescoreupdate(ref oldscore,ref  mGameScore);



            }
        }



            private int _playerLives = 3;
            public int playerLives { get { return _playerLives; } set { _playerLives = value; ClientObject.Dorefreshstats = true; } }

            public  DateTime LastMacGuffinTime;
            private int _MacGuffins = 0;
            
            public TimeSpan MacGuffinExpandTime = new TimeSpan(0, 0, 0, 0, 750);
            public int MacGuffins { get { return _MacGuffins; } set 
            { 
                _MacGuffins = value; 
                if(_MacGuffins > 100)
                {
                    _MacGuffins-=100;
                    ScoreMultiplier+=0.5f;
                    Soundman.PlaySound("MULTIPLIERUP");

                }
                LastMacGuffinTime = DateTime.Now; 
                ClientObject.Dorefreshstats = true; 
            //in order to keep the stats updated, we need to enqueue the SideBarForceRefresher.
                Stopwatch swatch = new Stopwatch();
                swatch.Start();
                NextFrameCalls.Enqueue(new NextFrameStartup(SideBarForceRefresher, new object[] { MacGuffinExpandTime,swatch}));
            
            } }




            private bool SideBarForceRefresher(NextFrameStartup nfs, BCBlockGameState gstate)
            {
                
                TimeSpan ExpandTime = (TimeSpan)(((Object[])nfs.Tag)[0]);
                Stopwatch watcher = (Stopwatch)(((Object[])nfs.Tag)[1]);
                
                gstate.ClientObject.Dorefreshstats = true;
                //gstate.Forcerefresh = true;
                if (watcher.Elapsed < ExpandTime)
                {
                    //reenqueue
                    return true;

                }
                return false;
            }

        public delegate void BallHitBottomProcedure(cBall ball);
        public delegate void LevelCompleteProc();
        public delegate void ScoreUpdateRoutine(ref long oldscore,ref  long newscore);
        public event BallHitBottomProcedure OnBallHitBottom;
        public event LevelCompleteProc LevelComplete;
        public event ScoreUpdateRoutine ScoreUpdate;
        public const String tempfoldername="BBLOCKZTEMP";
        private static DeletionHelper mTempDeleter;
        private bool mscoreupdateinprogress=false;
        private double _ScoreMultiplier = 1;
        public double ScoreMultiplier { get{return _ScoreMultiplier;} set{
            _ScoreMultiplier=value;
           ClientObject.Dorefreshstats=true; 
        } }

        public String NumCompletionsText;
       // public Font CompletionsFont = new Font("Arial", 20);
        public Font CompletionsFont = BCBlockGameState.GetScaledFont(new Font("Arial", 20), 32);
        public SizeF CompletionsFontsize;
        private int _NumCompletions;
        public event Action NumCompletionsChanged;


        private void InvokeCompletionsChanged()
        {

            var copied = NumCompletionsChanged;

            if (copied != null)
                copied.Invoke();



        }

        public int NumCompletions
        {
            get { return _NumCompletions; }
            set {
                _NumCompletions = value; 
                NumCompletionsText = " × " + _NumCompletions.ToString();

                CompletionsFontsize = GetStringSize(NumCompletionsText, CompletionsFont);
                InvokeCompletionsChanged();
            }
        }

        public bool Forcerefresh;
        public iGameClient ClientObject { get; set; }
        
      

        public static SizeF GetStringSize(String numCompletionsText, Font CompletionFont)
        {
            Bitmap testbmp = new Bitmap(1, 1);
            Graphics getcanvas = Graphics.FromImage(testbmp);
            getcanvas.PageUnit = GraphicsUnit.Point;
            return getcanvas.MeasureString(numCompletionsText, CompletionFont);



        }

        public static float Distance(PointF PointA, PointF PointB)
        {
            return (float)Math.Sqrt(Math.Pow(PointB.X - PointA.X,2) + Math.Pow(PointB.Y - PointA.Y,2));


        }

        public static cBall Ball_HitTestOne(IEnumerable<cBall> balllist, PointF Position)
        {
            return balllist.FirstOrDefault(loopball => Distance(loopball.Location, Position) < loopball.Radius);
        }

        public static List<cBall> Ball_HitTest(IEnumerable<cBall> balllist, PointF Position)
        {
            return balllist.Where(loopball => Distance(loopball.Location, Position) < loopball.Radius).ToList();
        }

        public static List<cBall> Ball_HitTest(IEnumerable<cBall> balllist, RectangleF bounds)
        {
            return balllist.Where(loopball => bounds.Contains(loopball.Location)).ToList();
        }

        //returns all block that the point given is within.
        public static Block Block_HitTestOne(IEnumerable<Block> blocklist, PointF Position)
        {
            return blocklist.FirstOrDefault(loopblock => loopblock.HitTest(Position));
        }

        public static List<Block> Block_HitTest(IEnumerable<Block> blocklist, PointF Position)
        {
            return blocklist.Where(loopblock => loopblock.HitTest(Position)).ToList();
        }

        private static bool BlockTouchesCircle(Block testblock, PointF Center, float Distance)
        {
            //create the four points.
            RectangleF br = testblock.BlockRectangle;
            PointF[] cornerpoints = new PointF[] { new PointF(br.Left, br.Top), new PointF(br.Right, br.Top), new PointF(br.Left, br.Bottom), new PointF(br.Right, br.Bottom) };
            float[] distances = new float[cornerpoints.Length];
            for (int i = 0; i < distances.Length; i++)
            {
                distances[i] = BCBlockGameState.Distance(Center, cornerpoints[i]);


            }
            return (distances.Any((q) => q < Distance));


        }

        public static List<Block> Block_HitTestCircle(IEnumerable<Block> blocklist, PointF Center, float distance)
        {
            //returns all blocks "touched" by the given circle.
            List<Block> returnblocks = (from n in blocklist where BlockTouchesCircle(n,Center,distance) select n).ToList();

            return returnblocks;
        }
        public static List<cBall> Ball_HitTestCircle(IEnumerable<cBall> balllist, PointF Center, float distance)
        {

            return (from n in balllist where (Distance(Center, n.Location) < distance) select n).ToList();


        }

        public static List<Block> Block_HitTest(IEnumerable<Block> blocklist, RectangleF Bounds,bool Precise)
        {
            //returns all blocks that are "touched" by the given rectangle bounds; if Precise is true, only those fully contained by the larger rectangle.
            List<Block> returnblocks = new List<Block>();
            bool doadd=false;
            Polygon boundpoly = new Polygon(Bounds);
            foreach (Block loopblock in blocklist)
            {
                if (loopblock != null)
                {
                    //doadd = Precise ? Bounds.Contains(loopblock.BlockRectangle) : Bounds.IntersectsWith(loopblock.BlockRectangle);
                    doadd = Precise ? boundpoly.Contains(loopblock.GetPoly()) : loopblock.HitTest(Bounds);

                    if (doadd) returnblocks.Add(loopblock);
                }
            }

            return returnblocks;




        }
        public static void Block_Hit(BCBlockGameState gamestate, Block hitthisblock)
        {
            Block_Hit(gamestate,hitthisblock,new PointF(0,-1));

        }
        /// <summary>
        /// hits the block. This is achieved by creating a temporary ball, placing it in the center of the block with minimal speed, and
        /// calling PerformBlockHit.
        /// </summary>
        /// <param name="gamestate"></param>
        /// <param name="hitthisblock"></param>
        public static void Block_Hit(BCBlockGameState gamestate, Block hitthisblock,PointF impactvector)
        {

            //defer all of the things.
              gamestate.NextFrameCalls.Enqueue(new NextFrameStartup(()=> {
            cBall addedball = new cBall(hitthisblock.CenterPoint(), impactvector);

            //set the position...
            addedball.Location = new PointF(addedball.Location.X - impactvector.X, addedball.Location.Y - impactvector.Y);

            //add it to the gamestate...
            gamestate.Balls.AddLast(addedball);
            //now, perform the "hit"...
            List<cBall> ballstoadd = new List<cBall>();
            bool doremove = hitthisblock.PerformBlockHit(gamestate, addedball);

            if (doremove)
            {
                //we also need to fire the appropriate event.
                bool nodef = false;
                hitthisblock.RaiseBlockDestroy(gamestate, addedball,  ref nodef);
                gamestate.Blocks.Remove(hitthisblock);
                gamestate.Forcerefresh = true;
            }

            //remove addedball...
            if(gamestate.Balls.Contains(addedball))
                gamestate.Balls.Remove(addedball);


            foreach (cBall addthis in ballstoadd)
            {
                gamestate.Balls.AddLast(addthis);


            }

              }));



        }
        public void SpawnRisingText(String Message, PointF Origin, Brush FillBrush)
        {
            SpawnRisingText(Message, Origin, FillBrush, null);

        }
        public void SpawnRisingText(String Message, PointF Origin, Brush FillBrush,Font usefont)
        {
            if (usefont == null) usefont = new Font(GetMonospaceFont(), 20, FontStyle.Bold);
            PointF usevelocity = new PointF((float)rgen.NextDouble() - 0.5f, -(float)(rgen.NextDouble() * 2) + 1);
            BasicFadingText bfr = new BasicFadingText(Message, Origin, usevelocity, usefont, new Pen(Color.Black,1), FillBrush);

            //can't add it directly, so add it using a proxy.
            //
            QueueFrameEvent((nfs, gstate) => {gstate.GameObjects.AddLast(bfr);return false;},null);

            

        }

        private static BCBlockGameState ScratchState = null;
        public static void EmitBlockSound(BCBlockGameState stateobject,Block blocksound)
        {
            Block copied = (Block)blocksound.Clone();
            copied.NullifyHooks();
            if(ScratchState==null) 
                ScratchState=  new BCBlockGameState(stateobject.ClientObject, stateobject.TargetObject, stateobject.GameArea);
            cBall generateball = new cBall(blocksound.CenterPoint(),new PointF(.5f,.5f));
            ScratchState.Balls.Clear();
            ScratchState.Balls.AddLast(generateball);
            ScratchState.Blocks.Clear();
            ScratchState.Blocks.AddLast(copied);
            List<cBall> discardadded = new List<cBall>();
            copied.PerformBlockHit(ScratchState, generateball);


        }


        public void TriggerEvent(int TriggerID, BCBlockGameState gstate)
        {
            BASeBlock.Trigger.InvokeTriggerID(TriggerID, gstate);

        }

        private static PointF Getpercentbetween(PointF pointA, PointF pointB, float percent)
        {
            PointF differencepoint = new PointF(pointB.X - pointA.X, pointB.Y - pointA.Y);
            return new PointF((differencepoint.X * percent) + pointA.X, (differencepoint.Y * percent) + pointA.Y);



        }
        /// <summary>
        /// Acquires a position along the given set of points that is ptotaldistance from the start.
        /// </summary>
        /// <param name="pathpoints"></param>
        /// <param name="distances"></param>
        /// <param name="totaldistance"></param>
        /// <param name="ptotaldistance"></param>
        /// <returns></returns>
        private PointF PointFromTotalDistance(PointF[] pathpoints, double[] distances, double totaldistance, double ptotaldistance)
        {
            //this is a tricky routine; the task is the get the point thatis "totaldistance" along the graphicspath.
            //this is actually sort of easier given the housekeeping arrays we have.

            //first step: get the first element of distances that is larger then the given distance...
            int indexgot = -1;
            ptotaldistance = ptotaldistance % totaldistance;
            for (int i = 0; i < distances.Length; i++)
            {

                if (distances[i] > ptotaldistance)
                {
                    indexgot = i;
                    break;
                }


            }
            //subtract one from indexgot to get the last element that is not larger then the value...
            indexgot--;

            //at this point, indexgot now corresponds to the index of the last point that is not larger then the given distance.
            //therefore, we know that the point we are trying to acquire is between pathpoints[indexgot] and pathpoints[indexgot+1].
            if (ptotaldistance > distances[distances.Length - 1]) indexgot = distances.Length - 2;
            //first, find the percentage between the two points our target lies:
            //totaldistance minus our first point will get the "remainder" of the given value; we then get the percentage that is of
            //the distance between the "indexgot" point and the next one.
            double percentbetween = (ptotaldistance - distances[indexgot]) / (distances[indexgot + 1] - distances[indexgot]);


            //lastly, we return it:

            return Getpercentbetween(pathpoints[indexgot], pathpoints[indexgot + 1], (float)percentbetween);




        }
        public void ReplaceBlocks(Func<Block, Block> replacefunc)
        {


            List<Block> newblocks = new List<Block>();
            foreach (Block iterate in Blocks)
            {
                var itervalue = iterate;
                if (!iterate.BlockRectangle.IsEmpty)
                {
                    Block madeblock = replacefunc(iterate);
                    if (madeblock != iterate)
                    {

                        newblocks.Add(madeblock);
                        NextFrameCalls.Enqueue(new NextFrameStartup(() => Blocks.Remove(itervalue)));
                        madeblock.BlockEvents = iterate.BlockEvents;
                        madeblock.BlockTriggers = iterate.BlockTriggers;

                        foreach (var iteratee in madeblock.BlockEvents)
                            iteratee.OwnerBlock = madeblock;
                        foreach (var iteratet in madeblock.BlockTriggers)
                            iteratet.OwnerBlock = madeblock;
                    }


                }

            }
            foreach (var iterateblock in newblocks)
            {
                Blocks.AddLast(iterateblock);
            }
            ClientObject.UpdateBlocks();
            Forcerefresh = true;


        }
       
        /// <summary>
        /// Retrieves a list of RectangleF's which correspond to blocks of the given size following the given path.
        /// </summary>
        /// <param name="usepath">Path to follow</param>
        /// <param name="usesize">Size of blocks</param>
        /// <returns>List of rectangles corresponding to the layout of blocks along the given path so that they do not overlap.</returns>
        private List<RectangleF> GetBlockPositionsOnPath(GraphicsPath usepath, SizeF usesize, int skipcount)
        {
            usepath.Flatten();
            PointF[] points = usepath.PathPoints;
            //array storing the total accumulated distance.
            double[] TotalDistance = new double[points.Length];
            //array of the difference between that position and the previous (in total length).
            double[] Differences = new double[points.Length];
            //difference (X and Y) between the point and the previous point.
            PointF[] offsets = new PointF[points.Length];
            List<RectangleF> returnrects = new List<RectangleF>();
            double accumdist = 0;
            PointF prevpoint = new PointF(0, 0);
            for (int i = 0; i < points.Length; i++)
            {
                TotalDistance[i] = accumdist;
                offsets[i] = new PointF(points[i].X - prevpoint.X, points[i].Y - prevpoint.Y);
                Differences[i] = Math.Sqrt((offsets[i].X * offsets[i].X) + (offsets[i].Y * offsets[i].Y));
                accumdist += Differences[i];
                prevpoint = points[i];


            }
            /* for (int i = 0; i < points.Length; i++)
             {
                 returnrects.Add(new RectangleF(points[i].X, points[i].Y, usesize.Width, usesize.Height));


             }
             */
            PointF lastcreateblockposition = new PointF(-500, -500);

            for (double i = 0; i < accumdist; i++)
            {
                PointF alongpos = PointFromTotalDistance(points, TotalDistance, accumdist, i);
                if ((Math.Abs(alongpos.X - lastcreateblockposition.X) > usesize.Width) ||
                    (Math.Abs(alongpos.Y - lastcreateblockposition.Y) > usesize.Height))
                {
                    lastcreateblockposition = alongpos;
                    returnrects.Add(new RectangleF(alongpos.Y, alongpos.X, usesize.Width, usesize.Height));




                }





            }






            return returnrects;

        }


        internal void invokescoreupdate(ref long oldscore, ref long newscore)
        {
          

            if(mscoreupdateinprogress) return;
            mscoreupdateinprogress=true;
            ScoreUpdateRoutine temphold = ScoreUpdate;
            if (temphold != null)
                temphold.Invoke(ref oldscore, ref  newscore);
            else
                Debug.Print("no score update routine");


            mscoreupdateinprogress=false;

        }
        
        internal void invokeballhitbottom(cBall hitball)
        {
            BallHitBottomProcedure temphold = OnBallHitBottom;
            if (temphold != null)
                temphold.Invoke(hitball);




        }
        internal void invokeLevelComplete()
        {
            LevelCompleteProc temphold = LevelComplete;
            if (temphold != null)
                temphold.Invoke();



        }
        public static void Initgamestate()
        {
            Initgamestate(new Nullcallback());

        }
//        Private Declare Function GetKeyState Lib "user32.dll" (ByVal nVirtKey As Long) As Integer
        [DllImport("user32.dll")]
        private static extern Int16 GetKeyState(int nVirtKey);

        [DllImport("kernel32.dll")]
        private static extern Int32 GetTickCount();
        public static String GetExecutingVersion()
        {

            return GetAssemblyVersion(Assembly.GetExecutingAssembly());


        }

        public static T Choose<T>(IEnumerable<T> ChooseArray)
        {
            if (rgen == null) rgen = new Random();
            SortedList<double, T> sorttest = new SortedList<double, T>();
            foreach (T loopvalue in ChooseArray)
            {
                double rgg;
                do
                {
                    rgg = rgen.NextDouble();
                }
                while (sorttest.ContainsKey(rgg));
                sorttest.Add(rgg, loopvalue);

            }

            //return the first item.
            return sorttest.First().Value;
        }

        /// <summary>
        /// Selects numselect random items from Choosearray, returning those chosen items in a new array.
        /// If numselect is larger then the size of the array, the resulting array will be numselect, but any entries after the length of the selection array
        /// will be undefined.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ChooseArray"></param>
        /// <param name="numselect"></param>
        /// <returns></returns>
        public static T[] Choose<T>(IEnumerable<T> ChooseArray, int numselect)
        {
            if (rgen == null) rgen = new Random();
            T[] returnarray = new T[numselect];
            SortedList<double, T> sorttest = new SortedList<double, T>();
            foreach (T loopvalue in ChooseArray)
            {
                sorttest.Add(rgen.NextDouble(), loopvalue);
            }
            //Array.Copy(sorttest.ToArray(), returnarray, numselect);
            var usearray = sorttest.ToArray();
            for (int i = 0; i < numselect; i++)
            {
                returnarray[i] = usearray[i].Value;
            }
            return returnarray;
        }
        /// <summary>
        /// given a set of blocks, a center point and a radius,returns all blocks that fall within that radius.
        /// </summary>
        /// <param name="BlocksCheck"></param>
        /// <param name="CenterPoint"></param>
        /// <param name="radius"></param>
        /// <returns></returns>

        public static List<Block> GetWithinDistance(IEnumerable<Block> BlocksCheck, PointF CenterPoint,float radius)
        {
            //create a new graphics path containing the Circle that is the appropriate radius...
            List<Block> returnlist = new List<Block>();
            GraphicsPath usepathradius = new GraphicsPath();
            usepathradius.AddEllipse(CenterPoint.X - radius, CenterPoint.Y - radius, radius * 2, radius * 2);
            //create a region with this ellipse...
            Region radiusregion = new Region(usepathradius);

            foreach (Block loopblock in BlocksCheck)
            {
                Region testblockregion = new Region(loopblock.BlockRectangle);

                testblockregion.Intersect(radiusregion);
                if(testblockregion.GetRegionData().Data.Length==0)
                {
                    returnlist.Add(loopblock);


                }


            }
            return returnlist;


        }

        public static void Testchoose()
        {
            String[] testitems = IntroStrings;
            for (int i = 0; i < 10; i++)
            {
                String grabrandom = Choose(testitems, 1)[0];
                MessageBox.Show("got:" + grabrandom);


            }








        }

        private static String GetAssemblyVersion(Assembly forassembly)
        {


            return forassembly.GetName().Version.ToString();

        }
        public static String joinList<T>(List<T> valuelist)
        {
            if (valuelist.Count == 0) return "";
            StringBuilder buildreturn = new StringBuilder();
            T lastitem=valuelist.Last();
            foreach (T loopitem in valuelist)
            {

                buildreturn.Append(loopitem.ToString());
                if(!loopitem.Equals(lastitem))
                {
                    buildreturn.Append(",");


                }



            }
            return buildreturn.ToString();


        }
        public static void Swap<T>(ref T ItemA, ref T ItemB)
        {
            T temp=ItemA;
            ItemA=ItemB;
            ItemB=temp;


        }
        public static IEnumerable<String> TypesToString(IEnumerable<Type> iteratetypes)
        {

            foreach (Type tt in iteratetypes)
            {
                if(tt!=null)
                    yield return tt.Name;

            }


        }
        public static IEnumerable<Type> StringToTypes(IEnumerable<String> iteratestrings)
        {
            foreach (String iterates in iteratestrings)
            {

                yield return FindClass(iterates);

            }


        }


        public float SpeedMultiplier = 1;
       

        public static void TweakAndInc(BCBlockGameState gstate, ref PointF Location, PointF Delta, PointF Factor)
        {
            TweakVelocity(gstate, ref Delta, Factor);
            IncrementLocation(gstate, ref Location, Delta);


        }
        public float GetMultiplier()
        {
            if (ClientObject.CurrentFPS == 0) return 1;
            return Math.Min(DesiredFPS / ClientObject.CurrentFPS,2f);


        }
        //returns time between frames, in seconds.
        public float TimeBetweenFrames()
        {

            return 1 / ClientObject.CurrentFPS;

        }
        //given the current value of something and the Desired addition or subtraction per second, Calculate the change per frame based on
        //the current TimeBetwenFrames.

        public float ScaleValue(float DesiredChangePerSecond)
        {
            //multiply amount we want to change in a second by the amount of time between frames.
            return DesiredChangePerSecond * TimeBetweenFrames();


        }
        public static void TweakVelocity(BCBlockGameState gstate, ref PointF Velocity, PointF factor)
        {
#if USEVELINCREMENT
            float multiplier = 1;
            if (gstate != null)
            {
                float gotfps = gstate.ClientObject.CurrentFPS;
                multiplier = DesiredFPS / gotfps;
                gstate.SpeedMultiplier = multiplier;

            }
            Velocity = new PointF(Velocity.X * (factor.X * multiplier), Velocity.Y * (factor.Y * multiplier));
#else
            Velocity = new PointF(Velocity.X*factor.X,Velocity.Y * factor.Y);
#endif


        }



        public static readonly float DesiredFPS = 120;
        //used to increment the position by one frame, given a velocity.
        //
        public static void IncrementLocation(BCBlockGameState gstate,ref PointF Location,PointF Velocity)
        {
            PointF discard = PointF.Empty;
            IncrementLocation(gstate, ref Location, Velocity, ref discard);

        }
        public static void IncrementLocation(BCBlockGameState gstate,ref PointF Location, PointF Velocity,ref PointF Offset)
        {
            if (Offset == null) Offset = PointF.Empty;
#if USEVELINCREMENT
            float multiplier = 1;
            if (gstate != null)
            {
                multiplier = gstate.GetMultiplier();
                gstate.SpeedMultiplier = multiplier;
            }

            //if the current fps is the same as the desired FPS, we want to move Velocity.
            //If it is higher, we want to love less than Velocity; if less, more.
            Offset = new PointF(Velocity.X * multiplier, Velocity.Y * multiplier);
            Location = new PointF(Location.X + Offset.X, Location.Y + Offset.Y);
#else
            Location = new PointF(Location.X+Velocity.X,Location.Y+Velocity.Y);
#endif
            //examples:
            //CurrentFPS=30
            //desired=60
            //Velocity= 4,4
            //we should move 8,8 instead.


        }
        public static void SwapListItems(ListView.ListViewItemCollection inlist, ListViewItem ItemA, ListViewItem ItemB) 
        {
            
            //check parameters:
            if (inlist == null) throw new ArgumentNullException("inlist");
            if (ItemA == null) throw new ArgumentNullException("ItemA");
            if (ItemB == null) throw new ArgumentNullException("ItemB");

            //make sure both items are in the given list...
            if (!inlist.Contains(ItemA)) throw new ArgumentException("Item not in passed ListViewItemCollection", "ItemA");
            if (!inlist.Contains(ItemB)) throw new ArgumentException("Item not in passed ListViewItemCollection", "ItemB");

            //both items are in the list... get their indices...
            int indexA = ItemA.Index;
            int indexB = ItemB.Index;
            //make sure we have them "in order"; IndexA should be 
            if (indexA > indexB)
            {
                //swap them...
                Swap(ref indexA, ref indexB);
                Swap(ref ItemA, ref ItemB);


            }
            Debug.Assert(indexA < indexB && ItemA.Index == indexA && ItemB.Index== indexB);


            //remove the second item, and add the first to that position...
            //inlist.RemoveAt(indexB);

            inlist.Remove(ItemB);
            inlist.Remove(ItemA);


            inlist.Insert(indexA, ItemB);
            inlist.Insert(indexB, ItemA);

            //remove first item, and add second item to that position...
            //inlist.RemoveAt(indexA);
            //




        }
        public static String GetExecutablePath()
        {
            return Application.ExecutablePath.Substring(0, Application.ExecutablePath.Length - Path.GetFileName(Application.ExecutablePath).Length);

        }


        static string ShortenPath(string path)
        {
            const string pattern = @"^(\w+:|\\)(\\[^\\]+\\[^\\]+\\).*(\\[^\\]+\\[^\\]+)$";
            const string replacement = "$1$2...$3";
            if (Regex.IsMatch(path, pattern))
            {
                return Regex.Replace(path, pattern, replacement);
            }
            else
            {
                return path;
            }
            TimeSpan ts;
            
        }
        public static void PopulateRecentList(ToolStripMenuItem itemrecent,String RecentID,EventHandler recentclick)
        {
            //clear the dropdown...
            ToolStripMenuItem usestrip = (ToolStripMenuItem)itemrecent;
            usestrip.DropDown.Items.Clear();


            //populate with recent items list.
            var useMRU = BCBlockGameState.MRU[RecentID];
            if (useMRU.Names.Count > 0)
            {
                foreach (String recentitem in useMRU.Names.ToArray().Reverse())
                {

                    ToolStripItem added = usestrip.DropDown.Items.Add(recentitem);
                    added.Text = ShortenPath(recentitem);
                    added.Tag = recentitem;
                    added.Click += recentclick;



                }
            }
            else
            {
                ToolStripItem added = usestrip.DropDown.Items.Add("(Empty)");
                added.Enabled=false;
                
            }


        }
        static Color ColorFromHex(String hexstring)
        {
            
            return System.Drawing.ColorTranslator.FromHtml(hexstring);



            
        }

        internal static void setMenuRenderer(INIFile setfile)
        {

            String renderername = setfile["game"]["menu"].Value;

            Debug.Print("renderer:" + renderername);
            //try to use findclass to search for renderername...


            switch (renderername.ToUpper())
            {
                case "SYSTEM":
                    ToolStripManager.RenderMode = ToolStripManagerRenderMode.System;
                    break;
                case "PROFESSIONAL":
                case "PRO":
                    ToolStripManager.RenderMode = ToolStripManagerRenderMode.Professional;
                    break;
                    default:
                    Type gottypeuse = FindClass(renderername);
                    if (gottypeuse == null)
                    {
                        //use the default...
                        ToolStripManager.RenderMode = ToolStripManagerRenderMode.System;

                    }
                    else
                    {
                        ToolStripManager.Renderer = (ToolStripRenderer) Activator.CreateInstance(gottypeuse);
                        
                       // ToolStripManager.RenderMode = ToolStripManagerRenderMode.Custom;
                    }
                    //ToolStripManager.Renderer = new Office2007Renderer.Office2007Renderer();
                    break;
            }
        }

        public static List<PlayMessageData> ImportMessageData(String filename)
        {
            List<PlayMessageData> createlist = new List<PlayMessageData>();
            using (FileStream readerstream = new FileStream(filename, FileMode.Open))
            {
                StreamReader sr = new StreamReader(readerstream);

                while (!sr.EndOfStream)
                {
                    String lineread = sr.ReadLine();
                    //split at pipes.
                    String[] splitversion = lineread.Split('|');
                    

                    //first entry is timeindex, parse as float.
                    float tindex = float.Parse(splitversion[0]);
                    String mess = splitversion[1];
                   // Font usefont = new Font("Arial", 24);
                    Font usefont = BCBlockGameState.GetScaledFont(new Font("Arial", 24), 38);
                    Color useStroke = Color.Black;
                    Color useFill = Color.White;
                    if (splitversion.Length > 2)
                    {
                        //Font, in Form: "Fontname",size
                        String FontData = splitversion[2];
                        String[] splitfontdata = FontData.Split(',');
                        usefont = new Font(splitfontdata[0].Substring(2, splitfontdata[0].Length - 2), float.Parse(splitfontdata[1]));





                    }
                    if (splitversion.Length > 3)
                    {
                        //fourth parameter is defined, fill color in form #RRGGBB
                        useFill = ColorFromHex(splitversion[3]);
                    }

                    if (splitversion.Length > 4)
                    {
                        useStroke = ColorFromHex(splitversion[4]);
                        //fifth parameter, stroke color in form #RRGGBB

                    }

                    PlayMessageData pmd = new PlayMessageData(tindex, mess);
                    pmd.MessageFont=usefont;
                    pmd.StrokeColor=useStroke;
                    pmd.FillColor=useFill; 
                    createlist.Add(pmd);


                }

                return createlist;
            }
        }


        public static ImageList InitImageList(Size imagesize)
        {

            //use the toolbar folder in our appdata folder. the key is the basename, much like with the "standard" image manager.
            ImageList createlist = new ImageList();
            createlist.ImageSize=imagesize;
            //acquire the DirectoryInfo of the "toolbar" folder, if present.
            String toolbarfolder = Path.Combine(AppDataFolder, "toolbar");
            if (Directory.Exists(toolbarfolder))
            {
                DirectoryInfo tbardir = new DirectoryInfo(toolbarfolder);
                //use imagemanager to determine supported files....
                foreach (FileInfo loopfile in tbardir.GetFiles())
                {
                    if (ImageManager.isFileSupported(loopfile.FullName))
                    {
                        FileStream readimagestream = new FileStream(loopfile.FullName,FileMode.Open);
                        Image readimage = Image.FromStream(readimagestream);
                        //add to the imagelist.
                        createlist.Images.Add(Path.GetFileNameWithoutExtension(loopfile.FullName), readimage);

                        readimagestream.Close();

                    }

                }




            }

            return createlist;
            

        }
        private static void progresscallback(float mtypeprogress)
        {
            //datahook.UpdateProgress(0.3f);
            //MTypeManager = new MultiTypeManager(PluginFolders.ToArray(), TypesToEnum, ignoreassemblies, datahook, progresscallback);

          //  datahook.UpdateProgress(0.5f);
            float useprog = (0.5f - 0.3f) * mtypeprogress + 0.3f;
            initcallback.UpdateProgress(useprog);

        }

        /// <summary>
        /// Reflects a given vector "against" a given surface vector.
        /// </summary>
        /// <param name="Vect">Vector to reflect</param>
        /// <param name="SurfaceVector">Normal Axis to reflect along</param>
        /// <returns></returns>
        public static PointF ReflectVector(PointF Vect,PointF SurfaceVector)
        {
            double AngleQ = GetAngle(new PointF(0, 0), SurfaceVector);

            //rotate vector.
            PointF rotatedvector = RotateVector(Vect, AngleQ);
            //invert Y.
            rotatedvector = new PointF(rotatedvector.X, -rotatedvector.Y);
            //rotate in other direction.
            PointF fixedvector = RotateVector(rotatedvector, -AngleQ);

            return fixedvector;


        }
        public static PointF GetClosestPointOnPoly(Polygon Poly, PointF p, out LineSegment closestline)
        {
            PointF currminpt = new PointF();
            closestline = null;
            float currminfl=float.MaxValue;
            PointF[] parray =  (from y in Poly.Points.ToArray() select (PointF)y).ToArray();
            for (int i = 0; i < parray.Length - 1; i++)
            {
                PointF grabmin = GetClosestPointOnLineSegment(parray[i], parray[i + 1], p);
                float grabdistance;
                if ((grabdistance = BCBlockGameState.Distance(p, grabmin)) < currminfl)
                {
                    currminfl = grabdistance;
                    currminpt = grabmin;
                    closestline = new LineSegment(parray[i], parray[i + 1]);

                }


            }
            return currminpt;
        }
        public static PointF GetClosestPointOnLineSegment(PointF A, PointF B, PointF P)
        {
            PointF AP = new PointF(P.X - A.X,P.Y-A.Y);       //Vector from A to P   
            PointF AB = new PointF(B.X - A.X,B.Y-A.Y);       //Vector from A to B  

            float magnitudeAB = AB.X*AB.X + AB.Y*AB.Y ;     //Magnitude of AB vector (it's length squared)     
            //float ABAPproduct = Vector2.Dot(AP, AB);    //The DOT product of a_to_p and a_to_b     
            float ABAPproduct = AP.DotProduct(AB);
            float distance = ABAPproduct / magnitudeAB; //The normalized "distance" from a to your closest point  

            if (distance < 0)     //Check if P projection is over vectorAB     
            {
                return A;

            }
            else if (distance > 1)
            {
                return B;
            }
            else
            {
                PointF addpoint = new PointF(AB.X * distance, AB.Y * distance);
                return new PointF(A.X + addpoint.X,A.Y+addpoint.Y);
            }
        }



        public static PointF GetRandomVelocity(float usespeed)
        {
            return GetRandomVelocity(usespeed, usespeed);
        }

        public static PointF GetRandomVelocity(float minspeed, float maxspeed)
        {
            return GetRandomVelocity(minspeed,maxspeed,(Math.PI * rgen.NextDouble()*2));
        }

        public static PointF GetRandomVelocity(float minspeed,float maxspeed,double angle)
        {
            double usespeed;
            if (minspeed == maxspeed) 
                usespeed = maxspeed;
            else
            {
                //choose a random speed.
                usespeed = (rgen.NextDouble()*(maxspeed-minspeed))+minspeed;
            }
            double useangle = angle;


            return new PointF((float)(Math.Cos(useangle) * usespeed), (float)(Math.Sin(useangle) * usespeed));
        }
        public static PointF GetVelocity(double speed, double angle)
        {
            return new PointF((float)(Math.Cos(angle) * speed), (float)(Math.Sin(angle) * speed));

        }

        private static iManagerCallback initcallback;

        public static bool Isx64()
        {

            return Marshal.SizeOf(typeof(IntPtr)) == 8;


        }

        public static IEnumerable<string> CanonicalizePath(String[] inputstrings, String RelativeTo)
        {
            String[] result = new String[inputstrings.Length];
            for (int i = 0; i < inputstrings.Length; i++)
            {
                result[i] = CanonicalizePath(inputstrings[i], RelativeTo);



            }

            return result;
        }

        public static String CanonicalizePath(String inputstring, String RelativeTo)
        {
            if(Path.IsPathRooted(inputstring)) return inputstring;


            String getcombined = Path.Combine(RelativeTo, inputstring);
            return new DirectoryInfo(getcombined).FullName;
            


        }
        public static bool IncludeAssemblyTest(Assembly testassembly)
        {
            if (testassembly.GetName().Name.StartsWith("script_"))
                return true;

            if (IncludeAssemblies.Any((w)=>w.Contains(testassembly.GetName().Name)))
                return true;

            return false;
        }


  
        public static Image GenerateBitmap(int width, int height, Color background, Func<int, int, Color> GenerationRoutine)
        {

            if (GenerationRoutine == null) throw new ArgumentNullException("GenerationRoutine");
            Bitmap drawbitmap = new Bitmap(width, height);
            
            
                for (int x = 0; x < width; x++)
                {
                    for (int y = 0; y < height; y++)
                    {
                        drawbitmap.SetPixel(x, y, GenerationRoutine(x, y));


                    }

                }

                return drawbitmap;

        }
        public static bool PortableMode = false; //set to true in some instances.

        private static IEnumerable<String> IgnoreAssemblies;
        private static IEnumerable<String> IncludeAssemblies;
        public static double TotalLength(PointF[] linepoints)
        {
            double accumulated = 0;
            for (int i = 0; i < linepoints.Length - 1; i++)
            {
                PointF firstpoint = linepoints[i];
                PointF secondpoint = linepoints[i + 1];
                accumulated += Distance(firstpoint, secondpoint);


            }
            
            return accumulated;
        }
        public static IEnumerable<T> Join<T>(params IEnumerable<T>[] enumerables)
        {
            return enumerables.SelectMany(iterate => iterate);
        }

        public static IEnumerable<T> Shuffle<T>(IEnumerable<T> Shufflethese)
        {
            if (rgen == null) rgen = new Random();
            var sl = new SortedList<float, T>();
            foreach (T iterate in Shufflethese)
            {
                sl.Add((float)rgen.NextDouble(), iterate);


            }
            Random rg = new Random();
            
            return sl.Select(iterator => iterator.Value);




        }
        public static void Initgamestate(iManagerCallback datahook)
        {
            initcallback = datahook;
            
            datahook.ShowMessage("Initializing BASeBlock- Version:" + GetExecutingVersion());
            _mtickgen = GetTickCount();
            rgen = new Random(_mtickgen);
            
            datahook.ShowMessage("Random Number Generator initialized with seed:" + _mtickgen.ToString());
            datahook.ShowMessage("Deleting leftover temporary files...");

            String gotpath = GetTempPath();
            Directory.Delete(gotpath, true);

            INIFileName = Path.Combine(AppDataFolder, "BASeBlock.ini");
            if (!File.Exists(INIFileName))
            {
                //hmm... in this case... well, look in our directory...
                var exepathdata = new FileInfo(Application.ExecutablePath).Directory;
                
                String exepath = (exepathdata.FullName);
                INIFileName = Path.Combine(exepath, "BASeBlock.ini");
                //if that doesn't exist either... well, just reset to the default folder anyway and let cINIFile Create it.
                if (File.Exists(exepath) && Directory.Exists(Path.Combine(exepath,"AppData")))
                {
                    //if the ini file is in the current directory and there is an appdata folder there as well, assume we are in "portable" mode, possibly
                    //running from a USB key or whatever.
                    //It could even be a CD, come to think of it.
                    PortableMode = true;
                    //also, because some of our "child" assemblies may in fact refer to the appdatafolder, we need to force them to use the
                    //portable location.
                    Environment.SetEnvironmentVariable("APPDATA", Path.Combine(exepath, "APPDATA"));
                }
                else
                {
                    INIFileName = Path.Combine(AppDataFolder, "BASeBlock.ini");
                }

            }
            try
            {
                datahook.ShowMessage("Using INI file:" + INIFileName);
                datahook.UpdateProgress(0.1f);
              //  MessageBox.Show("Set Progress...");
                GameSettings = new INIFile(INIFileName);
                GameSettings.AutoSave = true;
              //  MessageBox.Show("Loaded INIFile " + INIFileName);
                Settings = new BBSettings(GameSettings);
                MRU = new MRULists(GameSettings);
            }
            catch (Exception qp)
            {
                MessageBox.Show("Exception: " + qp.Message);


            }
            String statfile = Settings.StatisticsFile;
            Statman = new StatisticsManager(statfile);


            String Scorefile = Path.Combine(AppDataFolder, "scores.dat");
           // MessageBox.Show("Scorefile=" + Scorefile);
            if (File.Exists(Scorefile))
            {
                //if the scorefile exists, load it into Scoreman. 
                long gotlength = new FileInfo(Scorefile).Length;
                if (gotlength > 0)
                {
                    Debug.Print("Existing Scorefile found (" + gotlength.ToString() + " bytes)");
                    Scoreman = HighScoreManager.FromFile(Scorefile);
                    //if null, an error occured, so recreate it.
                    if (Scoreman == null)
                    {
                        Debug.Print("HighScoreManager.FromFile returned null for file \"" + Scorefile +
                                    "\", creating new file.");
                        Scoreman = new HighScoreManager(Scorefile);
                    }
                }
                else
                    {
                    //otherwise, create a new instance and make it refer to the scorefile.
                        Scoreman=new HighScoreManager(Scorefile);

                    }
                
            }

            else
            {
                Debug.Print("Creating new Highscore file...");
                Scoreman = new HighScoreManager(Scorefile);
            }

            String lockfile = Path.Combine(AppDataFolder, "level.lock");
       
                
                LockData = new LevelLockData(lockfile);
            


            setMenuRenderer(GameSettings);

            datahook.UpdateProgress(0.15f);
            //List<String> Soundfolders = {GameSettings["folders"]["sound"].Value};
            List<String> Soundfolders = Settings.SoundFolders;
            List<String> Imagefolders = Settings.ImageFolders;
            LevelFolders = Settings.LevelFolders;
            List<String> PluginFolders = Settings.PluginFolders;
            List<String> ScriptFolders =  Settings.ScriptFolders;
            List<String> Toolbarimagefolder = Settings.ToolbarImageFolders;
            List<String> Templatefolders = Settings.TemplateFolders;
            if(Settings.IgnoreAssemblies == "")
            {
                //use default to only inspect BASeBlock assembly.
                Settings.IgnoreAssemblies = "^(?!BASeBlock).*$";

            }
            
            List<String> _ignoreassemblies = Settings.IgnoreAssemblies.Split(new char[]{','}, StringSplitOptions.RemoveEmptyEntries).ToList();
            List<String> _includeassemblies = Settings.IncludeAssemblies.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList();
            IgnoreAssemblies = _ignoreassemblies;
            IncludeAssemblies = _includeassemblies;

            String SoundPluginFolder = Settings.SoundPluginFolders; //CanonicalizePath(GameSettings["folders"]["soundplugin",""].Value,AppDataFolder);
            if (SoundPluginFolder == "")
                SoundPluginFolder = Path.Combine(AppDataFolder, "SoundPlugin");



            

            String basetempfolder="";
        datahook.ShowMessage("using sound folders:" + joinList(Soundfolders));
            datahook.ShowMessage("using image folders:" + joinList(Imagefolders));
            datahook.ShowMessage("using Level folders:" + joinList(LevelFolders));
            datahook.ShowMessage("using Plugin folders:" + joinList(PluginFolders));
            datahook.ShowMessage("using Scripts folders:" + joinList(ScriptFolders));
            datahook.ShowMessage("using toolbarimage folders:" + joinList(Toolbarimagefolder));
            datahook.ShowMessage("Ignoring assemblies:" + joinList(_ignoreassemblies));
            datahook.ShowMessage("force including assemblies:" + joinList(_includeassemblies));
            
            //add current folder to all three...
            String execpath = GetExecutablePath();
            String executablelocation=execpath;
            if(!(execpath.EndsWith(Path.DirectorySeparatorChar.ToString())))
                executablelocation += + Path.DirectorySeparatorChar;
            Soundfolders.Add(executablelocation);
            Imagefolders.Add(executablelocation);
            LevelFolders.Add(executablelocation);
            PluginFolders.Add(executablelocation);
            Templatefolders.Add(executablelocation);
            
            //TODO: possibly split the soundfolder at semicolons to specify multiple directories.
            //Soundman = new cNewSoundManager(new irrklangDriver(),Soundfoldername);



            
            
            //LoadSoundEngines(ValidSoundEngines,SoundEngineTypes);

            //in order to load images,sounds, and so forth from zip files we need to extract them to a temporary location and add that temporary location as a folder
            // entry in the two soundfolders and imagefolders arrays.
            datahook.UpdateProgress(0.2f);
            datahook.ShowMessage("Creating temporary game data folder");
            //so, first, create a temporary folder;
           
            String[] DataFolders = new String[]{AppDataFolder};
            try
            {
                IEnumerable<DirectoryInfo> newtempfolders = ZipExtractFolder(DataFolders, out basetempfolder);
                //create the temporary directory.

                foreach (DirectoryInfo newtempfolder in newtempfolders)
                {
                    datahook.ShowMessage("adding temporary data folder " + newtempfolder);
                    Soundfolders.Add(newtempfolder.FullName);
                    Imagefolders.Add(newtempfolder.FullName);
                    LevelFolders.Add(newtempfolder.FullName);
                    Templatefolders.Add(newtempfolder.FullName);

                }


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);



            }
            //if control and shift are pressed during startup, display a dialog to display and update the folders for sound and images.

            


            #region user folder override

            //Debug.Print("keystates:" + (GetKeyState((int)Keys.Control).ToString() + "," + (GetKeyState((int)Keys.Shift).ToString())));
           // bool ispressed = KeyboardInfo.GetKeyState(Keys.ShiftKey).IsPressed && KeyboardInfo.GetKeyState(Keys.ControlKey).IsPressed;
            bool ispressed = (KeyboardInfo.GetAsyncKeyState((int)Keys.ShiftKey) != 0);
            if (ispressed)
            {
                //show the "config" form.
                //convert Soundfolders and imagefolders string arrays to directoryInfo[] arrays..
                List<DirectoryInfo> sfolders = new List<DirectoryInfo>();
                List<DirectoryInfo> imgfolders = new List<DirectoryInfo>();
                List<DirectoryInfo> lvlfolders = new List<DirectoryInfo>();
                List<DirectoryInfo> pluginfolderslist = new List<DirectoryInfo>();
                List<DirectoryInfo> Templatefolderslist = new List<DirectoryInfo>();
                //Old for-each code...
                /*
                foreach (String loopsound in Soundfolders)
                {
                    if (Directory.Exists(loopsound))
                        sfolders.Add(new DirectoryInfo(loopsound));



                }   
                 foreach (String loopimage in Imagefolders)
                {
                    if (Directory.Exists(loopimage))
                        imgfolders.Add(new DirectoryInfo(loopimage));



                }
                foreach (String looplvl in LevelFolders)
                {
                    if (Directory.Exists(looplvl))
                        lvlfolders.Add(new DirectoryInfo(looplvl));

  foreach (String loopplugfolder in PluginFolders)
                {
                    if (Directory.Exists(loopplugfolder))
                        pluginfolderslist.Add(new DirectoryInfo(loopplugfolder));

                }

                }*/

                sfolders.AddRange(from n in Soundfolders where Directory.Exists(n) select new DirectoryInfo(n));
                imgfolders.AddRange(from img in Imagefolders where Directory.Exists(img) select new DirectoryInfo(img));
                lvlfolders.AddRange(from lvl in LevelFolders where Directory.Exists(lvl) select new DirectoryInfo(lvl));

                Templatefolderslist.AddRange(from templatefolder in Templatefolders where Directory.Exists(templatefolder) select new DirectoryInfo(templatefolder));

                pluginfolderslist.AddRange(from plug in PluginFolders where Directory.Exists(plug) select new DirectoryInfo(plug));
              


                FolderListView flview = new FolderListView(new string[] {"Sound","Image","Levels","Plugins"},new DirectoryInfo[][] { sfolders.ToArray(),imgfolders.ToArray(),lvlfolders.ToArray(),pluginfolderslist.ToArray()});
                flview.Text = "Data Folders";
                flview.ShowDialog();


            }
            #endregion
            //set tempdeleter class, so that when app is unloaded folder is destroyed.
            Type[] TypesToEnum = new Type[] {
             typeof(iLevelSetBuilder)   ,
             typeof(iBBPlugin) ,
             typeof(iBallBehaviour) ,
             typeof(iEditBlockFilter) , 
             typeof(Trigger),
             typeof(TriggerEvent) , 
             typeof(BlockEvent),
             typeof(BlockTrigger),
             typeof(Block) ,
             typeof(iSoundEngineDriver),
             typeof(GamePowerUp),
             typeof(GameObject),
             typeof(EnemyTrigger),typeof(BlockCategoryAttribute),
             typeof(IGameInitializer),
             typeof(IBindingExtension),
             typeof(GameCharacterPowerup),
             typeof(GameCharacterAbility),
             typeof(CollectibleOrb),
             typeof(TemplateCategory),
             typeof(iGameInput),
             typeof(iProjectile)
            };
            try
            {
                
                datahook.UpdateProgress(0.3f);



                //load the scripting assemblies.
                Assembly[] scriptassemblies = BBScript.CompileScripts(ScriptFolders.ToArray(), datahook);
                if(scriptassemblies !=null) Debug.Print("compiled " + scriptassemblies.Length + " assemblies from scripts.");
                //MessageBox.Show("instantiating MultiTypeManager...");
                
                MTypeManager = new MultiTypeManager(PluginFolders.ToArray(), TypesToEnum,datahook,
                     IncludeAssemblyTest,
                                               progresscallback,scriptassemblies);

               // MTypeManager = new MultiTypeManager(new Assembly[] { Assembly.GetExecutingAssembly() }, TypesToEnum, ignoreassemblies, datahook,
               //                                  progresscallback, scriptassemblies);
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);

            }
            datahook.UpdateProgress(0.5f);

            


            #region get all Sound Engine Driver types...



            //String[] ValidSoundEngines //= new string[] {"IRRKLANG","NBASS","NULLSOUND"};
            //Type[] SoundEngineTypes //= new Type[] { typeof(irrklangDriver), typeof(BASSDriver),typeof(NullSound) };

            LoadSoundDriverTypes(PluginFolders, datahook);
            datahook.UpdateProgress(0.7f);
            String[] ValidSoundEngines = SoundPluginTypes.Keys.ToArray();
            Type[] SoundEngineTypes = SoundPluginTypes.Values.ToArray();
            #endregion

            String sSoundEngineSetting = Settings.SoundEngine;
            

            if (sSoundEngineSetting.Equals("CHOOSE", StringComparison.OrdinalIgnoreCase) || KeyboardInfo.IsPressed(Keys.Alt))
            {
                bool SaveSelection;
                datahook.ShowMessage("Sound Helper/Driver set to Choose, or Alt pressed. Presenting User Interface...");
                String sSelected =frmChooser<String>.DoChoose(ValidSoundEngines.ToList(), "Select Sound Driver", "Choose Sound",out SaveSelection);

                if (SaveSelection)
                {

                    Settings.SoundEngine=sSelected;
                   

                }
                sSoundEngineSetting = sSelected;
                   
            }
            //SoundPluginSelector
        //    MessageBox.Show("Instantiating sound engine...");
            datahook.ShowMessage("INI file Sound engine: " + sSoundEngineSetting);

            Type useSoundEngineType = null;


            if (KeyboardInfo.IsPressed(Keys.ControlKey))
            {
                useSoundEngineType = SoundPluginSelector.ChooseSoundPlugin();

            }
            if (useSoundEngineType==null)
            {

                for (int i = 0; i < ValidSoundEngines.Length; i++)
                {
                    if (ValidSoundEngines[i].Equals(sSoundEngineSetting.ToUpper(), StringComparison.OrdinalIgnoreCase))
                    {
                        useSoundEngineType = SoundEngineTypes[i];
                        break;


                    }
                }
            }
            Soundman = new cNewSoundManager((iSoundEngineDriver)Activator.CreateInstance(useSoundEngineType, new object[] { SoundPluginFolder }), Soundfolders.ToArray(), datahook);
            cNewSoundManager.Callback = datahook;
            Debug.Print("Using Sound Engine:" + Soundman.Driver.Name);


           // Soundman = new cNewSoundManager(new BASSDriver(), Soundfoldername);
            //Soundman = new SoundManager(Soundfoldername);
            datahook.UpdateProgress(0.8f);
            float QualityReductionFactor = Settings.ReduceQualityAmount ;
            Size MinReduceSize = new Size(32, 16); //any image smaller or equal then to will not be shrunk (in either X or y directions)
            if (QualityReductionFactor < 1)
            {
                datahook.ShowMessage("Reducing Quality of Loaded Images...");
                Imagefolders = CreateTempReductionFolders(basetempfolder,Imagefolders, QualityReductionFactor, MinReduceSize, datahook);




            }



            mTempDeleter = new DeletionHelper(basetempfolder);
            Imageman = new ImageManager(Imagefolders.ToArray(),datahook);




            ToolbarImages = new ImageManager(Toolbarimagefolder.ToArray(), datahook);

           

            /*
            Levelman = new LoadedTypeManager(LevelFolders.ToArray(), typeof(iLevelSetBuilder),ignoreassemblies, datahook);
            if(PluginFolders.Count>0)
                PluginManager = new LoadedTypeManager(PluginFolders.ToArray(), typeof(iBBPlugin),ignoreassemblies, datahook);
            

            BallBehaviourManager = new LoadedTypeManager(new Assembly[]{Assembly.GetExecutingAssembly()},typeof(iBallBehaviour),ignoreassemblies,new Nullcallback());
            FilterPluginManager = new LoadedTypeManager(PluginFolders.ToArray(), typeof(iEditBlockFilter),ignoreassemblies, datahook);

            BlockTriggerTypes = new LoadedTypeManager(PluginFolders.ToArray(), typeof(Trigger), ignoreassemblies,datahook);
            EventTypes = new LoadedTypeManager(PluginFolders.ToArray(), typeof(TriggerEvent),ignoreassemblies, datahook);
            BlockTypes = new LoadedTypeManager(PluginFolders.ToArray(), typeof(Block), ignoreassemblies, datahook);
             * */
            datahook.UpdateProgress(0.9f);
            ParticleGenerationFactor = Settings.ParticleGenerationFactor;
            ShowDebugInfo = Settings.ShowDebugInfo;
            WaterBlockAnimations = Settings.WaterBlockAnimations ;

            //hack...
            if (Levelman.ManagedTypes.Count == 0)
            {
                Levelman.ManagedTypes.Add(typeof(DefaultLevelBuilder));
                Levelman.ManagedTypes.Add(typeof(ArkanoidLevelBuilder));

            }

            


            //final stage: call classes static methods who implement IGameInitializer...
            foreach (var looptype in MTypeManager[typeof(IGameInitializer)].ManagedTypes)
            {
                //attempt to call GameInitialize() on the object..
                MethodInfo grabmethod = looptype.GetMethod("GameInitialize", new Type[]{typeof(iManagerCallback)});
                if (grabmethod != null)
                {
                    //wrap in a try...
                    try
                    {
                        grabmethod.Invoke(null, BindingFlags.Static,null, new object[]{datahook}, Thread.CurrentThread.CurrentCulture);

                    }
                    catch (Exception exx)
                    {
                        Debug.Print("Calling GameInitialize on " + grabmethod.Name + " threw an exception:" + exx.ToString());


                    }


                }

            }


            BlockDataMan = new BlockDataManager();

            Templatefolders = (from p in Templatefolders where Directory.Exists(p) select p).ToList();

            usetemplatefolders = Templatefolders;
            

            datahook.UpdateProgress(1.0f);
        }
        private static List<String> usetemplatefolders = null;

        public enum DataSaveFormats{
        Format_Binary,
            Format_XML,
        
    }
        
        public static IFormatter getFormatter<T>(DataSaveFormats dataformat)
        {
            return getFormatter(typeof(T), dataformat);
        }
        public static IFormatter getFormatter(Type forType,DataSaveFormats dataformat)
        {
            IFormatter formatter = null;
            switch (dataformat)
            {
                case DataSaveFormats.Format_Binary:
                    formatter = new BinaryFormatter();
                    break;
                    case DataSaveFormats.Format_XML:
                    formatter =  new SoapFormatter();
                    break;
            }
            
            if (formatter is BinaryFormatter)
            {
                (formatter as BinaryFormatter).AssemblyFormat = System.Runtime.Serialization.Formatters.FormatterAssemblyStyle.Simple;

            }
            formatter.Binder = new SerializationHelper.VersionConfigToNamespaceAssemblyObjectBinder();
            return formatter;

        }
        private int _BossCounter = 0;
        public int BossCounter
        {

            get { return _BossCounter; }
            set
            {
                
                _BossCounter = value;
                if (_BossCounter <= 0)
                {
                    int usechannel = Trigger.GetAvailableID(this);
                    PlayingLevel.LevelEvents.Add(new FinishLevelEvent(usechannel));
                    //Trigger.InvokeTriggerID(usechannel, this);
                    DelayInvoke(new TimeSpan(0, 0, 0, 1), (obj) => Trigger.InvokeTriggerID(usechannel, this));
                }
            }
        }
        private static void LoadSoundDriverTypes(List<String> PluginFolders,iManagerCallback datahook)
        {
           // SoundDriverManager = new LoadedTypeManager(PluginFolders.ToArray(), typeof(iSoundEngineDriver), datahook);

           // SoundPluginTypes = new Dictionary<string, Type>();
            //For each type, load it, and get the name, and toss the new item into the dictionary.
            if (SoundPluginTypes == null) SoundPluginTypes = new Dictionary<string, Type>();
            

            foreach (Type looptype in SoundDriverManager.ManagedTypes)
            {

                //load an instance of this type. The "standard" method is to support two constructors, an empty constructor 
                //and one accepting a single Pluginfolder string argument. Why one? I don't know.
                iSoundEngineDriver sengine=null;
                String returnedname=null;
                try
                {
                   // sengine =
                    //    (iSoundEngineDriver) Activator.CreateInstance(looptype, (PluginFolders.ToArray()));
                    //no need to create instance. It should have a static method named "DrvName"...
                    
                    foreach(var loopitem in looptype.GetFields(BindingFlags.Public | BindingFlags.Static))
                    {

                        Debug.Print(loopitem.Name);


                    }


                    var gotmethod = looptype.GetField("DrvName", BindingFlags.Public | BindingFlags.Static);
                    //returnedname = (String)looptype.InvokeMember("DrvName", BindingFlags.Static,null,looptype,new object[]{});
                    returnedname = (String)gotmethod.GetValue(null);

                    
                }
                catch
                {
                    

                }
                
                if (returnedname == null)
                {
                    try
                    {
                        sengine =
                            (iSoundEngineDriver)Activator.CreateInstance(looptype, (PluginFolders.ToArray()));
                    }
                    catch
                    {
                        sengine =
                            (iSoundEngineDriver) Activator.CreateInstance(looptype);
                    }
                    //acquire the name...
                    String enginename = sengine.Name;
                    //dispose...
                    sengine = null;
                    returnedname = enginename;
                }


                //add the type and the name to the dictionary...
                if (!SoundPluginTypes.ContainsKey(returnedname))
                {

                    SoundPluginTypes.Add(returnedname, looptype);

                }







            }


        }

        private void InitializePlugins(iManagerCallback datahook)
        {
            LoadedPlugins = new List<iBBPlugin>();



            foreach (Type looptype in Levelman.ManagedTypes)
            {
                try
                {
                    iBBPlugin createdobj = (iBBPlugin)Activator.CreateInstance(looptype);
                    

                    if(createdobj.Initialize(this))
                        LoadedPlugins.Add(createdobj);
                    else
                    {
                        datahook.ShowMessage("Didn't load plugin:" + createdobj.getName());
                    }
                }
                catch
                {

                }





            }



        }

        private static String[] stdbyteprefixes = new String[]
    {
        " Bytes",
        "KB",
        "MB",
        "GB",
        "TB"
    };
        private static int getbyteprefixindex(long bytevalue)
        {
            int currindex = 0;
            long reduceit = bytevalue;
            while (reduceit > 1024)
            {
                reduceit /= 1024;
                currindex++;

            }
            return currindex;


        }
        public static String FormatSize(long amount)
        {
            int gotindex = getbyteprefixindex(amount);
            double calcamount = amount;
            for (int i = 0; i < gotindex; i++)
            {
                calcamount /= 1024;

            }
            return calcamount.ToString("F2", CultureInfo.InvariantCulture) + " " + stdbyteprefixes[gotindex];

        }
        /// <summary>
        /// given a font, a pixel height, and a graphics object, creates a new Font
        /// Object based on the one passed, changing the Point size to reflect the required
        /// size to be the desired height in pixels.
        /// </summary>
        /// <param name="BasedOn">Font to base calculation on</param>
        /// <param name="DesiredPixelHeight">Desired height, in pixels.</param>
        /// <param name="g">Graphics context</param>
        /// <returns>Font that can be used to draw text at the given pixel size.</returns>
        public static Font GetScaledFont(Font BasedOn, int DesiredPixelHeight, Graphics g)
        {

            //return BasedOn;
            int pointsuse = (int)((DesiredPixelHeight / g.DpiY)*72);
            Font createfont = new Font(BasedOn.FontFamily, pointsuse, BasedOn.Style, GraphicsUnit.Pixel);
            return createfont;


        }

        [DllImport("user32.dll")]
        private static extern IntPtr GetDesktopWindow();


        private static bool FontPresent(String testfont)
        {

            Font grabit = new Font(testfont, 18);
            return grabit.Name == testfont;


        }
        public static String GetMonospaceFont()
        {
            const string DefaultMonospace = "Consolas,Courier New,Liberation Mono,Liberation Sans";

            //list of candidates...
            var INIOption = Settings.MonospaceFonts;
            foreach(var loopoption in INIOption)
            {
                //determine if font exists.
                if (FontPresent(loopoption)) return loopoption;

            }


            return "Monospace";

            
        }
        

        private static Graphics Screeng = null;
        public static Font GetScaledFont(Font BasedOn,int DesiredPixelHeight)
        {
            if(Screeng==null)
            {
                Screeng= Graphics.FromHwnd(GetDesktopWindow());


            }
            return GetScaledFont(BasedOn,DesiredPixelHeight,Screeng);

        }
        public CollectibleOrb[] SpawnOrbs(int Count,PointF Position)
        {
            return SpawnOrbs(Count,Position, BCBlockGameState.MTypeManager[typeof(CollectibleOrb)].ManagedTypes.ToArray());

        }
        public CollectibleOrb[] SpawnOrbs(int Count, PointF Position, Type createtype)
        {
            return SpawnOrbs(Count, Position,new Type[] { createtype });


        }
        public CollectibleOrb[] SpawnOrbs(int Count, PointF Position,Type[] validtypes)
        {
            
            CollectibleOrb[] returnarray = new CollectibleOrb[Count];
            for (int i = 0; i < Count; i++)
            {
                //choose one of the types...
                Type spawntype = Choose(validtypes);
                //spawn it.
                //Constructor takes a single PointF.
                CollectibleOrb co = (CollectibleOrb)Activator.CreateInstance(spawntype, Position);
                //set a random velocity in the given range.
                co.Velocity = BCBlockGameState.GetRandomVelocity(0, 4);
                returnarray[i] = co;
                //add it to the gamestate, too.
                NextFrameCalls.Enqueue(new NextFrameStartup(() => GameObjects.AddLast(co)));
                //GameObjects.AddLast(co);
            }

            return returnarray;


        }


        private static String FormatSizeDirect(long amount, int index)
        {
            double amountuse=amount;
            for (int i = 0; i < index; i++)
            {
                amountuse /= 1024;
            }
            string buildresult = String.Format("0.00", amountuse);
            buildresult += " " + stdbyteprefixes[index];
            return buildresult;
        }

        /// <summary>
        /// formats a set of byte values to use the most honest display; that is, if we have 23 bytes and 1440 bytes, both will be displayed as bytes, but if it is 1330 and 1440, it shows as KB.
        /// 
        /// </summary>
        /// <param name="bytesizes"></param>
        /// <returns></returns>
        public static String[] FormatSizes(long[] bytesizes)
        {

            //iterate through all the elements, and find the lowest byteprefixindex...
            int currlowest = stdbyteprefixes.Length + 1;
            foreach (long looplong in bytesizes)
            {
                if (getbyteprefixindex(looplong) < currlowest) currlowest = getbyteprefixindex(looplong);


            }


            //currlowest is the current lowest value.
            String[] returnthis = new String[bytesizes.Length];

            for(int i=0;i<returnthis.Length;i++)
            {
                returnthis[i] = FormatSizeDirect(bytesizes[i],currlowest);

            }

            return returnthis;


        }


        public static void QueueDelete(String foldername)
        {
            if(!QueuedDeletions.Exists((q)=>(q.DeleteThis==foldername)))
            QueuedDeletions.Add(new DeletionHelper(foldername));


        }
        public static IEnumerable<T> Filter<T>(IEnumerable<T> enumerable, Func<T, bool> testpredicate)
        {
            return (from p in enumerable where testpredicate(p) select p);
        }

        public static bool HasAttribute(Type typecheck, Type checkforattribute)
        {
            return (System.Attribute.GetCustomAttributes(typecheck).Any((p) => p.GetType() == checkforattribute));

        }
        private static void ReduceImages(String sourcefolder,String DestFolder,float factor,iManagerCallback datahook)
        {

            //iterate through all the files in sourcefolder...
            DirectoryInfo sourcedir = new DirectoryInfo(sourcefolder);
            DirectoryInfo destdir = new DirectoryInfo(DestFolder);


            foreach (FileInfo loopfile in sourcedir.GetFiles())
            {
                
                if (ImageManager.isFileSupported(loopfile.FullName))
                {
                    //assume an image... so open it, resize it, and then save it to the new location. Create the appropriate new filename first.
                    String newfilename = Path.Combine(DestFolder,Path.GetFileName(loopfile.FullName));

                    
                    //we aren't keeping it around, so we should be fine using .FromImage...
                    Image resizeme = Image.FromFile(loopfile.FullName);
                    
                    //Addition: only images whose width and height are larger then 32 are reduced.
                    //also, don't resize images whose base name ends with "bg".
                    if (resizeme.Height > 32 && resizeme.Width > 32 && 
                        !(Path.GetFileNameWithoutExtension( loopfile.FullName).EndsWith("bg",StringComparison.OrdinalIgnoreCase)))
                    {
                        
                        //resize the image...
                        SizeF newsize = new SizeF(resizeme.Width*factor, resizeme.Height*factor);
                        if (newsize.Width<1) newsize=new SizeF(1,newsize.Height);
                        if (newsize.Height< 1) newsize = new SizeF(newsize.Width,1);

                        Image resizedimage = new Bitmap((int) newsize.Width, (int) newsize.Height);
                        

                        datahook.ShowMessage("Reducing image " + Path.GetFileName(loopfile.FullName) + "Size:" + resizeme.Size.ToString() + " to " + newsize.ToString());

                        using (Graphics usedraw = Graphics.FromImage(resizedimage))
                        {
                            usedraw.CompositingQuality = CompositingQuality.HighQuality;
                            usedraw.SmoothingMode = SmoothingMode.HighQuality;
                            usedraw.InterpolationMode = InterpolationMode.HighQualityBicubic;
                            usedraw.DrawImage(resizeme, new Rectangle(0, 0, (int) newsize.Width, (int) newsize.Height));
                        }
                        //save to new location.
                        resizedimage.Save(newfilename);
                    }
                    else
                    {
                        //the image is too small.
                        //simply copy the source to the new destination.
                        File.Copy(loopfile.FullName, newfilename);



                    }






                }





            }

            QueueDelete(DestFolder);



        }
        public static Bitmap Blur(Bitmap image, Int32 blurSize)
        {
            return Blur(image, new Rectangle(0, 0, image.Width, image.Height), blurSize);
        }
        public static Bitmap Blur(Bitmap image, Rectangle rectangle, Int32 blurSize)
        {
            Bitmap blurred = new Bitmap(image.Width, image.Height);

            // make an exact copy of the bitmap provided
            using (Graphics graphics = Graphics.FromImage(blurred))
                graphics.DrawImage(image, new Rectangle(0, 0, image.Width, image.Height),
                    new Rectangle(0, 0, image.Width, image.Height), GraphicsUnit.Pixel);

            // look at every pixel in the blur rectangle
            for (Int32 xx = rectangle.X; xx < rectangle.X + rectangle.Width; xx++)
            {
                for (Int32 yy = rectangle.Y; yy < rectangle.Y + rectangle.Height; yy++)
                {
                    Int32 avgR = 0, avgG = 0, avgB = 0;
                    Int32 blurPixelCount = 0;

                    // average the color of the red, green and blue for each pixel in the
                    // blur size while making sure you don't go outside the image bounds
                    for (Int32 x = xx; (x < xx + blurSize && x < image.Width); x++)
                    {
                        for (Int32 y = yy; (y < yy + blurSize && y < image.Height); y++)
                        {
                            Color pixel = blurred.GetPixel(x, y);

                            avgR += pixel.R;
                            avgG += pixel.G;
                            avgB += pixel.B;

                            blurPixelCount++;
                        }
                    }

                    avgR = avgR / blurPixelCount;
                    avgG = avgG / blurPixelCount;
                    avgB = avgB / blurPixelCount;

                    // now that we know the average for the blur size, set each pixel to that color
                    for (Int32 x = xx; x < xx + blurSize && x < image.Width && x < rectangle.Width; x++)
                        for (Int32 y = yy; y < yy + blurSize && y < image.Height && y < rectangle.Height; y++)
                            blurred.SetPixel(x, y, Color.FromArgb(avgR, avgG, avgB));
                }
            }

            return blurred;
        }


        private static List<DeletionHelper> QueuedDeletions = new List<DeletionHelper>();
        /// <summary>
        /// Takes a List of Strings that are folder names, and takes all the images in those folders, shrinks them by "factor", and stores them
        /// in temporary folders.
        /// </summary>
        /// <param name="temporarybasefolder">folder to use for storing the resized images</param>
        /// <param name="imagefolders">Set of folders containing image files.</param>
        /// <param name="factor">reduction factor; 0.5 would reduce quality by half.</param>
        /// <param name="minsize">minimum size for images to be resized; defaults to 32 by 16 (images with a width of 32 or less or a height of 16 or less will not be resized)</param>
        /// <param name="callback">ManagerCallback for displaying progress on the splash screen</param>
        /// <returns></returns>
        private static List<string> CreateTempReductionFolders(String temporarybasefolder,IEnumerable<string> imagefolders, float factor, Size minsize, iManagerCallback callback)
        {

            if(factor>1)
            {
                throw new ArgumentException("factor must be  between 0 and 1.","factor");

            }

            Debug.Print("Scaling Images by a factor of " + factor);

            DirectoryInfo reducefolder;
            //TODO: add the code to do this.
            if (!Directory.Exists(temporarybasefolder))
            {
                //we use Imageman.isFileSupported  to determine if a file can be loaded.
                

                reducefolder =Directory.CreateDirectory(temporarybasefolder);
            }
            else
            {
                reducefolder = new DirectoryInfo(temporarybasefolder);



            }
            callback.ShowMessage("Base temporary folder is \"" + reducefolder.FullName + "\"");
            List<String> returnedFolderNames=new List<string>();
            //iterate through each folder...
            foreach(String currfolder in imagefolders)
            {
                //create a folder of the same name in reducefolder.
                String createpath = Path.Combine(reducefolder.FullName, Path.GetFileName(currfolder));
                //if, by some chance, this folder already exists...
                if (Directory.Exists(createpath))
                {
                    //and if that existent directory is also present in the current "returnedFoldernames" list, meaning we just created it...
                    string value = createpath;
                    if (returnedFolderNames.Exists((q) => q.Equals(value)))
                    {
                        //then use the same name, but paste a GUID to the end...
                        createpath += Guid.NewGuid().ToString();


                    }
                    else
                    {
                        //otherwise, the directory exists but we haven't used it in this set;
                        //we'll use it again. (take no action here).
                    }
                }
                else
                {
                    //the directory "createpath" doesn't exist, so create it.
                    Directory.CreateDirectory(createpath);



                }

                //now we perform the actual logic of shrinking the images.
                //At this point, we simply need to take all supported image files in currfolder, resize them, and save them with the same
                //filename in createpath.

                ReduceImages(currfolder, createpath, factor,callback);



                returnedFolderNames.Add(createpath);






            
            }
            return returnedFolderNames;
            



            //callback.ShowMessage("QualityReduction for images not yet implemented.");
            //return imagefolders;

        }

        //ZipExtractFolder: extracts all zips from DataFolder into their own directories in a folder created in
        //the systems temporary folder. all subfolders of that new folder are returned in a Array.
        private static IEnumerable<DirectoryInfo> ZipExtractFolder(IEnumerable<string> DataFolders,out String basetempfolder)
        {
            DirectoryInfo tempfolder = new DirectoryInfo(Path.GetTempPath());
            //create a subdirectory. CACHE THIS SUBDIRECTORY! we will delete it when we close (in the finalizer)
            String mTempFolderName = Path.Combine(tempfolder.FullName,tempfoldername);
            if(Directory.Exists(mTempFolderName))
            {
                //delete the directory...
                Directory.Delete(mTempFolderName,true);



            }
            List<FileInfo> bbpfiles = new List<FileInfo>();
            DirectoryInfo newTempfolder = Directory.CreateDirectory(mTempFolderName);
            basetempfolder = mTempFolderName;
            foreach (String DFolder in DataFolders)
            {
                
                DirectoryInfo basefolder = new DirectoryInfo(DFolder);
               
                //now, enumerate ALL files in DataFolder, and then search for files that have the bbp extension.
                //and of course that are zipfiles.
               
                foreach (FileInfo loopfile in basefolder.GetFiles())
                {
                    //check the extension... 
                    if (loopfile.Extension.ToLower() == ".bbp" || loopfile.Extension.ToLower() == ".zip")
                        if (ZipFile.IsZipFile(loopfile.FullName))
                            bbpfiles.Add(loopfile);

                }
            }
            //now we have our bbpfiles that we need to load in the bbpfiles list.
            List<DirectoryInfo> bbpdirectories = new List<DirectoryInfo>();
            foreach (FileInfo bbpfile in bbpfiles)
            {
                //create a directory beneath newTempfolder that is the same as the base name of the zip; throw a GUID in there for no reason too.
                DirectoryInfo thisbbpdir = newTempfolder.CreateSubdirectory(bbpfile.Name + Guid.NewGuid().ToString());
                bbpdirectories.Add(thisbbpdir);
                //extract the contents of the file into this folder.

                ZipFile usezip = new ZipFile(bbpfile.FullName);
                usezip.ExtractAll(thisbbpdir.FullName);
                




            }




            return bbpdirectories.ToArray();

        }

        ~BCBlockGameState()
        {
            GameSettings.SaveINI(INIFileName);


        }

        public BCBlockGameState(iGameClient gamecli,Control pTargetObject,Rectangle pGameArea)
        {
            ClientObject=gamecli;
            TargetObject = pTargetObject;
            GameArea=pGameArea;

            //initialize our handler for the gamecli sourced input events...
         //   gamecli.ButtonDown += new Func<ButtonConstants, bool>(gamecli_ButtonDown);
        //    gamecli.ButtonUp += new Func<ButtonConstants, bool>(gamecli_ButtonUp);
      //      gamecli.MoveAbsolute += new Func<PointF, bool>(gamecli_MoveAbsolute);
            gamecli.ButtonDown += gamecli_ButtonDown;
            gamecli.ButtonUp += gamecli_ButtonUp;
            gamecli.MoveAbsolute += new Func<PointF, bool>(gamecli_MoveAbsolute);
//            PlayerPaddle = new Paddle(this,new Size(48, 15), new PointF(GameArea.Width / 2, TargetObject.Height - 35),Imageman.getLoadedImage("paddle"));
            
            PlayerPaddle = new Paddle(this, new Size(48, 15), new PointF(GameArea.Width / 2, TargetObject.Height - 35), Imageman.getLoadedImage("paddle"));
            
            //PlayerPaddle.Behaviours.Add(new StickyBehaviour(this));
            InitializePlugins(new Nullcallback());


            Soundman.Driver.OnSoundStop += new OnSoundStopDelegate(Driver_OnSoundStop);

        }
        public static BCBlockGameState MainGameState = null;
        void Driver_OnSoundStop(iActiveSoundObject objstop)
        {
            InvokeSoundStop(this, objstop.Source);
        }
        

        bool gamecli_MoveAbsolute(PointF arg)
        {
            //throw new NotImplementedException();
            if(PlayerPaddle==null) return true;
            if(!PlayerPaddle.Interactive ) return true;
                float useXcoord = 0;
                if (arg.X > GameArea.Width - (PlayerPaddle.PaddleSize.Width / 2))
                    useXcoord = GameArea.Width - (PlayerPaddle.PaddleSize.Width / 2);
                else if (arg.X < PlayerPaddle.PaddleSize.Width / 2)
                    useXcoord = PlayerPaddle.PaddleSize.Width / 2;
                else
                    useXcoord = arg.X;


                PlayerPaddle.Position = new PointF(useXcoord, PlayerPaddle.Position.Y);
                //PicGame.Invalidate(mGameState.PlayerPaddle.Getrect());
                //PicGame.Update();

            return true;

            
        }

        void gamecli_ButtonUp(Object sender, ButtonEventArgs<bool> e)
        {
            e.Result = true;
        }

        void  gamecli_ButtonDown(Object sender, ButtonEventArgs<bool> e)
        {
            //throw new NotImplementedException();
            e.Result = true;
        }

      
        //public void AddScore(int scoreadd)
        //{




 //       }

        public void BallHitBottom(cBall ballhit)
        {

            invokeballhitbottom(ballhit);



        }
        public enum BrushTypeConstants
        {
            Brush_Solid,   //SolidBrush
            Brush_Hatch,   //HatchBrush
            Brush_Texture, //TextureBrush
            Brush_LinearGradient, //LinearGradientBrush
            Brush_PathGradient, //PathGradientBrush

        }
        private static readonly Size SphereDrawSize = new Size(128, 128);
        private static readonly Size RectDrawSize = new Size(256, 128);
        private static Dictionary<Color, Image> SphereImageCache = new Dictionary<Color, Image>();
        private static Dictionary<Color, Image> RectImageCache = new Dictionary<Color, Image>(); 
        public static Image GetSphereImage(Color usecolor, Size forcesize)
        {
            Image grabimage = GetSphereImage(usecolor);
            Bitmap buildimage = new Bitmap(forcesize.Width, forcesize.Height);
            using (Graphics bcanvas = Graphics.FromImage(buildimage))
            {
                bcanvas.DrawImage(grabimage, 0, 0, forcesize.Width, forcesize.Height);


            }
            return buildimage;



        }
        public static PointF[] GenPoly(double RadiusMin,double RadiusMax,int minpoints,int maxpoints)
        {
        
            int PointCount = BCBlockGameState.rgen.Next(minpoints, maxpoints);

            var PolyPoints = new PointF[PointCount];
            float Angle = (float)((Math.PI * 2) / PointCount);
            for (int i = 0; i < PointCount; i++)
            {
                //generate this point...
                float RadiusUse = (float)((BCBlockGameState.rgen.NextDouble() * (RadiusMax - RadiusMin)) + RadiusMin);
                float useangle =Angle*i;
                PointF newPoint = new PointF((float)Math.Sin(useangle) * RadiusUse,(float) Math.Cos(useangle) * RadiusUse);

                PolyPoints[i] = newPoint;


            }



            return PolyPoints;



        }
        public static Image GetSphereImage(Color usecolor)
        {
            if (!SphereImageCache.ContainsKey(usecolor))
            {
                Bitmap DrawSphere = new Bitmap(SphereDrawSize.Width, SphereDrawSize.Height);
                Graphics ds = Graphics.FromImage(DrawSphere);
                ds.SmoothingMode= SmoothingMode.HighQuality;
                ds.InterpolationMode= InterpolationMode.Bicubic;

                //first ellipse.
                // Base ellipse
                Rectangle r1 = new Rectangle(new Point(0, 0), new Size(SphereDrawSize.Width, SphereDrawSize.Height));
                Rectangle r2 = new Rectangle(r1.Location, new Size(r1.Size.Width - 2, r1.Size.Height - 2));

                GraphicsPath path = new GraphicsPath(FillMode.Winding);
                path.AddEllipse(r2);
                PathGradientBrush br1 = new PathGradientBrush(path);
                br1.CenterColor = usecolor;
                br1.SurroundColors = new Color[] { Color.FromArgb(255, Color.Black) };
                br1.CenterPoint = new PointF((float)(r1.Width / 1.5), r1.Top - Convert.ToInt16(r1.Height * 2));

                Blend bl1 = new Blend(5);
                bl1.Factors = new float[] { 0.5f, 1.0f, 1.0f, 1.0f, 1.0f };
                bl1.Positions = new float[] { 0.0f, 0.05f, 0.5f, 0.75f, 1.0f };
                br1.Blend = bl1;

                ds.FillPath(br1, path);

                br1.Dispose();
                path.Dispose();

                // 1st hilite ellipse
                int r3w = Convert.ToInt16(r2.Width * 0.8);
                int r3h = Convert.ToInt16(r2.Height * 0.6);

                int r3posX = (r2.Width / 2) - (r3w / 2);
                int r3posY = r2.Top + 1;

                Rectangle r3 = new Rectangle(
                    new Point(r3posX, r3posY),
                    new Size(r3w, r3h));

                Color br3c1 = Color.White;
                Color br3c2 = Color.Transparent;

                LinearGradientBrush br2 = new LinearGradientBrush(r3, br3c1, br3c2, 90);
                br2.WrapMode = WrapMode.TileFlipX;
                ds.FillEllipse(br2, r3);

                br2.Dispose();

                // 2nd hilite ellipse
                int r4w = Convert.ToInt16(r2.Width * 0.3);
                int r4h = Convert.ToInt16(r2.Height * 0.2);

                int r4posX = (r2.Width / 2) + (r4w / 2);
                int r4posY = r2.Top + Convert.ToInt16(r2.Height * 0.2);

                Rectangle r4 = new Rectangle(
                    new Point(-(int)(r4w / 2), -(int)(r4h / 2)),
                    new Size(r4w, r4h));

                LinearGradientBrush br3 = new LinearGradientBrush(r4, br3c1, br3c2, 90, true);
                ds.TranslateTransform(r4posX, r4posY);
                ds.RotateTransform(30);
                ds.FillEllipse(br3, r4);

                br3.Dispose();

                SphereImageCache.Add(usecolor, DrawSphere);

                




            }
            return SphereImageCache[usecolor];

        }

        public static T Select<T>(T[] items, float[] Probabilities)
        {
            return Select(items, Probabilities, new Random());

        }
        public static T Select<T>(T[] items, float[] Probabilities, Random rgen)
        {
            float[] sumulator = null;
            return Select(items, Probabilities, rgen, ref sumulator);

        }
        public static T Select<T>(T[] items, float[] Probabilities, ref float[] sumulations)
        {
            return Select(items, Probabilities, new Random(), ref sumulations);

        }
        public static T Select<T>(T[] items, float[] Probabilities, Random rgen, ref float[] sumulations)
        {
            //first, sum all the probabilities; unless a cached value is being given to us.
            //we do this manually because we will also build a corresponding list of the sums up to that element.
            float getsum = 0;
            if (sumulations == null)
            {
                sumulations = new float[Probabilities.Length + 1];
                for (int i = 0; i < Probabilities.Length; i++)
                {

                    sumulations[i] = getsum;
                    getsum += Probabilities[i];
                }

                sumulations[sumulations.Length - 1] = getsum; //add this last value in...
            }
            else
            {
                getsum = sumulations[sumulations.Length - 1];
            }
            //get a percentage using nextDouble. we use doubles, just in case the probabilities array uses rather large numbers to attempt to prevent
            //abberations as a result of floating point errors.
            double usepercentage = rgen.NextDouble();
            //convert this percentage into a value we can use, that corresponds to the sum of float values:
            float searchtotal = (float)(usepercentage * getsum);
            //now find the corresponding index and return the corresponding value in the items array.
            for (int i = 0; i < Probabilities.Length; i++)
            {
                if (searchtotal > sumulations[i] && searchtotal < sumulations[i + 1])
                    return items[i];

            }
            return default(T);
        }





        public static Image GetGummyImage(Color usecolor, Size usesize)
        {
            Image grabimage = GetGummyImage(usecolor);
            Bitmap buildimage = new Bitmap(usesize.Width, usesize.Height);
            using (Graphics bcanvas = Graphics.FromImage(buildimage))
            {
                bcanvas.DrawImage(grabimage, 0, 0, usesize.Width, usesize.Height);


            }
            return buildimage;



        }
        public static Image GetGummyImage(Color usecolor)
        {
            if (!RectImageCache.ContainsKey(usecolor))
            {
                Bitmap DrawGummy = new Bitmap(RectDrawSize.Width, RectDrawSize.Height);
                Graphics ds = Graphics.FromImage(DrawGummy);
                ds.SmoothingMode = SmoothingMode.HighQuality;
                ds.InterpolationMode = InterpolationMode.Bicubic;

                //first ellipse.
                // Base ellipse
                Rectangle r1 = new Rectangle(new Point(0, 0), new Size(RectDrawSize.Width, RectDrawSize.Height));
                Rectangle r2 = new Rectangle(r1.Location, new Size(r1.Size.Width - 2, r1.Size.Height - 2));

                GraphicsPath path = new GraphicsPath(FillMode.Winding);
                path.AddRectangle(r2);
                PathGradientBrush br1 = new PathGradientBrush(path);
                br1.CenterColor = usecolor;
                br1.SurroundColors = new Color[] { Color.FromArgb(255, Color.Black) };
                br1.CenterPoint = new PointF((float)(r1.Width / 1.5), r1.Top - Convert.ToInt16(r1.Height * 2));

                Blend bl1 = new Blend(5);
                bl1.Factors = new float[] { 0.5f, 1.0f, 1.0f, 1.0f, 1.0f };
                bl1.Positions = new float[] { 0.0f, 0.05f, 0.5f, 0.75f, 1.0f };
                br1.Blend = bl1;

                ds.FillPath(br1, path);

                br1.Dispose();
                path.Dispose();

                // 1st hilite ellipse
                int r3w = Convert.ToInt16(r2.Width * 1);
                int r3h = Convert.ToInt16(r2.Height * 0.6);

                int r3posX = (r2.Width / 2) - (r3w / 2);
                int r3posY = r2.Top + 1;

                Rectangle r3 = new Rectangle(
                    new Point(r3posX, r3posY),
                    new Size(r3w, r3h));

                Color br3c1 = Color.White;
                Color br3c2 = Color.Transparent;

                LinearGradientBrush br2 = new LinearGradientBrush(r3, br3c1, br3c2, 90);
                br2.WrapMode = WrapMode.TileFlipX;
                ds.FillRectangle(br2, r3);

                br2.Dispose();

                // 2nd hilite ellipse
                int r4w = Convert.ToInt16(r2.Width * 0.3);
                int r4h = Convert.ToInt16(r2.Height * 0.2);

                int r4posX = (r2.Width / 2) - (r4w / 2);
                int r4posY = r2.Top + Convert.ToInt16(r2.Height * 0.2);

                Rectangle r4 = new Rectangle(
                    new Point(-(int)(r4w / 2), -(int)(r4h / 2)),
                    new Size(r4w, r4h));

                LinearGradientBrush br3 = new LinearGradientBrush(r4, br3c1, br3c2, 90, true);
                ds.TranslateTransform(r4posX, r4posY);
                ds.RotateTransform(-30);
                ds.FillEllipse(br3, r4);

                br3.Dispose();

                RectImageCache.Add(usecolor, DrawGummy);






            }
            return RectImageCache[usecolor];

        }
        public void PersistBrush(Brush brushpersist, String PropertyName,SerializationInfo info, StreamingContext context)
        {
            
            SolidBrush sbrush = brushpersist as SolidBrush;

            //is it a solid brush?

            if (sbrush != null)
            {
                //if so, persist it's colour, as well as it's type.
                info.AddValue(PropertyName + "BrushType",(int) BrushTypeConstants.Brush_Solid);
                info.AddValue(PropertyName + "BrushColour", sbrush.Color);
                
                return;



            }
            HatchBrush hbrush = brushpersist as HatchBrush;
            // is it a hatchbrush?
            if (hbrush != null)
            {
                info.AddValue(PropertyName + "BrushType", BrushTypeConstants.Brush_Hatch);
                info.AddValue(PropertyName + "BrushBackground", hbrush.BackgroundColor);
                info.AddValue(PropertyName + "BrushForeground", hbrush.ForegroundColor);




            }





        }
        public static PointF MidPoint(PointF PointA, PointF PointB)
        {

            return new PointF(PointA.X + ((PointB.X - PointA.X) / 2), PointA.Y + ((PointB.Y - PointA.Y) / 2));

        }
        public static Image DrawTextToImage(String Text, Font useFont, Brush useBrush, Pen usePen)
        {
            Size CalcSize = TextRenderer.MeasureText(Text, useFont);
            return DrawTextToImage(Text, useFont, useBrush, usePen,CalcSize);


        }

        public static Image DrawTextToImage(String Text, Font useFont, Brush useBrush, Pen usePen, Size DesiredSize)
        {
            //Draw the text centered.
            //First create the graphics context and bitmap.
            Bitmap DrawText = new Bitmap(DesiredSize.Width,DesiredSize.Height);
            using (Graphics Textg = Graphics.FromImage(DrawText))
            {
                //Determine where to draw first.
                Size Textsize = TextRenderer.MeasureText(Textg, Text, useFont);

                Point DrawPosition = new Point((DesiredSize.Width / 2) - (Textsize.Width / 2), (DesiredSize.Height / 2) - (Textsize.Height / 2));
                using (GraphicsPath usepath = new GraphicsPath())
                {
                    usepath.AddString(Text, useFont, DrawPosition, StringFormat.GenericDefault);
                    //stroke & fill...
                    if(useBrush!=null)
                        Textg.FillPath(useBrush, usepath);
                    if(usePen!=null)
                        Textg.DrawPath(usePen, usepath);

                }


            }

            return DrawText;

        }
        public static Image ScaleImage(Image source, float Factor)
        {
            Size newsize = new Size((int)((float)source.Size.Width * Factor), (int)((float)source.Size.Height * Factor));
            Bitmap scaledimage = new Bitmap(newsize.Width, newsize.Height);
            using (Graphics guse = Graphics.FromImage(scaledimage))
            {
                guse.DrawImage(source, 0, 0, newsize.Width, newsize.Height);

            }
            return scaledimage;


        }
        public PointF AimAtPaddle(PointF Location, float initialspeed)
        {
            PointF mVelocity;
            PointF targetposition;
            var currstate = this;
            //if the paddle exists...
            if (currstate.PlayerPaddle != null)
                targetposition = currstate.PlayerPaddle.Position;
            else
            {
                //otherwise, choose a random spot.
                Random genner = rgen;
                targetposition = new PointF((float)(currstate.GameArea.Left + (currstate.GameArea.Width * genner.NextDouble())),
                                            (float)(currstate.GameArea.Top + (currstate.GameArea.Height * genner.NextDouble())));
            }
            double angleuse = GameObject.Angle((double)Location.X, (double)Location.Y, (double)targetposition.X,
                                    (double)targetposition.Y);
            float usespeed = initialspeed;
            double degree = (Math.PI * 2) / 360;
            angleuse += (degree * 2) - degree;

            mVelocity = new PointF((float)Math.Cos(angleuse) * usespeed,
                                  (float)Math.Sin(angleuse) * usespeed);
            return mVelocity;
        }


        //attempts to mix between the two colours, the given percentage.
        public static Color MixColor(Color FirstColor,Color SecondColor,float Percentbetween)
        {
            //first we get the RGBA differences.
            float[] Firstdata = new float[] { FirstColor.R, FirstColor.G, FirstColor.B,FirstColor.A };
            float[] Diffs = new float[] { SecondColor.R-FirstColor.R,
                                        SecondColor.G-FirstColor.G,
                                        SecondColor.B-FirstColor.B,
                                        SecondColor.A-FirstColor.A};
            //multiply each by the percentage.
            Diffs = (from c in Diffs select  c * Percentbetween).ToArray();

            //now select the firstcolor component plus this new difference, clamped.
            int[] result = new int[Firstdata.Length];
            for (int i = 0; i < result.Length; i++)
            {

                result[i] = (int)ClampValue(Firstdata[i] + Diffs[i], 0, 255);

            }
            return Color.FromArgb(result[3], result[0], result[1], result[2]);
        }
        public static Image DrawLevelToImage(Level leveldraw)
        {
            return DrawLevelToImage(leveldraw, DefaultLevelSize);

        }

        public static Image DrawLevelToImage(Level leveldraw,Size DrawSize)
        {
            //create a new bitmap; make it the same size as the PicEditor picturebox...
            Bitmap imgDraw = new Bitmap(DrawSize.Width, DrawSize.Height);
            //create a graphics object for it...
            
            Graphics drawgraphics = Graphics.FromImage(imgDraw);
            drawgraphics.Clear(Color.White);
            Brush usebgBrush = null;
            /* if (!String.IsNullOrEmpty(leveldraw.BackgroundPicKey))
             {
                 var gotframes = BCBlockGameState.Imageman.getImageFrames(leveldraw.BackgroundPicKey);
                 if (gotframes == null|| gotframes.Length==0)
                 {
                     gotframes = new Image[] { BCBlockGameState.Imageman.getLoadedImage(leveldraw.BackgroundPicKey) };

                 }
                

                 usebgBrush = new TextureBrush(gotframes[0]);

             }
             else
                 usebgBrush = new SolidBrush(leveldraw.BackgroundColor);
             drawgraphics.FillRectangle(usebgBrush, 0, 0, imgDraw.Width, imgDraw.Height);
             * */
            leveldraw.Background.DrawBackground(null, drawgraphics, new Rectangle(0, 0, imgDraw.Width, imgDraw.Height), true);

            foreach (Block drawblock in leveldraw.levelblocks)
            {
                drawblock.Draw(drawgraphics);



            }
            foreach (cBall drawball in leveldraw.levelballs)
            {
                drawball.Draw(drawgraphics);


            }


            return imgDraw;

        }
        public static float Distance(float X,float Y,float X2,float Y2)
        {
            return (float)Math.Sqrt(Math.Pow(X2 - X, 2) + Math.Pow(Y2 - Y, 2));



        }
        

        public static PointF[] GetEllipsePoints(RectangleF EllipseRect)
        {
            GraphicsPath gp = new GraphicsPath();
            gp.AddEllipse(EllipseRect);
            gp.Flatten();
            return gp.PathPoints;
            



        }

        public static PointF[] GetEllipsePoints(PointF foci1, PointF foci2, float Radius,out float EllipseAngle,out RectangleF ellipserectangle)
        {
            //first we want to condense it to a rectangle. 

            float MinX = Math.Min(foci1.X, foci2.X);
            float MinY = Math.Min(foci1.Y, foci2.Y);
            float MaxX = Math.Max(foci1.X, foci2.X);
            float MaxY = Math.Max(foci1.Y, foci2.Y);


            //we'll need the angle between the midpoint and one of the focus points. Probably doesn't matter which...

            PointF middlespot = MidPoint(foci1, foci2);


            EllipseAngle = (float)GetAngle(middlespot, foci1);
            float totalwidth = MaxX - MinX + (Radius * 2);
            float totalheight = MaxY - MinY + (Radius * 2);
            //armed with the midpoint and the width and height, we can create a rectangle.

            RectangleF userectangle = new RectangleF(middlespot.X - totalwidth / 2, middlespot.Y - totalheight / 2, totalwidth, totalheight);
            //get the rectangle's points...
            ellipserectangle=userectangle;
            PointF[] rectpoints = GetEllipsePoints(userectangle);

            for (int i = 0; i < rectpoints.Length; i++)
            {
                RotatePoint(middlespot,ref rectpoints[i], EllipseAngle);


            }

            return rectpoints;

        }
        public enum Coordinates2D
        {
            COORD_X,
            COORD_Y


        }
        public static void GetEllipseAxes(RectangleF ellipserect, out float Major, out float Minor,out Coordinates2D majoraxis)
        {
            if (ellipserect.Width > ellipserect.Height)
            {
                Major = ellipserect.Width / 2;
                Minor = ellipserect.Height / 2;
                majoraxis = Coordinates2D.COORD_X;
            }
            else
            {
                Minor = ellipserect.Width / 2;
                Major = ellipserect.Height / 2;
                majoraxis = Coordinates2D.COORD_Y;
            }



        }

        public static void RotatePoint(PointF centerpoint, ref PointF rotatepoint, float angle)
        {
            float useangle = (float)GetAngle(centerpoint, rotatepoint) +angle;
            float distance = Distance(centerpoint, rotatepoint);

            float newX = (float)(Math.Cos(useangle) * distance) + centerpoint.X;
            float newY = (float)(Math.Sin(useangle) * distance) + centerpoint.Y;

            rotatepoint = new PointF(newX, newY);





        }

        public static PointF getClosestPointToEllipse(PointF originpoint,RectangleF EllipseRect)
        {
            return getClosestPoint(originpoint, GetEllipsePoints(EllipseRect));


        }

        public static PointF RotateVector(PointF pointrotate,double angle)
        {
            //rotates the given vector by the specified amount.


            var ca = Math.Cos(angle);
            var sa = Math.Sin(angle);
            var x = pointrotate.X;
            var y = pointrotate.Y;
            return new PointF((float)(x * ca - y * sa), (float)(x * sa + y * ca));

            /*

             * 
            var ca=Math.cosD(angle);
var sa=Math.sinD(angle);
with(this){
var rx=x*ca-y*sa;
var ry=x*sa+y*ca;
x=rx;
y=ry;
            
            */





        }
     


        /// <summary>
        /// Given a Location, a Velocity, and a second Location and a function, changes the angle of Velocity to move towards the Towards location.
        /// 
        /// </summary>
        /// <param name="Location"></param>
        /// <param name="Velocity"></param>
        /// <param name="Towards"></param>
        /// <returns></returns>
        public static PointF NudgeTowards(PointF pLocation, PointF pVelocity, PointF pTarget, Func<float, float> NudgeFunction)
        {
            double TargetAngle = GetAngle(pLocation, pTarget);
            double CurrentAngle = GetAngle(PointF.Empty, pVelocity);
            PointF v1 = new PointF((float)Math.Cos(CurrentAngle), (float)Math.Sin(CurrentAngle));
            PointF v2 = new PointF((float)Math.Cos(TargetAngle), (float)Math.Sin(TargetAngle));
            float dot = v1.X*v2.X + v1.Y*v2.Y;
            float cross = v1.X*v2.Y - v1.Y*v2.X;
            double AngleDiff = Math.Atan2(cross, dot);
            
            //TargetAngle = TargetAngle < 0 ? TargetAngle + Math.PI * 2 : TargetAngle;
            //CurrentAngle = CurrentAngle < 0 ? CurrentAngle + Math.PI * 2 : CurrentAngle;
            //double AngleDiff = AngleDifference(CurrentAngle, TargetAngle);
            
            float nudgeamount = NudgeFunction((float)AngleDiff);
            Debug.Print(AngleDiff.ToString() + " Nudge:" + nudgeamount);
            nudgeamount = (float)Math.Min(nudgeamount, AngleDiff);
            CurrentAngle += nudgeamount;
            return new PointF((float)Math.Cos(CurrentAngle), (float)Math.Sin(CurrentAngle));

        }
     
        /*
        public static PointF NudgeTowards(PointF Location, PointF Velocity, PointF Target, Func<float,float> NudgeFunction)
        {
            Swap(ref Target, ref Location);

            //w = (v1.X * v2.Y) - (v1.Y * v2.X)
            double CurrentSpeed = Velocity.Magnitude();
            
            double TargetAngle = GetAngle(Location, Target);
            double CurrentAngle = GetAngle(new PointF(0, 0), Velocity);

            PointF TargetVector = new PointF((float)Math.Cos(TargetAngle), (float)Math.Sin(TargetAngle));
            PointF CurrentVector = new PointF((float)Math.Cos(CurrentAngle), (float)Math.Sin(CurrentAngle));


            TargetVector = TargetVector.Normalize();
            CurrentVector = CurrentVector.Normalize();

            PointF v1 = TargetVector, v2 = CurrentVector;

            //calculate crossproduct...

            double w = (v1.X * v2.Y) - (v1.Y * v2.X);

            float dot = (v1.X * v2.X) + (v1.Y * v2.Y);
            double theta = Math.Acos(dot);
            float Amount =NudgeFunction((float)theta);
            CurrentAngle+=(float)(Amount)*(w>0?1:-1);

            if (theta < Amount)
                CurrentAngle = TargetAngle;

            return new PointF((float)(Math.Cos(CurrentAngle) * CurrentSpeed), (float)(Math.Sin(CurrentAngle) * CurrentSpeed));


        }*/
        public static PointF NudgeTowards(PointF Location, PointF Velocity, PointF Towards, float Amount)
        {

            return NudgeTowards(Location, Velocity, Towards, (w) => Math.Sign(w)* Amount);

        }
        public static double GetAngle(PointF PointA, PointF PointB)
        {
            return   Math.Atan2(PointB.Y - PointA.Y, PointB.X - PointA.X);
           // var result = SignedAngleTo(new Vector3(PointA.X, PointA.Y, 0), new Vector3(PointB.X, PointB.Y, 0), new Vector3(0, 0, 1));
           // return result;

        }
        public static double AngleDifference(double AngleA, double AngleB)
        {
            //float dif = (float)Math.Abs(a1 - a2) % 360;

            //if (dif > 180)
                //dif = 360 - dif;

            double diff = Math.Abs(AngleA - AngleB) % (Math.PI * 2);
            diff = diff > Math.PI ? (Math.PI * 2) - diff : diff;
            return diff;


        }
        public static bool DoesBallTouch(RectangleF checkrect,cBall hitball)
        {
        
           // return hitball.Location.X + hitball.Radius > BlockRectangle.Left &&
           //     hitball.Location.X - hitball.Radius < BlockRectangle.Right &&
           //     hitball.Location.Y + hitball.Radius > BlockRectangle.Top &&
           //     hitball.Location.Y - hitball.Radius < BlockRectangle.Bottom;
            return hitball.getRect().IntersectsWith(checkrect);

        }
        public static Image AppyImageAttributes(Image applyto, ImageAttributes applyattribs)
        {
            Image newimage = new Bitmap(applyto.Width, applyto.Height);
            Graphics usegraph = Graphics.FromImage(newimage);
            usegraph.DrawImage(applyto, new Rectangle(0, 0, applyto.Width, applyto.Height), 0, 0, applyto.Width, applyto.Height, GraphicsUnit.Pixel, applyattribs);



            return newimage;
        }

        public static bool CheckImpact(BCBlockGameState currentgamestate,RectangleF withrect, cBall hitball,out Block.BallRelativeConstants ballrelmode)
        {
            //return true to destroy this block.
            //basic Block->Ball collision detection. 
            //step one: check if the ball hit this block.


            //if the ball isn't moving, return false.
            ballrelmode = Block.BallRelativeConstants.Relative_None;
            if (hitball.Velocity.X == 0 && hitball.Velocity.Y == 0)
            {
                
                return false;

            }

            //first see if it's within hitball.radius of the left and right...

            if (DoesBallTouch(withrect,hitball))
            {
                if (hitball is FrustratorBall)
                {
                    Debug.Print("Frustrate");

                }
                //remove this block from the game.
                //TODO: change code to not redraw all blocks every frame. (first implementation will take after dodger and redraw
                //everything after every frame

                //((BCBlockGameState)(ParentGame.Target)).Soundman.PlaySound("BBOUNCE",1f);
                //direction should change here based on it's relative position...

                
                    ballrelmode = Block.getBallRelativeOld(hitball,withrect);

                    if ((ballrelmode & Block.BallRelativeConstants.Relative_Left) != 0)
                        hitball.Velocity = new PointF(-Math.Abs(hitball.Velocity.X), hitball.Velocity.Y);

                    if ((ballrelmode & Block.BallRelativeConstants.Relative_Up) != 0)
                        hitball.Velocity = new PointF(hitball.Velocity.X, -Math.Abs(hitball.Velocity.Y));

                    if ((ballrelmode & Block.BallRelativeConstants.Relative_Right) != 0)
                        hitball.Velocity = new PointF(Math.Abs(hitball.Velocity.X), hitball.Velocity.Y);

                    if ((ballrelmode & Block.BallRelativeConstants.Relative_Down) != 0)
                        hitball.Velocity = new PointF(hitball.Velocity.X, Math.Abs(hitball.Velocity.Y));

                



               // washit = PerformBlockHit(currentgamestate, hitball, ref ballsadded);
                return true;

            }
           
            return false;
        }

        #region iGameInput Members

        public void ButtonDown(ButtonConstants buttonpressed)
        {
            //throw new NotImplementedException();
        }

        public void ButtonUp(ButtonConstants buttonreleased)
        {
            //throw new NotImplementedException();
        }

        public void MoveAbsolute(PointF newlocation)
        {
            //throw new NotImplementedException();
        }

        #endregion

        public static BlockDataManager BlockDataMan = null;


        private static Graphics usemeasureg = null;
        public static Graphics getmeasureg()
        {
            if (usemeasureg == null)
            {
                Bitmap measurebit = new Bitmap(1, 1);
                usemeasureg = Graphics.FromImage(measurebit);
                usemeasureg.PageUnit = GraphicsUnit.Point;
            }
            return usemeasureg;

        }

        public static SizeF MeasureString(string s, Font fontuse)
        {
            Graphics tempgraph = getmeasureg();
            tempgraph.PageUnit = GraphicsUnit.Point;
            return tempgraph.MeasureString(s, fontuse);
        }




        /*
        /// <summary>
        /// routine that generates a set of "overlay" images consisting for cracks that appear on the sides of the image going inwards.
        /// </summary>
        /// <param name="gensize">Size of the image to generate</param>
        /// <param name="keybase">Name of key to use as base. frame number will be appended to this for each image.</param>
        /// <param name="numframes">Number of frames to generate</param>
        /// <returns></returns>
       
        public static IEnumerable<Image> GenerateCrackImages(Size gensize,String keybase, int numframes)
        {
            //PointF[][] PrevLines; //previous set of lines.
            //PointF[][] currlines;


            







        }
         
        */
        public static void getExtents(IEnumerable<PointF> ofset, out PointF Minimum, out PointF Maximum)
        {
            float minX = float.MaxValue, minY = float.MaxValue;
            float maxX = float.MinValue, maxY = float.MinValue;
            //iterate through every point.
            foreach (PointF iteratepoint in ofset)
            {
                if (iteratepoint.X < minX) minX = iteratepoint.X;
                if (iteratepoint.X > maxX) maxX = iteratepoint.X;

                if (iteratepoint.Y < minY) minY = iteratepoint.Y;
                if (iteratepoint.Y > maxY) maxY = iteratepoint.Y;



            }
            Minimum = new PointF(minX, minY);
            Maximum = new PointF(maxX, maxY);

        }

        public static PointF CenterPoint(PointF[] ofset)
        {
            //retrieve the center point of the given set.

            //retrieve the maximum and minimum X and Y coordinates.
            PointF max, min;
            getExtents(ofset, out min, out max);
            return new PointF(min.X + ((max.X - min.X) / 2), min.Y + ((max.Y - min.Y) / 2));




        }

        public static string GenerateSnowImage(RectangleF pblockrectangle)
        {
            return GenerateRandomImage(pblockrectangle, new double[] { 170, 170 }, new double[] { 0, 25 }, new double[] { 180, 255 },"SNOW");

        }
        public static bool SegmentIntersectsCircle(PointF P, PointF Q, PointF c, float r)
        {

            return DistanceToLine(c, P, Q) < r;

            //If in the triangle PQC one of the two angles on the segment PQ is not acute, 
            //then moving along the segment PQ and away from the non-acute angle the distance 
            //from the center C increases, so that there is no intersection in this case because of 1).
            //Otherwise, the two angles on the segment PQ are acute, so that the minimum distance between the 
            //center C and the points on the segment PQ is achieved inside the segment. In this case 
            //(i.e. when the two angles on PQ are acute), we make sure that the distance between the center C 
            //and the line joining P and Q is larger than r. In view of 1) and the previous discussion this 
            //implies that there is no contact between the segment and the disk.


        }
        public static float DistanceToLine(PointF CheckPoint, PointF PointA, PointF PointB)
        {

            float x = CheckPoint.X;
            float y = CheckPoint.Y;
            float x1 = PointA.X;
            float y1 = PointA.Y;
            float x2 = PointB.X;
            float y2 = PointB.Y;
            

            float A = x - x1;
            float B = y - y1;
            float C = x2 - x1;
            float D = y2 - y1;

            float dot = A * C + B * D;
            float len_sq = C * C + D * D;
            float param = dot / len_sq;

            float xx,yy;

            if(param < 0)
            {    xx = x1;
                yy = y1;
            }
            else if(param > 1)
            {    xx = x2;
                yy = y2;
            }
            else
            {    xx = x1 + param * C;
                yy = y1 + param * D;
            }

            float dist = Distance(x,y,xx,yy);
            //your distance function

            return dist;

}
        public static bool RectangleIntersectsCircle(RectangleF rectangle, PointF centerpoint, float Radius)
        {

            return RectangleCircleDistance(rectangle, centerpoint, Radius) != -1; //distance gives us -1 for no intersection at all, otherwise
            //we're given the distance.


        }

        public static float RectangleCircleDistance(RectangleF rectangle,PointF centerpoint,float Radius)
        {
            PointF[] TopLine = new PointF[] { new PointF(rectangle.Left, rectangle.Top), new PointF(rectangle.Right, rectangle.Top) };
            PointF[] RightLine = new PointF[] { new PointF(rectangle.Right, rectangle.Top), new PointF(rectangle.Right, rectangle.Bottom) };
            PointF[] BottomLine = new PointF[] { new PointF(rectangle.Right, rectangle.Bottom), new PointF(rectangle.Left, rectangle.Bottom) };
            PointF[] LeftLine = new PointF[] { new PointF(rectangle.Left, rectangle.Bottom), new PointF(rectangle.Left, rectangle.Top) };


            List<PointF[]> Lines = new List<PointF[]>();
            Lines.Add(TopLine);
            Lines.Add(RightLine);
            Lines.Add(BottomLine);
            Lines.Add(LeftLine);

            foreach (var looppoints in Lines)
            {


                if (SegmentIntersectsCircle(looppoints[0], looppoints[1], centerpoint, Radius))
                {
                    return DistanceToLine(centerpoint, looppoints[0], looppoints[1]);

                }


            }
            return -1; //-1 indicates no intersection.

        }

        /// <summary>
        /// Creates a Randomized image colourized to a "tan" colour; adds it to the Image manager, and returns the resulting key.
        /// </summary>
        /// <param name="pblockrectangle"></param>
        /// <returns></returns>

        public static string GenerateSandImage(RectangleF pblockrectangle)
        {

            return GenerateRandomImage(pblockrectangle, Color.Tan,15,"SAND");
        

        }
        public void CreateExplosion(PointF Location,float Radius)
        {
            ExplosionEffect explosioneffectobj = new ExplosionEffect(Location, Radius);

            GameObjects.AddLast(explosioneffectobj);
            //BCBlockGameState.Soundman.PlaySound("explode");



        }
        public static TimeSpan TimeSpanFromFloat(float seconds)
        {
            TimeSpan build = new TimeSpan();
            float ms = (float)(seconds - Math.Floor(seconds));
            seconds -=ms;

            var TotalDays = (int)(Math.Floor(seconds / 60 / 60 / 24));
            seconds -= (TotalDays * 24 * 60 * 60);
            var Hourvalue = (int)(Math.Floor(seconds / 60 / 60));
            seconds -= (Hourvalue * 60 * 60);
            var MinuteValue = (int)(Math.Floor(seconds * 60));
            seconds -= (MinuteValue * 60);
            return new TimeSpan(TotalDays, Hourvalue, MinuteValue, (int)seconds, (int)(ms*1000));
            
            



        }

    public static void ObjectToStream<T>(T saveme, Stream outstream) where T : ISerializable
    {

        IFormatter bf = getFormatter<T>(BCBlockGameState.DataSaveFormats.Format_Binary);
        using(GZipStream gz = new GZipStream(outstream,CompressionMode.Compress))
        {
            bf.Serialize(gz, saveme);


            
        }



    }
    public static T StreamToObject<T>(Stream instream) where T : ISerializable
    {

        IFormatter bf = getFormatter<T>(BCBlockGameState.DataSaveFormats.Format_Binary);
        using(GZipStream gz = new GZipStream(instream,CompressionMode.Decompress))
        {

            return (T)bf.Deserialize(gz);
            
        }



    }

        public static Image FadeImage(Image FadeImage, Image pBackgroundImage, int Alpha)
        {
            Image BackgroundImage = null;
            if (pBackgroundImage == null)
                BackgroundImage = new Bitmap(FadeImage.Width, FadeImage.Height);
            else
                BackgroundImage = (Image)pBackgroundImage.Clone();


            using (Graphics backgroundcanvas = Graphics.FromImage(BackgroundImage))
            {
                ImageAttributes useia = new ImageAttributes();
                useia.SetColorMatrix(ColorMatrices.GetFader(Alpha));
                backgroundcanvas.DrawImage(FadeImage, new Rectangle(0, 0, FadeImage.Width, FadeImage.Height), 0, 0, FadeImage.Width, FadeImage.Height, GraphicsUnit.Pixel, useia);

            }


            return BackgroundImage;

        }
        public static string GenerateRandomImage(RectangleF pblockrectangle, Color basecolor, double variation,string prefix)
        {
            //get hue, sat, and lum from the color.
            var parts = new HSLColor(basecolor);
            var componentranges = new []{
                new double[] {0,240},
                new double[] {0,255},
                new double[] {0,240}


            };
            double[] components = new double[] { parts.Hue, parts.Saturation, parts.Luminosity };

            double[] minranges = new double[components.Length];
            double[] maxranges = new double[components.Length];
            for (int i = 0; i < components.Length; i++)
            {
                minranges[i] = ClampValue(components[i] - variation, componentranges[i][0], componentranges[i][1]);
                maxranges[i] = ClampValue(components[i] + variation, componentranges[i][0], componentranges[i][1]);


            }

            return GenerateRandomImage(pblockrectangle, new double[] { minranges[0], maxranges[0] },
                new double[] { minranges[1], maxranges[1] },
                new double[] { minranges[2], maxranges[2] }, prefix);


        }
        /// <summary>
        /// Creates a randomized image using Hue,Saturation, and Luminousity ranges.
        /// </summary>
        /// <param name="pblockrectangle"></param>
        /// <param name="huerange"></param>
        /// <param name="satrange"></param>
        /// <param name="lumrange"></param>
        /// <param name="prefix"></param>
        /// <returns></returns>

        public static string GenerateRandomImage(RectangleF pblockrectangle, double[] huerange, double[] satrange, double[] lumrange, String prefix)
        {

            Bitmap createbitmap = new Bitmap((int)pblockrectangle.Width, (int)pblockrectangle.Height);


            String genname = (prefix + pblockrectangle.ToString() + BCBlockGameState.rgen.Next(0, 500).ToString()).ToUpper();


            //set each pixel.
            //28,162,148
       

            
            
            for (int x = 0; x < createbitmap.Width;x++ )
            {
                for (int y = 0; y < createbitmap.Height; y++)
                {
                    double usehue = BCBlockGameState.rgen.NextDouble(huerange[0], huerange[1]);
                    double usesat = BCBlockGameState.rgen.NextDouble(satrange[0], satrange[1]);
                    double uselum = BCBlockGameState.rgen.NextDouble(lumrange[0], lumrange[1]);
                    createbitmap.SetPixel(x, y, new HSLColor(usehue, usesat, uselum));
                    //createbitmap.SetPixel(x, y, Color.Red);






                }


            }

            
            //bitmap created. add to images set; create a new name for it though.
            BCBlockGameState.Imageman.AddImage(genname, createbitmap);
            return genname;


        }
        /// <summary>
        /// Adds a ball, block, GameObject, or Particle to the appropriate State List.
        /// </summary>
        /// <param name="Element"></param>
        public void AddElement(Object Element)
        {
            if (Element is cBall)
            {
                lock (Balls)
                {
                    Balls.AddLast((cBall)Element);
                }
            }
            else if (Element is Block)
            {
                Blocks.AddLast((Block)Element);
            }
            else if (Element is GameObject)
            {
                GameObjects.AddLast((GameObject)Element);
            }
            else if (Element is Particle)
            {
                Particles.Add((Particle)Element);
            }
        }
    }
}
 
