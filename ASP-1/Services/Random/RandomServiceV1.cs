using ASP_1.Models.User;
using ASP_1.Services.Hash;
using ASP_1.Data;
using ASP_1.Data.Entity;
using ASP_1.Services.Kdf;
using Microsoft.Extensions.Primitives;

namespace ASP_1.Services.Random
{
    public class RandomServiceV1 : IRandomService
    {
        private readonly String _codeChars = "abcdefghijklmnopqrstuvwxyz0123456789";
        private readonly String _safeChars = new String(Enumerable.Range(20, 107).Select(x => (char)x).ToArray());
        public readonly IHashService _hashService;

        public RandomServiceV1(IHashService hashService)
        {
            _hashService = hashService;
        }

        private readonly System.Random _random = new();

        public string ConfirmCode(int length)
        {
            char[] chars = new char[length];
            for(int i = 0; i < length; i++)
            {
                chars[i] = _codeChars[_random.Next(_codeChars.Length)];
            }
            return new String(chars);
        }

        public string RandomString(int length)
        {
            char[] chars = new char[length];
            for (int i = 0; i < length; i++)
            {
                chars[i] = _safeChars[_random.Next(_safeChars.Length)];
            }
            return new String(chars);
        }



        public string RandomAvatar(RegistrationModel registrationModel, string ext)
        {
            String savedName = null!;
            if (registrationModel.Avatar is not null)
            {
                savedName = _hashService.Hash("avtr" + DateTime.Now + registrationModel.Avatar.FileName )[..16] + ext;
                return savedName;
            }
            return "Avatar is null";
        }

    }
}
