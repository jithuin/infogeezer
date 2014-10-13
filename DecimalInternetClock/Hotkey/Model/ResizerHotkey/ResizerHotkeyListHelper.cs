using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Windows;
using System.Xml.Serialization;

using ManagedWinapi;
using Forms = System.Windows.Forms;

namespace HotKey
{
    public static class ResizerHotkeyListHelper
    {
        #region Set to Default

        private static List<double> _portions = new List<double>() { 0.25, 1.0 / 3, 0.5, 0.75 };

        private static FKeyModifiers _defaultModifiers = FKeyModifiers.Alt | FKeyModifiers.Ctrl;

        private struct Alignment
        {
            public Alignment(HorizontalAlignment horizontalAlignment_in, VerticalAlignment verticalAlignment_in)
            {
                horizontalAlignment = horizontalAlignment_in;
                verticalAlignment = verticalAlignment_in;
            }

            public HorizontalAlignment horizontalAlignment;
            public VerticalAlignment verticalAlignment;
        }

        private static Dictionary<Alignment, List<Forms.Keys>> _posCommandKey = new Dictionary<Alignment, List<Forms.Keys>>()
        {
            {new Alignment(HorizontalAlignment.Left, VerticalAlignment.Stretch), new List<Forms.Keys>(){Forms.Keys.Left}},
            {new Alignment(HorizontalAlignment.Right, VerticalAlignment.Stretch), new List<Forms.Keys>(){Forms.Keys.Right}},

            {new Alignment(HorizontalAlignment.Left, VerticalAlignment.Top), new List<Forms.Keys>(){Forms.Keys.NumPad7}},
            {new Alignment(HorizontalAlignment.Center, VerticalAlignment.Top), new List<Forms.Keys>(){Forms.Keys.NumPad8}},
            {new Alignment(HorizontalAlignment.Right, VerticalAlignment.Top), new List<Forms.Keys>(){Forms.Keys.NumPad9}},
            {new Alignment(HorizontalAlignment.Stretch, VerticalAlignment.Top), new List<Forms.Keys>(){Forms.Keys.Up}},
            {new Alignment(HorizontalAlignment.Left, VerticalAlignment.Bottom), new List<Forms.Keys>(){Forms.Keys.NumPad1}},
            {new Alignment(HorizontalAlignment.Center, VerticalAlignment.Bottom), new List<Forms.Keys>(){Forms.Keys.NumPad2}},
            {new Alignment(HorizontalAlignment.Right, VerticalAlignment.Bottom), new List<Forms.Keys>(){Forms.Keys.NumPad3}},
            {new Alignment(HorizontalAlignment.Stretch, VerticalAlignment.Bottom), new List<Forms.Keys>(){Forms.Keys.Down}},
            {new Alignment(HorizontalAlignment.Left, VerticalAlignment.Center), new List<Forms.Keys>(){Forms.Keys.NumPad4}},
            {new Alignment(HorizontalAlignment.Center, VerticalAlignment.Center), new List<Forms.Keys>(){Forms.Keys.NumPad5}},
            {new Alignment(HorizontalAlignment.Right, VerticalAlignment.Center), new List<Forms.Keys>(){Forms.Keys.NumPad6}},
        };

        static ResizerHotkeyListHelper()
        {
        }

        public static void SetToDefault(this ResizerHotkeyList rhkList)
        {
            rhkList.Clear();
            bool isError = false;
            StringBuilder sbError = new StringBuilder();

            foreach (Alignment wp in _posCommandKey.Keys)
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
                        hk.ResizeStates.Add(new ResizerHotkeyState(wp.horizontalAlignment, wp.verticalAlignment, portion));

                    rhkList.Add(hk);
                }
            }
            if (isError)
            {
                Application.Current.Dispatcher.BeginInvoke(new Action(delegate()
                {
                    System.Windows.Forms.MessageBox.Show(sbError.ToString());
                }));
            }
        }

        #endregion Set to Default

        #region Open options

        private static string BinaryOptionsFilePath = ".\\Options\\probe.bin";
        private static string XmlOptionsFilePath = ".\\Options\\probe.xml";

        public static void Init(this ResizerHotkeyList rhkList_in)
        {
            FileInfo BinaryOptionsFileInfo = new FileInfo(BinaryOptionsFilePath);
            FileInfo XmlOptionsFileInfo = new FileInfo(XmlOptionsFilePath);

            try
            {
                if (XmlOptionsFileInfo.Exists)
                    rhkList_in.DeserializeThisFrom(XmlOptionsFilePath, ESerializationType.Xml);
                else if (BinaryOptionsFileInfo.Exists)
                    rhkList_in.DeserializeThisFrom(BinaryOptionsFilePath, ESerializationType.Binary);
                else
                    rhkList_in.SetToDefault();
            }
            catch (Exception)
            {
                Application.Current.Dispatcher.BeginInvoke(new Action(delegate()
                {
                    System.Windows.Forms.MessageBox.Show("Resizer Hotkey Init Error");
                }));
                rhkList_in.SetToDefault();
            }
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