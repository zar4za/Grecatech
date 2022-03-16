using Grecatech.Market;
using NUnit.Framework;
using RichardSzalay.MockHttp;
using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace GrecatechTests.Market
{
    [TestFixture]
    public class Buff163ClientTests
    {
        private const string MockServerUrl = "https://799dcc1f-ec84-438c-8b47-938e80f177e3.mock.pstmn.io/buff";
        private const string Session = "1-ASDFDSFDFDFGFDGFDGDFGDFGDFGDFGDFGDFGGDGDGFDG2036642521";
        private const string ItemIdPath = "idsb.json";

        private HttpClient _httpClient;

        [SetUp]
        public void Setup()
        {
            _httpClient = new HttpClient();
        }

        [Test]
        public async Task GetActiveBalanceAsync_MockedResponseContainsTwoPointTwelve_ReturnsZeroPointThree()
        {
            // Arrange
            var balanceResponse = await _httpClient.GetStringAsync($"{MockServerUrl}/api/asset/get_brief_asset/");
            var rateResponse = await _httpClient.GetStringAsync($"{MockServerUrl}/account/api/user/info/");
            var mockHandler = new MockHttpMessageHandler();
            mockHandler.Expect("https://buff.163.com/api/asset/get_brief_asset/")
                .Respond("application/json", balanceResponse);
            mockHandler.Expect("https://buff.163.com/account/api/user/info")
                .Respond("application/json", rateResponse);
            var client = new Buff163Client(new HttpClient(mockHandler), Session, ItemIdPath);

            // Act
            var result = await client.GetActiveBalanceAsync();

            // Assert
            Assert.AreEqual(0.3331m, result);
        }

        [Test]
        public async Task GetItemPriceAsync_MockedResponseContainsNovaRedQuartzPrice_ReturnsTwoPointEleven()
        {
            var marketHashName = "Nova | Red Quartz (Factory New)";

            // Arrange
            var rateResponse = await _httpClient.GetStringAsync($"{MockServerUrl}/account/api/user/info/");
            var priceResponse = await _httpClient.GetStringAsync($"{MockServerUrl}/api/market/goods/sell_order");
            var mockHandler = new MockHttpMessageHandler();
            mockHandler.Expect($"https://buff.163.com/api/market/goods/sell_order?game=csgo&goods_id=871596&page_num=1&sort_by=price.asc&mode=&allow_tradable_cooldown=0")
                .Respond("application/json", priceResponse);
            mockHandler.Expect("https://buff.163.com/account/api/user/info")
                .Respond("application/json", rateResponse);
            var bitSkinsClient = new Buff163Client(new HttpClient(mockHandler), Session, ItemIdPath);

            // Act
            var result = await bitSkinsClient.GetItemPriceAsync(marketHashName);

            // Assert
            Assert.AreEqual(2.1114m, result);
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
            var bitSkinsClient = new BitSkinsClient(new HttpClient(mockHandler), Session, ItemIdPath);

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
            var bitSkinsClient = new BitSkinsClient(new HttpClient(mockHandler), Session, ItemIdPath);

            // Act
            var result = await bitSkinsClient.SellItemAsync(assetId, price);

            // Assert
            Assert.AreEqual(tradeToken, result);
        }

        [Test]
        public async Task GetUsdConvertRateAsync_MockResponseWithUsdRate_ReturnsSameRate()
        {
            //Arrange
            var response = await _httpClient.GetStringAsync($"{MockServerUrl}/account/api/user/info/");
            var mockHandler = new MockHttpMessageHandler();
            mockHandler.Expect("https://buff.163.com/account/api/user/info")
                .Respond("application/json", response);
            var client = new Buff163Client(new HttpClient(mockHandler), Session, ItemIdPath);

            // Act
            var result = await client.GetUsdConvertRateAsync();

            // Assert
            Assert.AreEqual(0.157099318188959m, result);
        }
    }
}
