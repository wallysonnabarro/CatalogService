using Microsoft.AspNetCore.Mvc;
using OAuthServices.Aplicacao;
using OAuthServices.Models;

namespace OAuthServices.Controllers
{
    [Route("api/usuario")]
    [ApiController]
    public class UsuarioController(ICadastroUsuarioUseCase _cadastro, IValidarEmailUseCase _validarEmail) : ControllerBase
    {
        [HttpPost("novo")]
        public async Task<IActionResult> Novo(NovoUsuarioModel model)
        {
            var result = await _cadastro.Executar(model);

            if (result.Succeeded)
            {
                //await _logsService.RegistrarLogInfoAsync($"Retornando resultado: Sucesso={result.Succeeded}", "PaymentController", "RegistrarUsuario");

                return Ok();
            }

            //await _logsService.RegistrarLogInfoAsync($"Não foi possivel finalizar seu cadastro. E-mail: {model.Email}, usuário: {model.Nome} e error: {result.ToString()}", "PaymentController", "RegistrarUsuario");

            return BadRequest(new { message = $"Não foi possivel finalizar seu cadastro. {result.ToString()}" });
        }

        [HttpPost("confirmar-email")]
        public async Task<IActionResult> ConfirmarEmail([FromBody] string email)
        {
            var result = await _validarEmail.Executar(email);

            if (!result.Succeeded) return BadRequest(new { message = result.ToString() });

            return Ok(result.Dados);
        }
    }
}
