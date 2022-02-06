using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AuthorizeNetPgw.Models
{
    public class PaymentModel
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string Postcode { get; set; }
        public string Phone { get; set; }
        public decimal Amount { get; set; }
        public string CardNumber { get; set; }
        public string CardCode { get; set; }
        public string Month { get; set; }
        public string Year { get; set; }
    }
}