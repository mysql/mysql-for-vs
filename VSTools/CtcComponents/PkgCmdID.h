// PkgCmdID.h
// Command IDs used in defining command bars
//

// do not use #pragma once - used by ctc compiler
#ifndef __PKGCMDID_H_
#define __PKGCMDID_H_

///////////////////////////////////////////////////////////////////////////////
// Menu IDs
#define MyToolbar                   0x1000
#define ServerCtxtMenu              0x1001
#define DatabaseCtxtMenu            0x1002
#define TablesCtxtMenu              0x1003
#define ProceduresCtxtMenu          0x1004
#define ProcedureCtxtMenu           0x1005
#define TableCtxtMenu               0x1006
#define ViewsCtxtMenu               0x1007
#define FunctionsCtxtMenu           0x1008
#define ViewCtxtMenu                0x1009
#define FunctionCtxtMenu            0x1010
#define TriggerCtxtMenu             0x1011
#define ColumnCtxtMenu              0x1012

///////////////////////////////////////////////////////////////////////////////
// Menu Group IDs

#define MyMenuGroup                 0x1020
#define MyToolbarGroup              0x1050
#define ConnectionGroup             0x1051
#define DeleteGroup                 0x1052
#define PropertiesGroup             0x1053
#define TablesGroup                 0x1054
#define ProceduresGroup             0x1055
#define ProcedureGroup              0x1056
#define TableGroup                  0x1057
#define ViewsGroup                  0x1058
#define FunctionsGroup              0x1059
#define ViewGroup                   0x1060
#define FunctionGroup               0x1061
#define TriggerGroup                0x1062

///////////////////////////////////////////////////////////////////////////////
// Command IDs

#define cmdidMyCommand        0x100
#define cmdidMyExplorerWindow 0x101
#define cmdidRefresh          0x102
#define cmdidModifyConnection 0x103
#define cmdidDelete           0x104
#define cmdidRename           0x105
#define cmdidProperties       0x106
#define cmdidAddNewTable      0x107
#define cmdidAddNewProcedure  0x108
#define cmdidOpen             0x109
#define cmdidClone            0x110
#define cmdidAddNewTrigger    0x111
#define cmdidOpenTableDef     0x112
#define cmdidShowTableData    0x113
#define cmdidAddNewView       0x114
#define cmdidAddNewFunction   0x115

///////////////////////////////////////////////////////////////////////////////
// Bitmap IDs
#define bmpPic1          1
#define bmpPic2          2
#define bmpPicSmile      3
#define bmpPicX          4
#define bmpPicArrows     5
#define bmpPicRefresh    6
#define bmpPicDelete     7
#define bmpPicProperties 8
#define bmpPicOpen       9
#define bmpPicClone      10


#endif // __PKGCMDID_H_
