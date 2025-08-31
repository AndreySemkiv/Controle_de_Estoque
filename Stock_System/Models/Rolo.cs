namespace EstoqueRolos.Models
{
    public class Rolo
    {
        public int Id { get; set; }
        public string IdRolo { get; set; } = "";
        public int Milimetragem { get; set; }
        public double MetragemDisponivel { get; set; }
        public string WIP { get; set; } = "Estoque";
    }
}
