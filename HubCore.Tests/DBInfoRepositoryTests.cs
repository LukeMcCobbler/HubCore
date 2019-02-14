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
    public class DBInfoRepositoryTests
    {
        [SetUp]
        public void Setup()
        {
        }
        [Test]
        public void DBInfoRepository_GetInfoContext_QueriesDB()
        {
            //Arrange
            var stubInfoContextList = new List<InfoContext>() { new InfoContext() { }, new InfoContext() { InfoTypeName = "someInfoType", QueryLogic = "someLogic" } };
            var stubDataLayer = Substitute.For<IDataLayer>();
            stubDataLayer.Query<InfoContext>(Arg.Any<string>(), Arg.Any<IEnumerable<QueryParameter>>()).Returns(stubInfoContextList);
            var stubSettingsManager = Substitute.For<ISettingsManager>();
            var target = new DBInfoRepository(stubDataLayer, stubSettingsManager, new ApplicationInfo()
            {
                ApplicationName = "HubCoreTests"
            });
            //Act
            var result = target.GetInfoContext("someInfoType");
            //Assert
            Assert.That(result, Has.Property("InfoTypeName").EqualTo("someInfoType") & Has.Property("QueryLogic").EqualTo("someLogic"));
        }

    }
}