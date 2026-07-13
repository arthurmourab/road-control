namespace RC.Domain.Interfaces.Services
{
    // Geração/validação determinística (estilo TOTP) do código de confirmação do frentista.
    // Não guarda códigos: tudo é derivado do segredo do usuário + do instante atual.
    public interface IConfirmationCodeService
    {
        // Duração de cada janela (em segundos). O código muda a cada PeriodSeconds.
        int PeriodSeconds { get; }

        // Gera um novo segredo aleatório (hex), para persistir no usuário frentista.
        string GenerateSecret();

        // Deriva o código de 6 dígitos válido para o instante informado.
        string GetCode(string secret, DateTime instantUtc);

        // Confere o código na janela atual e na imediatamente anterior (grace de 1 janela).
        bool IsValid(string secret, string code, DateTime instantUtc);
    }
}
