using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using BASeBlock.Particles;

namespace BASeBlock.GameObjects
{
   
    public abstract class FireworkTrail : GameObject,iProjectile
    {
        private PointF _LaunchPosition;
        private PointF _Location;
        private PointF _Velocity;
        private TimeSpan _FuseLength;
        private FireworkEffect _EndPointEffect;
        
        public PointF LaunchPosition { get { return _LaunchPosition; } set { _LaunchPosition = value; } }
        public PointF Location { get { return _Location; } set { _Location = value; } }
        public PointF Velocity { get { return _Velocity; } set { _Velocity = value; } }
        public TimeSpan FuseLength { get { return _FuseLength; } set { _FuseLength = value; } }
        public FireworkEffect EndPointEffect { get { return _EndPointEffect; } set { _EndPointEffect = value; } }

        private DateTime? LaunchTime = null;

        protected FireworkTrail(PointF pLocation, PointF pVelocity, TimeSpan pFuseLength,FireworkEffect puseEffect)
        {
            LaunchPosition = Location = pLocation;
            _Velocity = pVelocity;
            _FuseLength = pFuseLength;
            _EndPointEffect = puseEffect;
        }
        public override void Draw(Graphics g)
        {
            //nuttin...
            //throw new NotImplementedException();
        }
        public override sealed bool PerformFrame(BCBlockGameState gamestate)
        {
            if (LaunchTime == null)
            {
                LaunchTime = DateTime.Now;
                BCBlockGameState.Soundman.PlaySound("fworklaunch");
                gamestate.DelayInvoke(FuseLength, (objs) =>
                    {
                        //detonate by setting off the firework effect here.
                        _EndPointEffect.Location = Location;
                        gamestate.NextFrameCalls.Enqueue(new BCBlockGameState.NextFrameStartup(() =>
                        {
                            gamestate.GameObjects.Remove(this);
                            gamestate.GameObjects.AddLast(_EndPointEffect);
                            
                        }));
                    });
            }

            
            
            //update position.
            BCBlockGameState.IncrementLocation(gamestate, ref _Location, Velocity);
            TrailEffect(gamestate);
            return false;
            
        }

        public abstract void TrailEffect(BCBlockGameState gamestate);

    }
    public class GenericFireworkBlast<T> : FireworkEffect where T : iLocatable, IMovingObject
    {
        public GenericFireworkBlast(PointF pLocation, float pSize, int pMinCount, int pMaxCount) 
            : base(typeof(T), pLocation, pSize, pMinCount, pMaxCount)
        {
        }

        public GenericFireworkBlast(PointF pLocation, float pSize, int pMinCount, int pMaxCount, Dictionary<Type, SpawnTypeEffect> pinitializers)
            : base(typeof(T), pLocation, pSize, pMinCount, pMaxCount, pinitializers)
        {
        }

        public GenericFireworkBlast( PointF pLocation, float pSize, int pMinCount, int pMaxCount, SizeF pSpawnSize)
            : base(typeof(T), pLocation, pSize, pMinCount, pMaxCount, pSpawnSize)
        {
        }

        public GenericFireworkBlast(PointF pLocation, float pSize, int pMinCount, int pMaxCount, SizeF pSpawnSize, Dictionary<Type, SpawnTypeEffect> pinitializers) : 
            base(typeof(T), pLocation, pSize, pMinCount, pMaxCount, pSpawnSize, pinitializers)
        {
        }

      
    }
    public class DustTrail : FireworkTrail
    {
        public DustTrail(PointF pLocation, PointF pVelocity, TimeSpan pFuseLength, FireworkEffect puseEffect) : base(pLocation, pVelocity, pFuseLength, puseEffect)
        {
        }
        public override void TrailEffect(BCBlockGameState gamestate)
        {
            gamestate.Particles.Add(new DustParticle(Location, 5, 50, Color.Red));
        }
    }
    

    public class FireworkEffect : InstantEffect
    {
        /// <summary>
        /// delegate called when the appropriate type is being created.
        /// </summary>
        /// <param name="sender">FireworkEffect instance that instantiated the type.</param>
        /// <param name="gstate">GameState object applicable to the currently running game.</param>
        /// <param name="spawntype">Type that was spawned.</param>
        /// <param name="Target">Object parameter that is of the given type that was just spawned.</param>
        public delegate void SpawnTypeEffect(FireworkEffect sender,BCBlockGameState gstate,Type spawntype,Object Target);
        private Dictionary<Type, SpawnTypeEffect> _SpawnInitializers;
        private List<Type> _SpawnTypes;
        private int _MinCount;
        private int _MaxCount;
        private PointF _Location;
        private float _Size;
        private SizeF _SpawnSize = new SizeF(16, 16);
        public PointF Location { get { return _Location; } set { _Location = value; } }
        public float Size { get { return _Size; } set { _Size = value; } }
        public SizeF SpawnSize { get { return _SpawnSize; } set { _SpawnSize = value; } }
        public IEnumerable<Type> SpawnTypes
        {
            get { return _SpawnTypes; }


            set
            {

                if(!value.All((s)=>BCBlockGameState.IsType(s,typeof(iLocatable))))
                    throw new ArgumentException("All types Must implement iLocatable");



                _SpawnTypes = new List<Type>(value);
            }
        }
        public int MinCount { get { return _MinCount; } set { _MinCount = value; } }
        public int MaxCount { get { return _MaxCount; } set { _MaxCount = value; } }


        public FireworkEffect(Type pSpawnType, PointF pLocation, float pSize, int pMinCount, int pMaxCount)
            : this(pSpawnType, pLocation, pSize, pMinCount, pMaxCount, new SizeF(16, 16), null)
        {

        }

        public FireworkEffect(Type pSpawnType, PointF pLocation, float pSize, int pMinCount, int pMaxCount, Dictionary<Type, SpawnTypeEffect> pinitializers)
            : this(pSpawnType, pLocation, pSize, pMinCount, pMaxCount, new SizeF(16, 16),pinitializers)
        {

        }

        public FireworkEffect(Type pSpawnType, PointF pLocation, float pSize, int pMinCount, int pMaxCount, SizeF pSpawnSize)
            : this(new Type[] { pSpawnType }, pLocation, pSize, pMinCount, pMaxCount, pSpawnSize, null)
        {
        }
        public FireworkEffect(Type pSpawnType, PointF pLocation, float pSize, int pMinCount, int pMaxCount, SizeF pSpawnSize, Dictionary<Type, SpawnTypeEffect> pinitializers)
            :this(new Type[] {pSpawnType},pLocation,pSize,pMinCount,pMaxCount,pSpawnSize,pinitializers) 
        {
            


        }
        public FireworkEffect(IEnumerable<Type> pSpawnTypes, PointF pLocation, float pSize, int pMinCount, int pMaxCount, SizeF pSpawnSize
            )
            : this(pSpawnTypes, pLocation, pSize, pMinCount, pMaxCount, pSpawnSize, null)
        {
        }
        public FireworkEffect(IEnumerable<Type> pSpawnTypes, PointF pLocation, float pSize, int pMinCount, int pMaxCount, SizeF pSpawnSize,Dictionary<Type, SpawnTypeEffect> pinitializers)
        {
            _SpawnInitializers = pinitializers;
            SpawnTypes = pSpawnTypes;
            Location = pLocation;
            Size = pSize;
            SpawnSize = pSpawnSize;
            MinCount = Math.Min(pMinCount, pMaxCount);
            MaxCount = Math.Max(pMinCount, pMaxCount);

        }


        public override void ApplyEffect(BCBlockGameState gstate)
        {
            BCBlockGameState.Soundman.PlaySound("fworkblast");
            //choose random value.
            int RandomAmount = BCBlockGameState.rgen.Next(_MinCount, _MaxCount);
            for (int i = 0; i < RandomAmount; i++)
            {

                //instantiate. Assume presence of PointF,SizeF constructor.
                Type SelectedType = BCBlockGameState.Choose(_SpawnTypes);

                iLocatable createobject = (iLocatable)Activator.CreateInstance(SelectedType, _Location);
                if (createobject is iSizedProjectile)
                {
                    ((iSizedProjectile)createobject).Size = _SpawnSize;

                }
                else if (createobject is SizeableGameObject)
                {
                    ((SizeableGameObject)createobject).Size = _SpawnSize;


                }
                if (_SpawnInitializers !=null && _SpawnInitializers.ContainsKey(SelectedType))
                    _SpawnInitializers[SelectedType](this, gstate, SelectedType, createobject);
                //choose random velocity based on size, if it is a projectile.
                if (createobject is IMovingObject)
                {
                    var usespeed = BCBlockGameState.GetRandomVelocity(0, Size);
                    (createobject as IMovingObject).Velocity = usespeed;
                }
                if (createobject is GameObject)
                    gstate.GameObjects.AddLast((GameObject)createobject);
                else if (createobject is Particle)
                    gstate.Particles.Add((Particle)createobject);

            }
        }
    }
}
