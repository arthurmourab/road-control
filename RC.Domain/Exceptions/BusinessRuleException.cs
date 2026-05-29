namespace RC.Domain.Exceptions
{
    // Violação de regra de negócio (requisição bem-formada, mas que não pode ser processada).
    // Mapeada para HTTP 422 no ExceptionHandlingMiddleware.
    public class BusinessRuleException(string message) : Exception(message)
    {
    }
}
