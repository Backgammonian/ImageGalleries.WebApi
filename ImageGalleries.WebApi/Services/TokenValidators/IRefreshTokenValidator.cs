﻿namespace ImageGalleries.WebApi.Services.TokenValidators
{
    public interface IRefreshTokenValidator
    {
        bool Validate(string refreshToken);
    }
}
