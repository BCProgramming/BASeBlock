using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace BASeCamp.BASeBlock
{


  

    //Dev log:
    //trying to decide whether to use a single attribute and have it's constructor take a string to denote the category, or whether
    //to use separate attribute classes. leaning for the latter.


    public class BlockDescriptionAttribute :Attribute
    {
        private readonly String _Description;
        public String Description { get { return _Description; } }
        public BlockDescriptionAttribute(String pDescription)
        {
            _Description = pDescription;

        }

    }


    /// <summary>
    /// Category attribute that can be used to categorize Blocks. This is the base class. All BlockCategories must derive from it.
    /// </summary>
    [System.AttributeUsage(System.AttributeTargets.Class)]
    public abstract class BlockCategoryAttribute : Attribute 
    {
        /// <summary>
        /// returns the short name of this attribute.
        /// </summary>
        /// <returns></returns>
        public abstract String GetName();

        /// <summary>
        /// returns the description for this Block category.
        /// </summary>
        /// <returns></returns>
        public abstract String GetDescription();

        public abstract Image CategoryImage();

        /// <summary>
        /// can be overridden to perform extra tests to see if this category should be shown or not.
        /// </summary>
        /// <returns>True to show the category. False to hide it (and thus any and all blocks that fall in it)</returns>
        public virtual bool ShowCategory()
        {

            return true;
        }

    }

    
    public class PassiveEffectCategoryAttribute : BlockCategoryAttribute
    {
        public override string GetName()
        {
            return "Passive Effect";
        }
        public override string GetDescription()
        {
            return "'Passive' Effect blocks don't bounce the ball, but rather effect the ball as it passes through it, or perform other effects when the ball passes through.";
        }

        public override Image CategoryImage()
        {
            return null;
        }
    }
    public class PolygonBlockCategoryAttribute : BlockCategoryAttribute
    {
        public override Image CategoryImage()
        {
            return null;
        }
        public override string GetName()
        {
            return "Polygon Blocks";
        }
        public override string GetDescription()
        {
            return "Polygonal Blocks (non rectangular)";
        }
    }
    public class StandardBlockCategoryAttribute : BlockCategoryAttribute
    {
        public override Image CategoryImage()
        {
            return null;
        }
        public override string GetName()
        {
            return "Standard Blocks";
        }
        public override string GetDescription()
        {
            return "Standard Block types.";
        }


    }
    public class ImpactEffectBlockCategoryAttribute : BlockCategoryAttribute
    {
        public override Image CategoryImage()
        {
           return null;
        }
        public override string GetName()
        {
            return "Impact Effect Blocks";
        }
        public override string GetDescription()
        {
            return "Blocks whose effect occurs on impact.";
        }

    }
    public class PowerupEffectCategoryAttribute : BlockCategoryAttribute
    {

        public override string GetName()
        {
            return "Powerups";
        }
        public override string GetDescription()
        {
            return "Blocks that provide a power-up, or reduce your abilities.";
        }
        public override Image CategoryImage()
        {
            return null;
        }

    }
    public class MaliceCategoryAttribute : BlockCategoryAttribute
    {

        public override string GetName()
        {
            return "Malicious Blocks";
        }
        public override string GetDescription()
        {
            return "Blocks that spawn enemies, attack, or have an otherwise detrimental effect on the game.";
        }
        public override Image CategoryImage()
        {
            return null;
        }
    }


}
