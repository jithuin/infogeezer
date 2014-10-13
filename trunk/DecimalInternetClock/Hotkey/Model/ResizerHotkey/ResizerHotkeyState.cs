using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Xml.Serialization;

namespace HotKey
{
    [Serializable]
    public class ResizerHotkeyState
    {
        /// <summary>
        /// Size of the desktop window relative to the active desktop (equals to 1.0)
        /// </summary>
        public const double DesktopSize = 1.0;

        public ResizerHotkeyState()
        {
            Location = new Vector();
            Size = new Vector();
        }

        public ResizerHotkeyState(Vector location_in, Vector size_in)
        {
            Location = location_in;
            Size = size_in;
        }

        public ResizerHotkeyState(HorizontalAlignment horizontalAlignment_in, VerticalAlignment verticalAlignment_in, double portion_in)
        {
            _horizontalAlignment = horizontalAlignment_in;
            _verticalAlignment = verticalAlignment_in;
            _windowSizePortion = portion_in;
        }

        [XmlIgnore]
        public bool IsExplicitlySet { get; protected set; }

        protected Vector _location;

        /// <summary>
        /// Location of the application window relative to the active desktop ([0,0] means the top left corner of the desktop [1,1] means the bottom right)
        /// </summary>
        public Vector Location
        {
            get
            {
                if (!IsExplicitlySet)
                    UpdateLocation();
                return _location;
            }
            set
            {
                if (_location != value)
                {
                    _location = value;
                    IsExplicitlySet = true;
                }
            }
        }

        private void UpdateLocation()
        {
            switch (_horizontalAlignment)
            {
                case HorizontalAlignment.Center:
                    _location.X = (DesktopSize - _windowSizePortion) / 2.0;
                    break;

                case HorizontalAlignment.Right:
                    _location.X = DesktopSize - _windowSizePortion;
                    break;

                case HorizontalAlignment.Left:
                case HorizontalAlignment.Stretch:
                default:
                    _location.X = 0;
                    break;
            }

            switch (_verticalAlignment)
            {
                case VerticalAlignment.Bottom:
                    _location.Y = DesktopSize - _windowSizePortion;
                    break;

                case VerticalAlignment.Center:
                    _location.Y = (DesktopSize - _windowSizePortion) / 2.0;
                    break;

                case VerticalAlignment.Stretch:
                case VerticalAlignment.Top:
                default:
                    _location.Y = 0;
                    break;
            }
        }

        protected Vector _size;

        /// <summary>
        /// Size of the application window relative to the active desktop (the X and Y value can be vary between [0,1]. 1 means the size of the active desktop)
        /// </summary>
        public Vector Size
        {
            get
            {
                if (!IsExplicitlySet)
                    UpdateSize();
                return _size;
            }
            set
            {
                if (_size != value)
                {
                    _size = value;
                    IsExplicitlySet = true;
                }
            }
        }

        private void UpdateSize()
        {
            if (_horizontalAlignment == HorizontalAlignment.Stretch)
                _size.X = DesktopSize;
            else
                _size.X = _windowSizePortion;

            if (_verticalAlignment == VerticalAlignment.Stretch)
                _size.Y = DesktopSize;
            else
                _size.Y = _windowSizePortion;
        }

        #region HorizontalAlignment

        /// <summary>
        /// The <see cref="HorizontalAlignment" /> property's name.
        /// </summary>
        public const string HorizontalAlignmentPropertyName = "HorizontalAlignment";

        private HorizontalAlignment _horizontalAlignment = HorizontalAlignment.Stretch;

        /// <summary>
        /// Sets and gets the HorizontalAlignment property.
        /// </summary>
        public HorizontalAlignment HorizontalAlignment
        {
            get { return _horizontalAlignment; }
            set
            {
                if (_horizontalAlignment != value)
                {
                    _horizontalAlignment = value;
                    IsExplicitlySet = false;
                }
            }
        }

        #endregion HorizontalAlignment

        #region VerticalAlignment

        /// <summary>
        /// The <see cref="VerticalAlignment" /> property's name.
        /// </summary>
        public const string VerticalAlignmentPropertyName = "VerticalAlignment";

        private VerticalAlignment _verticalAlignment = VerticalAlignment.Stretch;

        /// <summary>
        /// Sets and gets the VerticalAlignment property.
        /// </summary>
        public VerticalAlignment VerticalAlignment
        {
            get { return _verticalAlignment; }
            set
            {
                if (_verticalAlignment != value)
                {
                    _verticalAlignment = value;
                    IsExplicitlySet = false;
                }
            }
        }

        #endregion VerticalAlignment

        #region DesktopPortion

        /// <summary>
        /// The <see cref="DesktopPortion" /> property's name.
        /// </summary>
        public const string DesktopPortionPropertyName = "DesktopPortion";

        private double _windowSizePortion = 1;

        /// <summary>
        /// Sets and gets the DesktopPortion property.
        /// </summary>
        public double DesktopPortion
        {
            get { return _windowSizePortion; }
            set
            {
                if (_windowSizePortion != value)
                {
                    _windowSizePortion = value;
                    IsExplicitlySet = false;
                }
            }
        }

        #endregion DesktopPortion
    }
}