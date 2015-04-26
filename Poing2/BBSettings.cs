﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using BASeCamp.Configuration;

namespace BASeBlock
{
    public class BBSettings
    {
        private readonly INIFile _gameSettings = null;
        //BaseBlock Settings
        public BBSettings(INIFile settingsFile)
        {
            _gameSettings = settingsFile;


        }
        public List<String> SoundFolder
        {
            get { return GetDataFolder("sound"); }
            set { 
            SetDataFolder("sound",value);
            
            }


        }
        public String IgnoreAssemblies { 
            get{
                return _gameSettings["ignore"]["assemblies"].Value;    
        
            }
            set
            {

                _gameSettings["ignore"]["assemblies"].Value = value;
            }
    }
        public String IncludeAssemblies
        {
            get { return _gameSettings["include"]["assemblies"].Value; }
            set { _gameSettings["include"]["assemblies"].Value = value; }

        }
        public String SoundEngine
        {
            get
            {
                return _gameSettings["game"]["SoundEngine"].Value;
            }
            set { _gameSettings["game"]["SoundEngine"].Value = value; }
        }

        public string DefaultEditorFilename
        {
            get
            {

                return Path.Combine(BCBlockGameState.AppDataFolder, "default.blf");

            }
        }
            
        
        public String DefaultEditorSet
        {
            get {

                //some checks to try to find the template file...

                String gotfilename = _gameSettings["Editor"]["Template", ""].Value;
                if (gotfilename == "") return "";
                if(File.Exists(gotfilename)) return gotfilename;
                //if the file doesn't exist, try plonking the datafolder  for levels.
                List<String> levelpaths = LevelsFolder;
                String defaultfolder = BCBlockGameState.AppDataFolder;

                string testappfolder = Path.Combine(defaultfolder, gotfilename);
                if(File.Exists(testappfolder)) return testappfolder;
                foreach (String checkpath in levelpaths)
                {
                    String testappend = Path.Combine(checkpath, gotfilename);
                    //if it exists, return it.
                    if (File.Exists(testappend)) return testappend;


                }

                return ""; //if nothing checks out, return an empty string.


            }
            set { _gameSettings["Editor"]["Template"].Value = value; }

        }
    
        private static readonly Dictionary<String, List<String>> CachedFolders = new Dictionary<string, List<string>>(); 
        private List<String> xGetDataFolder(String id)
        {
            if (!CachedFolders.ContainsKey(id))
                CachedFolders.Add(id,
                    BCBlockGameState.CanonicalizePath(_gameSettings["folders"][id].getArrayValue(), BCBlockGameState.AppDataFolder).ToList());

            return CachedFolders[id];


            
            
        }
        private readonly Dictionary<String, String> _Zippwd = null; 
        public Dictionary<String, String> ZipPasswords
        {
            get
            {
                if (_Zippwd == null)
                {
                    //examine [zippassword] section.
                    //filename.zip="<password>"
                    foreach (var iterate in _gameSettings["ZipPasswords"].getValues())
                    {
                        _Zippwd.Add(iterate.Name, iterate.Value);
                        
                    }


                }
                return _Zippwd;
            }



        }
        public String StatisticsFile
        {
            get
            {
                String defaultstatfolder = Path.Combine(BCBlockGameState.AppDataFolder,"statistics");
                String sfolder = BCBlockGameState.CanonicalizePath(_gameSettings["folders"]["statistics"].GetValue(defaultstatfolder), BCBlockGameState.AppDataFolder);
                return Path.Combine(sfolder, "Stats.dat");

            }

        }
        // CanonicalizePath(GameSettings["folders"]["sound"].getArrayValue(),AppDataFolder).ToList();
        
        public List<String> SoundFolders
        {
            get
            {
                return xGetDataFolder("sounds");

            }

        }
        public List<String> ImageFolders
        {
            get
            {
                return xGetDataFolder("images");

            }


        }
        public List<String> LevelFolders
        {

            get
            {
                return xGetDataFolder("levels");
            }

        }
        public List<String> PluginFolders
        {
            get
            {
                return xGetDataFolder("plugins");
            }

        }
        public List<String> ScriptFolders
        {

            get
            {
                return xGetDataFolder("Scripts");
            }

        }
        public List<String> ToolbarImageFolders
        {

            get
            {
                return xGetDataFolder("toolbarimages");

            }
        }
        public List<String> TemplateFolders
        {

            get
            {
                return xGetDataFolder("Templates");
            }
        }
        public String SoundPluginFolders
        {
            get
            {
                return _gameSettings["Folders"]["SoundPlugins"].Value;

            }

        }
//"templates"
        public List<String> GetRecentFilesList(String id)
        {
            /*
             * foreach (var loopitem in GameSettings["Recent." + id].getValues())
            {
                createlist.Add(loopitem.Value); //name isn't important.



            }
            return createlist;
             * */

            return _gameSettings["Recent." + id].getValues().Select(loopitem => loopitem.Value).ToList();
        }

        /// <summary>
        /// adds an item to the specified recent item list.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="filename"></param>
        public void AddToRecentList(String id,String filename)
        {
            var usesection=_gameSettings["Recent." + id];
            usesection["item" + usesection.INIItems.Count.ToString()].Value = filename;

            
        }




        //           List<String> Imagefolders = new String[] {GameSettings["folders"]["image"].Value}.ToList();
        //List<String> LevelFolders = new String[] {GameSettings["folders"]["levels"].Value}.ToList();
        //List<String> PluginFolders = new String[] {GameSettings["folders"]["plugins"].Value}.ToList();
        public List<String> ImageFolder
        {
            

            get { return GetDataFolder("image"); }
            set { 
            SetDataFolder("image",value);
            
            }


        }
        public List<String> LevelsFolder
        {


            get { return GetDataFolder("levels"); }
            set
            {
                SetDataFolder("levels", value);

            }


        }
        public float ParticleGenerationFactor
        {
            get { return _gameSettings["game"]["ParticleGenerationFactor", "0.5"].GetValue(0.5f); }
            set { _gameSettings["game"]["ParticleGenerationFactor", "0.5"].SetValue(value); }

        }
       
        public List<String> PluginsFolder
        {


            get { return GetDataFolder("plugins"); }
            set
            {
                SetDataFolder("plugins", value);

            }


        }

        public Color EditorBlockHighlightColor
        {
            get
            {
                return Color.FromArgb(Int32.Parse(_gameSettings["Editor"]["BlockHighlight", Color.Blue.ToArgb().ToString()].Value));
            
            
            
            }
            set { _gameSettings["Editor"]["BlockHighlight"].Value = value.ToArgb().ToString(); }



        }


        public String PlayerName
        {
            get {
                return _gameSettings["game"]["PlayerName", BCBlockGameState.CurrentUserName].Value;
            
            }
            set { 
            
            _gameSettings["game"]["PlayerName"].Value = value;
            }


        }

        public bool ShowDebugInfo
        {
            get { return bool.Parse(_gameSettings["game"]["Debug", bool.TrueString].Value); }
            set { _gameSettings["game"]["Debug"].Value=value.ToString(); }
        }
        public bool WaterBlockAnimations
        {

            get { return bool.Parse(_gameSettings["game"]["WaterBlockAnimations", "true"].Value);}
            set { _gameSettings["game"]["WaterBlockAnimations"].Value = value.ToString();}

        }
        public float ReduceQualityAmount
        {
            get { return Single.Parse(_gameSettings["game"]["ReduceQualityAmount", "1"].Value); }
            set { _gameSettings["game"]["ReduceQualityAmount"].Value = value.ToString(); }
        }
        private readonly string DefaultMonospace = "Consolas,Courier New,Liberation Mono,Liberation Sans";
        public IEnumerable<String> MonospaceFonts
        {
            get
            {
                

                //list of candidates...
                return _gameSettings["game"]["monospace", DefaultMonospace].Value.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

            }

            set
            {
                _gameSettings["game"]["monospace"].Value = String.Join(",", value);

            }
        }

        public int EditorUndoStackSize
        {
            get { return _gameSettings["editor"]["MaxUndoStack"].GetValue(10); }
            set { _gameSettings["editor"]["MaxUndoStack"].SetValue(value); }
        }


        public List<String> GetDataFolder(String foldername)
        {

            return _gameSettings["folders"][foldername].Value.Split(new string[] { "/:" }, StringSplitOptions.RemoveEmptyEntries).ToList();


        }
        public void SetDataFolder(String foldername,List<String> folders)
        {
            _gameSettings["folders"][foldername].Value = String.Join("/:", folders.ToArray());


        }



    }
}
