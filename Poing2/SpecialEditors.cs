using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing.Design;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace BASeBlock
{
    //Testing class. Not used.
    public class TestCheckObject
    {

        private List<Type> _SelectedPowerups = BCBlockGameState.MTypeManager[typeof(GamePowerUp)].ManagedTypes;

        [Editor(typeof(TypeCheckboxItem<GamePowerUp>), typeof(UITypeEditor))]
        public List<Type> SelectedPowerups
        {
            get { return _SelectedPowerups; }
            set
            {
                _SelectedPowerups = value;

                foreach (Type iterate in _SelectedPowerups)
                {
                    Debug.Print(iterate.FullName);

                }


            }
        }


    }
    //TypeCheckBoxItem: edits types of type T.
    //this will only work with types enumerated during initialization, such as GamePowerup, Block etc.

    //The UI presents a listbox with checkboxes for each item, allowing one to select a subset.
    //this class can be used in concert with any type T that is managed by the games Type Manager.
    public class TypeCheckboxItem<T> : UITypeEditor
    {
        private class CheckboxListItem
        {
            public Type ObjectType = null;
            public CheckboxListItem(Type typefor)
            {
                ObjectType = typefor;

            }
            public override string ToString()
            {
                return ObjectType.Name;
            }
        }
        private class CheckboxTypeList : CheckedListBox
        {
            public CheckboxTypeList(Type[] currentselection)
            {
                foreach (var iterate in BCBlockGameState.MTypeManager[typeof(T)].ManagedTypes)
                {
                    bool selectit = currentselection.Contains(iterate);
                    this.Items.Add(new CheckboxListItem(iterate), selectit);

                }

                this.Sorted = true;
                this.BorderStyle = BorderStyle.None;
            }

            public List<Type> GetSelection()
            {
                List<Type> tt = new List<Type>();
                foreach (var iterate in CheckedItems)
                {

                    tt.Add(((CheckboxListItem)iterate).ObjectType);

                }
                return tt;

            }
        }

        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            Debug.Print("In GetEditStyle for CheckboxItem");

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
                    //convert to an array....
                    Type[] editarray = null;
                    if (value is Type[]) editarray = (Type[])value;
                    if (value is IEnumerable<Type>) editarray = ((IEnumerable<Type>)value).ToArray();

                    CheckboxTypeList list = new CheckboxTypeList(editarray);


                    // Drop the list control.
                    //list.DoubleClick += new EventHandler(list_DoubleClick);
                    mserv.DropDownControl(list);


                    value = list.GetSelection();


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
