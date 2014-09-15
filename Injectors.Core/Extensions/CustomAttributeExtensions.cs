using Mono.Cecil;
using System;

namespace Injectors.Core.Extensions
{
	internal static class CustomAttributeExtensions
	{
		internal static T Create<T>(this CustomAttribute @this) where T : class
		{
			var type = @this.AttributeType.Resolve();
			var attributeTypeName = type.FullName + ", " + type.Module.Assembly.Name.Name;
			var attributeType = Type.GetType(attributeTypeName);

			object[] arguments = null;

			if(@this.HasConstructorArguments)
			{
				arguments = new object[@this.ConstructorArguments.Count];

				for(var i = 0; i < @this.ConstructorArguments.Count; i++)
				{
					arguments[i] = @this.ConstructorArguments[i].Value;
				}
			}

			T value = Activator.CreateInstance(attributeType, arguments) as T;

			if(@this.HasProperties)
			{
				foreach(var attributeProperty in @this.Properties)
				{
					attributeType.GetProperty(attributeProperty.Name)
						.SetValue(value, attributeProperty.Argument.Value, null);
				}
			}

			return value;
		}
	}
}
