namespace SIG_DefesaCivil.API.Data.DTO
{

    public class RefreshTokenRequestDTO
    {
        public string RefreshToken { get; set; }
    }

    public class AuthResponseDTO
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public bool PrimeiroAcesso { get; set; }
        public string Message { get; set; }
    }
}
