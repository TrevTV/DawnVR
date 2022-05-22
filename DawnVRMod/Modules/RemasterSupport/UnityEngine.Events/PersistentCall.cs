using System;
using System.Reflection;
using UnityEngine.Serialization;

namespace DawnVR.Events
{
	// Token: 0x02000389 RID: 905
	[Serializable]
	internal class PersistentCall
	{
		// Token: 0x06002EF8 RID: 12024 RVA: 0x0004B750 File Offset: 0x00049950
		public PersistentCall()
		{
		}

		// Token: 0x17000B04 RID: 2820
		// (get) Token: 0x06002EF9 RID: 12025 RVA: 0x0004B774 File Offset: 0x00049974
		public UnityEngine.Object target
		{
			get
			{
				return this.m_Target;
			}
		}

		// Token: 0x17000B05 RID: 2821
		// (get) Token: 0x06002EFA RID: 12026 RVA: 0x0004B790 File Offset: 0x00049990
		public string methodName
		{
			get
			{
				return this.m_MethodName;
			}
		}

		// Token: 0x17000B06 RID: 2822
		// (get) Token: 0x06002EFB RID: 12027 RVA: 0x0004B7AC File Offset: 0x000499AC
		// (set) Token: 0x06002EFC RID: 12028 RVA: 0x0004B7C8 File Offset: 0x000499C8
		public PersistentListenerMode mode
		{
			get
			{
				return this.m_Mode;
			}
			set
			{
				this.m_Mode = value;
			}
		}

		// Token: 0x17000B07 RID: 2823
		// (get) Token: 0x06002EFD RID: 12029 RVA: 0x0004B7D4 File Offset: 0x000499D4
		public ArgumentCache arguments
		{
			get
			{
				return this.m_Arguments;
			}
		}

		// Token: 0x17000B08 RID: 2824
		// (get) Token: 0x06002EFE RID: 12030 RVA: 0x0004B7F0 File Offset: 0x000499F0
		// (set) Token: 0x06002EFF RID: 12031 RVA: 0x0004B80C File Offset: 0x00049A0C
		public UnityEventCallState callState
		{
			get
			{
				return this.m_CallState;
			}
			set
			{
				this.m_CallState = value;
			}
		}

		// Token: 0x06002F00 RID: 12032 RVA: 0x0004B818 File Offset: 0x00049A18
		public bool IsValid()
		{
			return this.target != null && !string.IsNullOrEmpty(this.methodName);
		}

		// Token: 0x06002F01 RID: 12033 RVA: 0x0004B850 File Offset: 0x00049A50
		public BaseInvokableCall GetRuntimeCall(UnityEventBase theEvent)
		{
			BaseInvokableCall result;
			if (this.m_CallState == UnityEventCallState.Off || theEvent == null)
			{
				result = null;
			}
			else
			{
				MethodInfo methodInfo = theEvent.FindMethod(this);
				if (methodInfo == null)
				{
					result = null;
				}
				else
				{
					switch (this.m_Mode)
					{
					case PersistentListenerMode.EventDefined:
						result = theEvent.GetDelegate(this.target, methodInfo);
						break;
					case PersistentListenerMode.Void:
						result = new InvokableCall(this.target, methodInfo);
						break;
					case PersistentListenerMode.Object:
						result = PersistentCall.GetObjectCall(this.target, methodInfo, this.m_Arguments);
						break;
					case PersistentListenerMode.Int:
						result = new CachedInvokableCall<int>(this.target, methodInfo, this.m_Arguments.intArgument);
						break;
					case PersistentListenerMode.Float:
						result = new CachedInvokableCall<float>(this.target, methodInfo, this.m_Arguments.floatArgument);
						break;
					case PersistentListenerMode.String:
						result = new CachedInvokableCall<string>(this.target, methodInfo, this.m_Arguments.stringArgument);
						break;
					case PersistentListenerMode.Bool:
						result = new CachedInvokableCall<bool>(this.target, methodInfo, this.m_Arguments.boolArgument);
						break;
					default:
						result = null;
						break;
					}
				}
			}
			return result;
		}

		// Token: 0x06002F02 RID: 12034 RVA: 0x0004B974 File Offset: 0x00049B74
		private static BaseInvokableCall GetObjectCall(Object target, MethodInfo method, ArgumentCache arguments)
		{
			Type type = typeof(Object);
			if (!string.IsNullOrEmpty(arguments.unityObjectArgumentAssemblyTypeName))
			{
				type = Type.GetType(arguments.unityObjectArgumentAssemblyTypeName, false) ?? typeof(Object);
			}
			Type typeFromHandle = typeof(CachedInvokableCall<>);
			Type type2 = typeFromHandle.MakeGenericType(new Type[] { type });
			ConstructorInfo constructor = type2.GetConstructor(new Type[]
			{
				typeof(Object),
				typeof(MethodInfo),
				type
			});
			Object @object = arguments.unityObjectArgument;
			if (@object != null && !type.IsAssignableFrom(@object.GetType()))
			{
				@object = null;
			}
			return constructor.Invoke(new object[] { target, method, @object }) as BaseInvokableCall;
		}

		// Token: 0x06002F03 RID: 12035 RVA: 0x0004BA54 File Offset: 0x00049C54
		public void RegisterPersistentListener(UnityEngine.Object ttarget, string mmethodName)
		{
			this.m_Target = ttarget;
			this.m_MethodName = mmethodName;
		}

		// Token: 0x06002F04 RID: 12036 RVA: 0x0004BA68 File Offset: 0x00049C68
		public void UnregisterPersistentListener()
		{
			this.m_MethodName = string.Empty;
			this.m_Target = null;
		}

		private UnityEngine.Object m_Target;
		private string m_MethodName;
		private PersistentListenerMode m_Mode = PersistentListenerMode.EventDefined;
		private ArgumentCache m_Arguments = new ArgumentCache();
		private UnityEventCallState m_CallState = UnityEventCallState.RuntimeOnly;
	}
}
