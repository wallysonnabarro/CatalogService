namespace WorkerOrdem.Data
{
    public interface IOrdemRepository
    {
        Task CancelarOrdem(string mensagem);
    }
}
