// Copyright (C) 2006-2007 MySQL AB
//
// This file is part of MySQL Tools for Visual Studio.
// MySQL Tools for Visual Studio is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public 
// License version 2.1 as published by the Free Software Foundation
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with this program; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA using System;

/*
 * Definition of the numeric part of the IDs for the CTC commands.
 */

///////////////////////////////////////////////////////////////////////////////
// Commands
#define cmdidCreateTable 0x0100

#define cmdidAlterView  0x0111

#define cmdidCreateProcedure 0x0120
#define cmdidAlterProcedure  0x0121

#define cmdidCreateFunction 0x0130

#define cmdidCreateTrigger 0x0140
#define cmdidAlterTrigger 0x0141

#define cmdidCreateUDF 0x0150
#define cmdidDelete 0x0151


//#define cmdidNewQuery 0x0200
#define cmdidAddNewTableGlobal 500
#define cmdidAddNewViewGlobal 501
#define cmdidAddNewProcedureGlobal 502
#define cmdidAddNewFunctionGlobal 503
#define cmdidAddNewUDFGlobal 504

#define menuAddNew2005 1000
#define menuAddNew2008 1001

#define groupAddNew2005 1002
#define groupAddNew2008 1003

#define cmdidPrimaryKey  109
#define cmdidIndexesAndKeys  675
#define cmdidForeignKeys  676
#define cmdidGenerateChangeScript  173