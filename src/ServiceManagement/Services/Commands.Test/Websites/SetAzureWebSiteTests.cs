﻿// ----------------------------------------------------------------------------------
//
// Copyright Microsoft Corporation
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// http://www.apache.org/licenses/LICENSE-2.0
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.WindowsAzure.Commands.Common;
using Microsoft.WindowsAzure.Commands.Common.Models;
using Microsoft.WindowsAzure.Commands.Common.Test.Mocks;
using Microsoft.WindowsAzure.Commands.Test.Utilities.Websites;
using Microsoft.WindowsAzure.Commands.Utilities.Websites;
using Microsoft.WindowsAzure.Commands.Utilities.Websites.Services.WebEntities;
using Microsoft.WindowsAzure.Commands.Websites;
using Moq;

namespace Microsoft.WindowsAzure.Commands.Test.Websites
{
    [TestClass]
    public class SetAzureWebsiteTests : WebsitesTestBase
    {
        [TestMethod]
        public void SetAzureWebsiteProcess()
        {
            const string websiteName = "website1";
            const string webspaceName = "webspace";
            const string suffix = "azurewebsites.com";

            // Setup
            Mock<IWebsitesClient> clientMock = new Mock<IWebsitesClient>();
            clientMock.Setup(f => f.GetWebsiteDnsSuffix()).Returns(suffix);

            bool updatedSite = false;
            bool updatedSiteConfig = false;

            clientMock.Setup(c => c.GetWebsite(websiteName, null))
                .Returns(new Site {Name = websiteName, WebSpace = webspaceName});
            clientMock.Setup(c => c.GetWebsiteConfiguration(websiteName, null))
                .Returns(new SiteConfig {NumberOfWorkers = 1});
            clientMock.Setup(c => c.UpdateWebsiteConfiguration(websiteName, It.IsAny<SiteConfig>(), null))
                .Callback((string name, SiteConfig config, string slot) =>
                    {
                        Assert.IsNotNull(config);
                        Assert.AreEqual(config.NumberOfWorkers, 3);
                        updatedSiteConfig = true;
                    }).Verifiable();

            clientMock.Setup(c => c.UpdateWebsiteHostNames(It.IsAny<Site>(), It.IsAny<IEnumerable<string>>(), null))
                .Callback((Site site, IEnumerable<string> names, string slot) =>
                    {
                        Assert.AreEqual(websiteName, site.Name);
                        Assert.IsTrue(names.Any(hostname => hostname.Equals(string.Format("{0}.{1}", websiteName, suffix))));
                        Assert.IsTrue(names.Any(hostname => hostname.Equals("stuff.com")));
                        updatedSite = true;
                    });
            clientMock.Setup(f => f.GetHostName(websiteName, null)).Returns(string.Format("{0}.{1}", websiteName, suffix));

            // Test
            SetAzureWebsiteCommand setAzureWebsiteCommand = new SetAzureWebsiteCommand
            {
                CommandRuntime = new MockCommandRuntime(),
                Name = websiteName,
                NumberOfWorkers = 3,
                WebsitesClient = clientMock.Object
            };
            AzureSession.SetCurrentContext(new AzureSubscription { Id = new Guid(base.subscriptionId) }, null, null);

            setAzureWebsiteCommand.ExecuteCmdlet();
            Assert.IsTrue(updatedSiteConfig);
            Assert.IsFalse(updatedSite);

            // Test updating site only and not configurations
            updatedSite = false;
            updatedSiteConfig = false;
            setAzureWebsiteCommand = new SetAzureWebsiteCommand
            {
                CommandRuntime = new MockCommandRuntime(),
                Name = websiteName,
                HostNames = new [] { "stuff.com" },
                WebsitesClient = clientMock.Object
            };
            AzureSession.SetCurrentContext(new AzureSubscription { Id = new Guid(base.subscriptionId) }, null, null);

            setAzureWebsiteCommand.ExecuteCmdlet();
            Assert.IsFalse(updatedSiteConfig);
            Assert.IsTrue(updatedSite);
        }

        [TestMethod]
        public void SetsWebsiteSlot()
        {
            const string websiteName = "website1";
            const string webspaceName = "webspace";
            const string suffix = "azurewebsites.com";
            const string slot = "staging";

            // Setup
            Mock<IWebsitesClient> clientMock = new Mock<IWebsitesClient>();
            clientMock.Setup(f => f.GetWebsiteDnsSuffix()).Returns(suffix);

            bool updatedSite = false;
            bool updatedSiteConfig = false;

            clientMock.Setup(c => c.GetWebsite(websiteName, slot))
                .Returns(new Site { Name = websiteName, WebSpace = webspaceName });
            clientMock.Setup(c => c.GetWebsiteConfiguration(websiteName, slot))
                .Returns(new SiteConfig { NumberOfWorkers = 1 });
            clientMock.Setup(c => c.UpdateWebsiteConfiguration(websiteName, It.IsAny<SiteConfig>(), slot))
                .Callback((string name, SiteConfig config, string slotName) =>
                {
                    Assert.IsNotNull(config);
                    Assert.AreEqual(config.NumberOfWorkers, 3);
                    updatedSiteConfig = true;
                }).Verifiable();

            clientMock.Setup(c => c.UpdateWebsiteHostNames(It.IsAny<Site>(), It.IsAny<IEnumerable<string>>(), slot))
                .Callback((Site site, IEnumerable<string> names, string slotName) =>
                {
                    Assert.AreEqual(websiteName, site.Name);
                    Assert.IsTrue(names.Any(hostname => hostname.Equals(string.Format("{0}.{1}", websiteName, suffix))));
                    Assert.IsTrue(names.Any(hostname => hostname.Equals("stuff.com")));
                    updatedSite = true;
                });
            clientMock.Setup(f => f.GetHostName(websiteName, slot)).Returns(string.Format("{0}.{1}", websiteName, suffix));

            // Test
            SetAzureWebsiteCommand setAzureWebsiteCommand = new SetAzureWebsiteCommand
            {
                CommandRuntime = new MockCommandRuntime(),
                Name = websiteName,
                NumberOfWorkers = 3,
                WebsitesClient = clientMock.Object,
                Slot = slot
            };
            AzureSession.SetCurrentContext(new AzureSubscription { Id = new Guid(base.subscriptionId) }, null, null);

            setAzureWebsiteCommand.ExecuteCmdlet();
            Assert.IsTrue(updatedSiteConfig);
            Assert.IsFalse(updatedSite);

            // Test updating site only and not configurations
            updatedSite = false;
            updatedSiteConfig = false;
            setAzureWebsiteCommand = new SetAzureWebsiteCommand
            {
                CommandRuntime = new MockCommandRuntime(),
                Name = websiteName,
                HostNames = new[] { "stuff.com" },
                WebsitesClient = clientMock.Object,
                Slot = slot
            };
            AzureSession.SetCurrentContext(new AzureSubscription { Id = new Guid(base.subscriptionId) }, null, null);

            setAzureWebsiteCommand.ExecuteCmdlet();
            Assert.IsFalse(updatedSiteConfig);
            Assert.IsTrue(updatedSite);
        }
    }
}
