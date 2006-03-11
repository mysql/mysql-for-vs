// Guids.h
//

// do not use #pragma once - used by ctc compiler
#ifndef __GUIDS_H_
#define __GUIDS_H_

#ifndef _CTC_GUIDS_


// guidPersistanceSlot ID for our Tool Window
// {ae5d17fe-e250-4f36-b38b-533686e0e0ca}
DEFINE_GUID(GUID_guidPersistanceSlot, 
0xAE5D17FE, 0xE250, 0x4F36, 0xB3, 0x8B, 0x53, 0x36, 0x86, 0xE0, 0xE0, 0xCA);

#define guidMyVSToolsPkg   CLSID_MyVSToolsPackage

// Command set guid for our commands (used with IOleCommandTarget)
// {9f3a024b-f2ba-4b0b-82a3-56a3ec5e3d21}
DEFINE_GUID(guidMyVSToolsCmdSet, 
0x9F3A024B, 0xF2BA, 0x4B0B, 0x82, 0xA3, 0x56, 0xA3, 0xEC, 0x5E, 0x3D, 0x21);

#else  // _CTC_GUIDS

#define guidMyVSToolsPkg      { 0x5CEB61C4, 0x7111, 0x44F8, { 0xB7, 0xF2, 0xAC, 0x4, 0x9B, 0x81, 0xAD, 0x32 } }
#define guidMyVSToolsCmdSet	  { 0x9F3A024B, 0xF2BA, 0x4B0B, { 0x82, 0xA3, 0x56, 0xA3, 0xEC, 0x5E, 0x3D, 0x21 } }


#endif // _CTC_GUIDS_

#endif // __GUIDS_H_
