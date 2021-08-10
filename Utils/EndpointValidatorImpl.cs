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
using System.Security.Cryptography;
using System.Text;
using EbayEventNotificationSDK.Exceptions;
using EbayEventNotificationSDK.Models;

namespace EbayEventNotificationSDK.Utils
{
    public class EndpointValidatorImpl : IEndPointValidator
    {
        private readonly IConfig config;

        public EndpointValidatorImpl(IConfig configuration)
        {
            this.config = configuration;
        }

        public ChallengeResponse generateChallengeResponse(string challengeCode)
        {
            if (string.IsNullOrEmpty(config.endpoint) || string.IsNullOrEmpty(config.verificationToken))
            {
                throw new MissingEndpointValidationConfig("Endpoint and verificationToken is required");
            }
            try
            {
                IncrementalHash sha256 = IncrementalHash.CreateHash(HashAlgorithmName.SHA256);
                sha256.AppendData(Encoding.UTF8.GetBytes(challengeCode));
                sha256.AppendData(Encoding.UTF8.GetBytes(config.verificationToken));
                sha256.AppendData(Encoding.UTF8.GetBytes(config.endpoint));
                byte[] bytes = sha256.GetHashAndReset();
                return new ChallengeResponse(BitConverter.ToString(bytes).Replace("-", string.Empty).ToLower());
            }
            catch (Exception ex)
            {
                throw new EndpointValidationException("End point validation failed", ex);
            }
        }
    }
}
