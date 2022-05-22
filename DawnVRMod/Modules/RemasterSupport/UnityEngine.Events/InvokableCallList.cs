using System;
using System.Collections.Generic;
using System.Reflection;

namespace DawnVR.Events
{
	// Token: 0x0200038B RID: 907
	internal class InvokableCallList
	{
		// Token: 0x06002F17 RID: 12055 RVA: 0x0004BDBC File Offset: 0x00049FBC
		public InvokableCallList()
		{
		}

		// Token: 0x17000B0A RID: 2826
		// (get) Token: 0x06002F18 RID: 12056 RVA: 0x0004BDEC File Offset: 0x00049FEC
		public int Count
		{
			get
			{
				return this.m_PersistentCalls.Count + this.m_RuntimeCalls.Count;
			}
		}

		// Token: 0x06002F19 RID: 12057 RVA: 0x0004BE18 File Offset: 0x0004A018
		public void AddPersistentInvokableCall(BaseInvokableCall call)
		{
			this.m_PersistentCalls.Add(call);
			this.m_NeedsUpdate = true;
		}

		// Token: 0x06002F1A RID: 12058 RVA: 0x0004BE30 File Offset: 0x0004A030
		public void AddListener(BaseInvokableCall call)
		{
			this.m_RuntimeCalls.Add(call);
			this.m_NeedsUpdate = true;
		}

		// Token: 0x06002F1B RID: 12059 RVA: 0x0004BE48 File Offset: 0x0004A048
		public void RemoveListener(object targetObj, MethodInfo method)
		{
			List<BaseInvokableCall> list = new List<BaseInvokableCall>();
			for (int i = 0; i < this.m_RuntimeCalls.Count; i++)
			{
				if (this.m_RuntimeCalls[i].Find(targetObj, method))
				{
					list.Add(this.m_RuntimeCalls[i]);
				}
			}
			this.m_RuntimeCalls.RemoveAll(new Predicate<BaseInvokableCall>(list.Contains));
			this.m_NeedsUpdate = true;
		}

		// Token: 0x06002F1C RID: 12060 RVA: 0x0004BEC4 File Offset: 0x0004A0C4
		public void Clear()
		{
			this.m_RuntimeCalls.Clear();
			this.m_NeedsUpdate = true;
		}

		// Token: 0x06002F1D RID: 12061 RVA: 0x0004BEDC File Offset: 0x0004A0DC
		public void ClearPersistent()
		{
			this.m_PersistentCalls.Clear();
			this.m_NeedsUpdate = true;
		}

		// Token: 0x06002F1E RID: 12062 RVA: 0x0004BEF4 File Offset: 0x0004A0F4
		public void Invoke(object[] parameters)
		{
			if (this.m_NeedsUpdate)
			{
				this.m_ExecutingCalls.Clear();
				this.m_ExecutingCalls.AddRange(this.m_PersistentCalls);
				this.m_ExecutingCalls.AddRange(this.m_RuntimeCalls);
				this.m_NeedsUpdate = false;
			}
			for (int i = 0; i < this.m_ExecutingCalls.Count; i++)
			{
				this.m_ExecutingCalls[i].Invoke(parameters);
			}
		}

		// Token: 0x04000D44 RID: 3396
		private readonly List<BaseInvokableCall> m_PersistentCalls = new List<BaseInvokableCall>();

		// Token: 0x04000D45 RID: 3397
		private readonly List<BaseInvokableCall> m_RuntimeCalls = new List<BaseInvokableCall>();

		// Token: 0x04000D46 RID: 3398
		private readonly List<BaseInvokableCall> m_ExecutingCalls = new List<BaseInvokableCall>();

		// Token: 0x04000D47 RID: 3399
		private bool m_NeedsUpdate = true;
	}
}
