// Copyright (c) MOSA Project. Licensed under the New BSD License.

// This code was generated by an automated template.

using Mosa.Compiler.Framework;

namespace Mosa.Platform.x86.Instructions
{
	/// <summary>
	/// Ucomiss
	/// </summary>
	/// <seealso cref="Mosa.Platform.x86.X86Instruction" />
	public sealed class Ucomiss : X86Instruction
	{
		public static readonly LegacyOpCode LegacyOpcode = new LegacyOpCode(new byte[] { 0x0F, 0x2E } );

		public Ucomiss()
			: base(0, 2)
		{
		}

		public override void Emit(InstructionNode node, BaseCodeEmitter emitter)
		{
			System.Diagnostics.Debug.Assert(node.ResultCount == DefaultResultCount || VariableOperands);
			System.Diagnostics.Debug.Assert(node.OperandCount == DefaultOperandCount || VariableOperands);

			StaticEmitters.EmitUcomiss(node, emitter);
		}

		// The following is used by the automated code generator.

		public override LegacyOpCode __legacyopcode { get { return LegacyOpcode; } }

		public override string __staticEmitMethod { get { return "StaticEmitters.Emit%"; } }
	}
}

