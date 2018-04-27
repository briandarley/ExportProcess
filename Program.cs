using System;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using Aspose.Pdf;
using Aspose.Pdf.Devices;
using ExportProcess.ESpeed;
using ExportProcess.Utilities;
using Jscape.Ftp;
using log4net;


namespace ExportProcess
{
    class Program
    {

        public static readonly ILog log = LogManager.GetLogger(typeof(Program));
        static void Main(string[] args)
        {
            log4net.Config.XmlConfigurator.Configure();
            
            
            try
            {
                var records = Records.GetRecords(new Records.Criteria(Types.DocStatusTypes.HealthE_Waiting));
                //var records = Records.GetRecords(new Records.Criteria(Types.DocStatusTypes.Healthe_Hold));
                
                if (records != null && records.ESpeedRecords != null)
                {
                    if (records.ESpeedRecords.Any(c => c.IsValid()))
                    {
                        records.FileCopiedEventHandler += OnFileCopied;

                        var recordCount = records.ESpeedRecords.Count(c => c.IsValid()).ToString().PadLeft(5, '0');
                        var zipFile = GetZipFileName(recordCount);

                        log.Info($"Adding records to zipped file, {records.ESpeedRecords.Count}");
                        //Console.WriteLine("Adding records to zipped file");
                        AddRecordsToZipFile(records, zipFile);

                        log.Info("Adding control file to zip file");
                        //Console.WriteLine("Adding control file to zip file");
                        AddSourceControlFileToZipFile(records, zipFile);

                        log.Info("Submitting zipped file to FTP");
                        //Console.WriteLine("Submitting zipped file to FTP");
                        SubmitZipFileToVendorViaFtp(zipFile);


                        log.Info("Committing changes to database");
                        //Console.WriteLine("Committing changes to database");

                        records.UpdateStatus();


                        SendCompleteNotification(zipFile);
                        
                        SubmitInvalidFileLog(records);
                        
                    }
                    else
                    {
                        
                        log.Warn("No valid records to submit");
                        //Console.WriteLine("No valid records to submit");

                    }
                    log.Info("\nDone copying files");
                    //Console.WriteLine("\nDone copying files");
                }
                else
                {
                    log.Info("Process Ran Successfully, there were no files to process");
                    //Console.WriteLine("Process Ran Successfully, there were no files to process");
                }

            }
            catch (Exception ex)
            {
                SendErrorNotification(ex);
            }


        }

        private static void TestConvertingPdfToTiff()
        {
            var file = @"C:\Users\bdarley\Google Drive\Claim_Files_Details.pdf";
            //var file = @"C:\Users\bdarley\Google Drive\_obj_2_D1700_478_PROB.pdf";
            var arrBytes = TiffConversion.ExtractMultiTiffDocumentsToByteArray(file);
            File.WriteAllBytes(@"C:\Users\bdarley\Google Drive\Foo.pdf", arrBytes);
            


        }

        private static void TestReadingPdfToFile()
        {
            var obj = new Records();
            var arrBytes = obj.LoadFileToByteArray(@"C:\Users\bdarley\Google Drive\_obj_2_D1700_478_PROB.pdf");
            File.WriteAllBytes(@"C:\Users\bdarley\Google Drive\Foo.pdf", arrBytes);
            
        }

        private static void TestConvertingPdfToTiffWithAspose()
        {
            var lic = new License();
            lic.SetLicense("Aspose.Total.lic");
            
            //var ass = 
            var file = @"C:\Users\bdarley\Google Drive\_obj_2_D1700_478_PROB.pdf";
            var output = @"C:\Users\bdarley\Google Drive\output.tff";
            Document pdfDocument = new Document(file);

            // Create Resolution object
            Resolution resolution = new Resolution(300);

            // Create TiffSettings object
            TiffSettings tiffSettings = new TiffSettings();
            tiffSettings.Compression = CompressionType.None;
            tiffSettings.Depth = ColorDepth.Default;
            tiffSettings.Shape = ShapeType.Landscape;
            tiffSettings.SkipBlankPages = false;

            // Create TIFF device
            TiffDevice tiffDevice = new TiffDevice(resolution, tiffSettings);

            // Convert a particular page and save the image to stream
            tiffDevice.Process(pdfDocument, output);

        }
        private static void SubmitZipFileToVendorViaFtp(string zipFile)
        {
            var ftp = RetrieveInitializedFtpObject();
            if (ftp == null) return;
            ftp.Connect();

            ftp.RemoteDir = ConfigurationManager.AppSettings["FTPDestinationFolder"];
            ftp.LocalDir = Path.GetDirectoryName(zipFile);

            ftp.Upload(Path.GetFileName(zipFile));

            ftp.Disconnect();
            if (DebugFtp())
                ftp.DebugStream.Close();



        }



        private static void AddSourceControlFileToZipFile(Records records, string zipFileName)
        {
            var zipFile = new Ionic.Zip.ZipFile(zipFileName);

            var controlFileName = string.Format("imageControl_Out_{0}.csv", DateTime.Now.ToString("yyyyMMddhhmmss"));
            zipFile.AddEntry(controlFileName, records.GenerateOutput(true));
            zipFile.Save();
        }

        private static void AddRecordsToZipFile(Records records, string zipFileName)
        {
            records.AddSourceFilesToZipArchive(zipFileName);
        }

        private static string GetZipFileName(string recordCount)
        {
            var path = ConfigurationManager.AppSettings["Outputfolder"];
            return Path.Combine(path, string.Format("PBM-JECO-{0}{1}.zip", DateTime.Now.ToString("yyyyMMdd"), recordCount));
        }

        static void OnFileCopied(object sender, EventArgs e)
        {
            Console.Write(".");

        }

        private static Ftp RetrieveInitializedFtpObject()
        {
            //Secure FTP Factory for .NET:Single Developer:Registered User:01-01-3999:I5eiUi3WpB4FspHQ5uOlKRh2eabOrFtcMbh74FmX/UzFUolDU9f5hbostB6xBnB9Xq8sNOFDx7jmiPkXbKhpZw4k0sWKlSywRceV4ir8csHWdBYdmFkrOSY+eYgjp+ud9GsP+m8sO1dun1qlzBMW87Fkpsmgbs88W7USXkMBl/U=

            var host = ConfigurationManager.AppSettings["FTP:Address"];
            var user = ConfigurationManager.AppSettings["FTP:UserId"];
            var pwd = ConfigurationManager.AppSettings["FTP:Password"];
            if (string.IsNullOrEmpty(host))
            {
                return null;
            }
            var ftp = new Ftp(host, user, pwd)
                      {
                          LicenseKey = ConfigurationManager.AppSettings["FTP:SFTP_License"],
                          ConnectionType = Ftp.DEFAULT,
                          Debug = DebugFtp()

                      };
            if (DebugFtp())
            {
                var path = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
                ftp.DebugStream = File.CreateText(Path.Combine(path, "FTP_DEBUG.LOG"));
            }

            return ftp;
        }

        private static bool DebugFtp()
        {
            return (ConfigurationManager.AppSettings["FTP:Debug"] == "true" ? true : false);
        }

        private static void SendErrorNotification(Exception exception)
        {

            var sb = new StringBuilder();
            sb.Append("Description of error\n\n");
            sb.Append(exception.Message);
            sb.Append("\n\n");
            if (exception.InnerException != null)
            {
                sb.Append("Inner exception\n\n");
                sb.Append(exception.InnerException.Message);
                sb.Append("\n\n");
            }
            sb.Append("Stack tract\n\n");
            sb.Append(exception.StackTrace);


            SendEmailNotification("Failure - FTP process to HealthESystems ",
                  sb.ToString());


        }

        private static void SendCompleteNotification(string zipFile)
        {
            SendEmailNotification("HealthESystems Success",
                string.Format("File successfully submitted to HealthESystems - File {0}", zipFile));

            
        }

        private static void SubmitInvalidFileLog(ESpeed.Records records)
        {
            if (records.ESpeedRecords.All(c => c.IsValid())) return;
            var sb = new StringBuilder();
            sb.Append("The following records are invalid and could not be sent");
            sb.Append("\n");
            sb.AppendFormat("{0}\t\t\t\t{1}", "Claim Number", "DCN");
            sb.Append("\n");
            foreach (var eSpeedRecord in records.ESpeedRecords)
            {
                sb.AppendFormat("{0}\t\t\t{1}", eSpeedRecord.IdxClaimNumber, 
                    (string.IsNullOrEmpty(eSpeedRecord.IdxRxBillId) ? "Missing DCN (IdxRxBillId)": eSpeedRecord.IdxRxBillId));
                sb.Append("\n");
            }

            SendEmailNotification("HealthESystems Invalid files", sb.ToString());
        }

        private static void SendEmailNotification(string subject, string messageBody)
        {
            if (string.IsNullOrEmpty(ConfigurationManager.AppSettings["emailAuthenPWD"])) return;
            var smtp = new System.Net.Mail.SmtpClient("shire")
                         {
                             Credentials = new NetworkCredential(ConfigurationManager.AppSettings["emailAuthenPWD"], ConfigurationManager.AppSettings["emailAuthenUID"])
                         };
            var msg = new System.Net.Mail.MailMessage
                      {
                          From =
                              new System.Net.Mail.MailAddress(ConfigurationManager.AppSettings["SourceEmail"])

                      };


            foreach (var recipient in ConfigurationManager.AppSettings["emailNotification"].Split(','))
            {
                msg.To.Add(recipient);
            }

            msg.Subject = subject;
            msg.Body = messageBody;
            smtp.Send(msg);

        }


        private static void TestFtpConnection()
        {
            var ftp = RetrieveInitializedFtpObject();
            ftp.Connect();
            //ftp.DownloadDir(ConfigurationManager.AppSettings["FTPDestinationFolder"]);

            ftp.RemoteDir = ConfigurationManager.AppSettings["FTPDestinationFolder"];
            ftp.LocalDir = @"c:\temp\";

            ftp.Upload(@"test.txt"); 


            ftp.Disconnect();
            ftp.DebugStream.Close();

        }

      
    }


}

