﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace WiiUDownloaderLibrary
{
    public class Downloader
    {
        public const string TYPE_GAME = "GAME";
        public const string TYPE_DEMO = "DEMO";
        public const string TYPE_UPDATE = "GAME-UPDATE";
        public const string TYPE_DLC = "GAME-DLC";
        public const string TYPE_SYSAPP = "SYSTEM-APP";
        public const string TYPE_SYSDATA = "SYSTEM-DATA";
        public const string TYPE_BACKGROUND = "BACKGROUND-TITLE";

        public const string REG_JPN = "JPN";
        public const string REG_USA = "USA";
        public const string REG_EUR = "EUR";
        public const string REG_CHN = "CHN";
        public const string REG_KOR = "KOR";
        public const string REG_TWN = "TWN";
        public const string REG_UNK = "UNK";
        public const string NINTYCDN_BASEURL = "http://ccs.cdn.c.shop.nintendowifi.net/ccs/download/";
        private static string GetTitleType(string titleID)
        {
            string titleType = "";
            switch (new string(new char[2] { titleID[6], titleID[7] }))
            {
                case "00":
                    titleType = TYPE_GAME;
                    break;

                case "02":
                    titleType = TYPE_DEMO;
                    break;

                case "0c":
                    titleType = TYPE_DLC;
                    break;

                case "0e":
                    titleType = TYPE_UPDATE;
                    break;

                case "10":
                    titleType = TYPE_SYSAPP;
                    break;

                case "1b":
                    titleType = TYPE_SYSDATA;
                    break;

                case "30":
                    titleType = TYPE_BACKGROUND;
                    break;
            }

            return titleType;
        }
        public static void DownloadAsync(TitleData td, string savefolder)
        {
            Thread dlQueueThread = new Thread(() =>
            {
                DownloadTitle(td, savefolder);
            });
            dlQueueThread.IsBackground = true;
            dlQueueThread.Start();
           
        }
        public static void Download(TitleData td, string savefolder)
        {
            Task dlQueueThread = new Task(() =>
            {
                    DownloadTitle(td, savefolder);
            });
            dlQueueThread.Start();
            dlQueueThread.Wait();
        }
        public static void DownloadTitle(TitleData titleInfo, string savefolder)
        {
            using (WebClient wc = new WebClient())
            {
                try
                {

                    string baseUrl = NINTYCDN_BASEURL + titleInfo.TitleID + "/";
                    string titleType = GetTitleType(titleInfo.TitleID);
                    string saveDir = "";

                    //if (config.appDlGroupDlsIntoSubfolders)
                    //    saveDir = String.Format(@"{0}\install\{1} ({2})\{1} ({2}) [{3}] [{4}]", Environment.CurrentDirectory, titleInfo.Name.SanitizeFileName(), titleInfo.Region, titleType, titleInfo.TitleID);
                    //else
                    //    saveDir = String.Format(@"{0}\install\{1} ({2}) [{3}] [{4}]", Environment.CurrentDirectory, titleInfo.Name.SanitizeFileName(), titleInfo.Region, titleType, titleInfo.TitleID);

                    //if (config.appDlGroupDlsIntoSubfolders)
                    //    saveDir = String.Format(@"{0}\{1} ({2})\{1} ({2}) [{3}] [{4}]", config.saveDir, titleInfo.Name.SanitizeFileName(), titleInfo.Region, titleType, titleInfo.TitleID);
                    //else
                    saveDir = Path.Combine(savefolder, titleInfo.TitleID); //String.Format(@"{0}\{1}", savefolder, titleInfo.TitleID);
                   
                    //if (!(Directory.Exists(Environment.CurrentDirectory + "\\install")))
                    //    Directory.CreateDirectory(Environment.CurrentDirectory + "\\install");
                    if (!(Directory.Exists(saveDir)))
                        Directory.CreateDirectory(saveDir);
                    if (!(Directory.Exists(Path.Combine(savefolder, "temp"))))
                        Directory.CreateDirectory(Path.Combine(savefolder, "temp"));
                    Directory.SetCurrentDirectory(Path.Combine(savefolder, "temp"));
                    UInt64 titleSize = 0;
                    Ticket ticket;
                    Tmd tmd;
                    Console.WriteLine("Currently Downloading Title:" + Environment.NewLine + Path.GetFileName(saveDir));
                    

                    try
                    {
                        Console.WriteLine("Downloading TMD from Nintendo CDN...");
                        
                        tmd = new Tmd(wc.DownloadData(baseUrl + "tmd"));
                        Console.WriteLine("Saving TMD - title.tmd");
                        
                        File.WriteAllBytes(Path.Combine(saveDir, "title.tmd"), tmd.ExportTmdData());
                    }
                    catch (WebException we)
                    {
                        string message = "ERROR! Could not download TMD!";
                        throw new WebException(message, we);
                    }
                    catch (IOException ioe)
                    {
                        string message = "ERROR! Could not save title.tmd!";
                        throw new IOException(message, ioe);
                    }


                    for (int i = 0; i < tmd.GetContentCount(); i++)
                        titleSize += tmd.GetContentSize((uint)i);

                    
                    string titleSizeStr = String.Format("Estimated Content Size: {0:n0} bytes. (Approx. {1})", titleSize, ((double)titleSize).ConvertByteToText(true));
                    Console.WriteLine(titleSizeStr);

                    string currentTitleLogStr = String.Format("{0}" + Environment.NewLine + "{1}", Path.GetFileName(saveDir), titleSizeStr);

                    Console.WriteLine(currentTitleLogStr);

                    // Only GAME, DEMO, and GAME-DLC need "Unnofficial" Ticket, either from "That site" or a generated one
                    // So All other types of titles should just grab official ticket direct from CDN
                    if (!(titleType == TYPE_GAME || titleType == TYPE_DEMO || titleType == TYPE_DLC))
                    {
                        Console.WriteLine("Downloading Ticket from Nintendo CDN...");
                        
                        try
                        {
                            byte[] cetk = wc.DownloadData(baseUrl + "cetk");
                            byte[] tik = new byte[0x350];

                            for (int i = 0; i < tik.Length; i++)
                                tik[i] = cetk[i];

                            ticket = new Ticket(tik);
                        }
                        catch (WebException we)
                        {
                            string message = "ERROR! Could not download Ticket from Nintendo CDN!";
                            throw new WebException(message, we);
                        }
                    }
                    else
                    {
                        Console.WriteLine("Generating Fake Ticket...");
                            
                            ticket = new Ticket();
                            ticket.PatchTitleID(titleInfo.TitleID);
                            ticket.PatchTitleKey(titleInfo.TitleKey);
                            ticket.PatchTitleVersion(tmd.GetTitleVersion());

                            

                        

                    }

                    // Write tik, tmd, and cert to file
                    Console.WriteLine("Saving Ticket - title.tik");

                    try
                    {
                        File.WriteAllBytes(Path.Combine(saveDir, "title.tik"), ticket.ExportTicketData());
                    }
                    catch (IOException ioe)
                    {
                        string message = "ERROR! Could not save title.tik!";
                        throw new IOException(message, ioe);
                    }

                    Console.WriteLine("Saving title.cert...");
                   

                    try
                    {
                        //File.Copy(Environment.CurrentDirectory + "\\magic.cert", saveDir + "\\title.cert", true);
                        File.WriteAllBytes(Path.Combine(saveDir, "title.cert"), Utils.GetByteArrayFromHexString("00010003704138EFBBBDA16A987DD901326D1C9459484C88A2861B91A312587AE70EF6237EC50E1032DC39DDE89A96A8E859D76A98A6E7E36A0CFE352CA893058234FF833FCB3B03811E9F0DC0D9A52F8045B4B2F9411B67A51C44B5EF8CE77BD6D56BA75734A1856DE6D4BED6D3A242C7C8791B3422375E5C779ABF072F7695EFA0F75BCB83789FC30E3FE4CC8392207840638949C7F688565F649B74D63D8D58FFADDA571E9554426B1318FC468983D4C8A5628B06B6FC5D507C13E7A18AC1511EB6D62EA5448F83501447A9AFB3ECC2903C9DD52F922AC9ACDBEF58C6021848D96E208732D3D1D9D9EA440D91621C7A99DB8843C59C1F2E2C7D9B577D512C166D6F7E1AAD4A774A37447E78FE2021E14A95D112A068ADA019F463C7A55685AABB6888B9246483D18B9C806F474918331782344A4B8531334B26303263D9D2EB4F4BB99602B352F6AE4046C69A5E7E8E4A18EF9BC0A2DED61310417012FD824CC116CFB7C4C1F7EC7177A17446CBDE96F3EDD88FCD052F0B888A45FDAF2B631354F40D16E5FA9C2C4EDA98E798D15E6046DC5363F3096B2C607A9D8DD55B1502A6AC7D3CC8D8C575998E7D796910C804C495235057E91ECD2637C9C1845151AC6B9A0490AE3EC6F47740A0DB0BA36D075956CEE7354EA3E9A4F2720B26550C7D394324BC0CB7E9317D8A8661F42191FF10B08256CE3FD25B745E5194906B4D61CB4C2E000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000526F6F7400000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000001434130303030303030330000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000007BE8EF6CB279C9E2EEE121C6EAF44FF639F88F078B4B77ED9F9560B0358281B50E55AB721115A177703C7A30FE3AE9EF1C60BC1D974676B23A68CC04B198525BC968F11DE2DB50E4D9E7F071E562DAE2092233E9D363F61DD7C19FF3A4A91E8F6553D471DD7B84B9F1B8CE7335F0F5540563A1EAB83963E09BE901011F99546361287020E9CC0DAB487F140D6626A1836D27111F2068DE4772149151CF69C61BA60EF9D949A0F71F5499F2D39AD28C7005348293C431FFBD33F6BCA60DC7195EA2BCC56D200BAF6D06D09C41DB8DE9C720154CA4832B69C08C69CD3B073A0063602F462D338061A5EA6C915CD5623579C3EB64CE44EF586D14BAAA8834019B3EEBEED3790001000100000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000100042EA66C66CFF335797D0497B77A197F9FE51AB5A41375DC73FD9E0B10669B1B9A5B7E8AB28F01B67B6254C14AA1331418F25BA549004C378DD72F0CE63B1F7091AAFE3809B7AC6C2876A61D60516C43A63729162D280BE21BE8E2FE057D8EB6E204242245731AB6FEE30E5335373EEBA970D531BBA2CB222D9684387D5F2A1BF75200CE0656E390CE19135B59E14F0FA5C1281A7386CCD1C8EC3FAD70FBCE74DEEE1FD05F46330B51F9B79E1DDBF4E33F14889D05282924C5F5DC2766EF0627D7EEDC736E67C2E5B93834668072216D1C78B823A072D34FF3ECF9BD11A29AF16C33BD09AFB2D74D534E027C19240D595A68EBB305ACC44AB38AB820C6D426560C000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000526F6F742D43413030303030303033000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000143503030303030303062000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000137A080BA689C590FD0B2F0D4F56B632FB934ED0739517B33A79DE040EE92DC31D37C7F73BF04BD3E44E20AB5A6FEAF5984CC1F6062E9A9FE56C3285DC6F25DDD5D0BF9FE2EFE835DF2634ED937FAB0214D104809CF74B860E6B0483F4CD2DAB2A9602BC56F0D6BD946AED6E0BE4F08F26686BD09EF7DB325F82B18F6AF2ED525BFD828B653FEE6ECE400D5A48FFE22D538BB5335B4153342D4335ACF590D0D30AE2043C7F5AD214FC9C0FE6FA40A5C86506CA6369BCEE44A32D9E695CF00B4FD79ADB568D149C2028A14C9D71B850CA365B37F70B657791FC5D728C4E18FD22557C4062D74771533C70179D3DAE8F92B117E45CB332F3B3C2A22E705CFEC66F6DA3772B000100010000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000010004919EBE464AD0F552CD1B72E7884910CF55A9F02E50789641D896683DC005BD0AEA87079D8AC284C675065F74C8BF37C88044409502A022980BB8AD48383F6D28A79DE39626CCB2B22A0F19E41032F094B39FF0133146DEC8F6C1A9D55CD28D9E1C47B3D11F4F5426C2C780135A2775D3CA679BC7E834F0E0FB58E68860A71330FC95791793C8FBA935A7A6908F229DEE2A0CA6B9B23B12D495A6FE19D0D72648216878605A66538DBF376899905D3445FC5C727A0E13E0E2C8971C9CFA6C60678875732A4E75523D2F562F12AABD1573BF06C94054AEFA81A71417AF9A4A066D0FFC5AD64BAB28B1FF60661F4437D49E1E0D9412EB4BCACF4CFD6A3408847982000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000526F6F742D43413030303030303033000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000158533030303030303063000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000137A0894AD505BB6C67E2E5BDD6A3BEC43D910C772E9CC290DA58588B77DCC11680BB3E29F4EABBB26E98C2601985C041BB14378E689181AAD770568E928A2B98167EE3E10D072BEEF1FA22FA2AA3E13F11E1836A92A4281EF70AAF4E462998221C6FBB9BDD017E6AC590494E9CEA9859CEB2D2A4C1766F2C33912C58F14A803E36FCCDCCCDC13FD7AE77C7A78D997E6ACC35557E0D3E9EB64B43C92F4C50D67A602DEB391B06661CD32880BD64912AF1CBCB7162A06F02565D3B0ECE4FCECDDAE8A4934DB8EE67F3017986221155D131C6C3F09AB1945C206AC70C942B36F49A1183BCD78B6E4B47C6C5CAC0F8D62F897C6953DD12F28B70C5B7DF751819A98346526250001000100000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000"));
                    }
                    catch (IOException ioe)
                    {
                        string message = "ERROR! Could not save title.cert!";
                        throw new IOException(message, ioe);
                    }

                    // Download Content .app and .h3 files
                    uint contentCount = tmd.GetContentCount();
                    for (uint i = 0; i < contentCount; i++)
                    {
                        //
                        string cidStr = tmd.GetContentIDString(i);

                        // .app files
                        try
                        {
                            while (wc.IsBusy)
                                System.Threading.Thread.Sleep(5);

                            string currentContentLogStr = String.Format("Downloading Content No. {0} of {1} from Nintendo CDN - {2}.app", i + 1, contentCount, cidStr);


                            string v = Path.Combine(saveDir, cidStr.ToUpper() + ".app");
                            string contentFilePath = v;
                           
                            
                                Console.WriteLine(currentContentLogStr);

                                wc.DownloadFileAsync(new Uri(baseUrl + cidStr), Path.Combine(saveDir, cidStr.ToUpper() + ".app"));
                            

                        }
                        catch (WebException we)
                        {
                            string message = "ERROR! Could not download " + cidStr.ToUpper() + ".app";
                            throw new WebException(message, we);
                        }
                        catch (IOException ioe)
                        {
                            string message = "ERROR! Could not save " + cidStr.ToUpper() + ".app";
                            throw new IOException(message, ioe);
                        }

                        // .h3 files
                        try
                        {
                            while (wc.IsBusy)
                                System.Threading.Thread.Sleep(5);

                            Console.WriteLine(String.Format("Downloading H3 for Content No.{0} from Nintendo CDN - {1}.h3", i + 1, cidStr));
                            wc.DownloadFile(baseUrl + cidStr + ".h3", Path.Combine(saveDir, cidStr.ToUpper() + ".h3"));

                        }
                        catch (WebException we)
                        {
                            if (((HttpWebResponse)we.Response).StatusCode == HttpStatusCode.NotFound)
                                Console.WriteLine(String.Format("WARNING: {0}.h3 not found, ignoring...", cidStr));
                            else
                            {
                                string message = "ERROR! Could not download " + cidStr + ".h3";
                                throw new WebException(message, we);
                            }
                        }
                        catch (IOException ioe)
                        {
                            string message = "ERROR! Could not save " + cidStr + ".h3";
                            throw new IOException(message, ioe);
                        }
                    }
                    string titleCompleteStr = String.Format("Title: {0} completed!", Path.GetFileName(saveDir));
                    Console.WriteLine("Download Complete!");

                }
                catch (Exception ex)
                {
                    string errorMessage = ex.Message + Environment.NewLine + ex.InnerException.Message + Environment.NewLine + "ABORTING DOWNLOAD OF THIS TITLE";
                    Console.WriteLine(errorMessage);
                }

            }
        }

    }
}
