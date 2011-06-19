﻿/*
 * (c) 2008 MOSA - The Managed Operating System Alliance
 *
 * Licensed under the terms of the New BSD License.
 *
 * Authors:
 *  Michael Ruck (grover) <sharpos@michaelruck.de>
 */

using System;

namespace Mosa.Runtime.CompilerFramework.IR
{
	/// <summary>
	/// Intermediate representation of the unsigned subtraction operation.
	/// </summary>
	/// <remarks>
	/// The add instruction is a three-address instruction, where the result receives
	/// the value of the second operand (index 1) subtracted from the first operand (index 0).
	/// <para />
	/// Both the first and second operand must be the same integral type. If the second operand
	/// is statically or dynamically equal to or larger than the number of bits in the first
	/// operand, the result is undefined.
	/// </remarks>
	public sealed class SubUInstruction : ThreeOperandInstruction
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="SubUInstruction"/>.
		/// </summary>
		public SubUInstruction()
		{
		}

		/// <summary>
		/// Allows visitor based dispatch for this instruction object.
		/// </summary>
		/// <param name="visitor">The visitor object.</param>
		/// <param name="context">The context.</param>
		public override void Visit(IIRVisitor visitor, Context context)
		{
			visitor.SubUInstruction(context);
		}

	}
}
