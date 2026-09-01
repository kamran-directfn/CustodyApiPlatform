using Directfn.Custody.ApiFramework.Common.DTOs.FOP;
using Directfn.Custody.ApiFramework.Common.DTOs.FOPSettlement;
using Directfn.Custody.ApiFramework.Common.DTOs.Pagination;
using Directfn.Custody.ApiFramework.Database;
using Directfn.Custody.ApiFramework.Repositories.Common;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using Oracle.ManagedDataAccess.Client;
using SQLitePCL;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Text;
using System.Text.Json;
using static System.Net.Mime.MediaTypeNames;
using Text = DocumentFormat.OpenXml.Spreadsheet.Text;

namespace Directfn.Custody.ApiFramework.Repositories.FOP_Settlement
{
    public sealed class FOPSettlementRepository : IFOPSettlementRepository
    {
        private readonly IOracleDbManagerAsync _dbManager;
        private readonly ICommonRepository _commonRepository;
        public FOPSettlementRepository(IOracleDbManagerAsync dbManager, ICommonRepository commonRepository)
        {
            _dbManager = dbManager;
            _commonRepository = commonRepository;
        }     

        public async Task<List<FOPSettlementViewModel>> GetFopSettlementAsync(PaginationRequest<FOPSettlementFilter> req, CancellationToken cancellationToken)
        {
            string filtersObj = JsonSerializer.Serialize(req.filter);

            var parameters = new FOPSettlementFilter();
            parameters.PageNo = req.PageNo;
            parameters.PageSize = req.PageSize;

            if (req.filter != null && !string.IsNullOrEmpty(req.filter.Trim()) && req.filter != "null")
            {
                FilterClass? filters = JsonSerializer.Deserialize<FilterClass>(filtersObj);
                foreach (var item in filters.filters)
                {
                }
            }

            if (req.sort.Length > 0 && req.sort != "[]")
            {
                var sortValue = JsonSerializer.Deserialize<List<Sort>>(req.sort);
                var field = sortValue.FirstOrDefault().field;
                var dir = sortValue.FirstOrDefault().dir;
                parameters.sorting = field + " " + dir;
            }

            parameters.SettlementDate = req.Filters.SettlementDate.HasValue ?  req.Filters.SettlementDate.Value: (DateTime?)null;
            parameters.TradeDate = req.Filters.TradeDate.HasValue ?  req.Filters.TradeDate.Value: (DateTime?)null;
            parameters.UniqueReference = req.Filters.UniqueReference;
            parameters.Rf48Id = req.Filters.Rf48Id;
            parameters.PortfolioGroupId = req.Filters.PortfolioGroupId;

            List <FOPSettlementViewModel> _lst = new List<FOPSettlementViewModel>();
            List<OracleParameter> lstParams = new List<OracleParameter>();

            lstParams.Add(new OracleParameter { ParameterName = "pview", OracleDbType = OracleDbType.RefCursor, Direction = ParameterDirection.Output });
            lstParams.Add(new OracleParameter() { ParameterName = "p_page_number", Value = parameters.PageNo, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "p_page_size", Value = parameters.PageSize, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "pSettlementDate", Value = parameters.SettlementDate, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "pTradeDate", Value = parameters.TradeDate, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "pUniqueReference", Value = parameters.UniqueReference, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "p_rf48_id", Value = parameters.Rf48Id, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "p_group", Value = parameters.PortfolioGroupId, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "p_sorting", Value = parameters.sorting, Direction = System.Data.ParameterDirection.Input, });

            _lst = await _dbManager.GetStoredProcedureRefCursorAsync<FOPSettlementViewModel>("Pkg_PRS50_FOP.GET_FOP_SETTLEMENT_NEW", lstParams, "pview", cancellationToken);

            return _lst;
        }

        public byte[] ExportToExcel(List<FOPSettlementViewModel> data)
        {
            var result = data.Select(item => new FOPSettlementExport()
            {
                TRADE_TYPE = item.TRADE_TYPE,
                TRANSFER_TYPE = item.TRANSFER_TYPE,
                TRADE_DATE = item.TRADE_DATE,
                SETTLEMENT_DATE = item.SETTLEMENT_DATE,
                SYMBOL = item.rf02_symbol,
                ISIN = item.ISIN,
                QUANTITY = item.QUANTITY.ToString(),
                UNIQUE_REFERENCE = item.UNIQUE_REFERENCE,
                SENDER_CUSTODIAN = item.SENDER_CUSTODIAN,
                SENDER_CUSTODIAN_ACCOUNT = item.ACC_SENDER_CUSTODIAN,
                RECEIVER_CUSTODIAN = item.RECEIVER_CUSTODIAN,
                RECEIVER_CUSTODIAN_ACCOUNT = item.ACC_RECEIVER_CUSTODIAN,
                MESSAGE_REF = item.MESSAGE_REF,
                REQUEST_MESSAGE = item.prs50_req_msg,
                HOLD_REQUEST_MESSAGE = item.prs50_hold_req_msg,
                CANCEL_REQUEST_MESSAGE = item.prs50_cancel_req_msg,
                RELEASE_REQUEST_MESSAGE = item.prs50_req_msg

            }).ToList();

            using var stream = new MemoryStream();

            using (var document = SpreadsheetDocument.Create(
                stream,
                SpreadsheetDocumentType.Workbook))
            {
                WorkbookPart workbookPart = document.AddWorkbookPart();
                workbookPart.Workbook = new Workbook();

                Sheets sheets = workbookPart.Workbook.AppendChild(
                    new Sheets());

                WorksheetPart worksheetPart =
                    workbookPart.AddNewPart<WorksheetPart>();

                SheetData sheetData = new SheetData();

                worksheetPart.Worksheet = new Worksheet(sheetData);

                Sheet sheet = new Sheet
                {
                    Id = workbookPart.GetIdOfPart(worksheetPart),
                    SheetId = 1,
                    Name = "FOP Settlement"
                };

                sheets.Append(sheet);

                // Header
                Row headerRow = new Row();

                string[] headers =
                {
            "TRADE_TYPE",
            "TRANSFER_TYPE",
            "TRADE_DATE",
            "SETTLEMENT_DATE",
            "SYMBOL",
            "ISIN",
            "QUANTITY",
            "UNIQUE_REFERENCE",
            "SENDER_CUSTODIAN",
            "SENDER_CUSTODIAN_ACCOUNT",
            "RECEIVER_CUSTODIAN",
            "RECEIVER_CUSTODIAN_ACCOUNT",
            "MESSAGE_REF",
            "REQUEST_MESSAGE",
            "HOLD_REQUEST_MESSAGE",
            "CANCEL_REQUEST_MESSAGE",
            "RELEASE_REQUEST_MESSAGE"
        };

                foreach (string header in headers)
                {
                    headerRow.Append(
                        CreateTextCell(header));
                }

                sheetData.Append(headerRow);

                // Data
                foreach (var item in result)
                {
                    Row row = new Row();

                    row.Append(CreateTextCell(item.TRADE_TYPE));
                    row.Append(CreateTextCell(item.TRANSFER_TYPE));
                    row.Append(CreateTextCell(item.TRADE_DATE));
                    row.Append(CreateTextCell(item.SETTLEMENT_DATE));
                    row.Append(CreateTextCell(item.SYMBOL));
                    row.Append(CreateTextCell(item.ISIN));
                    row.Append(CreateTextCell(item.QUANTITY));
                    row.Append(CreateTextCell(item.UNIQUE_REFERENCE));
                    row.Append(CreateTextCell(item.SENDER_CUSTODIAN));
                    row.Append(CreateTextCell(item.SENDER_CUSTODIAN_ACCOUNT));
                    row.Append(CreateTextCell(item.RECEIVER_CUSTODIAN));
                    row.Append(CreateTextCell(item.RECEIVER_CUSTODIAN_ACCOUNT));
                    row.Append(CreateTextCell(item.MESSAGE_REF));
                    row.Append(CreateTextCell(item.REQUEST_MESSAGE));
                    row.Append(CreateTextCell(item.HOLD_REQUEST_MESSAGE));
                    row.Append(CreateTextCell(item.CANCEL_REQUEST_MESSAGE));
                    row.Append(CreateTextCell(item.RELEASE_REQUEST_MESSAGE));

                    sheetData.Append(row);
                }

                workbookPart.Workbook.Save();
            }

            return stream.ToArray();
        }

        private static Cell CreateTextCell(object? value)
        {
            return new Cell
            {
                DataType = CellValues.InlineString,
                InlineString = new InlineString(
                    new Text(value?.ToString() ?? string.Empty))
            };
        }
    }
}