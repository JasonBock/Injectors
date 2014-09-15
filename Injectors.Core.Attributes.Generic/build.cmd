copy "..\packages\Spackle.5.0.0.0\lib\.NetFramework 4.0\Spackle.dll" "Spackle.dll" /B /Y
ilasm /dll Injectors.Core.Attributes.Generic.il /deb=opt
peverify /md /il Injectors.Core.Attributes.Generic.dll