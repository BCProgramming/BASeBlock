using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
namespace BASeCamp.BASeBlock
{

    public abstract class BaseCompareDiff
    {
        //represents a single difference between two objects. 
        




    }
    public class CompareFieldDiff : BaseCompareDiff
    {


        /// <summary>
        /// Field that differs; this is a reference to the FieldInfo of A.
        /// </summary>
        public readonly FieldInfo ObjectAField=null;

        public readonly FieldInfo ObjectBField=null;
        /// <summary>
        /// Value of this field for ItemA
        /// </summary>
        public readonly Object AValue;
        /// <summary>
        /// Value of this field for  ItemB.
        /// </summary>
        public readonly Object BValue;

      
        public CompareFieldDiff(Object ItemA, Object ItemB, String FieldName)
        {
            Type TypeA = ItemA.GetType();
            Type TypeB = ItemB.GetType();


            FieldInfo FieldA = TypeA.GetField(FieldName);
            FieldInfo FieldB = TypeB.GetField(FieldName);
            ObjectAField = FieldA;
            ObjectBField = FieldB;

            AValue = ObjectAField.GetValue(ItemA);
            BValue = ObjectBField.GetValue(ItemB);




        }

    }
    

    /// <summary>
    /// Provides Generic class for comparing Two classes/structs.
    /// </summary>
    class GenericComparer
    {

        public static List<BaseCompareDiff> DoCompare<T>(T ItemA, T ItemB)
        {
            List<BaseCompareDiff> bcd = new List<BaseCompareDiff>();


            //get all the field names.
            Type AType = ItemA.GetType();


            String[] FieldNames = (from p in AType.GetFields(BindingFlags.Public) select p.Name).ToArray();
            //create a new FieldCompareDiff and add it to the list.
            foreach (String Fieldname in FieldNames)
            {

                CompareFieldDiff cfd = new CompareFieldDiff(ItemA, ItemB, Fieldname);
                bcd.Add(cfd);


            }




            return bcd;


        }


    }
}
