﻿namespace Domain.UserModels;

public class ExternalLoginModel
{
    public string Provider { get; set; }
    public string Token { get; set; }
}