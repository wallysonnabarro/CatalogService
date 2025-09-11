using Microsoft.EntityFrameworkCore;

namespace WorkerOrdem.Data
{
    public class OrdemRepository : IOrdemRepository
    {
        private readonly ContextDb _context;
        private readonly ILogger<OrdemRepository> _logger;

        public OrdemRepository(ContextDb context, ILogger<OrdemRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task CancelarOrdem(string mensagem)
        {
            try
            {
                var ordem = await _context.OrdermServicos
                    .FirstOrDefaultAsync(o => o.Id == Guid.Parse(mensagem));

                if (ordem == null)
                {
                    _logger.LogWarning($"Ordem com ID {mensagem} não encontrada para cancelamento.");
                    return;
                }
                else
                {
                    ordem.Status = "Cancelada";
                    _logger.LogWarning($"Ordem com ID {mensagem} cancelada.");
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Erro ao cancelar ordem com a mensagem: {mensagem}");
                throw;
            }
        }
    }
}
