namespace TradesStorage.Api.Infrastructure.Services
{
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using TradesStorage.Infrastructure.Dto;

    public class SymbolDictionary : ISymbolDictionary
    {
        private readonly ConcurrentDictionary<string, Symbol> symbols;

        public SymbolDictionary()
        {
            symbols = new ConcurrentDictionary<string, Symbol>();
        }

        public IDictionary<string, Symbol> Symbols => symbols;

        public bool Exists(Symbol symbol)
        {
            var key = GetSymbolKey(symbol);
            return symbols.ContainsKey(key);
        }

        public void AddSymbol(Symbol symbol)
        {
            var key = GetSymbolKey(symbol);

            if (!symbols.ContainsKey(key))
            {
                symbols.GetOrAdd(key, symbol);
            }
        }

        public void DeleteSymbol(Symbol symbol)
        {
            var key = GetSymbolKey(symbol);
            if (symbols.ContainsKey(key))
            {
                if (!symbols.TryRemove(key, out _))
                {
                    //Надо записать в лог
                }
            }
        }

        private string GetSymbolKey(Symbol symbol)
        {
            return $"{symbol.ExchangeName}-{symbol.CurrencyPairName}";
        }
    }
}
