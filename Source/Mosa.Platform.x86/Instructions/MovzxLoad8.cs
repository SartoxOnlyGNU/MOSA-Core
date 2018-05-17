// Copyright (c) MOSA Project. Licensed under the New BSD License.

// This code was generated by an automated template.

using Mosa.Compiler.Framework;

namespace Mosa.Platform.x86.Instructions
{
	/// <summary>
	/// MovzxLoad8
	/// </summary>
	/// <seealso cref="Mosa.Platform.x86.X86Instruction" />
	public sealed class MovzxLoad8 : X86Instruction
	{
		internal MovzxLoad8()
			: base(1, 2)
		{
		}

		public override bool IsMemoryRead { get { return true; } }

		public override void Emit(InstructionNode node, BaseCodeEmitter emitter)
		{
			System.Diagnostics.Debug.Assert(node.ResultCount == DefaultResultCount);
			System.Diagnostics.Debug.Assert(node.OperandCount == DefaultOperandCount);

			StaticEmitters.EmitMovzxLoad8(node, emitter);
		}
	}
}
