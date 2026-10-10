using Core.DTO;
using Core.Entidade;
using Core.Entidade.Enums;
using Core.Output;
using Core.Repository;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using UsersAPI.Interface;
using BC = BCrypt.Net.BCrypt;

namespace WebAPI.Service
{
    public class AutenticacaoService : IAuthService
    {
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly INivelAcessoRepository _nivelAcessoRepository;
        private readonly IConfiguration _configuration;
        private readonly ILogger<AutenticacaoService> _logger;

        public AutenticacaoService(IUsuarioRepository usuarioRepository, INivelAcessoRepository nivelAcessoRepository, IConfiguration configuration, ILogger<AutenticacaoService> logger)
        {
            _usuarioRepository = usuarioRepository;
            _nivelAcessoRepository = nivelAcessoRepository;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<ServiceResponse<AutenticarResposta>> Autenticar(string email, string senha)
        {
            _logger.LogInformation("Tentativa de login para {Email}", email);

            Usuario? usuario = await _usuarioRepository.ValidaEmailSenha(email);

            if (usuario == null || !BC.Verify(senha, usuario.Senha))
            {
                _logger.LogWarning("Falha de autenticação para {Email}", email);
                return ServiceResponse<AutenticarResposta>.Fail("E-mail ou senha inválidos");
            }

            NivelAcesso acesso = await _nivelAcessoRepository.ObterPorId(usuario.NivelAcessoId);

            var nivelEnum = (NivelAcessoEnum)usuario.NivelAcessoId;
            string jwt = GerarJWT(usuario.Email.Endereco, acesso.Nome);

            AutenticarResposta auth = new AutenticarResposta
            {
                NomeUsuario = usuario.Nome,
                Token = jwt
            };

            _logger.LogInformation("Login realizado com sucesso para {Email}", email);

            return ServiceResponse<AutenticarResposta>.Ok(auth);
        }

        private string GerarJWT(string username, string role)
        {
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, username),
                new Claim(ClaimTypes.Role, role),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(30),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
