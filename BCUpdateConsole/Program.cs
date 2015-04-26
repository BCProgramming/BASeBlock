using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using BASeCamp.Updating;
namespace BCUpdateConsole
{
    class Program
    {
        static String GetVersionInfo()
        {
            String UpdatelibVersion = Assembly.GetAssembly(typeof(BASeCamp.Updating.BCUpdate)).GetName().Version.ToString();
            String consoleversion = Assembly.GetExecutingAssembly().GetName().Version.ToString();

            return "Update Library Version:" + UpdatelibVersion + "\n" +
                "Console View Version:" + consoleversion + "\n";


        }
        static void ShowHelp()
        {

            Console.Write(
                "BASeCamp Update Library Console Control Program\n" + GetVersionInfo() + "\nSyntax:\n" + Path.GetFileNameWithoutExtension(Assembly.GetEntryAssembly().Location));
                
                   




        }
        static void Main(string[] args)
        {
            BCUpdate bu = new BCUpdate();
            Console.WriteLine("Update Summary");
            foreach (var iterate in bu.LoadedUpdates)
            {
                String installedver = bu.getinstalledVersion(iterate.dlID);
                installedver = installedver == "" ? "(none)" : installedver;
                Console.WriteLine(iterate.DlName +
                    "\n\tInstalled:" + installedver + "\n\t" +
                    "Update Version:" + iterate.UpdateVersion + "\n----\n");



            }


            Console.ReadKey();
        }
    }
}
