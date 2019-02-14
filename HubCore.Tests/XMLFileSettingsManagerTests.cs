using System.Linq;
using System.Xml.XPath;
using HubCore;
using HubCore.Controllers;
using HubCore.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using NSubstitute;
using NSubstitute.Extensions;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace Tests
{
    public class XmlFileSettingsManagerTests
    {
        private readonly string glob = "<SETTINGS><APPLICATION Name=\"HubCoreTests\"><SECTION Name=\"SomeSection\"><KEY Name=\"SomeKey\" Value=\"SomeValueFromGlobal\"></KEY><KEY Name=\"SomeOtherKey\" Value=\"SomeValueFoundOnlyInGlobal\"></KEY></SECTION></APPLICATION></SETTINGS>";
        private readonly string inst = "<SETTINGS><APPLICATION Name=\"HubCoreTests\"><SECTION Name=\"SomeSection\"><KEY Name=\"SomeKey\" Value=\"SomeValueFromInstance\"></KEY></SECTION></APPLICATION></SETTINGS>";
        private readonly string blank = "<SETTINGS/>";
        [SetUp]
        public void Setup()
        {
        }
        [Test]
        public void XMLFileSettings_SaveSetting_WritesToXMLFile()
        {
            var stubApplicationInfo = new ApplicationInfo() { GlobalSettingsFilePath = "blankGlob", InstanceSettingsFilePath = "blankInst" };
            var target = Substitute.ForPartsOf<XMLFileSettingsManager>(stubApplicationInfo);
            Predicate<string> isCorrectXml = (rawXml) =>
              {
                  var doc = XDocument.Parse(rawXml);
                  var key = doc.XPathSelectElement("SETTINGS/APPLICATION/SECTION/KEY");
                  return (key.NodeType == System.Xml.XmlNodeType.Element && (key as XElement).Attribute("Value").Value == "ANewValue");
              };
            target.Configure().ReadFileContents("blankGlob").Returns(blank);
            target.Configure().ReadFileContents("blankInst").Returns(blank);
            target.WhenForAnyArgs(x => x.WriteFileContents("", "")).DoNotCallBase();
            //Act
            target.SaveSetting("HubCoreTests", "SomeSection", "SomeKey","ANewValue",SettingSaveLocation.Global);
            //Assert
            target.Received().WriteFileContents("blankGlob", Arg.Is<string>(rawXml => isCorrectXml(rawXml)));
        }
        [Test]
        public void XMLFileSettings_GetSetting_SavesDefaultValueWhenNotFound()
        {
            var stubApplicationInfo = new ApplicationInfo() { GlobalSettingsFilePath = "blankGlob", InstanceSettingsFilePath = "blankInst" };
            var target = Substitute.ForPartsOf<XMLFileSettingsManager>(stubApplicationInfo);
            Predicate<string> isCorrectXml = (rawXml) =>
            {
                var doc = XDocument.Parse(rawXml);
                var key = doc.XPathSelectElement("SETTINGS/APPLICATION/SECTION/KEY");
                return (key.NodeType == System.Xml.XmlNodeType.Element && (key as XElement).Attribute("Value").Value == "NotFound");
            };
            target.Configure().ReadFileContents("blankGlob").Returns(blank);
            target.Configure().ReadFileContents("blankInst").Returns(blank);
            target.WhenForAnyArgs(x => x.WriteFileContents("", "")).DoNotCallBase();
            //Act
            target.GetSetting("HubCoreTests", "SomeSection", "SomeKey");
            //Assert
            target.Received().WriteFileContents("blankGlob", Arg.Is<string>(rawXml => isCorrectXml(rawXml)));
            target.Received().WriteFileContents("blankInst", Arg.Is<string>(rawXml => isCorrectXml(rawXml)));
        }
        [Test]
        public void XMLFileSettings_GetSetting_ReadsXMLFile()
        {
            //Arrange
            var stubApplicationInfo = new ApplicationInfo() { GlobalSettingsFilePath = "glob", InstanceSettingsFilePath = "inst" };
            var target = Substitute.ForPartsOf<XMLFileSettingsManager>(stubApplicationInfo);
            target.Configure().ReadFileContents("glob").Returns(glob);
            target.Configure().ReadFileContents("inst").Returns(inst);
            //Act
            var result = target.GetSetting("HubCoreTests", "SomeSection", "SomeKey");
            //Assert
            Assert.That(result, Is.EqualTo("SomeValueFromInstance"));
        }
        [Test]
        public void XMLFileSettings_GetSetting_ReadsFromGlobalWhenKeyNotFoundInInstance()
        {
            //Arrange
            var stubApplicationInfo = new ApplicationInfo() { GlobalSettingsFilePath = "glob", InstanceSettingsFilePath = "inst" };
            var target = Substitute.ForPartsOf<XMLFileSettingsManager>(stubApplicationInfo);
            target.Configure().ReadFileContents("glob").Returns(glob);
            target.Configure().ReadFileContents("inst").Returns(inst);
            //Act
            var result = target.GetSetting("HubCoreTests", "SomeSection", "SomeOtherKey");
            //Assert
            Assert.That(result, Is.EqualTo("SomeValueFoundOnlyInGlobal"));
        }
    }
}