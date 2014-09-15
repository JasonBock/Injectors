using System;
using System.Reflection;
using System.Text;

namespace Injectors.SampleTarget
{
	public sealed class VerboseCustomers : Customer
	{
		private const string Separator = " || ";

		private VerboseCustomers()
			: base() { }

		public VerboseCustomers(Guid id, int age, string firstName, string lastName)
			: this()
		{
			var currentMethod = MethodBase.GetCurrentMethod().ToString();
			Console.Out.WriteLine(currentMethod + " started");

			if(firstName == null)
			{
				Console.WriteLine(currentMethod + " - exception was thrown");
				throw new ArgumentNullException("firstName");
			}

			if(lastName == null)
			{
				Console.WriteLine(currentMethod + " - exception was thrown");
				throw new ArgumentNullException("lastName");
			}

			this.Id = id;
			this.Age = age;
			this.FirstName = FirstName;
			this.LastName = lastName;

			Console.WriteLine(currentMethod + " finished");
		}

		public override string ToString()
		{
			return new StringBuilder()
				.Append("Age: ").Append(this.Age)
				.Append(VerboseCustomers.Separator)
				.Append("Id: ").Append(this.Id)
				.Append(VerboseCustomers.Separator)
				.Append("LastName: ").Append(this.LastName)
				.Append(VerboseCustomers.Separator)
				.Append("FirstName: ").Append(this.FirstName).ToString();
		}
	}
}
