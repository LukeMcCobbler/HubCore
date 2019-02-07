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
            var stubRepository = Substitute.For<IInfoRepository>();
            stubRepository.GetInfoContext(Arg.Any<string>()).Returns(new InfoContext() { QueryLogicType = QueryLogicType.REST, QueryLogic = "someUri" });
            var stubQueryLogicProvider = Substitute.For<IQueryLogicResolver>();
            stubQueryLogicProvider.PerformQuery("someUri", Arg.Any<string[]>()).Returns("aSampleResult");
            var stubQueryLogicProviders = Substitute.For<IQueryLogicResolverFactory>();
            stubQueryLogicProviders.getQueryLogicResolver(QueryLogicType.REST).Returns(stubQueryLogicProvider);
            var target = new InfoController(stubRepository, stubQueryLogicProviders);
            //Act
            var result = target.GetInfo("someTestInfo");
            var okResult = result as OkObjectResult;
            //Assert
            Assert.IsNotNull(okResult);
            Assert.That(okResult.Value, Has.Property("Content").EqualTo("aSampleResult"));

        }
        [TestCase("a,b,c")]
        [TestCase("d,e")]
        [TestCase("f")]
        [TestCase("g,h,i,j,k,l")]
        [Test]
        public void InfoController_GetInfo_UsesParameters(string commaSeparatedParams)
        {
            //Arrange
            var stubRepository = Substitute.For<IInfoRepository>();
            stubRepository.GetInfoContext(Arg.Any<string>()).Returns(new InfoContext() { QueryLogicType = QueryLogicType.REST, QueryLogic = "someUri" });
            var mockQueryLogicProvider = Substitute.For<IQueryLogicResolver>();            
            var mockQueryLogicProviders = Substitute.For<IQueryLogicResolverFactory>();
            mockQueryLogicProviders.getQueryLogicResolver(QueryLogicType.REST).Returns(mockQueryLogicProvider);
            var target = new InfoController(stubRepository, mockQueryLogicProviders);
            var paramArray=commaSeparatedParams.Split(',');
            //Act
            var result = target.GetInfo("someTestInfo", paramArray);
            //Assert
            mockQueryLogicProvider.Received().PerformQuery(Arg.Any<string>(), Arg.Is<string[]>(prms => string.Join(",", prms) == commaSeparatedParams));
        } 
    }
}