﻿namespace webapi.DTOs.Account
{
    public class SearchedAccountDto
    {
        public required string Fullname { get; set; }
        public required string Email { get; set; }
        public required string Role { get; set; }
        public string? PicturePath { get; set; }
    }
}
