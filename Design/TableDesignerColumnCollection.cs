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

namespace MySql.Design
{
	/// <summary>
	/// Summary description for TableDesignerColumnCollection.
	/// </summary>
	public class TableDesignerColumnCollection : CollectionBase, IBindingList
	{
		public TableDesignerColumn this[ int index ]  
		{
			get  
			{
				return( (TableDesignerColumn) List[index] );
			}
			set  
			{
				List[index] = value;
			}
		}

		public int Add( TableDesignerColumn value )  
		{
			return List.Add( value );
		}

		public int IndexOf( TableDesignerColumn value )  
		{
			return( List.IndexOf( value ) );
		}

		public void Insert( int index, TableDesignerColumn value )  
		{
			List.Insert( index, value );
		}

		public void Remove( TableDesignerColumn value )  
		{
			List.Remove( value );
		}

		public bool Contains( TableDesignerColumn value )  
		{
			// If value is not of type TableDesignerColumn, this will return false.
			return( List.Contains( value ) );
		}

		protected virtual void OnListChanged( ListChangedEventArgs ev )
		{
			if (ListChanged != null)
				ListChanged( this, ev );
		}

		protected override void OnClear() 
		{
			foreach (TableDesignerColumn c in List) 
			{
				c.Parent = null; 
			}
		}

		protected override void OnClearComplete() 
		{
			OnListChanged( new ListChangedEventArgs( ListChangedType.Reset, -1 ) );
		}

		protected override void OnInsertComplete(int index, object value) 
		{
			TableDesignerColumn c = (TableDesignerColumn)value;
			c.Parent = this;
			OnListChanged(new ListChangedEventArgs(ListChangedType.ItemAdded, index));
		}

		protected override void OnRemoveComplete(int index, object value) 
		{
			TableDesignerColumn c = (TableDesignerColumn)value;
			c.Parent = this;
			OnListChanged(new ListChangedEventArgs(ListChangedType.ItemDeleted, index));
		}

		protected override void OnSetComplete(int index, object oldValue, object newValue) 
		{
			if (oldValue != newValue) 
			{

				TableDesignerColumn oldCol = (TableDesignerColumn)oldValue;
				TableDesignerColumn newCol = (TableDesignerColumn)newValue;
            
				oldCol.Parent = null;
				newCol.Parent = this;
            
            
				OnListChanged(new ListChangedEventArgs(ListChangedType.ItemAdded, index));
			}
		}

		internal void ColumnChanged(TableDesignerColumn col)
		{
        
			int index = List.IndexOf( col );
        
			OnListChanged(new ListChangedEventArgs(ListChangedType.ItemChanged, index));
		}


		#region IBindingList Members

		void IBindingList.AddIndex(PropertyDescriptor property) 
		{
			throw new NotSupportedException(); 
		}


		public bool AllowNew
		{
			get	{ return true; }
		}

		void IBindingList.ApplySort(PropertyDescriptor property, ListSortDirection direction) 
		{
			throw new NotSupportedException(); 
		}


		PropertyDescriptor IBindingList.SortProperty 
		{ 
			get { throw new NotSupportedException(); }
		}


		ListSortDirection IBindingList.SortDirection 
		{ 
			get { throw new NotSupportedException(); }
		}

		int IBindingList.Find(PropertyDescriptor property, object key) 
		{
			throw new NotSupportedException(); 
		}


		public bool SupportsSorting
		{
			get	{ return false; }
		}

		bool IBindingList.IsSorted
		{
			get	{ throw new NotSupportedException(); }
		}

		public bool AllowRemove
		{
			get	{ return true; }
		}

		public bool SupportsSearching
		{
			get	{ return false; }
		}

		public System.ComponentModel.ListSortDirection SortDirection
		{
			get	{ return new System.ComponentModel.ListSortDirection (); }
		}

		public event System.ComponentModel.ListChangedEventHandler ListChanged;

		public bool SupportsChangeNotification
		{
			get	{ return true; }
		}

		void IBindingList.RemoveSort() 
		{
			throw new NotSupportedException(); 
		}

		object IBindingList.AddNew() 
		{
			TableDesignerColumn c = new TableDesignerColumn();
			List.Add( c );
			return c;
		}

		public object AddNew()
		{
			return (TableDesignerColumn)((IBindingList)this).AddNew();
		}

		public bool AllowEdit
		{
			get	{ return true; }
		}

		void IBindingList.RemoveIndex(PropertyDescriptor property) 
		{
			throw new NotSupportedException(); 
		}


		#endregion

	}
}
