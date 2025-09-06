using OAuthServices.Data;
using OAuthServices.Models.Resultados;

namespace OAuthServices.Aplicacao
{
    public class ValidarEmailUseCase(IUsuarioRepository _usuario) : IValidarEmailUseCase
    {
        public async Task<Result<bool>> Executar(string input)
        {
            try
            {
                // Pode ser feita uma validação mais complexa do email aqui

                if (string.IsNullOrWhiteSpace(input) || !input.Contains("@"))
                {
                    return Result<bool>.Failed(new Errors() { Code = "400", Description = "Email inválido." });
                }

                var existe = await _usuario.GetUserByEmailAny(input);

                return Result<bool>.Success;
            }
            catch (Exception e)
            {
                return Result<bool>.Failed(new Errors() { Code = "500", Description = e.Message });
            }
        }
    }
}
