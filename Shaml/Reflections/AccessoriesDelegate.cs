﻿namespace Shaml.Reflections
{
	public delegate object GetterInstance();
	public delegate object GetterDelegate(object instance);
	public delegate void SetterDelegate(object instance, object value);
}
