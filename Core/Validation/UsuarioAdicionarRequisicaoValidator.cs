using Core.DTO;
using FluentValidation;

namespace Core.Validation
{
    public class UsuarioAdicionarRequisicaoValidator : AbstractValidator<UsuarioAdicionarRequisicao>
    {
        public UsuarioAdicionarRequisicaoValidator()
        {
            RuleFor(x => x.Nome)
                .NotEmpty().WithMessage("O nome é obrigatório.");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("O e-mail é obrigatório.")
                .EmailAddress().WithMessage("Formato de e-mail inválido.");

            RuleFor(x => x.Senha)
                .NotEmpty().WithMessage("A senha é obrigatória.")
                .MinimumLength(8).WithMessage("A senha deve ter no mínimo 8 caracteres.")
                .Matches(@"[A-Z]").WithMessage("A senha deve conter ao menos uma letra maiúscula.")
                .Matches(@"[a-z]").WithMessage("A senha deve conter ao menos uma letra minúscula.")
                .Matches(@"[0-9]").WithMessage("A senha deve conter ao menos um número.")
                .Matches(@"[\W_]").WithMessage("A senha deve conter ao menos um caractere especial.");
        }
    }
}