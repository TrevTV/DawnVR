using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace DawnVR.Events
{
	// Token: 0x02000380 RID: 896
	[Serializable]
	internal class ArgumentCache : ISerializationCallbackReceiverC
    {
		// Token: 0x06002EC3 RID: 11971 RVA: 0x0004AE48 File Offset: 0x00049048
		public ArgumentCache()
		{
		}

		// Token: 0x17000AFE RID: 2814
		// (get) Token: 0x06002EC4 RID: 11972 RVA: 0x0004AE50 File Offset: 0x00049050
		// (set) Token: 0x06002EC5 RID: 11973 RVA: 0x0004AE6C File Offset: 0x0004906C
		public UnityEngine.Object unityObjectArgument
		{
			get
			{
				return this.m_ObjectArgument;
			}
			set
			{
				this.m_ObjectArgument = value;
				this.m_ObjectArgumentAssemblyTypeName = ((!(value != null)) ? string.Empty : value.GetType().AssemblyQualifiedName);
			}
		}

		// Token: 0x17000AFF RID: 2815
		// (get) Token: 0x06002EC6 RID: 11974 RVA: 0x0004AEA0 File Offset: 0x000490A0
		public string unityObjectArgumentAssemblyTypeName
		{
			get
			{
				return this.m_ObjectArgumentAssemblyTypeName;
			}
		}

		// Token: 0x17000B00 RID: 2816
		// (get) Token: 0x06002EC7 RID: 11975 RVA: 0x0004AEBC File Offset: 0x000490BC
		// (set) Token: 0x06002EC8 RID: 11976 RVA: 0x0004AED8 File Offset: 0x000490D8
		public int intArgument
		{
			get
			{
				return this.m_IntArgument;
			}
			set
			{
				this.m_IntArgument = value;
			}
		}

		// Token: 0x17000B01 RID: 2817
		// (get) Token: 0x06002EC9 RID: 11977 RVA: 0x0004AEE4 File Offset: 0x000490E4
		// (set) Token: 0x06002ECA RID: 11978 RVA: 0x0004AF00 File Offset: 0x00049100
		public float floatArgument
		{
			get
			{
				return this.m_FloatArgument;
			}
			set
			{
				this.m_FloatArgument = value;
			}
		}

		// Token: 0x17000B02 RID: 2818
		// (get) Token: 0x06002ECB RID: 11979 RVA: 0x0004AF0C File Offset: 0x0004910C
		// (set) Token: 0x06002ECC RID: 11980 RVA: 0x0004AF28 File Offset: 0x00049128
		public string stringArgument
		{
			get
			{
				return this.m_StringArgument;
			}
			set
			{
				this.m_StringArgument = value;
			}
		}

		// Token: 0x17000B03 RID: 2819
		// (get) Token: 0x06002ECD RID: 11981 RVA: 0x0004AF34 File Offset: 0x00049134
		// (set) Token: 0x06002ECE RID: 11982 RVA: 0x0004AF50 File Offset: 0x00049150
		public bool boolArgument
		{
			get
			{
				return this.m_BoolArgument;
			}
			set
			{
				this.m_BoolArgument = value;
			}
		}

		// Token: 0x06002ECF RID: 11983 RVA: 0x0004AF5C File Offset: 0x0004915C
		private void TidyAssemblyTypeName()
		{
			if (!string.IsNullOrEmpty(this.m_ObjectArgumentAssemblyTypeName))
			{
				int num = int.MaxValue;
				int num2 = this.m_ObjectArgumentAssemblyTypeName.IndexOf(", Version=");
				if (num2 != -1)
				{
					num = Math.Min(num2, num);
				}
				num2 = this.m_ObjectArgumentAssemblyTypeName.IndexOf(", Culture=");
				if (num2 != -1)
				{
					num = Math.Min(num2, num);
				}
				num2 = this.m_ObjectArgumentAssemblyTypeName.IndexOf(", PublicKeyToken=");
				if (num2 != -1)
				{
					num = Math.Min(num2, num);
				}
				if (num != 2147483647)
				{
					this.m_ObjectArgumentAssemblyTypeName = this.m_ObjectArgumentAssemblyTypeName.Substring(0, num);
				}
			}
		}

		// Token: 0x06002ED0 RID: 11984 RVA: 0x0004B008 File Offset: 0x00049208
		public void OnBeforeSerialize()
		{
			this.TidyAssemblyTypeName();
		}

		// Token: 0x06002ED1 RID: 11985 RVA: 0x0004B014 File Offset: 0x00049214
		public void OnAfterDeserialize()
		{
			this.TidyAssemblyTypeName();
		}

		private UnityEngine.Object m_ObjectArgument;
		private string m_ObjectArgumentAssemblyTypeName;
		private int m_IntArgument;
		private float m_FloatArgument;
		private string m_StringArgument;
		private bool m_BoolArgument;
	}
}
