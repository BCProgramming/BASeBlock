using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Design;
using System.Runtime.Serialization;
using BASeCamp.BASeBlock.Events;

namespace BASeCamp.BASeBlock.Blocks
{
    [Serializable]
    public class SpawnerBlock : ImageBlock,IEditorBlockExtensions
    {
        private Type _useSpawnType = null;

        private String _EnemySpawnType = typeof(SnakeEnemy).FullName;
        //[Editor(typeof(ObjectPropertyEditor),typeof(UITypeEditor))]

        private List<EnemyTrigger> _EnemySpawnTriggers = new List<EnemyTrigger>();

        [Editor(typeof(EnemyTriggerCollectionEditor), typeof(UITypeEditor))]
        public List<EnemyTrigger> EnemySpawnTriggers
        {
            get { return _EnemySpawnTriggers; }
            set { _EnemySpawnTriggers = value; }

        } //spawn trigger value to give to the spawned enemy.
        [Editor(typeof(GameObjectTypeEditor ),typeof(UITypeEditor))]
        public Type useSpawnType { 
            get {

                if (_useSpawnType == null && !String.IsNullOrEmpty(EnemySpawnType))
                {
                    _useSpawnType = BCBlockGameState.FindClass(_EnemySpawnType);

                }
                return _useSpawnType;
            } 
            set { _useSpawnType = value; 
                _EnemySpawnType = _useSpawnType==null?"":_useSpawnType.Name; 
            } }
        
        private String EnemySpawnType { get { return _EnemySpawnType; } set { _EnemySpawnType = value; } }
        public bool InstantSpawn { get; set; }
        public object[] EnemySpawnArguments;
        public string ChangeMusicTo { get; set; }

        public SpawnerBlock(RectangleF pblockrect)
            : this(pblockrect, typeof(SnakeEnemy).FullName, null, "wart")
        {

        }

        public SpawnerBlock(RectangleF pblockrect, String SpawnType, object[] spawnerargs, String pChangeMusicTo)
            : base(pblockrect, "Generic_4")
        {
            Type useSpawnType = BCBlockGameState.FindClass(SpawnType);



            if (!useSpawnType.IsSubclassOf(typeof(GameObject)))
                throw new ArgumentException("Argument must be a subclass type of GameObject", "SpawnType");


            EnemySpawnArguments = spawnerargs;
            ChangeMusicTo = pChangeMusicTo;

        }
        public SpawnerBlock(SpawnerBlock clonethis)
            : base(clonethis)
        {

            //EnemySpawnTriggers = clonethis.EnemySpawnTriggers;
            EnemySpawnTriggers = new List<EnemyTrigger>();
            if (clonethis.EnemySpawnTriggers != null)
            {
                foreach (var loopit in clonethis.EnemySpawnTriggers)
                {
                    EnemySpawnTriggers.Add((EnemyTrigger)loopit.Clone());

                }
            }
            EnemySpawnArguments = clonethis.EnemySpawnArguments;
            ChangeMusicTo = clonethis.ChangeMusicTo;
            EnemySpawnType = clonethis.EnemySpawnType;
            useSpawnType = BCBlockGameState.FindClass(EnemySpawnType);
            InstantSpawn = clonethis.InstantSpawn;

        }
        public SpawnerBlock(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            //EnemySpawnType = (Type)info.GetValue("EnemySpawnType", typeof(Type));
            EnemySpawnType = info.GetString("EnemySpawnType");
            useSpawnType = BCBlockGameState.FindClass(EnemySpawnType);
            EnemySpawnTriggers = (List<EnemyTrigger>)info.GetValue("EnemySpawnTriggers", typeof(List<EnemyTrigger>));
            EnemySpawnArguments = (object[])info.GetValue("EnemySpawnArguments", typeof(object[]));
            ChangeMusicTo = info.GetString("ChangeMusicTo");
            try { InstantSpawn = info.GetBoolean("InstantSpawn"); }catch { }

        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("EnemySpawnTriggers", EnemySpawnTriggers);
            info.AddValue("EnemySpawnType", EnemySpawnType);
            info.AddValue("EnemySpawnArguments", EnemySpawnArguments);
            info.AddValue("ChangeMusicTo", ChangeMusicTo);
            info.AddValue("InstantSpawn", InstantSpawn);
        }

        public override object Clone()
        {
            return new SpawnerBlock(this);
        }
        public void Musicreverter(Object sender,EnemyDeathEventArgs e)
        {
            if (!String.IsNullOrEmpty(ChangeMusicTo))
                //BCBlockGameState.Soundman.PopMusic(ChangeMusicTo);
                BCBlockGameState.Soundman.StopTemporaryMusic(ChangeMusicTo);
        }
        public override bool RequiresPerformFrame()
        {
            return InstantSpawn; 
        }
        public override bool PerformFrame(BCBlockGameState gamestate)
        {
            if (InstantSpawn)
            {
                try
                {
                    InstantiateObject(gamestate);
                }
                catch
                {

                 
                }
                ProxyObject po = new ProxyObject(ProxyObjectFunc, null);
                gamestate.GameObjects.AddLast(po);
                return true;
            }
            return false;
        }

        private bool ProxyObjectFunc(ProxyObject p, BCBlockGameState g)
        {
            return g.Blocks.Remove(this);
            return true;
        }

        
        public void InstantiateObject(BCBlockGameState parentstate)
        {


            //GameEnemy createenemy = (GameEnemy)Activator.CreateInstance(EnemySpawnType, EnemySpawnArguments);
            GameObject createenemy = null;
            try
            {
                createenemy = (GameObject)Activator.CreateInstance(useSpawnType, new object[] { CenterPoint() });
            }
            catch
            {
                
                createenemy = (GameObject)Activator.CreateInstance(useSpawnType,new object[] { CenterPoint(),new SizeF(16,16)});

            }
            Debug.Print("Giving Spawned enemy  " + EnemySpawnTriggers.Count + " Triggers...");
            if (createenemy is GameEnemy)
            {
                GameEnemy castenemy = createenemy as GameEnemy;
                castenemy.Triggers = EnemySpawnTriggers;


               
                //also, tell the Triggers they now belong to an enemy...
                foreach (EnemyTrigger looptrigger in castenemy.Triggers)
                {
                    looptrigger.OurEnemy = castenemy;


                }
            }
            //add to current game;
            parentstate.GameObjects.AddLast(createenemy);

            //if we have a music to change to, do so.
            if (!String.IsNullOrEmpty(ChangeMusicTo))
            {
                if (BCBlockGameState.Soundman.scurrentPlayingMusic != ChangeMusicTo)
                {
                    // oldmusic = BCBlockGameState.Soundman.scurrentPlayingMusic;
                    //BCBlockGameState.Soundman.StopMusic();
                    //BCBlockGameState.Soundman.PlayMusic(ChangeMusicTo, true);
                    //BCBlockGameState.Soundman.PushMusic(ChangeMusicTo, 1.0f, true);
                    BCBlockGameState.Soundman.PlayTemporaryMusic(ChangeMusicTo, 1.0f, true);
                    if(createenemy is GameEnemy)
                        (createenemy as GameEnemy).OnDeath += Musicreverter;
                }
            }

        }
        public override bool PerformBlockHit(BCBlockGameState parentstate, cBall ballhit)
        {
            //throw new NotImplementedException();
            //spawn the Enemy.
            if(!InstantSpawn)
                InstantiateObject(parentstate);

        
            return true;
        }

        /// <summary>
        /// called when this block is drawn in the editor. the "normal" implementation of Draw will not be called afterwards.
        /// </summary>
        /// <param name="g"></param>
        public override void EditorDraw(Graphics g, IEditorClient Client)
        {
            //nuttin'
            Draw(g);
        }

        /// <summary>
        /// retrieves additional tooltip information for a block.
        /// </summary>
        /// <returns></returns>
        public override string GetToolTipInfo(IEditorClient Client)
        {
            return "Contains: " + _EnemySpawnType + ", with " + _EnemySpawnTriggers.Count + " Triggers.";
        }
    }
}