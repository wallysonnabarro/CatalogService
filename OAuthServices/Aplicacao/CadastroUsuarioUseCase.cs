using OAuthServices.Data;
using OAuthServices.Models;
using OAuthServices.Models.Resultados;
using OAuthServices.Services;

namespace OAuthServices.Aplicacao
{
    public class CadastroUsuarioUseCase : ICadastroUsuarioUseCase
    {
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly ICodeGenerate _utilitario;
        private readonly IGenerateHashs _generateHashs;
        public CadastroUsuarioUseCase(IUsuarioRepository usuarioRepository, IRoleRepository roleRepository,
            ICodeGenerate utilitario, IGenerateHashs generateHashs)
        {
            _usuarioRepository = usuarioRepository;
            _roleRepository = roleRepository;
            _utilitario = utilitario;
            _generateHashs = generateHashs;
        }

        public async Task<Result<Guid>> Executar(NovoUsuarioModel model)
        {
            try
            {
                if (model == null)
                {
                    throw new ArgumentNullException(nameof(model), "Usuário não pode ser nulo.");
                }
                if (string.IsNullOrWhiteSpace(model.Nome))
                {
                    throw new ArgumentException("O nome do usuário não pode ser vazio.", nameof(model.Nome));
                }
                if (string.IsNullOrWhiteSpace(model.Email))
                {
                    throw new ArgumentException("O email do usuário não pode ser vazio.", nameof(model.Email));
                }

                var role = await _roleRepository.GetRoleByName("GESTOR");

                if (role == null) return Result<Guid>.Failed(new Errors { Description = "Role não encontrada." });

                var codigo = await _utilitario.GerarCodigo();

                if (codigo == null) return Result<Guid>.Failed(new Errors { Description = "Erro ao gerar o código. " });

                var salt = await _generateHashs.GenerateSalt();

                if (salt == null || salt.Length == 0)
                {
                    return Result<Guid>.Failed(new Errors { Description = "Erro ao tentar atualizar a senha." });
                }

                var usuario = new Usuario
                {
                    Email = model.Email,
                    Nome = model.Nome,
                    PasswordHash = Convert.ToBase64String(await _generateHashs.GeneratePasswordHash(model.Senha, salt)),
                    Salt = Convert.ToBase64String(salt),
                    SecurityStamp = Guid.NewGuid().ToString(),
                    EmailConfirmed = false,
                    Roles = role
                };

                var id = await _usuarioRepository.Novo(usuario);

                return id != Guid.Empty
                    ? Result<Guid>.Sucesso(id)
                    : Result<Guid>.Failed(new Errors { Description = "Erro ao cadastrar o usuário." });
            }
            catch (Exception e)
            {
                return Result<Guid>.Failed(new Errors { Description = e.Message });
            }
        }
    }
}
