namespace RC.Shared.Dtos.Common
{
    // Body dos endpoints PATCH .../{id}/status (ativar/desativar)
    public class UpdateStatusDto
    {
        public bool IsActive { get; set; }
    }
}
