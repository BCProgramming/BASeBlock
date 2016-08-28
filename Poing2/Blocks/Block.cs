
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
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Xml.Linq;
using BASeCamp.BASeBlock.Events;
using BASeCamp.BASeBlock.GameObjects.Orbs;
using BASeCamp.BASeBlock.Particles;
using BASeCamp.Elementizer;

/*
 * Block Ideas
 * WindBlock
 * BlackHole/Gravity Block
 * Anomaly Block- instead of bouncing, ball flits in random direction at the same speed.
 * Laser Block- similar to "ray" block, but doesn't need to be shot, and shoots balls in a specified direction after a given timeout.
 * 
 * */


namespace BASeCamp.BASeBlock.Blocks
{





    /// <summary>
    /// used to indicate to the Level editor's menuing code that a block has many-to-one instancing.
    /// What this means is that the block will have multiple entries.
    /// </summary>
    /// 
    /// When the editor sees that a Block Type has a ManyToOneBlockAttribute set, instead of adding a single item for that 
    /// type, it will call into a static method on that type: "GetManyToOneBlockData()", which will return
    /// an array (or IEnumerable) of ManyToOneBlockData objects.
    /// 
    


    ///<summary>Defines an attribute that is set on blocks to indicate that they should be represented by multiple entries in
    /// Block listings and drop-downs. This means that they should provide a static Instantiate Method for instantiating one of those multiple definitions:
    /// static Block Instantiate(ManyToOneBlockData data, RectangleF blockrect)
    /// 
    /// as well as a static GetmanyToOneData() method that returns an array of ManyToOneBlockData[], indicating the valid Multiple types; when instantiating
    /// a "multiple" block of this type, the Instantiate() static method is called with the appropriate ManyToOneBlockData[].
    /// 
    /// Arguably this attribute would make more sense as an interface, but we can't have static interfaces so I guess it doesn't really matter
    /// at this point.
    /// public static ManyToOneBlockData[] GetManyToOneData()
    /// </summary>
    public class ManyToOneBlockAttribute:Attribute
    {


    }
    /// <summary>
    /// Class that represents Data to be used for instantiating a block that has the ManyToOneBlockAttribute attribute.
    /// This is used by such implementations to both return the multiple types that the Block can be instantiated with, by calling GetManytoOneData().
    /// The specific block that is represented by that type is created using Instantiate(). Block types that require more data (most will) for instantiation
    /// can create a derived class, or use the provided subclass which adds a Type parameter and relevant property of that type.
    /// </summary>
    public class ManyToOneBlockData
    {
        private Type _OwnerType;
        private String _DisplayText;
        private Image _DisplayImage;
        

        public Type OwnerType
        {
            get { return _OwnerType; }
            set { _OwnerType = value; }
        }

        public String DisplayText
        {
            get { return getDisplayText(); }
            set { setDisplayText(value); }
        }

        public Image DisplayImage
        {
            get { return getDisplayImage(); }
            set { setDisplayImage(value); }
        }
      
        public virtual String getDisplayText()
        {
            return _DisplayText;
        }

        public virtual void setDisplayText(String newvalue)
        {

            _DisplayText = newvalue;
        }
        public virtual Image getDisplayImage()
        {
            return _DisplayImage;
        }
        public virtual void setDisplayImage(Image value)
        {
            _DisplayImage = value;

        }

        public ManyToOneBlockData(Type ownerblock,String displaytext,Image drawimage)
        {
            _OwnerType=ownerblock;
            _DisplayText = displaytext;
            _DisplayImage = drawimage;
            
        }


    }
    public class ManyToOneBlockData<T> : ManyToOneBlockData
    {
        private T _Value;
        public T Value { get { return _Value; } set { _Value = value; } }

        public ManyToOneBlockData(Type pownerblock,String pdisplaytext,Image pdrawimage):base(pownerblock,pdisplaytext,pdrawimage)
        {
            
        }
        public ManyToOneBlockData(Type pownerblock,String pdisplaytext,Image pdrawimage,T value):this(pownerblock,pdisplaytext,pdrawimage)
        {

        }

    }



    public class ManyToOneTestBlockData : ManyToOneBlockData
    {
        public Color displaycolor;
        public ManyToOneTestBlockData(Type ownertype,String pdisplaytext, Color pusecolor):base(ownertype,pdisplaytext,null)
        {

            DisplayImage = null;
            DisplayText = pdisplaytext;
            displaycolor = pusecolor;
        }

    }


    [BBEditorInvisible]
    [ManyToOneBlock]
    [Serializable]
    public class ManyToOneTestBlock:Block 
    {
        public Color DrawColor;


        public static ManyToOneBlockData[] GetManyToOneData()
        {
            return new ManyToOneBlockData[] { new ManyToOneTestBlockData(typeof(ManyToOneTestBlock),"A Red Block",Color.Red),
new ManyToOneTestBlockData(typeof(ManyToOneTestBlock),"A Green Block",Color.Green)};

        }
        public static Block Instantiate(ManyToOneBlockData Fromdata,RectangleF userectangle)
        {

            NormalBlock nb = new NormalBlock(userectangle);
            nb.BlockColor = ((ManyToOneTestBlockData)Fromdata).displaycolor;

            return nb;

        }
        public ManyToOneTestBlock(RectangleF rect)
        {
            BlockRectangle = rect;

        }
        public ManyToOneTestBlock(ManyToOneTestBlock tester)
            : this(tester.BlockRectangle)
        {


        }

        public override object Clone()
        {
            return new ManyToOneTestBlock(this);
        }
    }


    public static class LinkedListExtender
    {
        //public AddRange(thi
        public static void AddRangeAfter<T>(this LinkedList<T> ll, IEnumerable<T> Range)
        {
            foreach (T loopvalue in Range)
            {
                ll.AddLast(loopvalue);


            }

        }

        //public static test()
        //{

        //    x.AddRange(

        //}



    }
    //the following class is only required for mono...
    // for some reason there is a Sum() function defined in .NET 3.5 but it's not defined by
    //mono.... so I cheat and create an extension method.
#if MONO
    public static class monoextensions
    {
        public static float Sum(this IEnumerable<float> floats)
        {
            float accumulator = 0;
            foreach (float loopfloat in floats)
                accumulator += loopfloat;

            return accumulator;


        }



    }


#endif
    /// <summary>
    /// Attribute that forces the block to be visible in the editor, even if it inherited the Invisible attribute from it's superclass.
    /// </summary>
    public class BBEditorVisibleAttribute : Attribute
    {
        public BBEditorVisibleAttribute()
        {
        }

    }
    /// <summary>
    /// Attribute that hides a block, so it doesn't display in the editor.
    /// </summary>
    public class BBEditorInvisibleAttribute : Attribute
    {
        public BBEditorInvisibleAttribute()
        {


        }


    }


    //block that derives from NormalBlock: BatchedDrawBlock
    //purpose: only ONE of these blocks has it's Draw Method called. additionally, it seals the standard "Draw" routine and instead provides a "new"
    //abstract Draw that passes in the group of blocks to draw.

    [Serializable]

    public abstract class BatchedDrawBlock : Block
    {
        private bool _isMainBlock = false; //determines if this is the primary block for drawing.
        private bool pframe = false;
        protected bool isMainBlock { get { return _isMainBlock; } set { _isMainBlock = value; } }
        BCBlockGameState ownerstate = null;
        public sealed override void Draw(Graphics g)
        {
            if (!isMainBlock && ownerstate != null) return;
            //only ONE batchedDrawBlock of each type manages the drawing of all BatchedDrawBlocks in the level of that type.
            

            //first, collect all the BatchedDrawBlocks of our type into a list.
            List<BatchedDrawBlock> grabourblocks=null;
            if (ownerstate != null)
            {
                grabourblocks =
                    (from n in ownerstate.Blocks
                     where n.GetType() == GetSpecificBlockType()
                     select (BatchedDrawBlock)n).ToList();
            }
            else
            {
                //otherwise, only have this one. meh...
                grabourblocks = new List<BatchedDrawBlock>() { this };

            }


            //call the abstract method...
            Draw(g, grabourblocks); 
            
            //ok, all done.


            
        }
        protected BatchedDrawBlock(SerializationInfo info,StreamingContext context):base(info,context)
        {



        }
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
        }
        protected BatchedDrawBlock(RectangleF drawrect)
        {
            //set a handler for the event...
            OnBlockDestroy += BatchedDrawBlock_OnBlockDestroy;

        }

        protected BatchedDrawBlock(BatchedDrawBlock clonethisblock):base(clonethisblock)
        {
            


        }
      
        void BatchedDrawBlock_OnBlockDestroy(Object Sender,BlockHitEventArgs<bool> e )
        {
            //if we are the main block, and are being destroyed, we need to set another main block for this BatchedDrawBlock type.
            //we just grab another one. If none exist- oh well.
            if (_isMainBlock)
            {
                //select a new block to be the main block.
                var grabit = from n in e.GameState.Blocks where n.GetType() == GetSpecificBlockType() select n;
                if(grabit.Count()!=0)
                    ((BatchedDrawBlock)grabit.First()).isMainBlock = true;




            }
            //throw new NotImplementedException();
            e.Result = true;
        }
        public override bool RequiresPerformFrame()
        {
            return pframe;
        }

        public override bool PerformFrame(BCBlockGameState gamestate)
        {
            pframe=false;
            ownerstate=gamestate;
            //if there are no other blocks in the gamestate of our type that have isMainBlock set, set ours. Either way, we won't "PerformFrame" again.


            if(!(from n in gamestate.Blocks where n.GetType()==GetSpecificBlockType() select n).Any((o)=>((BatchedDrawBlock)o).isMainBlock))
            {
                //doesn't contain Any that are the main block. Set ours.
                _isMainBlock=true;

            }

            //return true;
            return base.PerformFrame(gamestate);
        }

        public abstract Type GetSpecificBlockType();






        public abstract void Draw(Graphics g, List<BatchedDrawBlock> BlocksDraw);

    }

    

    /*
    public class NormalBatchBlock : BatchedDrawBlock 
    {
        public override Type GetSpecificBlockType()
        {
            return typeof(NormalBatchBlock);
        }
        public NormalBatchBlock(RectangleF rectangletest):base(rectangletest)
        {



        }
        public NormalBatchBlock(NormalBatchBlock cloneblock):base(cloneblock)
        {
            

        }
        public override object Clone()
        {
            return new NormalBatchBlock(this);
        }
        public override void Draw(Graphics g, List<BatchedDrawBlock> BlocksDraw)
        {
            //GraphicsPath gpath = new GraphicsPath();
            //Region createregion = new Region();

            //we want to only draw the outline blocks.
            //solution ought to be to combine all the passed blocks into a large poly and stroke & Fill that poly as needed.


            
            



            //throw new NotImplementedException();
        }

    }

     * 
     * 
     * 
    */

    


    /*
     * Blocks we should have (# mark means working)
     * #Normal Block
     * ----------------------------------
     * Standard block. adds a few token points to the score and is destroyed.
     * 
     * Strong Block
     * ----------------------------------
     * Similiar to the Normal block, but takes several hits to destroy. Either use a set of different images (like Poing I) or 
     * simply draw a black rectangle over top of the image with a higher alpha the more damage.
     * 
     * #Invincible Block
     * -----------------------------------
     * Impervious block that can never be destroyed remember to find a good metallic "ting" sound.
     * #Add Ball Block
     * ------------------------------------
     * Adds a single ball to the play field. ATM it shoots in a random direction at a specific speed, which works fine
     * 
     * #Ray Block
     * shoots a single temp ball straight up. (perhaps change this to allow for left/right/down shooting as well, which could help for complicated puzzles)
     * 
     * */
    
    [Serializable]

    public abstract class Block : ExpandableObjectConverter, ICloneable, ISerializable, IDeserializationCallback,iLocatable ,IEditorBlockExtensions,iImagable,IXmlPersistable
    {
        protected bool _AutoRespawn = false;
        protected bool _Destructable = true;
        protected List<IBlockEffect> _BlockEffects = new List<IBlockEffect>();
        /// <summary>
        /// Determines if this block will respawn when a life is lost.
        /// </summary>
        public bool AutoRespawn { get { return _AutoRespawn; } set { _AutoRespawn = value; } }
        
        public virtual bool Destructable { get { return _Destructable; } set { _Destructable = value; } }
        private String _DefaultHitSound = "bbounce";
        public String DefaultHitSound { get { return _DefaultHitSound; } set { _DefaultHitSound = value; } }
        
        [Editor(typeof(GenericCollectionEditor<IBlockEffect>),typeof(UITypeEditor))]
        public List<IBlockEffect> BlockEffects { get { return _BlockEffects;} 
            set 
            { 
                if(value==null) value = new List<IBlockEffect>();
            
                _BlockEffects=value;}}

        protected Polygon _BlockPoly;
      
#if DEBUG
        protected RectangleF _BlockRect;
        protected RectangleF mBlockrect
        {
            get { return _BlockRect; }
            set {
                if (Single.IsNaN(value.X) || Single.IsNaN(value.Y))
                    Debug.Print("NaN...");
                
                _BlockRect = value; }


        }
#else
        protected RectangleF mBlockrect;
#endif 

        //EditorBlock Extensions
        public virtual void EditorDraw(Graphics g, IEditorClient Client)
        {
            Draw(g);

        }
        public virtual String GetToolTipInfo(IEditorClient Client)
        {
            StringBuilder buildreturn = new StringBuilder();
            //retrieve event and trigger information.
            if (BlockTriggers.Count > 0)
            {
                buildreturn.Append(BlockTriggers.Count + " Triggers.(");
                buildreturn.Append(String.Join(",", (from p in BlockTriggers where 
                                                         p is IEditorExtensions select (p as IEditorExtensions).GetToolTipInfo(Client) )));
                buildreturn.AppendLine(")");

            }
            if (BlockEvents.Count > 0)
            {
                buildreturn.Append(BlockEvents.Count + " Events.(");
                buildreturn.Append(String.Join(",",
                    (from p in BlockEvents where p is IEditorExtensions select (p as IEditorExtensions).GetToolTipInfo(Client))));
                buildreturn.AppendLine(")");

            }


            return "";

        }
        public virtual void DrawSelection(Brush selectionbrush, Graphics g, IEditorClient Client)
        {
            var userect = BlockRectangle;
            userect.Inflate(1, 1);
            g.FillRectangle(selectionbrush, userect);

        }

        public virtual Polygon GetPoly()
        {
            //make sure the setter is called, it recreates the poly.
            if (_BlockPoly == null) BlockRectangle = BlockRectangle;
            return _BlockPoly;

        }
        
        //public int TriggerID { get; set; }
        private List<BlockTrigger> _BlockTriggers = new List<BlockTrigger>();

        //[MergableProperty(true)]
        [Editor(typeof(BlockTriggerCollectionEditor), typeof(UITypeEditor))]
        //[Editor(typeof(ItemTypeEditor<BlockTrigger>),typeof(UITypeEditor))]
        public List<BlockTrigger> BlockTriggers
        {
            get { return _BlockTriggers; }
            set { _BlockTriggers = value; }
        }
        private List<BlockEvent> _BlockEvents = new List<BlockEvent>();
        //[Editor(typeof(ObjectTypeEditor), typeof(UITypeEditor))]

        //[MergableProperty(true)]
        [Editor(typeof(BlockTriggerEventCollectionEditor), typeof(UITypeEditor))]
        //[Editor(typeof(ItemTypeEditor<BlockEvent>), typeof(UITypeEditor))]
        public List<BlockEvent> BlockEvents
        {
            get { return _BlockEvents; }
            set { _BlockEvents = value; }
        }
        
        public delegate bool OnBlockHitFunc(Block block, BCBlockGameState gamestate, cBall ballparam, ref List<cBall> ballsadded, ref bool nodefault);
        //public event OnBlockHitFunc OnBlockHit;
        public event EventHandler<BASeBlock.Events.BlockHitEventArgs<bool>> OnBlockHit;
        public event EventHandler<BASeBlock.Events.BlockHitEventArgs<bool>> OnBlockDestroy;
        //public event OnBlockHitFunc OnBlockDestroy;

        public event Action<RectangleF> OnBlockRectangleChange;
        /// <summary>
        /// Array containing the set of valid Powerups types that this block can generate when hit.
        /// </summary>
        protected Type[] Powerups = GamePowerUp.GetPowerUpTypes();
        protected float[] Powerupchance = GamePowerUp.GetPowerUpChance();
        //powerups defaults to all powerups...
        //powerupchance defaults to all 1's.
        float mPowerupChanceSum = 0;
        public float chancepowerup = 0.25f;

        public void DrawDamage(Graphics g, IDamageableBlock forblock)
        {
            DrawDamage(g, forblock.Damage, forblock.Health);
        }
        Image[] BlockCrackOverlay = null;
        public void DrawDamage(Graphics g, float CurrentDamage, float MaxDamage)
        {

            //draws the damage item for this block.
            //first, get all the "Crack" sprites....
            //if we have no damage, then we cahn early exit.
            if (CurrentDamage <= 0) return;
            int FrameCount;
            if(BlockCrackOverlay==null)
                BlockCrackOverlay = BCBlockGameState.Imageman.getImageFrames("CRACK", out FrameCount);

            //acquire the appropriate index...
            //we use max+1 because we need to account for having no cracked image overlay at all.
            int chosenIndex = (int)(Math.Floor(CurrentDamage / (MaxDamage+1)));
            BCBlockGameState.ClampValue(chosenIndex, 1, (MaxDamage+1));
            chosenIndex--;
            if (chosenIndex <= 0) return;
            //grab that image...
            Image drawthis = BlockCrackOverlay[chosenIndex];
            g.DrawImage(drawthis, BlockRectangle.Left, BlockRectangle.Top, BlockRectangle.Width, BlockRectangle.Height);
        }
        


        /// <summary>
        /// used to callback into the application when a category item is added.
        /// </summary>
        /// <param name="addeditem"></param>
        /// <param name="CategoryType"></param>
        public delegate void BlockCategoryAddedCallback(ToolStripMenuItem addeditem,BlockCategoryAttribute bca);


        public delegate void BlockItemAddedCallback(ToolStripMenuItem addeditem, BlockData addeddata);


        public delegate void BlockItemClick(ToolStripMenuItem clickeditem, BlockData dataclick);
        //Static helper methods.
        //this is a direct call. most useful for MultiBlockType's. The client
        //would then clone this block as needed for it's dark purposes.
        public delegate void BlockItemClickDirect(ToolStripMenuItem clickeditem, Block blockobject,BlockData bdata,ManyToOneBlockData mtodata);




        //main muscle class that works with the below implementation and standalone to list all Block Types that have given attributes or no attributes.
        //null means it needs no attributes at all. Pass in the base class Type, typeof(BlockCategoryAttribute) to force it to add <ALL> Blocks to the listing.
        public static void PopulateDropDownWithBlocks(ToolStripDropDown ondropdown, bool Clear,
            Type requireattribute, BlockItemAddedCallback addedcallback, 
            BlockItemClick clickcallback, 
            BlockItemClickDirect ClickDirect,bool ExpandManyToOne)
        {
            if (Clear) ondropdown.Items.Clear();
            //Iterate over all available managed block types.
            Func<Type, Type, bool> HasAttribute = BCBlockGameState.HasAttribute;
            foreach (BlockData bdata in BCBlockGameState.BlockDataMan.BlockInfo)
            {
                //now, we need to determine whether we should add this item.
                

                if (requireattribute == typeof(BlockCategoryAttribute))
                { }
                else if (requireattribute == null)
                {
                    //null, so cannot have any categories set at all.
                    if((Attribute.GetCustomAttributes(bdata.BlockType).Any((p) => p.GetType().IsSubclassOf(typeof(BlockCategoryAttribute)))))


                  
                        continue;
                }
                else
                {

                    //not null, so btype.BlockType has to be that category.
                    //reverse logic from above; continue the loop if it does <not> have the item.
                    if (!HasAttribute(bdata.BlockType, requireattribute))
                        continue;
                }

                //if we get here, we are "safe" to add the item to the dropdown.
                 bool invisattrib = BCBlockGameState.HasAttribute(bdata.BlockType, typeof(BBEditorInvisibleAttribute));
                bool vistrib = BCBlockGameState.HasAttribute(bdata.BlockType, typeof(BBEditorVisibleAttribute));
                bool shiftpressed = KeyboardInfo.IsPressed(Keys.Shift);
                if (!invisattrib || vistrib || shiftpressed)
                {

                    //this part is a bit sticky. For ManyToOneBlocks, we want them to support adding multiple items.
                    //as such we need to acquire the attribute data.


                    if (ExpandManyToOne && HasAttribute(bdata.BlockType, typeof(ManyToOneBlockAttribute)))
                    {
                        foreach (var iterateitem in bdata.GetManyToOneAttributeData().AcquiredData)
                        {
                            ToolStripItem createsubitem = ondropdown.Items.Add(iterateitem.DisplayText,
                                                                          iterateitem.DisplayImage);
                            createsubitem.Click += new EventHandler(manytoone_Click);
                            createsubitem.Tag = new Object[] { iterateitem, bdata, clickcallback,ClickDirect };


                            ondropdown.Items.Add(createsubitem);


                        }
                    }
                    else
                    {
                        //ToolStripItem createitem = new ToolStripItem(loopdat.Usename, loopdat.useBlockImage, eventhandle);

                        //Extra code ...in order to allow a single Type to show up as multiple entries


                        ToolStripItem createitem = ondropdown.Items.Add(bdata.Usename, bdata.useBlockImage);

                        //the tag needs two pieces of data; the BlockData (bdata) and the callback function.
                        Object[] usetag = new object[] { bdata, clickcallback,ClickDirect }; ;

                        createitem.Tag = usetag;
                        //hook it with a "normal" routine.
                        createitem.Click += new EventHandler(normalitem_Click);
                        if (addedcallback != null) addedcallback(createitem as ToolStripMenuItem, bdata);
                    }







                }





            }




        }

        static void normalitem_Click(object sender, EventArgs e)
        {
            //sender.tag is object[] array.
            //first item is BlockData.
            //second item is callback.
            //third item is the <Direct> callback.
            ToolStripMenuItem menuitem = sender as ToolStripMenuItem;
            object[] parameters = (Object[])menuitem.Tag;

            BlockData bdata = (BlockData)parameters[0];
            BlockItemClick bic = (BlockItemClick)parameters[1];
            BlockItemClickDirect bicd = (BlockItemClickDirect)parameters[2];

            //call first callback.
            if (bic != null) bic(menuitem, bdata);
            if (bicd != null)
            {
                //create a block. Default size.
                Block instantiated = bdata.Instantiate(DefaultRect);
                bicd(menuitem, instantiated,bdata,null);



            }





        }
        public static readonly RectangleF DefaultRect = new RectangleF(0, 0, 33, 16);
        static void manytoone_Click(object sender, EventArgs e)
        {
            //sender.tag is a object[] array.
            //first item is a ManyToOneBlockData
            //second item is BlockData
            //third item is a BlockItemClick callback routine.
            //fourth item is the direct callback, which accepts a Block object and our sender.

            ToolStripMenuItem clickeditem = sender as ToolStripMenuItem;
            Object[] parms = (Object[])clickeditem.Tag;
            ManyToOneBlockData mtodata = (ManyToOneBlockData)parms[0];
            BlockData bdata = (BlockData)parms[1];
            BlockItemClick bic = (BlockItemClick)parms[2];
            BlockItemClickDirect bicd = (BlockItemClickDirect)parms[3];

            //we know the type itself, so we can call the basic click callback.
            Debug.Print("Calling Bic");
            if (bic != null) bic(clickeditem, bdata);

            
            if (bicd != null)
            {
                
                //ManyToOneBlocks need to support this:
                //static Block Instantiate(ManyToOneBlockData data, RectangleF blockrect)
                Type blocktype = mtodata.OwnerType;
                //now we need to Instantiate it. acquire the Method...
                MethodInfo mi = blocktype.GetMethod("Instantiate", new Type[] { typeof(ManyToOneBlockData), typeof(RectangleF) });

                if (mi != null)
                {
                    Block gottenblock = (Block)mi.Invoke(null, BindingFlags.Static, (Binder)null, new object[] { mtodata, DefaultRect },Thread.CurrentThread.CurrentCulture);
                    Debug.Print("Calling bicd...");
                    bicd(clickeditem, gottenblock,bdata,mtodata);



                }

            }
            



        }
        
        
        //routine for populating a Toolstrip with Blocks. This is the "high level" variant which actually fills the toolstrip
        //with the categories.
        public static void PopulateDropDownWithBlocksCategorized(ToolStripDropDown tsDropdown, bool Clear,
            BlockCategoryAddedCallback BlockCategoryCallback,
            BlockItemAddedCallback BlockItemAdded,
            BlockItemClick ClickItem, BlockItemClickDirect directclick, bool expandManyToOne)
        {

            if (Clear) tsDropdown.Items.Clear();

            int addedcategories = 0;
            foreach (var iterateattribute in BCBlockGameState.MTypeManager[typeof(BlockCategoryAttribute)].ManagedTypes)
            {
                //add a new item for each attribute.
                //we need to instantiate a copy, to call the appropriate methods. The constructor is parameterless:
                BlockCategoryAttribute bca = Activator.CreateInstance(iterateattribute) as BlockCategoryAttribute;

                if (bca == null)
                {

                    //balls...
                    Debug.Print("Failed to instantiate BlockCategory:" + iterateattribute.Name);

                }
                else
                {

                    //all is well.
                    //call the appropriate methods to create a new menu item.
                    //make sure we are "allowed" to first.
                    if (bca.ShowCategory())
                    {
                        Debug.Print("Category:" + bca.GetName());
                        //create the toolstripitem, hook dropdownopening, and call the CategoryAdded callback.
                        ToolStripMenuItem categoryitem = new ToolStripMenuItem(bca.GetName(), bca.CategoryImage());
                        //add the handler for the Opening Event.
                        categoryitem.DropDownOpening += new EventHandler(categoryitem_DropDownOpening);
                        //set the tags. it will need to know the Attribute and the callbacks for blockitemadded and blockitemclicked.

                        Object[] usecategorytag = new Object[] { bca, BlockItemAdded, ClickItem, directclick, expandManyToOne };
                        categoryitem.Tag = usecategorytag;
                        //give it a ghost subitem so the DropDownOpening event will fire.
                        categoryitem.DropDownItems.Add("GHOST");
                        if (BlockCategoryCallback != null) BlockCategoryCallback(categoryitem, bca);
                        tsDropdown.Items.Add(categoryitem);
                        addedcategories++;

                    }


                }




            }

            //well now that that's out of the way. If we added categories, also insert a separator.
            if (addedcategories > 0)
            {
                //add separator.
                tsDropdown.Items.Add(new ToolStripSeparator());

            }

            //now we add all the items that have no category attribute.
            PopulateDropDownWithBlocks(tsDropdown, false, (Type)null, BlockItemAdded, ClickItem, directclick, expandManyToOne);
        }
        static void categoryitem_DropDownOpening(object sender, EventArgs e)
        {
            //sender.tag is an Object array.
            //first element is a BlockCategoryAttribute
            //second element is the BlockItemAdded callback
            //third element is the clickItem callback
            //fourth is direct click item callback.
            //fifth is manytoone boolean.
            //first cast to Object[].
            ToolStripMenuItem categoryitem = (ToolStripMenuItem)sender;
            Object[] objarr = (Object[])categoryitem.Tag;
            BlockCategoryAttribute bca = objarr[0] as BlockCategoryAttribute;
            BlockItemAddedCallback biac = objarr[1] as BlockItemAddedCallback;
            BlockItemClick bic = objarr[2] as BlockItemClick;
            BlockItemClickDirect bicd = objarr[3] as BlockItemClickDirect;
            bool manytoone = (bool)objarr[4];
            PopulateDropDownWithBlocks(categoryitem.DropDown, true, bca.GetType(), biac, bic,bicd,manytoone);


        }


        


        #region ExpandableObjectconverter
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            if (destinationType == typeof(Block))
                return true;
            return base.CanConvertTo(context, destinationType);


        }
        public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(System.String) &&
        value is Block)
            {

                Block so = (Block)value;

                return so.GetType().Name;
            }
            return base.ConvertTo(context, culture, value, destinationType);

        }
        #endregion
        protected void RandomSpawnPowerup(BCBlockGameState parentstate)
        {
            double nextdbl = BCBlockGameState.rgen.NextDouble();


            if (nextdbl < (double)chancepowerup)
            {
                GamePowerUp addpowerup = CreateRandomPowerup(parentstate,GamePowerUp.defaultSize);
                if (addpowerup != null)
                {
                    parentstate.NextFrameCalls.Enqueue(new BCBlockGameState.NextFrameStartup(() =>
                         parentstate.GameObjects.AddLast(addpowerup)));
                    
                }

            }
        }
        public void NullifyHooks()
        {
            OnBlockDestroy = null;
            OnBlockHit = null;
            OnBlockRectangleChange = null;


        }

        public float PowerupChanceSum()
        {
            if (mPowerupChanceSum == 0)
                mPowerupChanceSum = Powerupchance.Sum();

            return mPowerupChanceSum;


        }
        public Image DrawToImage(SizeF drawsize)
        {
            Bitmap resultimage = null;
            Graphics resultg = null;
            DrawBlock(this, out resultimage, out resultg,drawsize);
            return resultimage;


        }
        public Image DrawToImage()
        {
            return DrawToImage(BlockSize);

        }
        public static void DrawBlock(Type blocktype, out Bitmap blockbitmap, out Graphics bitmapcanvas)
        {
            DrawBlock(blocktype, out blockbitmap, out bitmapcanvas, new SizeF(32, 32));

        }
        public static void DrawBlock(Type blocktype, out Bitmap blockbitmap, out Graphics bitmapcanvas,SizeF drawsize)
        {
            Block createblock = (Block)Activator.CreateInstance(blocktype, new object[] { new RectangleF(new PointF(0, 0), drawsize) });
            DrawBlock(createblock, out blockbitmap, out bitmapcanvas, drawsize);


        }
        public static void DrawBlock(Block sourceblock, out Bitmap blockbitmap, out Graphics bitmapcanvas)
        {
            DrawBlock(sourceblock, out blockbitmap, out bitmapcanvas, sourceblock.BlockSize);

        }

        public static void DrawBlock(Block sourceblock, out Bitmap blockbitmap, out Graphics bitmapcanvas,SizeF drawsize)
        {
            
            //First step: create the bitmap.
            blockbitmap = new Bitmap((int)drawsize.Width, (int)drawsize.Height);
            bitmapcanvas = Graphics.FromImage(blockbitmap);

            //draw the block.
            RectangleF templocation = sourceblock.BlockRectangle;
            sourceblock.BlockRectangle = new RectangleF(0, 0, blockbitmap.Width, blockbitmap.Height);
            sourceblock.Draw(bitmapcanvas);
            sourceblock.BlockRectangle = templocation;
        }

        public GamePowerUp CreateRandomPowerup(BCBlockGameState gstate,SizeF usesize)
        {
            RectangleF userect = CenterRect(usesize);
            PointF position = new PointF(userect.Left, userect.Top);
            return CreateRandomPowerup(gstate,position, userect.Size);




        }
        
        public GamePowerUp CreateRandomPowerup(BCBlockGameState gstate,PointF Location, SizeF size)
        {
            Type[] ChooseFrom;
            
            if (gstate.PlayingLevel == null)
            {

                ChooseFrom = GamePowerUp.GetPowerUpTypes();
            }
            else
            {
                ChooseFrom = gstate.PlayingLevel.AvailablePowerups.ToArray();


                
            }
            
            List<Type> alltypes = GamePowerUp.GetPowerUpTypes().ToList();
            float[] allchance = GamePowerUp.GetPowerUpChance();

            float[] chancevalues = new float[ChooseFrom.Length];
            for (int i = 0; i < ChooseFrom.Length; i++)
            {

                int indexfound = alltypes.ToList().IndexOf(ChooseFrom[i]);
                chancevalues[i] = allchance[i];


            }

            Type usetype = null;

            while (usetype == null || usetype.ContainsGenericParameters)
            {
                usetype = BCBlockGameState.Select(ChooseFrom, chancevalues);
            }




            //Type usetype = Powerups[indexuse];
            //all Powerups must support a constructor like this:
            //ctor(PointF Location, SizeF Size)
            GamePowerUp madepowerup = null;
            if(usetype!=null)
                madepowerup = (GamePowerUp)Activator.CreateInstance(usetype, new Object[] { Location, size });
            
                
                
            
            return madepowerup;


        }

        private int RandomValueToIndex(float randvalue)
        {
            float currentaccum = 0;
            float prevaccum = 0;
            int i = 0;
            foreach (float loopvalue in Powerupchance)
            {

                currentaccum += loopvalue;
                if (currentaccum > randvalue)
                    return i;

                i++;


            }

            return i;


        }

        public static void PlayDefaultSound(cBall forball)
        {
            String usesound = forball.GetWallHitSound();
            BCBlockGameState.Soundman.PlaySound(usesound);



        }
        [Category("Size/Position"), Description("Location of this Block")]
        public PointF BlockLocation
        {
            get { return new PointF(mBlockrect.Left, mBlockrect.Top); }

            set
            {
                BlockRectangle = new RectangleF(value.X, value.Y, mBlockrect.Size.Width, mBlockrect.Size.Height);




            }


        }

        [Category("Size/Position"), Description("Size of this Block")]
        public SizeF BlockSize
        {
            get { return mBlockrect.Size; }
            set
            {

                mBlockrect = new RectangleF(mBlockrect.Left, mBlockrect.Top, value.Width, value.Height);




            }

        }

        internal Rectangle BlockRectangle_int { get; set; }


        [Category("Size/Position"), Description("Rectangle this block occupies")]
        [TypeConverter(typeof(FloatFConverter))]
        public virtual RectangleF BlockRectangle
        {
            get
            {
                return mBlockrect;

            }
            set
            {
                if (Single.IsNaN(value.X) || Single.IsNaN(value.Y))
                    Debug.Print("NaN...");
                mBlockrect = value;
                _BlockPoly = new Polygon(mBlockrect);
                RaiseBlockRectangleChange(mBlockrect);
                Debug.Assert(!(Single.IsNaN(mBlockrect.X) || Single.IsNaN(mBlockrect.Y)), "Rectangle has NaN Component");
                regenpoly();
                BlockRectangle_int = new Rectangle((int)mBlockrect.Left,(int)mBlockrect.Top,(int)mBlockrect.Width,(int)mBlockrect.Height);
                hasChanged = true;
            }
        }
        public virtual bool HitTest(PointF position)
        {
            //return BlockRectangle.Contains(position);
            return GetPoly().PointInPoly(position);

        }
        public virtual bool HitTest(RectangleF rect)
        {

            Polygon rectpoly = new Polygon(rect);
            var result = GeometryHelper.PolygonCollision(GetPoly(), rectpoly, Vector.Empty);
            return result.Intersect;

        }
        
        public PointF Location
        {
            get { return CenterPoint(); }

            set
            {

                mBlockrect = new RectangleF(value.X - (mBlockrect.Width / 2), value.Y - (mBlockrect.Height / 2), mBlockrect.Width, mBlockrect.Height);

            }
        }

        public Rectangle getRectangle()
        {
            return BlockRectangle.ToRectangle();
        }

        //public WeakReference ParentGame { get; set; }
        private Polygon ourpoly = null;
        private void regenpoly()
        {


            ourpoly = new Polygon(new PointF[] { new PointF(BlockRectangle.Left,BlockRectangle.Top),
                new PointF(BlockRectangle.Right,BlockRectangle.Top),
                new PointF(BlockRectangle.Right,BlockRectangle.Bottom),
                new PointF(BlockRectangle.Left,BlockRectangle.Bottom)});


        }

        public virtual PointF CenterPoint()
        {
            return new PointF(BlockRectangle.Left + (BlockRectangle.Width / 2), BlockRectangle.Top + (BlockRectangle.Height / 2));

            //return new PointF();

        }
        public TimeSpan TimeScore = new TimeSpan(0, 0, 0, 3); //default to 5 seconds.
        public RectangleF CenterRect(SizeF rectsize)
        {
            PointF gotcenter = CenterPoint();
            return new RectangleF(new PointF(gotcenter.X - rectsize.Width / 2, gotcenter.Y - rectsize.Height / 2), rectsize);



        }
        public static void AddScore(BCBlockGameState ParentGame, int addedscore, PointF Position)
        {
            addedscore = (int)((double)addedscore * ParentGame.ScoreMultiplier);
            ParentGame.GameScore += addedscore;
            Brush usetextbrush;
            if (addedscore < 0)
                usetextbrush = redbrush;
            else
                usetextbrush = BlackBrush;
            Font scorefont = new Font(BCBlockGameState.GetMonospaceFont(), 16);

            ParentGame.NextFrameCalls.Enqueue(new BCBlockGameState.NextFrameStartup(() =>
            ParentGame.GameObjects.AddLast(new BasicFadingText(addedscore.ToString(), Position, new PointF(((float)BCBlockGameState.rgen.NextDouble() * 0.2f) * -0.1f,
                ((float)BCBlockGameState.rgen.NextDouble()) * -0.7f), scorefont, new Pen(usetextbrush,1), usetextbrush))));

        }
        private static Brush redbrush = new SolidBrush(Color.Red);
        private static Brush BlackBrush = new SolidBrush(Color.Black);
        public void AddScore(BCBlockGameState ParentGame, int scoreadd)
        {
            Brush usetextbrush = null;
            if (scoreadd == 0) return;
            Font scorefont = new Font(BCBlockGameState.GetMonospaceFont(), 16);
            int addedscore = scoreadd + (ParentGame.GameObjects.Count * 10);
            SizeF textsize = ParentGame.ClientObject.MeasureString(addedscore.ToString(), scorefont);
            PointF aMidPoint = new PointF(mBlockrect.Left + (mBlockrect.Width / 2), mBlockrect.Top + (mBlockrect.Height / 2));
            PointF MidPoint = new PointF(aMidPoint.X - (textsize.Width / 2), aMidPoint.Y - (textsize.Height / 2));

            AddScore(ParentGame, scoreadd, MidPoint);
            //PointF MidPoint = new PointF(mBlockrect.Left + (mBlockrect.Width / 2), mBlockrect.Top + (mBlockrect.Height / 2));

           

        }
        
        public GameObject PopupText(BCBlockGameState ParentGame, String textshow, Font usefont)
        {

            
            
            return PopupText(ParentGame, textshow, usefont, new SolidBrush(Color.Gray), new Pen(Color.Black), 150);
            

        }
        public static GameObject PopupText(BCBlockGameState ParentGame, String textshow, Font usefont, Brush usebrush, Pen usepen, int timetolive, PointF Velocity, PointF InitialPosition)
        {


            var addedobject = new BasicFadingText(textshow, InitialPosition, Velocity, BCBlockGameState.GetScaledFont(usefont,(int)usefont.Size), usepen, usebrush, timetolive);
            ParentGame.GameObjects.AddLast(addedobject);
            return addedobject;
            

        }

        public GameObject PopupText(BCBlockGameState ParentGame, String textshow, Font usefont, Brush usebrush, Pen usepen, int Timetolive)
        {
            //SizeF textsize = ParentGame.ClientObject.MeasureString(textshow,usefont);
            //PointF aMidPoint = new PointF(mBlockrect.Left + (mBlockrect.Width / 2), mBlockrect.Top + (mBlockrect.Height / 2));
            //PointF MidPoint = new PointF(aMidPoint.X-(textsize.Width/2),aMidPoint.Y - (textsize.Height/2));
            //var addedobject = new BasicFadingText(textshow, MidPoint, , usefont,usepen,usebrush, Timetolive);
            //ParentGame.GameObjects.AddLast(addedobject); 
            //return addedobject;
            return PopupText(ParentGame, textshow, usefont, usebrush, usepen, Timetolive, new PointF(((float)BCBlockGameState.rgen.NextDouble() * 0.2f) * -0.1f, ((float)BCBlockGameState.rgen.NextDouble()) * -0.7f));


        }
        public GameObject PopupText(BCBlockGameState ParentGame, String textshow, Font usefont, Brush usebrush, Pen usepen, int timetolive, PointF Velocity)
        {
            SizeF textsize = ParentGame.ClientObject.MeasureString(textshow, usefont);
            PointF aMidPoint = new PointF(mBlockrect.Left + (mBlockrect.Width / 2), mBlockrect.Top + (mBlockrect.Height / 2));
            PointF MidPoint = new PointF(aMidPoint.X - (textsize.Width / 2), aMidPoint.Y - (textsize.Height / 2));
            return PopupText(ParentGame, textshow, usefont, usebrush, usepen, timetolive, Velocity, MidPoint);

        }

        protected void PlayBlockSound(cBall ballhit, String soundplay)
        {
            String ballhitsound = ballhit.getSpecificSound(this);

            if (ballhitsound != "")
                soundplay = ballhitsound;
            else if (soundplay == "")
            {
                soundplay = DefaultHitSound;
            }

            BCBlockGameState.Soundman.PlaySoundRnd(soundplay);




        }

        public virtual void Draw(Graphics g)
        {
            foreach (var iterate in BlockEffects)
                iterate.Draw(this, g);
            hasChanged = false;


        }

        public static void HandleEffects(BCBlockGameState gstate)
        {

            foreach(var lookblock in gstate.Blocks)
            {
                foreach(var iterate in from p in lookblock.BlockEffects where p.RequiresPerformFrame(lookblock) select p)
                {
                    iterate.PerformFrame(gstate,lookblock);

                }


            }

               



        }

        public virtual bool RequiresPerformFrame()
        {
            //return true if this block needs a frame-by-frame call to PerformFrame; otherwise return false.
            //BoundedMovingBlock, for example, overrides this and returns true.
            //return false for blocks that don't move or don't require a frame-by-frame update routine. (normal block and most ImageBlock derived blocks fall into this category).
            //return BlockEffects.Any((w) => w.RequiresPerformFrame(this));
            return false;

        }
        public virtual bool MustDestroy()
        {

            return true;

        }
        public virtual bool PerformFrame(BCBlockGameState gamestate)
        {
            //performs a frame of animation, or whatever, for this block.
            //note that by default this does nothing, and returns false to indicate no change.
            //true means the block needs to be redrawn.
         
            return false;
        }

        /// <summary>
        /// returns wether this block has changed since it was last drawn.
        /// </summary>
        [Browsable(false)]
        public bool hasChanged { get { return gethasChanged(); } set { sethasChanged(value); } }
        protected bool _hasChanged = false;

        protected virtual void sethasChanged(bool newvalue)
        {
            _hasChanged = newvalue;
        }
        protected virtual bool gethasChanged()
        {
            return _hasChanged;
        }

        protected Block(Block clonefrom)
        {
            AutoRespawn = clonefrom.AutoRespawn;
            BlockTriggers = new List<BlockTrigger>();
            BlockEvents = new List<BlockEvent>();
            BlockEffects = new List<IBlockEffect>();
            Destructable = clonefrom.Destructable;
            foreach (var triggerloop in clonefrom.BlockTriggers)
            {
                BlockTrigger copied = (BlockTrigger)triggerloop.Clone();
                //we need to set the copied instance's owner to ourselves.
                copied.OwnerBlock = this;
                BlockTriggers.Add(copied);

            }

            foreach (var eventloop in clonefrom.BlockEvents)
            {
                BlockEvent copied = (BlockEvent)eventloop.Clone();
                copied.OwnerBlock = this;
                BlockEvents.Add(copied);

            }
            foreach (var effectloop in clonefrom.BlockEffects)
            {
                IBlockEffect copied = effectloop.Clone() as IBlockEffect;
                BlockEffects.Add(copied);


            }
            
            
            BlockRectangle = clonefrom.BlockRectangle;



        }

        protected Block()
        {

            //empty...
            for (int i = 0; i < Powerupchance.Length; i++)
            {
                Powerupchance[i] = 1;


            }
            if (BlockTriggers == null) BlockTriggers = new List<BlockTrigger>();
            if (BlockEvents == null) BlockEvents = new List<BlockEvent>();
            // BlockTriggers.Add(new BlockTrigger(50, new TimeSpan(0, 0, 0, 4),this));
        }


       
     
        protected Block(SerializationInfo info, StreamingContext context)
            : this()
        {


            BlockEffects = (List<IBlockEffect>)info.GetValue("BlockEffects", typeof(List<IBlockEffect>));
            BlockTriggers = (List<BlockTrigger>)info.GetValue("BlockTriggers", typeof(List<BlockTrigger>));
            BlockEvents = (List<BlockEvent>)info.GetValue("BlockEvents", typeof(List<BlockEvent>));
            try { _AutoRespawn = info.GetBoolean("AutoRespawn"); }catch { }

            try { _Destructable = info.GetBoolean("Destructable"); }
            catch { _Destructable = true; }
            String[] poweruptypenames = (String[])info.GetValue("PowerUpTypes", typeof(string[]));
            Powerupchance = (float[])info.GetValue("PowerUpChance", typeof(float[]));

            Powerups = new Type[poweruptypenames.Length];
            for (int i = 0; i < Powerups.Length; i++)
            {

                Powerups[i] = Type.GetType(poweruptypenames[i]);


            }

            BlockRectangle = (RectangleF)info.GetValue("BlockRectangle", typeof(RectangleF));

        }

        [Flags]
        public enum BallRelativeConstants
        {
            Relative_None = -700,
            Relative_Up = 1,
            Relative_Left = 2,
            Relative_Down = 4,
            Relative_Right = 8



        }
        public PointF? IntersectLine(PointF LinePointA, PointF LinePointB)
        {

            return BCBlockGameState.IntersectLine(LinePointA, LinePointB, BlockRectangle);
        }

        public bool DoesIntersectLine(PointF LinePointA, PointF LinePointB)
        {

            return IntersectLine(LinePointA, LinePointB) != null;



        }


        public PointF? getcollisionPoint(BCBlockGameState gstate,cBall ballcheck)
        {
            PointF? tempresult;
            return getcollisionPoint(gstate,ballcheck, out tempresult);


        }

        /// <summary>
        /// returns the position at which the "ball" (well, it doesn't have to be a ball) collides with this block first.
        /// </summary>
        /// <param name="PreviousFramePosition">Previous PointF from last frame for the object.</param>
        /// <param name="currentFramePosition"> Current Location of object</param>
        /// <returns>Point on this block where the object hit this block; null if there is no intersection.</returns>
        public PointF? getcollisionPoint(BCBlockGameState gstate,cBall ballcheck, out PointF? usedoffset)
        {//change to use:

            float gotmult = gstate.GetMultiplier();
            PointF twaddledVelocity = new PointF(ballcheck.Velocity.X * gotmult, ballcheck.Velocity.Y * gotmult);

#if useoldballcode

            var collisionresult = GeometryHelper.PolygonCollision(ourpoly, ballcheck.GetBallPoly(), new Vector(ballcheck.Velocity.X, ballcheck.Velocity.Y));
            usedoffset = new PointF(collisionresult.MinimumTranslationVector.X, collisionresult.MinimumTranslationVector.Y);
            return new PointF(ballcheck.Location.X + usedoffset.Value.X, ballcheck.Location.Y + usedoffset.Value.Y);
#else

            var collisionresult = GeometryHelper.PolygonCollision(ourpoly, ballcheck.GetBallPoly(), new Vector(twaddledVelocity.X, twaddledVelocity.Y));
            usedoffset = new PointF(collisionresult.MinimumTranslationVector.X, collisionresult.MinimumTranslationVector.Y);
            return new PointF(ballcheck.Location.X + usedoffset.Value.X, ballcheck.Location.Y + usedoffset.Value.Y);

#endif
            //return null;




        }
        /// <summary>
        /// refactor this for the love of hamburgers. And possibly pancakes.
        /// </summary>
        /// <param name="WindowRect"></param>
        /// <param name="PointA"></param>
        /// <param name="PointB"></param>
        /// <returns></returns>
        public PointF[] LiangBarsky(RectangleF WindowRect, PointF PointA, PointF PointB)
        {
            return BCBlockGameState.LiangBarsky(WindowRect, PointA, PointB);
        }




        /// <summary>
        /// this routine has side effects! the given ball will be moved to the collision point.
        /// </summary>
        /// <param name="hitball">ball to check.</param>
        /// <param name="moveball">whether to move the ball's location to the calculated point.</param>
        /// <returns></returns>

        /// <summary>
        /// returns whether the given ball touches this block.
        /// </summary>
        /// <param name="hitball">Ball to test</param>
        /// <param name="usedoffset">out parameter to store "offset"; for example if a ball of radius 3 touches a block on it's left side this will be (-3,0)</param>
        /// <returns></returns>
        public virtual bool DoesBallTouch(cBall hitball)
        {
            PointF? getoffset;
            //PointF? collidepoint = getcollisionPoint(hitball,out getoffset);
            //ugh still broken...
            Vector Speeduse;
            float useX, useY;


            var collisionresult = GeometryHelper.PolygonCollision(ourpoly, hitball.GetBallPoly(), new Vector(Math.Abs(hitball.Velocity.X), Math.Abs(hitball.Velocity.Y)));


            return collisionresult.Intersect || collisionresult.WillIntersect;


        }
        public static bool BallTouches(cBall hitball,Polygon SourcePoly)
        {
            PointF? getoffset;
            Vector speeduse;
            float useX, useY;
            var collisionresult = GeometryHelper.PolygonCollision(SourcePoly, hitball.GetBallPoly(), new Vector(Math.Abs(hitball.Velocity.X), Math.Abs(hitball.Velocity.Y)));
            return collisionresult.Intersect || collisionresult.WillIntersect;
        }
        public static float min(params float[] values)
        {
            float currmin = float.MaxValue;
            foreach (float loopfloat in values)
            {
                if (loopfloat < currmin) currmin = loopfloat;

            }
            return currmin;



        }

        private static float Distance(PointF pointA, PointF PointB)
        {
            return (float)Math.Sqrt(Math.Pow(pointA.X + PointB.X, 2) + Math.Pow(pointA.Y + PointB.Y, 2));


        }
        public static double GetAngle(PointF PointA, PointF PointB)
        {
            return Math.Atan2(PointB.Y - PointA.Y, PointB.X - PointA.X);


        }

        public static BallRelativeConstants getMainDirection(double angletest)
        {

            PointF usevector = new PointF((float)Math.Cos(angletest) * 5, (float)Math.Sin(angletest) * 5);


            if (Math.Abs(usevector.X) > Math.Abs(usevector.Y))
            {
                //horizontal.
                return Math.Sign(usevector.X) > 0 ? BallRelativeConstants.Relative_Right : BallRelativeConstants.Relative_Left;

            }
           
                return Math.Sign(usevector.Y) > 0 ? BallRelativeConstants.Relative_Down : BallRelativeConstants.Relative_Up;
            




        }

        public virtual BallRelativeConstants getBallRelative(cBall forball)
        {
            if (BCBlockGameState.GameSettings["game"]["CollisionCode", "old"].Value == "new")
                return getBallRelativeNewest(forball);
            else
                return getBallRelativeOld(forball);

        }
        
        public static BallRelativeConstants BallRelative(RectangleF pRect, cBall source)
        {
            PointF[] CornerPoints = new PointF[4];
            RectangleF br = pRect;
            PointF centerpt = pRect.CenterPoint();

            CornerPoints[0] = new PointF(br.Left, br.Top);
            CornerPoints[1] = new PointF(br.Right, br.Top);
            CornerPoints[2] = new PointF(br.Right, br.Bottom);
            CornerPoints[3] = new PointF(br.Left, br.Bottom);
            float compareangle = (float)GetAngle(centerpt, source.Location);
            float[] cornerangles = new float[4];
            for (int i = 0; i < cornerangles.Length; i++)
            {
                cornerangles[i] = (float)GetAngle(centerpt, CornerPoints[i]);


            }

            //if between 0 and 1; top
            //1 and 2- Right.
            //2 and 3- bottom
            //anything else- left.
            if (compareangle > cornerangles[0] && compareangle < cornerangles[1])
            {
                return BallRelativeConstants.Relative_Up;

            }
            if (compareangle > cornerangles[1] && compareangle < cornerangles[2])
            {
                return BallRelativeConstants.Relative_Right;
            }
            if (compareangle > cornerangles[2] && compareangle < cornerangles[3])
            {
                return BallRelativeConstants.Relative_Down;

            }

            return BallRelativeConstants.Relative_Left;


        }

        public BallRelativeConstants getBallRelativeNewest(cBall forball)
        {
            //algorithm: get the angle between the center of the ball and the center of the block, then the angles between the center
            //of the block and it's four corner points. determine which direction the hit occured by testing which angles the ball->center angle lies between.
            PointF[] CornerPoints = new PointF[4];
            RectangleF br = BlockRectangle;
            PointF centerpt = CenterPoint();
            CornerPoints[0] = new PointF(br.Left, br.Top);
            CornerPoints[1] = new PointF(br.Right, br.Top);
            CornerPoints[2] = new PointF(br.Right, br.Bottom);
            CornerPoints[3] = new PointF(br.Left, br.Bottom);
            float compareangle = (float)GetAngle(centerpt, forball.Location);
            float[] cornerangles = new float[4];
            for (int i = 0; i < cornerangles.Length; i++)
            {
                cornerangles[i] = (float)GetAngle(centerpt, CornerPoints[i]);


            }

            //if between 0 and 1; top
            //1 and 2- Right.
            //2 and 3- bottom
            //anything else- left.
            if (compareangle > cornerangles[0] && compareangle < cornerangles[1])
            {
                return BallRelativeConstants.Relative_Up;

            }
            if (compareangle > cornerangles[1] && compareangle < cornerangles[2])
            {
                return BallRelativeConstants.Relative_Right;
            }
            if (compareangle > cornerangles[2] && compareangle < cornerangles[3])
            {
                return BallRelativeConstants.Relative_Down;

            }

            return BallRelativeConstants.Relative_Left;



        }

        public BallRelativeConstants getBallRelativeNew(cBall forball)
        {
            BallRelativeConstants returnval = 0;
            RectangleF br = BlockRectangle;
            PointF BlockCenter = CenterPoint();
            float middlex = br.Left + (br.Width / 2);
            float middley = br.Top + (br.Height / 2);
            float angletolerance = 0.1f;
            BallRelativeConstants[] returncorners = new BallRelativeConstants[4];
            returncorners[0] = BallRelativeConstants.Relative_Left | BallRelativeConstants.Relative_Up;
            returncorners[1] = BallRelativeConstants.Relative_Right | BallRelativeConstants.Relative_Up;
            returncorners[2] = BallRelativeConstants.Relative_Right | BallRelativeConstants.Relative_Down;
            returncorners[3] = BallRelativeConstants.Relative_Left | BallRelativeConstants.Relative_Down;
            var CornerPoints = new PointF[4];
            CornerPoints[0] = new PointF(br.Left, br.Top);
            CornerPoints[1] = new PointF(br.Right, br.Top);
            CornerPoints[2] = new PointF(br.Right, br.Bottom);
            CornerPoints[3] = new PointF(br.Left, br.Bottom);
            double[] CornerAngles = new double[4];
            for (int i = 0; i < CornerPoints.Length; i++)
            {
                CornerAngles[i] = GetAngle(BlockCenter, CornerPoints[i]);

            }

            //algorithm: first, see if the angle of the vector from the center of the block to the ball is within a tolerance of the four corner points; if so, we can be reasonably sure that the ball is touching that corner.
            //if none of these tests "go through" we will proceed to simply test the various X and Y coordinates.
            float Grabangle = (float)GetAngle(BlockCenter, forball.Location);


            //determine if any of the CornerAngles is within a given tolerance of Grabangle; if so, multiply the components of the balls speed by
            for (int i = 0; i < CornerAngles.Length; i++)
            {
                if (Math.Abs(Grabangle - CornerAngles[i]) <= angletolerance)
                {
                    return returncorners[i];

                }


            }
            //otherwise, we can safely assume it's not "on a corner" therefore we merely see which of the four sides it has hit.

            float[] distances = new float[4];
            distances[0] = Math.Abs(forball.Location.X - BlockRectangle.Left); //left
            distances[1] = Math.Abs(forball.Location.Y - BlockRectangle.Top); //top
            distances[2] = Math.Abs(forball.Location.X - BlockRectangle.Right); //Right
            distances[3] = Math.Abs(forball.Location.Y - BlockRectangle.Bottom); //Bottom
            BallRelativeConstants[] linearreturns = new BallRelativeConstants[4];
            linearreturns[0] = BallRelativeConstants.Relative_Left;
            linearreturns[1] = BallRelativeConstants.Relative_Up;
            linearreturns[2] = BallRelativeConstants.Relative_Right;
            linearreturns[3] = BallRelativeConstants.Relative_Down;

            int minindex = -1;
            float curminimum = 0;
            for (int i = 0; i < distances.Length; i++)
            {
                if (minindex == -1 || (distances[i] < curminimum))
                {
                    curminimum = distances[i];
                    minindex = i;
                }


            }

            return linearreturns[minindex];

        }
        public BallRelativeConstants getBallRelativeOld(cBall forball)
        {

            return getBallRelativeOld(forball, BlockRectangle);


        }

        public static BallRelativeConstants getBallRelativeOld(cBall forball, RectangleF userectangle)
        {
            //Pre: forball is touching/within this block.

            //return: a bitmask representing it's position. This is used to calc which direction to bounce it.




            //create a array of points, corresponding to the upper left point, middle centre point, top right point,
            //middle left,middle right, bottom left, bottom centre, bottom right of the block:
            BallRelativeConstants returnval = 0;
            RectangleF br = userectangle;

            float middlex = br.Left + (br.Width / 2);
            float middley = br.Top + (br.Height / 2);
            float[] dist = new float[8];
            PointF[] blockpoints = new PointF[] {new PointF(br.Left,br.Top),
            new PointF(middlex,br.Top),new PointF(br.Right,br.Top),  

            new PointF(br.Left,middley),new PointF(br.Right,middley),
  
            new PointF(br.Left,br.Bottom),new PointF(middlex,br.Bottom),  new PointF(br.Right,br.Bottom) 
            
            
            
            
            };


            //             For I = 0 To 7
            //          
            //        disti(I) = distance(BlockPoints(I).X, BlockPoints(I).Y, Pball.X, Pball.Y) + _
            //                Pball.radius
            //        'OK, scale to compensate for aspect.
            //          
            //        'negate the radius to use the surface.
            //    Next I

            for (int i = 0; i < 8; i++)
            {
                dist[i] = Distance(blockpoints[i], forball.Location) + forball.Radius;



            }

            //    If Pball.X - Pball.radius < Pblock.X Then
            //        'if the balls Left value us lower, then it's mroe left than the
            //        'Block, and so it's to the left.
            //        'its to the left.
            //        ReturnVal = ReturnVal + LEFTW
            //        Pball.X = Pblock.X - Pball.radius - 1
            //    End If
            if ((forball.Location.X - forball.Radius) < br.Left)
            {

                returnval |= BallRelativeConstants.Relative_Left;
                // forball.Location = new PointF(forball.Location.X-forball.Radius-1,forball.Location.Y);


            }

            if ((forball.Location.X + forball.Radius) > br.Right)
            {
                returnval |= BallRelativeConstants.Relative_Right;
                //forball.Location = new PointF(br.Right+forball.Radius+1,forball.Location.Y);


            }
            //    If Pball.X + Pball.radius > Pblock.X + Pblock.Bwidth Then
            //        ReturnVal = ReturnVal + RIGHTW
            //        Pball.X = Pblock.X + Pblock.Bwidth + Pball.radius + 1
            //    End If


            if ((forball.Location.Y - forball.Radius) < br.Top)
            {
                returnval |= BallRelativeConstants.Relative_Up;
                //forball.Location = new PointF(forball.Location.X,br.Top-forball.Radius-1);

            }
            //    If Pball.Y - Pball.radius < Pblock.Y Then
            //        Pball.Y = Pblock.Y - Pball.radius - 1
            //        ReturnVal = ReturnVal + UPW
            //    End If

            if ((forball.Location.Y + forball.Radius) > br.Bottom)
            {
                returnval |= BallRelativeConstants.Relative_Down;

            }



            //    If Pball.Y + Pball.radius > Pblock.Y + Pblock.Bheight Then
            //        ReturnVal = ReturnVal + DOWNW
            //        Pball.Y = Pblock.Y + Pblock.Bheight + Pball.radius + 1
            //    End If

            if (returnval == 0)
            {

                float lowestfound = min(dist);
                int lowestindex = 0;
                for (int k = 0; k < dist.Length; k++)
                {
                    if (lowestfound == dist[k])
                    {
                        lowestindex = k;
                        break;
                    }


                }







                //    If ReturnVal = 0 Then
                //        'Hmm, Somehow it got to zero... well, we'll se which Side it's Closest to, and return that.
                //        'generally, we just need to find which one has the lowest distance: the middle of the Top, left, botton or right.
                //        lowestVal = Min(disti(0), disti(1), disti(2), disti(3), disti(4), disti(5), _
                //                disti(6), disti(7))
                //        For I = 0 To 7
                //            If Round(lowestVal, 3) = Round(disti(I), 3) Then
                //                lowest = I
                //                Exit For
                //            End If
                //        Next I
                //        'now we should have a Index in Lowest. use that to determine the outcome.


                switch (lowestindex)
                {
                    case 0:
                        if (dist[3] < dist[1])
                            returnval = BallRelativeConstants.Relative_Left;
                        else
                            returnval = BallRelativeConstants.Relative_Up;

                        break;




                    //        Select Case lowest
                    //            Case 0
                    //                'top left
                    //                'if the top left is the closest, etermine which of either Left middle or top middle is closer:
                    //                'if the left middle is closer say it's leftward, otherwise, upward.
                    //                If disti(3) < disti(1) Then
                    //                    'the left is closer, send it up.
                    //                    ReturnVal = LEFTW
                    //                Else                     'otherwise, upward.
                    //                    ReturnVal = UPW
                    //                End If

                    case 1:
                        returnval = BallRelativeConstants.Relative_Up;
                        break;
                    //            Case 1
                    //                'Top middle
                    //                'if the top middle is closest, send it up.
                    //                ReturnVal = UPW
                    //            Case 2
                    case 2:
                        if (dist[1] < dist[4])
                            returnval = BallRelativeConstants.Relative_Up;
                        else
                            returnval = BallRelativeConstants.Relative_Right;

                        break;
                    //                'Top left
                    //                'if the top left is closest, see if either top middle if middle right is closer.
                    //                If disti(1) < disti(4) Then
                    //                    'top middle is closer, send it upward.
                    //                    ReturnVal = UPW
                    //                Else                     'otherwise, rightward Ho!
                    //                    ReturnVal = RIGHTW
                    //                End If
                    case 3:
                        returnval = BallRelativeConstants.Relative_Left;
                        break;
                    //            Case 3
                    //                'Left middle
                    //                ReturnVal = LEFTW
                    case 4:
                        returnval = BallRelativeConstants.Relative_Right;
                        break;
                    //            Case 4
                    //                'right middle
                    //                ReturnVal = RIGHTW
                    case 5:
                        if (dist[6] < dist[3])
                            returnval = BallRelativeConstants.Relative_Down;
                        else
                            returnval = BallRelativeConstants.Relative_Left;


                        break;

                    //            Case 5
                    //                'bottom left
                    //                'either bottom middle of left middle.
                    //                If disti(6) < disti(3) Then
                    //                    'bottom
                    //                    ReturnVal = DOWNW
                    //                Else
                    //                    ReturnVal = LEFTW
                    //                End If
                    //            Case 6
                    case 6:
                        returnval = BallRelativeConstants.Relative_Down;
                        break;
                    //                ReturnVal = DOWNW
                    //                'bottom middle
                    //            Case 7
                    //                'bottom right
                    //                'either bottom or right.
                    //                If disti(6) < disti(4) Then
                    //                    'Bottom is closer
                    //                    ReturnVal = DOWNW
                    //                Else
                    //                    ReturnVal = RIGHTW
                    //                End If
                    case 7:
                        if (dist[6] < dist[4])
                            returnval = BallRelativeConstants.Relative_Down;
                        else
                            returnval = BallRelativeConstants.Relative_Right;

                        break;
                    //        End Select
                    //    End If
                }
                //    getBallrelative = ReturnVal 
                //              
            }






            return returnval;




        }




        /*
         
         * Old VB6 code for GetBallRelative:
         * 
         * 
         * Public Enum RelativeBallModes
    UPW = 1
    LEFTW = 2
    DOWNW = 4
    RIGHTW = 8
End Enum
          Public Function getBallrelative(Pball As Ball) As RelativeBallModes
    'Returns A Bit Value.
    'returns a value that decides which direction it should go.
    Dim Pblock As Block
    Set Pblock = Me
    
    Dim BlockPoints() As PointAPI
    Dim ReturnVal     As RelativeBallModes, I As Integer
    Dim lowest%
    Dim lowestVal
    ReDim disti(7)
    ReDim BlockPoints(7)
    Const TOPLEFT = 0
    Const TOPMIDDLE = 1
    Const TOPRIGHT = 2
    Const MIDDLELEFT = 3
    Const MIDDLERIGHT = 4
    Const BOTTOMLEFT = 5
    Const BOTTOMMIDDLE = 6
    Const BOTTOMRIGHT = 7
     
    'create the middle points.
    '0----------1------------2
    '|#######################|
    '|#######################|
    '3#######################4
    '|#######################|
    '5-----------6-----------7
    BlockPoints(0).X = Pblock.X
    BlockPoints(0).Y = Pblock.Y
    BlockPoints(1).X = Pblock.X + (Pblock.Bwidth / 2)
    BlockPoints(1).Y = Pblock.Y
    BlockPoints(2).X = Pblock.X + Pblock.Bwidth
    BlockPoints(2).Y = Pblock.Y
    BlockPoints(3).X = Pblock.X
    BlockPoints(3).Y = Pblock.Y + (Pblock.Bheight / 2)
    BlockPoints(4).X = Pblock.X + Pblock.Bwidth
    BlockPoints(4).Y = BlockPoints(3).Y
    BlockPoints(5).X = Pblock.X
    BlockPoints(5).Y = Pblock.Y + Pblock.Bheight
    BlockPoints(6).X = Pblock.X + (Pblock.Bwidth / 2)
    BlockPoints(6).Y = Pblock.Y + (Pblock.Bheight)
    BlockPoints(7).X = Pblock.X + Pblock.Bwidth
    BlockPoints(7).Y = Pblock.Y + Pblock.Bheight
     
    For I = 0 To 7
          
        disti(I) = distance(BlockPoints(I).X, BlockPoints(I).Y, Pball.X, Pball.Y) + _
                Pball.radius
        'OK, scale to compensate for aspect.
          
        'negate the radius to use the surface.
    Next I
    If Pball.X - Pball.radius < Pblock.X Then
        'if the balls Left value us lower, then it's mroe left than the
        'Block, and so it's to the left.
        'its to the left.
        ReturnVal = ReturnVal + LEFTW
        Pball.X = Pblock.X - Pball.radius - 1
    End If
    If Pball.X + Pball.radius > Pblock.X + Pblock.Bwidth Then
        ReturnVal = ReturnVal + RIGHTW
        Pball.X = Pblock.X + Pblock.Bwidth + Pball.radius + 1
    End If
    If Pball.Y - Pball.radius < Pblock.Y Then
        Pball.Y = Pblock.Y - Pball.radius - 1
        ReturnVal = ReturnVal + UPW
    End If
    If Pball.Y + Pball.radius > Pblock.Y + Pblock.Bheight Then
        ReturnVal = ReturnVal + DOWNW
        Pball.Y = Pblock.Y + Pblock.Bheight + Pball.radius + 1
    End If
    If ReturnVal = 0 Then
        'Hmm, Somehow it got to zero... well, we'll se which Side it's Closest to, and return that.
        'generally, we just need to find which one has the lowest distance: the middle of the Top, left, botton or right.
        lowestVal = Min(disti(0), disti(1), disti(2), disti(3), disti(4), disti(5), _
                disti(6), disti(7))
        For I = 0 To 7
            If Round(lowestVal, 3) = Round(disti(I), 3) Then
                lowest = I
                Exit For
            End If
        Next I
        'now we should have a Index in Lowest. use that to determine the outcome.
        Select Case lowest
            Case 0
                'top left
                'if the top left is the closest, etermine which of either Left middle or top middle is closer:
                'if the left middle is closer say it's leftward, otherwise, upward.
                If disti(3) < disti(1) Then
                    'the left is closer, send it up.
                    ReturnVal = LEFTW
                Else                     'otherwise, upward.
                    ReturnVal = UPW
                End If
            Case 1
                'Top middle
                'if the top middle is closest, send it up.
                ReturnVal = UPW
            Case 2
                'Top left
                'if the top left is closest, see if either top middle if middle right is closer.
                If disti(1) < disti(4) Then
                    'top middle is closer, send it upward.
                    ReturnVal = UPW
                Else                     'otherwise, rightward Ho!
                    ReturnVal = RIGHTW
                End If
            Case 3
                'Left middle
                ReturnVal = LEFTW
            Case 4
                'right middle
                ReturnVal = RIGHTW
            Case 5
                'bottom left
                'either bottom middle of left middle.
                If disti(6) < disti(3) Then
                    'bottom
                    ReturnVal = DOWNW
                Else
                    ReturnVal = LEFTW
                End If
            Case 6
                ReturnVal = DOWNW
                'bottom middle
            Case 7
                'bottom right
                'either bottom or right.
                If disti(6) < disti(4) Then
                    'Bottom is closer
                    ReturnVal = DOWNW
                Else
                    ReturnVal = RIGHTW
                End If
        End Select
    End If
    getBallrelative = ReturnVal
     
End Function

         * */


        //public virtual 
        internal virtual bool RaiseBlockHit(BCBlockGameState parentstate, cBall ballhit, ref bool nodefault)
        {
            var copied = OnBlockHit;
            if (copied != null)
            {
                var makeentry = new BlockHitEventArgs<bool>(parentstate,this,ballhit);
                copied.Invoke(this,makeentry);
                return makeentry.Result;

                //return copied.Invoke(this, parentstate, ballhit, ref ballsadded, ref nodefault);
        }
            else
                return false;






        }
        protected void RaiseBlockRectangleChange(RectangleF newrect)
        {
            Action<RectangleF> copied = OnBlockRectangleChange;
            if (copied != null)
                copied.Invoke(newrect);


        }
        protected class OrbProbabilityData
        {
            public Type OrbType;
            public float Probability;
            public OrbProbabilityData(Type pOrbType)
            {
                OrbType = pOrbType;
        

            }
        }
        protected Dictionary<OrbProbabilityData, int[]> OrbProbData = null;

        protected bool _DoSpawnOrbs = true;
        public bool DoSpawnOrbs { get { return _DoSpawnOrbs; } set { _DoSpawnOrbs = false; } }
        protected Type[] ValidTypes = new Type[] { typeof(ScoreOrb), typeof(MacGuffinOrb), typeof(HealingOrb),typeof(PowerupOrb) };
        protected float[] prob = new float[] { 50, 40, 20,5 };
        protected virtual void CreateOrbs(PointF Location,BCBlockGameState gstate)
        {
            //if we aren't supposed to spawn orbs at all- don't!
            if (!DoSpawnOrbs) return;
            if (OrbProbData == null)
            {

                OrbProbData = new Dictionary<OrbProbabilityData, int[]>();
                OrbProbData.Add(new OrbProbabilityData(typeof(ScoreOrb)),new int[]{0,3});
                OrbProbData.Add(new OrbProbabilityData(typeof(MacGuffinOrb)), new int[] { 2, 15 });
                OrbProbData.Add(new OrbProbabilityData(typeof(HealingOrb)), new int[] { 4, 13 });
                OrbProbData.Add(new OrbProbabilityData(typeof(PowerupOrb)),new int[] {1,2});
            }

           //use Select routine to choose one from the dictionaries keys.
            OrbProbabilityData opd = BCBlockGameState.Select(OrbProbData.Keys.ToArray(), prob, BCBlockGameState.rgen);
            int spawnmin = OrbProbData[opd][0];
            int spawnmax = OrbProbData[opd][1];
            gstate.SpawnOrbs(BCBlockGameState.rgen.Next(spawnmin, spawnmax + 1), Location, opd.OrbType);

        }
        internal virtual bool RaiseBlockDestroy(BCBlockGameState parentstate, cBall ballhit, ref bool nodefault)
        {
            CreateOrbs(CenterPoint(),parentstate);
            var copied = OnBlockDestroy;
            if (copied != null)
            {
                var makeevent = new BlockHitEventArgs<bool>(parentstate, this, ballhit);
                copied(this, makeevent);
                return makeevent.Result;
            }
            else
                return false;


        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="parentstate"></param>
        /// <param name="ballhit"></param>
        /// <param name="ballsadded"></param>
        /// <returns>true to destroy this block. False otherwise.</returns>
        public virtual bool PerformBlockHit(BCBlockGameState parentstate, cBall ballhit)
        {
            //default to playing a sound and returning true (to indicate block was destroyed).
            //BCBlockGameState.Soundman.PlaySound("BBOUNCE", 1f);
            bool nodef = false;
            bool resultval = RaiseBlockHit(parentstate, ballhit, ref nodef);
            ballhit.numImpacts++;
            if (nodef)
                return resultval;

            

            //PointF middlespot = new PointF((float)BlockRectangle.Left + (BlockRectangle.Width / 2), (float)BlockRectangle.Top + (BlockRectangle.Height / 2));
            StandardSpray(parentstate, ballhit);

            //parentstate.Particles.AddRange(new Particle[] { new DustParticle(middlespot), new DustParticle(middlespot), new DustParticle(middlespot), new DustParticle(middlespot), new DustParticle(middlespot), new DustParticle(middlespot), new DustParticle(middlespot) });

            //parentstate.GameScore += 30;
            return true;

        }
        public void StandardSpray(BCBlockGameState parentstate)
        {
            StandardSpray(parentstate, null);
        }
        protected virtual Particle AddStandardSprayParticle(BCBlockGameState parentstate, cBall ballhit)
        {

            return AddSprayParticle_Default(parentstate, ballhit);

        }
        //
        protected Particle AddStandardSprayParticle<T>(BCBlockGameState parentstate, cBall ballhit) where T : Particle
        {
            PointF middlespot;
            middlespot = ballhit != null ? ballhit.Location : CenterPoint();

            EmitterParticle.EmissionRoutine Emissionroutine = ((gs,ep,TTL,TTLx)=> (T)Activator.CreateInstance(typeof(T),ep.Location,3));


            EmitterParticle createdEmitter = ballhit != null ? new EmitterParticle(ballhit, Emissionroutine)
                : new EmitterParticle(middlespot, Emissionroutine);
            createdEmitter.VelocityDecay = new PointF(0.99f, 0.99f);
            createdEmitter.Velocity = BCBlockGameState.GetRandomVelocity(1, 3);
            return createdEmitter;
        }

        protected Particle AddSprayParticle_Default(BCBlockGameState parentstate, cBall ballhit)
        {
            PointF middlespot;
            if (ballhit != null)
                middlespot = ballhit.Location;
            else
            {
                middlespot = CenterPoint();
            }
            //return ballhit != null ? new DustParticle(ballhit) : new DustParticle(middlespot, 3);
            EmitterParticle CreatedEmitter = ballhit != null ? new EmitterParticle(ballhit, DefaultDustEmitter) : new EmitterParticle(middlespot, DefaultDustEmitter);

            CreatedEmitter.VelocityDecay = new PointF(0.99f, 0.99f);
            CreatedEmitter.Velocity = BCBlockGameState.GetRandomVelocity(1, 3);

            return CreatedEmitter;

        }
        private Particle DefaultDustEmitter(BCBlockGameState gstate, EmitterParticle emitter, int framenum, int TTL)
        {
            return new DustParticle(emitter.Location, 3);



        }

        /// <summary>
        /// used to generate the "standard" spray of particles from a block.
        /// </summary>
        /// <param name="parentstate"></param>
        /// <param name="ballhit"></param>
        protected virtual void StandardSpray(BCBlockGameState parentstate, cBall ballhit)
        {



            for (int i = 0; i < (int)(14 * BCBlockGameState.ParticleGenerationFactor); i++)
            {

                //DustParticle createparticle = new DustParticle(middlespot);
                Random rrgen = BCBlockGameState.rgen;
                //DebrisParticle createparticle = (DebrisParticle)DebrisParticle.CreateDebris(ballhit);

                Particle createparticle = AddStandardSprayParticle(parentstate, ballhit);
                if (createparticle != null)
                {
                    createparticle.Velocity = new PointF((float)((rrgen.NextDouble() * 5) - 2.5),
                                                         (float)((rrgen.NextDouble() * 5) - 2.5));
                    if (ballhit != null)
                        createparticle.Velocity = new PointF(createparticle.Velocity.X + (ballhit.Velocity.X) / 5,
                                                             createparticle.Velocity.Y + (ballhit.Velocity.Y) / 5);
                    createparticle.Velocity =
                        new PointF(
                            createparticle.Velocity.X * 2 - (float)(rrgen.NextDouble() * createparticle.Velocity.X),
                            (float)
                            (createparticle.Velocity.Y * 2 - (rrgen.NextDouble() * createparticle.Velocity.Y)));
                    parentstate.Particles.Add(createparticle);
                }

            }
        }

        public virtual bool CheckImpact(BCBlockGameState currentgamestate, cBall hitball, out bool washit, ref List<cBall> ballsadded)
        {
            //return true to destroy this block.
            //basic Block->Ball collision detection. 
            //step one: check if the ball hit this block.

            washit = false;
            //if the ball isn't moving, return false.
            if (hitball.Velocity.X == 0 && hitball.Velocity.Y == 0)
            {
                //Debug.Print("Ball isn't moving returning false (CheckImpact)");

                return false;

            }

            //first see if it's within hitball.radius of the left and right...
            PointF gotoffset;
            if (DoesBallTouch(hitball))
            {
                //TODO: fix this, it's not working with blocks that aren't destroyed (the ball get's stuck inside it)
                //remove this block from the game.
                //TODO: change code to not redraw all blocks every frame. (first implementation will take after dodger and redraw
                //everything after every frame
                hitball.invokebeforeblockimpact(hitball);
                //((BCBlockGameState)(ParentGame.Target)).Soundman.PlaySound("BBOUNCE",1f);
                //direction should change here based on it's relative position...

                if (hitball.HitBlock(currentgamestate, this))
                {
                    BallRelativeConstants ballrelmode = getBallRelative(hitball);

                    if ((ballrelmode & BallRelativeConstants.Relative_Left) != 0)
                        hitball.Velocity = new PointF(-Math.Abs(hitball.Velocity.X), hitball.Velocity.Y);

                    if ((ballrelmode & BallRelativeConstants.Relative_Up) != 0)
                        hitball.Velocity = new PointF(hitball.Velocity.X, -Math.Abs(hitball.Velocity.Y));

                    if ((ballrelmode & BallRelativeConstants.Relative_Right) != 0)
                        hitball.Velocity = new PointF(Math.Abs(hitball.Velocity.X), hitball.Velocity.Y);

                    if ((ballrelmode & BallRelativeConstants.Relative_Down) != 0)
                        hitball.Velocity = new PointF(hitball.Velocity.X, Math.Abs(hitball.Velocity.Y));

                    Debug.Print("Velocity changed to " + hitball.Velocity + " rel:" + ballrelmode);

                    // var result = getBallRelative(hitball);
                    var result = ballrelmode;

                    if ((result & BallRelativeConstants.Relative_Left) == BallRelativeConstants.Relative_Left)
                    {
                        hitball.Location = new PointF(hitball.Location.X - hitball.Radius, hitball.Location.Y);

                    }
                    if ((result & BallRelativeConstants.Relative_Up) == BallRelativeConstants.Relative_Up)
                    {
                        hitball.Location = new PointF(hitball.Location.X, hitball.Location.Y - hitball.Radius);

                    }
                    if ((result & BallRelativeConstants.Relative_Down) == BallRelativeConstants.Relative_Down)
                    {
                        hitball.Location = new PointF(hitball.Location.X, hitball.Location.Y + hitball.Radius);

                    }
                    if ((result & BallRelativeConstants.Relative_Right) == BallRelativeConstants.Relative_Right)
                    {
                        hitball.Location = new PointF(hitball.Location.X + hitball.Radius, hitball.Location.Y);

                    }



                    //if (usedoffset != null)
                    //{
                    //    hitball.Location = new PointF(hitball.Location.X + gotoffset.X,
                    //                                hitball.Location.Y + gotoffset.Y);
                    //}
                }




                washit = PerformBlockHit(currentgamestate, hitball);

                bool nodef = false;
                if (washit)
                {

                    RaiseBlockDestroy(currentgamestate, hitball, ref nodef);


                }
                else if (this is NormalBlock)
                {
                    Debug.Print("WTF");

                }
                return true;

            }
            //washit=false;
            return false;
        }

        //probably add some "helper" methods....



        #region ICloneable Members

        public abstract object Clone();

        #endregion


        #region ISerializable Members
        public Block(XElement Source) 
        {
            BlockRectangle = StandardHelper.ReadElement<RectangleF>(Source.Element("BlockRectangle"));
            _AutoRespawn = bool.Parse(Source.Attribute("AutoRespawn").Value);
            _Destructable = bool.Parse(Source.Attribute("Destructable").Value);
            BlockEffects = StandardHelper.ReadList<IBlockEffect>(Source.Element("BlockEffects"));
            BlockTriggers = StandardHelper.ReadList<BlockTrigger>(Source.Element("BlockTriggers"));
            BlockEvents = StandardHelper.ReadList<BlockEvent>(Source.Element("BlockEvents"));
            ReadPowerupXML(Source.Element("Powerups"));

        }
        /// <summary>
        /// function which saves the PowerupChance information stored in Powerups and the Powerupchance information.
        /// </summary>
        /// <param name="pNodeName">Name to assign to the XElement node that will be created.</param>
        /// <returns>XElement Node representing the information for the Powerup spawning information of this block.</returns>
        protected XElement GetPowerupXML(String pNodeName)
        {
            //Powerupchance
            //Powerups 
            XElement BuildPowerup = new XElement(pNodeName);
            for(int i=0;i<Powerups.Length;i++)
            {
                if (Powerups[i] != null)
                {
                    XElement ChangeItem = new XElement("ChanceItem", new XAttribute("Type", Powerups[i].FullName), new XAttribute("Chance", Powerupchance[i]));
                    BuildPowerup.Add(ChangeItem);
                }
            }
            return BuildPowerup;
        }
        //This function appears to make loading blocks be incredibly slow.
        //most of the effort is expended using the FindClass function.
        protected void ReadPowerupXML(XElement Source)
        {
            //assumes given XElement is XML created by GetPowerupXML.
            //TODO: More exception handling and logging here...
            int numelements = Source.Elements("ChanceItem").Count();
            //initialize the two arrays.
            Powerups = new Type[numelements];
            Powerupchance = new float[numelements];
            int currelement = 0;
            foreach(var iterateElement in Source.Elements("ChanceItem"))
            {
                String TypeAttribute = iterateElement.Attribute("Type").Value;
                String ChanceAttribute = iterateElement.Attribute("Chance").Value;
                Powerups[currelement] = BCBlockGameState.FindClass(TypeAttribute);
                float parsedChance = 0;
                float.TryParse(ChanceAttribute, out parsedChance);
                Powerupchance[currelement] = parsedChance;
            }
            mPowerupChanceSum = Powerupchance.Sum((w) => w);
        }
        public virtual XElement GetXmlData(String pNodeName)
        {

            XElement Result = new XElement(pNodeName,null);
            Result.Add(StandardHelper.SaveElement(BlockRectangle, "BlockRectangle"));
            Result.Add(new XAttribute("AutoRespawn", _AutoRespawn));
            Result.Add(new XAttribute("Destructable", _Destructable));
            Result.Add(StandardHelper.SaveList(BlockEffects, "BlockEffects"));
            Result.Add(StandardHelper.SaveList(BlockTriggers, "BlockTriggers"));
            Result.Add(StandardHelper.SaveList(BlockEvents,"BlockEvents"));
            Result.Add(GetPowerupXML("Powerups"));
            return Result;

         
        }
        public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("BlockRectangle", this.BlockRectangle);
            info.AddValue("AutoRespawn", AutoRespawn);
            info.AddValue("Destructable", _Destructable);
            info.AddValue("BlockEffects", _BlockEffects);
            //info.AddValue("TriggerID", TriggerID);

            //serialize the type array. easy enough, simply store the array of strings of their fullname properties.

           // String[] TypeFullnames = new string[Powerups.Length];
            List<String> TypeFullnames = new List<string>();
            //for (int i = 0; i < TypeFullnames.Length; i++)
            //{
                
            //    TypeFullnames[i] = Powerups[i].FullName;


            //}
            foreach (var iterate in Powerups)
            {
                if (iterate != null)
                    TypeFullnames.Add(iterate.FullName);


            }
            //serialize the type array as well as the chance array...
            info.AddValue("PowerUpTypes", TypeFullnames.ToArray());
            info.AddValue("PowerUpChance", Powerupchance);


            //cast BlockTriggers and BlockEvents to the base type and save...

            //SerializeList("BlockTriggers", BlockTriggers, info);
            //SerializeList("BlockEvents", BlockEvents, info);
            info.AddValue("BlockTriggers", BlockTriggers, typeof(List<BlockTrigger>));
            info.AddValue("BlockEvents", BlockEvents, typeof(List<BlockEvent>));
            //info.AddValue("BlockTriggers", trigs.ToArray());
            //info.AddValue("BlockEvents", trigevent.ToArray());
        }

        private void SerializeList<T>(String listname, List<T> serializethis, SerializationInfo destination) where T : ISerializable
        {
            //ridiculous. This shouldn't be fucking necessary, but I can't for the fucking life of me figure out why in the
            //FUCKING GOD DAMNED HELL it decides to serialize and pretend everything is FUCKING ALLRIIGHT but
            //then eserialize a shitload of fucking god damned son of a fucking christ null values. Piece of SHIT.


            //save as ListItemx where X is the sequence number.
            //also save total count.
            destination.AddValue(listname + ".ItemCount", serializethis.Count);
            int i = 1;
            foreach (T itemloop in serializethis)
            {
                String buildname = listname + ".ListItem" + i.ToString();
                destination.AddValue(buildname, itemloop);


                i++;
            }






        }
        private List<T> DeserializeList<T>(String Listname, SerializationInfo source) where T : ISerializable
        {
            List<T> buildlist = new List<T>();
            int Itemcount = source.GetInt32(Listname + ".ItemCount");
            for (int i = 1; i < Itemcount; i++)
            {
                String buildname = Listname + ".ListItem" + i.ToString();
                buildlist.Add((T)source.GetValue(buildname, typeof(T)));


            }
            return buildlist;

        }

        #endregion

        #region IDeserializationCallback Members

        public virtual void OnDeserialization(object sender)
        {
            //give all triggers and events this block as their owner...
            foreach (BlockTrigger looptrigger in BlockTriggers)
            {
                Debug.Print("giving self to trigger...");
                if (looptrigger != null)
                {
                    looptrigger.OwnerBlock = this;
                }
            }

            foreach (BlockEvent loopevent in BlockEvents)
            {
                if (loopevent != null)
                    loopevent.OwnerBlock = this;

            }
        }

        #endregion

        #region iImagable Members

        void iImagable.Draw(Graphics g)
        {
            Draw(g);
        }

        Size iImagable.Size
        {
            get
            {
                return new Size((int)BlockSize.Width, (int)BlockSize.Height);
            }
            set
            {
                BlockSize = new SizeF(value.Width, value.Height);
            }
        }

        Point iImagable.Location
        {
            get
            {
                return new Point((int)Location.X, (int)Location.Y);
            }
            set
            {
                BlockLocation = new Point((int)value.X, (int)value.Y);
            }
        }

        Rectangle iImagable.getRectangle()
        {
            return BlockRectangle_int;
        }

        #endregion

      
    }


    /*
    [Serializable]
    [ImpactEffectBlockCategory]
    [BlockDescription("Secret and arguably useless block used in early builds. Why are you pressing shift anyway?")]
    public class CreditBlock : ImageBlock
    {
        public String[] ItemsDisplay { get; set; }
        public int curritemindex { get; set; }
        public GameObject lastobjadded = null;
        public PointF TextEmitVelocity = new PointF(0, -0.75f);
        public PointF? TextEmitPosition = new PointF();
        public bool neverdestroy = false;
        public bool RemovePrevious = false;
        public CreditBlock(RectangleF blockrect)
            : this(blockrect, new String[] { "Example", "credits" }, BCBlockGameState.CenterPoint(blockrect))
        {

        }

        public CreditBlock(RectangleF blockrect, String[] DisplayItems, PointF ptextEmitPosition)
            : base(blockrect, "CREDITS")
        {
            ItemsDisplay = DisplayItems;
            TextEmitPosition = ptextEmitPosition;
        }
        public CreditBlock(CreditBlock CloneThis)
            : base(CloneThis.BlockRectangle, CloneThis.BlockImageKey)
        {

            ItemsDisplay = CloneThis.ItemsDisplay;
            curritemindex = CloneThis.curritemindex;
            TextEmitPosition = CloneThis.TextEmitPosition;
            TextEmitVelocity = CloneThis.TextEmitVelocity;
            neverdestroy = CloneThis.neverdestroy;
            RemovePrevious = CloneThis.RemovePrevious;
        }
        public CreditBlock(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            ItemsDisplay = (String[])(info.GetValue("ItemsDisplay", typeof(String[])));
            TextEmitPosition = (PointF)info.GetValue("TextEmitPosition", typeof(PointF));
            TextEmitVelocity = (PointF)info.GetValue("TextEmitVelocity", typeof(PointF));
            neverdestroy = info.GetBoolean("neverdestroy");
            RemovePrevious = info.GetBoolean("RemovePrevious");

        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("ItemsDisplay", ItemsDisplay, typeof(String[]));
            info.AddValue("TextEmitPosition", TextEmitPosition);
            info.AddValue("TextEmitVelocity", TextEmitVelocity);
            info.AddValue("neverdestroy", neverdestroy);
            info.AddValue("RemovePrevious", RemovePrevious);
        }
        public override object Clone()
        {
            return new CreditBlock(this);
        }
        public override bool MustDestroy()
        {
            return true;
        }
        public override bool PerformBlockHit(BCBlockGameState parentstate, cBall ballhit, ref List<cBall> ballsadded)
        {
            if (curritemindex > ItemsDisplay.Length && !neverdestroy) return true;
            if (curritemindex > ItemsDisplay.Length) curritemindex = 0;
            GameObject gotobject;
            if (TextEmitPosition == null)
            {
                gotobject = PopupText(parentstate, ItemsDisplay[curritemindex], new Font("Arial", 24),
                                                 new SolidBrush(Color.White), new Pen(Color.Black), 7500,
                                                 TextEmitVelocity);
            }
            else
            {
                gotobject = PopupText(parentstate, ItemsDisplay[curritemindex], new Font("Arial", 24),
                                                new SolidBrush(Color.White), new Pen(Color.Black), 7500,
                                                TextEmitVelocity, TextEmitPosition.Value);

            }
            BasicFadingText bft = ((BasicFadingText)gotobject);
            bft.myspeedfalloff = 1.001f;
            //bft.YSpeedDelegate = (t) => (-(1 + (float)Math.Abs(Math.Sin(t.numticks))));
            curritemindex++;
            if (RemovePrevious)
            {
                if (lastobjadded != null)
                {
                    if (parentstate.GameObjects.Contains(lastobjadded))
                    {

                        parentstate.GameObjects.Remove(lastobjadded);

                    }
                }
            }
            //return base.PerformBlockHit(parentstate, ballhit, ref ballsadded);
            lastobjadded = gotobject;
            // return curritemindex >= ItemsDisplay.Length && !neverdestroy ;
            return false;
        }



    }
    */


    //block that speeds up the ball that hits it


    [Browsable(false)]
    public class ProxyBallBehaviour : iBallBehaviour
    {
        public string Tag { get; set; }
        public delegate bool HitBlockFunction(BCBlockGameState currentstate, cBall ballobject, Block blockhit);
        public delegate List<Block> PerformFrameFunction(cBall ballobject, BCBlockGameState ParentGameState, ref List<cBall> ballsadded, ref List<cBall> ballsremove, out bool removethis);
        public delegate HitWallReturnConstants HitWallFunction(BCBlockGameState currentstate, cBall ballobject);
        public delegate void BehaviourAddedFunction(cBall onBall, BCBlockGameState currstate);
        public delegate void BehaviourRemovedFunction(cBall onBall, BCBlockGameState currstate);
        public delegate void DrawFunction(cBall balldraw, Graphics g);
        public delegate string PaddleBallSoundFunction(cBall ball);
        public delegate string BlockBallSoundFunction(cBall ball);
        

        public HitBlockFunction HitBlockRoutine = null;
        public PerformFrameFunction PerformFrameRoutine = null;
        public HitWallFunction HitWallRoutine = null;
        public BehaviourAddedFunction BehaviourAddedRoutine = null;
        public BehaviourRemovedFunction BehaviourRemovedRoutine = null;
        public DrawFunction DrawRoutine = null;
        public PaddleBallSoundFunction PaddleBallSoundRoutine = null;
        public BlockBallSoundFunction BlockBallSoundRoutine = null;


        public cBall ownerball = null;

        #region constructors
        public ProxyBallBehaviour(String pTag, HitBlockFunction HitBlockFunc, PerformFrameFunction PerformFrameFunc, HitWallFunction HitWallFunc,
            BehaviourAddedFunction BehaviourAddedFunc, BehaviourRemovedFunction BehaviourRemovedFunc, DrawFunction DrawFunc, PaddleBallSoundFunction PaddleBallSoundFunc,
            BlockBallSoundFunction BlockBallSoundFunc)
        {
            Tag = pTag;
            HitBlockRoutine = HitBlockFunc;
            PerformFrameRoutine = PerformFrameFunc;
            HitWallRoutine = HitWallFunc;
            BehaviourAddedRoutine = BehaviourAddedFunc;
            BehaviourRemovedRoutine = BehaviourRemovedFunc;
            DrawRoutine = DrawFunc;
            PaddleBallSoundRoutine = PaddleBallSoundFunc;
            BlockBallSoundRoutine = BlockBallSoundFunc;

        }


        #endregion


        #region iBallBehaviour Members

        public bool HitBlock(BCBlockGameState currentstate, cBall ballobject, Block blockhit)
        {
            if (HitBlockRoutine != null) return HitBlockRoutine(currentstate, ballobject, blockhit); else return false;
        }

        public List<Block> PerformFrame(cBall ballobject, BCBlockGameState ParentGameState, ref List<cBall> ballsadded, ref List<cBall> ballsremove, out bool removethis)
        {
            ownerball = ballobject;
            if (PerformFrameRoutine != null)
                return PerformFrameRoutine(ballobject, ParentGameState, ref ballsadded, ref ballsremove, out removethis);
            else
            {
                removethis = false;
                return new List<Block>();
            }
        }

        public HitWallReturnConstants HitWall(BCBlockGameState currentstate, cBall ballobject)
        {
            if (HitWallRoutine != null) return HitWallRoutine(currentstate, ballobject); else return HitWallReturnConstants.HitBall_Default;
        }

        public void BehaviourAdded(cBall onBall, BCBlockGameState currstate)
        {
            ownerball = onBall;
            if (BehaviourAddedRoutine != null) BehaviourAddedRoutine(onBall, currstate);
        }

        public void BehaviourRemoved(cBall onBall, BCBlockGameState currstate)
        {
            if (BehaviourRemovedRoutine != null) BehaviourRemovedRoutine(onBall, currstate);
        }

        public void Draw(Graphics g)
        {
            if (DrawRoutine != null) DrawRoutine(ownerball, g);
        }

        public string PaddleBallSound()
        {
            if (PaddleBallSoundRoutine != null) return PaddleBallSoundRoutine(ownerball); else return "";
        }

        public string BlockBallSound()
        {
            if (BlockBallSoundRoutine != null) return BlockBallSoundRoutine(ownerball); else return "";
        }

        #endregion

        #region ISerializable Members
        public ProxyBallBehaviour(XElement Source)
        {

        }
        public XElement GetXmlData(String pNodeName)
        {
            return new XElement(pNodeName);
        }
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            //nothing, so far.
            //throw new NotImplementedException();
        }

        #endregion
    }


    //sort of like the gravity ball behaviour, but more or less a "proxy" ball behaviour.







    //new block ideas:
    //Launcher block:
    //a "basic" block that is sort of like the rayblock, but activates differently, and fires from a event rather than from a block impact.

    //TetherBlock
    //This one will be tricky. The idea is that, when a ball is passing within a certain radius and it's direction vector is perpendicular to the line between the center of
    //the tetherblock and the ball, the tether block will "attach" the ball to a "tether" which will end up swinging the ball around exactly 180 degrees around  the tether block.


    //BCBlockGameState parentstate = ((BCBlockGameState)ParentGame.Target);
    //        parentstate.Soundman.PlaySound("BOMB", 1.0f);
}
