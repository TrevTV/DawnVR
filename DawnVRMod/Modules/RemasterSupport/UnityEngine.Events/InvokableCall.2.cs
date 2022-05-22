using MonoMod.Utils;
using System;
using System.Reflection;
using UnityEngine;

namespace DawnVR.Events
{
	// Token: 0x02000383 RID: 899
	internal class InvokableCall<T1> : BaseInvokableCall
	{
		// Token: 0x06002EDE RID: 11998 RVA: 0x0004B1E8 File Offset: 0x000493E8
		public InvokableCall(object target, MethodInfo theFunction)
			: base(target, theFunction)
		{
			this.Delegate += (UnityAction<T1>)theFunction.CreateDelegate(typeof(UnityAction<T1>), target);
		}

		// Token: 0x06002EDF RID: 11999 RVA: 0x0004B210 File Offset: 0x00049410
		public InvokableCall(UnityAction<T1> action)
		{
			this.Delegate += action;
		}

		// Token: 0x06002EE2 RID: 12002 RVA: 0x0004B290 File Offset: 0x00049490
		public override void Invoke(object[] args)
		{
			if (args.Length != 1)
			{
				throw new ArgumentException("Passed argument 'args' is invalid size. Expected size is 1");
			}
			BaseInvokableCall.ThrowOnInvalidArg<T1>(args[0]);
			if (BaseInvokableCall.AllowInvoke(this.Delegate))
			{
				this.Delegate((T1)((object)args[0]));
			}
		}

		// Token: 0x06002EE3 RID: 12003 RVA: 0x0004B2E0 File Offset: 0x000494E0
		public override bool Find(object targetObj, MethodInfo method)
		{
			return this.Delegate.Target == targetObj && this.Delegate.GetMethodInfo().Equals(method);
		}

		// Token: 0x04000D35 RID: 3381
		private UnityAction<T1> Delegate;
	}
}
