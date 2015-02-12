using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using ExportProcess.Utilities;

namespace ExportProcess.ESpeed
{
    public class Records
    {
        public EventHandler FileCopiedEventHandler;
        public List<Record> ESpeedRecords { get; set; }
        #region Criteria
        public class Criteria
        {
            public Types.DocStatusTypes DocStatus { get; set; }


            public Criteria(Types.DocStatusTypes docStatus)
            {
                DocStatus = docStatus;
            }
        }

        #endregion //Criteria

        public class MockCriteria : Criteria
        {
            public MockCriteria(Types.DocStatusTypes docStatus) : base(docStatus)
            {
            }


        }
        #region Methods 
        public static Records GetRecords(Criteria criteria)
        {
            if (criteria.GetType() == typeof (MockCriteria))
            {
                return Data.GetMockRecords(criteria);
            }
          return Data.GetRecords(criteria);
        }


        public string GenerateOutput(bool appendHeader)
        {
            if (ESpeedRecords == null || ESpeedRecords.Count == 0 || !ESpeedRecords.Any(c => c.IsValid())) return string.Empty;
            var sb = new System.Text.StringBuilder();
            if (appendHeader)
                sb.AppendFormat("\"{0}\",\"{1}\",\"{2}\",\"{3}\",\"{4}\",\"{5}\"{6}", "JECO Claim Number", "JECO DCN", "Claimant Last", "Date of Loss", "Date Submitted", "Source Name", "\r\n");
            foreach (var record in ESpeedRecords.Where(c => c.IsValid()))
            {
                sb.Append(record);
            }

            return sb.ToString();
        }

        public void SaveToFile(string path, string fileName, bool appendToFile)
        {
            if (ESpeedRecords == null || ESpeedRecords.Count == 0)
            {
                return;
            }
            GenerateOutput(!appendToFile).SaveToFile(path, fileName, appendToFile);
        }

        public void AddSourceFilesToZipArchive(string fileName)
        {

            List<Record> badFiles = null;
            var zipFile = new Ionic.Zip.ZipFile(fileName);
            try
            {
                foreach (var record in ESpeedRecords.Where(c => c.IsValid()))
                {

                    if (!File.Exists(record.Pointertosource))
                    {
                        if (badFiles == null) badFiles = new List<Record>();
                        badFiles.Add(record);
                    }
                    else
                    {
                        //var fileContents = record.Pointertosource.IsTiff() 
                        //    ? TiffConversion.ExtractMultiTiffDocumentsToByteArray(record.Pointertosource) 
                        //    : File.ReadAllBytes(record.Pointertosource);
                        var fileContents = record.Pointertosource.IsTiff() 
                            ? TiffConversion.ExtractMultiTiffDocumentsToByteArray(record.Pointertosource)
                            : TiffConversion.ConvertPdfToByteArray(record.Pointertosource);
                        
                        zipFile.AddEntry(record.FileName, fileContents);
                    }
                    FileCopiedEventHandler(this, EventArgs.Empty);

                }
            }
            finally
            {
                zipFile.Save();
                zipFile.Dispose();
            }
            if (badFiles == null) return;
            foreach (var record in badFiles)
                ESpeedRecords.Remove(record);
        }

        void LoadStreamToStream(Stream inputStream, Stream outputStream)
        {
            const int bufferSize = 64 * 1024;
            var buffer = new byte[bufferSize];

            while (true)
            {
                var bytesRead = inputStream.Read(buffer, 0, bufferSize);
                if (bytesRead > 0)
                {
                    outputStream.Write(buffer, 0, bytesRead);
                }
                if ((bytesRead == 0) || (bytesRead < bufferSize))
                    break;
            }
        }

        public byte[] LoadFileToByteArray(string inputFile)
        {
            byte[] result;
            using (var streamInput = new FileStream(inputFile, FileMode.Open, FileAccess.Read))
            {
                var mem = new MemoryStream();
                LoadStreamToStream(streamInput, mem);
                mem.Position = 0;
                result = mem.ToArray();
                streamInput.Close();
            }
            return result;
        }


        public void CopyImages(string path)
        {
            List<Record> badFiles = null;
            foreach (var record in ESpeedRecords)
            {
                if (!File.Exists(record.Pointertosource))
                {
                    if (badFiles == null) badFiles = new List<Record>();
                    badFiles.Add(record);
                }
                else
                {
                    File.Copy(record.Pointertosource, Path.Combine(path, record.FileName), true);
                }
                FileCopiedEventHandler(this, EventArgs.Empty);
            }

            if (badFiles == null) return;

            foreach (var record in badFiles)
                ESpeedRecords.Remove(record);
        }

        public void UpdateStatus()
        {
            if (ESpeedRecords == null || ESpeedRecords.Count <= 0) return;


            foreach (var record in ESpeedRecords.Where(c=>c.IsValid()))
            {
                record.SetStatus(Types.DocStatusTypes.HealthE_Sent);

            }
        }

        #endregion //Methods

        #region DAL

        internal class Data
        {
            public static Records GetRecords(Criteria criteria)
            {
                var result = new Records();
                using (var cn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["espeed"].ConnectionString))
                {
                    cn.Open();
                    var cmd = cn.CreateCommand();

                    const string sql = "select * from _obj_2 "
                                       + " where IDX_DOC_STATUS = @IDX_DOC_STATUS and status != 'X' ";
                    cmd.CommandText = sql;
                    cmd.Parameters.AddWithValue("@IDX_DOC_STATUS", criteria.DocStatus.GetDescription());



                    var dr = cmd.ExecuteReader();

                    while (dr.Read())
                    {
                        if (result.ESpeedRecords == null) result.ESpeedRecords = new List<Record>();
                        result.ESpeedRecords.Add(new Record(dr));
                    }
                    dr.Close();
                }

                return result;
            }

            public static Records GetMockRecords(Criteria criteria)
            {
                var result = new Records();
                result.ESpeedRecords =new List<Record>();
                result.ESpeedRecords.Add(new Record
                {
                    IdxRxBillId = "FakeRxBillId",
                    Pointertosource = @"D:\Google Drive\_obj_2_D1754_971.pdf",
                    IsMock = true
                });
                return result;
            }
        }

        #endregion //DAL

    }
}
