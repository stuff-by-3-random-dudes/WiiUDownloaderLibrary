using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml.Serialization;

namespace WiiUDownloaderLibrary
{
    [JsonObject]
    public class TitleData
    {
        [JsonProperty("titleID")]
        public string TitleID { get; set; }

        [JsonProperty("titleKey")]
        public string TitleKey { get; set; }

        public string Name { get; set; }

        public string Region { get; set; }

        public bool TicketIsAvailable { get; set; }

        [JsonConstructor]
        public TitleData(string _titleID, string _titleKey, string name, string region, string ticket)
        {
            if (name == null || name == "")
                name = "UNKNOWN_TITLE";
            if (region == null || region == "")
                region = "UNK";

            TitleID = _titleID;
            TitleKey = _titleKey;
            Name = name;
            Region = region;
            TicketIsAvailable = Convert.ToBoolean(Convert.ToInt32(ticket));
        }

    }
    public  class Tmd
    {
        private byte[] tmdData { get; set; }

        public Tmd(byte[] _tmdData)
        {
            tmdData = _tmdData;
        }

        public byte[] ExportTmdData()
        {
            return tmdData;
        }

        public UInt16 GetTitleVersion()
        {
            byte[] version = new byte[2] { tmdData[0x1DC], tmdData[0x1DD] };
            return version.ToUInt16();
        }

        public uint GetContentCount()
        {
            byte[] count = new byte[2] { tmdData[0x1DE], tmdData[0x1DF] };
            return count.ToUInt16();
        }

        public uint GetContentID(uint contentIndex)
        {
            uint contentDataLoc = 0xB04 + (0x30 * contentIndex);

            byte[] id = new byte[4] { tmdData[contentDataLoc], tmdData[contentDataLoc + 1], tmdData[contentDataLoc + 2], tmdData[contentDataLoc + 3], };

            return id.ToUInt32();
        }

        public string GetContentIDString(uint contentIndex)
        {
            uint contentDataLoc = 0xB04 + (0x30 * contentIndex);

            byte[] id = new byte[4] { tmdData[contentDataLoc], tmdData[contentDataLoc + 1], tmdData[contentDataLoc + 2], tmdData[contentDataLoc + 3], };

            string contentString = "";
            foreach (byte idbyte in id)
                contentString += idbyte.ToString("x2");

            return contentString;
        }

        public uint GetContentType(uint contentIndex)
        {
            uint contentDataLoc = 0xB04 + (0x30 * contentIndex);

            byte[] type = new byte[2] { tmdData[contentDataLoc + 0x6], tmdData[contentDataLoc + 0x7] };

            return type.ToUInt16();
        }

        public UInt64 GetContentSize(uint contentIndex)
        {
            uint contentDataLoc = 0xB04 + (0x30 * contentIndex);

            byte[] size = new byte[8]
            {
                tmdData[contentDataLoc + 0x8],
                tmdData[contentDataLoc + 0x9],
                tmdData[contentDataLoc + 0xA],
                tmdData[contentDataLoc + 0xB],
                tmdData[contentDataLoc + 0xC],
                tmdData[contentDataLoc + 0xD],
                tmdData[contentDataLoc + 0xE],
                tmdData[contentDataLoc + 0xF] };

            return size.ToUInt64();
        }
    }
    public class Dummy : XmlSerializationReader
    {
        public static byte[] FromHex(string hext)
        {
            return Dummy.ToByteArrayHex(hext);
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
    public static class Utils
    {
        public static dynamic GetJsonObject(string jsonString)
        {
            dynamic json = JsonConvert.DeserializeObject<dynamic>(jsonString);
            return json;
        }

        public static string ToJson(this object obj)
        {
            return JsonConvert.SerializeObject(obj);
        }

        public static byte[] GetByteArrayFromHexString(string hexString)
        {
            
            return Dummy.FromHex(hexString);
            //SoapHexBinary shb = SoapHexBinary.Parse(hexString);
            //return shb.Value;
        }

        public static UInt16 ToUInt16(this byte[] byteArray)
        {
            return BitConverter.ToUInt16(byteArray.FixEndianness(), 0);
        }

        public static UInt32 ToUInt32(this byte[] byteArray)
        {
            return BitConverter.ToUInt32(byteArray.FixEndianness(), 0);
        }

        public static UInt64 ToUInt64(this byte[] byteArray)
        {
            return BitConverter.ToUInt64(byteArray.FixEndianness(), 0);
        }

        public static byte[] FixEndianness(this byte[] byteArray)
        {
            if (BitConverter.IsLittleEndian)
                Array.Reverse(byteArray);

            return byteArray;
        }

        public static string SanitizeFileName(this string fileName)
        {
            char[] badChars = System.IO.Path.GetInvalidFileNameChars();

            for (int i = 0; i < badChars.Length; i++)
                fileName = fileName.Replace(badChars[i], '_');

            return fileName;
        }


        public static string GetByteSuffix(int power, bool IEC)
        {
            string baseStr = "B";
            if (IEC)
                baseStr = "iB";

            switch (power)
            {
                case (0):
                default:
                    return "B";

                case (1):
                    return "K" + baseStr;

                case (2):
                    return "M" + baseStr;

                case (3):
                    return "G" + baseStr;

                case (4):
                    return "T" + baseStr;
            }
        }

        public static string ConvertByteToText(this double byteNum, bool IEC = false)
        {
            string suffix = "B";

            int baseNum = 1000;
            if (IEC)
                baseNum = 1024;

            for (int i = 3; i > 0; i--)
            {
                if (byteNum.CompareTo(Math.Pow(baseNum, i)) == 1)
                {
                    byteNum = byteNum / Math.Pow(baseNum, i);
                    return byteNum.ToString("N2") + " " + GetByteSuffix(i, IEC);
                }
            }
            return byteNum.ToString() + " " + suffix;
        }

        //public static string ConvertByteToText2(this double byteNum)
        //{
        //    string suffix = "B";
        //    for (int i = 3; i > 0; i--)
        //    {
        //        if (byteNum.CompareTo(Math.Pow(1000, i)) == 1)
        //        {
        //            byteNum = byteNum / Math.Pow(1000, i);
        //            return byteNum.ToString("N2") + " " + GetByteSuffix(i);
        //        }
        //    }
        //    return byteNum.ToString() + " " + suffix;
        //}

        public static long GetFileLength(this string filePath)
        {
            FileInfo fi = new FileInfo(filePath);

            return fi.Length;
        }
    }
    public class Ticket
    {
        private static byte[] TICKET_TEMPLATE = Utils.GetByteArrayFromHexString("00010004d15ea5ed15abe11ad15ea5ed15abe11ad15ea5ed15abe11ad15ea5ed15abe11ad15ea5ed15abe11ad15ea5ed15abe11ad15ea5ed15abe11ad15ea5ed15abe11ad15ea5ed15abe11ad15ea5ed15abe11ad15ea5ed15abe11ad15ea5ed15abe11ad15ea5ed15abe11ad15ea5ed15abe11ad15ea5ed15abe11ad15ea5ed15abe11ad15ea5ed15abe11ad15ea5ed15abe11ad15ea5ed15abe11ad15ea5ed15abe11ad15ea5ed15abe11ad15ea5ed15abe11ad15ea5ed15abe11ad15ea5ed15abe11ad15ea5ed15abe11ad15ea5ed15abe11ad15ea5ed15abe11ad15ea5ed15abe11ad15ea5ed15abe11ad15ea5ed15abe11ad15ea5ed15abe11ad15ea5ed15abe11a000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000526f6f742d434130303030303030332d585330303030303030630000000000000000000000000000000000000000000000000000000000000000000000000000feedfacefeedfacefeedfacefeedfacefeedfacefeedfacefeedfacefeedfacefeedfacefeedfacefeedfacefeedfacefeedfacefeedfacefeedface010000cccccccccccccccccccccccccccccccc00000000000000000000000000aaaaaaaaaaaaaaaa00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000010000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000010014000000ac000000140001001400000000000000280000000100000084000000840003000000000000ffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffff000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000");
        private byte[] ticketData { get; set; }

        public Ticket()
        {
            ticketData = TICKET_TEMPLATE;
        }

        public Ticket(byte[] _ticket)
        {
            if (_ticket.Length != 0x350)
                ticketData = TICKET_TEMPLATE;
            else
                ticketData = _ticket;
        }

        public byte[] ExportTicketData()
        {
            return ticketData;
        }

        public void PatchDLCUnlockAll()
        {
            byte[] patch = Utils.GetByteArrayFromHexString("00010014000000AC000000140001001400000000000000280000000100000084000000840003000000000000FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000");

            patch.CopyTo(ticketData, 0x2A4);
        }

        public void PatchDemoKillTimeLimit()
        {
            byte[] patch = new byte[64];
            for (int i = 0; i < patch.Length; i++)
                patch[i] = 0;

            patch.CopyTo(ticketData, 0x264);
        }

        public void PatchTitleVersion(UInt16 version)
        {
            byte[] patch = BitConverter.GetBytes(version);
            if (BitConverter.IsLittleEndian)
                Array.Reverse(patch);
            patch.CopyTo(ticketData, 0x1E6);
        }

        public void PatchVersionNo(byte majorVer, byte minorVer)
        {
            ticketData[0x1E6] = majorVer;
            ticketData[0x1E7] = minorVer;
        }

        public void PatchTitleID(string titleIdStr)
        {
            byte[] patch = Utils.GetByteArrayFromHexString(titleIdStr);
            patch.CopyTo(ticketData, 0x1DC);
        }

        public void PatchTitleKey(string titleKeyStr)
        {
            byte[] patch = Utils.GetByteArrayFromHexString(titleKeyStr);
            patch.CopyTo(ticketData, 0x1BF);
        }
    }
}
