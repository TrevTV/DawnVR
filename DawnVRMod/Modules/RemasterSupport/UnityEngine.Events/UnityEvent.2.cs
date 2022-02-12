using System;
using System.Reflection;
using UnityEngine.Scripting;
using UnityEngineInternal;

namespace DawnVR.Events
{
	/// <summary>
	///   <para>One argument version of UnityEvent.</para>
	/// </summary>
	// Token: 0x02000390 RID: 912
	[Serializable]
	public abstract class UnityEvent<T0> : UnityEventBase
	{
		// Token: 0x06002F42 RID: 12098 RVA: 0x0004C450 File Offset: 0x0004A650
		public UnityEvent()
		{
		}

		// Token: 0x06002F43 RID: 12099 RVA: 0x0004C468 File Offset: 0x0004A668
		public void AddListener(UnityAction<T0> call)
		{
			base.AddCall(UnityEvent<T0>.GetDelegate(call));
		}

		// Token: 0x06002F44 RID: 12100 RVA: 0x0004C478 File Offset: 0x0004A678
		public void RemoveListener(UnityAction<T0> call)
		{
			base.RemoveListener(call.Target, call.GetMethodInfo());
		}

		// Token: 0x06002F45 RID: 12101 RVA: 0x0004C490 File Offset: 0x0004A690
		protected override MethodInfo FindMethod_Impl(string name, object targetObj)
		{
			return UnityEventBase.GetValidMethodInfo(targetObj, name, new Type[] { typeof(T0) });
		}

		// Token: 0x06002F46 RID: 12102 RVA: 0x0004C4C0 File Offset: 0x0004A6C0
		internal override BaseInvokableCall GetDelegate(object target, MethodInfo theFunction)
		{
			return new InvokableCall<T0>(target, theFunction);
		}

		// Token: 0x06002F47 RID: 12103 RVA: 0x0004C4DC File Offset: 0x0004A6DC
		private static BaseInvokableCall GetDelegate(UnityAction<T0> action)
		{
			return new InvokableCall<T0>(action);
		}

		// Token: 0x06002F48 RID: 12104 RVA: 0x0004C4F8 File Offset: 0x0004A6F8
		public void Invoke(T0 arg0)
		{
			this.m_InvokeArray[0] = arg0;
			base.Invoke(this.m_InvokeArray);
		}

		// Token: 0x04000D4D RID: 3405
		private readonly object[] m_InvokeArray = new object[1];
	}
}
