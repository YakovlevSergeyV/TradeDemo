﻿namespace TradesStorage.Api.Infrastructure.Services
{
    using System.Collections.Generic;
    using TradesStorage.Infrastructure.Dto;

    public interface ISymbolDictionary
    {
        IDictionary<string, Symbol> Symbols { get; }
        bool Exists(Symbol symbol);
        void AddSymbol(Symbol symbol);
        void DeleteSymbol(Symbol symbol);
    }
}
