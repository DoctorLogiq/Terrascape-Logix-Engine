using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace LogixEngine.Utility
{
	/* ---------------------------------------------------------------------------- *
     * StringUtils.cs created by DrLogiq on 19-01-2020 19:40.
     * Copyright © DrLogiq. All rights reserved.
     * ---------------------------------------------------------------------------- */
	public static class StringUtils
	{
		// NOTE(LOGIQ): Thanks to StackOverflow user 'Enigmativity' for this function; https://stackoverflow.com/a/22368809/11878570
		[SuppressMessage("ReSharper", "PossibleMultipleEnumeration")]
		public static IEnumerable<string> SplitToLines(string string_to_split, int maximum_line_length)
		{
			IEnumerable<string> words = string_to_split.Split(' ').Concat(new [] { "" });
			return
				words
					.Skip(1)
					.Aggregate(
						words.Take(1).ToList(),
						(a, w) =>
						{
							var last = a.Last();
							while (last.Length > maximum_line_length)
							{
								a[a.Count() - 1] = last.Substring(0, maximum_line_length);
								last             = last.Substring(maximum_line_length);
								a.Add(last);
							}
							var test = last + " " + w;
							if (test.Length > maximum_line_length)
							{
								a.Add(w);
							}
							else
							{
								a[a.Count() - 1] = test;
							}
							return a;
						});
		}
	}
}