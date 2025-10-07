
//  Microsoft (R) .NET Framework IL Disassembler.  Version 4.8.3928.0
//  Copyright (c) Microsoft Corporation.  All rights reserved.



// Metadata version: v4.0.30319
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
.assembly JP_NET_LAB.O
{
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilationRelaxationsAttribute::.ctor(int32) = ( 01 00 08 00 00 00 00 00 ) 
  .custom instance void [mscorlib]System.Runtime.CompilerServices.RuntimeCompatibilityAttribute::.ctor() = ( 01 00 01 00 54 02 16 57 72 61 70 4E 6F 6E 45 78   // ....T..WrapNonEx
                                                                                                             63 65 70 74 69 6F 6E 54 68 72 6F 77 73 01 )       // ceptionThrows.

  // --- The following custom attribute is added automatically, do not uncomment -------
  //  .custom instance void [mscorlib]System.Diagnostics.DebuggableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggableAttribute/DebuggingModes) = ( 01 00 02 00 00 00 00 00 ) 

  .hash algorithm 0x00008004
  .ver 0:0:0:0
}
.module JP_NET_LAB.O.dll
// MVID: {7BF68882-6B42-46F1-94ED-6E46F3E38194}
.custom instance void System.Runtime.CompilerServices.RefSafetyRulesAttribute::.ctor(int32) = ( 01 00 0B 00 00 00 00 00 ) 
.imagebase 0x10000000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000001    //  ILONLY
// Image base: 0x07110000


// =============== CLASS MEMBERS DECLARATION ===================

.class private auto ansi sealed beforefieldinit Microsoft.CodeAnalysis.EmbeddedAttribute
       extends [mscorlib]System.Attribute
{
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .custom instance void Microsoft.CodeAnalysis.EmbeddedAttribute::.ctor() = ( 01 00 00 00 ) 
  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [mscorlib]System.Attribute::.ctor()
    IL_0006:  ret
  } // end of method EmbeddedAttribute::.ctor

} // end of class Microsoft.CodeAnalysis.EmbeddedAttribute

.class private auto ansi sealed beforefieldinit System.Runtime.CompilerServices.RefSafetyRulesAttribute
       extends [mscorlib]System.Attribute
{
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .custom instance void Microsoft.CodeAnalysis.EmbeddedAttribute::.ctor() = ( 01 00 00 00 ) 
  .custom instance void [mscorlib]System.AttributeUsageAttribute::.ctor(valuetype [mscorlib]System.AttributeTargets) = ( 01 00 02 00 00 00 02 00 54 02 0D 41 6C 6C 6F 77   // ........T..Allow
                                                                                                                         4D 75 6C 74 69 70 6C 65 00 54 02 09 49 6E 68 65   // Multiple.T..Inhe
                                                                                                                         72 69 74 65 64 00 )                               // rited.
  .field public initonly int32 Version
  .method public hidebysig specialname rtspecialname 
          instance void  .ctor(int32 A_1) cil managed
  {
    // Code size       14 (0xe)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [mscorlib]System.Attribute::.ctor()
    IL_0006:  ldarg.0
    IL_0007:  ldarg.1
    IL_0008:  stfld      int32 System.Runtime.CompilerServices.RefSafetyRulesAttribute::Version
    IL_000d:  ret
  } // end of method RefSafetyRulesAttribute::.ctor

} // end of class System.Runtime.CompilerServices.RefSafetyRulesAttribute

.class public auto ansi beforefieldinit JP_NET.Lab1.ClassA
       extends [mscorlib]System.Object
{
  .field private int32 _a
  .field public int32 b
  .method public hidebysig specialname instance int32 
          get_A() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldfld      int32 JP_NET.Lab1.ClassA::_a
    IL_0006:  ret
  } // end of method ClassA::get_A

  .method public hidebysig specialname instance void 
          set_A(int32 'value') cil managed
  {
    // Code size       8 (0x8)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldarg.1
    IL_0002:  stfld      int32 JP_NET.Lab1.ClassA::_a
    IL_0007:  ret
  } // end of method ClassA::set_A

  .method public hidebysig instance void 
          MethodOfClassA() cil managed
  {
    // Code size       57 (0x39)
    .maxstack  3
    .locals init (class [System]System.Diagnostics.FileVersionInfo V_0,
             string V_1)
    IL_0000:  call       class [mscorlib]System.Reflection.Assembly [mscorlib]System.Reflection.Assembly::GetExecutingAssembly()
    IL_0005:  callvirt   instance string [mscorlib]System.Reflection.Assembly::get_Location()
    IL_000a:  call       class [System]System.Diagnostics.FileVersionInfo [System]System.Diagnostics.FileVersionInfo::GetVersionInfo(string)
    IL_000f:  stloc.0
    IL_0010:  ldstr      "{0}.{1}"
    IL_0015:  ldloc.0
    IL_0016:  callvirt   instance int32 [System]System.Diagnostics.FileVersionInfo::get_ProductMajorPart()
    IL_001b:  box        [mscorlib]System.Int32
    IL_0020:  ldloc.0
    IL_0021:  callvirt   instance int32 [System]System.Diagnostics.FileVersionInfo::get_ProductMinorPart()
    IL_0026:  box        [mscorlib]System.Int32
    IL_002b:  call       string [mscorlib]System.String::Format(string,
                                                                object,
                                                                object)
    IL_0030:  stloc.1
    IL_0031:  ldarg.0
    IL_0032:  ldloc.1
    IL_0033:  call       instance void JP_NET.Lab1.ClassA::PartialMethodOfClassA(object)
    IL_0038:  ret
  } // end of method ClassA::MethodOfClassA

  .method private hidebysig instance void 
          PartialMethodOfClassA(object x) cil managed
  {
    // Code size       12 (0xc)
    .maxstack  8
    IL_0000:  ldstr      "Wydruk z klasy A. {0}"
    IL_0005:  ldarg.1
    IL_0006:  call       void [mscorlib]System.Console::WriteLine(string,
                                                                  object)
    IL_000b:  ret
  } // end of method ClassA::PartialMethodOfClassA

  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
    IL_0006:  ret
  } // end of method ClassA::.ctor

  .property instance int32 A()
  {
    .get instance int32 JP_NET.Lab1.ClassA::get_A()
    .set instance void JP_NET.Lab1.ClassA::set_A(int32)
  } // end of property ClassA::A
} // end of class JP_NET.Lab1.ClassA


// =============================================================

// *********** DISASSEMBLY COMPLETE ***********************
// WARNING: Created Win32 resource file JP_NET_LAB.O.res
