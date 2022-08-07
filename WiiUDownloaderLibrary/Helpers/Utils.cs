using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml.Serialization;
using WiiUDownloaderLibrary.Models;

namespace WiiUDownloaderLibrary.Helpers
{
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

        public static ushort ToUInt16(this byte[] byteArray)
        {
            return BitConverter.ToUInt16(byteArray.FixEndianness(), 0);
        }

        public static uint ToUInt32(this byte[] byteArray)
        {
            return BitConverter.ToUInt32(byteArray.FixEndianness(), 0);
        }

        public static ulong ToUInt64(this byte[] byteArray)
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
            char[] badChars = Path.GetInvalidFileNameChars();

            for (int i = 0; i < badChars.Length; i++)
                fileName = fileName.Replace(badChars[i], '_');

            return fileName;
        }


        public static string GetByteSuffix(int power, bool IEC)
        {
            string baseStr = "B";
            if (IEC)
                baseStr = "iB";

            return power switch
            {
                1 => "K" + baseStr,
                2 => "M" + baseStr,
                3 => "G" + baseStr,
                4 => "T" + baseStr,
                _ => "B",
            };
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
                    byteNum /= Math.Pow(baseNum, i);
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
            FileInfo fi = new(filePath);

            return fi.Length;
        }
    }
}
