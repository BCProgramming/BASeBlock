using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Linq;
using System.Runtime.Serialization;
using System.Xml.Linq;

namespace BASeCamp.BASeBlock.Blocks
{
    [Serializable]
    [ImpactEffectBlockCategory]
    [BlockDescription("Replaces one type of block in the Level with another when broken.")]

    public class ReplacerBlock : Block
    {

        protected Type SearchBlockType = typeof(InvincibleBlock);
        protected Type ReplaceWithBlockType = typeof(StrongBlock);


        [Editor(typeof(BlockTypeStringEditor), typeof(UITypeEditor))]
        public String SearchFor { get { return SearchBlockType.Name; } set { SearchBlockType = retrieveType(value)??SearchBlockType; } }

        [Editor(typeof(BlockTypeStringEditor), typeof(UITypeEditor))]
        public String ReplaceWith { get { return ReplaceWithBlockType.Name; } set { ReplaceWithBlockType = retrieveType(value)??ReplaceWithBlockType; } }


        public ReplacerBlock(RectangleF blockrect)
        {
            BlockRectangle = blockrect;

        }
        public ReplacerBlock(ReplacerBlock clonethis)
            : base(clonethis)
        {
            ReplaceWithBlockType = clonethis.ReplaceWithBlockType;
            SearchBlockType = clonethis.SearchBlockType;


        }

        public ReplacerBlock(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            SearchFor = info.GetString("SearchFor");
            ReplaceWith = info.GetString("ReplaceWith");


        }
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("SearchFor", SearchBlockType.Name);
            info.AddValue("ReplaceWith", ReplaceWithBlockType.Name);
        }

        public override XElement GetXmlData(string pNodeName)
        {
            var Result = base.GetXmlData(pNodeName);
            Result.Add(new XAttribute("SearchFor",SearchBlockType.Name));
            Result.Add(new XAttribute("ReplaceWith",ReplaceWithBlockType.Name));
            return Result;
        }

        public override object Clone()
        {
            return new ReplacerBlock(this);
        }
        private Type retrieveType(String findit)
        {
            return BCBlockGameState.FindClass(findit);

        }

        public bool proxyfunction(ProxyObject sourceobject, BCBlockGameState gstate)
        {
            List<Block> removethese = new List<Block>();
            List<Block> addthese = new List<Block>();



            foreach (var loopit in from q in gstate.Blocks where q.GetType() == SearchBlockType select q)
            {
                removethese.Add(loopit);
                addthese.Add((Block)Activator.CreateInstance(ReplaceWithBlockType, new Object[] { loopit.BlockRectangle }));



            }


            foreach (var removeit in removethese)
            {
                gstate.Blocks.Remove(removeit);


            }
            foreach (var addit in addthese)
            {
                gstate.Blocks.AddLast(addit);

            }
            gstate.Forcerefresh = true;
            return true;




        }
        //serialization...
        public override void Draw(Graphics g)
        {
            //base.Draw(g);
            //draw routine: draw a small imager of the two blocks with a arrow from the old to the new.

            //first draw the background. screw it, just go with blue for now.
            g.FillRectangle(new SolidBrush(Color.Blue), BlockRectangle);
            g.DrawRectangle(new Pen(Color.Black), new Rectangle((int)BlockRectangle.Left, (int)BlockRectangle.Top, (int)BlockRectangle.Width, (int)BlockRectangle.Height));

            //first block rectangle:
            //top: ourtop+ourheight/4
            //left: ourleft+ourwidth/4

            //width ours/5
            //height ours/5


            //second block
            //top ourtop+ourheight/4
            //left ourright-ourwidth/3
            var br = BlockRectangle;
            RectangleF firstblockrect = new RectangleF(br.Left + br.Width / 4, br.Top + br.Height / 4,
                                                       br.Width / 5, br.Height / 5);

            RectangleF secondblockrect = new RectangleF(br.Right - br.Width / 3, br.Top + br.Height / 4, br.Width / 5, br.Height / 5);



            //get the appropriate images.
            Image firstblockimage = BCBlockGameState.BlockDataMan[SearchBlockType].Value.useBlockImage;
            Image secondblockimage = BCBlockGameState.BlockDataMan[ReplaceWithBlockType].Value.useBlockImage;


            //and paint them.
            g.DrawImage(firstblockimage, firstblockrect.Left, firstblockrect.Top, firstblockrect.Width, firstblockrect.Height);
            g.DrawImage(secondblockimage, secondblockrect.Left, secondblockrect.Top, secondblockrect.Width, secondblockrect.Height);


            //arrow sold separately.





        }
        public override bool PerformBlockHit(BCBlockGameState parentstate, cBall ballhit)
        {
            parentstate.Defer(()=>
                              parentstate.GameObjects.AddLast(new ProxyObject(proxyfunction, null)));

            return base.PerformBlockHit(parentstate, ballhit);
        }

    }
}