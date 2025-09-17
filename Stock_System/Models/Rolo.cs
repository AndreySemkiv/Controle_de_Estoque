namespace EstoqueRolos.Models
{
    public class Rolo
    {
        public string Code { get; set; } = string.Empty;      
        public string Descricao { get; set; } = string.Empty;
        public Double Milimetragem { get; set; }
        public int MOQ { get; set; }
        public int Estoque { get; set; }
        public int MetragemWIP { get; set; }
    }
}

