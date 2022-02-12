using System;

namespace DawnVR.Events
{
	/// <summary>
	///   <para>Controls the scope of UnityEvent callbacks.</para>
	/// </summary>
	// Token: 0x02000388 RID: 904
	public enum UnityEventCallState
	{
		/// <summary>
		///   <para>Callback is not issued.</para>
		/// </summary>
		// Token: 0x04000D3B RID: 3387
		Off,
		/// <summary>
		///   <para>Callback is always issued.</para>
		/// </summary>
		// Token: 0x04000D3C RID: 3388
		EditorAndRuntime,
		/// <summary>
		///   <para>Callback is only issued in the Runtime and Editor playmode.</para>
		/// </summary>
		// Token: 0x04000D3D RID: 3389
		RuntimeOnly
	}
}
