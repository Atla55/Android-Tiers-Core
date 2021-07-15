using System;
using System.Reflection;

namespace Androids
{
	// Token: 0x02000020 RID: 32
	public static class ReflectionUtility
	{
		// Token: 0x06000078 RID: 120 RVA: 0x000052CC File Offset: 0x000034CC
		public static object CloneObjectShallowly(this object sourceObject)
		{
			bool flag = sourceObject == null;
			object result;
			if (flag)
			{
				result = null;
			}
			else
			{
				Type type = sourceObject.GetType();
				bool isAbstract = type.IsAbstract;
				if (isAbstract)
				{
					result = null;
				}
				else
				{
					bool flag2 = type.IsPrimitive || type.IsValueType || type.IsArray || type == typeof(string);
					if (flag2)
					{
						result = sourceObject;
					}
					else
					{
						object obj = Activator.CreateInstance(type);
						bool flag3 = obj == null;
						if (flag3)
						{
							result = null;
						}
						else
						{
							foreach (FieldInfo fieldInfo in type.GetFields())
							{
								bool flag4 = !fieldInfo.IsLiteral;
								if (flag4)
								{
									object value = fieldInfo.GetValue(sourceObject);
									fieldInfo.SetValue(obj, value);
								}
							}
							result = obj;
						}
					}
				}
			}
			return result;
		}
	}
}
