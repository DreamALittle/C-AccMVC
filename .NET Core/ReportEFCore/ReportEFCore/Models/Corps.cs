using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ReportEFCore.Models
{
    public class Corps
    {
        public int RegionId {get; set; }
        public string AreaCode {set;get;}
        public string AreaName{set;get;}
        [Key]
        public virtual int CorpId {get;set;}
        public int CorpKind { get; set; }
        public int CorpType{get;set;}
        public int? CorpRank{get;set;}
        public string CorpCode{get;   set;}
        public string CorpECode { get; set; }
        public string CorpName { get; set; }
        public string CorpAlias { get; set; }
        public string CorpPinyin { get; set; }
        public int CorpStatus { get; set; }
        public string CorpAddress { get; set; }
        public string CorpPostcode { get; set; }
        public string CorpDescription { get; set; }
        public string CorpTelephone { get; set; }
        public string CorpWebsite { get; set; }
        public string CorpEmail { get; set; }
        public string CorpFax { get; set; }
        public string ContactMan { get; set; }
        public string ContactCard { get; set; }
        public string ContactTel { get; set; }
        public DateTime? AuditDate { get; set; }
        public string AuditResult { get; set; }
        public string Auditor { get; set; }
        public DateTime CreatedTime { get; set; }
        public string Creator { get; set; }
        public DateTime ModifiedTime { get; set; }
        public string Modifier { get; set; }
        public string Salesman { get; set; }
    }
}
