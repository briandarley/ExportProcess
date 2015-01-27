using System;
using System.Data.SqlClient;
using ExportProcess.Utilities;

namespace ExportProcess.ESpeed
{



    public class Record
    {
        #region properties
        /// <summary>
        /// ArchiveStatus - 
        /// </summary>
        public string Archivestatus { get; set; }

        /// <summary>
        /// BatchID - 
        /// </summary>
        public int Batchid { get; set; }

        /// <summary>
        /// CreateDate - 
        /// </summary>
        public DateTime Createdate { get; set; }

        /// <summary>
        /// CreatedBy - 
        /// </summary>
        public string Createdby { get; set; }

        /// <summary>
        /// IDX_AdjInit - 
        /// </summary>
        public string IdxAdjinit { get; set; }

        /// <summary>
        /// IDX_AdjSupInit - 
        /// </summary>
        public string IdxAdjsupinit { get; set; }

        /// <summary>
        /// IDX_AdjTAInit - 
        /// </summary>
        public string IdxAdjtainit { get; set; }

        /// <summary>
        /// IDX_Approved_By - 
        /// </summary>
        public string IdxApprovedBy { get; set; }

        /// <summary>
        /// IDX_Archive - 
        /// </summary>
        public string IdxArchive { get; set; }

        /// <summary>
        /// IDX_Assign_To - 
        /// </summary>
        public string IdxAssignTo { get; set; }

        /// <summary>
        /// IDX_Batch - 
        /// </summary>
        public int IdxBatch { get; set; }

        /// <summary>
        /// IDX_Bill_Id - 
        /// </summary>
        public string IdxBillId { get; set; }

        /// <summary>
        /// IDX_Bypass_Workflow - 
        /// </summary>
        public string IdxBypassWorkflow { get; set; }

        /// <summary>
        /// IDX_CC_Checked - 
        /// </summary>
        public string IdxCcChecked { get; set; }

        /// <summary>
        /// IDX_CC_ReviewDt - 
        /// </summary>
        public DateTime IdxCcReviewdt { get; set; }

        /// <summary>
        /// IDX_CC_Reviewer - 
        /// </summary>
        public string IdxCcReviewer { get; set; }

        /// <summary>
        /// IDX_Claim_Number - 
        /// </summary>
        public int IdxClaimNumber { get; set; }

        /// <summary>
        /// IDX_Claim_Type - 
        /// </summary>
        public string IdxClaimType { get; set; }

        /// <summary>
        /// IDX_Clmnt_First_Name - 
        /// </summary>
        public string IdxClmntFirstName { get; set; }

        /// <summary>
        /// IDX_Clmnt_Last_Name - 
        /// </summary>
        public string IdxClmntLastName { get; set; }

        /// <summary>
        /// IDX_Company_Name - 
        /// </summary>
        public string IdxCompanyName { get; set; }

        /// <summary>
        /// IDX_Company_Num - 
        /// </summary>
        public int IdxCompanyNum { get; set; }

        /// <summary>
        /// IDX_DOC_CAT - 
        /// </summary>
        public string IdxDocCat { get; set; }

        /// <summary>
        /// IDX_Doc_Date_1 - 
        /// </summary>
        public DateTime IdxDocDate1 { get; set; }

        /// <summary>
        /// IDX_Doc_Date_2 - 
        /// </summary>
        public DateTime IdxDocDate2 { get; set; }

        /// <summary>
        /// IDX_Doc_Int_Flow - 
        /// </summary>
        public int IdxDocIntFlow { get; set; }

        /// <summary>
        /// IDX_DOC_ORIGIN - 
        /// </summary>
        public string IdxDocOrigin { get; set; }

        /// <summary>
        /// IDX_DOC_ORIGIN_TYPE - 
        /// </summary>
        public string IdxDocOriginType { get; set; }

        /// <summary>
        /// IDX_DOC_STATUS - 
        /// </summary>
        public string IdxDocStatus { get; set; }

        /// <summary>
        /// IDX_DOC_TYPE - 
        /// </summary>
        public string IdxDocType { get; set; }

        /// <summary>
        /// IDX_Event_Key - 
        /// </summary>
        public int IdxEventKey { get; set; }

        /// <summary>
        /// IDX_Loss_Date - 
        /// </summary>
        public DateTime IdxLossDate { get; set; }

        /// <summary>
        /// IDX_NCCI_Cd - 
        /// </summary>
        public int IdxNcciCd { get; set; }

        /// <summary>
        /// IDX_NCMInit - 
        /// </summary>
        public string IdxNcminit { get; set; }

        /// <summary>
        /// IDX_NCMSupInit - 
        /// </summary>
        public string IdxNcmsupinit { get; set; }

        /// <summary>
        /// IDX_NCMTAInit - 
        /// </summary>
        public string IdxNcmtainit { get; set; }

        /// <summary>
        /// IDX_Received_Dt - 
        /// </summary>
        public DateTime IdxReceivedDt { get; set; }

        /// <summary>
        /// IDX_Rx_Bill_ID - 
        /// </summary>
        public string IdxRxBillId { get; set; }

        /// <summary>
        /// IDX_Secure - 
        /// </summary>
        public string IdxSecure { get; set; }

        /// <summary>
        /// IDX_Soc_Sec_Num - 
        /// </summary>
        public string IdxSocSecNum { get; set; }

        /// <summary>
        /// IDX_State - 
        /// </summary>
        public string IdxState { get; set; }

        /// <summary>
        /// IDX_Tax_ID_Number - 
        /// </summary>
        public int IdxTaxIdNumber { get; set; }

        /// <summary>
        /// IDX_Vendor_Name - 
        /// </summary>
        public string IdxVendorName { get; set; }

        /// <summary>
        /// IDX_WORKFLOW_STATUS - 
        /// </summary>
        public string IdxWorkflowStatus { get; set; }

        /// <summary>
        /// Locked - 
        /// </summary>
        public string Locked { get; set; }

        /// <summary>
        /// ModifiedBy - 
        /// </summary>
        public string Modifiedby { get; set; }

        /// <summary>
        /// ModifiedDate - 
        /// </summary>
        public DateTime Modifieddate { get; set; }

        /// <summary>
        /// ObjectID - 
        /// </summary>
        public int Objectid { get; set; }

        /// <summary>
        /// Pages - 
        /// </summary>
        public int Pages { get; set; }

        /// <summary>
        /// PointerToSource - 
        /// </summary>
        public string Pointertosource { get; set; }

        /// <summary>
        /// Status - 
        /// </summary>
        public string Status { get; set; }


        public string FileName
        {
            get
            {
                return string.Format("{0}_{1}.tif", IdxClaimNumber, IdxRxBillId);
            }
        }

        #endregion //properties

        #region constructors
        public Record() { }

        public Record(SqlDataReader dr)
        {
            Archivestatus = dr.GetString("ArchiveStatus");
            Batchid = dr.GetInt32("BatchID");
            Createdate = dr.GetDateTime("CreateDate");
            Createdby = dr.GetString("CreatedBy");
            IdxAdjinit = dr.GetString("IDX_AdjInit");
            IdxAdjsupinit = dr.GetString("IDX_AdjSupInit");
            IdxAdjtainit = dr.GetString("IDX_AdjTAInit");
            IdxApprovedBy = dr.GetString("IDX_Approved_By");
            IdxArchive = dr.GetString("IDX_Archive");
            IdxAssignTo = dr.GetString("IDX_Assign_To");
            IdxBatch = dr.GetInt32("IDX_Batch");
            IdxBillId = dr.GetString("IDX_Bill_Id");
            IdxBypassWorkflow = dr.GetString("IDX_Bypass_Workflow");
            IdxCcChecked = dr.GetString("IDX_CC_Checked");
            IdxCcReviewdt = dr.GetDateTime("IDX_CC_ReviewDt");
            IdxCcReviewer = dr.GetString("IDX_CC_Reviewer");
            IdxClaimNumber = dr.GetInt32("IDX_Claim_Number");
            IdxClaimType = dr.GetString("IDX_Claim_Type");
            IdxClmntFirstName = dr.GetString("IDX_Clmnt_First_Name");
            IdxClmntLastName = dr.GetString("IDX_Clmnt_Last_Name");
            IdxCompanyName = dr.GetString("IDX_Company_Name");
            IdxCompanyNum = dr.GetInt32("IDX_Company_Num");
            IdxDocCat = dr.GetString("IDX_DOC_CAT");
            IdxDocDate1 = dr.GetDateTime("IDX_Doc_Date_1");
            IdxDocDate2 = dr.GetDateTime("IDX_Doc_Date_2");
            IdxDocIntFlow = dr.GetInt32("IDX_Doc_Int_Flow");
            IdxDocOrigin = dr.GetString("IDX_DOC_ORIGIN");
            IdxDocOriginType = dr.GetString("IDX_DOC_ORIGIN_TYPE");
            IdxDocStatus = dr.GetString("IDX_DOC_STATUS");
            IdxDocType = dr.GetString("IDX_DOC_TYPE");
            IdxEventKey = dr.GetInt32("IDX_Event_Key");
            IdxLossDate = dr.GetDateTime("IDX_Loss_Date");
            IdxNcciCd = dr.GetInt32("IDX_NCCI_Cd");
            IdxNcminit = dr.GetString("IDX_NCMInit");
            IdxNcmsupinit = dr.GetString("IDX_NCMSupInit");
            IdxNcmtainit = dr.GetString("IDX_NCMTAInit");
            IdxReceivedDt = dr.GetDateTime("IDX_Received_Dt");
            IdxRxBillId = dr.GetString("IDX_Rx_Bill_ID");
            IdxSecure = dr.GetString("IDX_Secure");
            IdxSocSecNum = dr.GetString("IDX_Soc_Sec_Num");
            IdxState = dr.GetString("IDX_State");
            IdxTaxIdNumber = dr.GetInt32("IDX_Tax_ID_Number");
            IdxVendorName = dr.GetString("IDX_Vendor_Name");
            IdxWorkflowStatus = dr.GetString("IDX_WORKFLOW_STATUS");
            Locked = dr.GetString("Locked");
            Modifiedby = dr.GetString("ModifiedBy");
            Modifieddate = dr.GetDateTime("ModifiedDate");
            Objectid = dr.GetInt32("ObjectID");
            Pages = dr.GetInt32("Pages");
            Pointertosource = dr.GetString("PointerToSource");
            Status = dr.GetString("Status");
            //FileName = System.IO.Path.GetFileName(Pointertosource);
        }
        #endregion //constructors


        #region Methods

        public override string ToString()
        {
            //"JECO Claim Number", "JECO DCN", "Claimant Last", "Date of Loss", "Date Submitted", "Source Name"
            //string sourceName = String.Format("{0}_{1}", IdxClaimNumber, IdxRxBillId) + ".tif";
            return string.Format("\"{0}\",\"{1}\",\"{2}\",\"{3}\",\"{4}\",\"{5}\"\r\n",
                IdxClaimNumber, 
                IdxRxBillId,
                IdxClmntLastName,
                IdxLossDate.ToString("MM/dd/yyyy"),
                IdxReceivedDt.ToString("MM/dd/yyyy"),
                FileName);


        }

        internal void SetStatus(Types.DocStatusTypes status)
        {
            IdxDocStatus = status.GetDescription();
            var data = new Data();
            data.UpdateStatus(new Criteria(Objectid, IdxDocStatus.ConvertToEnum<Types.DocStatusTypes>()));
        }


        public bool IsValid()
        {
            //Check DCN 
            return !string.IsNullOrEmpty(IdxRxBillId);
        }

        #endregion //Methods

        private class Criteria
        {
            public Types.DocStatusTypes? IDXDocStatus { get; set; }
            public int? ObjectId { get; set; }

            public Criteria(int objectId, Types.DocStatusTypes docStatus)
            {
                ObjectId = objectId;
                IDXDocStatus = docStatus;
            }
        }

        #region Data
        private class Data
        {
            public void UpdateStatus(Criteria criteria)
            {

                using (var cn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["espeed"].ConnectionString))
                {
                    cn.Open();
                    var cmd = cn.CreateCommand();

                    const string sql = "update _obj_2 set IDX_DOC_STATUS = @IDXDocStatus where objectid = @ObjectId";
                    cmd.CommandText = sql;
                    if (criteria.IDXDocStatus.HasValue)
                        cmd.Parameters.AddWithValue("@IDXDocStatus", criteria.IDXDocStatus.Value.GetDescription());
                    if (criteria.ObjectId.HasValue)
                        cmd.Parameters.AddWithValue("@ObjectId", criteria.ObjectId.Value);

                    cmd.ExecuteNonQuery();
                }


            }
        }


        #endregion //Data



    }

}


