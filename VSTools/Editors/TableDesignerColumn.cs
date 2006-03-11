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
using System.Collections;

namespace Vsip.MyVSTools
{
	public enum ColumnType 
	{
		TinyInt, SmallInt, MediumInt, Int, BigInt,
		Real, Double, Float, Decimal, Numeric,
		Char, VarChar,
		Date, Time, TimeStamp, DateTime, 
		TinyBlob, Blob, MediumBlob, LongBlob,
		TinyText, Text, MediumText, LongText,
		Enum,
		Set
	}

	internal struct TableDesignerColumnData 
	{
		public string		desc;
		public int			len;
		public ColumnType	colType;
		public object		defaultValue;
		public string		colName;
		public bool			allowNull;
		public string		charSet;
		public string		collation;
		public bool			autoInc;
		public bool			unsigned;
		public bool			zeroFill;
		public int			decimals;
	}

	/// <summary>
	/// Summary description for TableDesignerColumn.
	/// </summary>
	public class TableDesignerColumn : IEditableObject, ICustomTypeDescriptor
	{
		private TableDesignerColumnData			data;
		private TableDesignerColumnData			backupData;
		private TableDesignerColumnCollection	parent;
		private bool							inTxn;
		private bool							isNew = true;

		[Category("General")]
		[DisplayName("Column Name")]
		public string ColumnName 
		{
			get { return data.colName; }
			set { data.colName = value; }
		}

		[Category("Properties")]
		[DisplayName("Character Set")]
		public string CharacterSet 
		{
			get { return data.charSet; }
			set { data.charSet = value; }
		}

		[Category("Properties")]
		[DisplayName("Collation")]
		public string Collation 
		{
			get { return data.collation; }
			set { data.collation = value; }
		}

		[Category("Properties")]
		[DisplayName("Auto Increment")]
		public bool AutoIncremenet 
		{
			get { return data.autoInc; }
			set { data.autoInc = value; }
		}

		[Category("Properties")]
		[DisplayName("Allow Unsigned Values")]
		public bool Unsigned 
		{
			get { return data.unsigned; }
			set { data.unsigned = value; }
		}

		[Category("Properties")]
		[DisplayName("Zero Fill")]
		public bool ZeroFill 
		{
			get { return data.zeroFill; }
			set { data.zeroFill = value; }
		}

		[Category("Properties")]
		[DisplayName("Decimal Places")]
		public int Decimals 
		{
			get { return data.decimals; }
			set { data.decimals = value; }
		}

		[Category("Properties")]
		[DisplayName("Allow Nulls")]
		public bool AllowNull 
		{
			get { return data.allowNull; }
			set { data.allowNull = value; }
		}

		[Category("General")]
		[Description("A field comment")]
		public string Description 
		{
			get { return data.desc; }
			set { data.desc = value; }
		}

		[Category("Properties")]
		[Description("Length of the field")]
		[DisplayName("Field Length")]
		public int Length 
		{
			get { return data.len; }
			set { data.len = value; }
		}

		public string ColumnTypeForGrid 
		{
			get { return data.colType == 0 ? String.Empty : data.colType.ToString(); }
			set { data.colType = (ColumnType)Enum.Parse( typeof(ColumnType), value, true ); }
		}

		[Category("General")]
		[DisplayName("Data Type")]
		public ColumnType ColumnType 
		{
			get { return data.colType; }
			set { data.colType = value; }
		}

		[Category("Properties")]
		[DisplayName("Default Value")]
		public object DefaultValue 
		{
			get { return data.defaultValue; }
			set { data.defaultValue = value; }
		}

		internal TableDesignerColumnCollection Parent 
		{
			get { return parent; }
			set { parent = value; }
		}

		private void OnColumnChanged() 
		{
			if (!inTxn && Parent != null) 
			{
				Parent.ColumnChanged(this);
			}
		}

		private bool IsTextType() 
		{
			if (data.colType == ColumnType.Text ||
				data.colType == ColumnType.TinyText ||
				data.colType == ColumnType.MediumText ||
				data.colType == ColumnType.LongText ||
				data.colType == ColumnType.VarChar) return true;
			return false;
		}


		#region IEditableObject Members

		public void EndEdit()
		{
			if (inTxn) 
			{
				isNew = false;
				backupData = new TableDesignerColumnData();
				inTxn = false;
			}
			OnColumnChanged();
		}

		public void CancelEdit()
		{
			if (inTxn) 
			{
				if (isNew)
					Parent.Remove( this );
				this.data = backupData;
				inTxn = false;
			}
		}

		public void BeginEdit()
		{
			if (!inTxn) 
			{
				this.backupData = data;
				inTxn = true;
			}
		}

		#endregion

		#region ICustomTypeDescriptor Members

		public TypeConverter GetConverter()
		{
			return TypeDescriptor.GetConverter(this, true);
		}

		public EventDescriptorCollection GetEvents(Attribute[] attributes)
		{
			return TypeDescriptor.GetEvents(this, attributes, true );
		}

		EventDescriptorCollection System.ComponentModel.ICustomTypeDescriptor.GetEvents()
		{
			return TypeDescriptor.GetEvents(this, true);
		}

		public string GetComponentName()
		{
			return TypeDescriptor.GetComponentName(this, true);
		}

		public object GetPropertyOwner(PropertyDescriptor pd)
		{
			return this;
		}

		public AttributeCollection GetAttributes()
		{
			return TypeDescriptor.GetAttributes(this, true);
		}

		public PropertyDescriptorCollection GetProperties(Attribute[] attributes)
		{
			PropertyDescriptorCollection coll = TypeDescriptor.GetProperties(this, attributes, true);

			PropertyDescriptorCollection newColl = new PropertyDescriptorCollection(null);

			foreach ( PropertyDescriptor pd in coll ) 
			{
				if (! pd.IsBrowsable) continue;

				if (pd.Name == "ColumnTypeForGrid") continue;
				int index = newColl.Add(new CustomPropertyDescriptor(pd));
				CustomPropertyDescriptor newPD = (CustomPropertyDescriptor)newColl[index];
				
				foreach (Attribute a in pd.Attributes)
				{
					if (a is DisplayNameAttribute) 
						newPD.SetDisplayName( (a as DisplayNameAttribute).DisplayName );
				}

				// if we are not a text type, then character set and collation does not 
				// make any sense.
				if ((pd.Name == "CharacterSet" || pd.Name == "Collation" )&& ! IsTextType())
					newPD.SetReadOnly(true);
			}

			return newColl;
		}

		PropertyDescriptorCollection System.ComponentModel.ICustomTypeDescriptor.GetProperties()
		{
			return TypeDescriptor.GetProperties(this, true);
		}

		public object GetEditor(Type editorBaseType)
		{
			 return TypeDescriptor.GetEditor(this, editorBaseType, true);
		}

		public PropertyDescriptor GetDefaultProperty()
		{
			return TypeDescriptor.GetDefaultProperty(this, true);
		}

		public EventDescriptor GetDefaultEvent()
		{
			return TypeDescriptor.GetDefaultEvent(this, true);
		}

		public string GetClassName()
		{
			return TypeDescriptor.GetClassName(this, true);
		}

		#endregion
	}
}
