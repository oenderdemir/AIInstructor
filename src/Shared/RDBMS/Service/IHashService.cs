namespace AIInstructor.src.Shared.RDBMS.Service
{
    public interface IHashService
    {
        Task<string> ComputeHash(string data);
    }
}
