namespace BuildingBlocks.Exceptions
{
    public class NotFountException:Exception
    {
        public NotFountException(string message): base(message) 
        {
            
        }

        public NotFountException(string name, object key): base($"Entity \"{name}\" ({key}) was not found.")
        {
            
        }
    }
}
