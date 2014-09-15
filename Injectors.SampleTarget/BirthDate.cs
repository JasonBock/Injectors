using System;

namespace Injectors.SampleTarget
{
	public struct BirthDate
	{
		public BirthDate(DateTime value)
			: this()
		{
			this.Value = value;
		}

		public override string ToString()
		{
			return this.Value.ToString();
		}

		public DateTime Value { get; private set; }
	}
}
