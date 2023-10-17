using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using GroundTemperatures.Models;
using jaytwo.FluentHttp;

namespace GroundTemperatures
{
    public class GroundTemperatureScraper
    {
        // https://ag.us.clearapis.com/v1.1/daily/soil?app_id=a2f0d7a4&app_key=742a069efe55c7015c2245032fb16bbb&location=40.7607793,-111.8910474&start=1357023600&end=1388494800

        private readonly HttpClient _httpClient;
        private readonly string _baseUrl;
        private readonly string _appId;
        private readonly string _appKey;

        public GroundTemperatureScraper(string appId, string appKey)
            : this(new HttpClient(), "https://ag.us.clearapis.com/v1.1/daily/soil", appId, appKey)
        {
        }

        public GroundTemperatureScraper(HttpClient httpClient, string baseUrl, string appId, string appKey)
        {
            _httpClient = httpClient;
            _baseUrl = baseUrl;
            _appId = appId;
            _appKey = appKey;
        }

        public async Task<FooResult> GetGroundTemperatures(string location, int year)
            => await GetGroundTemperatures(location, new DateOnly(year, 1, 1), new DateOnly(year + 1, 1, 1));

        public async Task<FooResult> GetGroundTemperatures(string location, DateOnly start, DateOnly end)
        {
            var response = await SendAsync(request => request
                .WithUriQueryParameter("location", location)
                .WithUriQueryParameter("start", GetUnixTime(start))
                .WithUriQueryParameter("end", GetUnixTime(end)));

            var result = await response.Content.ReadFromJsonAsync<FooResult>();

            if (result == null)
            {
                throw new Exception("It's Null!!");
            }

            return result;
        }

        private static int GetUnixTime(DateOnly value)
            => (int)(value.ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc) - DateTime.UnixEpoch).TotalSeconds;

        private async Task<HttpResponseMessage> SendAsync(Action<HttpRequestMessage> requestBuilder)
        {
            using var request = new HttpRequestMessage(HttpMethod.Get, _baseUrl);
            request
                .WithUriQueryParameter("app_id", _appId)
                .WithUriQueryParameter("app_key", _appKey);
            requestBuilder.Invoke(request);

            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            return response;
        }
    }
}
