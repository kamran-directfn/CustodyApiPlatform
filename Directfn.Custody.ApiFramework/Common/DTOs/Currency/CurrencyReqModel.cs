using System;
using System.Collections.Generic;
using System.Text;

namespace Directfn.Custody.ApiFramework.Common.DTOs.Currency
{
    public class CurrencyReqModel
    {
        public int RF08_ID { get; set; }
        public string RF08_CODE { get; set; }
        public string RF08_DESCRIPTION { get; set; }
        public string RF08_DESCRIPTION_SEC { get; set; }
        public decimal RF08_SELL_RATE { get; set; }
        public decimal RF08_BUY_RATE { get; set; }
        public decimal RF08_AVG_RATE { get; set; }
        public decimal RF08_FIX_RATE { get; set; }
        public int RF08_CREATED_BY { get; set; }
        public int RF08_MODIFIED_BY { get; set; }
    }
}
