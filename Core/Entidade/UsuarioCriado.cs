using Core.ValueObjects;

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