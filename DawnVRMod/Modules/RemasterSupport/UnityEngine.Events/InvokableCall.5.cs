using MonoMod.Utils;
using System;
using System.Reflection;
using System.Threading;
using UnityEngine;
using UnityEngineInternal;

namespace DawnVR.Events
{
	// Token: 0x02000386 RID: 902
	internal class InvokableCall<T1, T2, T3, T4> : BaseInvokableCall
	{
		// Token: 0x06002EF0 RID: 12016 RVA: 0x0004B5B4 File Offset: 0x000497B4
		public InvokableCall(object target, MethodInfo theFunction)
			: base(target, theFunction)
		{
			this.Delegate = (UnityAction<T1, T2, T3, T4>)theFunction.CreateDelegate(typeof(UnityAction<T1, T2, T3, T4>), target);
		}

		// Token: 0x06002EF1 RID: 12017 RVA: 0x0004B5DC File Offset: 0x000497DC
		public InvokableCall(UnityAction<T1, T2, T3, T4> action)
		{
			this.Delegate += action;
		}

		// Token: 0x06002EF4 RID: 12020 RVA: 0x0004B65C File Offset: 0x0004985C
		public override void Invoke(object[] args)
		{
			if (args.Length != 4)
			{
				throw new ArgumentException("Passed argument 'args' is invalid size. Expected size is 1");
			}
			BaseInvokableCall.ThrowOnInvalidArg<T1>(args[0]);
			BaseInvokableCall.ThrowOnInvalidArg<T2>(args[1]);
			BaseInvokableCall.ThrowOnInvalidArg<T3>(args[2]);
			BaseInvokableCall.ThrowOnInvalidArg<T4>(args[3]);
			if (BaseInvokableCall.AllowInvoke(this.Delegate))
			{
				this.Delegate((T1)((object)args[0]), (T2)((object)args[1]), (T3)((object)args[2]), (T4)((object)args[3]));
			}
		}

		// Token: 0x06002EF5 RID: 12021 RVA: 0x0004B6DC File Offset: 0x000498DC
		public override bool Find(object targetObj, MethodInfo method)
		{
			return this.Delegate.Target == targetObj && this.Delegate.GetMethodInfo().Equals(method);
		}

		// Token: 0x04000D38 RID: 3384
		private UnityAction<T1, T2, T3, T4> Delegate;
	}
}
