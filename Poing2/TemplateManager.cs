using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace BASeBlock.Templates 
{
    //class that manages Templates.
    //templates are nothing more than .blf data files in a specific directory. These can be used to initialize a new level.
    //a iLevelSetBuilder implementation, similar to the "from file" builder, but instead presenting a template dialog, uses this data.
    //we don't keep all the template data in memory, only the information we need to populate the listing.

    //we actually have two kinds of templates: Level<SETS> and plain levels.
    //both are loaded from the .blf files in the template folder.

    public class TemplateManager
    {
        public class TemplateSetData
        {
            private String _Name = "TEMPLATE";
            private String _Description = "DESCRIPTION";
            private String _Comment = "COMMENT";
            private String _SourceFile;
            private String _Author;
            private bool _isTemplate;
            private List<TemplateLevelData> _LevelTemplates = new List<TemplateLevelData>();
            private EditorSet SetObject = null;
            public String Name { get { return _Name; } set { _Name = value; } }
            public String Description { get { return _Description; } set { _Description = value; } }
            public String Comment { get { return _Comment; } set { _Comment = value; } }
            public String SourceFile { get { return _SourceFile; } set { _SourceFile = value; } }
            public String Author { get { return _Author; } set { _Author = value; } }
            public bool isTemplate { get { return _isTemplate; } set { _isTemplate = value; } }
            public List<TemplateLevelData> Leveltemplates { get { return _LevelTemplates; } set { _LevelTemplates = value; } }


            public CreatorProperties CreationData { get { return (CreatorProperties)SetObject.CreateData.Clone(); } }
            public LevelSet CloneSet()
            {
                MemoryStream serializehere = new MemoryStream();
                BinaryFormatter bff = new BinaryFormatter();
                bff.Serialize(serializehere, SetObject);
                serializehere.Seek(0, SeekOrigin.Begin);

                EditorSet cloned = EditorSet.FromStream(serializehere, null);
                return cloned.LevelData;



            }
            public TemplateSetData(String pSourceFile)
            {
                _SourceFile = pSourceFile;




                //read SetObject from the file, re-throw any exceptions.
                try
                {

                    SetObject = EditorSet.FromFile(SourceFile);





                }
                catch (Exception ex)
                {
                    throw;
                }

                //assume success reading the file.
                //now populate our data.
                _Name = SetObject.LevelData.SetName;
                _Description = SetObject.CreateData.Description;
                _Author = SetObject.Author;
                _Comment = SetObject.CreateData.Comment;
                _isTemplate = SetObject.CreateData.isTemplate;
                //iterate and create a TemplateLevelData for each.
                foreach (Level leveliterate in SetObject.LevelData.Levels)
                {

                    _LevelTemplates.Add(new TemplateLevelData(leveliterate, this));

                }
            }


        }
        public class TemplateLevelData
        {
            private String _Name = "LEVEL NAME";
            private String _Description = "";
            private TemplateSetData _SetTemplate = null;
            private Level _LevelObject = null;
            public String Name { get { return _Name; } set { _Name = value; } }
            public String Description { get { return _Description; } set { _Description = value; } }
            public Level LevelData { get { return _LevelObject; } set { _LevelObject = value; } }
            public TemplateSetData SetOwner { get { return _SetTemplate; } set { _SetTemplate = value; } }
            public TemplateLevelData(Level FromLevel, TemplateSetData Owner)
            {
                _Name = FromLevel.LevelName;
                _Description = FromLevel.Description;
                _SetTemplate = Owner;
                _LevelObject = FromLevel;

            }
            public Level CloneLevelData()
            {

                MemoryStream serializehere = new MemoryStream();
                BinaryFormatter bff = new BinaryFormatter();
                bff.Serialize(serializehere, _LevelObject);
                serializehere.Seek(0, SeekOrigin.Begin);

                Level cloned = (Level)bff.Deserialize(serializehere);
                return cloned;


            }


        }

        private Dictionary<String, TemplateSetData> LoadedTemplates = new Dictionary<string, TemplateSetData>();

        //enumerator that goes through all the TemplateSetData classes and retrieves the LevelTemplateData's from each.
        public IEnumerable<TemplateLevelData> LevelTemplates()
        {
            foreach (var iterate in LoadedTemplates)
            {

                foreach (var looptemplate in iterate.Value.Leveltemplates)
                    yield return looptemplate;

            }


        }
        public IEnumerable<TemplateSetData> SetTemplates()
        {
            foreach (var iterate in LoadedTemplates)
            {

                if (iterate.Value.isTemplate)
                    yield return iterate.Value;


            }



        }
        public TemplateManager(IEnumerable<String> Directories, iManagerCallback callback)
            : this((from p in Directories select new DirectoryInfo(p)), callback)
        {



        }
        public TemplateManager(IEnumerable<DirectoryInfo> Dirinfos, iManagerCallback callback)
        {
            foreach (DirectoryInfo loopdir in Dirinfos)
            {
                LoadTemplates(loopdir, callback);


            }


        }

        public void LoadTemplates(String Directory, iManagerCallback callback)
        {
            LoadTemplates(new DirectoryInfo(Directory), callback);
        }
        public void LoadTemplates(DirectoryInfo Directory, iManagerCallback callback)
        {

            //load all .blf files from the given folder.
            DirectoryInfo di = Directory;
            callback.ShowMessage("Loading Templates from " + Directory);
            //iterate...
            foreach (FileInfo blffile in di.GetFiles("*.blf"))
            {
                try
                {
                    TemplateSetData tsd = new TemplateSetData(blffile.FullName);
                    LoadedTemplates.Add(tsd.Name, tsd);
                }
                catch (Exception exx)
                {
                    callback.ShowMessage("Failed to load template data from " + blffile.FullName + " Exception:" + exx);
                }



            }


        }



    }

}
