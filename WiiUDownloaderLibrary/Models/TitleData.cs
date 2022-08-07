using Newtonsoft.Json;
using System;

namespace WiiUDownloaderLibrary.Models
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
        public TitleData(string titleID, string titleKey, string name, string region, string ticket=null)
        {
            if (string.IsNullOrEmpty(name))
                name = "UNKNOWN_TITLE";
            if (string.IsNullOrEmpty(region))
                region = "UNK";

            TitleID = titleID;
            TitleKey = titleKey;
            Name = name;
            Region = region;
            TicketIsAvailable = Convert.ToBoolean(Convert.ToInt32(ticket));
        }

        public TitleData(string titleID)
        {
            TitleID = titleID;
        }

        public TitleData(string titleID, string titleKey)
        {
            TitleID = titleID;
            TitleKey = titleKey;
        }

    }
}
