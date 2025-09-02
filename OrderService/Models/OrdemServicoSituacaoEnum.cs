namespace OrderService.Models
{
    public enum OrdemServicoSituacaoEnum
    {
        Aberta = 1,          // Quando o cliente solicita a OS
        EmAndamento = 2,     // Técnico está executando
        AguardandoPecas = 3, // Esperando material/peças
        AguardandoCliente = 4, // Parada aguardando retorno do cliente
        Concluida = 5,       // Serviço finalizado
        Cancelada = 6        // OS cancelada
    }
}
