using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using Ionic.Zlib;

namespace BASeCamp.BASeBlock
{
    /// <summary>
    /// sets/returns data regarding locked levels.
    /// a locked level will show up with a padlock or question mark in the 
    /// eventual level browser. The first level of any levelset is
    /// unlocked automatically.
    /// </summary>
    [Serializable]
    public class LevelLockData :ISerializable
    {
        protected Dictionary<String, bool> Lockeddata = new Dictionary<string, bool>();

        protected String sFileName = "";

        public LevelLockData(SerializationInfo info, StreamingContext context)
        {
            Lockeddata = (Dictionary<String, bool>)info.GetValue("LockedData", typeof(Dictionary<String, bool>));


        }
        public LevelLockData(String lockfilename)
        {
            if (File.Exists(lockfilename))
            {
                LevelLockData testload = FromFile(lockfilename);
                if (testload != null)
                {
                    sFileName = testload.sFileName;
                    Lockeddata = testload.Lockeddata;
                    



                }


            }
            else
            {
                sFileName = lockfilename;

            }


        }
        ~LevelLockData()
        {

            Save();

        }

        public static LevelLockData FromFile(String sfilename)
        {
            BinaryFormatter ibin = new BinaryFormatter();
            try
            {
                using (FileStream fstream = new FileStream(sfilename, FileMode.Open))
                {

                    GZipStream gzs = new GZipStream(fstream, CompressionMode.Decompress);
                    LevelLockData returnthis = (LevelLockData)ibin.Deserialize(gzs);

                    returnthis.sFileName= sfilename;
                    gzs.Close();
                    return returnthis;
                }
            }
            catch
            {
                return null; //null to indicate error...
            }





        }
        public void Save()
        {
            Save(sFileName);


        }

        public void Save(String targetfile)
        {
            if (String.IsNullOrEmpty(targetfile)) return;
            Debug.Print("Saving lockdata to " + targetfile);
            BinaryFormatter ibin = new BinaryFormatter();
            FileStream fstream = new FileStream(targetfile, FileMode.Create);

            GZipStream gzstream = new GZipStream(fstream, CompressionMode.Compress);
            //ibin.Serialize(fstream, this);
            ibin.Serialize(gzstream, this);
            gzstream.Close();


        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {

            info.AddValue("LockedData", Lockeddata);


        }

        private String HashLevel(LevelSet inset, Level levelobj)
        {
            String levelsethash = inset.MD5Hash();
            string levelhash = levelobj.MD5Hash();

            return levelsethash+levelhash;


        }


        public bool isLocked(LevelSet lset,Level testlevel)
        {

            String hashvalue=HashLevel(lset, testlevel);
            if(lset.Levels.First()==testlevel)
                return false; //first level is never locked.
            else
            {
                if (Lockeddata.ContainsKey(hashvalue))
                {
                    return Lockeddata[hashvalue];

                }
                else
                {
                    return true;
                }
            }




        }
        public void setLocked(LevelSet lset, Level setlevel, bool setvalue)
        {
            string hashvalue = HashLevel(lset, setlevel);
            Lockeddata[hashvalue]=setvalue;


        }







    }
}
