namespace OAuthServices.Aplicacao
{
    public interface ICodeGenerate
    {
        Task<string> GerarCodigo();
    }
}
