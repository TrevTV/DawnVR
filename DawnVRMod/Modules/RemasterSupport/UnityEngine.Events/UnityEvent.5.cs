using System;
using System.Reflection;
using UnityEngine;
using UnityEngine.Scripting;
using UnityEngineInternal;

namespace DawnVR.Events
{
	/// <summary>
	///   <para>Four argument version of UnityEvent.</para>
	/// </summary>
	// Token: 0x02000396 RID: 918
	[Serializable]
	public abstract class UnityEvent<T0, T1, T2, T3> : UnityEventBase
	{
		// Token: 0x06002F63 RID: 12131 RVA: 0x0004C6F8 File Offset: 0x0004A8F8
		public UnityEvent()
		{
		}

		// Token: 0x06002F64 RID: 12132 RVA: 0x0004C710 File Offset: 0x0004A910
		public void AddListener(UnityAction<T0, T1, T2, T3> call)
		{
			base.AddCall(UnityEvent<T0, T1, T2, T3>.GetDelegate(call));
		}

		// Token: 0x06002F65 RID: 12133 RVA: 0x0004C720 File Offset: 0x0004A920
		public void RemoveListener(UnityAction<T0, T1, T2, T3> call)
		{
			base.RemoveListener(call.Target, call.GetMethodInfo());
		}

		// Token: 0x06002F66 RID: 12134 RVA: 0x0004C738 File Offset: 0x0004A938
		protected override MethodInfo FindMethod_Impl(string name, object targetObj)
		{
			return UnityEventBase.GetValidMethodInfo(targetObj, name, new Type[]
			{
				typeof(T0),
				typeof(T1),
				typeof(T2),
				typeof(T3)
			});
		}

		// Token: 0x06002F67 RID: 12135 RVA: 0x0004C790 File Offset: 0x0004A990
		internal override BaseInvokableCall GetDelegate(object target, MethodInfo theFunction)
		{
			return new InvokableCall<T0, T1, T2, T3>(target, theFunction);
		}

		// Token: 0x06002F68 RID: 12136 RVA: 0x0004C7AC File Offset: 0x0004A9AC
		private static BaseInvokableCall GetDelegate(UnityAction<T0, T1, T2, T3> action)
		{
			return new InvokableCall<T0, T1, T2, T3>(action);
		}

		// Token: 0x06002F69 RID: 12137 RVA: 0x0004C7C8 File Offset: 0x0004A9C8
		public void Invoke(T0 arg0, T1 arg1, T2 arg2, T3 arg3)
		{
			this.m_InvokeArray[0] = arg0;
			this.m_InvokeArray[1] = arg1;
			this.m_InvokeArray[2] = arg2;
			this.m_InvokeArray[3] = arg3;
			base.Invoke(this.m_InvokeArray);
		}

		// Token: 0x04000D50 RID: 3408
		private readonly object[] m_InvokeArray = new object[4];
	}
}
