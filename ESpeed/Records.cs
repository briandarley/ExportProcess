using System;
using System.Collections.Generic;
using System.Data.SqlClient;
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

        #region Methods
        public static Records GetRecords(Criteria criteria)
        {
            var data = new Data();
            return data.GetRecords(criteria);
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

                    if (!System.IO.File.Exists(record.Pointertosource))
                    {
                        if (badFiles == null) badFiles = new List<Record>();
                        badFiles.Add(record);
                    }
                    else
                    {
                        byte[] fileContents = null;
                        if (record.FileName.IsTiff())
                            fileContents = TiffConversion.ConvertTiff(record.FileName);
                        else
                            fileContents = System.IO.File.ReadAllBytes(record.Pointertosource);

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


        public void CopyImages(string path)
        {
            List<Record> badFiles = null;
            foreach (var record in ESpeedRecords)
            {
                if (!System.IO.File.Exists(record.Pointertosource))
                {
                    if (badFiles == null) badFiles = new List<Record>();
                    badFiles.Add(record);
                }
                else
                {
                    System.IO.File.Copy(record.Pointertosource, System.IO.Path.Combine(path, record.FileName), true);
                }
                FileCopiedEventHandler(this, EventArgs.Empty);
            }

            if (badFiles != null)
            {
                foreach (var record in badFiles)
                    ESpeedRecords.Remove(record);


            }

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
        private class Data
        {

            public Records GetRecords(Criteria criteria)
            {
                var result = new Records();
                using (var cn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["espeed"].ConnectionString))
                {
                    cn.Open();
                    SqlCommand cmd = cn.CreateCommand();

                    string sql = "select * from _obj_2 "
                                + " where IDX_DOC_STATUS = @IDX_DOC_STATUS and status != 'X' ";
                    cmd.CommandText = sql;
                    cmd.Parameters.AddWithValue("@IDX_DOC_STATUS", criteria.DocStatus.GetDescription());



                    SqlDataReader dr = cmd.ExecuteReader();

                    while (dr.Read())
                    {
                        if (result.ESpeedRecords == null) result.ESpeedRecords = new List<Record>();
                        result.ESpeedRecords.Add(new Record(dr));
                    }
                    dr.Close();
                }

                return result;
            }
        }

        #endregion //DAL

    }
}
