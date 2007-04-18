using System;
using System.Collections;
using System.ComponentModel;
using System.Reflection;
using System.Resources;

namespace MySql.Design
{
	public class PropertiesCustomTypeConverter : TypeConverter
	{
		public override bool GetPropertiesSupported(ITypeDescriptorContext context)
		{
			System.Windows.Forms.MessageBox.Show("GetPropertiesSupported");
			return true;
		}


        public override PropertyDescriptorCollection GetProperties( ITypeDescriptorContext context, 
            object value, Attribute[] attributes)
        {
			System.Windows.Forms.MessageBox.Show("GetProperties");
			// Get the collection of properties
			PropertyDescriptorCollection baseProps = 
			    TypeDescriptor.GetProperties(value, attributes);

			return baseProps;

/*			PropertyDescriptorCollection deluxeProps = 
				new PropertyDescriptorCollection(null);

			// For each property use a property descriptor of 
			// our own that has custom behaviour.
			ArrayList orderedPropertyAttributesList = new ArrayList();
			foreach( PropertyDescriptor oProp in baseProps )
			{
				PropertyAttributes propertyAttributes = GetPropertyAttributes(oProp, value);
				
				if (propertyAttributes.IsBrowsable) {
					orderedPropertyAttributesList.Add(propertyAttributes);
					deluxeProps.Add(
						new PropertyDescriptorEx(oProp, propertyAttributes));
				}
			}
			orderedPropertyAttributesList.Sort();
            //
            // Build a string list of the ordered names
            //
            ArrayList propertyNames = new ArrayList();
            foreach (PropertyAttributes propertyAttributes in orderedPropertyAttributesList)
            {
                propertyNames.Add(propertyAttributes.Name);
            }
            //
            // Pass in the ordered list for the PropertyDescriptorCollection to sort by.
			// (Sorting by passing a custom IComparer somehow doesn't work.
            //
            return deluxeProps.Sort((string[])propertyNames.ToArray(typeof(string)));*/
		}
/*		
		/// <summary>
		/// Get property attributes for given property descriptor and target object.
		/// </summary>
		private PropertyAttributes GetPropertyAttributes( PropertyDescriptor propertyDescriptor, object target )
		{
			PropertyAttributes propertyAttributes =  new PropertyAttributes(propertyDescriptor.Name);
            string resourceBaseName = null;
			string displayName = null;
		    string displayNameResourceName = null;
		    string descriptionResourceName = null;
		    string categoryResourceName = null;
			ResourceManager rm = null;

			//
			// First fill propertyAttributes with statically defined information.
			//
			
			foreach( Attribute attribute in propertyDescriptor.Attributes )
			{
			    Type type = attribute.GetType();
    		    // If there's a DisplayNameAttribute defined, use that DisplayName.
				if( type.Equals(typeof(DisplayNameAttribute)) )
				{
					displayName = ((DisplayNameAttribute)attribute).DisplayName;
				} else if( type.Equals(typeof(GlobalizedPropertyAttribute)) )
				{
				    // Get specific info about where to find resources for given property.
					displayNameResourceName = ((GlobalizedPropertyAttribute)attribute).DisplayNameId;
					descriptionResourceName = ((GlobalizedPropertyAttribute)attribute).DescriptionId;
				    categoryResourceName = ((GlobalizedPropertyAttribute)attribute).CategoryId;
					resourceBaseName = ((GlobalizedPropertyAttribute)attribute).BaseName;
				} else if (type.Equals(typeof(PropertyOrderAttribute))) {
					propertyAttributes.Order = ((PropertyOrderAttribute)attribute).Order;
                }
			}
			
			if (resourceBaseName == null) {
			    foreach (
			             Attribute attribute in 
			                 propertyDescriptor.ComponentType.GetCustomAttributes(true)
			    ) {
			        if( attribute.GetType().Equals(typeof(GlobalizedTypeAttribute)) ) {
    				    // Get specific info about where to find resources for given Type.
			            resourceBaseName = ((GlobalizedTypeAttribute)attribute).BaseName;
			        }
			    }
			    if (resourceBaseName == null) {
    			    resourceBaseName = propertyDescriptor.ComponentType.Namespace + 
    		            "." + propertyDescriptor.ComponentType.Name;
			    }
			}

		    // See if at least the culture neutral resources are there.
		    // If not, disable globalization
		    Assembly assembly = propertyDescriptor.ComponentType.Assembly;
		    if (assembly.GetManifestResourceInfo(resourceBaseName + ".resources") == null) {
		        rm = null;
		    } else {
    		    rm = new ResourceManager(resourceBaseName, assembly);
		        if (displayNameResourceName == null) {
            		displayNameResourceName = 
            		    propertyDescriptor.DisplayName + ".DisplayName";
		        }
		        if (descriptionResourceName == null) {
            		descriptionResourceName = 
            		    propertyDescriptor.DisplayName + ".Description";
		        }
		        if (categoryResourceName == null) {
		            categoryResourceName =
		                propertyDescriptor.Category + ".Category";
		        }
		    }
			
			// Display name.
			if (rm != null) {
			    propertyAttributes.DisplayName = rm.GetString(displayNameResourceName);
			} else {
			    propertyAttributes.DisplayName = null;
			}
			if (propertyAttributes.DisplayName == null) {
		        propertyAttributes.DisplayName = displayName;
		    }
			if (propertyAttributes.DisplayName == null)
		    {
		        propertyAttributes.DisplayName = propertyDescriptor.DisplayName;
			}
			
			// Description.
			if (rm != null) {
			    propertyAttributes.Description = rm.GetString(descriptionResourceName);
			} else {
			    propertyAttributes.Description = null;
			}
			if (propertyAttributes.Description == null)
		    {
		        propertyAttributes.Description = propertyDescriptor.Description;
			}
			
			// Category.
			if (rm != null) {
			    propertyAttributes.Category = rm.GetString(categoryResourceName);
			} else {
			    propertyAttributes.Category = null;
			}
			if (propertyAttributes.Category == null)
		    {
		        propertyAttributes.Category = propertyDescriptor.Category;
			}
			
			// IsReadonly.
			propertyAttributes.IsReadOnly = propertyDescriptor.IsReadOnly;
			
			// IsBrowsable.
			propertyAttributes.IsBrowsable = propertyDescriptor.IsBrowsable;
			
			//
			// Now let target be able to override each of these property attributes
			// dynamically.
			//

			PropertyAttributesProviderAttribute propertyAttributesProviderAttribute =
				(PropertyAttributesProviderAttribute)
					propertyDescriptor.Attributes[typeof(PropertyAttributesProviderAttribute)];
			if (propertyAttributesProviderAttribute != null) {
				MethodInfo propertyAttributesProvider = 
					propertyAttributesProviderAttribute.GetPropertyAttributesProvider(target);
				if (propertyAttributesProvider != null) {
					propertyAttributesProvider.Invoke(target, new object[] {propertyAttributes});
				}
			}
			
			return propertyAttributes;
		}*/
	}
}
