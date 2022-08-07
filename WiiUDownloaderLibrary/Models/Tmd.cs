using WiiUDownloaderLibrary.Helpers;

namespace WiiUDownloaderLibrary.Models
{
    public class Tmd
    {
        private byte[] TmdData { get; set; }

        public Tmd(byte[] tmdData)
        {
            TmdData = tmdData;
        }

        public byte[] ExportTmdData()
        {
            return TmdData;
        }

        public ushort GetTitleVersion()
        {
            byte[] version = new byte[2] { TmdData[0x1DC], TmdData[0x1DD] };
            return version.ToUInt16();
        }

        public uint GetContentCount()
        {
            byte[] count = new byte[2] { TmdData[0x1DE], TmdData[0x1DF] };
            return count.ToUInt16();
        }

        public uint GetContentID(uint contentIndex)
        {
            uint contentDataLoc = 0xB04 + (0x30 * contentIndex);

            byte[] id = new byte[4] { TmdData[contentDataLoc], TmdData[contentDataLoc + 1], TmdData[contentDataLoc + 2], TmdData[contentDataLoc + 3], };

            return id.ToUInt32();
        }

        public string GetContentIDString(uint contentIndex)
        {
            uint contentDataLoc = 0xB04 + (0x30 * contentIndex);

            byte[] id = new byte[4] { TmdData[contentDataLoc], TmdData[contentDataLoc + 1], TmdData[contentDataLoc + 2], TmdData[contentDataLoc + 3], };

            string contentString = "";
            foreach (byte idbyte in id)
                contentString += idbyte.ToString("x2");

            return contentString;
        }

        public uint GetContentType(uint contentIndex)
        {
            uint contentDataLoc = 0xB04 + (0x30 * contentIndex);

            byte[] type = new byte[2] { TmdData[contentDataLoc + 0x6], TmdData[contentDataLoc + 0x7] };

            return type.ToUInt16();
        }

        public ulong GetContentSize(uint contentIndex)
        {
            uint contentDataLoc = 0xB04 + (0x30 * contentIndex);

            byte[] size = new byte[8]
            {
                TmdData[contentDataLoc + 0x8],
                TmdData[contentDataLoc + 0x9],
                TmdData[contentDataLoc + 0xA],
                TmdData[contentDataLoc + 0xB],
                TmdData[contentDataLoc + 0xC],
                TmdData[contentDataLoc + 0xD],
                TmdData[contentDataLoc + 0xE],
                TmdData[contentDataLoc + 0xF] };

            return size.ToUInt64();
        }
    }
}
