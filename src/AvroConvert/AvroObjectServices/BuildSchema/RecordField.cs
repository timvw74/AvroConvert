﻿// Copyright (c) Microsoft Corporation
// All rights reserved.
// 
// Licensed under the Apache License, Version 2.0 (the "License"); you may not
// use this file except in compliance with the License.  You may obtain a copy
// of the License at http://www.apache.org/licenses/LICENSE-2.0
// 
// THIS CODE IS PROVIDED *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
// KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED
// WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE,
// MERCHANTABLITY OR NON-INFRINGEMENT.
// 
// See the Apache Version 2.0 License for specific language governing
// permissions and limitations under the License.

/** Modifications copyright(C) 2020 Adrian Strugała **/

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reflection;
using Newtonsoft.Json;
using SolTechnology.Avro.AvroObjectServices.Schema.Abstract;

namespace SolTechnology.Avro.AvroObjectServices.BuildSchema
{
    /// <summary>
    ///  Sort order.
    /// </summary>
    internal enum SortOrder
    {
        /// <summary>
        /// The ascending order.
        /// </summary>
        Ascending,

        /// <summary>
        /// The descending order.
        /// </summary>
        Descending,

        /// <summary>
        /// The order is ignored.
        /// </summary>
        Ignore
    }

    /// <summary>
    ///     Class representing a field of the record.
    ///     For more information please see <a href="http://avro.apache.org/docs/current/spec.html#schema_record">the specification</a>.
    /// </summary>
    internal sealed class RecordField : Schema
    {
        private readonly NamedEntityAttributes namedEntityAttributes;
        private readonly TypeSchema typeSchema;
        private readonly SortOrder order;
        private readonly bool hasDefaultValue;
        private readonly object defaultValue;
        private readonly int position;
        private MemberInfo info;

        /// <summary>
        /// Initializes a new instance of the <see cref="RecordField" /> class.
        /// </summary>
        /// <param name="namedEntityAttributes">The named entity attributes.</param>
        /// <param name="typeSchema">The type schema.</param>
        /// <param name="order">The order.</param>
        /// <param name="hasDefaultValue">Whether the field has a default value or not.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <param name="info">The info.</param>
        /// <param name="position">The position of the field in the schema.</param>
        internal RecordField(
            NamedEntityAttributes namedEntityAttributes,
            TypeSchema typeSchema,
            SortOrder order,
            bool hasDefaultValue,
            object defaultValue,
            MemberInfo info,
            int position)
            : this(namedEntityAttributes, typeSchema, order, hasDefaultValue, defaultValue, info, position, new Dictionary<string, string>())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RecordField" /> class.
        /// </summary>
        /// <param name="namedEntityAttributes">The named entity attributes.</param>
        /// <param name="typeSchema">The type schema.</param>
        /// <param name="order">The order.</param>
        /// <param name="hasDefaultValue">Whether the field has a default value or not.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <param name="info">The info.</param>
        /// <param name="position">The position of the field in the schema.</param>
        /// <param name="attributes">The attributes.</param>
        internal RecordField(
            NamedEntityAttributes namedEntityAttributes,
            TypeSchema typeSchema,
            SortOrder order,
            bool hasDefaultValue,
            object defaultValue,
            MemberInfo info,
            int position,
            Dictionary<string, string> attributes)
            : base(attributes)
        {
            this.namedEntityAttributes = namedEntityAttributes;
            this.typeSchema = typeSchema;
            this.order = order;
            this.hasDefaultValue = hasDefaultValue;
            this.defaultValue = defaultValue;
            this.info = info;
            this.position = position;

            this.ShouldBeSkipped = false;
            this.UseDefaultValue = false;
        }

        /// <summary>
        ///     Gets the full name.
        /// </summary>
        internal string FullName
        {
            get { return this.namedEntityAttributes.Name.FullName; }
        }

        /// <summary>
        ///     Gets the name.
        /// </summary>
        internal string Name
        {
            get { return this.namedEntityAttributes.Name.Name; }
        }

        /// <summary>
        ///     Gets the namespace.
        /// </summary>
        internal string Namespace
        {
            get { return this.namedEntityAttributes.Name.Namespace; }
        }

        /// <summary>
        ///     Gets the aliases.
        /// </summary>
        internal ReadOnlyCollection<string> Aliases
        {
            get { return this.namedEntityAttributes.Aliases.AsReadOnly(); }
        }

        /// <summary>
        ///     Gets the doc.
        /// </summary>
        internal string Doc
        {
            get { return this.namedEntityAttributes.Doc; }
        }

        /// <summary>
        ///     Gets the type schema.
        /// </summary>
        internal TypeSchema TypeSchema
        {
            get { return this.typeSchema; }
        }

        /// <summary>
        ///     Gets the sort order.
        /// </summary>
        internal SortOrder Order
        {
            get { return this.order; }
        }

        /// <summary>
        ///     Gets a value indicating whether the field has a default value or not.
        /// </summary>
        internal bool HasDefaultValue
        {
            get { return this.hasDefaultValue; }
        }

        /// <summary>
        ///     Gets the default value.
        /// </summary>
        internal object DefaultValue
        {
            get { return this.defaultValue; }
        }

        /// <summary>
        /// Gets the position.
        /// </summary>
        internal int Position
        {
            get { return this.position; }
        }

        /// <summary>
        /// Gets or sets the member info.
        /// </summary>
        internal MemberInfo MemberInfo
        {
            get { return this.info; }
            set { this.info = value; }
        }

        internal NamedEntityAttributes NamedEntityAttributes
        {
            get { return this.namedEntityAttributes; }
        }

        /// <summary>
        ///     Converts current not to JSON according to the avro specification.
        /// </summary>
        /// <param name="writer">The writer.</param>
        /// <param name="seenSchemas">The seen schemas.</param>
        internal override void ToJsonSafe(JsonTextWriter writer, HashSet<NamedSchema> seenSchemas)
        {
            writer.WriteStartObject();
            writer.WriteProperty("name", Name);
            writer.WriteOptionalProperty("namespace", Namespace);
            writer.WriteOptionalProperty("doc", this.Doc);
            writer.WriteOptionalProperty("aliases", this.Aliases);
            writer.WritePropertyName("type");
            this.TypeSchema.ToJson(writer, seenSchemas);
            writer.WriteOptionalProperty("default", this.defaultValue, this.hasDefaultValue);

            writer.WriteEndObject();
        }

        /// <summary>
        /// Gets or sets a value indicating whether the field should be skipped or not.
        /// </summary>
        internal bool ShouldBeSkipped
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether use default value should be used.
        /// </summary>
        /// <value>
        ///   <c>true</c> if [use default value]; otherwise, <c>false</c>.
        /// </value>
        internal bool UseDefaultValue
        {
            get; set;
        }
    }
}
