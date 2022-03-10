using Grecatech.Steam.Clients;
using NUnit.Framework;
using RichardSzalay.MockHttp;
using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace GrecatechTests.Steam.Clients
{
    [TestFixture]
    public class Buff163ClientTests
    {
        [TestCase(0.00, ExpectedResult = 0.0000)]
        [TestCase(0.10, ExpectedResult = 0.0158)]
        [TestCase(2.12, ExpectedResult = 0.3355)]
        [TestCase(1500.00, ExpectedResult = 237.3906)]
        [TestCase(100000.00, ExpectedResult = 15826.0402)]
        public async Task<decimal> GetActiveBalanceAsync_InitializedWithMockedHttpClient_ReturnRightAmountInUsd(decimal cny)
        {
            // Arrange
            var session = "";
            var mockResponse = File.ReadAllText("MockResponses/Mock-Buff163Client-GetActiveBalanceAsync.json").Replace("CNY", cny.ToString().Replace(',', '.'));
            var mockCurrency = File.ReadAllText("MockResponses/Mock-Buff163Client-GetUsdConvertRateAsync.json");
            var mockHandler = new MockHttpMessageHandler();
            mockHandler.When("https://buff.163.com/api/asset/get_brief_asset/*").Respond("application/json", mockResponse);
            mockHandler.When("https://buff.163.com/account/api/user/info").Respond("application/json", mockCurrency);
            var httpClient = new HttpClient(mockHandler);
            var buff163Client = new Buff163Client(httpClient, session, "idsb.json");

            //Act
            var balance = await buff163Client.GetActiveBalanceAsync();

            return balance;
        }

        [Test]
        public async Task GetItemPriceAsync_StateUnderTest_ExpectedBehavior()
        {
            throw new NotImplementedException();
        }

        [Test]
        public async Task BuyItemAsync_StateUnderTest_ExpectedBehavior()
        {
            throw new NotImplementedException();
        }

        [Test]
        public async Task SellItemAsync_StateUnderTest_ExpectedBehavior()
        {
            throw new NotImplementedException();
        }

        [Test]
        public async Task GetUsdConvertRateAsync_StateUnderTest_ExpectedBehavior()
        {
            throw new NotImplementedException();
        }
    }
}
