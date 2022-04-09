using MonoMod.Utils;
using System;
using System.Reflection;
using System.Threading;
using UnityEngine;
using UnityEngineInternal;

namespace DawnVR.Events
{
	// Token: 0x02000384 RID: 900
	internal class InvokableCall<T1, T2> : BaseInvokableCall
	{
		// Token: 0x06002EE4 RID: 12004 RVA: 0x0004B31C File Offset: 0x0004951C
		public InvokableCall(object target, MethodInfo theFunction)
			: base(target, theFunction)
		{
			this.Delegate = (UnityAction<T1, T2>)theFunction.CreateDelegate(typeof(UnityAction<T1, T2>), target);
		}

		// Token: 0x06002EE5 RID: 12005 RVA: 0x0004B344 File Offset: 0x00049544
		public InvokableCall(UnityAction<T1, T2> action)
		{
			this.Delegate += action;
		}

		// Token: 0x06002EE8 RID: 12008 RVA: 0x0004B3C4 File Offset: 0x000495C4
		public override void Invoke(object[] args)
		{
			if (args.Length != 2)
			{
				throw new ArgumentException("Passed argument 'args' is invalid size. Expected size is 1");
			}
			BaseInvokableCall.ThrowOnInvalidArg<T1>(args[0]);
			BaseInvokableCall.ThrowOnInvalidArg<T2>(args[1]);
			if (BaseInvokableCall.AllowInvoke(this.Delegate))
			{
				this.Delegate((T1)((object)args[0]), (T2)((object)args[1]));
			}
		}

		// Token: 0x06002EE9 RID: 12009 RVA: 0x0004B424 File Offset: 0x00049624
		public override bool Find(object targetObj, MethodInfo method)
		{
			return this.Delegate.Target == targetObj && this.Delegate.GetMethodInfo().Equals(method);
		}

		// Token: 0x04000D36 RID: 3382
		private UnityAction<T1, T2> Delegate;
	}
}
