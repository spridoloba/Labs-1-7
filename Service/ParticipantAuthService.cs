namespace DSP.Service
{
    public class ParticipantAuthService
    {
        private Dictionary<string, string> _participantCredentials = new Dictionary<string, string>
{
    { "pridoloba", "123123123" },
   
    
};

        public async Task<bool> AuthenticateParticipantAsync(string login, string password)
        {
            await Task.Yield(); 

            if (_participantCredentials.TryGetValue(login, out var storedPassword))
            {
                return password == storedPassword;
            }
            return false;
        }

    }

    
}
