using Test2.Extensions.common.enums;

namespace Test2.API.ViewModels.Requests;

public class FriendActionRequest
{
    public Guid FriendRequestId { get; set; }
    public int ActionType { get; set; }
}