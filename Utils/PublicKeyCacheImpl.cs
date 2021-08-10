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
using EbayEventNotificationSDK.Client;
using EbayEventNotificationSDK.Exceptions;
using EbayEventNotificationSDK.Models;

namespace EbayEventNotificationSDK.Utils
{
    public class PublicKeyCacheImpl : IPublicKeyCache
    {
        private static readonly LruCacheNet.LruCache<String, PublicKey> cache = new LruCacheNet.LruCache<string, PublicKey>();

        private readonly IPublicKeyClient publicKeyClient;

        public PublicKeyCacheImpl(IPublicKeyClient publicKeyClient)
        {
            this.publicKeyClient = publicKeyClient;
        }

        public PublicKey getPublicKey(string keyId)
        { 
            PublicKey publicKey = null;
            if (cache.ContainsKey(keyId)) return cache.Get(keyId);
            try
            {
                publicKey = publicKeyClient.getPublicKey(keyId);
            } catch (Exception ex)
            {
                throw new PublicKeyCacheException(ex.Message);
            }
            cache.Add(keyId, publicKey);
            return publicKey;
        }
    }
}
