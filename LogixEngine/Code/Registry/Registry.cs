using LogixEngine.Utility;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace LogixEngine.Registry
{
	/* ---------------------------------------------------------------------------- *
     * Registry.cs created by DrLogiq on 18-01-2020 14:11.
     * Copyright © DrLogiq. All rights reserved.
     * ---------------------------------------------------------------------------- */
	[SuppressMessage("ReSharper", "MemberCanBeProtected.Global")]
	public class Registry<T> where T : class, IIdentifiable
	{
		private readonly Dictionary<identifier, T> values = new Dictionary<identifier, T>();
		private readonly string type;

		public Registry()
		{
			type = typeof(T).Name;
			Debug.LogDebug($"Created a {type} registry");
		}

		public T Register(T item)
		{
			return RegisterItem(item);
		}

		protected T RegisterItem(T item)
		{
			if (values.ContainsKey(item.Identifier))
				throw new IdentifierNotUniqueException(item.Identifier, type);
			
			values.Add(item.Identifier, item);
			Debug.LogDebug($"Registered {type} '{item.Identifier}' to the {type} registry");
			return item;
		}

		public T Find(identifier id, bool throw_if_not_found = true)
		{
			return FindItem(id, throw_if_not_found);
		}

		protected T FindItem(identifier id, bool throw_if_not_found = true)
		{
			if (values.ContainsKey(id))
				return values[id];

			if (throw_if_not_found)
				throw  new RegistryEntryNotFoundException(id, type);
			
			return null;
		}
	}

	public sealed class IdentifierNotUniqueException : ApplicationException
	{
		public IdentifierNotUniqueException(identifier identifier, string registry_name) 
			: base($"The identifier '{identifier}' is not unique in the {registry_name} registry.")
		{
		}
	}
	
	public sealed class RegistryEntryNotFoundException : ApplicationException
	{
		public RegistryEntryNotFoundException(identifier identifier, string registry_name) 
			: base($"An item with the identifier '{identifier}' could not be found in the {registry_name} registry.")
		{
		}
	}
}