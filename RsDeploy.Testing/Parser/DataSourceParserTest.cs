﻿using NUnit.Framework;
using Moq;
using RsDeploy.Execution;
using RsDeploy.Parser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RsDeploy.Parser.Xml;
using System.IO;
using System.Reflection;
using System.Xml;

namespace RsDeploy.Testing.Parser
{
    [TestFixture]
    public class DataSourceParserTest
    {
        [Test]
        public void ParseDataSourceNode()
        {
            var mock = new Mock<DataSourceService>();
            mock.Setup(s => s.Create(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Verifiable();
            var service = mock.Object;

            var parser = new DataSourceParser(service);

            var xmlDoc = new XmlDocument();
            using (Stream stream = Assembly.GetExecutingAssembly()
                        .GetManifestResourceStream("RsDeploy.Testing.Resources.BasicSample.xml"))
            using (StreamReader reader = new StreamReader(stream))
                xmlDoc.Load(reader);

            var root = xmlDoc.FirstChild.NextSibling;
            parser.Root = new ProjectParser();
            parser.Execute(root);

            Mock.Get(service).Verify(s => s.Create(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        }

        [Test]
        public void ParseDataSourceNodeAndForwardToRoot()
        {
            var stub = new Mock<DataSourceService>();
            stub.Setup(s => s.Create(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Verifiable();
            var service = stub.Object;

            var parser = new DataSourceParser(service);

            var xmlDoc = new XmlDocument();
            using (Stream stream = Assembly.GetExecutingAssembly()
                        .GetManifestResourceStream("RsDeploy.Testing.Resources.BasicSample.xml"))
            using (StreamReader reader = new StreamReader(stream))
                xmlDoc.Load(reader);

            var root = xmlDoc.FirstChild.NextSibling;
            parser.Root = new ProjectParser();
            parser.Execute(root);

            Assert.That(parser.Root.DataSources.ContainsKey("MyDataSource"), Is.True);
            Assert.That(parser.Root.DataSources["MyDataSource"], Is.EqualTo("/MyDataSource"));
        }

    }
}
