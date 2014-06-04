using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace DragDrop
{
    public static class XmlHelper
    {
        public static T XmlDeserialize<T>(this Stream stream)
        {
            var serializer = new System.Xml.Serialization.XmlSerializer(typeof(T));
            T theObject = (T)serializer.Deserialize(stream);
            return theObject;
        }

        public static Stream XmlSerialize(this Stream stream, object theObject)
        {
            var serializer = new System.Xml.Serialization.XmlSerializer(theObject.GetType());
            serializer.Serialize(stream, theObject);
            return stream;
        }
    }
}