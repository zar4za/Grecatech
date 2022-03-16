using Grecatech.Steam;
using Grecatech.Steam.Models;
using NUnit.Framework;
using RichardSzalay.MockHttp;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace GrecatechTests.Steam
{
    [TestFixture]
    public class SteamWebApiTests
    {
        private const string OriginalDomain = "https://api.steampowered.com";
        private const string MockDomain = "https://799dcc1f-ec84-438c-8b47-938e80f177e3.mock.pstmn.io/steamapi";

        private HttpClient _httpClient;

        [SetUp]
        public void SetUp()
        {
            _httpClient = new HttpClient();
        }

        [Test]
        public async Task GetTradeOffersAsync_JsonContainsSeveralOffers_ReturnsDeserializedList()
        {
            // Arrange
            var method = "/IEconService/GetTradeOffers/v1/";
            var query = "?amount=several";
            var steamClient = await CreateSteamClient(method, query);
            long unixTenMinutesBeforeNow = DateTimeOffset.Now.ToUnixTimeSeconds() - 600;

            var item1 = new Item("2", "25106006167", "1440506364", "188530139");
            var item2 = new Item("2", "24958017291", "310778198", "302028390");
            var item3 = new Item("2", "25072721052", "3574300210", "188530139");

            var tradeOffer1 = new TradeOffer("5091415114", 424517387, "BS Trade Token: 2e82550b7d8b9b49, Trade ID: 545da1092433046c", new List<Item>() { item1 }, 0);
            var tradeOffer2 = new TradeOffer("5091589530", 430824594, "BS Trade Token: a2397c9d571c0668, Trade ID: 0e6d3ebc60eafe5b", new List<Item>() { item2 }, 0);
            var tradeOffer3 = new TradeOffer("5091589100", 434521014, "BS Trade Token: 7507682d7cc96279, Trade ID: 90009eba65857306", new List<Item>() { item3 }, 0);
            var expectedResult = new List<TradeOffer>
            {
                tradeOffer1,
                tradeOffer2,
                tradeOffer3
            };

            // Act
            var result = await steamClient.GetTradeOffersAsync(unixTenMinutesBeforeNow);

            // Assert
            Assert.AreEqual(JsonSerializer.Serialize(expectedResult), JsonSerializer.Serialize(result));
        }

        [Test]
        public async Task GetTradeOffersAsync_JsonContainsOneOffer_ReturnsDeserializedList()
        {
            // Arrange
            var method = "/IEconService/GetTradeOffers/v1/";
            var query = "?amount=single";
            var steamClient = await CreateSteamClient(method, query);
            long unixTenMinutesBeforeNow = DateTimeOffset.Now.ToUnixTimeSeconds() - 600;

            var item = new Item("2", "25106006167", "1440506364", "188530139");
            var tradeOffer = new TradeOffer("5091415114", 424517387, "BS Trade Token: 2e82550b7d8b9b49, Trade ID: 545da1092433046c", new List<Item>() { item }, 0);
            var expectedResult = new List<TradeOffer> { tradeOffer };

            // Act
            var result = await steamClient.GetTradeOffersAsync(unixTenMinutesBeforeNow);

            // Assert
            Assert.AreEqual(JsonSerializer.Serialize(expectedResult), JsonSerializer.Serialize(result));
        }

        [Test]
        public async Task GetTradeOffersAsync_JsonContainsNoOffers_ReturnsEmptyList()
        {
            // Arrange
            var method = "/IEconService/GetTradeOffers/v1/";
            var query = "?amount=zero";
            var steamClient = await CreateSteamClient(method, query);
            long unixTenMinutesBeforeNow = DateTimeOffset.Now.ToUnixTimeSeconds() - 600;

            var expectedResult = new List<TradeOffer>();

            // Act
            var result = await steamClient.GetTradeOffersAsync(unixTenMinutesBeforeNow);

            // Assert
            Assert.AreEqual(JsonSerializer.Serialize(expectedResult), JsonSerializer.Serialize(result));
        }

        private async Task<SteamWebApi> CreateSteamClient(string method, string? query = null)
        {
            var mockHandler = new MockHttpMessageHandler();
            var response = await _httpClient.GetStringAsync(MockDomain + method + query ?? "");
            mockHandler.When(OriginalDomain + method).Respond("application/json", response);
            var client = new HttpClient(mockHandler);

            return new SteamWebApi(client, "6B64CFEADFD29D926F6851AB374AD451");
        }
    }
}
