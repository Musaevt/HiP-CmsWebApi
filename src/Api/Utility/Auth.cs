﻿using System;
using System.Security.Claims;
using System.Security.Principal;

namespace PaderbornUniversity.SILab.Hip.CmsApi.Utility
{
    public static class Auth
    {
        // Adds function to get User Id from Context.User.Identity
        public static string GetUserIdentity(this IIdentity identity)
        {
            var claimsIdentity = identity as ClaimsIdentity;
            if (claimsIdentity != null)
                return claimsIdentity.FindFirst(ClaimTypes.Name).Value;
            throw new InvalidOperationException("identity not found");
        }
    }
}
