﻿namespace AuthApiTemplate.Models
{
    public class JwtSettings
    {
        public required string Key { get; set; }
        public required string Issuer { get; set; }
        public required string Audience { get; set; }
        public required int ExpireMinutes { get; set; }
    }
}
