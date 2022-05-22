using System;
using System.Reflection;

namespace DawnVR.Events
{
	// Token: 0x02000387 RID: 903
	internal class CachedInvokableCall<T> : InvokableCall<T>
	{
		// Token: 0x06002EF6 RID: 12022 RVA: 0x0004B718 File Offset: 0x00049918
		public CachedInvokableCall(UnityEngine.Object target, MethodInfo theFunction, T argument)
			: base(target, theFunction)
		{
			this.m_Arg1[0] = argument;
		}

		// Token: 0x06002EF7 RID: 12023 RVA: 0x0004B740 File Offset: 0x00049940
		public override void Invoke(object[] args)
		{
			base.Invoke(this.m_Arg1);
		}

		// Token: 0x04000D39 RID: 3385
		private readonly object[] m_Arg1 = new object[1];
	}
}
