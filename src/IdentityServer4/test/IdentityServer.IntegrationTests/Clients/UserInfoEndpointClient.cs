// Copyright (c) 2023 Your Company Name. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace YourNamespace
{
    public class UserInfoEndpointClient
    {
        private readonly HttpClient _httpClient;

        public UserInfoEndpointClient(HttpClient httpClient)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        }

        public async Task<UserInfoResponse> GetUserInfoAsync(string token)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "userinfo");
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            using (var response = await _httpClient.SendAsync(request))
            {
                if (!response.IsSuccessStatusCode)
                {
                    return new UserInfoResponse
                    {
                        IsError = true,
                        HttpStatusCode = response.StatusCode
                    };
                }

                var content = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<UserInfoResponse>(content);
            }
        }
    }

    public class UserInfoResponse
    {
        public bool IsError { get; set; }
        public int? HttpStatusCode { get; set; }
        public object Json { get; set; }
    }
}
