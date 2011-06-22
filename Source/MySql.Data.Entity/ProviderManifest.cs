
// Copyright (C) 2008-2009 Sun Microsystems, Inc.
//
// MySQL Connector/NET is licensed under the terms of the GPLv2
// <http://www.gnu.org/licenses/old-licenses/gpl-2.0.html>, like most 
// MySQL Connectors. There are special exceptions to the terms and 
// conditions of the GPLv2 as it is applied to this software, see the 
// FLOSS License Exception
// <http://www.mysql.com/about/legal/licensing/foss-exception.html>.
//
// This program is free software; you can redistribute it and/or modify 
// it under the terms of the GNU General Public License as published 
// by the Free Software Foundation; version 2 of the License.
//
// This program is distributed in the hope that it will be useful, but 
// WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY 
// or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License 
// for more details.
//
// You should have received a copy of the GNU General Public License along 
// with this program; if not, write to the Free Software Foundation, Inc., 
// 51 Franklin St, Fifth Floor, Boston, MA 02110-1301  USA

using System;
using System.IO;
using System.Reflection;
using System.Data;
using System.Xml;
using System.Data.Common;
using System.Data.Metadata.Edm;
using System.Diagnostics;
using MySql.Data.Entity.Properties;

namespace MySql.Data.MySqlClient
{
    internal class MySqlProviderManifest : DbXmlEnabledProviderManifest
    {
        string manifestToken;

        public MySqlProviderManifest(string version)
            : base(GetManifest())
        {
            manifestToken = version;
        }

        private static XmlReader GetManifest()
        {
            return GetXmlResource("MySql.Data.Entity.Properties.ProviderManifest.xml");
        }

        protected override XmlReader GetDbInformation(string informationType)
        {
            if (informationType == DbProviderManifest.StoreSchemaDefinition)
            {
                return GetStoreSchemaDescription();
            }

            if (informationType == DbProviderManifest.StoreSchemaMapping)
            {
                return GetStoreSchemaMapping();
            }

            throw new ProviderIncompatibleException(String.Format("The provider returned null for the informationType '{0}'.", informationType));
        }

        private XmlReader GetStoreSchemaMapping()
        {
            return GetMappingResource("SchemaMapping.msl");
        }

        private XmlReader GetStoreSchemaDescription()
        {
            double version = double.Parse(manifestToken);

            if (version < 5.0) throw new NotSupportedException("Your version of MySQL is not currently supported");
            if (version < 5.1) return GetMappingResource("SchemaDefinition-5.0.ssdl");
            if (version < 5.5) return GetMappingResource("SchemaDefinition-5.1.ssdl");
            return GetMappingResource("SchemaDefinition-5.5.ssdl");     
        }

        public override TypeUsage GetEdmType(TypeUsage storeType)
        {
            if (storeType == null)
            {
                throw new ArgumentNullException("storeType");
            }

            string storeTypeName = storeType.EdmType.Name.ToLowerInvariant();

            if (!base.StoreTypeNameToEdmPrimitiveType.ContainsKey(storeTypeName))
            {
                throw new ArgumentException(String.Format("The underlying provider does not support the type '{0}'.", storeTypeName));
            }

            PrimitiveType edmPrimitiveType = base.StoreTypeNameToEdmPrimitiveType[storeTypeName];
            return TypeUsage.CreateDefaultTypeUsage(edmPrimitiveType);
        }

        private const int CHAR_MAXLEN = 255;
        private const int VARCHAR_MAXLEN = 65535;
        private const int MEDIUMTEXT_MAXLEN = 16777215;
        private const int LONGTEXT_MAXLEN = 1073741823;

        private const int BINARY_MAXLEN = 255;
        private const int VARBINARY_MAXLEN = 65535;
        private const int MEDIUMBLOB_MAXLEN = 16777215;
        private const int LONGBLOB_MAXLEN = 2147483647;

        public override TypeUsage GetStoreType(TypeUsage edmType)
        {
            if (edmType == null)
                throw new ArgumentNullException("edmType");

            Debug.Assert(edmType.EdmType.BuiltInTypeKind == BuiltInTypeKind.PrimitiveType);

            PrimitiveType primitiveType = edmType.EdmType as PrimitiveType;
            if (primitiveType == null)
                throw new ArgumentException(String.Format(Resources.TypeNotSupported, edmType));

            ReadOnlyMetadataCollection<Facet> facets = edmType.Facets;

            switch (primitiveType.PrimitiveTypeKind)
            {
                case PrimitiveTypeKind.Boolean:
                    return TypeUsage.CreateDefaultTypeUsage(StoreTypeNameToStorePrimitiveType["bool"]);

                case PrimitiveTypeKind.Byte:
                    return TypeUsage.CreateDefaultTypeUsage(StoreTypeNameToStorePrimitiveType["tinyint"]);

                case PrimitiveTypeKind.Int16:
                    return TypeUsage.CreateDefaultTypeUsage(StoreTypeNameToStorePrimitiveType["smallint"]);

                case PrimitiveTypeKind.Int32:
                    return TypeUsage.CreateDefaultTypeUsage(StoreTypeNameToStorePrimitiveType["int"]);

                case PrimitiveTypeKind.Int64:
                    return TypeUsage.CreateDefaultTypeUsage(StoreTypeNameToStorePrimitiveType["bigint"]);

                case PrimitiveTypeKind.Guid:
                    return TypeUsage.CreateDefaultTypeUsage(StoreTypeNameToStorePrimitiveType["guid"]);

                case PrimitiveTypeKind.Double:
                    return TypeUsage.CreateDefaultTypeUsage(StoreTypeNameToStorePrimitiveType["double"]);

                case PrimitiveTypeKind.Single:
                    return TypeUsage.CreateDefaultTypeUsage(StoreTypeNameToStorePrimitiveType["float"]);

                case PrimitiveTypeKind.Decimal:
                    {
                        byte precision = 10;
                        byte scale = 0;
                        Facet facet;

                        if (edmType.Facets.TryGetValue("Precision", false, out facet))
                        {
                            if (!facet.IsUnbounded && facet.Value != null)
                                precision = (byte)facet.Value;
                        }

                        if (edmType.Facets.TryGetValue("Scale", false, out facet))
                        {
                            if (!facet.IsUnbounded && facet.Value != null)
                                scale = (byte)facet.Value;
                        }

                        return TypeUsage.CreateDecimalTypeUsage(StoreTypeNameToStorePrimitiveType["decimal"], precision, scale);
                    }

                case PrimitiveTypeKind.Binary: 
                    {
                        bool isFixedLength = null != facets["FixedLength"].Value && (bool)facets["FixedLength"].Value;
                        Facet f = facets["MaxLength"];
                        bool isMaxLength = f.IsUnbounded || null == f.Value || (int)f.Value > MEDIUMBLOB_MAXLEN;
                        int maxLength = !isMaxLength ? (int)f.Value : LONGBLOB_MAXLEN;

                        string typeName = String.Empty;
                        if (isFixedLength)
                        {
                            if (maxLength < CHAR_MAXLEN) typeName = "tinyblob";
                            else if (maxLength < MEDIUMBLOB_MAXLEN) typeName = "blob";
                            else if (maxLength < LONGTEXT_MAXLEN) typeName = "mediumblob";
                            else typeName = "longblob";
                        }
                        else
                        {
                            typeName = isMaxLength || maxLength > BINARY_MAXLEN ? "varbinary" : "binary";
                            maxLength = isMaxLength ? VARBINARY_MAXLEN : maxLength;
                        }

                        return TypeUsage.CreateBinaryTypeUsage(StoreTypeNameToStorePrimitiveType[typeName], isFixedLength, maxLength);
                    }

                case PrimitiveTypeKind.String:
                    {
                        bool isUnicode = null == facets["Unicode"].Value || (bool)facets["Unicode"].Value;
                        bool isFixedLength = null != facets["FixedLength"].Value && (bool)facets["FixedLength"].Value;
                        Facet f = facets["MaxLength"];
                        // maxlen is true if facet value is unbounded, the value is bigger than the limited string sizes *or* the facet
                        // value is null. this is needed since functions still have maxlength facet value as null
                        bool isMaxLength = f.IsUnbounded || null == f.Value || (int)f.Value > MEDIUMTEXT_MAXLEN;
                        int maxLength = !isMaxLength ? (int)f.Value : LONGTEXT_MAXLEN;

                        string typeName = String.Empty;
                        if (isFixedLength)
                        {
                            if (maxLength < CHAR_MAXLEN) typeName = "char";
                            else if (maxLength < LONGTEXT_MAXLEN) typeName = "mediumtext";
                            else typeName = "longtext";
                        }
                        else
                        {
                            typeName = isMaxLength || maxLength > CHAR_MAXLEN ? "varchar" : "char";
                            maxLength = isMaxLength ? VARCHAR_MAXLEN : maxLength;
                        }
                        if (typeName.EndsWith("char") && isUnicode)
                            typeName = "n" + typeName;

                        return TypeUsage.CreateStringTypeUsage(StoreTypeNameToStorePrimitiveType[typeName], isUnicode, isFixedLength, maxLength);
                    }

                case PrimitiveTypeKind.DateTimeOffset:
                    return TypeUsage.CreateDefaultTypeUsage(StoreTypeNameToStorePrimitiveType["timestamp"]);
                case PrimitiveTypeKind.DateTime: 
                    return TypeUsage.CreateDefaultTypeUsage(StoreTypeNameToStorePrimitiveType["datetime"]);
                case PrimitiveTypeKind.Time:
                    return TypeUsage.CreateDefaultTypeUsage(StoreTypeNameToStorePrimitiveType["time"]);

                default:
                    throw new NotSupportedException(String.Format(Resources.NoStoreTypeForEdmType, edmType, primitiveType.PrimitiveTypeKind));
            }
        }

        private static XmlReader GetXmlResource(string resourceName)
        {
            Assembly executingAssembly = Assembly.GetExecutingAssembly();
            Stream stream = executingAssembly.GetManifestResourceStream(resourceName);
            return XmlReader.Create(stream);
        }

        private static XmlReader GetMappingResource(string resourceBaseName)
        {
            string rez = GetResourceAsString(
                String.Format("MySql.Data.Entity.Properties.{0}", resourceBaseName));

            StringReader sr = new StringReader(rez);
            return XmlReader.Create(sr);

        }

        private static string GetResourceAsString(string resourceName)
        {
            Assembly executingAssembly = Assembly.GetExecutingAssembly();
            Stream s = executingAssembly.GetManifestResourceStream(resourceName);
            StreamReader sr = new StreamReader(s);
            string resourceAsString = sr.ReadToEnd();
            sr.Close();
            s.Close();
            return resourceAsString;
        }
    }
}
