using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

static class ObjectCache
{

	private static Dictionary<string, List<Object>> cache;
	private static Dictionary<string, List<Object>> Cache
	{
		get
		{
			if (cache == null)
				cache = new Dictionary<string, List<Object>>();
			return cache;
		}
	}

	/// <summary>
	/// Gets the component of an object (typically one that will stay in the scene for a long time), will be cached.
	/// If the object is dead or the component doesn't exist, returns null.
	/// </summary>
	/// <typeparam name="T">The component's Type</typeparam>
	/// <param name="name">Name of the object</param>
	/// <returns>The component or null</returns>
	public static T Get<T>(string name) where T:Component
	{
		if (Cache.ContainsKey(name))
		{
			GameObject go = Cache[name][0] as GameObject;
			if (go == null)
			{
				// gameobject is probs destroyed, we remove the list
				Cache.Remove(name);
				return null;
			}

			List<Object> list = Cache[name];
			foreach (Object c in list)
			{
				if (c is T)
				{
					return c as T;
				}
			}

			T component = go.GetComponent<T>();
			if (component == null)
				return null;

			list.Add(component);
			return component;
		}
		else
		{
			GameObject go = Get(name);
			if (go == null)
				return null;

			T component = go.GetComponent<T>();
			if (component == null)
				return null;

			Cache[name].Add(component);
			return component;
		}
	}

	/// <summary>
	/// Gets a GameObject with the given name (typically one that will stay in the scene for a long time), will be cached.
	/// If the GameObject is dead or doesn't exist, returns null.
	/// </summary>
	/// <param name="name">Name of the GameObject</param>
	/// <returns>The GameObject, or null</returns>
	public static GameObject Get(string name)
	{
		if (Cache.ContainsKey(name))
		{
			GameObject go = Cache[name][0] as GameObject;
			if (go == null)
			{
				// gameobject is probs destroyed, we remove the list
				Cache.Remove(name);
				return null;
			}
			return go;
		}
		else
		{
			GameObject go = GameObject.Find(name);
			if (go == null)
				return null;
			Cache.Add(name, new List<Object>() { go });
			return go;
		}
	}

	/// <summary>
	/// Clears the cache. Recommended to do this every scene change.
	/// </summary>
	public static void Clear()
	{
		Cache.Clear();
	}

}