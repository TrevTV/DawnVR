using System;
using System.Reflection;
using UnityEngine.Scripting;
using UnityEngineInternal;

namespace DawnVR.Events
{
	/// <summary>
	///   <para>A zero argument persistent callback that can be saved with the scene.</para>
	/// </summary>
	// Token: 0x0200038E RID: 910
	[Serializable]
	public class UnityEvent : UnityEventBase
	{
		/// <summary>
		///   <para>Constructor.</para>
		/// </summary>
		// Token: 0x06002F37 RID: 12087 RVA: 0x0004C3A4 File Offset: 0x0004A5A4
		public UnityEvent()
		{
		}

		/// <summary>
		///   <para>Add a non persistent listener to the UnityEvent.</para>
		/// </summary>
		/// <param name="call">Callback function.</param>
		// Token: 0x06002F38 RID: 12088 RVA: 0x0004C3BC File Offset: 0x0004A5BC
		public void AddListener(UnityAction call)
		{
			base.AddCall(UnityEvent.GetDelegate(call));
		}

		/// <summary>
		///   <para>Remove a non persistent listener from the UnityEvent.</para>
		/// </summary>
		/// <param name="call">Callback function.</param>
		// Token: 0x06002F39 RID: 12089 RVA: 0x0004C3CC File Offset: 0x0004A5CC
		public void RemoveListener(UnityAction call)
		{
			base.RemoveListener(call.Target, call.GetMethodInfo());
		}

		// Token: 0x06002F3A RID: 12090 RVA: 0x0004C3E4 File Offset: 0x0004A5E4
		protected override MethodInfo FindMethod_Impl(string name, object targetObj)
		{
			return UnityEventBase.GetValidMethodInfo(targetObj, name, new Type[0]);
		}

		// Token: 0x06002F3B RID: 12091 RVA: 0x0004C408 File Offset: 0x0004A608
		internal override BaseInvokableCall GetDelegate(object target, MethodInfo theFunction)
		{
			return new InvokableCall(target, theFunction);
		}

		// Token: 0x06002F3C RID: 12092 RVA: 0x0004C424 File Offset: 0x0004A624
		private static BaseInvokableCall GetDelegate(UnityAction action)
		{
			return new InvokableCall(action);
		}

		/// <summary>
		///   <para>Invoke all registered callbacks (runtime and persistent).</para>
		/// </summary>
		// Token: 0x06002F3D RID: 12093 RVA: 0x0004C440 File Offset: 0x0004A640
		public void Invoke()
		{
			base.Invoke(this.m_InvokeArray);
		}

		// Token: 0x04000D4C RID: 3404
		private readonly object[] m_InvokeArray = new object[0];
	}
}
