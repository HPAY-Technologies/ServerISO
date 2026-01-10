using System;
using System.Collections.Generic;

namespace Iso8583
{
    public static class IsoResponseGenerator
    {
        private static readonly Random Random = new Random();

        public static string GenerateResponse(IsoMessage request)
        {
            string responseMti = GetResponseMti(request.MTI);
            var response = new IsoMessage(responseMti);

            CopyIfExists(request, response, 2);   // PAN
            CopyIfExists(request, response, 3);   // Processing Code
            CopyIfExists(request, response, 4);   // Amount
            CopyIfExists(request, response, 7);   // Transmission DateTime
            CopyIfExists(request, response, 11);  // STAN
            CopyIfExists(request, response, 41);  // Terminal ID
            CopyIfExists(request, response, 49);  // Currency

            // 🧠 Champs générés
            response.SetField(12, DateTime.Now.ToString("HHmmss")); // Local Time
            response.SetField(13, DateTime.Now.ToString("MMdd"));   // Local Date
            response.SetField(37, GenerateRRN());                   // RRN
            response.SetField(39, GenerateResponseCode());          // Response Code

            string mac = IsoMac.Calculate(response);
            response.SetField(64, mac);

            return IsoPacker.Pack(response);
        }

        private static string GetResponseMti(string requestMti)
        {
            return requestMti switch
            {
                "0200" => "0210",
                "1200" => "1210",
                "1800" => "1814",
                "1300" => "1314",
                _ => throw new Exception("MTI non supporté")
            };
        }

        private static void CopyIfExists(IsoMessage from, IsoMessage to, int field)
        {
            if (from.HasField(field))
                to.SetField(field, from.GetField(field));
        }

        private static string GenerateResponseCode()
        {
            string[] codes =
            {
                "00", // Approved
                "05", // Do not honor
                "12", // Invalid transaction
                "13", // Invalid amount
                "14", // Invalid PAN
                "51", // Insufficient funds
                "91"  // Issuer unavailable
            };

            return codes[Random.Next(codes.Length)];
        }

        private static string GenerateRRN()
        {
            return DateTime.Now.ToString("yyMMddHHmmss");
        }
    }
}
