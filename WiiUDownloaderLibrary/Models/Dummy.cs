using System;
using System.Xml.Serialization;

namespace WiiUDownloaderLibrary.Models
{
    //I don't understand the point of any of this code
    public class Dummy : XmlSerializationReader
    {
        public static byte[] FromHex(string hext)
        {
            return ToByteArrayHex(hext);
        }
        protected override void InitCallbacks()
        {
            throw new NotImplementedException();
        }
        protected override void InitIDs()
        {
            throw new NotImplementedException();
        }
    }
}
