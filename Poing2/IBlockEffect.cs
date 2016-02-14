using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Xml.Linq;
using System.Xml.Serialization;
using BASeCamp.BASeBlock.Blocks;
using BASeCamp.BASeBlock.Particles;
using BASeCamp.Elementizer;


namespace BASeCamp.BASeBlock.Blocks
{


    public class LinearGlintEffect : IBlockEffect
    {
        private PointF _Origin = new Point(0, 0);
        

        private PointF _OriginSpeed = new Point(0, 1);
        private float _Angle = 0;
        private float _AngleSpeed = (float)(Math.PI / 35);
        private Image _useTexture = null;
        public PointF Origin { get { return _Origin; } set { _Origin = value; } }
        public PointF OriginSpeed { get { return _OriginSpeed; } set { _OriginSpeed = value; } }
        public float Angle { get { return _Angle; } set { _Angle = value; } }
        public float AngleSpeed { get { return _AngleSpeed; } set { _AngleSpeed = value; } }
        public Image useTexture { get { return _useTexture; } set { _useTexture = value; } }
        public LinearGlintEffect()
            : this(Point.Empty,new PointF(0.1f,0.1f), 0,(float)(Math.PI/30), BCBlockGameState.Imageman.getLoadedImage("glint"))
        {
        }
        public LinearGlintEffect(PointF pOrigin,PointF pOriginSpeed, float pAngle,float pAngleSpeed, Image puseTexture)
        {
            
            _Origin = pOrigin;
            _OriginSpeed = pOriginSpeed;
            _Angle = pAngle;
            _AngleSpeed = pAngleSpeed;
            _useTexture = puseTexture;


        }
        public LinearGlintEffect(LinearGlintEffect copythis)
            :this(copythis.Origin,copythis.OriginSpeed,copythis.Angle,copythis.AngleSpeed,copythis.useTexture)
        {


        }
        public LinearGlintEffect(XElement Source)
        {
            foreach(XElement checkelements in Source.Descendants())
            {
                if(checkelements.Name=="Origin")
                {
                    _Origin = StandardHelper.ReadElement<PointF>(checkelements);
                }
                else if(checkelements.Name=="OriginSpeed")
                {
                    _OriginSpeed = StandardHelper.ReadElement<PointF>(checkelements);
                }
                else if (checkelements.Name == "useTexture")
                {
                    _useTexture = StandardHelper.ReadElement<Image>(checkelements);
                }
            }
            foreach(XAttribute checkattributes in Source.Attributes())
            {
                if (checkattributes.Name == "Angle") _Angle = float.Parse(checkattributes.Value);
                if (checkattributes.Name == "AngleSpeed") _AngleSpeed = float.Parse(checkattributes.Value);
            }
        }
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Origin", _Origin);
            info.AddValue("OriginSpeed", _OriginSpeed);
            info.AddValue("Angle", _Angle);
            info.AddValue("AngleSpeed", _AngleSpeed);
            info.AddValue("useTexture", _useTexture);
        }
        public XElement GetXmlData(String pNodeName)
        {
            return new XElement(pNodeName,
                StandardHelper.SaveElement(_Origin,"Origin"),
                StandardHelper.SaveElement(_OriginSpeed,"OriginSpeed"),
                new XAttribute("Angle",_Angle),
                new XAttribute("AngleSpeed",_AngleSpeed),
                StandardHelper.SaveElement(_useTexture,"useTexture"));
        }
        public object Clone()
        {
            return new LinearGlintEffect(this);
        }

        public void Draw(Block sourceblock, Graphics g)
        {
            g.FillRectangle(tb, sourceblock.BlockRectangle);
        }

        public bool RequiresPerformFrame(Block sourceblock)
        {
            return true;
        }
        TextureBrush tb = null;
        public bool PerformFrame(BCBlockGameState gamestate, Block sourceblock)
        {
            if(tb==null)
            {
                tb = new TextureBrush(useTexture,new Rectangle(0,0,useTexture.Width,useTexture.Height));
             
            }
            Angle += AngleSpeed;
            Origin = new PointF(Origin.X + OriginSpeed.X, Origin.Y + OriginSpeed.Y);
            tb.ResetTransform();
            tb.TranslateTransform(Origin.X,Origin.Y,MatrixOrder.Prepend);
            tb.RotateTransform(Angle);
            sourceblock.hasChanged = true;

            return false;

        }

      
    }


    public class ParticleEmanationEffect : IBlockEffect
    {
        private int _SpawnCount = 1;
        private int _SpawnRate = 10;
        private Type _SpawnType = typeof(Particles.DustParticle);
        /// <summary>
        /// number of wait states.
        /// </summary>
        public int SpawnRate { get { return _SpawnRate; } set { _SpawnRate = value; } }

        /// <summary>
        /// Number of particles to spawn.
        /// 
        /// </summary>
        public int SpawnCount { get { return _SpawnCount;} set {_SpawnCount=value;}}
        [Editor(typeof(ItemTypeEditor<Particles.Particle>),typeof(UITypeEditor))]
        public Type SpawnType { get { return _SpawnType; } set { _SpawnType = value; } }



        
        public ParticleEmanationEffect() { }
        public ParticleEmanationEffect(int pSpawnRate, int pSpawnCount, Type pSpawnType)
        {
            _SpawnRate = pSpawnRate;
            _SpawnCount = pSpawnCount;
            _SpawnType = pSpawnType;
            


        }
        public ParticleEmanationEffect(ParticleEmanationEffect clonethis)
            :this(clonethis.SpawnRate,clonethis.SpawnCount,clonethis.SpawnType)
        {

        }
        public ParticleEmanationEffect(XElement source)
        {
            foreach(XAttribute lookAttribute in source.Attributes())
            {
                if(lookAttribute.Name=="SpawnRate")
                {

                }
                else if(lookAttribute.Name=="SpawnType")
                {
                    _SpawnType = BCBlockGameState.FindClass(lookAttribute.Value);
                }
                else if(lookAttribute.Name=="SpawnCount")
                {
                    _SpawnCount = int.Parse(lookAttribute.Value);
                }
            }
        }
        public XElement GetXmlData(String pNodeName)
        {
            return new XElement(pNodeName,
            new XAttribute("SpawnRate", _SpawnRate),
            new XAttribute("SpawnType", _SpawnType.Name),
            new XAttribute("SpawnCount", _SpawnCount));
        }
        public ParticleEmanationEffect(SerializationInfo info, StreamingContext context)
        {
            _SpawnRate = info.GetInt32("SpawnRate");
            _SpawnType = info.GetValueType("SpawnType");
            _SpawnCount= info.GetInt32("SpawnCount");
        }
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("SpawnRate",_SpawnRate);
            info.AddValueType("SpawnType",_SpawnType);
            info.AddValue("SpawnCount",_SpawnCount);
        }

        public void Draw(Block sourceblock, Graphics g)
        {
            //nuttin here.
        }

        public bool RequiresPerformFrame(Block sourceblock)
        {
            return true;
        }
        int ratecount =0;
        public bool PerformFrame(BCBlockGameState gamestate, Block sourceblock)
        {
            ratecount++;
            if(ratecount==_SpawnRate)
            {
                ratecount=0;
            //spawn appropriate number of items.
                for(int i=0;i<_SpawnCount;i++)
                {

                    Particle genparticle = Activator.CreateInstance(_SpawnType) as Particle;
                    genparticle.Location = sourceblock.BlockRectangle.RandomSpot(BCBlockGameState.rgen);
                    genparticle.Velocity= BCBlockGameState.GetRandomVelocity(0,1);
                    gamestate.Particles.Add(genparticle);

                }


            }
            return false;
        }

        public object Clone()
        {
            return new ParticleEmanationEffect(this);
        }
    }

    /// <summary>
    /// the iBlockEffect interface allows for special "effects" to applied to any block.
    /// Main implementation (and only one I can think of so far) is a animated effect that emanates particles,
    /// something akin to redstone ore in Minecraft, I suppose. 
    /// </summary>
    public interface IBlockEffect:ISerializable,ICloneable,IXmlPersistable
    {
        /// <summary>
        /// called when the block is drawn.
        /// </summary>
        /// <param name="gamestate"></param>
        /// <param name="sourceblock"></param>
        /// <param name="g"></param>
        void Draw(Block sourceblock, Graphics g);
        /// <summary>
        /// called when the Block's "RequiresPerformFrame" routine is called.
        
        /// </summary>
        /// <param name="sourceblock"></param>
        /// <returns></returns>
        bool RequiresPerformFrame(Block sourceblock);
        /// <summary>
        /// called on instances of iBlockEffect on the Block that return true for RequiresPerformFrame.
        /// </summary>
        /// <param name="gamestate"></param>
        /// <param name="sourceblock"></param>
        /// <returns></returns>
        bool PerformFrame(BCBlockGameState gamestate, Block sourceblock);
    }
}
