using System;

namespace BASeBlock.PaddleBehaviours
{
    public class PowerupDescriptionAttribute :Attribute
    {
        private String _Name;
        public String Name { get { return _Name; } set { _Name = value; } }
        public PowerupDescriptionAttribute(String pName)
        {
            _Name = pName;
        }

    }
}