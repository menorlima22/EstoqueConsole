using System;

namespace SeuProjeto.Modelo
{
    public class Movimento
    {
        public int Id { get; set; }
        public int ProdutoId { get; set; }
        public string Tipo { get; set; } // ENTRADA / SAIDA
        public int Quantidade { get; set; }
        public DateTime Data { get; set; }
        public string Observacao { get; set; }
    }
}
