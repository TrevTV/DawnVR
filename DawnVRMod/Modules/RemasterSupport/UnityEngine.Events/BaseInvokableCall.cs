using System;
using System.Reflection;

namespace DawnVR.Events
{
	// Token: 0x02000381 RID: 897
	internal abstract class BaseInvokableCall
	{
		// Token: 0x06002ED2 RID: 11986 RVA: 0x0004B020 File Offset: 0x00049220
		protected BaseInvokableCall()
		{
		}

		// Token: 0x06002ED3 RID: 11987 RVA: 0x0004B02C File Offset: 0x0004922C
		protected BaseInvokableCall(object target, MethodInfo function)
		{
			if (target == null)
			{
				throw new ArgumentNullException("target");
			}
			if (function == null)
			{
				throw new ArgumentNullException("function");
			}
		}

		// Token: 0x06002ED4 RID: 11988
		public abstract void Invoke(object[] args);

		// Token: 0x06002ED5 RID: 11989 RVA: 0x0004B058 File Offset: 0x00049258
		protected static void ThrowOnInvalidArg<T>(object arg)
		{
			if (arg != null && !(arg is T))
			{
				throw new ArgumentException(string.Format("Passed argument 'args[0]' is of the wrong type. Type:{0} Expected:{1}", new object[]
				{
					arg.GetType(),
					typeof(T)
				}));
			}
		}

		// Token: 0x06002ED6 RID: 11990 RVA: 0x0004B098 File Offset: 0x00049298
		protected static bool AllowInvoke(Delegate @delegate)
		{
			object target = @delegate.Target;
			bool result;
			if (target == null)
			{
				result = true;
			}
			else
			{
				Object @object = target as Object;
				result = object.ReferenceEquals(@object, null) || @object != null;
			}
			return result;
		}

		// Token: 0x06002ED7 RID: 11991
		public abstract bool Find(object targetObj, MethodInfo method);
	}
}
