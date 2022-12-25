using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Common.Shared.Models
{
    public class VnPayResult
    {
        public string RspCode { get; set; }

        public string Message { get; set; }

        public VnPayResult(string rspCode, string message)
        {
            RspCode = rspCode;
            Message = message;
        }

        public VnPayResult()
        {
        }
    }
}
