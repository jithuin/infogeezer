using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Xml.Serialization;
using DecimalInternetClock.HotKeys;
using fastJSON;

namespace DecimalInternetClock.Helpers
{
    public static class SerializerHelper
    {
        public static void SerializeThisTo(this ResizerHotkeyList rhkList_in, String fileName_in)
        {
            XmlSerializer mySerializer = new XmlSerializer(typeof(ResizerHotkeyList));
            using (StreamWriter myWriter = new StreamWriter(fileName_in))
                mySerializer.Serialize(myWriter, rhkList_in);
        }

        public static void DeserializeThisFrom(this ResizerHotkeyList rhkList_in, String fileName_in)
        {
            XmlSerializer mySerializer = new XmlSerializer(typeof(ResizerHotkeyList));
            using (FileStream myFileStream = new FileStream(fileName_in, FileMode.Open))
                rhkList_in.AddRange((ResizerHotkeyList)mySerializer.Deserialize(myFileStream));
        }

        public static void SerializeThis(this ResizerHotkeyList rhkList_in)
        {
            String s = JSON.Instance.ToJSON(rhkList_in);
        }

        public static void BinSerializeThisTo(this ResizerHotkeyList rhkList_in, String fileName_in)
        {
            BinaryFormatter formatter = new BinaryFormatter();
            using (FileStream fs = new FileStream(fileName_in, FileMode.Create))
                formatter.Serialize(fs, rhkList_in);
        }

        public static void BinDeserializeThisFrom(this ResizerHotkeyList rhkList_in, String fileName_in)
        {
            BinaryFormatter formatter = new BinaryFormatter();
            using (FileStream fs = new FileStream(fileName_in, FileMode.Open))
                rhkList_in.AddRange((ResizerHotkeyList)formatter.Deserialize(fs));
        }
    }
}