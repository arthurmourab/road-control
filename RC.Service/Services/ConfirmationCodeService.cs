using System.Security.Cryptography;
using RC.Domain.Interfaces.Services;

namespace RC.Service.Services
{
    // Implementação TOTP (RFC 6238, HMAC-SHA1) do código de confirmação do frentista.
    // O segredo é armazenado como string hexadecimal (20 bytes / 160 bits aleatórios).
    public class ConfirmationCodeService : IConfirmationCodeService
    {
        private const int Digits = 6;                       // código de 6 dígitos numéricos
        private const int Period = 60;                      // janela de 60 segundos
        private const int SecretBytes = 20;                 // 160 bits, tamanho recomendado p/ HMAC-SHA1

        public int PeriodSeconds => Period;

        public string GenerateSecret()
        {
            // Segredo aleatório em hexadecimal — nunca é exposto pela API.
            return Convert.ToHexString(RandomNumberGenerator.GetBytes(SecretBytes));
        }

        public string GetCode(string secret, DateTime instantUtc)
        {
            var counter = ToCounter(instantUtc);
            return Derive(secret, counter);
        }

        public bool IsValid(string secret, string code, DateTime instantUtc)
        {
            if (string.IsNullOrWhiteSpace(secret) || string.IsNullOrWhiteSpace(code))
                return false;

            var counter = ToCounter(instantUtc);

            // Janela atual + anterior: cobre o código virar entre a leitura pelo frentista
            // e o envio pelo motorista/gestor. Comparação em tempo constante.
            for (var offset = 0; offset <= 1; offset++)
            {
                var expected = Derive(secret, counter - offset);
                if (CryptographicOperations.FixedTimeEquals(
                        System.Text.Encoding.ASCII.GetBytes(expected),
                        System.Text.Encoding.ASCII.GetBytes(code)))
                    return true;
            }

            return false;
        }

        // Número da janela TOTP: floor(unixSeconds / period)
        private static long ToCounter(DateTime instantUtc)
        {
            var unixSeconds = (long)(instantUtc.ToUniversalTime() - DateTime.UnixEpoch).TotalSeconds;
            return unixSeconds / Period;
        }

        // Truncamento dinâmico padrão TOTP sobre HMAC-SHA1(secret, counter big-endian)
        private static string Derive(string secretHex, long counter)
        {
            var key = Convert.FromHexString(secretHex);

            var counterBytes = BitConverter.GetBytes(counter);
            if (BitConverter.IsLittleEndian)
                Array.Reverse(counterBytes);

            var hash = HMACSHA1.HashData(key, counterBytes);

            var offset = hash[^1] & 0x0F;
            var binary =
                ((hash[offset] & 0x7F) << 24) |
                ((hash[offset + 1] & 0xFF) << 16) |
                ((hash[offset + 2] & 0xFF) << 8) |
                (hash[offset + 3] & 0xFF);

            var otp = binary % (int)Math.Pow(10, Digits);
            return otp.ToString().PadLeft(Digits, '0');
        }
    }
}
