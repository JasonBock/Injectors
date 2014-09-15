using Injectors.Core.Attributes.Generic;
using Mono.Cecil;
using Mono.Cecil.Cil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Injectors.Core.Attributes
{
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
	[Serializable]
	public sealed class ToStringAttribute : InjectorAttribute<TypeDefinition>
	{
		private const string AppendMethodName = "Append";
		private const string NameValueSeparator = ": ";
		private const string PropertiesSeparator = " || ";
		private const string ToStringMethodName = "ToString";

		private IEnumerable<PropertyDefinition> GetProperties(TypeDefinition target)
		{
			var properties = new List<PropertyDefinition>();

			var current = target;

			while(current != null)
			{
				properties.AddRange(
					from property in current.Properties
					where property.GetMethod != null
					where property.GetMethod.IsPublic
					select property);

				if(!this.FlattenHierarchy)
				{
					break;
				}

				current = current.BaseType != null ? current.BaseType.Resolve() : null;
			}

			return properties;
		}

		protected override void OnInject(TypeDefinition target)
		{
			if((from method in target.Methods
				 where method.Name == ToStringAttribute.ToStringMethodName
				 where method.Parameters.Count == 0
				 where (method.Attributes & MethodAttributes.Virtual) == MethodAttributes.Virtual
				 where (method.Attributes & MethodAttributes.Public) == MethodAttributes.Public
				 where (method.Attributes & MethodAttributes.HideBySig) == MethodAttributes.HideBySig
				 select method).SingleOrDefault() == null)
			{
				var toString = new MethodDefinition(ToStringAttribute.ToStringMethodName,
					MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.Virtual,
					target.Module.Import(typeof(string)));

				var builderType = typeof(StringBuilder);
				var builderCtor = target.Module.Import(builderType.GetConstructor(Type.EmptyTypes));
				var builderToString = target.Module.Import(
					builderType.GetMethod(ToStringAttribute.ToStringMethodName, Type.EmptyTypes));
				var appendString = target.Module.Import(
					builderType.GetMethod(ToStringAttribute.AppendMethodName, new Type[] { typeof(string) }));

				var processor = toString.Body.GetILProcessor();

				var properties = new List<PropertyDefinition>(this.GetProperties(target));

				if(properties.Count > 0)
				{
					processor.Append(processor.Create(OpCodes.Newobj, builderCtor));

					for(var i = 0; i < properties.Count; i++)
					{
						var property = properties[i];
						processor.Append(processor.Create(
							OpCodes.Ldstr, property.Name + ToStringAttribute.NameValueSeparator));
						processor.Append(processor.Create(OpCodes.Call, appendString));

						processor.Append(processor.Create(OpCodes.Ldarg_0));
						var propertyGetter = property.GetMethod;
						processor.Append(processor.Create(propertyGetter.IsVirtual ? OpCodes.Callvirt : OpCodes.Call,
							propertyGetter.DeclaringType.Module.Assembly != target.Module.Assembly ?
								target.Module.Import(propertyGetter) : propertyGetter));
						var append = target.Module.Import(builderType.GetMethod(ToStringAttribute.AppendMethodName,
							new Type[] { typeof(object) }));

						if(property.PropertyType.IsValueType)
						{
							processor.Append(processor.Create(OpCodes.Box,
								propertyGetter.DeclaringType.Module.Assembly != target.Module.Assembly ?
								target.Module.Import(property.PropertyType) : property.PropertyType));
						}

						processor.Append(processor.Create(OpCodes.Callvirt, append));

						if(i < properties.Count - 1)
						{
							processor.Append(processor.Create(OpCodes.Ldstr, ToStringAttribute.PropertiesSeparator));
							processor.Append(processor.Create(OpCodes.Callvirt, appendString));
						}
					}

					processor.Append(processor.Create(OpCodes.Callvirt, builderToString));
				}
				else
				{
					processor.Append(processor.Create(OpCodes.Ldstr, string.Empty));
				}

				processor.Append(processor.Create(OpCodes.Ret));
				target.Methods.Add(toString);
			}
		}

		public bool FlattenHierarchy { get; set; }
	}
}
