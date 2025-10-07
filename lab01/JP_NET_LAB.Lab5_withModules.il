
//  Microsoft (R) .NET Framework IL Disassembler.  Version 4.8.3928.0
//  Copyright (c) Microsoft Corporation.  All rights reserved.



// Metadata version: v4.0.30319
.module extern ProgA.netmodule
.assembly extern mscorlib
{
  .publickeytoken = (B7 7A 5C 56 19 34 E0 89 )                         // .z\V.4..
  .ver 4:0:0:0
}
.assembly extern System
{
  .publickeytoken = (B7 7A 5C 56 19 34 E0 89 )                         // .z\V.4..
  .ver 4:0:0:0
}
.assembly extern JP_NET_LAB.Lab1a
{
  .ver 1:0:0:0
}
.assembly JP_NET_LAB.Lab5_withModules
{
  .hash algorithm 0x00008004
  .ver 0:0:0:0
}
.file ClassA.netmodule
    .hash = (35 2B 0B B4 6F 73 84 6F 34 6B A9 5D 44 AB CA 85   // 5+..os.o4k.]D...
             17 09 1C EB ) 
.file ClassB.netmodule
    .hash = (0C 6E 56 0F BC A2 62 52 59 01 FA 7D D8 05 F4 34   // .nV...bRY..}...4
             5C EA 1C A7 )                                     // \...
.file ProgA.netmodule
    .hash = (56 64 49 BB 2E 02 A8 91 60 E2 BB 99 1C 9A 7A 7C   // VdI.....`.....z|
             6F 38 B4 AF )                                     // o8..
.class extern public JP_NET.Lab1.ClassA
{
  .file ClassA.netmodule
  .class 0x02000002
}
.class extern public JP_NET.Lab1.ClassB
{
  .file ClassB.netmodule
  .class 0x02000002
}
.module JP_NET_LAB.Lab5_withModules.exe
// MVID: {D2275169-7BDF-4B8A-B6A3-0DBBFD25A129}
.imagebase 0x00400000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000001    //  ILONLY
// Image base: 0x07680000


// ================== GLOBAL METHODS =========================

.method privatescope static void  __EntryPoint$PST06000001() cil managed
{
  .entrypoint
  // Code size       8 (0x8)
  .maxstack  8
  IL_0000:  tail.
  IL_0002:  call       void [.module ProgA.netmodule]JP_NET.Lab1.Program::Main()
  IL_0007:  ret
} // end of global method __EntryPoint


// =============================================================

// *********** DISASSEMBLY COMPLETE ***********************
// WARNING: Created Win32 resource file JP_NET_LAB.Lab5_withModules.res
