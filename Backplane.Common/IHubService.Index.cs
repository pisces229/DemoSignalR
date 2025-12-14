namespace Backplane.Common;

public partial interface IHubService
{
    Task Send(string message);
}
