using System.Text;

namespace OAuthServices.Aplicacao
{
    public class CodeGenerate : ICodeGenerate
    {
        public async Task<string> GerarCodigo()
        {
            return await Task.Run(() =>
            {
                const string caracteres = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
                var random = new Random();
                var sb = new StringBuilder();

                for (int i = 0; i < 10; i++)
                {
                    var indiceAleatorio = random.Next(caracteres.Length);
                    sb.Append(caracteres[indiceAleatorio]);
                }

                return sb.ToString();
            });
        }
    }
}
