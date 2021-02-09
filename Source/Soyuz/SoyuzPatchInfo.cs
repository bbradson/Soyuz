using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using RocketMan;
using Verse;

namespace Soyuz
{
	// Token: 0x02000006 RID: 6
	public class SoyuzPatchInfo
	{
		// Token: 0x17000002 RID: 2
		// (get) Token: 0x06000017 RID: 23 RVA: 0x00002863 File Offset: 0x00000A63
		public bool IsValid
		{
			get
			{
				if (this.attribute != null)
				{
					return this.targets.All((MethodBase t) => t != null);
				}
				return false;
			}
		}

		// Token: 0x06000018 RID: 24 RVA: 0x0000289C File Offset: 0x00000A9C
		public SoyuzPatchInfo(Type type)
		{
			this.attribute = type.TryGetAttribute<SoyuzPatch>();
			this.patchType = this.attribute.patchType;
			if (this.patchType == PatchType.normal)
			{
				if (this.attribute.methodType == MethodType.Getter)
				{
					this.targets = new MethodBase[]
					{
						AccessTools.PropertyGetter(this.attribute.targetType, this.attribute.targetMethod)
					};
				}
				else if (this.attribute.methodType == MethodType.Setter)
				{
					this.targets = new MethodBase[]
					{
						AccessTools.PropertySetter(this.attribute.targetType, this.attribute.targetMethod)
					};
				}
				else
				{
					if (this.attribute.methodType != MethodType.Normal)
					{
						throw new NotImplementedException();
					}
					this.targets = new MethodBase[]
					{
						AccessTools.Method(this.attribute.targetType, this.attribute.targetMethod, this.attribute.parameters, this.attribute.generics)
					};
				}
			}
			else if (this.patchType == PatchType.empty)
			{
				this.targets = (type.GetMethod("TargetMethods").Invoke(null, null) as IEnumerable<MethodBase>).ToArray<MethodBase>();
			}
			this.prepare = type.GetMethod("Prepare");
			this.prefix = type.GetMethod("Prefix");
			this.postfix = type.GetMethod("Postfix");
			this.transpiler = type.GetMethod("Transpiler");
			this.finalizer = type.GetMethod("Finalizer");
		}

		// Token: 0x06000019 RID: 25 RVA: 0x00002A28 File Offset: 0x00000C28
		public void Patch(Harmony harmony)
		{
			if (this.prepare != null && !(bool)this.prepare.Invoke(null, null))
			{
				return;
			}
			foreach (MethodBase target in this.targets.ToHashSet<MethodBase>())
			{
				if (target == null || target.IsAbstract || !target.HasMethodBody())
				{
					string format = "SOYUZ: patching {0}:{1} is not possible!";
					object arg;
					if (target == null)
					{
						arg = null;
					}
					else
					{
						Type declaringType = target.DeclaringType;
						arg = ((declaringType != null) ? declaringType.Name : null);
					}
					//Log.Warning(string.Format(format, arg, target), false);
				}
				else
				{
					try
					{
						harmony.Patch(target, (this.prefix != null) ? new HarmonyMethod(this.prefix) : null, (this.postfix != null) ? new HarmonyMethod(this.postfix) : null, (this.transpiler != null) ? new HarmonyMethod(this.transpiler) : null, (this.finalizer != null) ? new HarmonyMethod(this.finalizer) : null);
					}
					catch (Exception er)
					{
						//Log.Warning(string.Format("SOYUZ: patching {0}:{1} is not possible!", target.DeclaringType.Name, target), false);
					}
				}
			}
		}

		// Token: 0x0600001A RID: 26 RVA: 0x00002C00 File Offset: 0x00000E00
		public void Unpatch(Harmony harmony)
		{
			foreach (MethodBase target in this.targets.ToHashSet<MethodBase>())
			{
				try
				{
					harmony.Unpatch(target, HarmonyPatchType.All, Finder.HarmonyID + ".Soyuz");
				}
				catch (Exception er)
				{
					//Log.Warning(string.Format("SOYUZ: Unpatching {0}:{1} is not possible!", target.DeclaringType.Name, target), false);
				}
			}
		}

		// Token: 0x04000020 RID: 32
		private SoyuzPatch attribute;

		// Token: 0x04000021 RID: 33
		private MethodBase[] targets;

		// Token: 0x04000022 RID: 34
		private MethodInfo prefix;

		// Token: 0x04000023 RID: 35
		private MethodInfo postfix;

		// Token: 0x04000024 RID: 36
		private MethodInfo transpiler;

		// Token: 0x04000025 RID: 37
		private MethodInfo finalizer;

		// Token: 0x04000026 RID: 38
		private MethodBase prepare;

		// Token: 0x04000027 RID: 39
		private PatchType patchType;
	}
}
