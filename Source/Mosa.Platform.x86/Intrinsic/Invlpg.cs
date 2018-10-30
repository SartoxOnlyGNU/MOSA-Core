﻿// Copyright (c) MOSA Project. Licensed under the New BSD License.

using Mosa.Compiler.Framework;
using System.Diagnostics;

namespace Mosa.Platform.x86.Intrinsic
{
	/// <summary>
	/// Representations the x86 Invlpg instruction.
	/// </summary>
	internal sealed class Invlpg : IIntrinsicPlatformMethod
	{
		void IIntrinsicPlatformMethod.ReplaceIntrinsicCall(Context context, MethodCompiler methodCompiler)
		{
			//Debug.Assert(context.Operand1.IsConstant);
			context.SetInstruction(X86.Invlpg, null, context.Operand1);
		}
	}
}
