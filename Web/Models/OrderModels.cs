using System.ComponentModel.DataAnnotations;

namespace Web.Models
{
    public class ProdutoSelecionado
    {
        public Guid ProdutoId { get; set; }
        public string Nome { get; set; } = string.Empty;
        public decimal Valor { get; set; }
        public int Quantidade { get; set; } = 1;
        public int QuantidadeEstoque { get; set; }
    }

    public class OrdemServicoViewModel
    {
        public List<ProdutoSelecionado> ProdutosSelecionados { get; set; } = new List<ProdutoSelecionado>();
        public decimal Total => ProdutosSelecionados.Sum(p => p.Valor * p.Quantidade);
    }

    public class ProdutoParaOrdem
    {
        public Guid ProdutoId { get; set; }
        public int Quantidade { get; set; }
    }
}
