using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using System.ComponentModel;

namespace ExpandableObjectConverterDemo
{
    public class PersonObject
    {
        private string _Name = "";
        private int _Age = 0;
        private Brush _ShirtColor = Brushes.Blue;

        public string Name
        {
            get { return _Name; }
            set { _Name = value; }
        }

        public int Age
        {
            get { return _Age; }
            set { _Age = value; }
        }

        public Brush ShirtColor
        {
            get { return _ShirtColor; }
            set { _ShirtColor = value; }
        }
    }

    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class ExpandablePersonObject
    {
        private string _Name = "";
        private int _Age = 0;
        private Brush _ShirtColor = Brushes.Blue;

        public string Name
        {
            get { return _Name; }
            set { _Name = value; }
        }

        public int Age
        {
            get { return _Age; }
            set { _Age = value; }
        }

        public Brush ShirtColor
        {
            get { return _ShirtColor; }
            set { _ShirtColor = value; }
        }
    }
}
