namespace Backplane.Common;

public partial interface IClientReceiveHub
{
    Task Receive(string message);
    Task ReceiveDto(Dto dto);
}
