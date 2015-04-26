using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Design;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace BASeBlock
{

   

    public partial class SimpleTextEdit : Form
    {
        public String EditText { get; set; }
        public DialogResult returnresult{get;set;}

        public static void EditString(ref String stringtoedit)
        {

            SimpleTextEdit formtoedit;
            formtoedit = new SimpleTextEdit(stringtoedit);
            formtoedit.ShowDialog();

            if (formtoedit.returnresult == System.Windows.Forms.DialogResult.OK)
            {
                stringtoedit = formtoedit.EditText;


            }



        }

        public SimpleTextEdit(String editstring):this()
        {
            EditText=editstring;


        }

        public SimpleTextEdit()
        {
            InitializeComponent();
        }

        private void SimpleTextEdit_Resize(object sender, EventArgs e)
        {
            cmdOK.Location = new Point(ClientSize.Width-cmdOK.Width-5,ClientSize.Height-cmdOK.Height-5);
            cmdCancel.Location = new Point(cmdOK.Left-5-cmdCancel.Width,cmdOK.Top);


            txtTextEdit.Location=new Point(0,0);
            txtTextEdit.Size = new Size(ClientSize.Width,ClientSize.Height-cmdCancel.Top-5);
        }

        private void cmdOK_Click(object sender, EventArgs e)
        {
            returnresult=System.Windows.Forms.DialogResult.OK;
            EditText = txtTextEdit.Text;
            Hide();
        }

        private void cmdCancel_Click(object sender, EventArgs e)
        {
            returnresult = System.Windows.Forms.DialogResult.Cancel;
            EditText = txtTextEdit.Text;
            Hide();
        }

        private void SimpleTextEdit_Load(object sender, EventArgs e)
        {
            txtTextEdit.Text = EditText;
        }
    }
    public class SimpleTextEditor : UITypeEditor
    {

        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            Debug.Print("In GetEditStyle for SimpleTextEditor");

            return UITypeEditorEditStyle.Modal;
        }
        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            Debug.Print("SimpleTextEditor.EditValue");
            IWindowsFormsEditorService svc = (IWindowsFormsEditorService)
                provider.GetService(typeof(IWindowsFormsEditorService));


            if (svc != null)
            {
                //svc.ShowDialog(new frmCreatorPropertiesEditor((CreatorProperties)value));
                svc.ShowDialog(new SimpleTextEdit((string)value));
                // update etc
            }
            return value;
        }


    }
}
