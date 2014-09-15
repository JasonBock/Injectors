using Injectors.Core.Attributes;
using System;

namespace Injectors.SampleTarget
{
	[ToString(FlattenHierarchy = true)]
	public sealed class AttributedCustomer : Customer
	{
		private AttributedCustomer()
			: base() { }

		[Trace]
		public AttributedCustomer(Guid id, [NotNull] int age,
			[NotNull]string firstName, [NotNull]string lastName, BirthDate birthDate)
			: this()
		{
			this.Id = id;
			this.Age = age;
			this.FirstName = firstName;
			this.LastName = lastName;
			this.BirthDate = birthDate;
		}

		public BirthDate BirthDate { get; set; }
	}
}
