using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.Serialization;
using System.Xml.Linq;
using BASeCamp.XMLSerialization;

namespace BASeCamp.BASeBlock.Blocks
{
    /// <summary>
    /// has a given colour. When activated, will see if there is another activated
    /// AttractionRepulsionBlock. If there is and they are the same colour, it will repel from it. If they
    /// are not the same colour, they will attract to one another, exploding when they meet. both states destroy all objects in their path.
    /// The repel behaviour could be dealth with using a BoxDestructor.
    /// The Attraction Behaviour will likely be a subclass.
    /// </summary>
    [Serializable]
    [ImpactEffectBlockCategory]
    public class AttractionRepulsionBlock : ImageBlock
    {
        private Color _BlockColor = Color.Red;
        public Color BlockColor { get { return _BlockColor; } set { _BlockColor = value; }
        }

        public bool Activated { get; set; }

        public static Dictionary<Level,List<AttractionRepulsionBlock>> AttractionQueue= new Dictionary<Level, List<AttractionRepulsionBlock>>(); 

        private List<AttractionRepulsionBlock> GetListForLevel(Level levelobject)
        {
            if(!AttractionQueue.ContainsKey(levelobject)) 
                AttractionQueue.Add(levelobject,new List<AttractionRepulsionBlock>());
                
            return AttractionQueue[levelobject];



        }

        public AttractionRepulsionBlock(RectangleF BlockRect):base(BlockRect,"attractor_inactive")
        {
            BlockRectangle=BlockRect;

        }
        public AttractionRepulsionBlock(RectangleF BlockRect,Color color):this(BlockRect)
        {
            BlockColor=color;

        }
        public AttractionRepulsionBlock(AttractionRepulsionBlock clonethis):base(clonethis)
        {
            BlockColor=clonethis.BlockColor;

        }
        public AttractionRepulsionBlock(SerializationInfo info,StreamingContext context):base(info,context)
        {
            BlockColor = (Color)info.GetValue("BlockColor",typeof(Color));


        }
        public AttractionRepulsionBlock(XElement source):base(source)
        {
            BlockColor = source.ReadElement<Color>("BlockColor");
        }
        public override void  GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("BlockColor",BlockColor);
        }

        public override XElement GetXmlData(string pNodeName)
        {
            XElement result = base.GetXmlData(pNodeName);
            result.Add(StandardHelper.SaveElement(this.BlockColor,"BlockColor"));
            return result;
        }

        public override object  Clone()
        {
            return new AttractionRepulsionBlock(this);
        }
        /// <summary>
        /// activates this block
        /// </summary>
        /// <param name="gstate"></param>
        private void Activate(BCBlockGameState gstate)
        {
            base.BlockImageKey = "attractor_active";
            
            BCBlockGameState.Soundman.PlaySound("charge");
            //activation...


            var gotlist = GetListForLevel(gstate.PlayingLevel);
            if(!gotlist.Contains(this)) gotlist.Add(this);
            Activated=true;
        }
        private void DeActivate(BCBlockGameState gstate)
        {
            //Deactivates this block.
            base.BlockImageKey = "attractor_inactive";
            BCBlockGameState.Soundman.PlaySound("uncharge");
            var gotlist = GetListForLevel(gstate.PlayingLevel);
            if(gotlist.Contains(this)) gotlist.Remove(this);
            Activated=false;
        }
        public override void  Draw(Graphics g)
        {
            // Color usecolor = BlockColor;
            // DrawAttributes = new ImageAttributes();
            // DrawAttributes.SetColorMatrix(ColorMatrices.GetColourizer(BlockColor));

            g.FillRectangle(new SolidBrush(BlockColor), BlockRectangle);

            base.Draw(g);
        }
        public override bool  PerformBlockHit(BCBlockGameState parentstate, cBall ballhit)
        {


            if (!parentstate.Blocks.Any((w) => w is AttractionRepulsionBlock))
            {
                ExplosionEffect ee = new ExplosionEffect(CenterPoint(), 64);
                ee.DestroyAll = false;
                parentstate.NextFrameCalls.Enqueue(
                    new BCBlockGameState.NextFrameStartup(() => parentstate.GameObjects.AddLast(ee)));
                BCBlockGameState.Soundman.PlaySound("WRONG");
                return true;
            }

            //if this block is activated, deactivate it.
            //if it's deactivated, activate it.
            if(Activated)
                DeActivate(parentstate);
            else
            {
                Activate(parentstate);
            }

            var  gotlisting = GetListForLevel(parentstate.PlayingLevel);

            //also, check the listing to see if two blocks exist in the activated list
            //if so, create a AttractionRepulsion Object to manage two BoxDestructors to provide that functionality.
            //And if so, we also check to see if we are the last Attraction/Repulsion Blocks in the current gamestate. If so, we can safely clear 
            //ALL the entries in our dictionary (if we never do who knows what could happen).

            if(gotlisting.Count == 2)
            {
                Debug.Assert(gotlisting[0] != gotlisting[1]);
                AttractRepulseDestructor createdestructor = new AttractRepulseDestructor(parentstate, gotlisting[0],gotlisting[1]);
                parentstate.Defer(() =>
                                  parentstate.GameObjects.AddLast(createdestructor));
                List<Block> converted = new List<Block>();
                //downcast to Block from AttractionRepulsionBlock...
                foreach(AttractionRepulsionBlock iterate in gotlisting)
                    converted.Add(iterate);
                //BlockRemoverProxy br = new BlockRemoverProxy(parentstate,converted);
                parentstate.Defer(() =>
                                      {
                                          foreach (var removeit in converted)
                                          {
                                              parentstate.Blocks.Remove(removeit);
                                          }

                                      });
                gotlisting.Clear();
                //if we have 2 or fewer AttractionRepulsionBlocks in the listing, than there will be no need for the cache value.
                if(parentstate.Blocks.Count((w)=>w.GetType()==typeof(AttractionRepulsionBlock)) <=2)
                    AttractionQueue = new Dictionary<Level, List<AttractionRepulsionBlock>>();


                parentstate.Forcerefresh = true;
                return true;
            }

            return false;
 	         
            
        }

    }
}