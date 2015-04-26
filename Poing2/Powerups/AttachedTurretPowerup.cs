using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using BASeBlock.PaddleBehaviours;
using BASeBlock.WeaponTurrets;

namespace BASeBlock.Powerups
{
    public class AttachedTurretPowerup<TurretBehaviour,TurretType> : TerminatorPaddlePowerup<TurretBehaviour>, ITurretOwner
        where TurretType : BaseTurret 
        where TurretBehaviour : AttachedTurretBehaviour
    {
        /// <summary>
        /// property array of Objects which will be used when the Turret class type is instantiated to get the image.
        /// 
        /// </summary>
        private Object[] ExtraConstructionParams { get; set; }

        private Image BuildPowerupImage(BaseTurret useinstance)
        {
            //draw at our own drawsize.
            Image DrawImage = new Bitmap((int)Size.Width, (int)Size.Height);
            Graphics g = Graphics.FromImage(DrawImage);
            //paint the genericPowerup onto it.
            g.DrawImage(BCBlockGameState.Imageman["genericpowerup"], 0, 0, Size.Width, Size.Height);
            useinstance.TurretAngle = (float)Math.PI;
            useinstance.Draw(this, g);
            return DrawImage;

        }
        public override Image[] GetPowerUpImages()
        {
            return AnimationFrames;
        }
        public AttachedTurretPowerup(PointF pLocation, SizeF pSize, Object[] constructionparams):this(pLocation,pSize)
        {
            ExtraConstructionParams = constructionparams;
        }
        public AttachedTurretPowerup(PointF pLocation,SizeF pSize):base(pLocation,pSize){

            AnimationFrames = null;

            
            
            
        }
        bool flInitialized = false;
        public override bool PerformFrame(BCBlockGameState gamestate)
        {
            if (!flInitialized)
            {
                flInitialized = true;
                //build the image of the powerup.
                BaseTurret buildinstance;
                if(ExtraConstructionParams==null)
                    buildinstance = (BaseTurret)Activator.CreateInstance(typeof(TurretType),this,gamestate);
                else
                {
                    Object[] buildparams = new object[ExtraConstructionParams.Length+2];
                    buildparams[0] = this;
                    buildparams[1] = gamestate;
                    for(int i=0;i<ExtraConstructionParams.Length;i++){
                        buildparams[2+i] = ExtraConstructionParams[i];
                    }

                    buildinstance = (BaseTurret)Activator.CreateInstance(typeof(TurretType),this,gamestate,buildparams);
                }
                Image PowerupImage = BuildPowerupImage(buildinstance);
                AnimationFrames = new Image[]{PowerupImage};
            }
            return base.PerformFrame(gamestate);
        }
        public override void Draw(Graphics g)
        {
            if (base.AnimationFrames == null) return;
            base.Draw(g);

        }

        PointF ITurretOwner.getTurretPositionOffset(ITurret CheckTurret)
        {
            return this.CenterPoint();
        }
    }
}
