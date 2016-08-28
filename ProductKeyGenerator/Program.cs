using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using BASeCamp.Licensing;

namespace ProductKeyGenerator
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            LicenseKey lkd = new LicenseKey();
            lkd.LicensedEdition = LicenseKey.Edition.Professional;
            lkd.LicensedMajorVersion = 8;
            lkd.LicensedMinorVersion = 2;
            lkd.LicensedUsers = 50;
            lkd.ExpiryDate = DateTime.Now.AddMonths(1);
            String buildkey = LicenseHandler.ToProductCode(lkd, CryptHelper.LocalMachineID);
            buildkey = CryptHelper.InsertDashes(buildkey);

            LicenseKeyData rebuild = LicenseHandler.FromProductCode<LicenseKey>(buildkey, CryptHelper.LocalMachineID);


            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new frmKeyGen());
        }
    }
}
