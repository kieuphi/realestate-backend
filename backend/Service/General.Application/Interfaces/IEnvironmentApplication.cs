namespace General.Application.Interfaces
{
    public interface IEnvironmentApplication
    {
        string WebRootPath { get; }
        string EnvironmentName { get; }
    }
}
