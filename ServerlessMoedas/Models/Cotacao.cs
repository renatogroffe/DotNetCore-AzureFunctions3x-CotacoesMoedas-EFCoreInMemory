using System;

namespace ServerlessMoedas.Models
{
    public class Cotacao
    {
        public string Sigla { get; set; }
        public DateTime? UltimaCotacao   { get; set; }
        public decimal? Valor { get; set; }
    }
}