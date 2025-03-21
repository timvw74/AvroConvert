﻿#region license
/**Copyright (c) 2020 Adrian Strugała
*
* Licensed under the Apache License, Version 2.0 (the "License");
* you may not use this file except in compliance with the License.
* You may obtain a copy of the License at
*
* https://www.apache.org/licenses/LICENSE-2.0
*
* Unless required by applicable law or agreed to in writing, software
* distributed under the License is distributed on an "AS IS" BASIS,
* WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
* See the License for the specific language governing permissions and
* limitations under the License.
*/
#endregion

using System;
using System.Collections.Generic;

namespace SolTechnology.Avro.AvroObjectServices.Read
{
    internal partial class Resolver
    {
        internal object ResolveString(Type type, IReader reader)
        {
            var value = reader.ReadString();

            switch (type)
            {
                case Type _ when type == typeof(string):
                    return value;
                case Type _ when type == typeof(decimal):
                    return decimal.Parse(value);
                case Type _ when type == typeof(DateTimeOffset):
                case Type _ when type == typeof(DateTimeOffset?):
                    return DateTimeOffset.Parse(value);
                case Type _ when type == typeof(Uri):
                    return new Uri(value);
            }

            return value;
        }
    }
}
