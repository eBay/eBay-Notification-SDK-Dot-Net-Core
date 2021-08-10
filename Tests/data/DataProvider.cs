using System;
using System.IO;
using System.Text.Json;
using EbayEventNotificationSDK.Models;

namespace EbayEventNotificationSDK.Tests
{
    public class DataProvider
    {

        private static readonly String Message = @"../../../data/message.json";
        private static readonly String TamperedMessage = @"../../../data/tampered_message.json";
        private static readonly String PublicKeyResponse = @"../../../data/public_key_response.json";
        private static readonly String SignatureHeader = "eyJhbGciOiJlY2RzYSIsImtpZCI6Ijk5MzYyNjFhLTdkN2ItNDYyMS1hMGYxLTk2Y2NiNDI4YWY0OSIsInNpZ25hdHVyZSI6Ik1FWUNJUUNmeGZJV3V4bVdjSUJRSjljNS9YN2lHREpxczJSQ0dzQkVhQWppbnlycmZBSWhBSVY2d0djVGlCdVY1S0pVaWYyaG9reXJMK1E5c3NIa2FkK214Mm5FRTI1dyIsImRpZ2VzdCI6IlNIQTEifQ==";


        public static Message getMockMessage()
        {
            string message = File.ReadAllText(Message);
            return JsonSerializer.Deserialize<Message>(message);
        }

        public static Message getMockTamperedMessage()
        {
            string message = File.ReadAllText(TamperedMessage);
            return JsonSerializer.Deserialize<Message>(message);
        }

        public static PublicKey getMockPublicKeyResponse()
        {
            string response = File.ReadAllText(PublicKeyResponse);
            return JsonSerializer.Deserialize<PublicKey>(response);
        }

        public static String getMockXEbaySignatureHeader()
        {
            return SignatureHeader;
        }
    }
}
