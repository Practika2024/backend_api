﻿namespace Domain.Users.Models
{
    public class JwtModel
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
    }
}
