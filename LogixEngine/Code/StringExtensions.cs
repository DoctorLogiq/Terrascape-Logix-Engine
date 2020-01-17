namespace LogixEngine
{
	/* ----------------------------------------------------------------------------  *
     * StringExtensions.cs created by DrLogiq on 17-01-2020 10:23.
     * Copyright © DrLogiq. All rights reserved.
     * ----------------------------------------------------------------------------  */
	public static class StringExtensions
	{
		public static string Times(this char character, int count)
		{
			string str = "";
			for (int i = 0; i < count; ++i)
			{
				str += character;
			}

			return str;
		}
	}
}