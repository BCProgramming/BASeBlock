using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.Serialization;
using BASeBlock.Particles;

namespace BASeBlock.Blocks
{
    [Serializable]
    [ImpactEffectBlockCategory]
    
    public class CardinalPopperBlock : ImageBlock,IDamageableBlock
    {
        

        public int Damage { get; set; }
        public int Health { get; set; }

        public CardinalPopperBlock(RectangleF blockrect)
            : base(blockrect, "cardinalpopper")
        {
            Health = 5;
        }

        public CardinalPopperBlock(CardinalPopperBlock clonethis):base(clonethis)
        {
        
            this.Damage = clonethis.Damage;
            this.Health = clonethis.Health;

        }
        public CardinalPopperBlock(SerializationInfo info,StreamingContext context):base(info,context)
        {
            Damage = info.GetInt32("Damage");
            Health = info.GetInt32("Health");
        }
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("Damage",Damage);
            info.AddValue("Health", Health);

        }

        public override object Clone()
        {
            return new CardinalPopperBlock(this); 	    
        }
        public override void Draw(Graphics g)
        {
            base.Draw(g);
            DrawDamage(g, Damage, Health);
        }
        protected virtual bool RecursiveCall(BCBlockGameState gstate,IEnumerable<CardinalPopperBlock> exclude)
        {

            //look at the blocks in each cardinal direction.
            Damage++;
            hasChanged = true;
            PointF[] LocationsCheck = new PointF[]
            {
                new PointF(BlockRectangle.Left-BlockRectangle.Width/2,BlockRectangle.Y + BlockRectangle.Height/2), //left
                new PointF(BlockRectangle.Right+BlockRectangle.Width/2,BlockRectangle.Y+BlockRectangle.Height/2), //right
                new PointF(BlockRectangle.Left+BlockRectangle.Width/2,BlockRectangle.Y-BlockRectangle.Height/2), //top
                new PointF(BlockRectangle.Left+BlockRectangle.Width/2,BlockRectangle.Bottom+BlockRectangle.Height/2) //bottom
            };

            foreach (var lookat in LocationsCheck)
            {
                //are there blocks at this pos?
                List<Block> results = BCBlockGameState.Block_HitTest(gstate.Blocks, lookat);
                if (results != null && results.Count > 0)
                {
                    //hit each one...
                    foreach (Block iterate in results)
                    {
                        //if it's a CardinalPopperBlock, call it's RecursiveCall routine, but do it with a delay, for funky effect.
                        if (iterate is CardinalPopperBlock && !exclude.Contains(iterate))
                        {
                            
                            var closed = iterate;
                            gstate.DelayInvoke(new TimeSpan(0,0,0,0,200),(ob)=>{
                                closed.StandardSpray(gstate);
                                BCBlockGameState.Soundman.PlaySound("fworkblast");
                                
                                if ((closed as CardinalPopperBlock).RecursiveCall(gstate, exclude.Concat(new[] { this })))
                                {
                                    gstate.Blocks.Remove(closed);
                                }
                                gstate.Forcerefresh = true;
                                gstate.ClientObject.UpdateBlocks();
                                
                            });
                            //and, remove the block.

                        }
                        else if (!(iterate is CardinalPopperBlock))
                        {
                            //otherwise, "hit" it.
                            BCBlockGameState.Block_Hit(gstate, iterate);
                            gstate.Forcerefresh = true;
                        }

                    }



                }


            }
            bool returnresult = Damage > Health;
            //Create the PolyDebris!
            PolyDebris pd = new PolyDebris(CenterPoint(), 0, Color.Blue,4,8,4,6);

            pd.expansionfactor = 1.01;
            pd.TTL /= 3;
            pd.RotationSpeed = Math.PI / 14;
            if (returnresult)
            {
                //deadly
                pd.BrushColor = Color.Red;

            }
            else
            {
                pd.BrushColor = Color.Blue;
                //not deadly.
            }
            gstate.Defer(() => gstate.Particles.Add(pd));
            return returnresult;


        }

        public override bool PerformBlockHit(BCBlockGameState parentstate, cBall ballhit)
        {
            Damage++;
            hasChanged = true;
            parentstate.Forcerefresh = true;
            //task:
            RecursiveCall(parentstate, new CardinalPopperBlock[]{this});
            return Damage > Health || base.PerformBlockHit(parentstate, ballhit);

            
        }
        
    }
}
