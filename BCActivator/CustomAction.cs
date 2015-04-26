using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Deployment.WindowsInstaller;
using BASeCamp.Licensing;
namespace BCActivator
{
    /// <summary>
    /// Class interoperates with AdvancedInstaller, or rather Windows Installer, to show Activation screen on install.
    /// At least that's how it's supposed to work...
    /// </summary>
    public class CustomActions
    {
        [CustomAction]
        public static ActionResult CustomAction1(Session session)
        {
            int useid = 0;
            
            String productname = session["PRODUCT_NAME"].ToUpper();
            //session.Log("Begin CustomAction1");
            if (productname == "BASEBLOCK")
            {
                useid = 21;
            }
            ProductKey result = BASeCamp.Licensing.ProductKey.GetProductInformation("BASeBlock", ProductKey.Products.BASeBlock, true, null, BASeCamp.Licensing.ProductKey.getLocalMachineID());
            
            


            return ActionResult.Success;
        }
    }
}
