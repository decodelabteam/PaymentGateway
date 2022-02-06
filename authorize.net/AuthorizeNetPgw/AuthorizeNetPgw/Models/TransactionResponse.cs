using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AuthorizeNetPgw.Models
{
    public class TransactionResponse
    {
        public AuthorizeNet.Api.Contracts.V1.messageTypeEnum resultCode { get; set; }
        public string transId { get; set; }
        public string responseCode { get; set; }
        public string messageCode { get; set; }
        public string authCode { get; set; }
        public string description { get; set; }
        public string errorCode { get; set; }
        public string errorText { get; set; }
    }
}