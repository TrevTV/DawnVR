using MonoMod.Utils;
using System;
using System.Reflection;
using System.Threading;
using UnityEngine;
using UnityEngineInternal;

namespace DawnVR.Events
{
	// Token: 0x02000382 RID: 898
	internal class InvokableCall : BaseInvokableCall
	{
		// Token: 0x06002ED8 RID: 11992 RVA: 0x0004B0E4 File Offset: 0x000492E4
		public InvokableCall(object target, MethodInfo theFunction)
			: base(target, theFunction)
		{
			this.Delegate += (UnityAction)theFunction.CreateDelegate(typeof(UnityAction), target);
		}

		// Token: 0x06002ED9 RID: 11993 RVA: 0x0004B10C File Offset: 0x0004930C
		public InvokableCall(UnityAction action)
		{
			this.Delegate += action;
		}

		// Token: 0x06002EDC RID: 11996 RVA: 0x0004B18C File Offset: 0x0004938C
		public override void Invoke(object[] args)
		{
			if (BaseInvokableCall.AllowInvoke(this.Delegate))
			{
				this.Delegate();
			}
		}

		// Token: 0x06002EDD RID: 11997 RVA: 0x0004B1AC File Offset: 0x000493AC
		public override bool Find(object targetObj, MethodInfo method)
		{
			return this.Delegate.Target == targetObj && this.Delegate.GetMethodInfo().Equals(method);
		}

		// Token: 0x04000D34 RID: 3380
		private UnityAction Delegate;
	}
}
