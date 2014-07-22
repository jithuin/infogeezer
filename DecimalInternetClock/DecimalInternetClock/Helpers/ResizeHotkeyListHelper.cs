using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Windows;
using System.Xml.Serialization;
using DecimalInternetClock.HotKeys;
using ManagedWinapi;
using Forms = System.Windows.Forms;

namespace DecimalInternetClock.Helpers
{
    public static class ResizeHotkeyListHelper
    {
        #region Set to Default

        private enum EWinPosVert
        {
            Top,
            Centre,
            Bottom,
            Total,
        }

        private enum EWinPosHoriz
        {
            Left,
            Middle,
            Right,
            Total,
        }

        private class WinPos
        {
            public WinPos()
            {
            }

            public WinPos(EWinPosVert vpos_in, EWinPosHoriz hpos_in)
            {
                HPos = hpos_in;
                VPos = vpos_in;
            }

            public WinPos(EWinPosHoriz hpos_in, EWinPosVert vpos_in)
                : this(vpos_in, hpos_in)
            { }

            public EWinPosHoriz HPos;

            public EWinPosVert VPos;
        }

        private static List<double> _portions = new List<double>() { 0.25, 1.0 / 3, 0.5, 0.75 };

        private static FKeyModifiers _defaultModifiers = FKeyModifiers.Alt | FKeyModifiers.Ctrl;

        private static Dictionary<WinPos, List<Forms.Keys>> _posCommandKey = new Dictionary<WinPos, List<Forms.Keys>>()
        {
            {new WinPos(EWinPosHoriz.Left, EWinPosVert.Total), new List<Forms.Keys>(){Forms.Keys.Left}},
            //{new WinPos(EWinPosHoriz.Middle, EWinPosVert.Total), new List<Forms.Keys>(){Forms.Keys.Left, Forms.Keys.NumPad4}},
            {new WinPos(EWinPosHoriz.Right, EWinPosVert.Total), new List<Forms.Keys>(){Forms.Keys.Right}},
            {new WinPos(EWinPosHoriz.Left, EWinPosVert.Top), new List<Forms.Keys>(){Forms.Keys.NumPad7}},
            {new WinPos(EWinPosHoriz.Middle, EWinPosVert.Top), new List<Forms.Keys>(){Forms.Keys.NumPad8}},
            {new WinPos(EWinPosHoriz.Right, EWinPosVert.Top), new List<Forms.Keys>(){Forms.Keys.NumPad9}},
            {new WinPos(EWinPosHoriz.Total, EWinPosVert.Top), new List<Forms.Keys>(){Forms.Keys.Up}},
            {new WinPos(EWinPosHoriz.Left, EWinPosVert.Bottom), new List<Forms.Keys>(){Forms.Keys.NumPad1}},
            {new WinPos(EWinPosHoriz.Middle, EWinPosVert.Bottom), new List<Forms.Keys>(){Forms.Keys.NumPad2}},
            {new WinPos(EWinPosHoriz.Right, EWinPosVert.Bottom), new List<Forms.Keys>(){Forms.Keys.NumPad3}},
            {new WinPos(EWinPosHoriz.Total, EWinPosVert.Bottom), new List<Forms.Keys>(){Forms.Keys.Down}},
            {new WinPos(EWinPosHoriz.Left, EWinPosVert.Centre), new List<Forms.Keys>(){Forms.Keys.NumPad4}},
            {new WinPos(EWinPosHoriz.Middle, EWinPosVert.Centre), new List<Forms.Keys>(){Forms.Keys.NumPad5}},
            {new WinPos(EWinPosHoriz.Right, EWinPosVert.Centre), new List<Forms.Keys>(){Forms.Keys.NumPad6}},
            //{new WinPos(EWinPosHoriz.Total, EWinPosVert.Centre), new List<Forms.Keys>(){Forms.Keys.Left, Forms.Keys.NumPad4}},
        };

        static ResizeHotkeyListHelper()
        {
        }

        public static void SetToDefault(this ResizerHotkeyList rhkList)
        {
            rhkList.Clear();
            bool isError = false;
            StringBuilder sbError = new StringBuilder();

            foreach (WinPos wp in _posCommandKey.Keys)
            {
                ResizerHotKey hk = new ResizerHotKey();
                foreach (Forms.Keys key in _posCommandKey[wp])
                {
                    try
                    {
                        hk.Add(_defaultModifiers, key);
                    }
                    catch (HotkeyAlreadyInUseException ex)
                    {
                        sbError.AppendFormat("Couldn't register: \"{0} + {1}\" hotkey. It won't work\r\n", _defaultModifiers, key);
                        isError = true;
                    }
                }

                if (hk.Count != 0)
                {
                    foreach (double portion in _portions)
                        hk.ResizeStates.Add(CreateResizeState(wp, portion));

                    rhkList.Add(hk);
                }
            }
            if (isError)
            {
                Application.Current.Dispatcher.BeginInvoke(new Action(delegate()
                {
                    System.Windows.Forms.MessageBox.Show(sbError.ToString());
                }
                    ));
            }
        }

        static private ResizerHotkeyState CreateResizeState(WinPos wp, double portion)
        {
            Vector location = new Vector();
            Vector size = new Vector();

            #region Size calculation

            if (wp.HPos == EWinPosHoriz.Total)
                size.X = 1;
            else
                size.X = portion;
            if (wp.VPos == EWinPosVert.Total)
                size.Y = 1;
            else
                size.Y = portion;

            #endregion Size calculation

            #region Location calculation

            switch (wp.HPos)
            {
                case EWinPosHoriz.Middle:
                    {
                        location.X = (1 - portion) / 2.0;

                        break;
                    }
                case EWinPosHoriz.Right:
                    {
                        location.X = 1 - portion;

                        break;
                    }
                case EWinPosHoriz.Left:
                case EWinPosHoriz.Total:
                default:
                    {
                        location.X = 0;

                        break;
                    }
            }
            switch (wp.VPos)
            {
                case EWinPosVert.Centre:
                    {
                        location.Y = (1 - portion) / 2.0;

                        break;
                    }
                case EWinPosVert.Bottom:
                    {
                        location.Y = 1 - portion;

                        break;
                    }
                case EWinPosVert.Top:
                case EWinPosVert.Total:
                default:
                    {
                        location.Y = 0;

                        break;
                    }
            }

            #endregion Location calculation

            return new ResizerHotkeyState(location, size);
        }

        #endregion Set to Default

        #region Open options

        private static string BinaryOptionsFilePath = ".\\Options\\probe.bin";
        private static string XmlOptionsFilePath = ".\\Options\\probe.xml";

        public static void Init(this ResizerHotkeyList rhkList_in)
        {
            FileInfo BinaryOptionsFileInfo = new FileInfo(BinaryOptionsFilePath);
            FileInfo XmlOptionsFileInfo = new FileInfo(XmlOptionsFilePath);
            if (BinaryOptionsFileInfo.Exists)
                rhkList_in.DeserializeThisFrom(BinaryOptionsFilePath, ESerializationType.Binary);
            else if (XmlOptionsFileInfo.Exists)
                rhkList_in.DeserializeThisFrom(XmlOptionsFilePath, ESerializationType.Xml);
            else
                rhkList_in.SetToDefault();
        }

        public static void Save(this ResizerHotkeyList rhkList_in)
        {
            rhkList_in.SerializeThisTo(BinaryOptionsFilePath, ESerializationType.Binary);
            rhkList_in.SerializeThisTo(XmlOptionsFilePath, ESerializationType.Xml);
        }

        public enum ESerializationType
        {
            Binary,
            Xml,
            Json
        }

        public static void SerializeThisTo(this ResizerHotkeyList rhkList_in, string fileName_in)
        {
            SerializeThisTo(rhkList_in, fileName_in, ESerializationType.Binary);
        }

        public static void DeserializeThisFrom(this ResizerHotkeyList rhkList_in, string fileName_in)
        {
            DeserializeThisFrom(rhkList_in, fileName_in, ESerializationType.Binary);
        }

        public static void SerializeThisTo(this ResizerHotkeyList rhkList_in, string fileName_in, ESerializationType serializationType_in)
        {
            switch (serializationType_in)
            {
                case ESerializationType.Binary:
                    BinSerializeThisTo(rhkList_in, fileName_in);
                    break;

                case ESerializationType.Xml:
                    XmlSerializeThisTo(rhkList_in, fileName_in);
                    break;

                case ESerializationType.Json:
                    throw new NotImplementedException();
                default:
                    throw new InvalidOperationException();
            }
        }

        public static void DeserializeThisFrom(this ResizerHotkeyList rhkList_in, string fileName_in, ESerializationType serializationType_in)
        {
            switch (serializationType_in)
            {
                case ESerializationType.Binary:
                    BinDeserializeThisFrom(rhkList_in, fileName_in);
                    break;

                case ESerializationType.Xml:
                    XmlDeserializeThisFrom(rhkList_in, fileName_in);
                    break;

                case ESerializationType.Json:
                    throw new NotImplementedException();
                default:
                    throw new InvalidOperationException();
            }
        }

        private static void XmlSerializeThisTo(this ResizerHotkeyList rhkList_in, string fileName_in)
        {
            XmlSerializer mySerializer = new XmlSerializer(typeof(ResizerHotkeyList));
            using (StreamWriter myWriter = new StreamWriter(fileName_in))
                mySerializer.Serialize(myWriter, rhkList_in);
        }

        private static void XmlDeserializeThisFrom(this ResizerHotkeyList rhkList_in, string fileName_in)
        {
            XmlSerializer mySerializer = new XmlSerializer(typeof(ResizerHotkeyList));
            using (FileStream myFileStream = new FileStream(fileName_in, FileMode.Open))
                rhkList_in.AddRange((ResizerHotkeyList)mySerializer.Deserialize(myFileStream));
        }

        //public static void SerializeThis(this ResizerHotkeyList rhkList_in)
        //{
        //    String s = JSON.Instance.ToJSON(rhkList_in);
        //}

        private static void BinSerializeThisTo(this ResizerHotkeyList rhkList_in, string fileName_in)
        {
            BinaryFormatter formatter = new BinaryFormatter();
            using (FileStream fs = new FileStream(fileName_in, FileMode.Create))
                formatter.Serialize(fs, rhkList_in);
        }

        private static void BinDeserializeThisFrom(this ResizerHotkeyList rhkList_in, string fileName_in)
        {
            BinaryFormatter formatter = new BinaryFormatter();
            using (FileStream fs = new FileStream(fileName_in, FileMode.Open))
                rhkList_in.AddRange((ResizerHotkeyList)formatter.Deserialize(fs));
        }

        #endregion Open options
    }
}