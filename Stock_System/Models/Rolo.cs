namespace EstoqueRolos.Models
{
    public class Rolo
    {
        public string Code { get; set; } = string.Empty;      
        public string Descricao { get; set; } = string.Empty;
        public int Milimetragem { get; set; }
        public decimal MOQ { get; set; }
        public decimal Estoque { get; set; }
        public decimal MetragemWIP { get; set; }
    }
}

