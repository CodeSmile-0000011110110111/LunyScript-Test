using Luny;
using Luny.ContractTest;
using Luny.Engine.Bridge.Enums;
using NUnit.Framework;

namespace LunyScript.Test.Coroutines
{
	public sealed class Every2Frames_RunsAlternating_LunyScript : LunyScript
	{
		public override void Build()
		{
			Every(2).Frames().Do(GVar("Counter").Add(1));
		}
	}

	public sealed class Every3Heartbeats_RunsEveryThird_LunyScript : LunyScript
	{
		public override void Build()
		{
			Every(3).Heartbeats().Do(GVar("Counter").Add(1));
		}
	}

	public sealed class EveryEvenFrames_RunsOnEven_LunyScript : LunyScript
	{
		public override void Build()
		{
			Every(Even).Frames().Do(GVar("Counter").Add(1));
		}
	}

	public sealed class EveryOddHeartbeats_RunsOnOdd_LunyScript : LunyScript
	{
		public override void Build()
		{
			Every(Odd).Heartbeats().Do(GVar("Counter").Add(1));
		}
	}

	public sealed class Every2FramesDelayBy1_RunsAlternatingOffset_LunyScript : LunyScript
	{
		public override void Build()
		{
			Every(2).Frames().DelayBy(1).Do(GVar("Counter").Add(1));
		}
	}

	public abstract class CoroutineTimeSliceTests : ContractTestBase
	{
		[Test]
		public void Every2Frames_RunsAlternating()
		{
			LunyEngine.Instance.Object.CreateEmpty(nameof(Every2Frames_RunsAlternating_LunyScript));
			var gVars = LunyScriptEngine.Instance.GlobalVariables;

			SimulateFrames(6);

			// In 6 frames, with every 2nd frame running, should run ~3 times
			// (frame 2, 4, 6 if counting starts at 1)
			var counter = gVars["Counter"].AsInt32();
			Assert.That(counter, Is.InRange(2, 4), $"Expected counter ~3, got {counter}");
		}

		[Test]
		public void Every3Heartbeats_RunsEveryThird()
		{
			LunyEngine.Instance.Object.CreateEmpty(nameof(Every3Heartbeats_RunsEveryThird_LunyScript));
			var gVars = LunyScriptEngine.Instance.GlobalVariables;

			SimulateFrames(9);

			// In 9 heartbeats, every 3rd should run, so ~3 times
			var counter = gVars["Counter"].AsInt32();
			Assert.That(counter, Is.InRange(2, 4), $"Expected counter ~3, got {counter}");
		}

		[Test]
		public void EveryEvenFrames_RunsOnEven()
		{
			LunyEngine.Instance.Object.CreateEmpty(nameof(EveryEvenFrames_RunsOnEven_LunyScript));
			var gVars = LunyScriptEngine.Instance.GlobalVariables;

			SimulateFrames(8);

			// In 8 frames, even frames (2, 4, 6, 8) should run, so ~4 times
			var counter = gVars["Counter"].AsInt32();
			Assert.That(counter, Is.InRange(3, 5), $"Expected counter ~4, got {counter}");
		}

		[Test]
		public void EveryOddHeartbeats_RunsOnOdd()
		{
			LunyEngine.Instance.Object.CreateEmpty(nameof(EveryOddHeartbeats_RunsOnOdd_LunyScript));
			var gVars = LunyScriptEngine.Instance.GlobalVariables;

			SimulateFrames(8);

			// In 8 heartbeats, odd heartbeats (1, 3, 5, 7) should run, so ~4 times
			var counter = gVars["Counter"].AsInt32();
			Assert.That(counter, Is.InRange(3, 5), $"Expected counter ~4, got {counter}");
		}

		[Test]
		public void Every2FramesDelayBy1_RunsAlternatingOffset()
		{
			LunyEngine.Instance.Object.CreateEmpty(nameof(Every2FramesDelayBy1_RunsAlternatingOffset_LunyScript));
			var gVars = LunyScriptEngine.Instance.GlobalVariables;

			SimulateFrames(6);

			// In 6 frames with delay offset 1, should run on frames 1, 3, 5 (odd frames), so ~3 times
			var counter = gVars["Counter"].AsInt32();
			Assert.That(counter, Is.InRange(2, 4), $"Expected counter ~3, got {counter}");
		}
	}

	[TestFixture]
	public sealed class GodotCoroutineTimeSliceTests : CoroutineTimeSliceTests
	{
		protected override NativeEngine Engine => NativeEngine.Godot;
	}

	[TestFixture]
	public sealed class UnityCoroutineTimeSliceTests : CoroutineTimeSliceTests
	{
		protected override NativeEngine Engine => NativeEngine.Unity;
	}
}
