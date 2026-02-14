using Luny;
using Luny.ContractTest;
using Luny.Engine.Bridge;
using LunyScript.Activation;
using NUnit.Framework;

namespace LunyScript.Test.Coroutines
{
	public sealed class Counter_AutoStarts_LunyScript : Script
	{
		public override void Build(ScriptContext context)
		{
			// It should not be necessary to call counter.Start()
			var counter = Counter(nameof(Counter_AutoStarts_LunyScript)).In(5).Frames().Do(GVar["CounterFired"].Set(true));
		}
	}

	public sealed class Counter_StartsStopped_StartsLater_LunyScript : Script
	{
		public override void Build(ScriptContext context)
		{
			var counter = Counter("test")
				.In(5)
				.Frames()
				.Do(GVar["CounterFired"].Set(true));

			// This should start the coroutine as stopped
			On.Ready(counter.Stop());

			// This counter processes after the one above, thus when it calls Start()
			// the above counter has already processed in the current frame,
			// causing a 1-frame delay in the resumption of the Counter.
			var startLater = Counter("start").In(8).Frames().Do(counter.Start());
		}
	}

	public sealed class Counter_StartsPaused_ResumeLater_LunyScript : Script
	{
		public override void Build(ScriptContext context)
		{
			var counter = Counter(nameof(Counter_StartsPaused_ResumeLater_LunyScript))
				.In(5)
				.Heartbeats()
				.Do(GVar["CounterFired"].Set(true));

			On.Ready(counter.Pause());

			// This counter processes after the one above, thus when it calls Resume()
			// the above counter has already processed in the current frame,
			// causing a 1-frame delay in the resumption of the Counter.
			var resumeLater = Counter("RESUME").In(8).Frames().Do(counter.Resume());
		}
	}

	public sealed class Counter_PausedLater_ResumeLater_LunyScript : Script
	{
		public override void Build(ScriptContext context)
		{
			var counter = Counter("test")
				.In(9)
				.Frames()
				.Do(GVar["CounterFired"].Set(true));

			var pauseLater = Counter("pause").In(5).Frames().Do(counter.Pause());
			var resumeLater = Counter("resume").In(10).Frames().Do(counter.Resume());
		}
	}

	public sealed class Counter_FiresAfterDuration_LunyScript : Script
	{
		public override void Build(ScriptContext context)
		{
			var counter = Counter("test").In(8).Frames().Do(GVar["CounterFired"].Set(true));
			On.Ready(counter.Start());
		}
	}

	public sealed class Counter_DoesNotFireBeforeDuration_LunyScript : Script
	{
		public override void Build(ScriptContext context)
		{
			var counter = Counter("test").In(60).Frames().Do(GVar["CounterFired"].Set(true));
			On.Ready(counter.Start());
		}
	}

	public sealed class Counter_CanBeStopped_LunyScript : Script
	{
		public override void Build(ScriptContext context)
		{
			var counter = Counter("test").In(1).Frames().Do(GVar["CounterFired"].Set(true), Debug.LogInfo("COUNTER FIRED"));
			On.FrameUpdate(counter.Stop(), Debug.LogInfo("FRAME UPDATE")); // Stop immediately on first update
		}
	}

	public sealed class Counter_CanBePaused_LunyScript : Script
	{
		public override void Build(ScriptContext context)
		{
			var counter = Counter("test").In(1).Frames().Do(GVar["CounterFired"].Set(true));
			On.Ready(counter.Pause()); // Pause immediately - counter should never fire
		}
	}

	public sealed class Counter_Repeating_FiresMultipleTimes_LunyScript : Script
	{
		public override void Build(ScriptContext context)
		{
			var counter = Counter("test").Every(9).Frames().Do(GVar["Counter"].Inc());
			On.Ready(counter.Start());
		}
	}

	public abstract class CoroutineCounterTests : ContractTestBase
	{
		[Test]
		public void Counter_AutoStarts()
		{
			LunyEngine.Instance.Object.CreateEmpty(nameof(Counter_AutoStarts_LunyScript));
			var gVars = ScriptEngine.Instance.GlobalVariables;

			SimulateFrames(10);

			Assert.That(gVars["CounterFired"].AsBoolean(), Is.EqualTo(true));
		}

		[Test]
		public void Counter_FiresAfterDuration()
		{
			LunyEngine.Instance.Object.CreateEmpty(nameof(Counter_FiresAfterDuration_LunyScript));
			var gVars = ScriptEngine.Instance.GlobalVariables;

			SimulateFrames(10);

			Assert.That(gVars["CounterFired"], Is.EqualTo((Variable)true));
		}

		[Test]
		public void Counter_DoesNotFireBeforeDuration()
		{
			LunyEngine.Instance.Object.CreateEmpty(nameof(Counter_DoesNotFireBeforeDuration_LunyScript));
			var gVars = ScriptEngine.Instance.GlobalVariables;

			SimulateFrames(30);

			Assert.That(gVars["CounterFired"].AsBoolean(), Is.EqualTo(false));
		}

		[Test]
		public void Counter_CanBeStopped()
		{
			LunyEngine.Instance.Object.CreateEmpty(nameof(Counter_CanBeStopped_LunyScript));
			var gVars = ScriptEngine.Instance.GlobalVariables;

			// Simulate well past when counter would fire
			SimulateFrames(10);

			// Counter was stopped, should not have fired
			Assert.That(gVars["CounterFired"].AsBoolean(), Is.EqualTo(false));
		}

		[Test]
		public void Counter_CanBePaused()
		{
			LunyEngine.Instance.Object.CreateEmpty(nameof(Counter_CanBePaused_LunyScript));
			var gVars = ScriptEngine.Instance.GlobalVariables;

			// Simulate well past when counter would fire
			SimulateFrames(20);

			// Counter was paused, should not have fired
			Assert.That(gVars["CounterFired"].AsBoolean(), Is.EqualTo(false));
		}

		[Test]
		public void Counter_Repeating_FiresMultipleTimes()
		{
			LunyEngine.Instance.Object.CreateEmpty(nameof(Counter_Repeating_FiresMultipleTimes_LunyScript));
			var gVars = ScriptEngine.Instance.GlobalVariables;

			SimulateFrames(40);

			Assert.That(gVars["Counter"].AsInt32(), Is.GreaterThanOrEqualTo(4));
		}

		[Test]
		public void Counter_PausedLater_ResumeLater()
		{
			LunyEngine.Instance.Object.CreateEmpty(nameof(Counter_PausedLater_ResumeLater_LunyScript));
			var gVars = ScriptEngine.Instance.GlobalVariables;

			SimulateFrames(15);

			Assert.That(gVars["CounterFired"].AsBoolean(), Is.EqualTo(true));
		}

		[Test]
		public void Counter_StartsPaused_ResumeLater()
		{
			LunyEngine.Instance.Object.CreateEmpty(nameof(Counter_StartsPaused_ResumeLater_LunyScript));
			var gVars = ScriptEngine.Instance.GlobalVariables;

			SimulateFrames(15);

			Assert.That(gVars["CounterFired"].AsBoolean(), Is.EqualTo(true));
		}

		[Test]
		public void Counter_StartsStopped_StartsLater()
		{
			LunyEngine.Instance.Object.CreateEmpty(nameof(Counter_StartsStopped_StartsLater_LunyScript));
			var gVars = ScriptEngine.Instance.GlobalVariables;

			SimulateFrames(15);

			Assert.That(gVars["CounterFired"].AsBoolean(), Is.EqualTo(true));
		}
	}

	[TestFixture]
	public sealed class CoroutineCounterGodotTests : CoroutineCounterTests
	{
		protected override NativeEngine Engine => NativeEngine.Godot;
	}

	[TestFixture]
	public sealed class CoroutineCounterUnityTests : CoroutineCounterTests
	{
		protected override NativeEngine Engine => NativeEngine.Unity;
	}
}
