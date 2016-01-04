using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using BASeCamp.BASeBlock.Blocks;
using BASeCamp.BASeBlock.HighScores;

namespace BASeCamp.BASeBlock
{
    public class InvisibleBuilderAttribute : Attribute
    {
        


    }
    public abstract class BaseLevelBuilder : iLevelSetBuilder
    {
        /*
   public static class Levelbuilderextension
   {
       public static IHighScoreList getHighScores(this iLevelSetBuilder lbuilder)
       {

           return BCBlockGameState.Levelman.GetScoreDictionary()[lbuilder.GetType()];
       }
       public static void setHighScores(this iLevelSetBuilder lbuilder, IHighScoreList setscores)
       {
           BCBlockGameState.Levelman.GetScoreDictionary()[lbuilder.GetType()] = setscores; 


       }



   }
   */




        #region iLevelSetBuilder Members

        public abstract string getName();

        public abstract string getDescription();
        

        public abstract LevelSet BuildLevelSet(RectangleF targetrect,IWin32Window Owner);        

        public virtual IHighScoreList HighScores
        {
            get
            {
                return BCBlockGameState.Scoreman[this.GetType().Name];
            }
            set
            {
                BCBlockGameState.Scoreman[this.GetType().Name] = value as LocalHighScores;
            }
        }

        public virtual bool RequiresConfig()
        {
            return false;

        }

        public virtual void Configure()
        {

            throw new NotImplementedException("Configure Not implemented in Base DefaultLevelBuilder.");

        }
       

        #endregion
    }
    /*
    public class RandomLevelBuilder : BaseLevelBuilder
    {
        private class RandomLevelData
        {
            String useseed; //seed to use for random generator; empty to use timer, or use current seed when creating a "run" of levels.
            Dictionary<Type,double> BlockWeights = new Dictionary<Type, double>();

            public double Total
            {
                get {
                    double accum = 0;
                    foreach (var loopvalue in BlockWeights)
                    {
                        accum += loopvalue.Value;

                    }
                    return accum;


                }


            }
               /* public Type SelectOne(float percentage)
            {







            }
            
            public RandomLevelData()
            {
                foreach (var looptype in BCBlockGameState.BlockTypes.ManagedTypes)
            {
                BlockWeights.Add(looptype, 1.0);

            }

            }

        }

        private Random rg;

        public RandomLevelBuilder()
        {
            


        }

        public override string getName()
        {
            return "RandomLevelBuilder";
        }

        public override string getDescription()
        {
            return "Random level builder";
        }

        public override LevelSet BuildLevelSet(RectangleF targetrect,IWin32Window Owner)
        {
            //throw new NotImplementedException();
        }
        public override bool RequiresConfig()
        {
            return true;
        }
        public override void Configure()
        {
            //base.Configure();


        }
        
    }
    */

    [InvisibleBuilder]
    public class CreditLevelBuilder : BaseLevelBuilder
    {
        private String[] TitleNames = new string[] {
         "Concept:","Inspired by 'Arkanoid'",
         "Ball Design:","Mathematics",
         "Ball Behaviour Programming:","Michael Burgwin",
         "Ball Graphics:","Nonexistent",
         "Block Design:","Michael Burgwin & Graham Mulreay",
         "Block Behaviour Programming:","Michael Burgwin",
         "Block Graphics:","Michael Burgwin & Graham Mulreay",
         "Powerup Design:","Michael Burgwin & Graham Mulreay",
         "Powerup Programming:","Michael Burgwin",
         "Powerup Graphics:","Michael Burgwin & Graham Mulreay",
         "Enemy Design:","Michael Burgwin",
         "Enemy Programming:","Michael Burgwin",
         "Boss Enemy Programming:","Michael Burgwin",
         "Paddle design:","Michael Burgwin",
         "Paddle concept art:","Michael Burgwin",
         "Paddle nonentity reclamation programming:","Michael Burgwin",
         "Paddle Behavior design:","Michael Burgwin",
         "Paddle reintegration programming:","Michael Burgwin",
         "--------------------------------","____________________",
         "Special Thanks\n------","Tux2","Graham Mulreay",
         "Cameron Gray (what for, no idea)","Markus 'Notch' Persson for minecraft",
         "Oranges.","Tomatoes, I guess.",
         "Hell if I'm going","to thank them I ",
         "May as well thank ",
         "lettuce as well", "and lastly",
         "but certainly not","leastly, I would",
         "Like to offer my ","sincerest adoration",
         "For","THE GREAT POSSUM.","","","","","","","",
         "You're still here then.","",
         "Well this is awkward.",
         "You know those times when a ",
         "guest overstays their welcome?",
         "",
         "This is one of those times. ",
         "There are no more credits.",
         "Go away.",
         "","","","","","","","","","","","","",
         "You're a persistent bugger.",
         "Still here after all that?",
        "Surely this music is ",
        "driving you mad by this stage",
        "So what do you expect to gain?",
        "No macguffins here."
        ,"No Power Stars.",
        "No treasure at all.",
        "So go away. Stop reading this."
        ,"","","","","","","","","","","","","","",
        "Oh for the love of...",
        "Why are you still here?",
        "Have you seriously nothing better to do?",
        "Please. Do something productive.",
        "Build a kite. than set it on fire.",
        "Tell-em BC sent you.",
        "That made no sense.",
        "Do you see what you've",
        "Driven me too? Nonsense.",
        "I  hope you're pleased. Bastard."
        




         


        
            
            
            
            
        };
        private string[] Titlevalues;
        private string[] namevalues;
        private void Splitresources()
        {
            Titlevalues = new string[TitleNames.Length/2+1];
            namevalues = new string[TitleNames.Length / 2+1];
            int numentries = 0;
            for(int i=0;i<TitleNames.Length-1;i+=2)
            {
                Titlevalues[numentries] = TitleNames[i];
                namevalues[numentries] = TitleNames[i + 1];
                numentries++;

            }
        }

        public override string getName()
        {
            return "Credits";
        }

        public override string getDescription()
        {
            return "The credits level.";
        }

        public override LevelSet BuildLevelSet(RectangleF targetrect,IWin32Window Owner)
        {
            Splitresources();
            //create the credits level.
            LevelSet newset = new LevelSet();
            Level createlevel = new Level();
            cBall addball = new cBall(new PointF(targetrect.Width / 2-90, targetrect.Height - 60), new PointF(0, 0));
            createlevel.levelballs.Add(addball);
            cBall addball2 = new cBall(new PointF((targetrect.Width / 2) + 40, targetrect.Height -60), new PointF(0, 0));
            createlevel.levelballs.Add(addball2);
            createlevel.IntroMusicName = "bosskill";
            
            createlevel.LevelName = "Credits...";
            createlevel.MusicName = "creditsong";
            
            float currtimeindex=5;
            for(int i=0;i<TitleNames.Length;i+=2)
            {

                String Titlepart = TitleNames[i];
                String desc = TitleNames[i + 1];

                createlevel.MessageData.Add(new PlayMessageData(currtimeindex,Titlepart));
                createlevel.MessageData.Add(new PlayMessageData(currtimeindex+1, desc));
                currtimeindex+=4;
            }
            PointF middlebottom = new PointF(targetrect.Width/2+80,targetrect.Bottom);
            NormalBlock nb = new NormalBlock(new RectangleF(0, 0, 64, 64));
            createlevel.levelblocks.Add(nb);
            createlevel.NoPaddle = true;
            newset.Levels.Add(createlevel);
            return newset;
            //create two creditblocks.
            /*
             * 
            CreditBlock TitleNameBlock = new CreditBlock(new RectangleF(targetrect.Left+72, targetrect.Bottom - 64, 32, 64),Titlevalues , middlebottom);
            CreditBlock NameBlock = new CreditBlock(new RectangleF(targetrect.Right - 72-32, targetrect.Bottom - 64, 32, 64), namevalues, middlebottom);
            InvincibleBlock bottomhold = new InvincibleBlock(new RectangleF(targetrect.Left, targetrect.Height - 5, targetrect.Width, 32));
            InvincibleBlock separator = new InvincibleBlock(new RectangleF(targetrect.Width / 2 - 8, targetrect.Height - 90, 16, 90));
            TitleNameBlock.RemovePrevious =false;
            NameBlock.RemovePrevious=false;
            TitleNameBlock.neverdestroy=true;
            NameBlock.neverdestroy=true;
            createlevel.levelblocks.Add(separator);
            createlevel.levelblocks.Add(TitleNameBlock);
            createlevel.levelblocks.Add(NameBlock);
            createlevel.levelblocks.Add(bottomhold);
            createlevel.MusicName = "creditsong";
            createlevel.LevelName = "CREDITS";
            createlevel.LevelNameIntroTextFillColor=Color.White;
            createlevel.LevelnameIntroFont = new Font("Arial", 48);
            //createlevel.BackgroundPic = Image.FromFile("D:\\2011-03-08_20.12.46.png");
            createlevel.NoPaddle=true;
            newset.Levels.Add(createlevel);
            return newset;
             * */
        }
    }

    public class ArkanoidLevelBuilder : BaseLevelBuilder 
    {



        private IHighScoreList _SetHighScores = BCBlockGameState.Scoreman[typeof(ArkanoidLevelBuilder).Name];

        #region iLevelSetBuilder Members
        
        public override string getName()
        {
            return "Basic Levels";
        }

        public override string getDescription()
        {
            return "Creates very basic levels, consisting of the simpler blocks; No animated blocks are used.";
        }

        public override LevelSet BuildLevelSet(RectangleF targetrect, IWin32Window Owner)
        {
            LevelSet returnset = new LevelSet();
            returnset.SetName = "Basic Levels";

            for (int i = 1; i < 10; i++)
            {
                returnset.Levels.Add(BuildLevel(i, targetrect));



            }


            return returnset;

        }
        private Level BuildLevel(int levelnumber, RectangleF PicGame)
        {
            Level builtlevel = new Level();
            builtlevel.LevelName = "Basic Level " + levelnumber;
            var usebg = new BackgroundColourImageDrawer(Color.Gray);
            builtlevel.Background = usebg;
            usebg.BackgroundFrameKeys = new string[] { "mainbg" };
            usebg.RotateSpeed = 0;
            usebg.MoveVelocity=PointF.Empty;
            cBall createdball = new cBall(new PointF(PicGame.Width / 2, PicGame.Height - 50), new PointF(-2f, -2f));
            //createdball.Behaviours.Add(new LinearGravityBallBehaviour(0.04));
            builtlevel.levelballs.Add(createdball);
            //create 5 rows of blocks; select from the following types:
            Type[] basicblocks = new Type[] { typeof(NormalBlock), typeof(AddBallBlock),typeof(DemonBlock), typeof(StrongBlock), typeof(SpeedBallBlock),typeof(BombBlock),typeof(RayBlock) };
            const int blockheight = 16;
            const int blockwidth = 32;
            const int stackedsize = 5;
            for (int x = 0; x+32 < PicGame.Width; x += blockwidth) 
            {

                for (int y = 0; y < stackedsize * blockheight; y += 16)
                {
                    int randomvalue = BCBlockGameState.rgen.Next(basicblocks.Count());
                    Type createthis = basicblocks[randomvalue];
                    if (createthis == typeof(DemonBlock))
                        Debug.Print("Found demon block");
                    Block createdblock = (Block)Activator.CreateInstance(createthis,(Object)(new RectangleF((float)x,(float)y,32,16)));
                    builtlevel.levelblocks.Add(createdblock);
                }
            }
            builtlevel.levelblocks.Add(new BlackHoleBlock(new RectangleF(PicGame.Width/2,stackedsize*+blockheight+blockheight,32,16)));
            // builtlevel.levelblocks.Add(new CreditBlock(new RectangleF(PicGame.Width / 2, stackedsize * +blockheight + blockheight, 32, 16),new string[]{"Possum","And","friends","eat","delicious","Garbage"}));
            
            return builtlevel;

        }




        #endregion
    }

    public class LevelBuilder_FromStream : BaseLevelBuilder
    {
        public EditorSet BuilderSet = null;
        public LevelBuilder_FromStream()
        {


        }
        public override LevelSet BuildLevelSet(RectangleF targetrect,IWin32Window Owner)
        {
            if (BCBlockGameState.SetFromEditor == null)
            {
                MessageBox.Show("No EditorSet is currently loaded in the editor. Use File->Save Test or Close the editor.");
                return null;
            }
            else
            {







                BuilderSet = BCBlockGameState.SetFromEditor;
                return BuilderSet.LevelData;
            }
        }
        public override string getDescription()
        {
            return "Loaded Editor Set";
        }
        public override string getName()
        {
            return "(From Editor)";
        }


    }

    public class LevelBuilderFromFile : BaseLevelBuilder
    {
        private EditorSet BuilderLevelSet;
        private String currentfilename;
        private IHighScoreList useHighScores;
        public LevelBuilderFromFile(String filename)
        {
            currentfilename=filename;
            BuilderLevelSet = EditorSet.FromFile(filename);
            useHighScores = BuilderLevelSet.LevelData.HighScores;

        }
        public LevelBuilderFromFile()
        {
            

        }
        public void getopenName(IWin32Window Owner)
        {
            String DefaultlsPath = Path.Combine(BCBlockGameState.AppDataFolder, "Levelsets");
            String useopenpath = BCBlockGameState.Settings.LevelsFolder.Count > 0 ? BCBlockGameState.Settings.LevelsFolder.First() : DefaultlsPath;
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.InitialDirectory = useopenpath;
            
            ofd.Filter = BCBlockGameState.GetFileFilter();
            
            if (ofd.ShowDialog(Owner) != DialogResult.Cancel)
            {
                while (BuilderLevelSet == null)
                {

                    BuilderLevelSet = EditorSet.FromFile(ofd.FileName);
                    if (BuilderLevelSet == null)
                    {
                        switch(MessageBox.Show(Owner,"Unable to Open File","File Error",MessageBoxButtons.RetryCancel,MessageBoxIcon.Error))
                        {
                            case DialogResult.Retry:
                                break;
                                case DialogResult.Cancel:
                                throw new Exception("File open failed");

                        }

                    }
                }
                currentfilename = ofd.FileName;
                
//                useHighScores = BCBlockGameState.Scoreman[BuilderLevelSet.Author + BuilderLevelSet.Version + BuilderLevelSet.LevelData.SetName + BuilderLevelSet.LevelData.MD5Hash()];
                useHighScores = BCBlockGameState.Scoreman[BuilderLevelSet.Author + BuilderLevelSet.Version + BuilderLevelSet.LevelData.SetName ];
                

            }



        }

        public override IHighScoreList HighScores
        {
            get
            {
                return useHighScores;
            }
            set
            {
                useHighScores=value;
                
            }
        }

        public override string getName()
        {
            return "From File...";
        }

        public override string getDescription()
        {
            return "Load a Levelset from a file";
        }

        public override LevelSet BuildLevelSet(RectangleF targetrect,IWin32Window Owner)
        {
            if (BuilderLevelSet == null)
            {
                try
                {
                    getopenName(Owner);
                }
                catch (Exception exx)
                {
                    return null;

                }
            }
            BCBlockGameState.MRU["Game"].AddToList(currentfilename);
            if(BuilderLevelSet==null) return null;
            return BuilderLevelSet.LevelData;
            
        }
    }
    
    public class AdvancedLevelBuilder : BaseLevelBuilder
    {
        private static Random mRandom = BCBlockGameState.rgen;
        private IHighScoreList _SetHighScores = BCBlockGameState.Scoreman[typeof(AdvancedLevelBuilder).Name];

        public override string getName()
        {
            return "Advanced Builder";
        }

        public override string getDescription()
        {
            return "Creates a Varied set of levels with different themes";
        }

        public override LevelSet BuildLevelSet(RectangleF targetrect, IWin32Window Owner)
        {
            LevelSet Buildthis = new LevelSet();
            Buildthis.SetName = "BASeBlock:Core";
            for (int i = 0; i < 20; i++)
            {
                Level madeLevel = CreateLevel(targetrect,i);
                Buildthis.Levels.Add(madeLevel);

            } return Buildthis;

        }

        private static float[] genProbabilities(int length,float min = 1, float max = 100)
        {

            return (from p in Enumerable.Range(1, length) select (float)(min + ((BCBlockGameState.rgen.NextDouble()) * (max - min)))).ToArray();  


        }
        public Level CreateLevel(RectangleF Target,int levelnum)
        {

            Type[] UseTypes;
            Type[] AllTypes = BCBlockGameState.BlockTypes.ManagedTypes.ToArray();
            
            Level MakeLevel = new Level();
            MakeLevel.LevelName = NameGenerator.Gen.GenerateLevelName();
            int AddBlockTypes = BCBlockGameState.rgen.Next(2, AllTypes.Length);

            UseTypes = BCBlockGameState.Choose(AllTypes, AddBlockTypes);
            float[] probabilities = genProbabilities(UseTypes.Length);

         
            //create a randomized, base level. we'll twaddle this a bit later.
            
                    SizeF useblockSize = new Size(32, 16);
                    float CurrentX=0, CurrentY = 0;
                    for (int i = 0; i < 160; i++ )
                    {
                        RectangleF chosenposition = new RectangleF(CurrentX, CurrentY, useblockSize.Width, useblockSize.Height);


                        //choose the block type.
                        Block createdblock = null;

                        while (createdblock == null)
                        {
                            try
                            {
                                Type selectedType = BCBlockGameState.Select(UseTypes, probabilities);
                                //instantiate...
                                createdblock = Activator.CreateInstance(selectedType, chosenposition) as Block;
                                MakeLevel.levelblocks.Add(createdblock);
                            }
                            catch { }
                        }
                       

                        


                        CurrentX += useblockSize.Width;
                        if (CurrentX > Target.Width)
                        {
                            CurrentY += useblockSize.Height;
                            CurrentX = 0;
                        }



                    }


            //we've added the blocks, now make patchwork....

            //choose another three blocks to use as patches.
                    Type[] patchblobs = BCBlockGameState.Choose(BCBlockGameState.MTypeManager[typeof(Block)].ManagedTypes, 3);

                    foreach (Type iterate in patchblobs)
                    {
                        Debug.Print("Adding patch for " + iterate.ToString());
                        //choose a random origin within the level.
                        Block useposition = MakeLevel.levelblocks[BCBlockGameState.rgen.Next(MakeLevel.levelblocks.Count)];
                        ReplaceLocation(useposition, MakeLevel, iterate,0);
                        





                    }



            //let us not forget to add a ball...
                    cBall createball = new cBall(new PointF(Target.Width / 2, Target.Height / 2),new PointF(3,3));
                    createball.Radius = (float)(BCBlockGameState.rgen.NextDouble() * 3 + 4);
                    MakeLevel.levelballs.Add(createball);

            


            
            return MakeLevel;




        }


        private void ReplaceLocation(Block Source,Level inLevel,Type replacewith,int recursecount)
        {
            if (recursecount > 3) return;
            try
            {
                Block replacedpos = Activator.CreateInstance(replacewith, Source.BlockRectangle) as Block;
                //remove old.
                inLevel.levelblocks.Remove(Source);
                //add new.
                inLevel.levelblocks.Add(replacedpos);
                PointF cp = replacedpos.CenterPoint();
                PointF[] newpositions = new PointF[] {new PointF(
                cp.X-replacedpos.BlockSize.Width/2,cp.Y),
                new PointF(cp.Y+replacedpos.BlockSize.Width/2,cp.Y),
                new PointF(cp.X,cp.Y-replacedpos.BlockSize.Height/2),
                new PointF(cp.X,cp.Y+replacedpos.BlockSize.Height/2),
                new PointF(cp.X+replacedpos.BlockSize.Width/2,cp.Y+replacedpos.BlockSize.Height/2),  //++
                new PointF(cp.X+replacedpos.BlockSize.Width/2,cp.Y-replacedpos.BlockSize.Height/2), //+-
                new PointF(cp.X-replacedpos.BlockSize.Width/2,cp.Y-replacedpos.BlockSize.Height/2), //--
                new PointF(cp.X-replacedpos.BlockSize.Width/2,cp.Y+replacedpos.BlockSize.Height/2)  //-+
                };
                //select only points that have blocks and that are not of the type we are adding.
                newpositions = (from p in newpositions let q = BCBlockGameState.Block_HitTestOne(inLevel.levelblocks, p) where q!=null && q.GetType()!=replacewith select p).ToArray();


                if (newpositions.Count() > 2)
                {
                    foreach (var iterate in BCBlockGameState.Choose(newpositions, BCBlockGameState.rgen.Next(2) + 1))
                    {

                        //retrieve Block at this position.

                        Block grabbed = BCBlockGameState.Block_HitTestOne(inLevel.levelblocks, iterate);
                        ReplaceLocation(grabbed, inLevel, replacewith, recursecount++);

                    }
                }

            }
            catch (Exception exx) { }

                
            
        

        }



    }

    
    public class DefaultLevelBuilder :BaseLevelBuilder
    {
        private static Random mRandom = BCBlockGameState.rgen;
        private IHighScoreList _SetHighScores = BCBlockGameState.Scoreman[typeof(DefaultLevelBuilder).Name];

        private Color RandomColor()
        {
            return Color.FromArgb((int)(mRandom.NextDouble() * 128 + 128), (int)(mRandom.NextDouble() * 255), (int)(mRandom.NextDouble() * 255), (int)(mRandom.NextDouble() * 255));



        }
        private Level CreateDefaultLevel(int levelnumber,RectangleF PicGame)
        {

            Level returnlevel = new Level();



            returnlevel.levelballs.Add(new cBall(new PointF(PicGame.Width / 2, PicGame.Height - 50), new PointF(-2f, -2f)));
            cBall thepowerball = new cBall(new PointF(PicGame.Width / 2 + 45, PicGame.Height - 50), new PointF(2f, -2f));
            thepowerball.Behaviours.Add(new PowerBallBehaviour());
            returnlevel.levelballs.Add(thepowerball);
            returnlevel.LevelName = "Level " + levelnumber.ToString();
            returnlevel.MusicName = "BASESTOMP";
            returnlevel.ShowNameLength = new TimeSpan(0,0,0,5);
            returnlevel.Background = new BackgroundColourImageDrawer("mainbg");
            Type[] Softblocks = new Type[] {typeof(NormalBlock),typeof(AddBallBlock),typeof(SpeedBallBlock),typeof(BombBlock)};
            Type[] HardBlocks = new Type[]{typeof(InvincibleBlock),typeof(StrongBlock)};
            if (levelnumber == 2)
            {
                //returnlevel.MusicName = @"D:\music\duke3dmus\highres\music\stalker.ogg";
                Type addblocktype=null; 
                for (int x = 0; x < PicGame.Width; x += 32)
                {


                    for (int y = 0; y < PicGame.Height / 2; y += 16)
                    {
                        if ((x % 3) == 0)
                            addblocktype = BCBlockGameState.Choose(HardBlocks, 1)[0];
                        //if (y % 3 == 0)
                        //returnlevel.levelblocks.Add(new RayBlock(new RectangleF(x, y, 32, 16)));
                        //else
                        //returnlevel.levelblocks.Add(new InvincibleBlock(new RectangleF(x, y, 32, 16)));
                        else
                        {
                            addblocktype = BCBlockGameState.Choose(Softblocks, 1)[0];
                            //returnlevel.levelblocks.Add(new NormalBlock(new RectangleF(x, y, 32, 16),
                            //            new SolidBrush(RandomColor()),
                            //            new Pen(Color.Black, 1)));
                        }

                        returnlevel.levelblocks.Add((Block)Activator.CreateInstance(addblocktype, new Object[] { (Object)new RectangleF(x, y, 32, 16) }));


                    }
                }

            }
            else
            {
                
                /*
                
                //for(int q=0;q<5;q++)
                    

                GraphicsPath usepath = new GraphicsPath();
                //usepath.AddEllipse(50, 50, 120, 120);
                //usepath.CloseFigure();
                String charsadd = "Writing in Cursive";


                usepath.AddEllipse(50, 50, 200, 200);
                usepath.AddEllipse(50, 250, 200, 200);
                //usepath.AddString(charsadd, new FontFamily("Lucida Handwriting"), (int)FontStyle.Regular, 48f, new PointF(),StringFormat.GenericDefault);
                //usepath.AddString("8", FontFamily.GenericMonospace, (int)FontStyle.Regular, 512f, new PointF(), StringFormat.GenericDefault);

                


                for(int q=0;q<10;q++)
                {
                    returnlevel.levelblocks.Add(
                        new PathedMovingBlock(
                            new RayBlock(new RectangleF(movingorigin.X + (32), movingorigin.Y + (16), 32, 16)),
                    usepath,
                    q*70))
                    ;
                }

                */

                for (int x = 0; x < PicGame.Width-32; x += 32)
                {


                    for (int y = 0; y < PicGame.Height / 3; y += 16)
                    {
                        if (mRandom.NextDouble() < 0.2)
                        {
                            if (mRandom.NextDouble() < 0.4)
                                //returnlevel.levelblocks.Add(new BombBlock(new RectangleF(x, y, 32, 16)));
                                //returnlevel.levelblocks.Add(new AnimatedImageBlock(new RectangleF(x, y, 32, 16),new Image[] {BCBlockGameState.Imageman.getLoadedImage("BOMB"),BCBlockGameState.Imageman.getLoadedImage("RAYBLOCK")}));
                                returnlevel.levelblocks.Add(new StrongBlock(new RectangleF(x, y, 32, 16)));
                            else
                                if (mRandom.NextDouble() < 0.5)
                                {
                                    if (mRandom.NextDouble() < 0.6)
                                        returnlevel.levelblocks.Add(new GrowBlock(new RectangleF(x, y, 32, 16)));
                                    else
                                        returnlevel.levelblocks.Add(
                                        (StrongBlock)
                                        new StrongBlock(new RectangleF(x, y, 32, 16)));

                                }
                                else
                                {
                                    returnlevel.levelblocks.Add((SpeedBallBlock)new SpeedBallBlock(new RectangleF(x, y, 32, 16)));


                                }


                        }
                        else if (mRandom.NextDouble() > 0.3)
                            returnlevel.levelblocks.Add(new NormalBlock(new RectangleF(x, y, 32, 16),
                                                                        new SolidBrush(RandomColor()),
                                                                        new Pen(Color.Black, 1)));
                        else if (mRandom.NextDouble() > 0.5)
                            returnlevel.levelblocks.Add(new BombBlock(new RectangleF(x, y, 32, 16)));
                        else
                        {
                            DestructionBlock destruct = new DestructionBlock(new RectangleF(x, y, 32, 16));
                            destruct.EffectRadius = 48;
                            returnlevel.levelblocks.Add(destruct);

                        }


                    }
                }
            }
            //returnlevel.levelblocks.Add(new BlackHoleBlock(new RectangleF(PicGame.Width / 2, PicGame.Height/2, 32, 16)));
            return returnlevel;

        }
        public LevelSet CreateDefaultLevelSet(RectangleF Targetrect)
        {
            RectangleF PicGame = Targetrect;
            LevelSet createme = new LevelSet();
            createme.SetName = "Default Levels";
            const int numlevels=5;
            for (int j = 0; j < 2; j++)
            {
                for (int i = 1; i < numlevels+1; i++)
                {
                    //createme.Levels.Add(CreateDefaultLevel(
                    createme.Levels.Add(CreateDefaultLevel(i, Targetrect));


                }
            }
            //create a second "set" of levels, but with 
            for (int i = numlevels+1; i < numlevels*2; i++)
            {
                PointF movingorigin = new PointF(50, PicGame.Height - 90);
                //returnlevel.levelblocks.Add(new BoundedMovingBlock(new BlackHoleBlock(new RectangleF(movingorigin.X + (q * 32), movingorigin.Y + (q * 16), 16, 8)), new PointF(2f, 1f), new RectangleF(0, PicGame.Height / 2, PicGame.Width, PicGame.Height / 2)));

                createme.Levels[i].levelblocks.Add(new BoundedMovingBlock(new BlackHoleBlock(new RectangleF(movingorigin.X, movingorigin.Y, 16, 8)), new PointF(2f, 1f), new RectangleF(0, PicGame.Height / 2, PicGame.Width, PicGame.Height / 2)));
                createme.Levels[i].LevelName = createme.Levels[i].LevelName + "*";


            }

            //createme.Save(@"D:\testoutput.lgf");
            return createme;
        }



        #region iLevelSetBuilder Members

        public override string getName()
        {
            return "BaseBlock Default";
        }

        public override string getDescription()
        {
            return "Default Level Builder for BASeBlock";
        }

        public override LevelSet BuildLevelSet(RectangleF targetrect,IWin32Window Owner)
        {
            return CreateDefaultLevelSet(targetrect);
        }

      

      


       
        

       


     

        #endregion
    }
}
