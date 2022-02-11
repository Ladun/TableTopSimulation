using System;
using System.Collections.Generic;
using System.Text;

namespace ServerCore
{
	public class Singleton<T> where T : new()
	{

		static T _instance = new T();
		public static T Instance { get { return _instance; } }
	}
}