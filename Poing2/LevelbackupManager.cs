using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.Cryptography;
using System.Text;
using BASeCamp.Updating;

namespace BASeCamp.BASeBlock
{



    class LevelbackupManager
    {
        //class stores backup copies of Levels as they are opened in the Editor.
        //each level is 'indexed' by it's filename.
        //data is backed up in:
        //appdatafolder/.editorbackup/
        //this folder contains an "index.dat" file, which stores the main data table for looking up specific files.


        public class BackupDataItem : ISerializable 
        {
            /// <summary>
            /// Date/Time this backup was made
            /// </summary>
            public DateTime BackupDate;
            /// <summary>
            /// CRC of the file (used to make sure we don't recover a corrupted/changed file)
            /// The CRC of the file as it was at the time of the backup is stored as StoredCRC.
            /// </summary>
            
            public byte[] FileCRC;
            
            public String CRCString;
            

            public byte[] StoredCRC;
            public String StoredCRCString;
            /// <summary>
            /// Full path to the Backup File.
            /// </summary>
            public String FullPath;
            /// <summary>
            /// 
            /// </summary>
            /// <param name="pBackupDate"></param>
            /// <param name="pFullPath"></param>
            /// <param name="CopyFolder">Folder to copy the backup into.</param>
            public BackupDataItem(String OriginalFile,String CopyFolder)
            {

                BackupDate=DateTime.Now;
                
                
                //calculate CRC on the go. Since we are using this constructor, it must be a new item
                //and therefore our CRC values will be these.
                
                
                FileCRC = CRC32.GetCRC32forFile(FullPath);
                CRCString = CRC32.CRCToString(FileCRC);
                StoredCRC=FileCRC;
                StoredCRCString = CRCString;

                // TODO: This is also where we ought to make the actual copy, as well.
                //Generate a random name.
                String userandomname = GenerateRandomName() + ".dat";
                String copytarget = Path.Combine(CopyFolder, userandomname);
                try{Directory.CreateDirectory(CopyFolder);} catch{}
                if (Directory.Exists(CopyFolder))
                {
                    File.Copy(OriginalFile, copytarget);


                }



            }
            public static String GenerateRandomName()
            {
                StringBuilder buildname = new StringBuilder();
                Guid g = Guid.NewGuid();
                byte[] guidbytes = g.ToByteArray();
                for (int i = 0; i < guidbytes.Length; i++)
                {
                    //alpha chars only...
                    buildname.Append(97 + (guidbytes[i] % 26));


                }


                return buildname.ToString();


            }

            public BackupDataItem(SerializationInfo info, StreamingContext context)
            {
                BackupDate = info.GetDateTime("BackupDate");
                FullPath = info.GetString("FullPath");
                StoredCRC = (byte[])info.GetValue("StoredCRC", typeof(Byte[]));
                StoredCRCString = info.GetString("StoredCRCString");

                FileCRC = CRC32.GetCRC32forFile(FullPath);
                CRCString = CRC32.CRCToString(FileCRC);

            }
            public void GetObjectData(SerializationInfo info, StreamingContext context)
            {

                info.AddValue("BackupDate", BackupDate);
                info.AddValue("Fullpath", FullPath);
                info.AddValue("StoredCRC", StoredCRC);
                info.AddValue("StoredCRCString", StoredCRCString);

            }

        }
        /// <summary>
        /// data structure for containing the information pertaining to a single files backup history.
        /// </summary>
        public class BackupDataFileIndex:ISerializable 
        {
            /// <summary>
            /// Original filename that these items are a backup of
            /// </summary>
            String OriginalFilename;
            const int maxBackups = 10;
            Queue<BackupDataItem> BackupEntries = new Queue<BackupDataItem>();
            public String EntryFolder;
            public BackupDataFileIndex(String OriginalFile,String BaseFolder)
            {
                //creates the class. We don't actually create any BackupEntries until it is called.
                OriginalFilename = OriginalFile;


                //generate a random folder name. Again, this constructor will only be used to create new entries,
                // the Serialization constructor will be called for old ones. So we know this is a new entry.
                String usesubfolder = BackupDataItem.GenerateRandomName();

                EntryFolder = Path.Combine(BaseFolder, usesubfolder);

                Directory.CreateDirectory(EntryFolder);


            }

            public BackupDataItem CreateBackupEntry()
            {
                var returnthis = new BackupDataItem(OriginalFilename, EntryFolder);
                    
                    BackupEntries.Enqueue(returnthis);


                if (BackupEntries.Count > maxBackups)
                {
                    //limit of 'maxBackups'
                    BackupEntries.Dequeue();

                }

                return returnthis;

            }


            public BackupDataFileIndex(SerializationInfo info, StreamingContext context)
            {
                OriginalFilename = info.GetString("OriginalFilename");
                
                List<BackupDataItem> backlist = (List<BackupDataItem>)info.GetValue("BackupEntries", typeof(List<BackupDataItem>));

                BackupEntries = new Queue<BackupDataItem>(backlist);


            }

            public void GetObjectData(SerializationInfo info, StreamingContext context)
            {
                info.AddValue("OriginalFilename", OriginalFilename);
                info.AddValue("BackupEntries", BackupEntries.ToList());
                info.AddValue("EntryFolder", EntryFolder);
            }



        }




    }
}
