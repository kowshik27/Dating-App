﻿namespace API.Interfaces;

public interface ITokenService
{
    string CreateToken(string Username);
}
