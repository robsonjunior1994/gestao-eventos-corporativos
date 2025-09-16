namespace GestaoEventosCorporativos.Api._02_Core.Interfaces.Services
{
    public interface IEncryptionPasswordService
    {
        public string EncryptPassword(string openPassword);
        public bool ValidatePassword(string openPassword, string encryptedPassword);
    }
}
