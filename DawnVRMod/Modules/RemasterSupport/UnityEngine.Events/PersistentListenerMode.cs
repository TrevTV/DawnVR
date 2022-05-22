using System;

namespace DawnVR.Events
{
	/// <summary>
	///   <para>THe mode that a listener is operating in.</para>
	/// </summary>
	// Token: 0x0200037F RID: 895
	[Serializable]
	public enum PersistentListenerMode
	{
		/// <summary>
		///   <para>The listener will use the function binding specified by the even.</para>
		/// </summary>
		// Token: 0x04000D27 RID: 3367
		EventDefined,
		/// <summary>
		///   <para>The listener will bind to zero argument functions.</para>
		/// </summary>
		// Token: 0x04000D28 RID: 3368
		Void,
		/// <summary>
		///   <para>The listener will bind to one argument Object functions.</para>
		/// </summary>
		// Token: 0x04000D29 RID: 3369
		Object,
		/// <summary>
		///   <para>The listener will bind to one argument int functions.</para>
		/// </summary>
		// Token: 0x04000D2A RID: 3370
		Int,
		/// <summary>
		///   <para>The listener will bind to one argument float functions.</para>
		/// </summary>
		// Token: 0x04000D2B RID: 3371
		Float,
		/// <summary>
		///   <para>The listener will bind to one argument string functions.</para>
		/// </summary>
		// Token: 0x04000D2C RID: 3372
		String,
		/// <summary>
		///   <para>The listener will bind to one argument bool functions.</para>
		/// </summary>
		// Token: 0x04000D2D RID: 3373
		Bool
	}
}
