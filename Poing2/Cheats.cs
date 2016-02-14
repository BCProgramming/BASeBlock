using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using BASeCamp.BASeBlock.Blocks;
using BASeCamp.BASeBlock.PaddleBehaviours;
using BASeCamp.BASeBlock.Powerups;

namespace BASeCamp.BASeBlock.Cheats
{
    

    /// <summary>
    /// Base Cheat class.
    /// Cheats are entered into the "dialog" that opens in-game when pressing Control-Alt-C.
    /// </summary>
    public abstract class Cheat
    {
        /// <summary>
        /// Name of this Cheat code. This should describe what the cheat does.
        /// </summary>
        public abstract String Name { get; }
        private String _cachedCode;
        /// <summary>
        /// The actual text of the cheat code itself that should be entered in the cheat-entry screen.
        /// </summary>
        public virtual String CodeName { get{

            if (_cachedCode != null) return _cachedCode;
            //check for a CheatCode field.
            FieldInfo fi = this.GetType().GetField("CheatCode",BindingFlags.IgnoreCase);
            if (fi != null && fi.FieldType == typeof(String))
            {
                
                    return (_cachedCode=fi.GetValue(this) as String);
                
            }
            //still here, so no go with the Field.
            //we'll get the actual class type of this type.
            String useclassname = this.GetType().Name;
            //if it ends in cheat, remove that text....
            if (useclassname.EndsWith("Cheat"))
                useclassname = useclassname.Substring(0, useclassname.Length - "Cheat".Length);


            return (_cachedCode=useclassname);

        
        }
        }
        /// <summary>
        /// Applies this cheat.
        /// </summary>
        /// <param name="gState"></param>
        /// <param name="Parameters"></param>
        public abstract bool ApplyCheat(BCBlockGameState gState, int ParameterCount,String[] Parameters);

        private static Dictionary<String, Cheat> cachedList;
        public static Cheat GetCheat(String codename)
        {
            if (GetCheats().ContainsKey(codename))
                return GetCheats()[codename];
            else
                return null;
                
            
            


        }
       
        private static Dictionary<String, Cheat> GetCheats()
        {
            if (cachedList == null)
            {
                //construct a new dictionary, case insensitive.
                cachedList = new Dictionary<string, Cheat>(StringComparer.OrdinalIgnoreCase);
                foreach (var iterate in BCBlockGameState.MTypeManager[typeof(Cheat)].ManagedTypes)
                {
                    Cheat constructCheat = null;
                    try
                    {
                        constructCheat = (Cheat)Activator.CreateInstance(iterate);

                    }
                    catch (Exception exx)
                    {
                        Debug.WriteLine("Exception while instantiating Cheat \"" + iterate.Name + "\" - " + exx.ToString());

                    }
                    if (constructCheat != null)
                    {
                        String usekey = constructCheat.CodeName;
                        cachedList.Add(usekey, constructCheat);

                    }
                }
            }
            return cachedList;

        }



    }



    public class TheNullCheatCheat : Cheat
    {
        public override string Name
        {
            get { return "The Null Cheat"; }
        }

        public override bool ApplyCheat(BCBlockGameState gState,int ParameterCount, string[] Parameters)
        {
            return true;
        }
    }

    public class ThatsaPaddlinCheat : Cheat
    {
        public override string Name
        {
            get { return "That's a Paddlin' Cheat"; }
        }
        public override bool ApplyCheat(BCBlockGameState gState,int ParameterCount, string[] Parameters)
        {
            gState.PlayerPaddle.Behaviours.Add(new SizeChangeBehaviour(new SizeF(gState.PlayerPaddle.PaddleSize.Width * 1.5f, gState.PlayerPaddle.PaddleSize.Height), new TimeSpan(0, 0, 0, 0, 500)));
            return true;
            
        }
    }
    public class PlayMusicCheat : Cheat
    {
        public override string Name
        {
            get { return "Play Music Cheat"; }
        }

        public override bool ApplyCheat(BCBlockGameState gState, int ParameterCount,string[] Parameters)
        {
            if (ParameterCount == 0) return false;
            try
            {
                BCBlockGameState.Soundman.PlayMusic(Parameters[0], 1.0f, true);
            }
            catch
            {
                return false;

            }
            return true;
        }
    }
    public class GetMeWetCheat : Cheat
    {
        public override string Name
        {
            get { return "GetMeWet Cheat"; }
            
        }
        public override bool ApplyCheat(BCBlockGameState gState, int ParameterCount, string[] Parameters)
        {
            //replace all blocks in the arena with a water block.

            gState.ReplaceBlocks((w) => new WaterBlock(w.BlockRectangle));
            return true;
        }

    }
    public class BombsAwayCheat : Cheat
    {

        public override string Name
        {
            get { return "BombsAway Cheat"; }
        }
        public override bool ApplyCheat(BCBlockGameState gState, int ParameterCount, string[] Parameters)
        {
            gState.ReplaceBlocks((w) => new BombBlock(w.BlockRectangle));
            return true;
        }


    }
    public class SuperKrozCheat : Cheat
    {
        public override string Name
        {
            get {
                return "Super Kroz Cheat";
            }
        }


        public override bool ApplyCheat(BCBlockGameState gState, int ParameterCount, string[] Parameters)
        {
            if (gState.PlayerPaddle != null)
                gState.PlayerPaddle.Behaviours.Add(new InvinciblePaddleBehaviour());
            return true;
        }
    }
    public class imboredCheat : Cheat
    {
        public override string Name
        {
            get { return "I'm Bored Cheat"; }
        }
        public override bool ApplyCheat(BCBlockGameState gState, int ParameterCount, string[] Parameters)
        {
            gState.invokeLevelComplete();
            return true;
        }
    }
    public class ifeelthepowerCheat : Cheat
    {

        public override string Name
        {
            get { return "I Feel the Power Cheat"; }
        }
        public override bool ApplyCheat(BCBlockGameState gState, int ParameterCount, string[] Parameters)
        {
            foreach (cBall loopball in gState.Balls)
            {
                if (!loopball.hasBehaviour(typeof(PowerBallBehaviour)))
                    loopball.Behaviours.Add(new PowerBallBehaviour());


            }
            return true;
        }

    }
    public class PowerHasCorruptedMeCheat : Cheat
    {
        public override string Name
        {
            get { return "Power Ball Cheat"; }

        }
        public override bool ApplyCheat(BCBlockGameState gState, int ParameterCount, string[] Parameters)
        {
            //powerhascorruptedme
            foreach (cBall loopball in gState.Balls)
            {
                if (loopball.hasBehaviour(typeof(PowerBallBehaviour)))
                {
                    loopball.Behaviours.RemoveAll((w) => w.GetType() == typeof(PowerBallBehaviour));

                }

            }
            return true;
        }



    }
    public class ObstacleCheat : Cheat
    {
        public override string Name
        {
            get { return "Obstacle Cheat"; }
        }
        public override bool ApplyCheat(BCBlockGameState gState, int ParameterCount, string[] Parameters)
        {
            var Makeitem = new PolygonObstacle(gState.GameArea.CenterPoint(), 3, 8, 23, 50, new FilledPolyDrawMethod(Pens.Black,Brushes.Yellow));
            Makeitem.Velocity = new PointF(3,3);
            gState.Defer(()=>
                gState.GameObjects.AddLast(Makeitem));
            return true;
        }
    }


    public class ReplaceBlocksCheat : Cheat
    {
        public override string Name
        {
            get { return "Replace Blocks Cheat"; }
        }
        public override bool ApplyCheat(BCBlockGameState gState, int ParameterCount, string[] Parameters)
        {
            String[] splitit =Parameters;
            try
            {
                String findtype = splitit[0];
                string replacetype = splitit[1];
                Type replaceT = BCBlockGameState.FindClass(replacetype);

                if (findtype.Length > 0 && replacetype.Length > 0 && replaceT != null)
                {
                    
                    gState.ReplaceBlocks((w) =>
                    {

                        if (w.GetType().Name.Equals(findtype,
                                                    StringComparison.OrdinalIgnoreCase))
                            return
                                (Block)
                                Activator.CreateInstance(replaceT,
                                                         new Object[] { w.BlockRectangle });
                        else
                            return w;


                    });

                }
                else
                {

                    return false;


                }
            }
            catch (Exception q)
            {
                Debug.Print("Unexpected exception:" + q.Message);
            }
            return true;
        }


    }
    public class LightningCheat : Cheat
    {
        public override string Name
        {
            get { return "Lightning"; }
        }
        public override bool ApplyCheat(BCBlockGameState gState, int ParameterCount, string[] Parameters)
        {
            if (gState.PlayerPaddle != null)
            {
                if (!gState.PlayerPaddle.Behaviours.Any((w) => w.GetType() == typeof(TerminatorLightningBehaviour)))
                {
                    TerminatorLightningBehaviour tlb = new TerminatorLightningBehaviour(gState);
                    gState.PlayerPaddle.Behaviours.Add(tlb);
                }
                return true;
            }
            return false;
        }
    }
    public class TerminatedCheat : Cheat
    {
        public override string Name
        {
            get {
                return "Terminated Cheat";    
            }
        }
        public override bool ApplyCheat(BCBlockGameState gState, int ParameterCount, string[] Parameters)
        {
            if (gState.PlayerPaddle != null)
            {
                Type spawntype = typeof(TerminatorBehaviour);
                if (ParameterCount > 0)
                {
                    //parameter can be a Terminator.
                    spawntype = BCBlockGameState.FindClass(Parameters.First());


                }
                GamePowerUp makepower = new TerminatorBallShotPowerup(gState.PlayerPaddle.BlockRectangle.TopCenter(),
                    new SizeF(16, 16));
                /*
                var pp = typeof(TerminatorPaddlePowerup<>);
                Type makegen = pp.MakeGenericType(spawntype);
                GamePowerUp test = new TerminatorPaddlePowerup<TerminatorBehaviour>(gState.PlayerPaddle.BlockRectangle.TopCenter(),
                    new SizeF(16, 16));
                GamePowerUp makepower = (GamePowerUp)(Activator.CreateInstance(makegen, gState.PlayerPaddle.BlockRectangle.TopCenter(),
                    new SizeF(16, 16)));
                */
                gState.Defer(() => gState.GameObjects.AddLast(makepower));

                


            }
            return true;
        }

    }
    public class FireworksCheat : Cheat
    {
        public override string Name
        {
            get { return "Fireworks Cheat"; }
        }

        public override bool ApplyCheat(BCBlockGameState gState, int ParameterCount, string[] Parameters)
        {
            if (gState.PlayerPaddle.Behaviours.All(e => e.GetType() != typeof(TerminatorFireworkBehaviour)))
            {

                gState.PlayerPaddle.Behaviours.Add(new TerminatorFireworkBehaviour(gState));

            }
            return true;
        }

    }
    public class PermaballCheat : Cheat
    {
        public override string Name
        {
            get { return "Perma-ball cheat"; }
        }
        public override bool ApplyCheat(BCBlockGameState gState, int ParameterCount, string[] Parameters)
        {
            if (!gState.PlayerPaddle.Behaviours.Any((t) => t is TerminatorPermanentBallShootBehaviour))
            {
                TerminatorPermanentBallShootBehaviour tpb = new TerminatorPermanentBallShootBehaviour(gState);

                gState.PlayerPaddle.Behaviours.Add(tpb);
            }
            else
            {
                gState.PlayerPaddle.Behaviours.RemoveAll((t) => t is TerminatorPermanentBallShootBehaviour);
            }
            return true;
        }



    }
    public class NibblesBasCheat : Cheat
    {
        public override string CodeName
        {
            get
            {
                return "Nibbles.bas";
            }
        }
        public override string Name
        {
            get { return "Nibbles.bas Cheat"; }
        }
        public override bool ApplyCheat(BCBlockGameState gState, int ParameterCount, string[] Parameters)
        {
            String paramgot = ParameterCount > 0 ? Parameters.First() : "";
            String snaketype = paramgot.Trim();
            var AvailableClientArea = gState.GameArea;
            Type snakeblocktype;
            if (!String.IsNullOrEmpty(snaketype))
            {
                snakeblocktype = BCBlockGameState.FindClass(snaketype);

                if (snakeblocktype == null)
                {
                    return false;

                }


            }
            else
            {
                snakeblocktype = typeof(NormalBlock);

            }
            gState.Defer(() =>
            gState.GameObjects.AddLast(new SnakeEnemy(new PointF(AvailableClientArea.Width / 2, AvailableClientArea.Height / 2), 100, snakeblocktype)));
            return true;
        }


    }
    public class AStickySituationCheat : Cheat
    {

        public override string Name
        {
            get { return "A Sticky Situation"; }
        }
        public override bool ApplyCheat(BCBlockGameState gState, int ParameterCount, string[] Parameters)
        {
            if (gState.PlayerPaddle != null)
            {

                gState.PlayerPaddle.Behaviours.Add(new PaddleBehaviours.StickyBehaviour(gState));


            }
            return true;
        }


    }
    public class GiveBehaviourCheat : Cheat
    {
        public override string Name
        {
            get { return "Give Behaviour"; }
        }

        public override bool ApplyCheat(BCBlockGameState gState, int ParameterCount, string[] Parameters)
        {
            if (Parameters.Length == 0) return false;
            String GetBehaviour = Parameters.First();
            Type addBehaviour = BCBlockGameState.FindClass(GetBehaviour.Trim());
            
            iPaddleBehaviour spawned = Activator.CreateInstance(addBehaviour, new object[] { gState }) as iPaddleBehaviour;
            if (spawned == null) return false;
            //add to the paddle.
            gState.PlayerPaddle.Behaviours.Add(spawned);
            return true;
        }
    }
    public class SpawnPowerUpCheat : Cheat
    {
        public override string Name
        {
            get { return "Spawn PowerUp"; }
        }

        public override bool ApplyCheat(BCBlockGameState gState, int ParameterCount, string[] Parameters)
        {
            if (Parameters.Length == 0) return false;
            //spawnpowerup: paramgot will be the name of the powerup to instantiate.
            //plop it in the middle, also.
            String paramgot = Parameters.First();
            Type spawntype = BCBlockGameState.FindClass(paramgot.Trim());
            if (spawntype == null) return false;
            //use spawntype to create a powerup. Location (PointF) and Size (SizeF)
            PointF centpoint = gState.GameArea.CenterPoint();

            GamePowerUp createpowerup = Activator.CreateInstance(spawntype, new Object[] { centpoint, new SizeF(16, 8) }) as GamePowerUp;
            //check for null... again...
            if (createpowerup == null) return false;
            //otherwise, Add it
            gState.GameObjects.AddLast(createpowerup);
            return true;
        }

    }
}
