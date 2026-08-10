using Directfn.Custody.ApiFramework.Common.DTOs.SubMarket;
using Directfn.Custody.ApiFramework.Common.DTOs.Symbol;
using System;
using System.Collections.Generic;
using System.Text;

namespace Directfn.Custody.ApiFramework.Repositories.Symbol
{
    public interface ISymbolRepository
    {
        Task<List<SymbolViewModel>> GetAllSymbolsAsync(CancellationToken cancellationToken);
        Task<SymbolViewModel> GetSymbolById(int symbolId, CancellationToken cancellationToken);
        Task<List<SymbolViewModel>> UpdatePostStatus(int rf02_symbol_id, int isPosted, int user_id, CancellationToken cancellationToken);
        Task<List<SymbolViewModel>> DeleteSymbol(int rf02_symbol_id, int user_id, CancellationToken cancellationToken);
        Task<SymbolReqModel> AddSymbol(SymbolReqModel _symbol, CancellationToken cancellationToken);
        Task<SymbolReqModel> UpdateSymbol(SymbolReqModel _symbol, CancellationToken cancellationToken);
    }
}