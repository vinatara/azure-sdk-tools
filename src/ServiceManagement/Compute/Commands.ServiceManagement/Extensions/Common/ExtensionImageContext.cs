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
using Microsoft.WindowsAzure.Commands.Utilities.Common;

namespace Microsoft.WindowsAzure.Commands.ServiceManagement.Extensions
{
    public class ExtensionImageContext : ManagementOperationContext
    {
        public string ProviderNameSpace { get; set; }
        public string ExtensionName { get; set; }
        public string Version { get; set; }
        public string Label { get; set; }
        public string HostingResources { get; set; }
        public Uri MediaLink { get; set; }
        public string ThumbprintAlgorithm { get; set; }
        public string PublicConfigurationSchema { get; set; }
        public string PrivateConfigurationSchema { get; set; }
        public string Description { get; set; }
        public string PublisherName { get; set; }
        public DateTime? PublishedDate { get; set; }
        public bool? BlockRoleUponFailure { get; set; }
        public bool IsInternalExtension { get; set; }
        public string SampleConfig { get; set; }
        public bool? ReplicationCompleted { get; set; }
        public Uri Eula { get; set; }
        public Uri PrivacyUri { get; set; }
        public Uri HomepageUri { get; set; }
        public bool IsJsonExtension { get; set; }
        public bool DisallowMajorVersionUpgrade { get; set; }
        public string SupportedOS { get; set; }
        public string CompanyName { get; set; }
    }
}
