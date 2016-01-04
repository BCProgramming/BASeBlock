using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Windows.Forms;
using BASeCamp.Configuration;

namespace BASeCamp.BASeBlock
{
    /// <summary>
    /// FormPlacement.cs; used to save/load form position data using the GetWindowPlacement() and SetWindowPlacement() functions.
    /// </summary>
    public abstract class FormPlacement
    {
        Stream StorageStream;
        [StructLayout(LayoutKind.Sequential)]
        [Serializable]
        public struct POINTAPI
        {
            internal int x;
            internal int y;
           
        }
        
        [StructLayout(LayoutKind.Sequential)]
        [Serializable]
        public struct RECT
        {
            internal int Left;
            internal int Top;
            internal int Right;
            internal int Bottom;
         
        }


        [StructLayout(LayoutKind.Sequential)]
        [Serializable]
        public struct WINDOWPLACEMENT
        {
            internal int Length;
            internal int flags;
            internal int showCmd;
            internal POINTAPI ptMinPosition;
            internal POINTAPI ptMaxPosition;
            internal RECT rcNormalPosition;
            //adds all items in the DictB dictionary to DictA.
            private void MergeDictionary(Dictionary<String, int> DictA, ref Dictionary<String, int> DictB)
            {

                foreach (String keyloop in DictB.Keys)
                {
                    DictA.Add(keyloop, DictB[keyloop]);


                }


            }
            
        }

        [DllImport("user32.dll")]
        protected static extern int GetWindowPlacement(int hwnd, ref WINDOWPLACEMENT lpwndpl);
        [DllImport("user32.dll")]
        protected static extern int SetWindowPlacement(int hwnd, ref WINDOWPLACEMENT lpwndpl);
        //private WINDOWPLACEMENT mPlacementData;
        void LoadData(IntPtr loadtohandle)
        {
            try
            {
                BinaryFormatter fformatter = new BinaryFormatter();

                WINDOWPLACEMENT deserialized = (WINDOWPLACEMENT)fformatter.Deserialize(StorageStream);
                SetWindowPlacement((int)loadtohandle, ref deserialized);

            }
            finally
            {
            }
        }
        void SaveData(IntPtr savehandle)
        {
            try
            {
                WINDOWPLACEMENT useplacement = new WINDOWPLACEMENT();
                if (GetWindowPlacement((int)savehandle, ref useplacement) > 0)
                {
                    BinaryFormatter fformatter = new BinaryFormatter();
                    StorageStream.Seek(0, SeekOrigin.Begin);
                    StorageStream.SetLength(0);
                    fformatter.Serialize(StorageStream, useplacement);

                }


            }
            finally
            {
            }


        }
        public abstract WINDOWPLACEMENT LoadFormPlacement(Form forForm);
        public abstract void SaveFormPlacement(Form forForm,WINDOWPLACEMENT placementdata);
        
        


        Form constructorform=null;
        protected FormPlacement(Form formobject)
        {
            constructorform=formobject;
          


        }

        public void FormPlacement_Apply(Form formplace)
        {
            WINDOWPLACEMENT wp = LoadFormPlacement(formplace);
            SetWindowPlacement((int)formplace.Handle,ref wp);




        }
        public void FormPlacement_Save(Form formplace)
        {
            WINDOWPLACEMENT wp = new WINDOWPLACEMENT();
            GetWindowPlacement((int)formplace.Handle,ref wp);
            SaveFormPlacement(formplace, wp);

        }

    }
    public class FormPlacementINI : FormPlacement
    {
        private INIFile mINIFile;
        public FormPlacementINI(Form formobject, INIFile inifile):base(formobject)
        {
            mINIFile=inifile;
            //LoadPlacement();


        }


        private INISection GetPlacementSection(Form forform)
        {

            return mINIFile[forform.Name];

        }
        private int TryParse(String tryit)
        {
            int returnme;
            returnme = 0;
            int.TryParse(tryit, out returnme);

            return returnme;


        }

        public override FormPlacement.WINDOWPLACEMENT LoadFormPlacement(Form forForm)
        {
           

                INISection usesection = GetPlacementSection(forForm);
                WINDOWPLACEMENT returnplacement = new WINDOWPLACEMENT();
                if (usesection.getValues().Count() == 0)
                {
                    //go with default (current) values.. set at design time.
                    GetWindowPlacement((int)forForm.Handle, ref returnplacement);
                    return returnplacement;

                }
            
                returnplacement.Length = Marshal.SizeOf(returnplacement);
                
            returnplacement.ptMinPosition.x = TryParse(usesection["MinPosX"].Value);
            returnplacement.ptMinPosition.y = TryParse(usesection["MinPosY"].Value);

            returnplacement.ptMaxPosition.x = TryParse(usesection["MaxPosX"].Value);
            returnplacement.ptMaxPosition.y = TryParse(usesection["MaxPosY"].Value);


            returnplacement.flags = TryParse(usesection["Flags"].Value);


            returnplacement.rcNormalPosition.Left = TryParse(usesection["Left"].Value);
            returnplacement.rcNormalPosition.Top = TryParse(usesection["Top"].Value);
            returnplacement.rcNormalPosition.Right = TryParse(usesection["Right"].Value);
            returnplacement.rcNormalPosition.Bottom = TryParse(usesection["Bottom"].Value);


            returnplacement.showCmd = TryParse(usesection["ShowState"].Value);
                //returnplacement.ptMinPosition "MinPosX,MinPosY"
            //returnplacement.ptMaxPosition "MaxPosX,MaxPosY"
            //returnplacement.flags "flags"

            return returnplacement;
                

        }

        public override void SaveFormPlacement(Form forForm, FormPlacement.WINDOWPLACEMENT placementdata)
        {
            INISection usesection = GetPlacementSection(forForm);


            usesection["MinPosX"].Value = placementdata.ptMinPosition.x.ToString();
            usesection["MinPosY"].Value = placementdata.ptMinPosition.y.ToString();


            usesection["MaxPosX"].Value = placementdata.ptMaxPosition.x.ToString();
            usesection["MaxPosY"].Value = placementdata.ptMaxPosition.y.ToString();

            usesection["Flags"].Value = placementdata.flags.ToString();

            usesection["Left"].Value = placementdata.rcNormalPosition.Left.ToString();
            usesection["Top"].Value = placementdata.rcNormalPosition.Top.ToString();
            usesection["Right"].Value = placementdata.rcNormalPosition.Right.ToString();
            usesection["Bottom"].Value = placementdata.rcNormalPosition.Bottom.ToString();


        }
    }
    public class FormPlacementStream : FormPlacement
    {
        private Stream storagestream = null;
        public FormPlacementStream(Form formobject,Stream rwStream):base(formobject)
        {
            if (!(rwStream.CanRead && rwStream.CanWrite && rwStream.CanSeek))
            {

                throw new ArgumentException("FormPlacementStream rwstream argument must be readable, seekable, and writable.");
            }
            storagestream = rwStream;
            //LoadPlacement();

        }






        public override FormPlacement.WINDOWPLACEMENT LoadFormPlacement(Form forForm)
        {
           //
            BinaryFormatter useformatter = new BinaryFormatter();
            return (WINDOWPLACEMENT)useformatter.Deserialize(storagestream);


        }

        public override void SaveFormPlacement(Form forForm, FormPlacement.WINDOWPLACEMENT placementdata)
        {
            BinaryFormatter useformatter = new BinaryFormatter();
            storagestream.Seek(0, SeekOrigin.Begin);
            storagestream.SetLength(0);
            useformatter.Serialize(storagestream, placementdata);
        }
    }

    public class AutoFormSaver
    {
        private Form mFormSave;
        private FormPlacement useplacement;
        public AutoFormSaver(Form formuse, FormPlacement fsaver)
        {
            //must be called in the Form's Initialize() routine.
            mFormSave = formuse;
            mFormSave.Load += new EventHandler(mFormSave_Load);
            mFormSave.Closing += new System.ComponentModel.CancelEventHandler(mFormSave_Closing);
        }

        void mFormSave_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            useplacement.FormPlacement_Save(mFormSave);
        }

        
        void mFormSave_Load(object sender, EventArgs e)
        {
            useplacement.FormPlacement_Apply(mFormSave);
        }




    }

}
