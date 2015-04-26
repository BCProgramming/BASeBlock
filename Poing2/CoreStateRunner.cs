using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using BASeBlock.Blocks;
using BASeBlock.Particles;

namespace BASeBlock
{
    public class CoreStateRunner
    {
        private frmBaseBlock GameClient;
        private int MaxParticles = 800;
        private DateTime lastscoreshow;
        public static int ScoreRevealDelayTime = 500;
        private static GameRunStateConstants[] LoopingStates = new GameRunStateConstants[] { GameRunStateConstants.Game_Menu, GameRunStateConstants.Game_Paused, GameRunStateConstants.Game_ValueInput };
        public TimeSpan Deathtime = new TimeSpan(0,0,0,2);
        private List<Particle> mremoveparticles = new List<Particle>();

        public CoreStateRunner(frmBaseBlock frmBaseBlock1)
        {
            GameClient = frmBaseBlock1;
        }

        private void GameOver()
        {
            if (GameClient.GameThread != null && GameClient.GameThread!=Thread.CurrentThread)
                GameClient.GameThread.Abort();



            bool gotscore=false;
            BCBlockGameState.Soundman.StopMusic();
            //TODO: here is where we would "submit" the score to the HighScore management system... but ONLY if demo mode is off!
            //create the game over level.
            if (GameClient.DemoMode == false)
            {
                gotscore= GameClient.HiscoreCheck();


            }
            //if they didn't get a hi score, forcegameover().... otherwise, game over will be called after they enter their name.
            if(!gotscore) GameClient.forcegameover();
        }

        private void PostLifeLost()
        {
            Dorefreshstats = true;

            if (GameClient.mPlayingSet != null)
            {
                GameClient.mPlayingSet.Statistics.Deaths++;

            }

            GameClient.mGameState.playerLives--;
            //lblLives.Invoke(((MethodInvoker)(() => lblLives.Text = mGameState.playerLives.ToString())));

            GameClient.gamethreadsuspended = true;
            //Thread.Sleep(500);
            GameClient.gamethreadsuspended = false;
            GameClient.gamerunstate = GameRunStateConstants.Game_Running;
            if (GameClient.mGameState.playerLives <= 0)
            {

                GameOver();



            }
            else
            {
                //reinitialize the balls from the "stored" levelset...
                
                if (GameClient.mGameState.PlayerPaddle != null)
                {
                    lock (GameClient.mGameState.PlayerPaddle) //lock it to prevent race condition.
                    {
                        //added June 24th 2011- Destroy() routine needs to be called when reinstantiating the paddle, otherwise
                        //the "old" paddle, and it's behaviours, will still have active hooks to any events they hooked.
                        foreach (var loopbeh in GameClient.mGameState.PlayerPaddle.Behaviours)
                        {
                            loopbeh.UnHook();


                        }
                        GameClient.mGameState.PlayerPaddle.Behaviours.Clear();
                    }
                }

                GameClient.mGameState.PlayerPaddle = new Paddle(GameClient.mGameState, new Size(48, 15), new PointF(GameClient.mGameState.GameArea.Width / 2, GameClient.mGameState.TargetObject.Height - 35), BCBlockGameState.Imageman.getLoadedImage("paddle"));
                GameClient.mGameState.PlayerPaddle.OnHPChange+= GameClient.PlayerPaddle_OnHPChange;
                GameClient.mGameState.PlayerPaddle.OnEnergyChange +=new Action(GameClient.PlayerPaddle_OnEnergyChange);
                foreach (cBall loopball in GameClient.mPlayingSet.Levels[GameClient.mPlayingLevel - 1].levelballs)
                {
                    GameClient.mGameState.Balls.AddLast((cBall)loopball.Clone());



                }

                //iterate through the levels blocks as well and see which ones we need to respawn...
                foreach (Block iterateblock in GameClient.mPlayingSet.Levels[GameClient.mPlayingLevel - 1].levelblocks)
                {
                    if (iterateblock.AutoRespawn)
                    {
                        GameClient.mGameState.Blocks.AddLast((Block)iterateblock.Clone());

                    }
                }


            }




        }

        private void CountDown(ref int sourcevalue, ref int destvalue, int amount)
        {
            //CountDown: removes amount from sourcevalue and adds it to destvalue.
            if (GameClient.AccelerateCountdown) amount *= 500;

            if (sourcevalue < amount)
            {
                destvalue += sourcevalue;
                sourcevalue = 0;

            }
            else
            {
                destvalue+=amount;
                sourcevalue-=amount;



            }


        }

        public int GetTimeBonus(TimeSpan LevelTime, TimeSpan ParTime)
        {

            int buildresult = ((int)((ParTime - LevelTime).TotalSeconds)) * (5 *GameClient.mPlayingLevel);
            int testvalue = (int)Math.Floor(LevelTime.TotalHours*100);
            if (CPrimes.IsPrime(testvalue))
            {
                buildresult += LevelTime.Seconds*25;


            }


            return buildresult;


        }

        private void Runstate()
        {
            List<cBall> ballsadded= new List<cBall>();
            if (GameClient.gamerunstate == GameRunStateConstants.Game_Death)
            {
                //has the time expired? if so call the post-death routine...
                if (DateTime.Now - GameClient.DeathStart > Deathtime)
                    PostLifeLost();
            }
            while (LoopingStates.Contains(GameClient.gamerunstate))
            {
                while (GameClient.gamerunstate == GameRunStateConstants.Game_Menu)
                {
                    Application.DoEvents();
                    Thread.Sleep(5);
                    GameClient.PicGame.Invoke((MethodInvoker) (() =>
                                                                   {
                                                                       GameClient.PicGame.Invalidate();
                                                                       GameClient.PicGame.Update();
                                                                   }))
                        ;
                }
                while (GameClient.gamerunstate == GameRunStateConstants.Game_Paused)
                {
                    Thread.Sleep(100);
                    Application.DoEvents();
                    GameClient.PicGame.Invoke((MethodInvoker) (() =>
                                                                   {
                                                                       GameClient.PicGame.Invalidate();
                                                                       GameClient.PicGame.Update();
                                                                   }))
                        ;
                }

                #region Cheat Input

                while (GameClient.gamerunstate == GameRunStateConstants.Game_ValueInput)
                {
                    Application.DoEvents();
                    Thread.Sleep(100);
                    if (GameClient.inputdata.GameProcRoutine != null)
                    {
                        GameClient.inputdata.GameProcRoutine();
                    }
                    GameClient.PicGame.Invoke((MethodInvoker) (() =>
                                                                   {
                                                                       GameClient.PicGame.Invalidate();
                                                                       GameClient.PicGame.Update();
                                                                   }))
                        ;
                }

                //debugging code. Seems the Game_Menu setting doesn't play friendly with the other stuff. TSSK TSSK.
                if (GameClient.gamerunstate == GameRunStateConstants.Game_Menu)
                {
                    Debug.Print("Menu");
                }

                #endregion
            }
            //const int levelintroticks=500;
            int elapsedintroticks = 0;
            int elapsedoutroticks = 0;
            DateTime startlevelintroloop = DateTime.Now;
            bool foundanimated = false;

            #region LevelIntro

            while (GameClient.gamerunstate == GameRunStateConstants.Game_LevelIntro)
            {
                //TODO: there is a bug somewhere; AnimatedBlocks are not drawn properly during the intro sequence. (usually, they show up as normal blocks instead for some reason).
                //addendum to the above: now they no longer draw at all during the intro sequence.
                //do nothing but "wait" for a specific period. 
                //elapsedintroticks++;
                //Debug.Print("elapsedintroticks:" + elapsedintroticks.ToString() + "gamerunstate:" + gamerunstate.ToString());

                //tweak: if shownamelength is negative, then when we had played the music using PlayLevelMusic the  routine
                //will have hooked the sound stop event; we use that to determine when to end, since it also sets a flag.
                if (((DateTime.Now - startlevelintroloop) > GameClient.mPlayingSet.Levels[GameClient.mPlayingLevel - 1].ShowNameLength)
                    || (GameClient.mPlayingSet.Levels[GameClient.mPlayingLevel - 1].ShowNameLength.Ticks == 0 && GameClient.playintrodone))


                {
                    elapsedintroticks = 0;
                    //play the Level Music.
                    GameClient.mLevelTime = new TimeSpan(0);
                    GameClient.LastDateTime = DateTime.Now;
                    GameClient.PlayLevelMusic(GameClient.mPlayingSet.Levels[GameClient.mPlayingLevel - 1], false);
                    GameClient.gamerunstate = GameRunStateConstants.Game_Running;
                }


                foreach (Block LoopBlock in GameClient.mGameState.Blocks)
                {
                    if (LoopBlock is AnimatedBlock)
                    {
                        foundanimated = true;
                        if ((LoopBlock).PerformFrame(GameClient.mGameState)) GameClient.mredrawblocks = true;
                    }
                }
                // IntroAnimator.PerformFrame();
                GameClient.PicGame.Invoke((MethodInvoker) (() =>
                                                               {
                                                                   //if(mredrawblocks) 
                                                                   GameClient.PicGame.Invalidate();
                                                                   GameClient.PicGame.Update();
                                                               }));
                GameClient.mredrawblocks = false;
            }

            #endregion

            //int elapsedoutroticks = 0;

            //otherwise we are in the gameover or level outtro...

            #region Leveloutro and outro gameover

            DateTime startleveloutroloop = DateTime.Now;
            int Countdownlevelscore = (int) GameClient.mGameState.GameScore - GameClient.mScoreatlevelstart;
            //the level score counts down, and is added to our initial level score (mScoreAtlevelstart)
            int Addupgamescore = GameClient.mScoreatlevelstart;
            //game score that goes from scoreatlevelstart to CountDownlevelscore
            //int countdownbonusscore=250*(mPlayingLevel+1);


            int countdownbonusscore = GetTimeBonus(GameClient.mLevelTime, GameClient.mLevelParTime);

            //also, bonus for each ball in play. (why the heck not...)
            countdownbonusscore += (GameClient.mGameState.Balls.Count*50);
            //and add some score for macguffins
            //1 + (x/500)

            countdownbonusscore += (int) ((countdownbonusscore*(1 + ((float) GameClient.mGameState.MacGuffins/500))));
            countdownbonusscore = (int) (countdownbonusscore*GameClient.mGameState.ScoreMultiplier);
            //string tallykey = BCBlockGameState.Soundman.getRandomSound("TALLYTICK");
            String tallykey = "";
            lock (GameClient.mPlayingSet)
            {
                if (!(GameClient.mPlayingLevel - 1 > GameClient.mPlayingSet.Levels.Count))
                    tallykey = GameClient.mPlayingSet.Levels[0].TallyTickSound;
                else
                    tallykey = GameClient.mPlayingSet.Levels[GameClient.mPlayingLevel - 1].TallyTickSound;
            }
            int totalbonus = countdownbonusscore;
            TimeSpan aSecond = new TimeSpan(0, 0, 0, 2);
            //bonus score is added after. 
            String writeString = "";
            bool playingrollsound = false;
            bool tallyfinished = false;

            int scoredecrement;
            scoredecrement = Countdownlevelscore/100;
            if (scoredecrement == 0) scoredecrement = 1;

            DateTime finishtallytime = DateTime.Now.AddDays(1);

            bool doGameOver = GameClient.gamerunstate == GameRunStateConstants.Game_LevelOutro_GameOver;
            if (doGameOver)
                countdownbonusscore = (int) (Countdownlevelscore*-0.4);
            while (GameClient.gamerunstate == GameRunStateConstants.Game_LevelOutro || GameClient.gamerunstate == GameRunStateConstants.Game_LevelOutro_GameOver)
            {
                //note: we wait a full second before we start the "counting"...
                if (!playingrollsound)
                {
                    if (doGameOver)
                    {
                        //bugfix: when we get here, turns out the GameOver LevelSet had already been initialized. Or something.

                        BCBlockGameState.Soundman.PlayMusic(GameClient.mPlayingSet.Levels[0].GameOverMusic, 1.0f, false);
                    }
                    else
                        //musicobj = BCBlockGameState.Soundman.PlayMusic("TALLY", 1.0f, false);
                        BCBlockGameState.Soundman.PlayMusic(GameClient.mPlayingSet.Levels[GameClient.mPlayingLevel - 1].TallyMusicName, 1.0f, false);
                }

                playingrollsound = true;
                if ((DateTime.Now - startleveloutroloop) > (aSecond))
                {
                    //ok, good. perform a single "frame" of countdown.
                    //if our levelcountdown is non-zero, take some off of it, and add it to countdowngamescore.

                    if (Countdownlevelscore > 0)
                    {
                        CountDown(ref Countdownlevelscore, ref Addupgamescore, scoredecrement);
                        BCBlockGameState.Soundman.PlaySound(tallykey, 0.9f);
                    }

                    else if (countdownbonusscore != 0)
                    {
                        CountDown(ref countdownbonusscore, ref Addupgamescore, Math.Sign(countdownbonusscore)*50);
                        BCBlockGameState.Soundman.PlaySound(tallykey, 0.9f);
                    }
                    else if (!tallyfinished)
                    {
                        finishtallytime = DateTime.Now;
                        tallyfinished = true;
                    }
                }
                if ((DateTime.Now - finishtallytime) >= (aSecond + aSecond))
                {
                    if (doGameOver)
                    {
                    }
                    else
                    {
                        GameClient.mGameState.GameScore += totalbonus;
                        GameClient.BlockDrawBuffer.Clear(Color.Transparent);
                        int gotnext = GameClient.mPlayingSet.Levels[GameClient.mPlayingLevel - 1].NextLevel;
                        int currlevel;
                        if (gotnext == 0)
                            currlevel = GameClient.mPlayingLevel + 1;
                        else
                            currlevel = gotnext;


                        if (currlevel > GameClient.mPlayingSet.Levels.Count())
                        {
                            //MessageBox.Show("You have defeated all levels...");
                            //a "completion" occurs when any level is finished that tries to go to a level higher then the number of levels.

                            GameClient.mGameState.NumCompletions++;
                            GameClient.sidebarbgdrawn = false;
                            GameClient.gamerunstate = GameRunStateConstants.Game_Running;

                            currlevel = 1;
                        }
                        //currlevel = 1;
                        GameClient.mPlayingLevel = currlevel;
                        long usescore = GameClient.mGameState.GameScore;
                        GameClient.mGameState.invokescoreupdate(ref usescore, ref usescore);
                        GameClient.PlayLevel(GameClient.mGameState, GameClient.mPlayingSet.Levels[GameClient.mPlayingLevel - 1]);
                    }
                }
                if (doGameOver)
                {
                    if (KeyboardInfo.IsPressed(Keys.LButton))
                    {
                        lastscoreshow = DateTime.Now - new TimeSpan(0, 0, 0, 0, ScoreRevealDelayTime + 1);
                    }
                    //the game over screen will also show the high scores of that set.
                    //it does this on it's own, but we need to initialize the string...


                    //if the time has elapsed..
                    if ((DateTime.Now - lastscoreshow).TotalMilliseconds > ScoreRevealDelayTime)
                    {
                        //add the next score entry to the string...

                        if (GameClient.Nextshowhiscore <= 9)
                        {
                            var nextentry = GameClient.ActualPlaySet.HighScores.Scores.ElementAt(GameClient.ActualPlaySet.HighScores.Scores.Count - GameClient.Nextshowhiscore - 1).Value;
                            int useseparators = GameClient.HighScoresMaxSeparators - nextentry.Name.Length;
                            String usespaces = (GameClient.Nextshowhiscore + 1 == GameClient.HighScorePos) ? "**" : "  ";
                            String usename = nextentry.Name;
                            if (usename.Length > 22) usename = usename.Substring(0, 20) + ">>";
                            String nextshow = String.Format("{0:00}", GameClient.Nextshowhiscore + 1) + ":" + usespaces + usename +
                                              Repeat(".", useseparators + 4) + nextentry.Score;
                            GameClient.GameOverHiScores += "\n" + nextshow;
                            BCBlockGameState.Soundman.PlaySound("revel");

                            //GameOverHiScores 
                            lastscoreshow = DateTime.Now;
                            GameClient.Nextshowhiscore++;
                        }
                        else if (GameClient.Nextshowhiscore == 10)
                        {
                            //show a blank...
                            GameClient.GameOverHiScores += "\n" + Repeat(".", GameClient.HighScoresMaxSeparators + 1);
                            BCBlockGameState.Soundman.PlaySound("revel");
                            lastscoreshow = DateTime.Now;
                            GameClient.Nextshowhiscore++;
                        }
                        else if (GameClient.Nextshowhiscore == 11)
                        {
                            GameClient.GameOverHiScores += "\nPress Button B";
                            BCBlockGameState.Soundman.PlaySound("revel");
                            lastscoreshow = DateTime.Now;
                            GameClient.Nextshowhiscore++;
                        }
                        else if (GameClient.Nextshowhiscore == 12)
                        {
                            GameClient.GameOverHiScores += "\nTo Play Again.";
                            BCBlockGameState.Soundman.PlaySound("revel");
                            GameClient.CancelGameThread = true;
                            GameClient.RightClickRestart = true;
                            //Special code:
                            //we want to STOP the gameproc, since it's job is essential done at this point.


                            GameClient.BeginInvoke((MethodInvoker) (() =>
                                                                        {
                                                                            GameClient.PicGame.Invalidate();
                                                                            GameClient.PicGame.Update();
                                                                            GameClient.GameThread.Abort();
                                                                        }));
                            GameClient.Nextshowhiscore++;
                        }
                    }

                    //Yield.
                    Thread.Sleep(10);
                    writeString = "   GAME OVER   \n";
                    writeString += "LEVEL:      " + GameClient.mPlayingLevel.ToString() + "\n" +
                                   "SCORE:      " + GameClient.mGameState.GameScore.ToString();
                }
                else
                {
                    writeString = GameClient.mGameState.PlayingLevel.ClearTitle;


                    writeString += "LEVEL:      " + Countdownlevelscore.ToString() + "\n" +
                                   "TIME:       " + FormatLevelTime(GameClient.mLevelTime) + "\n" +
                                   "PAR TIME:   " + FormatLevelTime(GameClient.mLevelParTime) + "\n" +
                                   "BONUS:      " + countdownbonusscore + "\n" +
                                   "TOTAL:      " + Addupgamescore + "\n";
                }
                GameClient.tallydata.TallyScreenString = writeString;
                //Debug.Print("elapsedintroticks:" + elapsedintroticks.ToString() + "gamerunstate:" + gamerunstate.ToString());

                AnimateParticles();
                RefreshFrames();
            }

            #endregion

            //Thread.Sleep(framedelay);

            //while ((DateTime.Now - LastDateTime).Milliseconds < 3)
            //{
            //}
            Thread.Sleep(0);
            bool blocksaffected = false;
            List<Block> blocksfromperformframe;
            //go through all balls...


            if (GameClient.mGameState.Balls != null)
            {
                //purge balls beyond our limit.
                if (GameClient.mPlayingLevelobj.MaxBalls > 0 && GameClient.mGameState.Balls.Count() > GameClient.mPlayingLevelobj.MaxBalls)
                {
                    while (GameClient.mGameState.Balls.Count() > GameClient.mPlayingLevelobj.MaxBalls)
                        GameClient.mGameState.Balls.RemoveLast();
                }


                cBall continueball = null;
                cBall lastball = null;
                ballsadded.Clear();
                restartloop:

                try
                {
                    //initially start with the assumption that no blocks will need to be redrawn.
                    if (GameClient.mGameState.Balls.Count == 0)
                    {
                        GameClient.LifeLost();
                    }
                    GameClient.mredrawblocks = false;
                    //foreach (cBall loopball in mGameState.Balls)
                    // for(LinkedListNode<cBall> loopballnode = mGameState.Balls.First;
                    //    loopballnode!=null;loopballnode=loopballnode.Next)
                    // for(LinkedListNode<cBall> loopballnode = mGameState.Balls.Last;
                    //    loopballnode!=null;loopballnode=loopballnode.Previous)
                    cBall lowestBall = null;
                    //naturally, we iterate through every ball.

                    //testing: now clones the gamestate balls list...
                    if (GameClient.mGameState.ShootBalls.Count > 0)
                    {
                        lock (GameClient.mGameState.Balls)
                        {
                            GameClient.mGameState.Balls.AddRangeAfter(GameClient.mGameState.ShootBalls);
                        }
                        GameClient.mGameState.ShootBalls.Clear();
                    }
                    LinkedList<cBall> cloneballs = GameClient.mGameState.Balls;
                    lock (GameClient.mGameState.Balls)
                    {
                        cloneballs = new LinkedList<cBall>(GameClient.mGameState.Balls);


                        //foreach (cBall loopball in mGameState.Balls)
                        //var options = new System.Threading.Tasks.ParallelOptions() {MaxDegreeOfParallelism=8};
                        //System.Threading.Tasks.Parallel.ForEach(cloneballs,options,(loopball =>


                        foreach (cBall loopball in cloneballs)
                            //LinkedListNode<cBall> loopballnode= mGameState.Balls.First;
                        {
                            //cBall loopball = loopballnode.Value;
                            if (loopball != null)
                            {
                                //cBall loopball = mGameState.Balls[i];

                                if (lowestBall == null || loopball.Location.Y > lowestBall.Location.Y)
                                {
                                    if ((Math.Sign(loopball.Velocity.Y)) > 0)
                                        lowestBall = loopball;
                                    //used by demo mode (and possibly later by other stuff) stores the lowest ball that is coming towards the paddle. (well, actually, it's the lowest ball that 
                                    //is going downwards.
                                }
                                if (continueball == null || continueball == loopball)
                                {
                                    continueball = null;
                                    //make sure the ball is set to call us back if an impact occurs.
                                    if (!loopball.hasBallImpact())
                                        loopball.BallImpact += loopball_BallImpact;


                                    List<cBall> ballsremove = new List<cBall>();
                                    //Hey ball, can you move a single frame and give us back some info on the blocks
                                    //that you affected? thanks.
                                    blocksfromperformframe = loopball.PerformFrame(GameClient.mGameState, ref ballsadded,
                                                                                   ref ballsremove);
                                    //Blocksfromperformframe is now a collection of Blocks that were affected by this frame.

                                    foreach (cBall loopremball in ballsremove)
                                    {
                                        // mGameState.Balls.Remove(loopremball);
                                        //kill each ball in the removing collection.
                                        GameClient.balldeath(loopremball);
                                    }
                                    //mredrawblocks = mredrawblocks || (blocksfromperformframe.Count() > 0);

                                    GameClient.mredrawblocks = GameClient.mredrawblocks ||
                                                               (blocksfromperformframe.Any((w) => !(w.RequiresPerformFrame())));
                                    GameClient.mRedrawAnimated = GameClient.mredrawblocks ||
                                                                 (blocksfromperformframe.Any((w) => (w.RequiresPerformFrame())));
                                    //mredrawblocks=true;  
                                }
                                lastball = loopball;
                            }
                        }
                    }
                    Func<Block, bool> checkfrozen = ((a) =>
                                                         {
                                                             if (a is AnimatedBlock)
                                                                 if (((AnimatedBlock) a).Frozen)
                                                                     return true; //it's frozen.


                                                             return false;
                                                         });
                    //now, use LINQ to grab all Animated blocks Blocks that require animation.

                    #region block animation 

                    var result = from x in GameClient.mGameState.Blocks where x.RequiresPerformFrame() select x;
                    GameClient.AnimatedBlocks = result.ToList();

                    GameClient.staticblocks = (from x in GameClient.mGameState.Blocks where !x.RequiresPerformFrame() select x).ToList();

                    foreach (Block loopblock in result)
                    {
                        /*if (loopblock is DemonBlock)
                                {
                                    Debug.Print("break");


                                }*/
                        //FIX: changed order so that performframe is called regardless.
                        bool hc = false;
                        hc = loopblock.PerformFrame(GameClient.mGameState) || loopblock.hasChanged;

                        if (hc) GameClient.mRedrawAnimated = true;
                    }

                    #endregion

                    //combo check.
                    if (GameClient.mGameState.ComboCount > 1 && GameClient.mGameState.ComboFinishTime != null)
                    {
                        Font fontuse = new Font("Arial", 28);
                        //mGameState.ComboCountDown--;
                        //Debug.Print("ComboCountdown:" + mGameState.ComboCountDown);


                        int clamped = BCBlockGameState.ClampValue(GameClient.mGameState.ComboCount - 1, 0,
                                                                  BCBlockGameState.ComboNames.Length - 1);
                        if (GameClient.mGameState.ComboFinishTime.Value < DateTime.Now)
                        {
                            GameClient.mGameState.ComboFinishTime = null;
                            string endquote = " chained)";
                            if ((GameClient.mGameState.ComboCount - 1) == 1) endquote = " chain)";
                            string combotext = BCBlockGameState.ComboNames[clamped] + " \nCombo!" + "(" +
                                               (GameClient.mGameState.ComboCount - 1) + endquote;
                            SizeF calcsize = BCBlockGameState.MeasureString(combotext, fontuse);

                            PointF initialpos = new PointF(GameClient.mGameState.GameArea.Width/2, GameClient.mGameState.GameArea.Height);


                            Brush drawbrush = new SolidBrush(BCBlockGameState.ComboFillColour[clamped]);
                            Pen drawpen = new Pen(BCBlockGameState.ComboPenColour[clamped]);


                            Block.PopupText(GameClient.mGameState, combotext, fontuse, drawbrush, drawpen, 500, new PointF(0, -1.05f),
                                            initialpos);
                            BCBlockGameState.Soundman.PlaySound(BCBlockGameState.ComboNames[clamped], 2.0f);
                            GameClient.mGameState.ComboCount = 0;
                        }
                    }
                    Block.HandleEffects(GameClient.mGameState);

                    #region particle animation

                    AnimateParticles();

                    #endregion

                    //Changeup: now inspect to see if we need to add a message.
                    //the latest message will be 
                    //mPlayingLevelobj.MessageQueue.Peek()

                    //grab topmost...
                    if (GameClient.LevelMessageQueue != null && GameClient.LevelMessageQueue.Count > 0)
                    {
                        var topmostmessage = GameClient.LevelMessageQueue.Peek();


                        if (topmostmessage.TotalMS < (GameClient.mLevelTime).TotalMilliseconds)
                        {
                            GameClient.LevelMessageQueue.Dequeue();
                            topmostmessage.Invoke(GameClient.mGameState);
                            //enqueue a new item, add the length of the playing music to the timeindex...
                            float accumlength = BCBlockGameState.Soundman.GetPlayingMusic().getLength();
                            accumlength += topmostmessage.TimeIndex;
                            //create a new Message...
                            var clonedmess = (PlayMessageData) topmostmessage.Clone();
                            clonedmess.TimeIndex = accumlength;
                            GameClient.LevelMessageQueue.Enqueue(clonedmess);
                        }
                    }


                    AnimateGameObjects();

                    if (GameClient.DemoMode)
                    {
                        //reposition the paddle to be in the path of the lowest ball. we cached that earlier.
                        if (lowestBall != null)
                        {
                            if (GameClient.mGameState.PlayerPaddle != null)
                                GameClient.mGameState.PlayerPaddle.Position = new PointF(lowestBall.Location.X, GameClient.mGameState.PlayerPaddle.Position.Y);
                        }
                    }
                    int gotcount = GameClient.mGameState.Blocks.Count((w) => (w.MustDestroy() == true));
                    if (gotcount == 0)
                    {
                        //LifeLost();
                        GameClient.mGameState.invokeLevelComplete();
                    }
                }
                catch (Exception e)
                {
                    Trace.WriteLine("Exception in loop :(" + e.Message + " Stack:{" + e.StackTrace + "}");

                    continueball = lastball;
                    //goto restartloop;
                }

                foreach (cBall addthis in ballsadded)
                {
                    GameClient.mGameState.Balls.AddLast(addthis);
                }
                foreach (cBall removethis in GameClient.mGameState.removeballs)
                {
                    GameClient.mGameState.Balls.Remove(removethis);
                }

                GameClient.mGameState.removeballs.Clear();

                try
                {
                    // mredrawblocks = mredrawblocks || mGameState.Blocks.Any((w) => (w.hasChanged&&((w is AnimatedBlock))));

                    GameClient.PicGame.Invalidate();
                    GameClient.PicGame.Invoke((MethodInvoker) (() =>
                                                                   {
                                                                       GameClient.PicGame.Invalidate();
                                                                       GameClient.PicGame.Update();
                                                                   }));
                    //Application.DoEvents();
                }
                catch
                {
                }
            }
        }

        private void RefreshFrames()
        {
            GameClient.PicGame.Invoke((MethodInvoker)(() =>
                                                          {
                                                              GameClient.PicGame.Invalidate();
                                                              GameClient.PicGame.Update();
                                                          }));
        }

        private String FormatLevelTime(TimeSpan LevelTime)
        {
            if(LevelTime.Hours >0)
                return new DateTime(LevelTime.Ticks).ToString("hh:mm:ss.ff"); 
            else
                return new DateTime(LevelTime.Ticks).ToString("mm:ss.ff"); 








        }

        private void AnimateGameObjects()
        {
            List<GameObject> lAddObjects = new List<GameObject>();
            List<GameObject> lremoveObjects = new List<GameObject>();
            //bugfix: seems that issues will be abound if collections are modified during the loop (duh).
            //but mostly because execution leaves this procedure entirely, leaving the gameobjects to add and gameobjects to remove collections untransferred.
            GameObject lastinstance = null;
            try

            {
                
                lock (GameClient.mGameState.GameObjects)
                {
                    foreach (GameObject loopgobject in GameClient.mGameState.GameObjects)
                    {
                        if (loopgobject.Frozen) continue; //frozen objects can't move...
                        lastinstance = loopgobject;
                        if (loopgobject is SpinShot)
                        {
                            Debug.Print("test");
                        }
                        if (loopgobject.PerformFrame(GameClient.mGameState, ref lAddObjects, ref lremoveObjects))
                        {
                            
                            lremoveObjects.Add(loopgobject);
                        }

                    }
                    if (GameClient.mGameState.Forcerefresh)
                    {
                        GameClient.mredrawblocks = true;
                        GameClient.mGameState.Forcerefresh = false;


                    }
                }
            }

            catch (InvalidOperationException eex)
            {
                //unused catch handler. 
                Debug.Print("InvalidOperationException Caught in AnimateGameObjects-" + eex.ToString());
            }
            

            //add the added objects...
            lock (GameClient.mGameState.GameObjects)
            {

                foreach (GameObject addthisobject in lAddObjects)
                    GameClient.mGameState.GameObjects.AddLast(addthisobject);

                foreach (GameObject removethisobject in lremoveObjects)
                {
                    if (removethisobject is GameEnemy)
                    {
                        GameEnemy caste = removethisobject as GameEnemy;
                        List<GameObject> addtemp= new List<GameObject>();
                        List<GameObject> removetemp = new List<GameObject>();
                        caste.InvokeOnDeath(GameClient.mGameState, ref addtemp, ref removetemp);

                    }
                    GameClient.mGameState.GameObjects.Remove(removethisobject);
                }
            }
        }

        private void AnimateParticles()
        {
            
            mremoveparticles.Clear();
            lock (GameClient.mGameState.Particles)
            {
                /*
                if (mGameState.Particles.Count > particlelimit)
                {
                    //if above the particle limit, remove the "oldest" ones. This is more a guess then anything, we just remove the ones at the start until we are below the limit.
                    int numremove = particlelimit - mGameState.Particles.Count;
                    if (numremove > 0) mGameState.Particles.RemoveRange(0, numremove); //tada...

                }
                */
                int totalparticles = GameClient.mGameState.Particles.Count((w)=>!w.Important);
                foreach (Particle loopparticle in GameClient.mGameState.Particles)
                {
                    if(loopparticle!=null)
                        if (loopparticle.PerformFrame(GameClient.mGameState) || totalparticles > MaxParticles)
                        {
                            mremoveparticles.Add(loopparticle);
                            totalparticles--;
                        }


                }

                foreach (Particle removeparticle in mremoveparticles)
                    GameClient.mGameState.Particles.Remove(removeparticle);
            }
        }

        public String Repeat(String repeatthis, int count)
        {

            StringBuilder buildresult = new StringBuilder(repeatthis.Length * count);
            for (int i = 0; i < count; i++)
            {
                buildresult.Append(repeatthis);

            }
            return buildresult.ToString();
        }

        private void loopball_BallImpact(cBall ballimpact)
        {

            if ((ballimpact.numImpacts> 0) && ballimpact.isTempBall)
            {


                if (GameClient.mGameState.Balls.Count() == 1 && GameClient.mGameState.Balls.Contains(ballimpact))
                {
                    //we are the last ball, promote to a true ball.
                    ballimpact.isTempBall= false;



                }
                try
                {
                    // mGameState.Balls.Remove(ballimpact);

                    GameClient.mGameState.removeballs.Add(ballimpact);
                }
                catch
                {

                }


            }

            //throw new NotImplementedException();
            
        }
    }
}