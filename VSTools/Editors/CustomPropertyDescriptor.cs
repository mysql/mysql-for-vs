// Copyright (C) 2004 MySQL AB
//
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License version 2 as published by
// the Free Software Foundation
//
// There are special exceptions to the terms and conditions of the GPL 
// as it is applied to this software. View the full text of the 
// exception in file EXCEPTIONS in the directory of this software 
// distribution.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA 

using System;
using System.ComponentModel;

namespace MySql.VSTools
{
	public class CustomPropertyDescriptor : PropertyDescriptor
	{
		private PropertyDescriptor basePropertyDescriptor; 
		private string	displayName;
		private bool	isReadOnly;
		private bool	isBrowsable;
		private string	name;

		/// <summary>
		/// Constructor.
		/// </summary>
		public CustomPropertyDescriptor( PropertyDescriptor basePropertyDescriptor )
			:base(basePropertyDescriptor)
		{
			this.basePropertyDescriptor = basePropertyDescriptor;
			displayName = basePropertyDescriptor.DisplayName;
			isReadOnly = basePropertyDescriptor.IsReadOnly;
			isBrowsable = basePropertyDescriptor.IsBrowsable;
			name = basePropertyDescriptor.Name;
		}

		public override bool CanResetValue(object component)
		{
			return basePropertyDescriptor.CanResetValue( component ); 
		}

		public override Type ComponentType
		{
			get { return basePropertyDescriptor.ComponentType; }
		}

		public void SetReadOnly( bool value ) 
		{
			isReadOnly = value; 
		}

		public void SetDisplayName( string name ) 
		{
			displayName = name; 
		}

		public override string Name 
		{
			get { return base.Name; }
		}

		public override string DisplayName
		{
			get { return displayName; }
		}

		public override string Description
		{
			get	{ return base.Description; }
		}

		public override string Category
		{
			get	{ return base.Category; }
		}

		public override object GetValue(object component)
		{
			return this.basePropertyDescriptor.GetValue(component);
		}

		public override bool IsReadOnly
		{
			get { return isReadOnly; }
		}

		public new bool IsBrowsable
		{
			get { return isBrowsable; }
		}

		public override Type PropertyType
		{
			get { return this.basePropertyDescriptor.PropertyType; }
		}

		public override void ResetValue(object component)
		{
			this.basePropertyDescriptor.ResetValue(component);
		}

		public override bool ShouldSerializeValue(object component)
		{
			return this.basePropertyDescriptor.ShouldSerializeValue(component);
		}

		public override void SetValue(object component, object value)
		{
			this.basePropertyDescriptor.SetValue(component, value);
		}
	}
}
