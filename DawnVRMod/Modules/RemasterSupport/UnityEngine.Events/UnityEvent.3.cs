using System;
using System.Reflection;
using UnityEngine.Scripting;
using UnityEngineInternal;

namespace DawnVR.Events
{
	/// <summary>
	///   <para>Two argument version of UnityEvent.</para>
	/// </summary>
	// Token: 0x02000392 RID: 914
	[Serializable]
	public abstract class UnityEvent<T0, T1> : UnityEventBase
	{
		// Token: 0x06002F4D RID: 12109 RVA: 0x0004C518 File Offset: 0x0004A718
		public UnityEvent()
		{
		}

		// Token: 0x06002F4E RID: 12110 RVA: 0x0004C530 File Offset: 0x0004A730
		public void AddListener(UnityAction<T0, T1> call)
		{
			base.AddCall(UnityEvent<T0, T1>.GetDelegate(call));
		}

		// Token: 0x06002F4F RID: 12111 RVA: 0x0004C540 File Offset: 0x0004A740
		public void RemoveListener(UnityAction<T0, T1> call)
		{
			base.RemoveListener(call.Target, call.GetMethodInfo());
		}

		// Token: 0x06002F50 RID: 12112 RVA: 0x0004C558 File Offset: 0x0004A758
		protected override MethodInfo FindMethod_Impl(string name, object targetObj)
		{
			return UnityEventBase.GetValidMethodInfo(targetObj, name, new Type[]
			{
				typeof(T0),
				typeof(T1)
			});
		}

		// Token: 0x06002F51 RID: 12113 RVA: 0x0004C594 File Offset: 0x0004A794
		internal override BaseInvokableCall GetDelegate(object target, MethodInfo theFunction)
		{
			return new InvokableCall<T0, T1>(target, theFunction);
		}

		// Token: 0x06002F52 RID: 12114 RVA: 0x0004C5B0 File Offset: 0x0004A7B0
		private static BaseInvokableCall GetDelegate(UnityAction<T0, T1> action)
		{
			return new InvokableCall<T0, T1>(action);
		}

		// Token: 0x06002F53 RID: 12115 RVA: 0x0004C5CC File Offset: 0x0004A7CC
		public void Invoke(T0 arg0, T1 arg1)
		{
			this.m_InvokeArray[0] = arg0;
			this.m_InvokeArray[1] = arg1;
			base.Invoke(this.m_InvokeArray);
		}

		// Token: 0x04000D4E RID: 3406
		private readonly object[] m_InvokeArray = new object[2];
	}
}
