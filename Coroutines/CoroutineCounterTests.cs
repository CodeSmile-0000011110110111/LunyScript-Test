using Luny;
using Luny.ContractTest;
using Luny.Engine.Bridge.Enums;
using NUnit.Framework;

namespace LunyScript.Test.Coroutines
{
	public sealed class Counter_AutoStarts_LunyScript : LunyScript
	{
		public override void Build(ScriptBuildContext context)
		{
			// It should not be necessary to call counter.Start()
			var counter = Counter(nameof(Counter_AutoStarts_LunyScript)).In(5).Frames().Do(GVar("CounterFired").Set(true));
		}
	}

	public sealed class Counter_StartsStopped_StartsLater_LunyScript : LunyScript
	{
		public override void Build(ScriptBuildContext context)
		{
			var counter = Counter("test")
				.In(10)
				.Frames()
				.Do(GVar("CounterFired").Set(true),
					GVar("CounterFired_Seconds").Set(GVar("Time.ElapsedSeconds")));

			// This should start the coroutine as stopped
			counter.Stop();

			// counter should complete within >= 50+10 ms (0.06 seconds)
			var startLater = Counter("start").In(50).Frames().Do(counter.Start());
		}
	}

	public sealed class Counter_StartsPaused_ResumeLater_LunyScript : LunyScript
	{
		public override void Build(ScriptBuildContext context)
		{
			var counter = Counter(nameof(Counter_StartsPaused_ResumeLater_LunyScript))
				.In(3)
				.Frames()
				.Do(GVar("CounterFired").Set(true),
					GVar("CounterFired_Seconds").Set(GVar("Time.ElapsedSeconds")));

			// This should start the coroutine as paused
			On.Ready(counter.Pause());

			// counter should complete after >= 70+50 ms (0.12 seconds)
			var resumeLater = Counter("RESUME_StartsPaused_ResumeLater_LunyScript").In(6).Frames().Do(counter.Resume());
		}
	}

	public sealed class Counter_PausedLater_ResumeLater_LunyScript : LunyScript
	{
		public override void Build(ScriptBuildContext context)
		{
			var counter = Counter("test")
				.In(6)
				.Frames()
				.Do(GVar("CounterFired").Set(true),
					GVar("CounterFired_Seconds").Set(GVar("Time.ElapsedSeconds")));

			// This should pause about quarter-way
			var pauseLater = Counter("pause").In(3).Frames().Do(counter.Pause());

			// resume remaining 25 ms after 30 ms to complete after >= 30+25 ms (0.055 seconds)
			var resumeLater = Counter("resume").In(9).Frames().Do(counter.Resume());
		}
	}

	public sealed class Counter_FiresAfterDuration_LunyScript : LunyScript
	{
		public override void Build(ScriptBuildContext context)
		{
			// Counter fires after 0.1 seconds
			var counter = Counter("test").In(8).Frames().Do(GVar("CounterFired").Set(true));
			On.Ready(counter.Start());
		}
	}

	public sealed class Counter_DoesNotFireBeforeDuration_LunyScript : LunyScript
	{
		public override void Build(ScriptBuildContext context)
		{
			var counter = Counter("test").In(60).Frames().Do(GVar("CounterFired").Set(true));
			On.Ready(counter.Start());
		}
	}

	public sealed class Counter_CanBeStopped_LunyScript : LunyScript
	{
		public override void Build(ScriptBuildContext context)
		{
			var counter = Counter("test").In(1).Frames().Do(GVar("CounterFired").Set(true), Debug.LogInfo("COUNTER FIRED"));
			On.FrameUpdate(counter.Stop(), Debug.LogInfo("FRAME UPDATE")); // Stop immediately on first update
		}
	}

	public sealed class Counter_CanBePaused_LunyScript : LunyScript
	{
		public override void Build(ScriptBuildContext context)
		{
			var counter = Counter("test").In(1).Frames().Do(GVar("CounterFired").Set(true));
			On.Ready(counter.Pause()); // Pause immediately - counter should never fire
		}
	}

	public sealed class Counter_Repeating_FiresMultipleTimes_LunyScript : LunyScript
	{
		public override void Build(ScriptBuildContext context)
		{
			var counter = Counter("test").Every(10).Frames().Do(GVar("Counter").Inc());
			On.Ready(counter.Start());
		}
	}

	public abstract class CoroutineCounterTests : ContractTestBase
	{
		[Test]
		public void Counter_AutoStarts()
		{
			LunyEngine.Instance.Object.CreateEmpty(nameof(Counter_AutoStarts_LunyScript));
			var gVars = LunyScriptEngine.Instance.GlobalVariables;

			SimulateFrames(10);

			Assert.That(gVars["CounterFired"].AsBoolean(), Is.EqualTo(true));
		}

		[Test]
		public void Counter_FiresAfterDuration()
		{
			LunyEngine.Instance.Object.CreateEmpty(nameof(Counter_FiresAfterDuration_LunyScript));
			var gVars = LunyScriptEngine.Instance.GlobalVariables;

			SimulateFrames(10);

			Assert.That(gVars["CounterFired"], Is.EqualTo((Variable)true));
		}

		[Test]
		public void Counter_DoesNotFireBeforeDuration()
		{
			LunyEngine.Instance.Object.CreateEmpty(nameof(Counter_DoesNotFireBeforeDuration_LunyScript));
			var gVars = LunyScriptEngine.Instance.GlobalVariables;

			SimulateFrames(30);

			Assert.That(gVars["CounterFired"].AsBoolean(), Is.EqualTo(false));
		}

		[Test]
		public void Counter_CanBeStopped()
		{
			LunyEngine.Instance.Object.CreateEmpty(nameof(Counter_CanBeStopped_LunyScript));
			var gVars = LunyScriptEngine.Instance.GlobalVariables;

			// Simulate well past when counter would fire
			SimulateFrames(10);

			// Counter was stopped, should not have fired
			Assert.That(gVars["CounterFired"].AsBoolean(), Is.EqualTo(false));
		}

		[Test]
		public void Counter_CanBePaused()
		{
			LunyEngine.Instance.Object.CreateEmpty(nameof(Counter_CanBePaused_LunyScript));
			var gVars = LunyScriptEngine.Instance.GlobalVariables;

			// Simulate well past when counter would fire
			SimulateFrames(20);

			// Counter was paused, should not have fired
			Assert.That(gVars["CounterFired"].AsBoolean(), Is.EqualTo(false));
		}

		[Test]
		public void Counter_Repeating_FiresMultipleTimes()
		{
			LunyEngine.Instance.Object.CreateEmpty(nameof(Counter_Repeating_FiresMultipleTimes_LunyScript));
			var gVars = LunyScriptEngine.Instance.GlobalVariables;

			SimulateFrames(40);

			Assert.That(gVars["Counter"].AsInt32(), Is.GreaterThanOrEqualTo(4));
		}
		[Test]
		public void Counter_PausedLater_ResumeLater()
		{
			LunyEngine.Instance.Object.CreateEmpty(nameof(Counter_PausedLater_ResumeLater_LunyScript));
			var gVars = LunyScriptEngine.Instance.GlobalVariables;

			SimulateFrames(12);

			Assert.That(gVars["CounterFired"].AsBoolean(), Is.EqualTo(true));
			Assert.That(gVars["CounterFired_Seconds"].AsDouble(), Is.GreaterThanOrEqualTo(0.055));
		}

		[Test]
		public void Counter_StartsPaused_ResumeLater()
		{
			LunyEngine.Instance.Object.CreateEmpty(nameof(Counter_StartsPaused_ResumeLater_LunyScript));
			var gVars = LunyScriptEngine.Instance.GlobalVariables;

			SimulateFrames(10);

			Assert.That(gVars["CounterFired"].AsBoolean(), Is.EqualTo(true));
			Assert.That(gVars["CounterFired_Seconds"].AsDouble(), Is.GreaterThanOrEqualTo(0.12));
		}

		[Test]
		public void Counter_StartsStopped_StartsLater()
		{
			LunyEngine.Instance.Object.CreateEmpty(nameof(Counter_StartsStopped_StartsLater_LunyScript));
			var gVars = LunyScriptEngine.Instance.GlobalVariables;

			// Counter fires after 50ms (start) + 10ms (test) = 60ms
			SimulateFrames(10);

			Assert.That(gVars["CounterFired"].AsBoolean(), Is.EqualTo(true));
			Assert.That(gVars["CounterFired_Seconds"].AsDouble(), Is.GreaterThanOrEqualTo(0.06));
		}

	}

	[TestFixture]
	public sealed class GodotCoroutineCounterTests : CoroutineCounterTests
	{
		protected override NativeEngine Engine => NativeEngine.Godot;
	}

	[TestFixture]
	public sealed class UnityCoroutineCounterTests : CoroutineCounterTests
	{
		protected override NativeEngine Engine => NativeEngine.Unity;
	}
}
