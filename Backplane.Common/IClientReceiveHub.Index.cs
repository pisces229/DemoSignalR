namespace Backplane.Common;

public partial interface IClientReceiveHub
{
    Task Receive(string message);
}
