using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Design;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.BASeCamp;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Web.Script.Serialization;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using System.Xml.Linq;
using BASeCamp.XMLSerialization;
using Ionic.Zlib;
using ProgressStream;

namespace BASeCamp.BASeBlock
{

 




    [Serializable]

    public class CreatorPropertiesEditor : UITypeEditor
    {

        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.Modal;
        }
        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            IWindowsFormsEditorService svc = (IWindowsFormsEditorService)
                provider.GetService(typeof(IWindowsFormsEditorService));
            if (svc != null)
            {
                svc.ShowDialog(new frmCreatorPropertiesEditor((CreatorProperties)value));
                // update etc
            }
            return value;
        }


    }

   

    [Serializable]
    public class CreatorProperties : ISerializable,IDeserializationCallback ,ICloneable,IXmlPersistable
    {




        //Editors... for properties
        public class SoundDataItemListEditor : UITypeEditor
        {


            public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
            {
                return UITypeEditorEditStyle.Modal;
            }
            public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
            {
                IWindowsFormsEditorService svc = (IWindowsFormsEditorService)
                    provider.GetService(typeof(IWindowsFormsEditorService));
                if (svc != null)
                {
                    svc.ShowDialog(new frmSoundDataListEditor((Dictionary<String, SoundDataItem>)value));
                    // update etc
                }
                return value;
            }


        }
        
        public class ImageDataItemListEditor : UITypeEditor
        {
            public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
            {
                return UITypeEditorEditStyle.Modal;
            }
            
            public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
            {
                IWindowsFormsEditorService svc = (IWindowsFormsEditorService)
                    provider.GetService(typeof(IWindowsFormsEditorService));
                if (svc != null)
                {
                    svc.ShowDialog(new frmImageDataListEditor((Dictionary<String, ImageDataItem>)value));
                    // update etc
                }
                return value;
            }

        }
        #region Script management
      

        
        public class ScriptDataItemListEditor : UITypeEditor
        {
            public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
            {
                return UITypeEditorEditStyle.Modal;
            }
            public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
            {
                IWindowsFormsEditorService svc = (IWindowsFormsEditorService)
       provider.GetService(typeof(IWindowsFormsEditorService));
                if (svc != null)
                {
                    svc.ShowDialog(new frmScriptDataItemListEditor((Dictionary<String, ScriptDataItem>)value));
                    // update etc
                }
                return value;  
            }


        }

        [Serializable]
        //represents a script.

        public class ScriptDataItem : ISerializable, IDeserializationCallback,ICloneable
        {
            public String Name { get; set; } //Name of this script.
            public String Language { get; set; } //Script Language 
            public String Code { get; set; } //Source code

            public Object Clone()
            {

                return new ScriptDataItem(this);

            }
            public ScriptDataItem(ScriptDataItem copyfrom)
            {
                Name = (String)copyfrom.Name.Clone();
                Language = (String)copyfrom.Language.Clone();
                Code = (String)copyfrom.Code.Clone();


            }
            public ScriptDataItem(String pName, String pLanguage, String pCode)
            {
                Name=pName;
                Language=pLanguage;
                Code=pCode;


            }
            public ScriptDataItem(SerializationInfo info, StreamingContext context)
            {
                Name = info.GetString("Name");
                Language = info.GetString("Language");
                Code = info.GetString("Code");


            }

            public void GetObjectData(SerializationInfo info, StreamingContext context)
            {
                info.AddValue("Name", Name);
                info.AddValue("Language", Language);
                info.AddValue("Code", Code);

            }

            public void OnDeserialization(object sender)
            {
                //
            }
        }
        #endregion
        [Serializable]
        public class ImageDataItem : ISerializable, IDeserializationCallback,ICloneable ,IXmlPersistable
        {
            public enum ImageDataItemFlagConstants
            {
                Hidden=1,
                Normal=0



            }
            public String Name { get; set; }
            public Image ImageData { get; set; }
            public ImageDataItemFlagConstants Flag { get; set; }
            public ImageDataItem(SerializationInfo info, StreamingContext context)
            {
                
                Name = info.GetString("Name");
                
                ImageData = (Image)info.GetValue("ImageData",typeof(Image));
                try { Flag = (ImageDataItemFlagConstants)info.GetValue("Flag", typeof(ImageDataItemFlagConstants)); }catch{Flag = ImageDataItemFlagConstants.Normal;}



            }
            public Object Clone()
            {
                return new ImageDataItem((String)Name.Clone(), (Image)ImageData.Clone(), Flag);


            }
            public void GetObjectData(SerializationInfo info, StreamingContext context)
            {
                info.AddValue("Name", Name);
                info.AddValue("ImageData", ImageData);


            }
          
            public ImageDataItem(String pName,Image pImageData)
            {
                Name=pName;
                ImageData=pImageData;
                
            }
            public ImageDataItem(String pName, Image pImageData, ImageDataItemFlagConstants pflag)
                : this(pName, pImageData)
            {

                Flag = pflag;

            }

            /// <summary>
            /// obtuse constructor that populates member variables based only on a filename; the name becomes the basename of the file, the extension is the extension,
            /// and the image data is the file contents.
            /// </summary>
            /// <param name="filename">Filename containing Image data</param>
            public ImageDataItem(String filename):this(filename,Path.GetFileNameWithoutExtension(filename))
            {

               


            }
            public ImageDataItem(String filename, String pName)
            {
                Name = pName;
                
                
                using (FileStream readstream = new FileStream(filename, FileMode.Open))
                {
                    if (readstream.Length > int.MaxValue)
                    {
                        throw new ArgumentOutOfRangeException("filename", "Specified Filename \"" + filename + "\" contains too much data.");


                    }


                    ImageData = Image.FromStream(readstream);
                    
                }




            }

           



            public void OnDeserialization(object sender)
            {
                if (BCBlockGameState.Imageman.HasImage(Name))
                {
                    BCBlockGameState.Imageman.Remove(Name);

                }
                //add ours...
                BCBlockGameState.Imageman.AddImage(Name, ImageData);
                
             

            }
            public ImageDataItem(XElement SourceItem)
            {
                //Name attribute...
                String sName = SourceItem.GetAttributeString("Name");
                Image sImage = (Image)SourceItem.ReadElement(typeof(Image), "ImageData", null);
                if(sImage==null || sName==null)
                    throw new InvalidDataException("Name or Image is null loading ImageDataItem");

                Name = sName;
                ImageData = sImage;

            }

            public XElement GetXmlData(string pNodeName)
            {
                XElement buildnode = new XElement(pNodeName);
                buildnode.Add(new XAttribute("Name",Name));
                buildnode.Add(StandardHelper.SaveElement(ImageData,"ImageData"));
                return buildnode;
            }
        }

        /// <summary>
        /// Class used to help manage Sound Data that can be saved with Edited Levelsets.
        /// This is used in two scenarios: one, where the level data is saved, this is responsible for serializing all the Sound data
        /// within the file (thus the Serializable attribute).
        /// Afterwards, it will be deserialized, but there are two cases where that can occur; it could either be
        /// within the editor or it could be in-game. The former may need access to the sound data; the latter will not.
        /// in the latter case, this class does nothing special; however, since the data is unimportant after deserialization 
        /// (since we add the data to the global sound cache) the LevelSetBuilder for Files will command "us" all to purge (I say us, to refer to the list of this type)
        /// And we should OBEY.
        /// 
        /// </summary>
        [Serializable]
        public class SoundDataItem :ISerializable ,IDeserializationCallback,ICloneable
        {

            public String Name { get; set; }
            
            public String FileExtension { get; set; }
            public byte[] SoundData { get; set; }
            private DeletionHelper Tdel;
            public Object Clone()
            {
                //write to a stream and reconstruct.
                MemoryStream ms = new MemoryStream();
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(ms, this);
                ms.Seek(0, SeekOrigin.Begin);
                return bf.Deserialize(ms);



            }

            public SoundDataItem(SerializationInfo info, StreamingContext context)
            {
                Name = info.GetString("Name");
                FileExtension = info.GetString("Extension");
                SoundData = (byte[])info.GetValue("SoundData",typeof(byte[]));


            }
            /// <summary>
            /// direct constructor used to populate this classes members directly.
            /// </summary>
            /// <param name="Name">Name of this sound. (usually, filename without the extension)</param>
            /// <param name="FileExtension">File Extension to be used when loading. For example- "ogg", or "wav"</param>
            /// <param name="SoundData">Byte Array of Sound Data.</param>
            public SoundDataItem(String pName, String pFileExtension, byte[] pSoundData)
            {
                Name=pName;
                FileExtension=pFileExtension;
                SoundData = pSoundData;
            }
            /// <summary>
            /// obtuse constructor that populates member variables based only on a filename; the name becomes the basename of the file, the extension is the extension,
            /// and the sound data is the file contents.
            /// </summary>
            /// <param name="filename">Filename containing Sound data</param>
            public SoundDataItem(String filename)
            {

                Name = Path.GetFileNameWithoutExtension(filename);
                FileExtension = Path.GetExtension(filename);
                //open the file stream and read in all it's data to our byte array.
                using (FileStream readstream = new FileStream(filename, FileMode.Open)) 
                {
                    if (readstream.Length > int.MaxValue)
                    {
                        throw new ArgumentOutOfRangeException("filename", "Specified Filename \"" + filename + "\" contains too much data.");


                    }
                    byte[] readbuffer = new byte[readstream.Length];
                    readstream.Read(readbuffer, 0, (int)readstream.Length);
                    SoundData=readbuffer;

                }


            }
            public SoundDataItem(String filename, String pName)
            {
                Name = pName;
                FileExtension = Path.GetExtension(filename);
                //open the file stream and read in all it's data to our byte array.
                using (FileStream readstream = new FileStream(filename, FileMode.Open))
                {
                    if (readstream.Length > int.MaxValue)
                    {
                        throw new ArgumentOutOfRangeException("filename", "Specified Filename \"" + filename + "\" contains too much data.");


                    }
                    byte[] readbuffer = new byte[readstream.Length];
                    readstream.Read(readbuffer, 0, (int)readstream.Length);
                    SoundData = readbuffer;

                }




            }

            public void GetObjectData(SerializationInfo info,StreamingContext context)
            {
                info.AddValue("Name",Name);
                info.AddValue("Extension",FileExtension);
                info.AddValue("SoundData",SoundData);


            }
            /// <summary>
            /// Purge all sound data. This means the sounds will not be persisted if the parent EditorSet is Serialized.
            /// used by the LevelSetBuilder responsible for Files to restore the memory that would otherwise be used.
            /// </summary>
            public void Purge()
            {
                SoundData = new byte[]{0};


            }


        

            public void OnDeserialization(object sender)
            {
                //task: we have a extension, and a byte array of sound data.
                //reconstruct the file, and then add it to the statically instanced SoundManager.
                if (FileExtension.StartsWith(".")) FileExtension = FileExtension.Substring(1);
                String AcquiredFile = BCBlockGameState.GetTempFile(FileExtension);

                Debug.Print("OnDeserialization of " + this.Name);
                //write our data out to this file.
                FileStream writestream = new FileStream(AcquiredFile, FileMode.Create);
                writestream.Write(SoundData, 0, SoundData.Length);
                writestream.Close();
                //if a sound with the required key already exists, obliterate it. We Assume that this sound
                //is going to be "more important" in that we are either loading the level for a new game or we are opening in the editor and thus
                //the Sound Manager data should be up to date with this.
                BCBlockGameState.AddDeleter(AcquiredFile);
                if (BCBlockGameState.Soundman.HasSound(Name))
                {
                    //the sound exists... remove it.
                    BCBlockGameState.Soundman.RemoveSound(Name);


                }
                //now we can add our sound data...
                try
                {
                    BCBlockGameState.Soundman.AddSound(AcquiredFile, Name);
                    Tdel = new DeletionHelper(AcquiredFile);
                }
                catch(Exception q)
                {
                    Debug.Print("exception + " + q.Message + "occured...");

                }

            }

         
        }
        /// <summary>
        /// Determines whether the object to which this is a composite object in can be used as a template.
        /// </summary>
        public bool isTemplate { get; set; } //wether this can be used as a template. 
        private String _Author = String.Empty;
        private String _Version = "0.0.0.0";
        private String _Comment = String.Empty;
        private String _Description = "<Not Set>";
        public String Author { get { return _Author; } set { _Author = value; } }
        public String Version { get { return _Version; } set { _Version = value; } }
        public String Comment { get { return _Comment; } set { _Comment = value; } }
        public String Description { get { return _Description; } set { _Description = value; } }
        private Dictionary<String, SoundDataItem> _SavedSounds;
        private Dictionary<String, ScriptDataItem> _SavedScripts;
        public Dictionary<String, ImageDataItem> SavedImages { get; set; }

        [Editor(typeof(CreatorProperties.ScriptDataItemListEditor), typeof(UITypeEditor))]
        public Dictionary<String, ScriptDataItem> SavedScripts { get { return _SavedScripts; } set { _SavedScripts = value; } }

            [Editor(typeof(SoundDataItemListEditor),typeof(UITypeEditor))]
        public Dictionary<String, SoundDataItem> SavedSounds { get { return _SavedSounds; }  set { _SavedSounds = value; }  }

        /// <summary>
        /// adds the properties in the given CreatorProperties object with this one.
        /// Images and sounds are added and replaced.
        /// </summary>
        /// <param name="otherprops"></param>
            public void MergeWith(CreatorProperties otherprops)
            {
                foreach (var iterate in otherprops.SavedImages)
                {
                    if (!SavedSounds.ContainsKey(iterate.Key))
                        SavedImages.Add(iterate.Key, iterate.Value);
                    else
                        SavedImages[iterate.Key] = iterate.Value;
                        
                    

                }
                foreach (var iterate in otherprops.SavedSounds)
                {
                    if(!SavedSounds.ContainsKey(iterate.Key))
                        SavedSounds.Add(iterate.Key, iterate.Value);
                    else
                        SavedSounds[iterate.Key] = iterate.Value;
                        
                    

                }


            }
        
        public CreatorProperties(String pAuthor, String pVersion, String pComment)
        {

            Author = pAuthor;
            Version = pVersion;
            Comment=pComment;
            SavedImages = new Dictionary<string, ImageDataItem>();
            SavedSounds = new Dictionary<string, SoundDataItem>();
            SavedScripts = new Dictionary<string, ScriptDataItem>();

        }
        public Object Clone()
        {
            return new CreatorProperties(this);



        }
        public CreatorProperties(CreatorProperties clonethis)
        {
            Author = clonethis.Author;
            Version = clonethis.Version;
            Comment = clonethis.Version;
            SavedImages = new Dictionary<string, ImageDataItem>();
            foreach (var iterate in clonethis.SavedImages)
            {
                SavedImages.Add(iterate.Key, (ImageDataItem)iterate.Value.Clone());

            }
            SavedSounds = new Dictionary<string, SoundDataItem>();
            foreach (var iterate in clonethis.SavedSounds)
            {
                SavedSounds.Add(iterate.Key, (SoundDataItem)iterate.Value.Clone());

            }
            SavedScripts = new Dictionary<string, ScriptDataItem>();
            foreach (var iterate in clonethis.SavedScripts)
            {
                SavedScripts.Add(iterate.Key, (ScriptDataItem)iterate.Value.Clone());


            }

        }
        public CreatorProperties(SerializationInfo info, StreamingContext context)
        {
            Author = info.GetString("Author");
            Version = info.GetString("Version");
            Comment = info.GetString("Comment");
            SavedImages =
                (Dictionary<String, ImageDataItem>)
                info.GetValue("SavedImages", typeof (Dictionary<String, ImageDataItem>));
            Debug.Print("CreatorProperties: Loaded " + SavedImages.Count.ToString() + " Images...");
            _SavedSounds =
                (Dictionary<String, SoundDataItem>)
                info.GetValue("SavedSounds", typeof (Dictionary<String, SoundDataItem>));
            try
            {
                _SavedScripts = (Dictionary<String, ScriptDataItem>)info.GetValue("SavedScripts", typeof(Dictionary<String, ScriptDataItem>));



            }
            catch
            {
                _SavedScripts = new Dictionary<string, ScriptDataItem>(); //probably a levelset/data from an earlier version...

            }
            try
            {
                Description = info.GetString("Description");
            }
            catch
            {
                Description = "";
            }
            try
            {
                isTemplate = info.GetBoolean("isTemplate");
            }
            catch { isTemplate = false; }
        
    }
        public CreatorProperties(XElement Source)
        {
            //retrieve Attributes...
            Author = Source.GetAttributeString("Author", String.Empty);
            Version = Source.GetAttributeString("Version", String.Empty);
            Comment = Source.GetAttributeString("Comment", String.Empty);
            isTemplate = Source.GetAttributeBool("isTemplate", false);
            Description = Source.GetAttributeString("Description", Description);
            SavedImages = Source.ReadDictionary("SavedImages", new Dictionary<String, ImageDataItem>());
            SavedSounds = Source.ReadDictionary("SavedSounds", new Dictionary<String, SoundDataItem>());
            SavedScripts = Source.ReadDictionary("SavedScripts", new Dictionary<String, ScriptDataItem>());

        }
        public XElement GetXmlData(string pNodeName)
        {
            XElement resultnode = new XElement(pNodeName);
            resultnode.Add(new XAttribute("Author",Author));
            resultnode.Add(new XAttribute("Version",Version));
            resultnode.Add(new XAttribute("Comment",Comment));
            resultnode.Add(new XAttribute("isTemplate",isTemplate));
            resultnode.Add(new XAttribute("Description",Description));
            
            resultnode.Add(StandardHelper.SaveDictionary(SavedImages,"SavedImages"));
            resultnode.Add(StandardHelper.SaveDictionary(_SavedSounds,"SavedSounds"));
            resultnode.Add(StandardHelper.SaveDictionary(_SavedScripts,"SavedScripts"));

            return resultnode;
        }
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Author", Author);
            info.AddValue("Version", Version);
            info.AddValue("Comment", Comment);
            info.AddValue("isTemplate", isTemplate);
            
            


            info.AddValue("SavedImages", SavedImages);
            Debug.Print("CreatorProperties: Saved " + SavedImages.Count.ToString() + " Images...");
            info.AddValue("SavedSounds", _SavedSounds);
            info.AddValue("SavedScripts", _SavedScripts);
            info.AddValue("Description", Description);
        }

        #region IDeserializationCallback Members

        public void OnDeserialization(object sender)
        {
            //make sure all the images in our dictionary are in the gamestate object.
            //this isn't necessary for the sounds, since the SoundDataItem manages this itself.
            /*
            foreach (KeyValuePair<String, ImageDataItem> kvp in SavedImages)
            {

                //if present, remove the existing item and replace it with this one (hello "texture packs"?)
                if (BCBlockGameState.Imageman.Exists(kvp.Key))
                {
                    //remove it...
                    BCBlockGameState.Imageman.Remove(kvp.Key);



                }
                BCBlockGameState.Imageman.AddImage(kvp.Key, kvp.Value);
            }
             * */
            //commented out the above: should now be performed by OnDeSerialization in the ImageDataItem.
        }

        #endregion

      
    }
    /// <summary>
    /// SingleLevelContainer: contains some attributes much like the EditorSet, but for a Single Level File.
    /// </summary>

    [Serializable]
    public class SingleLevelContainer : ISerializable
    {
        public CreatorProperties CreateData;
        public Level LevelData;
        public SingleLevelContainer(SerializationInfo info, StreamingContext context)
        {
            CreateData = (CreatorProperties)info.GetValue("CreateData", typeof(CreatorProperties));
            LevelData = (Level)info.GetValue("LevelData", typeof(Level));


        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            
        }
    }

    /// <summary>
    /// EditorSet: this is the Serializable class that actually get's saved to the BLF files (or will, as I write this).
    /// Basically, a LevelSet stores the information about the levels; this will store any extra data about those levels, mostly relevant to user inspection
    /// and/or the editor. For example- Author name.
    /// </summary>
    [Serializable]
    public class EditorSet : ISerializable,IXmlPersistable
    {
        public const long HeaderMagicNumber=0x8a5381033;
        public const long HeaderIndexedMagicNumber = 0x346475467;
            private long _MagicNumber=HeaderMagicNumber;
        [Browsable(false)]
            public long MagicNumber { get { return _MagicNumber; } set { _MagicNumber = value; } }



        
        //public String Author { get; set; }
        //public String Version { get; set; }
        public String Author
        {
            get { return CreateData.Author; }


            set { CreateData.Author = value;}



        }
        public String Version
        {
            get
            {
                return CreateData.Version;


            }
            set {CreateData.Version = value; }

        }
        [Editor(typeof(CreatorPropertiesEditor), typeof(UITypeEditor))]
            public CreatorProperties CreateData { get; set; }
        
        public LevelSet LevelData { get; set; }

        public EditorSet()
        {
            CreateData = new CreatorProperties("No Author Specified", "0.0.0", "No Comment");
           
            LevelData = new LevelSet();

        }

        public EditorSet(SerializationInfo info, StreamingContext context)
        {
            Debug.Print("EditorSet Serialization constructor");
            try
            {
                MagicNumber = info.GetInt64("Magic Number");
                VersionInfo grabversion;
                try { grabversion = (VersionInfo)info.GetValue("Version", typeof(VersionInfo)); }
                catch { grabversion = VersionInfo.GetApplicationVersion(); }
                if (grabversion > VersionInfo.GetApplicationVersion())
                {
                    Debug.Print("Version of game for saved file is newer than this version.");
                    Debug.Print("This version:" + VersionInfo.GetApplicationVersion() + ", file version:" + grabversion);

                }


                CreateData = (CreatorProperties)info.GetValue("CreateData", typeof(CreatorProperties));
                LevelData = (LevelSet)info.GetValue("LevelData", typeof(LevelSet));
            }
            catch (Exception serialexception)
            {
                Debug.Print("Exception: " + serialexception.Message + " stack:" + serialexception.StackTrace);


            }

        }
 
        public EditorSet(XElement Source)
        {
            String sVersion = Source.GetAttributeString("Version");
            CreateData = Source.ReadElement<CreatorProperties>("CreatorProperties");
            LevelData = Source.ReadElement<LevelSet>("LevelSet");
            
        }
        public XElement GetXmlData(string pNodeName)
        {
            XElement newNode = new XElement(pNodeName);
            newNode.Add(new XAttribute("Version",VersionInfo.GetApplicationVersion()));
            RefreshPreviewImages();
            newNode.Add(CreateData.GetXmlData("CreatorProperties"));
            newNode.Add(LevelData.GetXmlData("LevelSet"));
            //newNode.Add(LevelData.GetXmlData("LevelData"));
            return newNode;
            
        }
        public void GetObjectData(SerializationInfo info,StreamingContext context)
        {
            
            info.AddValue("Magic Number", HeaderMagicNumber);

            //save version info too.
            info.AddValue("Version", VersionInfo.GetApplicationVersion());
            try
            {
                RefreshPreviewImages();
            }
            catch (SerializationException se)
            {
                Debug.Print("SerializationException caught");

            }
            info.AddValue("CreateData", CreateData);
            info.AddValue("LevelData", LevelData);
        }

        private void RefreshPreviewImages()
        {
            //make sure there is a image for each level, too. These can be used later, by being indexed
            //first, remove any images that start with '$LevelImage$'

            //remove all keys that start with levelimageprefix...

            //BUGFIX: this foreach was erroring out, due to modifying the dictionary while iterating over it, after the initial save.
            
          
            String builtname = "";
            const string levelimageprefix = "$LevelImage$";
            int i=0;
            List<String> removethese = new List<string>();
            foreach (var loopremove in from p in CreateData.SavedImages.Keys where p.StartsWith(levelimageprefix) select p)
            {
                removethese.Add(loopremove);
            }
            foreach (String removekey in removethese)
            {
                CreateData.SavedImages.Remove(removekey);
            }

            List<String> removekeys = new List<string>();
            while (CreateData.SavedImages.ContainsKey(builtname))
            {
                builtname = levelimageprefix + i;
                removekeys.Add(builtname);
                i++;
            }
            foreach (String removethis in removekeys)
            {
                CreateData.SavedImages.Remove(removethis);
            }
            i = 0;
            foreach (var looplevel in LevelData.Levels)
            {
                //add image as "levelimageprefix##" where number is the level number.
                //set it to be hidden so it doesn't show up (normally) in the image viewer portion of the editor
                Image addimage = BCBlockGameState.DrawLevelToImage(looplevel);
                String addkey = levelimageprefix + i;
                CreateData.SavedImages.Add(addkey, new CreatorProperties.ImageDataItem(addkey, addimage, CreatorProperties.ImageDataItem.ImageDataItemFlagConstants.Hidden));
                looplevel.PreviewImageKey = addkey;
                i++;
            }
        }


        public static EditorSet FromFile(String filename)
        {
            return FromFile(filename, null);

        }
        public static EditorSet FromStream(Stream inputstream, StreamProgress.ProgressFunction pcfunc)
        {

           
            string tempfile = BCBlockGameState.GetTempFile(".BLF");
           
            FileStream writeto = new FileStream(tempfile, FileMode.Create);
            byte[] membytes = new byte[inputstream.Length-inputstream.Position];
            inputstream.Read(membytes, 0, membytes.Length);
            writeto.Write(membytes, 0, membytes.Length);
            writeto.Close();
            EditorSet readset = FromFile(tempfile, pcfunc);
            File.Delete(tempfile);
            return readset;
        }

        public static EditorSet FromFile(String filename,StreamProgress.ProgressFunction pcfunc)
        {
            Debug.Print("loading EditorSet from file:" + filename);
            if (filename == null) throw new ArgumentException("filename cannot be null");
            
            
            
            //System.Web.Script.Serialization.JavaScriptSerializer oSerializer
            using (FileStream inputstream = new FileStream(filename, FileMode.Open))
            {
                FileStream pcs = inputstream;

                if (Path.GetExtension(filename).ToUpper() == ".BLF")
                {
                    IFormatter useformatter = BCBlockGameState.getFormatter<EditorSet>(BCBlockGameState.DataSaveFormats.Format_Binary);
                    bool donocompressheader = false; 
                    Stream acquirefrom = null;
                    using (StreamProgress sp = new StreamProgress(pcs, pcfunc))
                    {
                    tryagain:
                        try
                        {

                            //read in a bool. this determines whether it is compressed or not.
                            //bool readbool = new BinaryReader(inputstream).ReadBoolean();
                            if (!donocompressheader)
                            {
                                CompressionTypeConstants compression = (CompressionTypeConstants)inputstream.ReadByte();
                                Debug.Print("EditSet::FromFile- file compression header:" + compression);
                                switch (compression)
                                {
                                    case CompressionTypeConstants.Compression_GZip:
                                        acquirefrom = new GZipStream(inputstream, CompressionMode.Decompress);
                                        break;
                                    case CompressionTypeConstants.Compression_Deflate:
                                        acquirefrom = new DeflateStream(inputstream, CompressionMode.Decompress);
                                        break;
                                    default:
                                        acquirefrom = inputstream;
                                        break;
                                }
                            }
                            else
                            {
                                acquirefrom = inputstream;
                                inputstream.Seek(0, SeekOrigin.Begin);
                            }


                            using (acquirefrom)
                            {
                                EditorSet returnthis = (EditorSet)useformatter.Deserialize(acquirefrom);
                                if (returnthis.CreateData.SavedSounds != null)
                                    Debug.Print("returning EditorSet that contains " +
                                                returnthis.CreateData.SavedSounds.Count.ToString() + " Sounds.");
                                return returnthis;
                            }

                        }
                        catch (SerializationException se)
                        {
                            Trace.WriteLine("SerializationException:" + se.ToString());
                            donocompressheader = true;
                            goto tryagain;

                        }
                        finally
                        {
                            if (acquirefrom != null) acquirefrom.Close();
                        }
                    }
                    }
                else if(Path.GetExtension(filename).ToUpper() == ".XBLF")
                {
                    //uses CustomXmlFormatter to deserialize, and no compression.
                    using (StreamProgress sp = new StreamProgress(pcs, pcfunc))
                    {
                        XDocument loaddocument = XDocument.Load(inputstream);
                        EditorSet returnthis = new EditorSet(loaddocument.Root);
                        return returnthis;
                        /*IFormatter useformatter = BCBlockGameState.getFormatter<EditorSet>(BCBlockGameState.DataSaveFormats.Format_XML);
                        EditorSet returnthis = (EditorSet)useformatter.Deserialize(inputstream);
                        if (returnthis.CreateData.SavedSounds != null)
                            Debug.Print("returning EditorSet that contains " +
                                        returnthis.CreateData.SavedSounds.Count.ToString() + " Sounds.");
                        return returnthis;*/
                    }
                }
                    else
                    {
                        throw new Exception("Unsupported File Extension, \"" + Path.GetExtension(filename) + "\"");
                    }
                
                }
            
            return null;
            }




        
        

        public enum CompressionTypeConstants
        {
            Compression_None,
            Compression_GZip,
            Compression_Deflate 

        }
 
        public void Save(String filename,StreamProgress.ProgressFunction pfunction)
        {
            String getextension = Path.GetExtension(filename);
            SaveFormatInfo sfi = null;
            if(getextension.Equals(".XBLF",StringComparison.OrdinalIgnoreCase))
            {
                sfi = new XmlSaveFormatInfo();
            }
            else
            {
                sfi = new BinarySaveFormatInfo(); 
            }
            Save(filename,sfi,pfunction);
        }
        public void Save(String sTarget,SaveFormatInfo Format,StreamProgress.ProgressFunction pfunction)
        {
            //if(!File.Exists(sTarget)) throw new FileNotFoundException(sTarget);
            using (FileStream fs = new FileStream(sTarget, FileMode.Create))
            {
                Save(fs, Format, pfunction);
            }
            
        }
        public void Save(Stream Target,SaveFormatInfo Format,StreamProgress.ProgressFunction pfunction)
        {

            SerializationInfo sinfo;
            try
            {

                Stream outstream = Target;
                {
                    using (StreamProgress sp = new StreamProgress(outstream, pfunction))
                    {
                        if (Format is BinarySaveFormatInfo)
                        {
                            BinarySaveFormatInfo BinInfo = Format as BinarySaveFormatInfo;
                            //write out whether this is compressed.
                            IFormatter binformatter = BCBlockGameState.getFormatter<EditorSet>(BCBlockGameState.DataSaveFormats.Format_Binary);
                            outstream.WriteByte((byte)(BinInfo.CompressionType != CompressionTypeConstants.Compression_None ? 1 : 0));
                            Stream targetstream;
                            switch (BinInfo.CompressionType)
                            {
                            case CompressionTypeConstants.Compression_GZip:
                                targetstream = new GZipStream(outstream, CompressionMode.Compress);
                                break;
                            case CompressionTypeConstants.Compression_Deflate:
                                targetstream = new DeflateStream(outstream, CompressionMode.Compress, CompressionLevel.BestCompression);
                                break;
                            default:
                                targetstream = outstream;
                                break;
                            }
                            using (targetstream)
                            {
                                Cursor.Current = Cursors.WaitCursor;
                                binformatter.Serialize(targetstream, this);
                                targetstream.Close();
                                Cursor.Current = Cursors.Default;
                            }
                        }
                        else if(Format is XmlSaveFormatInfo)
                        {
                            Cursor.Current = Cursors.WaitCursor;
                            //almost forgot to change this code to save to XML properly. the SOAP stuff, we have to remove at some stage...
                            XElement BuildElement = this.GetXmlData("EditorSet");
                            XDocument buildDocument = new XDocument(BuildElement);

                            buildDocument.Save(outstream,SaveOptions.OmitDuplicateNamespaces);


                            /*IFormatter xmlformat = BCBlockGameState.getFormatter<EditorSet>(BCBlockGameState.DataSaveFormats.Format_XML);
                            Cursor.Current = Cursors.WaitCursor;
                            xmlformat.Serialize(outstream,this);*/
                        }

                    }
                }

                
            }
            catch (Exception e)
            {
                Debug.Print("Exception in EditorSet.Save:" + e.Message);



            }
            finally
            {
                Cursor.Current = Cursors.Default;
            }








        }

     
    }
}
