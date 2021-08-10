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
using System.Collections.Generic;

namespace EbayEventNotificationSDK.Client
{
    public class ClientConstants
    {
        private static readonly Dictionary<string, string> EndPoints = new Dictionary<string, string> {
        { Constants.Production, "https://api.ebay.com/commerce/notification/v1/public_key/" },
        { Constants.Sandbox, "https://api.sandbox.ebay.com/commerce/notification/v1/public_key/" }
     };

        public static readonly string Bearer = "Bearer ";
        public static readonly string Authorization = "Authorization";
        public static readonly IList<string> scopes = new List<string> { "https://api.ebay.com/oauth/api_scope" };
        

        public static string GetEndPoints(string key)
        {
            string url;
            EndPoints.TryGetValue(key, out url);
            return url;
        }
    }
}
