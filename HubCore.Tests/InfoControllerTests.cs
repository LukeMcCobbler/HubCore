using System.Linq;
using HubCore;
using HubCore.Controllers;
using HubCore.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using NSubstitute;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace Tests
{
    public class InfoControllerTests
    {
        [SetUp]
        public void Setup()
        {
        }
        [Test]
        public void InfoController_GetInfo_UsesCorrectQueryLogicProvider()
        {
            //Arrange
            var stubLogger = Substitute.For<ILogger>();
            stubLogger.LogRequest(Arg.Any<Func<JToken>>(), Arg.Any<string>(), Arg.Any<object>()).Returns((callInfo) => callInfo.ArgAt<Func<JToken>>(0)());
            var stubRepository = Substitute.For<IInfoRepository>();
            stubRepository.GetInfoContext(Arg.Any<string>()).Returns(new InfoContext() { QueryLogicType = QueryLogicType.REST, QueryLogic = "someUri" });
            var stubQueryLogicProvider = Substitute.For<IQueryLogicResolver>();
            stubQueryLogicProvider.PerformQuery("someUri", Arg.Any<string[]>()).Returns(JToken.Parse("\"aSampleResult\""));
            var stubQueryLogicProviders = Substitute.For<IQueryLogicResolverFactory>();
            stubQueryLogicProviders.getQueryLogicResolver(QueryLogicType.REST).Returns(stubQueryLogicProvider);
            var target = new InfoController(stubRepository, stubQueryLogicProviders, stubLogger);
            //Act
            var result = target.GetInfo("someTestInfo", null);
            var okResult = result as OkObjectResult;
            //Assert
            Assert.IsNotNull(okResult);
            Assert.That(okResult.Value.ToString(), Is.EqualTo("aSampleResult"));
        }
        [TestCase("a/b/c", "a,b,c")]
        [TestCase("d/e", "d,e")]
        [TestCase("f", "f")]
        [TestCase("g/h/i/j/k/l", "g,h,i,j,k,l")]
        [TestCase("", "")]
        [TestCase(null, "")]
        [Test]
        public void InfoController_GetInfo_UsesParameters(string paramUrlSegments, string commaSeparatedParams)
        {
            //Arrange
            var stubLogger = Substitute.For<ILogger>();
            stubLogger.LogRequest(Arg.Any<Func<JToken>>(), Arg.Any<string>(), Arg.Any<object>()).Returns((callInfo) => callInfo.ArgAt<Func<JToken>>(0)());

            var stubRepository = Substitute.For<IInfoRepository>();
            stubRepository.GetInfoContext(Arg.Any<string>()).Returns(new InfoContext() { QueryLogicType = QueryLogicType.REST, QueryLogic = "someUri" });
            var mockQueryLogicProvider = Substitute.For<IQueryLogicResolver>();
            var stubQueryLogicProviders = Substitute.For<IQueryLogicResolverFactory>();
            stubQueryLogicProviders.getQueryLogicResolver(QueryLogicType.REST).Returns(mockQueryLogicProvider);
            var target = new InfoController(stubRepository, stubQueryLogicProviders, stubLogger);
            string[] paramArray = null;
            if (paramUrlSegments != null)
            {
                paramArray = paramUrlSegments.Split(',');
            }
            //Act
            var result = target.GetInfo("someTestInfo", paramUrlSegments);
            //Assert
            mockQueryLogicProvider.Received().PerformQuery(Arg.Any<string>(), Arg.Is<string[]>(prms => string.Join(",", prms) == commaSeparatedParams));
        }
        [Test]
        public void InfoController_GetInfo_LogsRequests()
        {
            //Arrange
            var mockLogger = Substitute.For<ILogger>();
            var stubRepository = Substitute.For<IInfoRepository>();
            stubRepository.GetInfoContext(Arg.Any<string>()).Returns(new InfoContext() { QueryLogicType = QueryLogicType.REST, QueryLogic = "someUri" });
            var stubQueryLogicProvider = Substitute.For<IQueryLogicResolver>();
            var stubQueryLogicProviders = Substitute.For<IQueryLogicResolverFactory>();
            stubQueryLogicProviders.getQueryLogicResolver(QueryLogicType.REST).Returns(stubQueryLogicProvider);
            var target = new InfoController(stubRepository, stubQueryLogicProviders, mockLogger);
            //Act
            var result = target.GetInfo("someTestInfo", null);
            //Assert
            mockLogger.Received().LogRequest(Arg.Any<Func<JToken>>(), "someTestInfo", Arg.Is<object>(prm => true));
        }
    }
}