﻿using System.Globalization;
using Domain.EmailVerificationToken;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Application.Services.EmailVerificationLinkFactory;

public sealed class EmailVerificationLinkFactory(IHttpContextAccessor httpContextAccessor, LinkGenerator linkGenerator)
{
    public string Create(EmailVerificationToken emailVerificationToken)
    {
        string? verificationLink = linkGenerator.GetUriByName(httpContextAccessor.HttpContext!,
            "VerifyEmail", new { token = emailVerificationToken.Id });

        return verificationLink ?? throw new Exception("Could not create email verification link");
    }
}