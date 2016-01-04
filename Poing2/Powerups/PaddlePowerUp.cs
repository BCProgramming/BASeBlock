using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Reflection;
using BASeCamp.BASeBlock.PaddleBehaviours;

namespace BASeCamp.BASeBlock.Powerups
{
    /// <summary>
    /// Generic class used to create a powerup that gives the paddle a specific bnehaviour.
    /// </summary>
    /// <typeparam name="PaddlePower">Type of the iPaddleBehaviour that should be added.</typeparam>
    public abstract class PaddlePowerUp<PaddlePower> : GamePowerUp where PaddlePower : PaddleBehaviours.iPaddleBehaviour
    {
        protected int mScoreValue = 50;
        protected string getDescription()
        {
            //check for static field on PaddlePower.
            Type checktype = typeof(PaddlePower);
            
            //var grabprop = checktype.GetProperty("Name", BindingFlags.Static);
            PropertyInfo gotname=null;
            foreach(var iterate in checktype.GetProperties(BindingFlags.Static))
            {
                if(iterate.Name.Equals("Name",StringComparison.OrdinalIgnoreCase))
                {
                    gotname=iterate;
                    break;
                }

            }

            if (gotname != null)
            {
                return (String)gotname.GetValue(null, null);
            }
            else
            {

                foreach (var loopit in checktype.GetCustomAttributes(typeof(PowerupDescriptionAttribute),true))
                {
                    PowerupDescriptionAttribute casted = loopit as PowerupDescriptionAttribute;
                    if(casted!=null) return casted.Name;
                }

                return Name;

            }


            Debug.Assert(false, "getDescription boolean");
            return null;
        }
        public bool GivePaddlePower(BCBlockGameState gamestate)
        {
            String prefixit = getDescription();
            //first, destroy/remove existing instances of the mutually exclusive types.
        
            List<iPaddleBehaviour> removethese =  new List<iPaddleBehaviour>(gamestate.PlayerPaddle.GetExclusiveBehaviours(typeof(PaddlePower)));
            foreach (var removeit in removethese)
                gamestate.PlayerPaddle.Behaviours.Remove(removeit);


            if (gamestate.PlayerPaddle.Behaviours.All(w => w.GetType() != typeof(PaddlePower)))
            {
                
                var addit = (iPaddleBehaviour)Activator.CreateInstance(typeof(PaddlePower), (Object)gamestate);
                gamestate.PlayerPaddle.Behaviours.Add(addit);
                AddScore(gamestate, mScoreValue, prefixit);
            }
            else if (SupportsMulti())
            {
                //if we support having multiple calls, then perform the custom method.
                ApplyPower(gamestate);
            }

            //gamestate.PlayerPaddle.Behaviours.Add(new PaddleBehaviours.TerminatorBehaviour(gamestate));



            return true;



        }
        public virtual String Name
        {
            get
            {
                return "Power up!" + typeof(PaddlePower).Name;
            }
        }
        protected virtual void ApplyPower(BCBlockGameState gamestate)
        {
            //
            AddScore(gamestate, (mScoreValue + (4 * gamestate.GameObjects.Count)), Name);
        }

        protected virtual bool SupportsMulti()
        {
            return false;

        }

        protected PaddlePowerUp(PointF Location, SizeF ObjectSize)
            : base(Location, ObjectSize, null, null)
        {
            //calls virtual constructor, but this is on purpose...
            this.ObjectImages = GetPowerUpImages();
            usefunction = GivePaddlePower;

        }
        public abstract Image[] GetPowerUpImages();


    }
}