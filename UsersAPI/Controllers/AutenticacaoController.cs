using Core.DTO;
using Core.Output;
using Microsoft.AspNetCore.Mvc;
using UsersAPI.Interface;

namespace WebAPI.Controllers
{
    /// <summary>
    /// Autenticação e autorização de usuários.
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    [Produces("application/json")]
    public class AutenticacaoController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AutenticacaoController(IAuthService authService)
        {
            _authService = authService;
        }

        /// <summary>
        /// Autentica um usuário e retorna o token JWT de acesso.
        /// </summary>
        /// <param name="email">E-mail do usuário.</param>
        /// <param name="senha">Senha do usuário.</param>
        /// <returns>Nome do usuário e token JWT.</returns>
        /// <response code="401">Credenciais inválidas.</response>
        [HttpGet]
        [ProducesResponseType(typeof(ServiceResponse<AutenticarResposta>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ServiceResponse), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Autenticar(string email, string senha)
        {
            var response = await _authService.Autenticar(email, senha);

            if (!response.Success) return Unauthorized(response);

            return Ok(response);
        }
    }
}
