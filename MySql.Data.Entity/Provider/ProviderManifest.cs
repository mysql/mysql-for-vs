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
        public MySqlProviderManifest()
            : base(GetManifest())
        {
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
            return GetXmlResource("MySql.Data.Entity.Properties.SchemaMapping.msl");
        }

        private XmlReader GetStoreSchemaDescription()
        {
            return GetXmlResource("MySql.Data.Entity.Properties.SchemaMapping.ssdl");
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

        public override string Token
        {
            get { throw new NotImplementedException(); }
        }

        private static XmlReader GetXmlResource(string resourceName)
        {
            Assembly executingAssembly = Assembly.GetExecutingAssembly();
            Stream stream = executingAssembly.GetManifestResourceStream(resourceName);
            return XmlReader.Create(stream);
        }
    }
}
