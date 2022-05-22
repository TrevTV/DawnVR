using MonoMod.Utils;
using System;
using System.Reflection;
using System.Threading;
using UnityEngine;
using UnityEngineInternal;

namespace DawnVR.Events
{
	// Token: 0x02000385 RID: 901
	internal class InvokableCall<T1, T2, T3> : BaseInvokableCall
	{
		// Token: 0x06002EEA RID: 12010 RVA: 0x0004B460 File Offset: 0x00049660
		public InvokableCall(object target, MethodInfo theFunction)
			: base(target, theFunction)
		{
			this.Delegate = (UnityAction<T1, T2, T3>)theFunction.CreateDelegate(typeof(UnityAction<T1, T2, T3>), target);
		}

		// Token: 0x06002EEB RID: 12011 RVA: 0x0004B488 File Offset: 0x00049688
		public InvokableCall(UnityAction<T1, T2, T3> action)
		{
			this.Delegate += action;
		}

		// Token: 0x06002EEE RID: 12014 RVA: 0x0004B508 File Offset: 0x00049708
		public override void Invoke(object[] args)
		{
			if (args.Length != 3)
			{
				throw new ArgumentException("Passed argument 'args' is invalid size. Expected size is 1");
			}
			BaseInvokableCall.ThrowOnInvalidArg<T1>(args[0]);
			BaseInvokableCall.ThrowOnInvalidArg<T2>(args[1]);
			BaseInvokableCall.ThrowOnInvalidArg<T3>(args[2]);
			if (BaseInvokableCall.AllowInvoke(this.Delegate))
			{
				this.Delegate((T1)((object)args[0]), (T2)((object)args[1]), (T3)((object)args[2]));
			}
		}

		// Token: 0x06002EEF RID: 12015 RVA: 0x0004B578 File Offset: 0x00049778
		public override bool Find(object targetObj, MethodInfo method)
		{
			return this.Delegate.Target == targetObj && this.Delegate.GetMethodInfo().Equals(method);
		}

		// Token: 0x04000D37 RID: 3383
		private UnityAction<T1, T2, T3> Delegate;
	}
}
