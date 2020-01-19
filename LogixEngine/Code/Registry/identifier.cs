using System;
using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

namespace LogixEngine.Registry
{
	/* ---------------------------------------------------------------------------- *
     * identifier.cs created by DrLogiq on 18-01-2020 14:01.
     * Copyright © DrLogiq. All rights reserved.
     * ---------------------------------------------------------------------------- */
	[SuppressMessage("ReSharper", "InconsistentNaming")]
	public struct identifier
	{
		private static readonly Regex Regex = new Regex(@"^[a-z]{3}[a-z_0-9]{3,}$");
		
		// RegExprs for identifying failures
		private static readonly Regex RegexPart1 = new Regex(@"[a-z]{3}");
		private static readonly Regex RegexPart2 = new Regex(@"[a-z_0-9]{3,}");
		
		private readonly string value;

		public identifier(string value)
		{
			if (string.IsNullOrEmpty(value) || value.Length < 6)
			{
				throw new InvalidIdentifierException(value, "identifiers must consist of a minimum of 6 characters");
			}
			
			if (Regex.IsMatch(value))
			{
				this.value = value;
				return;
			}
			
			if (!RegexPart1.IsMatch(value.Substring(0, 3)))
			{
				throw new InvalidIdentifierException(value, "the first 3 characters of an identifier must consist of lower-case letters only");
			}

			if (!RegexPart2.IsMatch(value.Substring(3)))
			{
				throw new InvalidIdentifierException(value, "all characters after the first 3 must consist of lower-case letters; numbers and/or underscores");
			}
			
			throw new InvalidIdentifierException(value, "unknown error"); // NOTE(LOGIQ): This line should never be hit. TODO(LOGIQ): Test rigorously
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

	public sealed class InvalidIdentifierException : ApplicationException
	{
		public InvalidIdentifierException(string identifier, string reason)
			: base($"Invalid identifier '{identifier}'; {reason}")
		{
		}
	}
}