using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml.Media;

namespace HelpersPortable
{
    public static class Brushes
    {
        private static Dictionary<Color, Brush> _colorDictionary = new Dictionary<Color, Brush>();

        public static Brush Black { get { return GetBrushFromColor(Colors.Black); } }

        public static Brush Transparent { get { return GetBrushFromColor(Colors.Transparent); } }

        public static Brush GetBrushFromColor(Color index)
        {
                if (!_colorDictionary.ContainsKey(index))
                    _colorDictionary.Add(index, new SolidColorBrush(index));
                return _colorDictionary[index];
        }

    }
}
