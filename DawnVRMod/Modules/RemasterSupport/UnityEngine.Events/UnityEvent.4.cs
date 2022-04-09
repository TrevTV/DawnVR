using System;
using System.Reflection;
using UnityEngine;
using UnityEngine.Scripting;
using UnityEngineInternal;

namespace DawnVR.Events
{
	/// <summary>
	///   <para>Three argument version of UnityEvent.</para>
	/// </summary>
	// Token: 0x02000394 RID: 916
	[Serializable]
	public abstract class UnityEvent<T0, T1, T2> : UnityEventBase
	{
		// Token: 0x06002F58 RID: 12120 RVA: 0x0004C5F8 File Offset: 0x0004A7F8
		public UnityEvent()
		{
		}

		// Token: 0x06002F59 RID: 12121 RVA: 0x0004C610 File Offset: 0x0004A810
		public void AddListener(UnityAction<T0, T1, T2> call)
		{
			base.AddCall(UnityEvent<T0, T1, T2>.GetDelegate(call));
		}

		// Token: 0x06002F5A RID: 12122 RVA: 0x0004C620 File Offset: 0x0004A820
		public void RemoveListener(UnityAction<T0, T1, T2> call)
		{
			base.RemoveListener(call.Target, call.GetMethodInfo());
		}

		// Token: 0x06002F5B RID: 12123 RVA: 0x0004C638 File Offset: 0x0004A838
		protected override MethodInfo FindMethod_Impl(string name, object targetObj)
		{
			return UnityEventBase.GetValidMethodInfo(targetObj, name, new Type[]
			{
				typeof(T0),
				typeof(T1),
				typeof(T2)
			});
		}

		// Token: 0x06002F5C RID: 12124 RVA: 0x0004C684 File Offset: 0x0004A884
		internal override BaseInvokableCall GetDelegate(object target, MethodInfo theFunction)
		{
			return new InvokableCall<T0, T1, T2>(target, theFunction);
		}

		// Token: 0x06002F5D RID: 12125 RVA: 0x0004C6A0 File Offset: 0x0004A8A0
		private static BaseInvokableCall GetDelegate(UnityAction<T0, T1, T2> action)
		{
			return new InvokableCall<T0, T1, T2>(action);
		}

		// Token: 0x06002F5E RID: 12126 RVA: 0x0004C6BC File Offset: 0x0004A8BC
		public void Invoke(T0 arg0, T1 arg1, T2 arg2)
		{
			this.m_InvokeArray[0] = arg0;
			this.m_InvokeArray[1] = arg1;
			this.m_InvokeArray[2] = arg2;
			base.Invoke(this.m_InvokeArray);
		}

		// Token: 0x04000D4F RID: 3407
		private readonly object[] m_InvokeArray = new object[3];
	}
}
