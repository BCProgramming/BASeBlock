using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using BASeBlock.Blocks;

namespace BASeBlock
{
    /// <summary>
    /// a Blockproxy acts similarly to a block, but is a GameObject.
    /// The purpose is to act as an AnimatedBlock, since some block effects
    /// might require movement of blocks that aren't animated blocks.
    /// </summary>
    public class BlockProxy : Projectile
    {
        private Image _ProxyImage = null;

        
        
        private RectangleF _ProxyRect;
        
        private iImagable _Source;
        private Func<BlockProxy,BCBlockGameState,bool> _ProxyPerform = null;
        private bool _Collide = true;
        public RectangleF ProxyRect { get { return _ProxyRect; } set { _ProxyRect = value; } }
        public Image ProxyImage { get { return _ProxyImage; } set { _ProxyImage = value; } }
        public iImagable Source { get { return _Source; } set { _Source = value; } }
        public bool Collide { get { return _Collide;} set {_Collide=value;}}
        public Func<BlockProxy, BCBlockGameState,bool> ProxyPerform { get { return _ProxyPerform; } set { _ProxyPerform = value; } }
        
        public BlockProxy(iImagable Source, PointF pVelocity, Func<BlockProxy, BCBlockGameState,bool> pProxyPerform)
            : base(Source.Location, new PointF(0, 0))
        {
            _Source = Source;
            _ProxyImage = Source.getImage();
            _ProxyRect = Source.getRectangle();
            _Velocity = pVelocity;
            _ProxyPerform = pProxyPerform;
        }
        public BlockProxy(iImagable Source)
            : this(Source, new PointF(0, 0),null)
        {


        }
        
        public override bool PerformFrame(BCBlockGameState gamestate)
        {
            var result = base.PerformFrame(gamestate); ;
            if (_ProxyPerform != null)
            {
                result = _ProxyPerform(this, gamestate);
            }
            //collision check.
            if (Collide)
            {
                foreach (cBall iterateball in gamestate.Balls)
                {
                    if(BCBlockGameState.DoesBallTouch(_ProxyRect,iterateball))
                    {
                        Block.BallRelativeConstants brc = Block.BallRelative(_ProxyRect,iterateball);
                        iterateball.numImpacts++;
                        int xsign=0, ysign=0;

                        if ((brc & Block.BallRelativeConstants.Relative_Right) == Block.BallRelativeConstants.Relative_Right)
                        {
                            xsign = 1;
                        }
                        else if((brc & Block.BallRelativeConstants.Relative_Left) == Block.BallRelativeConstants.Relative_Left)
                        {
                            xsign = -1;
                        }

                        if ((brc & Block.BallRelativeConstants.Relative_Down) == Block.BallRelativeConstants.Relative_Down)
                        {
                            ysign = 1;
                        }
                        else if((brc & Block.BallRelativeConstants.Relative_Up) == Block.BallRelativeConstants.Relative_Up)
                        {
                            ysign = -1;
                        }

                        iterateball.Velocity = new PointF(Math.Abs(iterateball.Velocity.X) * xsign,
                            Math.Abs(iterateball.Velocity.Y * ysign));

                    }

                }


            }


            return result;
        }
        public override void Draw(Graphics g)
        {
            g.DrawImage(_ProxyImage, _ProxyRect.Left, _ProxyRect.Top, _ProxyRect.Width, _ProxyRect.Height);
        }
        

    }
}
