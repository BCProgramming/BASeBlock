using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using BASeCamp.Licensing;


namespace ProductKeyGenerator
{
    public partial class frmKeyGen : Form
    {
        private BASeCamp.Licensing.ProductKey KeyObject;
        
        private static void AddEnumToCombo(Type EnumType,ComboBox lb)

        {
            lb.Items.Clear();
            foreach (var value in Enum.GetValues(EnumType))
            {
                lb.Items.Add(value.ToString());

            }
            
            
        }

        public frmKeyGen()
        {
            InitializeComponent();
        }
        
        private void cmdGenerate_Click(object sender, EventArgs e)
        {
            ProductKey pk = new ProductKey();
            pk.MachineID = txtIDString.Text;
            ProductKey.Products useproduct = (ProductKey.Products)Enum.Parse(typeof(ProductKey.Products), (String)cboProducts.SelectedItem);
            pk.Product = useproduct;
            //pk.Product = (ProductKey.Products)cboProducts.SelectedIndex;
            txtProductCode.Text = CryptHelper.InsertDashes(pk.GetProductCode());
        }

        private void frmKeyGen_Load(object sender, EventArgs e)
        {
            txtIDString.SetCueBanner("ID goes here");
            //Debug.Print("Set CueBanner to " + txtIDString.GetCueBanner());
            AddEnumToCombo(typeof(ProductKey.Products), cboProducts);
        }

        private void cmdClose_Click(object sender, EventArgs e)
        {
            Close();
        }
    }


}
