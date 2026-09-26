using Core.Entidade;
using Core.Entidade.Enums;
using Core.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infra.Repository.Configuracoes
{
    public class UsuarioConfiguracao : IEntityTypeConfiguration<Usuario>
    {
        public void Configure(EntityTypeBuilder<Usuario> builder)
        {
            builder.ToTable("Usuario");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).HasColumnType("INT").UseIdentityColumn();
            builder.Property(x => x.Nome).IsRequired().HasMaxLength(100);
            builder.Property(x => x.Email)
                .IsRequired()
                .HasMaxLength(100)
                .HasConversion(email => email.Endereco, valor => new Email(valor));
            builder.Property(x => x.Senha).IsRequired().HasMaxLength(100);
            builder.Property(x => x.NivelAcessoId).HasColumnType("INT").IsRequired();

            builder.HasOne(n => n.NivelAcesso)
                .WithMany(u => u.Usuario)
                .HasForeignKey(u => u.NivelAcessoId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasData(new Usuario
            {
                Id = 1,
                Nome = "admin",
                Email = new Email("admin@gmail.com"),
                Senha = "$2a$11$/pStnQtIExzkwxiUM5r5yOmT972StJfW5M.j34r4xwPNerS6mSStC",
                NivelAcessoId = (int)NivelAcessoEnum.Admin,
                DataCriacao = new DateTime(2026, 1, 1),
                Ativo = true
            });
        }
    }
}
