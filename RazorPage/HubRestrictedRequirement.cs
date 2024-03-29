﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace RazorPage
{
    public class HubRestrictedRequirement :
        AuthorizationHandler<HubRestrictedRequirement, HubInvocationContext>,
        IAuthorizationRequirement
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context,
            HubRestrictedRequirement requirement,
            HubInvocationContext resource)
        {
            Console.WriteLine(resource.HubMethodName);
            context.Succeed(requirement);
            //if (context.User.Identity != null &&
            //  !string.IsNullOrEmpty(context.User.Identity.Name) &&
            //  IsUserAllowedToDoThis(resource.HubMethodName,
            //                       context.User.Identity.Name) &&
            //  context.User.Identity.Name.EndsWith("@microsoft.com"))
            //{
            //    context.Succeed(requirement);
            //}
            return Task.CompletedTask;
        }
        //private bool IsUserAllowedToDoThis(string hubMethodName,
        //    string currentUsername)
        //{
        //    return !(currentUsername.Equals("asdf42@microsoft.com") &&
        //        hubMethodName.Equals("banUser", StringComparison.OrdinalIgnoreCase));
        //}
    }
}
