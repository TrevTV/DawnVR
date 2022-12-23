namespace UnityEngine
{
	public interface ISerializationCallbackReceiverC
	{
		/// <summary>
		///   <para>Implement this method to receive a callback before Unity serializes your object.</para>
		/// </summary>
		// Token: 0x06002E8B RID: 11915
		void OnBeforeSerialize();

		/// <summary>
		///   <para>Implement this method to receive a callback after Unity deserializes your object.</para>
		/// </summary>
		// Token: 0x06002E8C RID: 11916
		void OnAfterDeserialize();
	}
}
