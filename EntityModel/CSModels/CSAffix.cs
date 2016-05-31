using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GJFZWebService.Models.CSModels
{
    public class CSAffix
    {
        public int TBNO
        {
            get;
            set;
        }
        public byte[] FileData
        {
            get;
            set;
        }
        public string FileName
        {
            get;
            set;
        }
        public decimal FileSize
        {
            get;
            set;
        }
        public int PageNO
        {
            get;
            set;
        }
        public string DownloadURL
        {
            get;
            set;
        }
    }
}