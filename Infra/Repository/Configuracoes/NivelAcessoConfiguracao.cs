using Core.Entidade;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infra.Repository.Configuracoes
{
    public class NivelAcessoConfiguracao : IEntityTypeConfiguration<NivelAcesso>
    {
        public void Configure(EntityTypeBuilder<NivelAcesso> builder)
        {
            builder.ToTable("NivelAcesso");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).HasColumnType("INT").UseIdentityColumn();
            builder.Property(x => x.Nome).IsRequired().HasMaxLength(100);
            builder.Property(x => x.Descricao).IsRequired().HasMaxLength(250);

            builder.HasData(
                new NivelAcesso { Id = 1, Nome = "Admin", Descricao = "Acesso global na aplicação" },
                new NivelAcesso { Id = 2, Nome = "Usuário", Descricao = "Usuário comum da aplicação" }
                );
        }
    }
}
