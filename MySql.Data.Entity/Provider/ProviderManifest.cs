
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

        public override TypeUsage GetStoreType(TypeUsage edmType)
        {
            throw new NotImplementedException();
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
