using Injectors.Core.Attributes;
using System;

namespace Injectors.SampleTarget
{
	[ToString]
	public sealed class AttributedCustomerWithDefinedToString : Customer
	{
		private AttributedCustomerWithDefinedToString()
			: base() { }

		[Trace]
		public AttributedCustomerWithDefinedToString(Guid id, [NotNull]int age,
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

		public override string ToString()
		{
			return "Hey! Don't change me!";
		}
	}
}
