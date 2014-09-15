.assembly extern mscorlib
{
	.publickeytoken = (B7 7A 5C 56 19 34 E0 89 )                        
	.ver 4:0:0:0
}

.assembly extern System.Core
{
	.publickeytoken = (B7 7A 5C 56 19 34 E0 89 )                        
	.ver 4:0:0:0
}

.assembly extern Spackle
{
	.ver 4:3:0:0
}

.assembly Injectors.Core.Attributes.Generic
{
	.hash algorithm 0x00008004
	.ver 0:7:0:0
}

.module Injectors.Core.Attributes.Generic.dll
.imagebase 0x00400000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003
.corflags 0x00000001

.class public abstract auto ansi beforefieldinit 
	Injectors.Core.Attributes.Generic.InjectorAttribute`1<class T>
		extends [mscorlib]System.Attribute
{
	.custom instance void [mscorlib]System.AttributeUsageAttribute::.ctor(
		valuetype [mscorlib]System.AttributeTargets) = ( 01 00 04 00 00 00 00 00 ) 

	.method family hidebysig specialname rtspecialname 
		instance void .ctor() cil managed
	{
		.maxstack  8
		IL_0000:  ldarg.0
		IL_0001:  call instance void [mscorlib]System.Attribute::.ctor()
		IL_0006:  ret
	}

	.method public hidebysig instance void 
		Inject(!T target) cil managed
	{
		.maxstack  8
		IL_0000:  ldarg.1
		IL_0001:  box  !T
		IL_0006:  ldstr "target"
		IL_000b:  call void 
			[Spackle]Spackle.Extensions.ObjectExtensions::CheckParameterForNull(object, string)
		IL_0010:  ldarg.0
		IL_0011:  ldarg.1
		IL_0012:  callvirt instance void class Injectors.Core.Attributes.Generic.InjectorAttribute`1<!T>::OnInject(!0)
		IL_0017:  ret
	}

	.method family hidebysig newslot abstract virtual 
		instance void OnInject(!T target) cil managed { }
}