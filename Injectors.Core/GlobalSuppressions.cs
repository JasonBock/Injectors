// This file is used by Code Analysis to maintain SuppressMessage 
// attributes that are applied to this project. 
// Project-level suppressions either have no target or are given 
// a specific target and scoped to a namespace, type, member, etc. 
//
// To add a suppression to this file, right-click the message in the 
// Error List, point to "Suppress Message(s)", and click 
// "In Project Suppression File". 
// You do not need to add suppressions to this file manually. 

[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1020:AvoidNamespacesWithFewTypes", Scope = "namespace", Target = "Injectors.Core")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1020:AvoidNamespacesWithFewTypes", Scope = "namespace", Target = "Injectors.Core.Attributes")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Scope = "member", Target = "Injectors.Core.InjectorRunner.#Run(System.IO.FileSystemInfo)")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Scope = "member", Target = "Injectors.Core.Attributes.NotNullAttribute.#OnInject(Mono.Cecil.ParameterDefinition)")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Scope = "member", Target = "Injectors.Core.Attributes.ToStringAttribute.#OnInject(Mono.Cecil.TypeDefinition)")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Scope = "member", Target = "Injectors.Core.Attributes.TraceAttribute.#OnInject(Mono.Cecil.MethodDefinition)")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA2210:AssembliesShouldHaveValidStrongNames")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "Injectors.Core.Attributes.TraceAttribute.AddTrace(Mono.Cecil.Cil.ILProcessor,Mono.Cecil.Cil.Instruction,Mono.Cecil.Cil.VariableDefinition,System.String,Mono.Cecil.MethodReference,Mono.Cecil.MethodReference)", Scope = "member", Target = "Injectors.Core.Attributes.TraceAttribute.#OnInject(Mono.Cecil.MethodDefinition)")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1307:SpecifyStringComparison", MessageId = "System.String.LastIndexOf(System.String)", Scope = "member", Target = "Injectors.Core.Attributes.NotNullAttribute.#CreatePoint(Mono.Cecil.Cil.SequencePoint,Mono.Cecil.MethodDefinition,Mono.Cecil.ParameterDefinition)")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1804:RemoveUnusedLocals", MessageId = "parameterNameIndex", Scope = "member", Target = "Injectors.Core.Attributes.NotNullAttribute.#CreatePoint(Mono.Cecil.Cil.SequencePoint,Mono.Cecil.MethodDefinition,Mono.Cecil.ParameterDefinition)")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1309:UseOrdinalStringComparison", MessageId = "System.String.LastIndexOf(System.String,System.StringComparison)", Scope = "member", Target = "Injectors.Core.Attributes.CSharpNotNullAttributeDebuggingParser.#GetPoint()")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Scope = "member", Target = "Injectors.Core.Attributes.NotNullAttributeParser+NotNullAttributeVisitor.#VisitMethodDeclaration(ICSharpCode.NRefactory.Ast.MethodDeclaration,System.Object)")]
