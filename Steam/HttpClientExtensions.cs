﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace Grecatech.Steam
{
    internal static class HttpClientExtensions
    {
        public static async Task<JsonObject> GetJsonAsync(this HttpClient client, Uri? url)
        {
            var response = await client.GetStringAsync(url);
            var json = JsonNode.Parse(response)?.AsObject();

            if (json == null)
                throw new Exception("Ошибка десериализации Json строки");

            return json;
        }
    }
}
