using Injectors.Core.Attributes.Generic;
using Mono.Cecil;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Injectors.Core.Extensions
{
	internal static class ICustomAttributeProviderExtensions
	{
		private static readonly string baseFullName = typeof(InjectorAttribute<>).FullName;
		private static readonly string baseScopeName = typeof(InjectorAttribute<>).Module.ScopeName;

		internal static ReadOnlyCollection<InjectorAttribute<T>> GetInjectors<T>(this T @this)
			where T : class, ICustomAttributeProvider
		{
			var injectors = new List<InjectorAttribute<T>>();

			foreach(var attribute in @this.CustomAttributes)
			{
				var baseAttributeType = attribute.AttributeType.Resolve().BaseType.Resolve();

				while(baseAttributeType != null && baseAttributeType.BaseType != null)
				{
					if(baseAttributeType.FullName == ICustomAttributeProviderExtensions.baseFullName &&
						baseAttributeType.Scope.Name == ICustomAttributeProviderExtensions.baseScopeName)
					{
						var injectorAttribute = attribute.Create<InjectorAttribute<T>>();
						injectors.Add(injectorAttribute);
						break;
					}

					baseAttributeType = baseAttributeType.BaseType.Resolve();
				}
			}

			return injectors.AsReadOnly();
		}

		internal static void RunInjectors<T>(this T @this)
			where T : class, ICustomAttributeProvider
		{
			var injectors = @this.GetInjectors();

			foreach(var injector in injectors)
			{
				injector.Inject(@this);
			}
		}
	}
}
