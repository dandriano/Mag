using Phils.Interfaces;
using System;
using System.Collections.Generic;

namespace Phils.Services
{
    public class UserRegistrationService : IUserRegistrationService
    {
        private readonly string _publicKey;
        private readonly Dictionary<string, string> _pendingRegistrations = [];

        public UserRegistrationService(string publicKey)
        {
            _publicKey = publicKey;
        }

        public bool RegisterUser(string userName)
        {
            var confirmationCode = GenerateConfirmationCode(userName);
            _pendingRegistrations[userName] = confirmationCode;

            return true;
        }

        public bool ConfirmUser(string userName, string code)
        {
            if (_pendingRegistrations.TryGetValue(userName, out var storedCode) && storedCode == code)
            {
                _pendingRegistrations.Remove(userName);
                return true;
            }
            return false;
        }

        private string GenerateConfirmationCode(string userName) => Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(userName + DateTime.UtcNow));

    }
}
