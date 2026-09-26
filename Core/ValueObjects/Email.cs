using System.Text.RegularExpressions;

namespace Core.ValueObjects
{
    public class Email : IEquatable<Email>
    {
        private static readonly Regex FormatoValido = new(
            @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
            RegexOptions.Compiled);

        public string Endereco { get; }

        public Email(string endereco)
        {
            if (string.IsNullOrWhiteSpace(endereco))
                throw new ArgumentException("O e-mail é obrigatório.", nameof(endereco));

            endereco = endereco.Trim();

            if (!FormatoValido.IsMatch(endereco))
                throw new ArgumentException("Formato de e-mail inválido.", nameof(endereco));

            Endereco = endereco.ToLowerInvariant();
        }

        public static bool TentarCriar(string? endereco, out Email? email)
        {
            try
            {
                email = new Email(endereco ?? string.Empty);
                return true;
            }
            catch (ArgumentException)
            {
                email = null;
                return false;
            }
        }

        public bool Equals(Email? other) => other is not null && Endereco == other.Endereco;

        public override bool Equals(object? obj) => Equals(obj as Email);

        public override int GetHashCode() => Endereco.GetHashCode();

        public override string ToString() => Endereco;

        public static bool operator ==(Email? left, Email? right) =>
            left is null ? right is null : left.Equals(right);

        public static bool operator !=(Email? left, Email? right) => !(left == right);
    }
}
