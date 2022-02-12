using System;
using System.Reflection;
using UnityEngine;
using UnityEngine.Serialization;

namespace DawnVR.Events
{
	/// <summary>
	///   <para>Abstract base class for UnityEvents.</para>
	/// </summary>
	// Token: 0x0200038C RID: 908
	[Serializable]
	public abstract class UnityEventBase : ISerializationCallbackReceiver
	{
		// Token: 0x06002F1F RID: 12063 RVA: 0x0004BF74 File Offset: 0x0004A174
		protected UnityEventBase()
		{
			this.m_Calls = new InvokableCallList();
			this.m_PersistentCalls = new PersistentCallGroup();
			this.m_TypeName = base.GetType().AssemblyQualifiedName;
		}

		// Token: 0x06002F20 RID: 12064 RVA: 0x0004BFAC File Offset: 0x0004A1AC
		void ISerializationCallbackReceiver.OnBeforeSerialize()
		{
		}

		// Token: 0x06002F21 RID: 12065 RVA: 0x0004BFB0 File Offset: 0x0004A1B0
		void ISerializationCallbackReceiver.OnAfterDeserialize()
		{
			this.DirtyPersistentCalls();
			this.m_TypeName = base.GetType().AssemblyQualifiedName;
		}

		// Token: 0x06002F22 RID: 12066
		protected abstract MethodInfo FindMethod_Impl(string name, object targetObj);

		// Token: 0x06002F23 RID: 12067
		internal abstract BaseInvokableCall GetDelegate(object target, MethodInfo theFunction);

		// Token: 0x06002F24 RID: 12068 RVA: 0x0004BFCC File Offset: 0x0004A1CC
		internal MethodInfo FindMethod(PersistentCall call)
		{
			Type argumentType = typeof(UnityEngine.Object);
			if (!string.IsNullOrEmpty(call.arguments.unityObjectArgumentAssemblyTypeName))
			{
				argumentType = Type.GetType(call.arguments.unityObjectArgumentAssemblyTypeName, false) ?? typeof(UnityEngine.Object);
			}
			return this.FindMethod(call.methodName, call.target, call.mode, argumentType);
		}

		// Token: 0x06002F25 RID: 12069 RVA: 0x0004C040 File Offset: 0x0004A240
		internal MethodInfo FindMethod(string name, object listener, PersistentListenerMode mode, Type argumentType)
		{
			MethodInfo result;
			switch (mode)
			{
			case PersistentListenerMode.EventDefined:
				result = this.FindMethod_Impl(name, listener);
				break;
			case PersistentListenerMode.Void:
				result = UnityEventBase.GetValidMethodInfo(listener, name, new Type[0]);
				break;
			case PersistentListenerMode.Object:
				result = UnityEventBase.GetValidMethodInfo(listener, name, new Type[] { argumentType ?? typeof(UnityEngine.Object) });
				break;
			case PersistentListenerMode.Int:
				result = UnityEventBase.GetValidMethodInfo(listener, name, new Type[] { typeof(int) });
				break;
			case PersistentListenerMode.Float:
				result = UnityEventBase.GetValidMethodInfo(listener, name, new Type[] { typeof(float) });
				break;
			case PersistentListenerMode.String:
				result = UnityEventBase.GetValidMethodInfo(listener, name, new Type[] { typeof(string) });
				break;
			case PersistentListenerMode.Bool:
				result = UnityEventBase.GetValidMethodInfo(listener, name, new Type[] { typeof(bool) });
				break;
			default:
				result = null;
				break;
			}
			return result;
		}

		/// <summary>
		///   <para>Get the number of registered persistent listeners.</para>
		/// </summary>
		// Token: 0x06002F26 RID: 12070 RVA: 0x0004C148 File Offset: 0x0004A348
		public int GetPersistentEventCount()
		{
			return this.m_PersistentCalls.Count;
		}

		/// <summary>
		///   <para>Get the target component of the listener at index index.</para>
		/// </summary>
		/// <param name="index">Index of the listener to query.</param>
		// Token: 0x06002F27 RID: 12071 RVA: 0x0004C168 File Offset: 0x0004A368
		public UnityEngine.Object GetPersistentTarget(int index)
		{
			PersistentCall listener = this.m_PersistentCalls.GetListener(index);
			return (listener == null) ? null : listener.target;
		}

		/// <summary>
		///   <para>Get the target method name of the listener at index index.</para>
		/// </summary>
		/// <param name="index">Index of the listener to query.</param>
		// Token: 0x06002F28 RID: 12072 RVA: 0x0004C19C File Offset: 0x0004A39C
		public string GetPersistentMethodName(int index)
		{
			PersistentCall listener = this.m_PersistentCalls.GetListener(index);
			return (listener == null) ? string.Empty : listener.methodName;
		}

		// Token: 0x06002F29 RID: 12073 RVA: 0x0004C1D4 File Offset: 0x0004A3D4
		private void DirtyPersistentCalls()
		{
			this.m_Calls.ClearPersistent();
			this.m_CallsDirty = true;
		}

		// Token: 0x06002F2A RID: 12074 RVA: 0x0004C1EC File Offset: 0x0004A3EC
		private void RebuildPersistentCallsIfNeeded()
		{
			if (this.m_CallsDirty)
			{
				this.m_PersistentCalls.Initialize(this.m_Calls, this);
				this.m_CallsDirty = false;
			}
		}

		/// <summary>
		///   <para>Modify the execution state of a persistent listener.</para>
		/// </summary>
		/// <param name="index">Index of the listener to query.</param>
		/// <param name="state">State to set.</param>
		// Token: 0x06002F2B RID: 12075 RVA: 0x0004C218 File Offset: 0x0004A418
		public void SetPersistentListenerState(int index, UnityEventCallState state)
		{
			PersistentCall listener = this.m_PersistentCalls.GetListener(index);
			if (listener != null)
			{
				listener.callState = state;
			}
			this.DirtyPersistentCalls();
		}

		// Token: 0x06002F2C RID: 12076 RVA: 0x0004C248 File Offset: 0x0004A448
		protected void AddListener(object targetObj, MethodInfo method)
		{
			this.m_Calls.AddListener(this.GetDelegate(targetObj, method));
		}

		// Token: 0x06002F2D RID: 12077 RVA: 0x0004C260 File Offset: 0x0004A460
		internal void AddCall(BaseInvokableCall call)
		{
			this.m_Calls.AddListener(call);
		}

		// Token: 0x06002F2E RID: 12078 RVA: 0x0004C270 File Offset: 0x0004A470
		protected void RemoveListener(object targetObj, MethodInfo method)
		{
			this.m_Calls.RemoveListener(targetObj, method);
		}

		/// <summary>
		///   <para>Remove all non-persisent (ie created from script) listeners  from the event.</para>
		/// </summary>
		// Token: 0x06002F2F RID: 12079 RVA: 0x0004C280 File Offset: 0x0004A480
		public void RemoveAllListeners()
		{
			this.m_Calls.Clear();
		}

		// Token: 0x06002F30 RID: 12080 RVA: 0x0004C290 File Offset: 0x0004A490
		protected void Invoke(object[] parameters)
		{
			this.RebuildPersistentCallsIfNeeded();
			this.m_Calls.Invoke(parameters);
		}

		// Token: 0x06002F31 RID: 12081 RVA: 0x0004C2A8 File Offset: 0x0004A4A8
		public override string ToString()
		{
			return base.ToString() + " " + base.GetType().FullName;
		}

		/// <summary>
		///   <para>Given an object, function name, and a list of argument types; find the method that matches.</para>
		/// </summary>
		/// <param name="obj">Object to search for the method.</param>
		/// <param name="functionName">Function name to search for.</param>
		/// <param name="argumentTypes">Argument types for the function.</param>
		// Token: 0x06002F32 RID: 12082 RVA: 0x0004C2D8 File Offset: 0x0004A4D8
		public static MethodInfo GetValidMethodInfo(object obj, string functionName, Type[] argumentTypes)
		{
			Type type = obj.GetType();
			while (type != typeof(object) && type != null)
			{
				MethodInfo method = type.GetMethod(functionName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, argumentTypes, null);
				if (method != null)
				{
					ParameterInfo[] parameters = method.GetParameters();
					bool flag = true;
					int num = 0;
					foreach (ParameterInfo parameterInfo in parameters)
					{
						Type type2 = argumentTypes[num];
						Type parameterType = parameterInfo.ParameterType;
						flag = type2.IsPrimitive == parameterType.IsPrimitive;
						if (!flag)
						{
							break;
						}
						num++;
					}
					if (flag)
					{
						return method;
					}
				}
				type = type.BaseType;
			}
			return null;
		}

		private InvokableCallList m_Calls;
		private PersistentCallGroup m_PersistentCalls;
		private string m_TypeName;
		private bool m_CallsDirty = true;
	}
}
