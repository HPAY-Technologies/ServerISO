using System.Security.Cryptography;
using System.Text;

namespace Iso8583
{
    public static class IsoMac
    {
        public static string Calculate(IsoMessage message)
        {
            var data = new StringBuilder();

            data.Append(message.MTI);

            foreach (var field in message.GetFields())
            {
                data.Append(field.Key);
                data.Append(field.Value);
            }

            using var sha256 = SHA256.Create();
            byte[] hash = sha256.ComputeHash(Encoding.ASCII.GetBytes(data.ToString()));

            return BitConverter.ToString(hash, 0, 8).Replace("-", "");
        }
    }
}
