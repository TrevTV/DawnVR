using System;
using System.Collections.Generic;
using UnityEngine.Serialization;

namespace DawnVR.Events
{
	// Token: 0x0200038A RID: 906
	[Serializable]
	internal class PersistentCallGroup
	{
		// Token: 0x06002F05 RID: 12037 RVA: 0x0004BA80 File Offset: 0x00049C80
		public PersistentCallGroup()
		{
			this.m_Calls = new List<PersistentCall>();
		}

		// Token: 0x17000B09 RID: 2825
		// (get) Token: 0x06002F06 RID: 12038 RVA: 0x0004BA94 File Offset: 0x00049C94
		public int Count
		{
			get
			{
				return this.m_Calls.Count;
			}
		}

		// Token: 0x06002F07 RID: 12039 RVA: 0x0004BAB4 File Offset: 0x00049CB4
		public PersistentCall GetListener(int index)
		{
			return this.m_Calls[index];
		}

		// Token: 0x06002F08 RID: 12040 RVA: 0x0004BAD8 File Offset: 0x00049CD8
		public IEnumerable<PersistentCall> GetListeners()
		{
			return this.m_Calls;
		}

		// Token: 0x06002F09 RID: 12041 RVA: 0x0004BAF4 File Offset: 0x00049CF4
		public void AddListener()
		{
			this.m_Calls.Add(new PersistentCall());
		}

		// Token: 0x06002F0A RID: 12042 RVA: 0x0004BB08 File Offset: 0x00049D08
		public void AddListener(PersistentCall call)
		{
			this.m_Calls.Add(call);
		}

		// Token: 0x06002F0B RID: 12043 RVA: 0x0004BB18 File Offset: 0x00049D18
		public void RemoveListener(int index)
		{
			this.m_Calls.RemoveAt(index);
		}

		// Token: 0x06002F0C RID: 12044 RVA: 0x0004BB28 File Offset: 0x00049D28
		public void Clear()
		{
			this.m_Calls.Clear();
		}

		// Token: 0x06002F0D RID: 12045 RVA: 0x0004BB38 File Offset: 0x00049D38
		public void RegisterEventPersistentListener(int index, UnityEngine.Object targetObj, string methodName)
		{
			PersistentCall listener = this.GetListener(index);
			listener.RegisterPersistentListener(targetObj, methodName);
			listener.mode = PersistentListenerMode.EventDefined;
		}

		// Token: 0x06002F0E RID: 12046 RVA: 0x0004BB60 File Offset: 0x00049D60
		public void RegisterVoidPersistentListener(int index, UnityEngine.Object targetObj, string methodName)
		{
			PersistentCall listener = this.GetListener(index);
			listener.RegisterPersistentListener(targetObj, methodName);
			listener.mode = PersistentListenerMode.Void;
		}

		// Token: 0x06002F0F RID: 12047 RVA: 0x0004BB88 File Offset: 0x00049D88
		public void RegisterObjectPersistentListener(int index, UnityEngine.Object targetObj, UnityEngine.Object argument, string methodName)
		{
			PersistentCall listener = this.GetListener(index);
			listener.RegisterPersistentListener(targetObj, methodName);
			listener.mode = PersistentListenerMode.Object;
			listener.arguments.unityObjectArgument = argument;
		}

		// Token: 0x06002F10 RID: 12048 RVA: 0x0004BBBC File Offset: 0x00049DBC
		public void RegisterIntPersistentListener(int index, UnityEngine.Object targetObj, int argument, string methodName)
		{
			PersistentCall listener = this.GetListener(index);
			listener.RegisterPersistentListener(targetObj, methodName);
			listener.mode = PersistentListenerMode.Int;
			listener.arguments.intArgument = argument;
		}

		// Token: 0x06002F11 RID: 12049 RVA: 0x0004BBF0 File Offset: 0x00049DF0
		public void RegisterFloatPersistentListener(int index, UnityEngine.Object targetObj, float argument, string methodName)
		{
			PersistentCall listener = this.GetListener(index);
			listener.RegisterPersistentListener(targetObj, methodName);
			listener.mode = PersistentListenerMode.Float;
			listener.arguments.floatArgument = argument;
		}

		// Token: 0x06002F12 RID: 12050 RVA: 0x0004BC24 File Offset: 0x00049E24
		public void RegisterStringPersistentListener(int index, UnityEngine.Object targetObj, string argument, string methodName)
		{
			PersistentCall listener = this.GetListener(index);
			listener.RegisterPersistentListener(targetObj, methodName);
			listener.mode = PersistentListenerMode.String;
			listener.arguments.stringArgument = argument;
		}

		// Token: 0x06002F13 RID: 12051 RVA: 0x0004BC58 File Offset: 0x00049E58
		public void RegisterBoolPersistentListener(int index, UnityEngine.Object targetObj, bool argument, string methodName)
		{
			PersistentCall listener = this.GetListener(index);
			listener.RegisterPersistentListener(targetObj, methodName);
			listener.mode = PersistentListenerMode.Bool;
			listener.arguments.boolArgument = argument;
		}

		// Token: 0x06002F14 RID: 12052 RVA: 0x0004BC8C File Offset: 0x00049E8C
		public void UnregisterPersistentListener(int index)
		{
			PersistentCall listener = this.GetListener(index);
			listener.UnregisterPersistentListener();
		}

		// Token: 0x06002F15 RID: 12053 RVA: 0x0004BCA8 File Offset: 0x00049EA8
		public void RemoveListeners(Object target, string methodName)
		{
			List<PersistentCall> list = new List<PersistentCall>();
			for (int i = 0; i < this.m_Calls.Count; i++)
			{
				if (this.m_Calls[i].target == target && this.m_Calls[i].methodName == methodName)
				{
					list.Add(this.m_Calls[i]);
				}
			}
			this.m_Calls.RemoveAll(new Predicate<PersistentCall>(list.Contains));
		}

		// Token: 0x06002F16 RID: 12054 RVA: 0x0004BD3C File Offset: 0x00049F3C
		public void Initialize(InvokableCallList invokableList, UnityEventBase unityEventBase)
		{
			foreach (PersistentCall persistentCall in this.m_Calls)
			{
				if (persistentCall.IsValid())
				{
					BaseInvokableCall runtimeCall = persistentCall.GetRuntimeCall(unityEventBase);
					if (runtimeCall != null)
					{
						invokableList.AddPersistentInvokableCall(runtimeCall);
					}
				}
			}
		}

		private List<PersistentCall> m_Calls;
	}
}
