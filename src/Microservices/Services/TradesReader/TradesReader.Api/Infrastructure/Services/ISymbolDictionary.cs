namespace TradesReader.Api.Infrastructure.Services
{
    using System.Collections.Generic;
    using TradesReader.Api.Dto;

    public interface ISymbolDictionary
    {
        IDictionary<string, Symbol> Symbols { get; }
        bool Exists(Symbol symbol);
        void AddSymbol(Symbol symbol);
        void DeleteSymbol(Symbol symbol);
    }
}
