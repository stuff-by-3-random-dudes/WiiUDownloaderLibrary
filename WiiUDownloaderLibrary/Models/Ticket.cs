using System;
using WiiUDownloaderLibrary.Helpers;

namespace WiiUDownloaderLibrary.Models
{
    public class Ticket
    {
        private static readonly byte[] TICKET_TEMPLATE = Utils.GetByteArrayFromHexString("00010004d15ea5ed15abe11ad15ea5ed15abe11ad15ea5ed15abe11ad15ea5ed15abe11ad15ea5ed15abe11ad15ea5ed15abe11ad15ea5ed15abe11ad15ea5ed15abe11ad15ea5ed15abe11ad15ea5ed15abe11ad15ea5ed15abe11ad15ea5ed15abe11ad15ea5ed15abe11ad15ea5ed15abe11ad15ea5ed15abe11ad15ea5ed15abe11ad15ea5ed15abe11ad15ea5ed15abe11ad15ea5ed15abe11ad15ea5ed15abe11ad15ea5ed15abe11ad15ea5ed15abe11ad15ea5ed15abe11ad15ea5ed15abe11ad15ea5ed15abe11ad15ea5ed15abe11ad15ea5ed15abe11ad15ea5ed15abe11ad15ea5ed15abe11ad15ea5ed15abe11ad15ea5ed15abe11ad15ea5ed15abe11a000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000526f6f742d434130303030303030332d585330303030303030630000000000000000000000000000000000000000000000000000000000000000000000000000feedfacefeedfacefeedfacefeedfacefeedfacefeedfacefeedfacefeedfacefeedfacefeedfacefeedfacefeedfacefeedfacefeedfacefeedface010000cccccccccccccccccccccccccccccccc00000000000000000000000000aaaaaaaaaaaaaaaa00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000010000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000010014000000ac000000140001001400000000000000280000000100000084000000840003000000000000ffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffff000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000");
        private byte[] TicketData { get; set; }

        public Ticket()
        {
            TicketData = TICKET_TEMPLATE;
        }

        public Ticket(byte[] ticket)
        {
            TicketData = ticket.Length != 0x350 ? TICKET_TEMPLATE : ticket;
        }
        public byte[] ExportTicketData()
        {
            return TicketData;
        }
        public void PatchDLCUnlockAll()
        {
            byte[] patch = Utils.GetByteArrayFromHexString("00010014000000AC000000140001001400000000000000280000000100000084000000840003000000000000FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000");

            patch.CopyTo(TicketData, 0x2A4);
        }
        public void PatchDemoKillTimeLimit()
        {
            byte[] patch = new byte[64];
            for (int i = 0; i < patch.Length; i++)
                patch[i] = 0;

            patch.CopyTo(TicketData, 0x264);
        }
        public void PatchTitleVersion(ushort version)
        {
            byte[] patch = BitConverter.GetBytes(version);
            if (BitConverter.IsLittleEndian)
                Array.Reverse(patch);
            patch.CopyTo(TicketData, 0x1E6);
        }
        public void PatchVersionNo(byte majorVer, byte minorVer)
        {
            TicketData[0x1E6] = majorVer;
            TicketData[0x1E7] = minorVer;
        }
        public void PatchTitleID(string titleIdStr)
        {
            byte[] patch = Utils.GetByteArrayFromHexString(titleIdStr);
            patch.CopyTo(TicketData, 0x1DC);
        }
        public void PatchTitleKey(string titleKeyStr)
        {
            byte[] patch = Utils.GetByteArrayFromHexString(titleKeyStr);
            patch.CopyTo(TicketData, 0x1BF);
        }
    }
}
