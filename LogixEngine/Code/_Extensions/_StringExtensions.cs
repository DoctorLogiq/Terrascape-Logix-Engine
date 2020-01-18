using System;
using System.Diagnostics.CodeAnalysis;

namespace LogixEngine._Extensions
{
	/* ----------------------------------------------------------------------------  *
     * StringExtensions.cs created by DrLogiq on 17-01-2020 10:23.
     * Copyright © DrLogiq. All rights reserved.
     * ----------------------------------------------------------------------------  */
	[SuppressMessage("ReSharper", "InconsistentNaming")]
	// NOTE(LOGIQ): This class is not indended to be called into, so the name begins with a discard to signify this.
	public static class _StringExtensions
	{
		/// <summary>
		/// Returns a string where the given character is repeated [count] times.
		/// <para>
		/// For example, if the character is 'T' and the count is 4, the returned string would be "TTTT".
		/// </para>
		/// </summary>
		/// <param name="character">The character to repeat in the string</param>
		/// <param name="count">The number of times to repeat the given [character]</param>
		/// <returns>a string where the given character is repeated [count] times</returns>
		/// <exception cref="ApplicationException">If the count is less than 1</exception>
		public static string Times(this char character, int count)
		{
			if (count < 1)
				throw new ApplicationException($"Cannot create a string with a character written {count} times!");
			
			string str = "";
			for (int i = 0; i < count; ++i)
			{
				str += character;
			}

			return str;
		}
	}
}