using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;

namespace HelpersPortable
{
    public static class Colors
    {
        private static Dictionary<string, Color> _colorDictionary = new Dictionary<string, Color>();

        private const int ColorStringLength = 7;  // 7 = 1 '#' sign + 3*2 char hexadecimal number 
        private const int ColorWithAlphaStringLength = 9;

        public static Color Black { get { return GetColorFromString("#FF000000"); } }

        public static Color Transparent { get { return GetColorFromString("#00FFFFFF"); } }

        public static Color GetColorFromString(string index_in)
        {
           
                if (!_colorDictionary.ContainsKey(index_in))
                    _colorDictionary.Add(index_in, ParseColor(index_in));
                return _colorDictionary[index_in];
            
        }

        private static Color ParseColor(string index)
        {
            if (!index.StartsWith("#"))
                throw new ArgumentException("Color string has to start with '#'");

            Color ret = new Color();
            if (index.Length == ColorWithAlphaStringLength)
            {
                ret.A = byte.Parse(index.Substring(1, 2), System.Globalization.NumberStyles.AllowHexSpecifier);
                ret.R = byte.Parse(index.Substring(3, 2), System.Globalization.NumberStyles.AllowHexSpecifier);
                ret.G = byte.Parse(index.Substring(5, 2), System.Globalization.NumberStyles.AllowHexSpecifier);
                ret.B = byte.Parse(index.Substring(7, 2), System.Globalization.NumberStyles.AllowHexSpecifier);
            }
            else if (index.Length == ColorStringLength)
            {
                ret.R = byte.Parse(index.Substring(1, 2), System.Globalization.NumberStyles.AllowHexSpecifier);
                ret.G = byte.Parse(index.Substring(3, 2), System.Globalization.NumberStyles.AllowHexSpecifier);
                ret.B = byte.Parse(index.Substring(5, 2), System.Globalization.NumberStyles.AllowHexSpecifier);
            }
            else
            {
                throw new ArgumentException("Length of Color string is invalid");
            }
            return ret;
        }

    }
}
