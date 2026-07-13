namespace RC.Shared.Dtos.Fueling
{
    // Código de confirmação corrente do frentista + validade, para o front exibir e renovar.
    public class ConfirmationCodeDto
    {
        public string Code { get; set; } = string.Empty;
        public DateTime ExpiresAt { get; set; }      // UTC
        public int SecondsRemaining { get; set; }
    }
}
