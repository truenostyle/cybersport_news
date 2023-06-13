﻿using Microsoft.Extensions.FileSystemGlobbing;
using System.Text.RegularExpressions;

namespace ASP_1.Services.Validation
{
    public class ValidationServiceV1 : IValidationService
    {
        public bool Validate(string source, params ValidationTerms[] terms)
        {
            if (terms.Length == 0) throw new ArgumentException("No term(s) for validation");
            if(terms.Length == 1 && terms[0] == ValidationTerms.None)
            {
                return true;
            }
            bool result = true;
            if (terms.Contains(ValidationTerms.NotEmpty))
            {
                result &= ValidateNotEmpty(source);
            }

            if(terms.Contains(ValidationTerms.Login))
            {
                result &= ValidateLogin(source); 
            }

            if (terms.Contains(ValidationTerms.Email))
            {
                result &= ValidateEmail(source);
            }

            if (terms.Contains(ValidationTerms.RealName))
            {
                result &= ValidateRealName(source);
            }
            if (terms.Contains(ValidationTerms.Password))
            {
                result &= ValidatePassword(source);
            }
            if (terms.Contains(ValidationTerms.Description))
            {
                result &= ValidateDescription(source);
            }



            return result;
        }
        private static bool ValidateRegex(string source, string pattern)
        {
            return Regex.IsMatch(source, pattern);
        }

        private static bool ValidateLogin(string source)
        {
            return ValidateRegex(source, @"^\w{3,}$");
        }

        private static bool ValidateEmail(string source)
        {
            return ValidateRegex(source, @"^[\w.%+-]+@[\w.-]+\.[a-zA-Z]{2,}$");
        }

        private static bool ValidateRealName(string source)
        {
            return ValidateRegex(source, @"^.+$");
        }

        private static bool ValidatePassword(string source)
        {
            return source.Length >= 3;
        }

        private bool ValidateNotEmpty(String source)
        {
            return !String.IsNullOrEmpty(source);
        }

        private bool ValidateDescription(String source)
        {
            return source.Length <= 25;
        }
    }

   
}
