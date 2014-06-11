namespace Diebold.Services.Infrastructure
{
    public class ValidationResult
    {
        public string Key { get; private set; }

        public string Message { get; private set; }

        public ValidationResult(string key, string message) 
        {
            Key = key;
            Message = message;
        }
    }
}
