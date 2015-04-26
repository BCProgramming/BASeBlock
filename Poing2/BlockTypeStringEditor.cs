using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Design;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using BASeBlock.Blocks;

namespace BASeBlock
{




    public class LoadedScriptsStringEditor : UITypeEditor
    {
        private class ScriptStringList : ListBox
        {
            public ScriptStringList(EditorSet editset)
            {
                //go through all loadedScripts.
                foreach(var loopvar in editset.CreateData.SavedScripts)
                {
                    this.Items.Add(loopvar.Key);


                }
                this.Sorted=true;
                this.BorderStyle=BorderStyle.None;
            }



        }
        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            Debug.Print("In GetEditStyle for BlockTypeStringEditor");

            return UITypeEditorEditStyle.DropDown;
        }
        private IWindowsFormsEditorService mserv;
        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            if (provider != null)
            {
                // This service is in charge of popping our ListBox.
                mserv = ((IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService)));

                if (mserv != null)
                {
                    // This is an internal Microsoft class representing the PropertyGrid entry for our component.

                    ScriptStringList list = new ScriptStringList((EditorSet)context.Instance );


                    // Drop the list control.
                    list.DoubleClick += new EventHandler(list_DoubleClick);
                    mserv.DropDownControl(list);

                    if (list.SelectedIndices.Count == 1)
                    {
                        value = list.SelectedItem.ToString();
                    }

                    // Close the list control after selection.
                    mserv.CloseDropDown();
                }
            }
            return value;
        }

        void list_DoubleClick(object sender, EventArgs e)
        {
            mserv.CloseDropDown();
        }


    }
    
    public class ItemTypeEditor<T> : UITypeEditor
    {
        private class ItemTypeEditorItem
        {
            public String Description="";
            public readonly Type TypeObject;

            public static implicit operator Type(ItemTypeEditorItem d)
            {
                return d.TypeObject;

            }

            public ItemTypeEditorItem(Type usetype)
            {
                TypeObject = usetype;
                foreach (var iterate in usetype.GetCustomAttributes(typeof(DescriptionAttribute),true))
                {
                    if (iterate is DescriptionAttribute)
                    {
                        Description = (iterate as DescriptionAttribute).Description;


                    }


                }

            }
            public override int GetHashCode()
            {
                return TypeObject.GetHashCode();
            }
            public override string ToString()
            {
                if (Description != "")
                    return Description;
                
                return TypeObject.Name;
                    
                
            }


        }
        private class TypeStringList : ListBox
        {
            
            public TypeStringList()
            {
                foreach (Type iterate in BCBlockGameState.MTypeManager[typeof(T)].ManagedTypes)
                {
                    this.Items.Add(new ItemTypeEditorItem(iterate));        

                }
                this.Sorted = true;
                this.BorderStyle = BorderStyle.None;


            }

        }


        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            
            Debug.Print("In GetEditStyle for ItemTypeEditor");

            return UITypeEditorEditStyle.DropDown;
        }
        
        private Font usefont = null;
        private IWindowsFormsEditorService mserv;
        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {

            Debug.Print(("Type of context.instance-" + context.Instance.GetType().Name));
            if (provider != null)
            {
                // This service is in charge of popping our ListBox.
                mserv = ((IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService)));

                if (mserv != null)
                {
                    // This is an internal Microsoft class representing the PropertyGrid entry for our component.

                    TypeStringList list = new TypeStringList();


                    // Drop the list control.
                    list.DoubleClick += new EventHandler(list_DoubleClick);
                    mserv.DropDownControl(list);

                    if (list.SelectedIndices.Count == 1)
                    {
                        value = ((list.SelectedItem)as  ItemTypeEditorItem).TypeObject;
                    }

                    // Close the list control after selection.
                    mserv.CloseDropDown();
                }
            }
            return value;
        }

        void list_DoubleClick(object sender, EventArgs e)
        {
            mserv.CloseDropDown();
        }




    }

    public class GameObjectTypeEditor : UITypeEditor
    {
        private class GameObjectListItem
        {
            public Type ObjectType = null;
            public GameObjectListItem(Type typefor)
            {
                ObjectType = typefor;

            }
            public override string ToString()
            {
                return ObjectType.Name;
            }
        }
        private class GameObjectTypeList : ListBox
        {
            public GameObjectTypeList()
            {
                foreach (var iterate in BCBlockGameState.MTypeManager[typeof(GameObject)].ManagedTypes)
                {

                    this.Items.Add(new GameObjectListItem(iterate));
                }
                this.Sorted = true;
                this.BorderStyle = BorderStyle.None;
            }


        }
        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            Debug.Print("In GetEditStyle for BlockTypeStringEditor");

            return UITypeEditorEditStyle.DropDown;
        }
        private IWindowsFormsEditorService mserv;
        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {

            Debug.Print(("Type of context.instance-" + context.Instance.GetType().Name));
            if (provider != null)
            {
                // This service is in charge of popping our ListBox.
                mserv = ((IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService)));

                if (mserv != null)
                {
                    // This is an internal Microsoft class representing the PropertyGrid entry for our component.

                    //BlockTypeStringEditor.BlockTypeStringList list = new BlockTypeStringEditor.BlockTypeStringList();
                    GameObjectTypeList list = new GameObjectTypeList();


                    // Drop the list control.
                    list.DoubleClick += new EventHandler(list_DoubleClick);
                    mserv.DropDownControl(list);

                    if (list.SelectedIndices.Count == 1)
                    {
                        value = ((GameObjectListItem)(list.SelectedItem)).ObjectType;
                    }

                    // Close the list control after selection.
                    mserv.CloseDropDown();
                }
            }
            return value;
        }

        void list_DoubleClick(object sender, EventArgs e)
        {
            mserv.CloseDropDown();
        }


    }


    /// <summary>
    /// Class used by various other classes as a Type Editor for certain properties. Specifically, it was created as a Type Editor for the Animated Blocks
    /// "blocktype" property, which is a string property used to instantiate the "baseBlock" object that it uses. The Editor merely displays a list of all Loaded Block Types, which is done when
    /// the game initially loads (initgamestate() initializes the BCBlockGameState.BlockTypes object).
    /// </summary>
    public class BlockTypeStringEditor : UITypeEditor
    {
        private class BlockTypeStringList : ListBox
        {
            
            public BlockTypeStringList()
            {
                // Go over all properties, filtering out the ones we need (public/get/set/boolean).
                // None is a reserved type for a case where no property is selected.
                foreach (BlockData loopBlock in BCBlockGameState.BlockDataMan.BlockInfo)
                {
                    this.Items.Add(loopBlock);


                }

                // this.Items.Add("None");
                this.Height = this.FontHeight * 12;

                this.Sorted = true;
                // Not setting the border to none just doesn't look good.
                this.BorderStyle = BorderStyle.None;
                this.DrawMode = DrawMode.OwnerDrawVariable;
               // this.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawVariable;
               // this.DrawItem += new DrawItemEventHandler(BlockTypeStringList_DrawItem);
               // this.MeasureItem += new MeasureItemEventHandler(BlockTypeStringList_MeasureItem);
                MeasureItem += BlockTypeStringList_MeasureItem;
                DrawItem+=new DrawItemEventHandler(BlockTypeStringList_DrawItem);
            }
            
            void BlockTypeStringList_MeasureItem(object sender, MeasureItemEventArgs e)
            {
                //measure the given item
                
                Object itemmeasure = Items[e.Index];
                BlockData castdata = (BlockData)itemmeasure;
                //cast to Block...
                SizeF usesize = DoMeasureBlockData(castdata,e.Graphics);

                e.ItemWidth = (int)(usesize.Width);
                e.ItemHeight = (int)usesize.Height;

            }
            private SizeF DoMeasureBlockData(BlockData castdata,Graphics useg)
            {
                SizeF usesize = useg.MeasureString(castdata.Usename, this.Font);
                return new SizeF(18 + usesize.Width,Math.Max(usesize.Height+2,20f));

            }

            void BlockTypeStringList_DrawItem(object sender, DrawItemEventArgs e)
            {
                //draw the item.
                Brush Fontbrush, backgroundBrush;



                
                Object itemmeasure = Items[e.Index];
                
                if (e.State==DrawItemState.Selected)
                {
                    //selected.
                    Fontbrush = SystemBrushes.HighlightText;
                    backgroundBrush = SystemBrushes.Highlight;

                }
                else
                {
                    Fontbrush = SystemBrushes.WindowText;
                    backgroundBrush = SystemBrushes.Window;
                }

                BlockData castdata = (BlockData)itemmeasure;
                SizeF measured = DoMeasureBlockData(castdata, e.Graphics);
                //cast to Block...
                SizeF usesize = e.Graphics.MeasureString(castdata.Usename, this.Font);
                
                //draw background...
                e.DrawBackground();
                if (e.State == DrawItemState.Focus)
                {
                    e.DrawFocusRectangle();


                }
                //e.Graphics.FillRectangle(backgroundBrush, e.Bounds.Left, e.Bounds.Top, measured.Width, measured.Height);
                //draw the image of the block.
                
                e.Graphics.DrawImage(castdata.useBlockImage, e.Bounds.Left+2, e.Bounds.Top, e.Bounds.Height, e.Bounds.Height);
                e.Graphics.DrawString(castdata.Usename, this.Font, Fontbrush, e.Bounds.Left + e.Bounds.Height, e.Bounds.Top + 2);

                //throw new NotImplementedException();
            }



            


        }


        public override void PaintValue(PaintValueEventArgs e)
        {
            
        }

        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            Debug.Print("In GetEditStyle for BlockTypeStringEditor");

            return UITypeEditorEditStyle.DropDown;
        }
        private IWindowsFormsEditorService mserv;
        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            
            Debug.Print(("Type of context.instance-" + context.Instance.GetType().Name));
            if (provider != null)
            {
                // This service is in charge of popping our ListBox.
                mserv = ((IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService)));

                if (mserv != null)
                {
                    // This is an internal Microsoft class representing the PropertyGrid entry for our component.

                    BlockTypeStringList list = new BlockTypeStringList();


                    // Drop the list control.
                    list.DoubleClick += new EventHandler(list_DoubleClick);
                    mserv.DropDownControl(list);

                    if (list.SelectedIndices.Count == 1)
                    {
                        value = ((BlockData)(list.SelectedItem)).Usename;
                    }

                    // Close the list control after selection.
                    mserv.CloseDropDown();
                }
            }
            return value;
        }

        void list_DoubleClick(object sender, EventArgs e)
        {
            mserv.CloseDropDown();
        }





    }
}
