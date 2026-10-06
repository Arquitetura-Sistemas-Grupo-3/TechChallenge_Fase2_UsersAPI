using Core.DTO;
using Core.Entidade;
using Core.Entidade.Enums;
using Core.Output;
using Core.Repository;
using Core.ValueObjects;
using Infra.Exceptions;
using MassTransit;
using MassTransit.Internals.ImTools;
using UsersAPI.Interface;
using BC = BCrypt.Net.BCrypt;

namespace UsersAPI.Services
{
    public class UsuarioService : IUsuarioService
    {
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly ILogger<UsuarioService> _logger;
        private readonly IMensageriaService _mensageriaService;
        

        public UsuarioService(IUsuarioRepository usuarioRepository, ILogger<UsuarioService> logger, IMensageriaService mensageriaService)
        {
            _usuarioRepository = usuarioRepository;
            _logger = logger;
            _mensageriaService = mensageriaService;
        }
        public async Task<ServiceResponse<List<UsuarioListarResposta>>> Listar(string? Nome = null, string? Email = null, string? NivelAcesso = null)
        {
            _logger.LogInformation("Buscando usuários com filtros Nome={Nome}, Email={Email}, NivelAcesso={NivelAcesso}", Nome, Email, NivelAcesso);

            var usuario = await _usuarioRepository.ListarUsuario();

            if (usuario == null)
                throw new ExcepetionUsuarioNaoEncontrado("Nenhum usuário encontrado");

            if (!string.IsNullOrWhiteSpace(Nome))
                usuario = usuario
                    .Where(u => u.Nome.Contains(Nome, StringComparison.OrdinalIgnoreCase))
                    .ToList();

            if (!string.IsNullOrWhiteSpace(Email))
                usuario = usuario
                    .Where(u => u.Email.Contains(Email, StringComparison.OrdinalIgnoreCase))
                    .ToList();

            if (!string.IsNullOrWhiteSpace(NivelAcesso))
                usuario = usuario
                    .Where(u => u.NivelAcesso.Contains(NivelAcesso, StringComparison.OrdinalIgnoreCase))
                    .ToList();

            _logger.LogInformation("Retornados {Count} usuários", usuario.Count);

            return ServiceResponse<List<UsuarioListarResposta>>.Ok(usuario);
        }

        public async Task<ServiceResponse<UsuarioAdicionarResposta>> AdicionarUsuario(UsuarioAdicionarRequisicao usuarioInput, NivelAcessoEnum nivelAcesso)
        {
            _logger.LogInformation("Adicionando usuário com e-mail {Email}", usuarioInput.Email);

            var senha = usuarioInput.Senha;
            var usuario = await _usuarioRepository.ValidaEmail(usuarioInput.Email);

            if (usuario != null)
            {
                _logger.LogWarning("Tentativa de cadastro com e-mail já existente {Email}", usuarioInput.Email);
                throw new ExceptionEmailCadastrado("E-mail já cadastrado");
            }

            if (!string.IsNullOrEmpty(senha))
                senha = BC.HashPassword(senha);
            else
                throw new ExceptionSenhaInvalida("Senha inválida");

            try
            {
                Usuario user = Usuario.AdicionarUsuario(usuarioInput, nivelAcesso, senha);

                await _usuarioRepository.Cadastrar(user);

                _logger.LogInformation("Usuário {Email} adicionado com sucesso, Id={Id}", usuarioInput.Email, user.Id);

                var e = await _mensageriaService.PublicarMensagemFila(user,CancellationToken.None);

                return ServiceResponse<UsuarioAdicionarResposta>.Ok(new UsuarioAdicionarResposta { Id = user.Id }, "Usuário adicionado com sucesso");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao adicionar usuário {Email}", usuarioInput.Email);
                throw new Exception($"Erro: {ex}");
            }
        }

        public async Task<ServiceResponse<UsuarioBuscarPorIdResposta>> BuscarPorId(int id)
        {
            _logger.LogInformation("Buscando usuário {Id}", id);

            var usuario = await _usuarioRepository.BuscarUsuarioPorId(id);

            if (usuario == null)
                throw new ExcepetionUsuarioNaoEncontrado("Usuário não encontrado");

            return ServiceResponse<UsuarioBuscarPorIdResposta>.Ok(usuario);
        }

        public async Task<ServiceResponse> AtualizarUsuario(UsuarioAtualizarRequisicao usuarioUpdate, int id)
        {
            _logger.LogInformation("Atualizando usuário {Id}", id);

            var usuario = await _usuarioRepository.ObterPorId(id);

            if (usuario == null) throw new ExcepetionUsuarioNaoEncontrado("Usuário não encontrado");

            string? senha;

            if (!string.IsNullOrEmpty(usuarioUpdate.Senha)) senha = BC.HashPassword(usuarioUpdate.Senha);
            else senha = null;

            usuario.Atualizar(usuario, usuarioUpdate, senha);

            await _usuarioRepository.AlterarAsync(usuario);

            _logger.LogInformation("Usuário {Id} atualizado com sucesso", id);

            return ServiceResponse.Ok("Usuário atualizado com sucesso");
        }

        public async Task<ServiceResponse> RemoverUsuario(int id)
        {
            _logger.LogInformation("Removendo usuário {Id}", id);

            var usuario = await _usuarioRepository.ObterPorId(id);

            if (usuario == null)
                throw new ExcepetionUsuarioNaoEncontrado("Usuário não encontrado");

            usuario.Desativar();
            await _usuarioRepository.AlterarAsync(usuario);

            _logger.LogInformation("Usuário {Id} removido com sucesso", id);

            return ServiceResponse.Ok("Deletado com sucesso");
        }

        public async Task<ServiceResponse<UsuarioBuscarAutenticadoResposta>> BuscarAutenticado(string email)
        {
            _logger.LogInformation("Buscando usuário autenticado {Email}", email);

            var usuario = await _usuarioRepository.BuscarUsuarioPorEmail(email);

            if (usuario == null)
                throw new ExcepetionUsuarioNaoEncontrado("Usuário não encontrado");

            return ServiceResponse<UsuarioBuscarAutenticadoResposta>.Ok(usuario);
        }



        public class UsuarioCriado
        {
            public Guid GUID { get; set; }
            public string nomeUsuario { get; set; }
            public Email emailUsuario { get; set; }
            public DateTime dataEvento { get; set; }

            public UsuarioCriado(Guid gUID, string nomeUsuario, Email emailUsuario, DateTime dataEvento)
            {
                GUID = gUID;
                this.nomeUsuario = nomeUsuario;
                this.emailUsuario = emailUsuario;
                this.dataEvento = dataEvento;
            }
        }

    }
}
