using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using BASeCamp.BASeBlock;
using BASeCamp.Updating;


namespace BASeCamp.BASeBlock.Templates
{
    public partial class frmFromTemplate : Form
    {
        public TemplateManager.TemplateLevelData SelectedTemplate = null;
        private ImageList TemplateImageList = null;
        private ImageList CategoryImageList = null;
        List<TemplateCategory> OurCategories = null;
        public frmFromTemplate()
        {
            InitializeComponent();
        }

        private void frmFromTemplate_Load(object sender, EventArgs e)
        {
           


            CategoryImageList = new ImageList();
            CategoryImageList.ImageSize = new Size(16, 16);
            CategoryImageList.Images.Add("TEXT",BCBlockGameState.ToolbarImages.getLoadedImage("TEXT"));
            lvwCategories.Items.Clear();
            lvwCategories.Columns.Clear();
            lvwCategories.LargeImageList = CategoryImageList;

            Dictionary<Image,String> Imagelookup = new Dictionary<Image, string>();


            OurCategories = new List<TemplateCategory>();
            
            foreach (Type templatetype in BCBlockGameState.MTypeManager[typeof(TemplateCategory)].ManagedTypes)
            {

                //public static IEnumerable<TemplateCategory> getTemplateClasses()
                //call the static method....
                try
                {
                    MethodInfo foundmethod = null;
                    MethodInfo[] mi = templatetype.GetMethods();
                    foreach (var loopmethod in mi)
                    {
                        if (loopmethod.Name.Equals("getTemplateClasses", StringComparison.OrdinalIgnoreCase))
                        {
                            foundmethod = loopmethod;
                            break;

                        }


                    }
                    if (foundmethod != null)
                    {

                        Object result = foundmethod.Invoke(null, BindingFlags.Static, null, null, null);
                        IEnumerable<TemplateCategory> resultenum = (IEnumerable<TemplateCategory>)result;

                        foreach (TemplateCategory addtemplate in resultenum)
                        {
                            //add it to the listing.
                            OurCategories.Add(addtemplate);
                            //add to the listview.

                            String useimagekey = "TEXT";
                            Image grabimage = addtemplate.getCategoryImage();
                            if (grabimage != null)
                            {
                                if (Imagelookup.ContainsKey(grabimage))
                                    useimagekey = Imagelookup[grabimage];
                                else
                                {
                                    useimagekey = addtemplate.Name + CategoryImageList.Images.Count.ToString();
                                    CategoryImageList.Images.Add(useimagekey, grabimage);
                                    Imagelookup.Add(grabimage, useimagekey);
                                }


                            }


                            ListViewItem CategoryItem = lvwCategories.Items.Add(addtemplate.GetType().AssemblyQualifiedName+addtemplate.Name, addtemplate.Name,
                                useimagekey);
                            CategoryItem.Tag = addtemplate;

                        }
                    }

                }
                catch
                {
                    //ignore exceptions...

                }


            }



            


            



        }

        private void cmdCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void lvwTemplates_DoubleClick(object sender, EventArgs e)
        {
            cmdOK_Click(sender, e);
        }

        private void cmdOK_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();
        }
        
        private void lvwTemplates_SelectedIndexChanged(object sender, EventArgs e)
        {
            var selitem = lvwTemplates.getSelectedItem();
            cmdOK.Enabled = selitem != null;
            if(selitem!=null)
                SelectedTemplate = (TemplateManager.TemplateLevelData)lvwTemplates.getSelectedItem().Tag;
        }

        private void lvwCategories_SelectedIndexChanged(object sender, EventArgs e)
        {

           



        }

        private void lvwTemplates_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
          
        }

        private void lvwCategories_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            if (!e.IsSelected) return;
            TemplateImageList = new ImageList();
            TemplateImageList.ImageSize = new Size(32, 32);
            TemplateImageList.Images.Add("DEFAULTTEMPLATE", BCBlockGameState.ToolbarImages.getLoadedImage("TEXT"));
            lvwTemplates.Items.Clear();
            lvwTemplates.Columns.Clear();
            lvwTemplates.LargeImageList = TemplateImageList;



            //go through all templates.
            ListViewItem selitem = lvwCategories.getSelectedItem();
            if (selitem == null) return;
            TemplateCategory newCategory = selitem.Tag as TemplateCategory;
            foreach (var iteratetemplate in BCBlockGameState.Templateman.LevelTemplates())
            {
                if (newCategory.isInCategory(iteratetemplate))
                {
                    //add a new item.
                    ListViewItem curritem = lvwTemplates.Items.Add(iteratetemplate.LevelData.LevelName + "-" + iteratetemplate.Name, iteratetemplate.Name, "DEFAULTTEMPLATE");
                    curritem.Tag = iteratetemplate;
                }

            }
        }
    }
}
