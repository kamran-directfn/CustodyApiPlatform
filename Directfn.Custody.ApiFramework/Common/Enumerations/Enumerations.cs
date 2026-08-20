using System;
using System.Collections.Generic;
using System.Text;

namespace Directfn.Custody.ApiFramework.Common.Enumerations
{
    public static class Enumerations
    {
        private static readonly Dictionary<string, string> _fopTransferTypes = new()
    {
        { "INTERNAL", "OWNI" },
        { "EXTERNAL", "OWNE" },
        { "DONATION", "DONA" },
        { "GIFT", "GIFT" },
        { "MURABAHA", "MURA" },
        { "WILL", "WILL" },
        { "VERDICT", "VERD" },
        { "PLEDGE_CALL", "PLCA" },
        { "INHERITANCE", "INHE" },
        { "ENDOWMENT", "EDOW" },
        { "APPROVED_BY_AUTHORITY", "AUTA" },
        { "EMPLOYEE_INCENTIVE_PLAN", "EEIP" },
        { "CONVERSION_TRANSFER_SWAP_TO_QFI", "CTSW" },
        { "CORRECTION_TRANSFER_SWAP_TO_QFI", "CRSW" },
        { "MOVE_OF_STOCK", "INSP" }
    };

        public static Dictionary<string, string> GetFopTransferTypes()
        {
            return _fopTransferTypes;
        }
    }
}
