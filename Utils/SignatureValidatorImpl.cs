/*
 * Copyright (c) 2021 eBay Inc.
 *
 *  Licensed under the Apache License, Version 2.0 (the "License");
 *  you may not use this file except in compliance with the License.
 *  You may obtain a copy of the License at
 *  http://www.apache.org/licenses/LICENSE-2.0
 *
 *  Unless required by applicable law or agreed to in writing, software
 *  distributed under the License is distributed on an "AS IS" BASIS,
 *  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 *  See the License for the specific language governing permissions and
 *  limitations under the License.
 *
 */

using System;
using System.Text;
using System.Text.Json;

using EbayEventNotificationSDK.Models;
using Org.BouncyCastle.Security;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;
using EbayEventNotificationSDK.Exceptions;

namespace EbayEventNotificationSDK.Utils
{
    public  class SignatureValidatorImpl : ISignatureValidator
    {

        private readonly IPublicKeyCache publicKeyCache;

        private readonly ILogger<SignatureValidatorImpl> logger;


        public SignatureValidatorImpl(IPublicKeyCache publicKeyCache, ILogger<SignatureValidatorImpl> logger)
        {
            this.publicKeyCache = publicKeyCache;
            this.logger = logger;
        }


        public Boolean validate(Message message, String signatureHeader)
        {
            try
            {
                String jsonString = DecodeBase64(signatureHeader);
                XeBaySignature xeBaySignature = JsonSerializer.Deserialize<XeBaySignature>(jsonString);
                var publicKey = publicKeyCache.getPublicKey(xeBaySignature.kid);
                var plainTextBytes = Encoding.UTF8.GetBytes(getJSONString(message));
                var pk = PublicKeyFactory.CreateKey(Convert.FromBase64String(GetRawKey(publicKey.key)));
                var verifier = SignerUtilities.GetSigner(String.Format(Constants.Algorithm, publicKey.digest, publicKey.algorithm));
                verifier.Init(false, pk);
                verifier.BlockUpdate(plainTextBytes, 0, plainTextBytes.Length);
                var result = verifier.VerifySignature(Convert.FromBase64String(xeBaySignature.signature));
                if (result == false) logger.LogError("Signature mismatch for payload:" + getJSONString(message) + ": with signature:" + signatureHeader);
                return result;
            }
            catch (Exception ex)
            {
                throw new SignatureValidationException(ex.Message);
            }
        }

        private String getJSONString(Message message)
        {
            JsonSerializerOptions options = new JsonSerializerOptions();
            options.Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping;
            return JsonSerializer.Serialize(message, options);
        }


        private string GetRawKey(string key)
        {
            var regex = new Regex(Constants.KeyPattern);
            var match = regex.Match(key);
            if (match.Success)
            {
                return match.Groups[1].Value;
            }
            return key;
        }

        private string DecodeBase64(string value)
        {
            var valueBytes = System.Convert.FromBase64String(value);
            return Encoding.UTF8.GetString(valueBytes);
        }

    }
}
