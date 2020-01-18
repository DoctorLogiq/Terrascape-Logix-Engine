using System.Diagnostics.CodeAnalysis;

namespace LogixEngine.Registry
{
	/* ---------------------------------------------------------------------------- *
     * identifier.cs created by DrLogiq on 18-01-2020 14:01.
     * Copyright © DrLogiq. All rights reserved.
     * ---------------------------------------------------------------------------- */
	[SuppressMessage("ReSharper", "InconsistentNaming")]
	public struct identifier
	{
		private readonly string value;

		public identifier(string value)
		{
			this.value = value;
			// TODO(LOGIQ): Validate
		}

		public static implicit operator identifier(string value)
		{
			return new identifier(value);
		}

		public static implicit operator string(identifier value)
		{
			return value.value;
		}

		public override string ToString()
		{
			return value;
		}
	}
}