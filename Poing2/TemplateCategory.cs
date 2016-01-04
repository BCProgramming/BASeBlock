using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using OfficePickers.ColorPicker;

namespace BASeCamp.BASeBlock.Templates
{

    /// <summary>
    /// base class for all TemplateCategories.
    /// 
    /// </summary>


    public abstract class TemplateCategory
    {
        //ALL TemplateCategory classes must implement this static method.
        //it cannot be enforced, but the TemplateChooser will ignore  those that do not so be warned.
        //public static TemplateCategory[] getTemplateClasses() { return null; }

        //the TemplateChooser doesn't instantiate these classes directly, instead it calls the type's getTemplateClasses routine to do so.
        //if it doesn't have one, it ignores that category entirely.

        public static IEnumerable<Type> getAllCategoryTypes()
        {


            return BCBlockGameState.MTypeManager[typeof(TemplateCategory)].ManagedTypes;
            

        }
        public static IEnumerable<TemplateCategory> GetAllCategories()
        {


            foreach (Type looptype in getAllCategoryTypes())
            {
                IEnumerable<TemplateCategory> Grabenum = GetCategoryList(looptype);
                if (Grabenum != null)
                {

                    foreach (var iteratereturn in Grabenum)
                    {

                        yield return iteratereturn;

                    }


                }


            }


        }
        public static IEnumerable<TemplateCategory> GetCategoryList(Type SourceCategoryType)
        {

            if (!SourceCategoryType.IsSubclassOf(typeof(TemplateCategory)))
                throw new ArgumentException("SourceCategoryType must be a subclass of BASeBlock.Templates.TemplateCategory");

            //call the static method.
            //first, go through the methodinfos. 
            MethodInfo usemethod = null;
            foreach (MethodInfo iteratemethod in SourceCategoryType.GetMethods())
            {
                if (iteratemethod.Name.Equals("getTemplateClasses", StringComparison.OrdinalIgnoreCase)) 
                {
                    usemethod = iteratemethod;
                    break;
                }


            }
            if (usemethod != null)
            {
                Object result = usemethod.Invoke(null, BindingFlags.Static, null, null, null);
                return (IEnumerable<TemplateCategory>)result;


            }

            return null;
        }
        public delegate void TemplateCategoryCallback(TemplateCategory categoryObject,ToolStripDropDown dropdownobject);
        public delegate void TemplateItemCallback(TemplateManager.TemplateLevelData TemplateItem,ToolStripMenuItem dropdownobject);
        //static method used to "automagically" fill a drop down with categories which themselves will "automagically" populate
        //with the Templates in each category.
        public static void CategoryClick(Object Sender, EventArgs e)
        {


        }
        public static void PopulateDropdown(ToolStripDropDown tdropdown, bool DoClear,TemplateCategoryCallback CategoryCallback,TemplateItemCallback itemcallback)
        {
            if (DoClear) tdropdown.Items.Clear();
            //should be called in the Dropdown event for tdropdown.
            //add one item for each category.
            foreach (TemplateCategory tc in GetAllCategories())
            {
                //create a new dropdown item...
                if (CategoryCallback != null) CategoryCallback(tc, tdropdown);
                ToolStripDropDownItem tsdd = new ToolStripMenuItem(tc.Name, tc.getCategoryImage(), CategoryClick);
                tdropdown.Items.Add(tsdd);
                //add ghost item.
                tsdd.Tag = new object[]{tc,CategoryCallback,itemcallback};
                tsdd.DropDownItems.Add("GHOST");
                tsdd.DropDownOpening += new EventHandler(tsdd_DropDownOpening);

                
                
                
            }


        }

        static void tsdd_DropDownOpening(object sender, EventArgs e)
        {
            //fired when a category item is opening. Here we need to take sender,
            //which should be a ToolStripDropDownItem, and add drop down elements
            //as required.

            ToolStripDropDownItem thisitem = sender as ToolStripDropDownItem;
            if (thisitem == null) return;
            Object[] casttag = (Object[])thisitem.Tag;
            TemplateCategory tc = casttag[0] as TemplateCategory;
            TemplateCategoryCallback templatecback = (TemplateCategoryCallback)casttag[1];
            TemplateItemCallback itemcback = (TemplateItemCallback)casttag[2];
            
            //clear this dropdown.
            thisitem.DropDown.Items.Clear();
            
            int currentcount = 0; //count of items.
            //iterate through all of the Templates...
            foreach (var iteratetemplate in BCBlockGameState.Templateman.LevelTemplates())
            {
                
                //is it part of this category?
                if (tc.isInCategory(iteratetemplate))
                {
                    currentcount++;
                    //add to the dropdown.
                    ToolStripMenuItem newtemplatemenuitem = new ToolStripMenuItem(iteratetemplate.Name);
                    newtemplatemenuitem.Tag = new object[] { iteratetemplate, itemcback };
                    newtemplatemenuitem.Click += new EventHandler(newtemplatemenuitem_Click);
                    thisitem.DropDown.Items.Add(newtemplatemenuitem);

                }




            }
            if (currentcount == 0)
            {

                //add "ghost" entry.
                thisitem.DropDown.Items.Add("(No Items)").Enabled = false;

            }





        }

        static void newtemplatemenuitem_Click(object sender, EventArgs e)
        {
            //fired when the item for a template is clicked.
            ToolStripMenuItem castitem = sender as ToolStripMenuItem;

            Object[] grabtag = (Object[])castitem.Tag;
            //first item is the Template Object. second is the callback delegate.
            TemplateManager.TemplateLevelData tld = (TemplateManager.TemplateLevelData)grabtag[0];
            TemplateItemCallback tic = (TemplateItemCallback)grabtag[1];
            if (tic != null)
            {

                tic(tld, castitem);

            }

        }









        /// <summary>
        /// Category Name.
        /// </summary>
        public String Name { get; set; }

        protected TemplateCategory(String pName)
        {
            Name = pName;

        }
        /// <summary>
        /// returns true if the given TemplateLevelData falls in this category. false otherwise.
        /// </summary>
        /// <param name="TemplateObject">Object to test</param>
        /// <returns></returns>
        public abstract bool isInCategory(TemplateManager.TemplateLevelData TemplateObject);
        protected Image _CategoryImage = null;
        public virtual void setCategoryImage(Image value)
        {

            _CategoryImage = value;
        }
        public virtual Image getCategoryImage()
        {
            if(_CategoryImage ==null)
                _CategoryImage = BCBlockGameState.ToolbarImages.getLoadedImage("text");


            return _CategoryImage;
        }
    }

    public class AllCategory:TemplateCategory
    {
        public AllCategory()
            : base("All Templates")
        {
            _CategoryImage=BCBlockGameState.ToolbarImages.getLoadedImage("allcategory");

        }

        
        public override bool isInCategory(TemplateManager.TemplateLevelData TemplateObject)
        {
            return true;
        }

        public static IEnumerable<TemplateCategory> getTemplateClasses() { return new TemplateCategory[] { new AllCategory() }; }

    }
    /// <summary>
    /// class  that filters Templates based on a given regular expression being applied to it's name.
    /// </summary>
    public class RegularExpressionNameCategoryFilter : TemplateCategory
    {
        private Regex re = null;
        private String _Pattern = "";
        private Image _useImage = null;
        public String Pattern { get { return _Pattern; } set { _Pattern = value; re = new Regex(_Pattern,RegexOptions.IgnoreCase); } }
        public RegularExpressionNameCategoryFilter(String pName, String pPattern,Image useImage)
            : base(pName)
        {
            this.Pattern = pPattern;
            _useImage = useImage;

        }
        public override bool isInCategory(TemplateManager.TemplateLevelData TemplateObject)
        {
            return re.IsMatch(TemplateObject.Name);
        }
    }

    public class AlphabetCategory : TemplateCategory
    {
        public AlphabetCategory()
            : base("Alphabet Soup")
        {
        }

        private static Image TextToImage(String text)
        {
            return BCBlockGameState.DrawTextToImage(text, BCBlockGameState.GetScaledFont(new Font("Arial", 48, GraphicsUnit.Pixel), 32), SystemBrushes.WindowText, null, new Size(32, 32));


        }
        public static IEnumerable<TemplateCategory> getTemplateClasses()
        {
            List<TemplateCategory> retrieved= new List<TemplateCategory>();
            String Alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";


            foreach (char iterate in Alphabet)
            {
                String usere = "^" + iterate.ToString();
                var madereobject = new RegularExpressionNameCategoryFilter("-" + iterate.ToString() + "-", usere, TextToImage(iterate.ToString()));
                madereobject.setCategoryImage(TextToImage(iterate.ToString()));
                
                retrieved.Add(madereobject);
                

            }
            //add one for numbers too.
            retrieved.Add(new RegularExpressionNameCategoryFilter("-#-","^[0-9]",TextToImage("#")));
            
        
            return retrieved;
        
        }


        /// <summary>
        /// returns true if the given TemplateLevelData falls in this category. false otherwise.
        /// </summary>
        /// <param name="TemplateObject">Object to test</param>
        /// <returns></returns>
        public override bool isInCategory(TemplateManager.TemplateLevelData TemplateObject)
        {
            throw new NotImplementedException();
        }
    }

}
