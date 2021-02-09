using System;
using HarmonyLib;

namespace Soyuz
{
	// Token: 0x02000005 RID: 5
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
	public class SoyuzPatch : Attribute
	{
		// Token: 0x06000015 RID: 21 RVA: 0x00002820 File Offset: 0x00000A20
		public SoyuzPatch()
		{
			this.patchType = PatchType.empty;
		}

		// Token: 0x06000016 RID: 22 RVA: 0x0000282F File Offset: 0x00000A2F
		public SoyuzPatch(Type targetType, string targetMethod, MethodType methodType = MethodType.Normal, Type[] parameters = null, Type[] generics = null)
		{
			this.patchType = PatchType.normal;
			this.targetType = targetType;
			this.targetMethod = targetMethod;
			this.methodType = methodType;
			this.parameters = parameters;
			this.generics = generics;
		}

		// Token: 0x0400001A RID: 26
		public Type targetType;

		// Token: 0x0400001B RID: 27
		public string targetMethod;

		// Token: 0x0400001C RID: 28
		public Type[] parameters;

		// Token: 0x0400001D RID: 29
		public Type[] generics;

		// Token: 0x0400001E RID: 30
		public MethodType methodType;

		// Token: 0x0400001F RID: 31
		public readonly PatchType patchType;
	}
}
