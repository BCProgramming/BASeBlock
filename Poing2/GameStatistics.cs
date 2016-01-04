using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using Ionic.Zlib;
using System.Diagnostics;
namespace BASeCamp.BASeBlock
{
    //idea is to be like the cNewSoundManager or ImageManager or various classes of that sort, and manage a set of statistics.


    //a Statistics file will be maintained for various things; there will be one that emcompasses all data ever, one for each levelset, and, possibly, one for each level as well.

    public class StatisticsManager:IDisposable
    {

        //StatIndex is the index file; key is unique name, and string is path/filename relative to the stat folder that contains that
        //relevant GameStatistics file serialized.
        public GameStatistics GlobalStatistics
        {
            get
            {
                return this["global stats"];
            }
        }
        private Dictionary<String, GameStatistics> StatIndex = new Dictionary<string, GameStatistics>();
        private String _Statfile = "";
        public String StatFile { get { return _Statfile; } set { _Statfile = value; } }
        /// <summary>
        /// Constructs this instance with the given file with which to index statistics. 
        
        /// </summary>
        /// <param name="sfilename">file to read the set of statistics from.</param>
        public StatisticsManager(String sfilename)
        {
            _Statfile = sfilename;
            if(File.Exists(_Statfile))
                try
                {
                    Read(new FileStream(_Statfile, FileMode.Open, FileAccess.Read));
                }
                catch (Exception exx)
                {
                    Debug.Print("Exception at startup:" + exx.ToString());
                }

        }
        private void Read(Stream sStatStream)
        {
            try
            {
                BinaryFormatter bf = new BinaryFormatter();
                using (ZlibStream zs = sStatStream is ZlibStream ? sStatStream as ZlibStream : new ZlibStream(sStatStream, CompressionMode.Decompress))
                {
                    StatIndex = (Dictionary<string, GameStatistics>)bf.Deserialize(zs);
                }
            }
            catch (SerializationException exx)
            {
                StatIndex = new Dictionary<string, GameStatistics>();
            }

        }
        public bool Exists(String Index)
        {
            return StatIndex.ContainsKey(Index);
        }
        public GameStatistics getItem(String Index)
        {
            if (!StatIndex.ContainsKey(Index))
            {
                StatIndex.Add(Index, new GameStatistics());

            }
            return StatIndex[Index];

        }
        
        public GameStatistics this[String index]
        {
            get
            {
                return getItem(index);
            }
            set
            {
                StatIndex[index] = value;

            }

        }
        #region IDisposable

        private bool _Disposed = false;
        public void Dispose()
        {
            if (_Disposed) return;
            _Disposed = true;
            Save();


        }
        ~StatisticsManager()
        {
            Dispose();


        }

        #endregion
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            foreach (var iterate in StatIndex)
            {
                sb.AppendLine("Statistics for " + iterate.Key);
                sb.AppendLine(iterate.Value.ToString());
                

            }


            return sb.ToString();
        }
        public void Save()
        {
            if (String.IsNullOrEmpty(_Statfile)) throw new ArgumentException("Statfile field must be populated for empty parameter Save method");
            Save(_Statfile);

        }
        public void Save(String sfilename)
        {
            //make sure the directory exists.
            String strpath = sfilename.Substring(0, sfilename.Length - Path.GetFileName(sfilename).Length -1);
            if (!Directory.Exists(strpath))
                Directory.CreateDirectory(strpath);
            


            using (FileStream fs = new FileStream(sfilename, FileMode.OpenOrCreate, FileAccess.Write))
            {
                ZlibStream zs = new ZlibStream(fs, CompressionMode.Compress);
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(zs,StatIndex);
            }
            
        }
        

    }
   
    


    /// <summary>
    /// Deals with, saves, and loads Various game statistics.
    /// </summary>
    [Serializable]
    public class GameStatistics : ISerializable,IDisposable 
    {
        
        private Dictionary<String, Object> _DataMembers = new Dictionary<string, object>();
        private String _SourceFile = "";
        public String SourceFile { get { return _SourceFile; } set { _SourceFile = value; } }
        private void setMemberValue(String Key, double Value)
        {
            double currvalue = getItem(Key, 0d);
            double difference = Value - currvalue;
            if (this != BCBlockGameState.Statman.GlobalStatistics)
            {
                double currentvalue = BCBlockGameState.Statman.GlobalStatistics.getItem(Key, 0d);
                BCBlockGameState.Statman.GlobalStatistics.setMemberValue(Key, currentvalue + Math.Abs(difference));

            }
            else
            {
                Debug.Print("Global Statistics, adding " + difference + " to " + Key + Environment.NewLine);
            }
            _DataMembers[Key] = Value;


        }
        private void setMemberValue(String Key, int Value)
        {

            int currvalue = getItem(Key, 0);
            int difference = Value - currvalue;
            if (this != BCBlockGameState.Statman.GlobalStatistics)
            {
                int currentvalue = BCBlockGameState.Statman.GlobalStatistics.getItem<int>(Key, 0);
                BCBlockGameState.Statman.GlobalStatistics.setMemberValue(Key, currentvalue + Math.Abs(difference));
            
            }
            else
            {
                Debug.Print("Global Statistics, adding " + difference + " to " + Key + Environment.NewLine);
            }
            _DataMembers[Key] = Value;



        }




        
        public int EnemyKills { get { return getItem("EnemyKills", 0); }
            set { setMemberValue("EnemyKills", value); }
        }
        public int BossKills { get { return getItem("BossKills", 0); }
            set { setMemberValue("BossKills", value); }
        }
        public int Deaths { get { return getItem("Deaths", 0); }
            set { setMemberValue("Deaths", value); }
        }
        public int TotalScore { get { return getItem("TotalScore", 0); }
            set { setMemberValue("TotalScore", value); }
        }
        public int TotalNegativeScore { get { return getItem("TotalNegativeScore", 0); }
            set { setMemberValue("TotalNegativeScore", value); }
        }
        public int LevelsCompleted
        {
            get { return getItem("LevelsCompleted", 0); }
            set { setMemberValue("LevelsCompleted", value); }
        }
        public double TotalDamage
        {
            get { return getItem("TotalDamage", 0d); }
            set
            {
                setMemberValue("TotalDamage", value);
            }
        }
        public double TotalHealed
        {
            get { return getItem("TotalHealed", 0d); }
            set { setMemberValue("TotalHealed", value); }

        }
        internal T getItem<T>(String key,T defaultvalue)
        {
            if (!_DataMembers.ContainsKey(key))
                _DataMembers.Add(key, defaultvalue);

            return (T)_DataMembers[key];


        }

        //Disposable stuff.
        #region IDisposable
        private bool _Disposed = false;
        public void Dispose()
        {
            if (_Disposed) return;
            _Disposed = true;

            if (!String.IsNullOrEmpty(_SourceFile))
            {
                //save to the source!
                Save();

            }



        }
        public override string ToString()
        {
            return "Level Statistics\n" +
                "Deaths:" + Deaths + "\n" +
                "Kills:" + EnemyKills + "\n" +
                "Boss Kills:" + BossKills + Environment.NewLine +
                "Total Score:" + TotalScore + Environment.NewLine +
                "Total Score Loss:" + TotalNegativeScore + Environment.NewLine +
                "Total Damage:" + TotalDamage + Environment.NewLine +
                "Total Healed:" + TotalHealed + Environment.NewLine;

            
        }
        ~GameStatistics()
        {
            Dispose();

        }

        #endregion


        public static GameStatistics FromFile(String sfile)
        {
            if (File.Exists(sfile))
            {
                using (FileStream fs = new FileStream(sfile, FileMode.Open))
                {
                    using (Ionic.Zlib.ZlibStream zl = new ZlibStream(fs, CompressionMode.Decompress))
                    {
                        BinaryFormatter bf = new BinaryFormatter();
                        GameStatistics retrievevalue = (GameStatistics)bf.Deserialize(zl);
                        retrievevalue.SourceFile = sfile;
                        return retrievevalue;

                    }

                }
            }
            else
            {
                return new GameStatistics(sfile);
            }

            
            
        }
        public void Save()
        {
            if (String.IsNullOrEmpty(_SourceFile))
            {
                throw new ArgumentException("zero parameter Save overload requires present SourceFile field");

            }
            Save(_SourceFile);
        }
        public void Save(String sfile)
        {
            if (File.Exists(sfile))
            {
                using (FileStream fs = new FileStream(sfile, FileMode.Create))
                {
                    _SourceFile = sfile;
                    Save(fs);

                }
            }

        }
        /// <summary>
        /// save this statistics file to the given stream.
        /// </summary>
        /// <param name="ToStream"></param>
        public void Save(Stream ToStream)
        {
            //we can only save to writable streams, for obvious reasons.
            if (!ToStream.CanWrite) throw new ArgumentException("ToStream must be a writable Stream.");
           //conditional: if it's already a zlibstream, don't make a new one. otherwise, make sure our output is compressed.
            Stream usestream = ToStream is ZlibStream ? ToStream : new ZlibStream(ToStream, CompressionMode.Compress);
            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(usestream, this);

        }
        public GameStatistics(String SourceFile)
        {
            _SourceFile = SourceFile;
            


        }
        public GameStatistics()
        {

        }
        public GameStatistics(SerializationInfo info, StreamingContext context)
        {
            _DataMembers = (Dictionary<String,Object>)(info.GetValue("DataMembers", typeof(Dictionary<String, Object>)));
        }
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("DataMembers", _DataMembers);

        }

    }

   
}
