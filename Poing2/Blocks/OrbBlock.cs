using System;
using System.Drawing;
using System.Runtime.Serialization;
using BASeCamp.BASeBlock.Events;
using BASeCamp.BASeBlock.GameObjects.Orbs;

namespace BASeCamp.BASeBlock.Blocks
{
    [Serializable()]
    [PowerupEffectCategory]
    [BlockDescription("Block that spawns a crapton of Orbs when hit.")]
    public class OrbBlock : Block,IGameInitializer 
    {
        //the OrbBlock releases 50 orbs when destroyed, rather than the default.

        //create the static images.
        private static Image DrawOrbBlockImage()
        {
            Type[] Collectibletypes = BCBlockGameState.MTypeManager[typeof(CollectibleOrb)].ManagedTypes.ToArray();
            Size usesize = new Size(256,128);
            //create the bitmap and get a graphics context...
            Bitmap buildbitmap = new Bitmap(usesize.Width,usesize.Height);
            using (Graphics useg = Graphics.FromImage(buildbitmap))
            {
                //first, paint out the "default" appearance, which will be generic_4
                useg.DrawImage(BCBlockGameState.Imageman.getLoadedImage("GENERIC_4"), 0, 0, usesize.Width, usesize.Height);

                const int NumDrawOrbs = 50;
                Random rg = BCBlockGameState.rgen;
                //paint 50 Orbs randomly across the block.
                for (int i = 0; i < 50; i++)
                {
                    //choose a random location.
                    PointF drawOrbPosition = new PointF((float)(rg.NextDouble() * usesize.Width), (float)(rg.NextDouble() * usesize.Height));
                    float userandomsize = (float)((usesize.Width/16) + rg.NextDouble() * (usesize.Width/8));
                    SizeF userndsize = new SizeF(userandomsize, userandomsize);

                    //now the fun part.
                    //choose a random CollectibleOrb type, instantiate it, and instruct it to draw on our graphics context.
                    Type InstantiateType = null;
                    //because some CollectibleOrb might not implement the constructor, we loop until we have one that is valid.
                    while (InstantiateType == null)
                    {
                        InstantiateType = BCBlockGameState.Choose(Collectibletypes);
                        //attempt to instantiate. Catch and ignore errors.
                        try
                        {
                            CollectibleOrb co = (CollectibleOrb)Activator.CreateInstance(InstantiateType, drawOrbPosition, userndsize);
                            //no exception. ALL IS WELL.
                            //force it to draw against it's will to the graphics context.
                            co.Draw(useg);


                        }
                        catch (Exception eex)
                        {
                            InstantiateType = null; //set to null to force another iteration.
                        }


                    }



                }

                //now, over top of that, draw bevel:
                useg.DrawImage(BCBlockGameState.Imageman.getLoadedImage("BEVEL"), 0, 0, usesize.Width, usesize.Height);
                

            }
            return buildbitmap;
            

        }
        const int PrebuiltOrbImageCount = 7;
        protected static Image[] OrbImages;
        private int _OrbSpawnCount = 50;

        public int OrbSpawnCount { get { return _OrbSpawnCount; } set { _OrbSpawnCount = value; } }

        public static void GameInitialize(iManagerCallback datahook)
        {
            OrbImages = new Image[PrebuiltOrbImageCount];
            for(int i=0;i<PrebuiltOrbImageCount;i++)
            {
                OrbImages[i] = DrawOrbBlockImage();


            }

        }
        public OrbBlock(RectangleF pblockrect)
        {
            BlockRectangle = pblockrect;
            HookEvent();

        }
        public OrbBlock(SerializationInfo info, StreamingContext context):base(info,context)
        {
            //no special code here.
            HookEvent();
        }
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
        }
        public OrbBlock(OrbBlock copythis)
            : base(copythis)
        {
            HookEvent();
        }
        public override object Clone()
        {
            return new OrbBlock(this);
        }

        private void HookEvent()
        {
            this.OnBlockDestroy += OrbBlock_OnBlockDestroy;
        }
        private Image ChosenImage = null;
        public override void Draw(Graphics g)
        {
            if (ChosenImage == null) ChosenImage = BCBlockGameState.Choose(OrbImages);
            g.DrawImage(ChosenImage, BlockRectangle);
        }
        void OrbBlock_OnBlockDestroy(Object Sender,BlockHitEventArgs<bool> e )
        {
            //SHOWER our subjects with GRORIOUS ORBS.
            Type[] ShowerOrbs = BCBlockGameState.MTypeManager[typeof(CollectibleOrb)].ManagedTypes.ToArray();

            for (int i = 0; i < OrbSpawnCount; i++)
            {
                Type SelectType = null;
                CollectibleOrb co = null;
                while (co == null)
                {
                    try
                    {
                        SelectType = BCBlockGameState.Choose(ShowerOrbs);
                        co = (CollectibleOrb)Activator.CreateInstance(SelectType, CenterPoint());
                        co.Velocity = BCBlockGameState.GetRandomVelocity(0,5);
                        e.GameState.GameObjects.AddLast(co);
                    }
                    catch
                    {
                       
                        co=null;
                    }



                }




            }



            e.Result = true;
        }


    }
}