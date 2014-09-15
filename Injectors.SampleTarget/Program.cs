using System;
using System.Diagnostics;

namespace Injectors.SampleTarget
{
	class Program
	{
		static void Main(string[] args)
		{
			Debugger.Break();
			Program.CreateCustomer(
				Guid.NewGuid(), 22, "Joe", "Smith",
				new BirthDate(new DateTime(1980, 1, 2)));
			Console.Out.WriteLine();
			Program.CreateCustomer(
				Guid.NewGuid(), 22, null, "Smith",
				new BirthDate(new DateTime(1980, 1, 2)));
			Console.Out.WriteLine();
			Program.CreateCustomerWithDefinedToString(
				Guid.NewGuid(), 22, "Joe", "Smith",
				new BirthDate(new DateTime(1980, 1, 2)));
			Console.Out.WriteLine("Press any key to continue ...");
			Console.In.ReadLine();
		}

		private static void CreateCustomer(Guid id, int age, string firstName, string lastName, BirthDate birthDate)
		{
			try
			{
				Console.Out.WriteLine(new AttributedCustomer(
					id, age, firstName, lastName, birthDate).ToString());
			}
			catch (ArgumentNullException)
			{
				Console.Out.WriteLine("ArgumentNullException");
			}
		}

		private static void CreateCustomerWithDefinedToString(Guid id, int age, string firstName, string lastName, BirthDate birthDate)
		{
			try
			{
				Console.Out.WriteLine(new AttributedCustomerWithDefinedToString(
					id, age, firstName, lastName, birthDate).ToString());
			}
			catch (ArgumentNullException)
			{
				Console.Out.WriteLine("ArgumentNullException");
			}
		}
	}
}
