namespace TradesReader.Api.Infrastructure.Services
{
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using TradesReader.Api.Dto;

    internal class SymbolDictionary : ISymbolDictionary
    {
        private readonly ConcurrentDictionary<string, Symbol> _symbols;

        public SymbolDictionary()
        {
            _symbols = new ConcurrentDictionary<string, Symbol>();
        }

        public IDictionary<string, Symbol> Symbols => _symbols;

        public bool Exists(Symbol symbol)
        {
            var key = GetSymbolKey(symbol);
            return _symbols.ContainsKey(key);
        }

        public void AddSymbol(Symbol symbol)
        {
            var key = GetSymbolKey(symbol);

            if (!_symbols.ContainsKey(key))
            {
                _symbols.GetOrAdd(key, symbol);
            }
        }

        public void DeleteSymbol(Symbol symbol)
        {
            var key = GetSymbolKey(symbol);
            if (_symbols.ContainsKey(key))
            {
                if (!_symbols.TryRemove(key, out _))
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
