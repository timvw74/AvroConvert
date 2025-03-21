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
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using FastMember;
using SolTechnology.Avro.AvroObjectServices.BuildSchema;
using SolTechnology.Avro.AvroObjectServices.Schema;

namespace SolTechnology.Avro.AvroObjectServices.Read
{
    internal partial class Resolver
    {
        private readonly Dictionary<int, Dictionary<string, Func<object>>> readStepsDictionary = new Dictionary<int, Dictionary<string, Func<object>>>();
        private readonly Dictionary<int, TypeAccessor> accessorDictionary = new Dictionary<int, TypeAccessor>();

        protected virtual object ResolveRecord(RecordSchema writerSchema, RecordSchema readerSchema, IReader dec, Type type)
        {
            object result = FormatterServices.GetUninitializedObject(type);
            var typeHash = type.GetHashCode();

            TypeAccessor accessor;
            Dictionary<string, Func<object>> readSteps;

            if (!accessorDictionary.ContainsKey(typeHash))
            {
                accessor = TypeAccessor.Create(type);
                readSteps = new Dictionary<string, Func<object>>();

                foreach (RecordField wf in writerSchema.Fields)
                {
                    if (readerSchema.TryGetField(wf.Name, out var rf))
                    {
                        string name = rf.Aliases.FirstOrDefault() ?? wf.Name;

                        var members = accessor.GetMembers();
                        var memberInfo = members.FirstOrDefault(n => n.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase));
                        if (memberInfo == null)
                        {
                            continue;
                        }

                        Func<object> func = () => Resolve(wf.TypeSchema, rf.TypeSchema, dec, memberInfo.Type) ?? wf.DefaultValue;
                        accessor[result, memberInfo.Name] = func.Invoke();

                        readSteps.Add(memberInfo.Name, func);

                    }
                    else
                        _skipper.Skip(wf.TypeSchema, dec);
                }

                readStepsDictionary.Add(typeHash, readSteps);
                accessorDictionary.Add(typeHash, accessor);
            }
            else
            {
                accessor = accessorDictionary[typeHash];
                readSteps = readStepsDictionary[typeHash];

                foreach (var readStep in readSteps)
                {
                    accessor[result, readStep.Key] = readStep.Value.Invoke();
                }
            }

            return result;
        }
    }
}
