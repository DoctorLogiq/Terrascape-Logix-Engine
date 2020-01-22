using System;

namespace LogixEngine.Utility
{
	/* ---------------------------------------------------------------------------- *
     * DeveloperFailureException.cs created by DrLogiq on 22-01-2020 13:52.
     * Copyright © DrLogiq. All rights reserved.
     * ---------------------------------------------------------------------------- */
	public class DeveloperFailureException : ApplicationException
	{
		public DeveloperFailureException(string message) : base(message)
		{
		}
		
		public DeveloperFailureException(string message, Exception cause) : base(message, cause)
		{
		}
	}
}