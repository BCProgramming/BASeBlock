using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using BASeBlock.Blocks;

namespace BASeBlock
{
    /// <summary>
    /// DebugHelper: draws extra information about enemies and objects.
    /// </summary>
    class DebugHelper : GameObject 
    {
    
        private BCBlockGameState cachestate = null;
        private readonly Brush BlockBrush = new SolidBrush(Color.FromArgb(15, Color.Green));
        private readonly Pen BlockPen = new Pen(Color.Gray, 1);
        private readonly Font BlockInfoFont = new Font(BCBlockGameState.GetMonospaceFont(), 14);
        private readonly Brush BlackBrush = new SolidBrush(Color.Black);

        private readonly Font GameObjectInfoFont = new Font(BCBlockGameState.GetMonospaceFont(), 10);

        private void Initialize()
        {
            BCBlockGameState.Soundman.Driver.OnSoundPlay += new OnSoundPlayDelegate(Driver_OnSoundPlay);
            BCBlockGameState.Soundman.Driver.OnSoundStop += new OnSoundStopDelegate(Driver_OnSoundStop);


        }
        private List<iActiveSoundObject> PlayingSounds = new List<iActiveSoundObject>();
        void Driver_OnSoundStop(iActiveSoundObject objstop)
        {
            if(PlayingSounds.Contains(objstop))
                PlayingSounds.Remove(objstop);
        }

        void Driver_OnSoundPlay(iActiveSoundObject objplay)
        {
            PlayingSounds.Add(objplay);
        }

        String getPlayingSoundsDesc()
        {

            String result = PlayingSounds.Count.ToString() + " Playing.\n";
            
            foreach (var iterate in PlayingSounds)
            {
                String soundfound = "";    
                foreach (String searchkey in BCBlockGameState.Soundman.SoundSources.Keys)
                {

                    if (BCBlockGameState.Soundman.SoundSources[searchkey] == iterate)
                    {
                        soundfound = searchkey;
                        break;
                        
                    }


                }
                if (soundfound != "") result += soundfound + "\n";


            }
            result += " \nMusic:" + BCBlockGameState.Soundman.scurrentPlayingMusic;
            return result;
        }

        public override bool PerformFrame(BCBlockGameState gamestate)
        {
            if (cachestate == null)
            {
                cachestate = gamestate;
                Initialize();
            }
          
            cachestate = gamestate;
            return false;
        }

        private static void DrawRectangle(Graphics g, String prefixtext, RectangleF drawthis, Pen RectPen, Brush RectBrush,
            Brush TextBrush, Font usefont)
        {

            g.FillRectangle(RectBrush, drawthis);

            g.DrawRectangle(RectPen, drawthis.ToRectangle());


            //also paint coordinate information.
            //cachestate.Blocks.Count.ToString() + " Blocks;\n"
            String BlockInfotext = prefixtext +
                "(" + Math.Round(drawthis.Left, 1) + "," + Math.Round(drawthis.Top, 1) + ")-(" +
                "(" + Math.Round(drawthis.Right, 1) + "," + Math.Round(drawthis.Bottom, 1) + ")";


            Size measurestring = TextRenderer.MeasureText(g, BlockInfotext, usefont);

            Point getcenter = drawthis.CenterPoint().ToPoint();
            Point DrawHere = new Point(getcenter.X - measurestring.Width / 2, getcenter.Y - measurestring.Height / 2);
            g.DrawString(BlockInfotext, usefont, TextBrush, DrawHere.X, DrawHere.Y);


        }
       
        private static int LastCount = 0; //caches the count of objects made in the last refresh.
        public override void Draw(Graphics g)
        {
            if (cachestate == null) return;
            //create a rectangle around all the blocks.
            //Debug.Print("DebugHelper::Draw");
            int currentindex = 0;
            RectangleF? BlockRect = null;
            RectangleF? GameObjectRect = null;
            foreach (Block iterate in cachestate.Blocks)
            {
                
                if (BlockRect == null)
                    BlockRect = iterate.BlockRectangle;
                else
                    BlockRect = BlockRect.Value.Union(iterate.BlockRectangle);
                g.DrawRectangle(new Pen(Color.Green,1), iterate.BlockRectangle.ToRectangle());
                var cp = iterate.BlockRectangle.CenterPoint().ToPoint();
                /*
                String blocknumber = currentindex.ToString();
                var numsize = TextRenderer.MeasureText(g, blocknumber, BlockInfoFont);
                TextRenderer.DrawText(g, blocknumber, BlockInfoFont, new Point(cp.X - numsize.Width / 2, cp.Y - numsize.Height / 2), Color.Black);
                */
                currentindex++;
            }

            foreach (GameObject iterate in cachestate.GameObjects)
            {
                
                if (iterate is ProxyObject || iterate is DebugHelper) continue;
                if (iterate is SizeableGameObject)
                {
                    SizeableGameObject iterated = ((SizeableGameObject)iterate);
                    if (GameObjectRect == null)
                        GameObjectRect = iterated.getRectangle();
                    else
                        GameObjectRect = GameObjectRect.Value.Union(iterated.getRectangle());

                }
                else if(iterate is iLocatable)
                {
                    iLocatable casted = (iLocatable)iterate;
                    RectangleF urect = new RectangleF(casted.Location.X, casted.Location.Y, 1, 1);
                    if (GameObjectRect == null)
                        GameObjectRect = urect;
                    else
                        GameObjectRect = GameObjectRect.Value.Union(urect);

                }




                if (iterate is iLocatable)
                {
                    PointF useposition = ((iLocatable)iterate).Location;
                    String usedrawstring = iterate.GetType().Name + "\n" + ((iLocatable)iterate).Location.Each((w)=>(float)Math.Round(w,1)).ToString();


                    if (iterate is IMovingObject)
                    {
                        usedrawstring += "\n" + ((IMovingObject)iterate).Velocity.Each((w) => (float)Math.Round(w, 1)).ToString();

                    }

                    g.DrawString(usedrawstring, GameObjectInfoFont, BlackBrush, useposition);




                }


            }


            

            //Draw an outline.
            //cachestate.Blocks.Count.ToString() + " Blocks;\n"
            if(BlockRect!=null)
                DrawRectangle(g, cachestate.Blocks.Count.ToString() + " Blocks;\n", BlockRect.Value, BlockPen, BlockBrush, BlackBrush, BlockInfoFont);

            if(GameObjectRect!=null)
                DrawRectangle(g, cachestate.GameObjects.Count.ToString() + " GameObjects;\n", GameObjectRect.Value, BlockPen, BlockBrush, BlackBrush, BlockInfoFont);


            String soundstring = getPlayingSoundsDesc();


            Size soundsize = TextRenderer.MeasureText(soundstring, GameObjectInfoFont);

            g.DrawString(soundstring, GameObjectInfoFont, BlackBrush, cachestate.GameArea.Right - soundsize.Width, cachestate.GameArea.Bottom - soundsize.Height);

            /*

             * 
             * 
             * 
            foreach (GameObject iterate in cachestate.GameObjects)
            {

                if (iterate != this)
                {
                    


                }


            }
            */

        }
    }
}
