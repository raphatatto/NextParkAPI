using System;

namespace NextParkAPI.Models
{
    public class Manutencao
    {
        public int IdManutencao { get; set; }
        public string? DsManutencao { get; set; }
        public DateTime? DtInicio { get; set; }
        public DateTime? DtFim { get; set; }
        public int IdMoto { get; set; }
    }
}
