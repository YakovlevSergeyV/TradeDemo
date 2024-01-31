using System;

namespace TradesReader.Api.Dto
{
    internal class ParametersTrade 
    {
        public string Symbol { get; set; }
        public DateTime TimeStart { get; set; }
        public DateTime TimeEnd { get; set; }

        public SymbolInfo SymbolInfo { get; set; }
    }
}
