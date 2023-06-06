using Microsoft.AspNetCore.Http;
using Test2.Extensions.common.consts;

namespace Test2.Extensions.common.helpers;

public static class GetAuthUser
{
    public static Guid GetCurrentUser(this HttpContext context)
    {
        var userId = context.User.Claims.FirstOrDefault(item => item.Type == ClaimTypes.UserId)?.Value;
        Guid.TryParse(userId, out var guid);
        return guid;
    }
}