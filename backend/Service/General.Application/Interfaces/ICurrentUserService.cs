namespace General.Application.Interfaces
{
    public interface ICurrentUserService
    {
        string UserId { get; }
        string UserName { get; }
    }
}
