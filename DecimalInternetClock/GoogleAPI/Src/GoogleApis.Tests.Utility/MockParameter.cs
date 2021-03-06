﻿/*
Copyright 2010 Google Inc

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

    http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
*/

using System.Collections.Generic;
using Google.Apis.Discovery;

namespace Google.Apis.Tests.Apis.Requests
{
    /// <summary>
    /// Mock class for testing purposes
    /// </summary>
    public class MockParameter : IParameter
    {
        #region IParameter Members

        public string Name { get; set; }
        public string ParameterType { get; set; }
        public string Pattern { get; set; }
        public bool IsRequired { get; set; }
        public bool IsRepeatable { get; set; }
        public bool Required { get; set; }
        public string DefaultValue { get; set; }
        public string ValueType { get; set; }
        public string Description { get; set; }
        public string Maximum { get; set; }
        public string Minimum { get; set; }
        public IEnumerable<string> EnumValues { get; set; }
        public IEnumerable<string> EnumValueDescriptions { get; set; }
        public IEnumerable<string> Enum { get; set; }

        #endregion
    }
}