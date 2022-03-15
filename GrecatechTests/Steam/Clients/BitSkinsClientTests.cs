﻿using Grecatech.Steam.Clients;
using NUnit.Framework;
using RichardSzalay.MockHttp;
using System.Net.Http;
using System.Threading.Tasks;

namespace GrecatechTests.Steam.Clients
{
    [TestFixture]
    public class BitSkinsClientTests
    {
        private const string MockServerUrl = "https://799dcc1f-ec84-438c-8b47-938e80f177e3.mock.pstmn.io/api/v1";
        private const string ApiKey = "08d05cea-b61f-4d47-afaf-bd92b0e01912";
        private const string TwoFactorSecret = "GT2NHO4HBSGKTU5";

        private HttpClient _httpClient;

        [SetUp]
        public void Setup()
        {
            _httpClient = new HttpClient();
        }

        [Test]
        public async Task GetActiveBalanceAsync_MockedResponseContainsTwoPointEight_ReturnsTwoPointEight()
        {
            // Arrange
            var response = await _httpClient.GetStringAsync($"{MockServerUrl}/get_account_balance/");
            var mockHandler = new MockHttpMessageHandler();
            mockHandler.Expect("https://bitskins.com/api/v1/get_account_balance/")
                .Respond("application/json", response);
            var bitSkinsClient = new BitSkinsClient(new HttpClient(mockHandler), ApiKey, TwoFactorSecret);

            // Act
            var result = await bitSkinsClient.GetActiveBalanceAsync();

            // Assert
            Assert.AreEqual(2.8m, result);
        }

        [Test]
        public async Task GetItemPriceAsync_MockedResponseContainsP250ValencePrice_ReturnsZeroPointZeroSeven()
        {
            var marketHashName = "P250 | Valence (Field-Tested)";

            // Arrange
            var response = await _httpClient.GetStringAsync($"{MockServerUrl}/get_inventory_on_sale/");
            var mockHandler = new MockHttpMessageHandler();
            mockHandler.Expect("https://bitskins.com/api/v1/get_inventory_on_sale/")
                .Respond("application/json", response);
            var bitSkinsClient = new BitSkinsClient(new HttpClient(mockHandler), ApiKey, TwoFactorSecret);

            // Act
            var result = await bitSkinsClient.GetItemPriceAsync(marketHashName);

            // Assert
            Assert.AreEqual(0.07m, result);
        }

        [Test]
        public async Task BuyItemAsync_MockedResponseContainsRightId_ReturnsTradeToken()
        {
            var tradeToken = "3a82209a1df81038";
            var marketHashName = "P250 | Valence (Field-Tested)";
            var price = 0.07m;

            // Arrange
            var priceResponse = await _httpClient.GetStringAsync($"{MockServerUrl}/get_inventory_on_sale/");
            var buyResponse = await _httpClient.GetStringAsync($"{MockServerUrl}/buy_item/success");
            var mockHandler = new MockHttpMessageHandler();
            mockHandler.Expect("https://bitskins.com/api/v1/get_inventory_on_sale/")
                .Respond("application/json", priceResponse);
            mockHandler.Expect("https://bitskins.com/api/v1/buy_item/")
                .Respond("application/json", buyResponse);
            var bitSkinsClient = new BitSkinsClient(new HttpClient(mockHandler), ApiKey, TwoFactorSecret);

            // Act
            var result = await bitSkinsClient.BuyItemAsync(marketHashName, price);

            // Assert
            Assert.AreEqual(tradeToken, result);
        }

        [Test]
        public async Task SellItemAsync_MockReponseWithIdAndPrice_ReturnsTradeToken()
        {
            var tradeToken = "0ba7f677a8cd6743";
            var assetId = "24041416664";
            var price = 3.2m;

            // Arrange
            var response = await _httpClient.GetStringAsync($"{MockServerUrl}/list_item_for_sale/");
            var mockHandler = new MockHttpMessageHandler();
            mockHandler.Expect("https://bitskins.com/api/v1/list_item_for_sale/")
                .Respond("application/json", response);
            var bitSkinsClient = new BitSkinsClient(new HttpClient(mockHandler), ApiKey, TwoFactorSecret);

            // Act
            var result = await bitSkinsClient.SellItemAsync(assetId, price);

            // Assert
            Assert.AreEqual(tradeToken, result);
        }
    }
}
