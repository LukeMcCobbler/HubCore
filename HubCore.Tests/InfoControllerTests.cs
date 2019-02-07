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
        public void InfoController_GetInfo_ThrowsExceptionWhenInfoContextHasInvalidQueryLogicType()
        {
            //Arrange
            var stubRepository = Substitute.For<IInfoRepository>();
            stubRepository.GetInfoContext(Arg.Any<string>()).Returns(new InfoContext() { QueryLogicType = QueryLogicType.DBQuery });
            var stubQueryLogicProviders = new Dictionary<QueryLogicType, IQueryLogicResolver>() { { QueryLogicType.REST, Substitute.For<IQueryLogicResolver>() } };
            var target = new InfoController(stubRepository, stubQueryLogicProviders);
            //Act
            TestDelegate result = () => target.GetInfo("someTestInfo");
            //Assert
            Assert.That(result, Throws.Exception.With.Property("Message").EqualTo("Cannot find a way to resolve DBQuery") );
        }
        [Test]
        public void InfoController_GetInfo_UsesCorrectQueryLogicProvider()
        {
            //Arrange
            var stubRepository = Substitute.For<IInfoRepository>();
            stubRepository.GetInfoContext(Arg.Any<string>()).Returns(new InfoContext() { QueryLogicType = QueryLogicType.REST, QueryLogic = "someUri" });
            var stubQueryLogicProvider = Substitute.For<IQueryLogicResolver>();
            stubQueryLogicProvider.PerformQuery("someUri", Arg.Any<string[]>()).Returns("aSampleResult");
            var stubQueryLogicProviders = new Dictionary<QueryLogicType, IQueryLogicResolver>() { { QueryLogicType.REST,stubQueryLogicProvider  } };
            var target = new InfoController(stubRepository, stubQueryLogicProviders);
            //Act
            var result = target.GetInfo("someTestInfo");
            var okResult = result as OkObjectResult;
            //Assert
            Assert.IsNotNull(okResult);
            Assert.That(okResult.Value, Has.Property("Content").EqualTo("aSampleResult"));

        }
        [Test]
        public void InfoController_GetInfo_UsesParameters()
        {
            //Arrange
            var stubRepository = Substitute.For<IInfoRepository>();
            stubRepository.GetInfoContext(Arg.Any<string>()).Returns(new InfoContext() { QueryLogicType = QueryLogicType.REST, QueryLogic = "someUri" });
            var mockQueryLogicProvider = Substitute.For<IQueryLogicResolver>();            
            var mockQueryLogicProviders = new Dictionary<QueryLogicType, IQueryLogicResolver>() { { QueryLogicType.REST, mockQueryLogicProvider } };            
            var target = new InfoController(stubRepository, mockQueryLogicProviders);
            //Act
            var result = target.GetInfo("someTestInfo", "a", "b", "c");
            //Assert
            mockQueryLogicProvider.Received().PerformQuery(Arg.Any<string>(), Arg.Is<string[]>(prms => string.Join(",", prms) == "a,b,c"));
        }
    }
}