using Grecatech.Steam;
using NUnit.Framework;
using RichardSzalay.MockHttp;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace GrecatechTests.Steam
{
    [TestFixture]
    public class SteamWebClientTests
    {
        HttpClient _httpClient;

        [SetUp]
        public void Setup()
        {
            _httpClient = new HttpClient();
        }

        //[Test]
        //public async Task AuthorizeAsync_StateUnderTest_ExpectedBehavior()
        //{
        //    // Arrange
        //    var steamWebClient = new SteamWebClient(TODO, TODO, TODO);
        //    string username = null;
        //    string password = null;
        //    string apiKey = null;

        //    // Act
        //    var result = await steamWebClient.AuthorizeAsync(
        //        username,
        //        password,
        //        apiKey);

        //    // Assert
        //    Assert.Fail();
        //}

        //[Test]
        //public async Task SearchForTradeOffer_StateUnderTest_ExpectedBehavior()
        //{
        //    var tradeToken = "2e82550b7d8b9b49";
        //    string tradeOfferId = "5091415114";

        //    var response = await _httpClient.GetStringAsync("https://799dcc1f-ec84-438c-8b47-938e80f177e3.mock.pstmn.io/steamapi/IEconService/GetTradeOffers/v1/");
        //    var mockHandler = new MockHttpMessageHandler();
        //    mockHandler.Expect("https://api.steampowered.com/IEconService/GetTradeOffers/v1/").Respond("application/json", response);

        //    // Arrange
        //    var steamWebClient = new SteamWebClient(new HttpClient(mockHandler), "5199510452:AAEoGg1KHs3KEW0ddiHiZ6ARAxqJvg3PCes", 348653040);
            

        //    // Act
        //    string result = await steamWebClient.SearchForTradeOffer(tradeToken);

        //    // Assert
        //    Assert.AreEqual(tradeOfferId, result);
        //}
    }
}
