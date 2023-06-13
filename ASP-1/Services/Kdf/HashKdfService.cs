﻿using ASP_1.Services.Hash;

namespace ASP_1.Services.Kdf
{
    public class HashKdfService : IKdfService
    {
        private readonly IHashService _hashService;

        public HashKdfService(IHashService hashService)
        {
            _hashService = hashService;
        }

        public string GetDerivedKey(string password, string salt)
        {
            return _hashService.Hash(salt + password);
        }
    }
}
