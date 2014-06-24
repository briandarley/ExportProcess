using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
namespace ExportProcess.ESpeed
{
    public class Types
    {
        public enum DocStatusTypes
        {
            [Description("Approved-RVWD")] ApprovedRVWD,
            [Description("HealthE_Waiting")] HealthE_Waiting,
            [Description("HealthE_Sent")] HealthE_Sent,
            [Description("Process Started")] ProcessStarted,
            [Description("Rx Review")] RxReview 
            
        }
    }
}
