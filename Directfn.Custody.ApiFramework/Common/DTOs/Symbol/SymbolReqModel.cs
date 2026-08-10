using System;
using System.Collections.Generic;
using System.Text;

namespace Directfn.Custody.ApiFramework.Common.DTOs.Symbol
{
    public class SymbolReqModel
    {
        public int RF02_SYMBOL_ID { get; set; }
        public string RF02_SYMBOL { get; set; }
        public string RF02_SYMBOL_DESC { get; set; }
        public string RF02_SYMBOL_DESC_SEC { get; set; }
        public decimal RF02_FACE_VALUE { get; set; }
        public string RF02_ROUTER_CODE { get; set; }
        public string RF02_CUSIP_NUMBER { get; set; }
        public decimal RF02_LOT_SIZE { get; set; }
        public int? RF02_RF45_ID { get; set; }
        public int RF02_RF40_ID { get; set; }
        public int RF02_RF01_ID { get; set; }
        public int RF02_RF16_ID { get; set; }
        public decimal RF02_ISSUE_SIZE { get; set; }
        public DateTime RF02_ISSUE_DATE { get; set; }
        public int RF02_BASIS { get; set; }
        public decimal RF02_STOPLOSS_PERCENT { get; set; }
        public decimal RF02_CAPITAL { get; set; }
        public decimal RF02_LISTED_PRICE { get; set; }
        public int RF02_RF43_ID { get; set; }
        public DateTime RF02_START_DATE { get; set; }
        public string RF02_CURRENCY { get; set; }
        public string RF02_ISINCODE { get; set; }
        public DateTime RF02_EXPIRE_DATE { get; set; }
        public decimal RF02_PRICE_RATIO { get; set; }
        public string RF02_BENCHMARK { get; set; }
        public string RF02_BLOOMBERG_CODE { get; set; }
        public int RF02_SHARIA_COMPLIENT { get; set; }
        public string RF02_CEDEL_NO { get; set; }
        public string RF02_SEDOL_NO { get; set; }
        public string RF02_GLOBAL_CODE { get; set; }
        public int RF02_CREATED_BY { get; set; }
        public int RF02_MODIFIED_BY { get; set; }
        public string? ERROR_MESSAGE { get; set; }
    }
}
