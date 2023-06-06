using Microsoft.AspNetCore.Identity;
using Test2.Extensions.common.consts;
using Test2.models;

namespace Test2.Extensions.common.helpers;

public static class CreateUserRoles
{
    public static async Task CreateRoles(RoleManager<Role> roleManager)
    {
        var roleCheck = await roleManager.RoleExistsAsync(UserRoles.User);
        roleCheck = await roleManager.RoleExistsAsync(UserRoles.Admin);
        if (!roleCheck)
        {
            await roleManager.CreateAsync(new Role(UserRoles.Admin));
        }
        roleCheck = await roleManager.RoleExistsAsync(UserRoles.User);
        if (!roleCheck)
        {
            await roleManager.CreateAsync(new Role(UserRoles.User));
        }
        roleCheck = await roleManager.RoleExistsAsync(UserRoles.Moderator);
        if (!roleCheck)
        {
            await roleManager.CreateAsync(new Role(UserRoles.Moderator));
        }
    }
}