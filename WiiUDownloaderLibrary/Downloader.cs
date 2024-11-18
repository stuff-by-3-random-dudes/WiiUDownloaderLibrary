using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using WiiUDownloaderLibrary.Helpers;
using WiiUDownloaderLibrary.Models;
using WiiUDownloaderLibrary.Models.DefaultInjectors;

namespace WiiUDownloaderLibrary
{
    public class Downloader
    {
        private const string TYPE_GAME = "GAME";
        private const string TYPE_DEMO = "DEMO";
        private const string TYPE_UPDATE = "GAME-UPDATE";
        private const string TYPE_DLC = "GAME-DLC";
        private const string TYPE_SYSAPP = "SYSTEM-APP";
        private const string TYPE_SYSDATA = "SYSTEM-DATA";
        private const string TYPE_BACKGROUND = "BACKGROUND-TITLE";
        private const string NINTYCDN_BASEURL = "http://ccs.cdn.c.shop.nintendowifi.net/ccs/download/";
        private const string HEXSTRING = "00010003704138EFBBBDA16A987DD901326D1C9459484C88A2861B91A312587AE70EF6237EC50E1032DC39DDE89A96A8E859D76A98A6E7E36A0CFE352CA893058234FF833FCB3B03811E9F0DC0D9A52F8045B4B2F9411B67A51C44B5EF8CE77BD6D56BA75734A1856DE6D4BED6D3A242C7C8791B3422375E5C779ABF072F7695EFA0F75BCB83789FC30E3FE4CC8392207840638949C7F688565F649B74D63D8D58FFADDA571E9554426B1318FC468983D4C8A5628B06B6FC5D507C13E7A18AC1511EB6D62EA5448F83501447A9AFB3ECC2903C9DD52F922AC9ACDBEF58C6021848D96E208732D3D1D9D9EA440D91621C7A99DB8843C59C1F2E2C7D9B577D512C166D6F7E1AAD4A774A37447E78FE2021E14A95D112A068ADA019F463C7A55685AABB6888B9246483D18B9C806F474918331782344A4B8531334B26303263D9D2EB4F4BB99602B352F6AE4046C69A5E7E8E4A18EF9BC0A2DED61310417012FD824CC116CFB7C4C1F7EC7177A17446CBDE96F3EDD88FCD052F0B888A45FDAF2B631354F40D16E5FA9C2C4EDA98E798D15E6046DC5363F3096B2C607A9D8DD55B1502A6AC7D3CC8D8C575998E7D796910C804C495235057E91ECD2637C9C1845151AC6B9A0490AE3EC6F47740A0DB0BA36D075956CEE7354EA3E9A4F2720B26550C7D394324BC0CB7E9317D8A8661F42191FF10B08256CE3FD25B745E5194906B4D61CB4C2E000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000526F6F742D43413030303030303033000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000143503030303030303062000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000137A080BA689C590FD0B2F0D4F56B632FB934ED0739517B33A79DE040EE92DC31D37C7F73BF04BD3E44E20AB5A6FEAF5984CC1F6062E9A9FE56C3285DC6F25DDD5D0BF9FE2EFE835DF2634ED937FAB0214D104809CF74B860E6B0483F4CD2DAB2A9602BC56F0D6BD946AED6E0BE4F08F26686BD09EF7DB325F82B18F6AF2ED525BFD828B653FEE6ECE400D5A48FFE22D538BB5335B4153342D4335ACF590D0D30AE2043C7F5AD214FC9C0FE6FA40A5C86506CA6369BCEE44A32D9E695CF00B4FD79ADB568D149C2028A14C9D71B850CA365B37F70B657791FC5D728C4E18FD22557C4062D74771533C70179D3DAE8F92B117E45CB332F3B3C2A22E705CFEC66F6DA3772B000100010000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000010004919EBE464AD0F552CD1B72E7884910CF55A9F02E50789641D896683DC005BD0AEA87079D8AC284C675065F74C8BF37C88044409502A022980BB8AD48383F6D28A79DE39626CCB2B22A0F19E41032F094B39FF0133146DEC8F6C1A9D55CD28D9E1C47B3D11F4F5426C2C780135A2775D3CA679BC7E834F0E0FB58E68860A71330FC95791793C8FBA935A7A6908F229DEE2A0CA6B9B23B12D495A6FE19D0D72648216878605A66538DBF376899905D3445FC5C727A0E13E0E2C8971C9CFA6C60678875732A4E75523D2F562F12AABD1573BF06C94054AEFA81A71417AF9A4A066D0FFC5AD64BAB28B1FF60661F4437D49E1E0D9412EB4BCACF4CFD6A3408847982000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000526F6F742D43413030303030303033000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000158533030303030303063000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000137A0894AD505BB6C67E2E5BDD6A3BEC43D910C772E9CC290DA58588B77DCC11680BB3E29F4EABBB26E98C2601985C041BB14378E689181AAD770568E928A2B98167EE3E10D072BEEF1FA22FA2AA3E13F11E1836A92A4281EF70AAF4E462998221C6FBB9BDD017E6AC590494E9CEA9859CEB2D2A4C1766F2C33912C58F14A803E36FCCDCCCDC13FD7AE77C7A78D997E6ACC35557E0D3E9EB64B43C92F4C50D67A602DEB391B06661CD32880BD64912AF1CBCB7162A06F02565D3B0ECE4FCECDDAE8A4934DB8EE67F3017986221155D131C6C3F09AB1945C206AC70C942B36F49A1183BCD78B6E4B47C6C5CAC0F8D62F897C6953DD12F28B70C5B7DF751819A98346526250001000100000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000";

        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<Downloader> _logger;

        public Downloader(IHttpClientFactory httpClientFactory, ILogger<Downloader> logger)
        {
            _httpClientFactory = httpClientFactory ?? new DefaultHttpClientFactory();
            _logger = logger ?? new DefaultLogger();
        }

        private static readonly Dictionary<string, string> TitleTypeMap = new Dictionary<string, string>
        {
            { "00", TYPE_GAME },
            { "02", TYPE_DEMO },
            { "0c", TYPE_DLC },
            { "0e", TYPE_UPDATE },
            { "10", TYPE_SYSAPP },
            { "1b", TYPE_SYSDATA },
            { "30", TYPE_BACKGROUND }
        };

        private static string GetTitleType(string titleID)
        {
            var key = new string(new char[2] { titleID[6], titleID[7] });
            return TitleTypeMap.TryGetValue(key, out var titleType) ? titleType : string.Empty;
        }

        private static async Task SaveFileAsync(string filePath, byte[] data)
        {
            try
            {
                using var fs = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None, 4096, useAsync: true);
                await fs.WriteAsync(data, 0, data.Length).ConfigureAwait(false);
            }
            catch (IOException ioe)
            {
                throw new IOException($"ERROR! Could not save {Path.GetFileName(filePath)}", ioe);
            }
        }

        private static void SetUpForTitle(string saveDir, string saveFolder)
        {
            SetUpDirectory(saveDir, saveFolder);
        }

        public async Task DownloadAsync(TitleData td, string saveFolder, IProgress<double> progress = null)
        {
            try
            {
                var saveDir = Path.Combine(saveFolder, td.TitleID);
                var baseURL = NINTYCDN_BASEURL + td.TitleID + "/";

                SetUpForTitle(saveDir, saveFolder);
                _logger.LogInformation("Currently downloading title: {TitleID}", td.TitleID);
                _logger.LogInformation("Downloading TMD from Nintendo CDN...");

                var tmd = await GetTmdAsync(baseURL).ConfigureAwait(false);

                await SaveFileAsync(Path.Combine(saveDir, "title.tmd"), tmd.ExportTmdData()).ConfigureAwait(false);
                CheckTitleSize(tmd, saveDir);

                var ticket = new Ticket();
                var fake = true;
                if (!string.IsNullOrEmpty(td.TitleKey))
                    fake = DetermineIfFake(td.TitleID);

                if (fake)
                {
                    using var httpClient = _httpClientFactory.CreateClient();
                    var cetk = await httpClient.GetByteArrayAsync(baseURL + "cetk").ConfigureAwait(false);
                    ticket = GetTicket(cetk);
                }
                else
                    ticket = GetTicket(td, tmd.GetTitleVersion());

                await SaveFileAsync(Path.Combine(saveDir, "title.tik"), ticket.ExportTicketData()).ConfigureAwait(false);
                await SaveFileAsync(Path.Combine(saveDir, "title.cert"), Utils.GetByteArrayFromHexString(HEXSTRING)).ConfigureAwait(false);

                await GetContentFilesAsync(tmd, saveDir, baseURL, progress).ConfigureAwait(false);
                _logger.LogInformation("Download Complete!");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to download title {TitleID}", td.TitleID);
            }
        }

        public Task DownloadAsync(string titleId, string saveFolder, IProgress<double> progress = null)
        {
            var titleData = new TitleData(titleId);
            return DownloadAsync(titleData, saveFolder, progress);
        }

        private static void SetUpDirectory(string saveDir, string saveFolder)
        {
            var temp = "temp";

            Directory.CreateDirectory(saveDir);
            Directory.CreateDirectory(Path.Combine(saveFolder, temp));

            Directory.SetCurrentDirectory(Path.Combine(saveFolder, temp));
        }

        private async Task<Tmd> GetTmdAsync(string baseUrl)
        {
            Tmd tmd;
            using (var httpClient = _httpClientFactory.CreateClient())
            {
                try
                {
                    tmd = new Tmd(await httpClient.GetByteArrayAsync(baseUrl + "tmd").ConfigureAwait(false));
                }
                catch (WebException we)
                {
                    _logger.LogError(we, "Failed to download TMD from {BaseUrl}", baseUrl);
                    throw;
                }
            }
            return tmd;
        }

        private void CheckTitleSize(Tmd tmd, string saveDir)
        {
            ulong titleSize = 0;
            for (int i = 0; i < tmd.GetContentCount(); i++)
                titleSize += tmd.GetContentSize((uint)i);

            var titleSizeStr = string.Format("Estimated Content Size: {0:n0} bytes. (Approx. {1})", titleSize, ((double)titleSize).ConvertByteToText(true));
            _logger.LogInformation(titleSizeStr);

            var currentTitleLogStr = string.Format("{0}" + Environment.NewLine + "{1}", Path.GetFileName(saveDir), titleSizeStr);
            _logger.LogInformation(currentTitleLogStr);
        }

        private static bool DetermineIfFake(string titleId)
        {
            var titleType = GetTitleType(titleId);
            return !(titleType == TYPE_GAME || titleType == TYPE_DEMO || titleType == TYPE_DLC);
        }

        private Ticket GetTicket(byte[] cetk)
        {
            _logger.LogInformation("Downloading Ticket from Nintendo CDN...");

            try
            {
                byte[] tik = new byte[0x350];

                for (int i = 0; i < tik.Length; i++)
                    tik[i] = cetk[i];

                return new Ticket(tik);
            }
            catch (WebException we)
            {
                _logger.LogError(we, "Failed to download Ticket from Nintendo CDN");
                throw;
            }
        }

        private Ticket GetTicket(TitleData titleData, ushort version)
        {
            _logger.LogInformation("Generating Fake Ticket...");

            var ticket = new Ticket();
            ticket.PatchTitleID(titleData.TitleID);
            ticket.PatchTitleKey(titleData.TitleKey);
            ticket.PatchTitleVersion(version);

            return ticket;
        }

        private async Task GetContentFilesAsync(Tmd tmd, string saveDir, string baseUrl, IProgress<double> progress = null)
        {
            uint contentCount = tmd.GetContentCount();
            ulong totalBytes = 0;
            ulong downloadedBytes = 0;

            // Calculate total bytes to download
            for (uint i = 0; i < contentCount; i++)
                totalBytes += tmd.GetContentSize(i);

            for (uint i = 0; i < contentCount; i++)
            {
                var cidStr = tmd.GetContentIDString(i);

                using var httpClient = _httpClientFactory.CreateClient();

                // Download content file
                try
                {
                    var contentBytes = await httpClient.GetByteArrayAsync(new Uri(baseUrl + cidStr)).ConfigureAwait(false);
                    downloadedBytes += (ulong)contentBytes.Length;
                    await SaveFileAsync(Path.Combine(saveDir, cidStr.ToUpper() + ".app"), contentBytes).ConfigureAwait(false);

                    // Report progress
                    progress?.Report((double)downloadedBytes / totalBytes * 100);
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException($"ERROR! Could not download or save {cidStr.ToUpper()}.app", ex);
                }

                // Download and save H3 file
                try
                {
                    var h3Bytes = await httpClient.GetByteArrayAsync(new Uri(baseUrl + cidStr + ".h3")).ConfigureAwait(false);
                    await SaveFileAsync(Path.Combine(saveDir, cidStr.ToUpper() + ".h3"), h3Bytes).ConfigureAwait(false);
                }
                catch (HttpRequestException hre)
                {
                    // Since I can't do catch when this is what I'm doing instead.
                    if (hre.Message.Contains("404"))
                    {
                        // Handle case where H3 file does not exist
                        _logger.LogWarning("{CidStr}.h3 not found, ignoring...", cidStr);
                    }
                    else
                        throw;
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException($"ERROR! Could not download or save {cidStr}.h3", ex);
                }
            }
        }
    }
}
