﻿// Copyright (c) MOSA Project. Licensed under the New BSD License.

using Mosa.Compiler.Common;
using Mosa.Compiler.Framework.IR;
using Mosa.Compiler.Trace;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

// INCOMPLETE

namespace Mosa.Compiler.Framework.Analysis.Live
{
	/// <summary>
	/// This stage determine were object references are located in code.
	/// </summary>
	public class LiveAnalysis
	{
		private BaseLiveAnalysisEnvironment Environment;
		private BasicBlocks BasicBlocks { get { return Environment.BasicBlocks; } }
		private int SlotCount { get { return Environment.SlotCount; } }

		private readonly ITraceFactory TraceFactory;

		private List<ExtendedBlock2> ExtendedBlocks;
		private LiveRanges[] LiveRanges;

		//private TraceLog trace;

		private TraceLog CreateTrace(string name)
		{
			return TraceFactory.CreateTraceLog(name);
		}

		public LiveAnalysis(BaseLiveAnalysisEnvironment environment, ITraceFactory traceFactory, bool numberInstructions)
		{
			Environment = environment;
			TraceFactory = traceFactory;

			LiveRanges = new LiveRanges[SlotCount];

			if (numberInstructions)
			{
				NumberInstructions();
			}

			CreateExtendedBlocks();

			ComputeLocalLiveSets();

			ComputeLocalLiveSets();
		}

		private void CreateExtendedBlocks()
		{
			ExtendedBlocks = new List<ExtendedBlock2>(BasicBlocks.Count);

			foreach (var block in BasicBlocks)
			{
				var extendedBlock = new ExtendedBlock2(block, SlotCount, 0)
				{
					Range = new Range(block.First.Offset, block.Last.Offset)
				};

				ExtendedBlocks.Add(extendedBlock);
			}
		}

		public void NumberInstructions()
		{
			const int increment = 2;
			int index = increment;

			foreach (var block in BasicBlocks)
			{
				for (var node = block.First; ; node = node.Next)
				{
					if (node.IsEmpty)
						continue;

					node.Offset = index;
					index += increment;

					if (node.IsBlockEndInstruction)
						break;
				}

				Debug.Assert(block.Last.Offset != 0);
			}
		}

		private void ComputeLocalLiveSets()
		{
			var liveSetTrace = CreateTrace("ComputeLocalLiveSets");

			foreach (var block in ExtendedBlocks)
			{
				if (liveSetTrace.Active)
					liveSetTrace.Log("Block # " + block.BasicBlock.Sequence.ToString());

				var liveGen = new BitArray(SlotCount, false);
				var liveKill = new BitArray(SlotCount, false);

				if (BasicBlocks.HeadBlocks.Contains(block.BasicBlock))
				{
					for (int s = 0; s < SlotCount; s++)
					{
						liveKill.Set(s, true);
					}

					if (liveSetTrace.Active)
						liveSetTrace.Log("KILL ALL PHYSICAL");
				}

				for (var node = block.BasicBlock.First; !node.IsBlockEndInstruction; node = node.Next)
				{
					if (node.IsEmpty)
						continue;

					if (liveSetTrace.Active)
						liveSetTrace.Log(node.ToString());

					foreach (var index in Environment.GetInput(node))
					{
						//if (liveSetTrace.Active)
						//	liveSetTrace.Log("INPUT:  " + op);

						if (!liveKill.Get(index))
						{
							liveGen.Set(index, true);

							if (liveSetTrace.Active)
							{
								liveSetTrace.Log("GEN:  " + index.ToString()/* + " " + op*/);
							}
						}
					}

					foreach (var index in Environment.GetKill(node))
					{
						liveKill.Set(index, true);
					}

					foreach (var index in Environment.GetOutput(node))
					{
						//if (liveSetTrace.Active)
						//liveSetTrace.Log("OUTPUT: " + op);

						liveKill.Set(index, true);

						if (liveSetTrace.Active)
							liveSetTrace.Log("KILL: " + index.ToString() /*+ " " + op*/);
					}
				}

				block.LiveGen = liveGen;
				block.LiveKill = liveKill;
				block.LiveKillNot = ((BitArray)liveKill.Clone()).Not();

				if (liveSetTrace.Active)
				{
					liveSetTrace.Log("GEN:     " + block.LiveGen.ToString2());
					liveSetTrace.Log("KILL:    " + block.LiveKill.ToString2());
					liveSetTrace.Log("KILLNOT: " + block.LiveKillNot.ToString2());
					liveSetTrace.Log(string.Empty);
				}
			}
		}

		private void ComputeGlobalLiveSets()
		{
			bool changed = true;

			while (changed)
			{
				changed = false;

				for (int i = BasicBlocks.Count - 1; i >= 0; i--)
				{
					var block = ExtendedBlocks[i];

					var liveOut = new BitArray(SlotCount);

					foreach (var next in block.BasicBlock.NextBlocks)
					{
						liveOut.Or(ExtendedBlocks[next.Sequence].LiveIn);
					}

					var liveIn = (BitArray)block.LiveOut.Clone();
					liveIn.And(block.LiveKillNot);
					liveIn.Or(block.LiveGen);

					// compare them for any changes
					if (!changed)
					{
						if (!block.LiveOut.AreSame(liveOut) || !block.LiveIn.AreSame(liveIn))
						{
							changed = true;
						}
					}

					block.LiveOut = liveOut;
					block.LiveIn = liveIn;
				}
			}
		}

		private void BuildLiveIntervals()
		{
			var intervalTrace = CreateTrace("BuildLiveIntervals");

			for (int b = BasicBlocks.Count - 1; b >= 0; b--)
			{
				var block = ExtendedBlocks[b];

				if (intervalTrace.Active)
					intervalTrace.Log("Block # " + block.BasicBlock.Sequence.ToString());

				for (int r = 0; r < SlotCount; r++)
				{
					if (!block.LiveOut.Get(r))
						continue;

					var liveRange = LiveRanges[r];

					if (b + 1 != BasicBlocks.Count && ExtendedBlocks[b + 1].LiveIn.Get(r))
					{
						if (intervalTrace.Active) intervalTrace.Log("Add (LiveOut) " + liveRange + " : " + block.Start + " destination " + ExtendedBlocks[b + 1].Start);
						if (intervalTrace.Active) intervalTrace.Log("   Before: " + liveRange);
						liveRange.Add(block.Start, ExtendedBlocks[b + 1].Start);
						if (intervalTrace.Active) intervalTrace.Log("    After: " + liveRange);
					}
					else
					{
						if (intervalTrace.Active) intervalTrace.Log("Add (!LiveOut) " + liveRange + " : " + block.Range.Start + " destination " + block.Range.End);
						if (intervalTrace.Active) intervalTrace.Log("   Before: " + liveRange);
						liveRange.Add(block.Range);
						if (intervalTrace.Active) intervalTrace.Log("    After: " + liveRange);
					}
				}

				for (var node = block.BasicBlock.Last; !node.IsBlockStartInstruction; node = node.Previous)
				{
					if (node.IsEmpty)
						continue;

					foreach (var index in Environment.GetKill(node))
					{
						var register = LiveRanges[index];

						if (intervalTrace.Active) intervalTrace.Log("Add (Call) " + register + " : " + index + " destination " + (index + 1));
						if (intervalTrace.Active) intervalTrace.Log("   Before: " + register);
						register.Add(index, index + 1);
						if (intervalTrace.Active) intervalTrace.Log("    After: " + register);
					}

					foreach (var index in Environment.GetOutput(node))
					{
						var liveRange = LiveRanges[index];
						var first = liveRange.FirstRange;

						if (first != null)
						{
							if (intervalTrace.Active) intervalTrace.Log("Replace First " + liveRange + " : " + index + " destination " + first.End);
							if (intervalTrace.Active) intervalTrace.Log("   Before: " + liveRange);
							liveRange.FirstRange = new Range(index, first.End);
							if (intervalTrace.Active) intervalTrace.Log("    After: " + liveRange);
						}
						else
						{
							// This is necesary to handled a result that is never used!
							// This is common with instructions with more than one result.
							if (intervalTrace.Active) intervalTrace.Log("Add (Unused) " + liveRange + " : " + index + " destination " + index);
							if (intervalTrace.Active) intervalTrace.Log("   Before: " + liveRange);
							liveRange.Add(index, index + 1);
							if (intervalTrace.Active) intervalTrace.Log("    After: " + liveRange);
						}
					}

					foreach (var index in Environment.GetInput(node))
					{
						var liveRange = LiveRanges[index];

						if (intervalTrace.Active) intervalTrace.Log("Add (normal) " + liveRange + " : " + block.Start + " destination " + index);
						if (intervalTrace.Active) intervalTrace.Log("   Before: " + liveRange);
						liveRange.Add(block.Start, index);
						if (intervalTrace.Active) intervalTrace.Log("    After: " + liveRange);
					}
				}
			}
		}
	}
}
