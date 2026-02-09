using Luny;
using Luny.ContractTest;
using Luny.Engine.Bridge.Enums;
using NUnit.Framework;

namespace LunyScript.Test.Coroutines
{
	public sealed class Counter_AutoStarts_LunyScript : LunyScript
	{
		public override void Build()
		{
			// It should not be necessary to call timer.Start()
			var counter = Counter("test").In(5).Frames().Do(GVar("TimerFired").Set(true));
		}
	}


	public abstract class CoroutineCounterTests : ContractTestBase
	{
		[Test]
		public void Timer_FiresAfterDuration()
		{
			LunyEngine.Instance.Object.CreateEmpty(nameof(Timer_FiresAfterDuration_LunyScript));
			var gVars = LunyScriptEngine.Instance.GlobalVariables;

			// 100ms = 6 frames at 60fps (16.67ms/frame)
			SimulateFrames(10);

			Assert.That(gVars["TimerFired"], Is.EqualTo((Variable)true));
		}

		[Test]
		public void Timer_DoesNotFireBeforeDuration()
		{
			LunyEngine.Instance.Object.CreateEmpty(nameof(Timer_DoesNotFireBeforeDuration_LunyScript));
			var gVars = LunyScriptEngine.Instance.GlobalVariables;

			// Only simulate 0.5 seconds (30 frames at 60fps) - timer is set for 1 second
			SimulateFrames(30);

			Assert.That(gVars["TimerFired"].AsBoolean(), Is.EqualTo(false));
		}

		[Test]
		public void Timer_CanBeStopped()
		{
			LunyEngine.Instance.Object.CreateEmpty(nameof(Timer_CanBeStopped_LunyScript));
			var gVars = LunyScriptEngine.Instance.GlobalVariables;

			// Simulate well past when timer would fire
			SimulateFrames(20);

			// Timer was stopped, should not have fired
			Assert.That(gVars["TimerFired"].AsBoolean(), Is.EqualTo(false));
		}

		[Test]
		public void Timer_CanBePaused()
		{
			LunyEngine.Instance.Object.CreateEmpty(nameof(Timer_CanBePaused_LunyScript));
			var gVars = LunyScriptEngine.Instance.GlobalVariables;

			// Simulate well past when timer would fire
			SimulateFrames(20);

			// Timer was paused, should not have fired
			Assert.That(gVars["TimerFired"].AsBoolean(), Is.EqualTo(false));
		}

		[Test]
		public void Timer_Repeating_FiresMultipleTimes()
		{
			LunyEngine.Instance.Object.CreateEmpty(nameof(Timer_Repeating_FiresMultipleTimes_LunyScript));
			var gVars = LunyScriptEngine.Instance.GlobalVariables;

			// 100ms = ~6 frames. 40 frames should be ~6-7 fires.
			SimulateFrames(40);

			Assert.That(gVars["Counter"].AsInt32(), Is.GreaterThanOrEqualTo(5));
		}

		[Test]
		public void Counter_AutoStarts()
		{
			LunyEngine.Instance.Object.CreateEmpty(nameof(Counter_AutoStarts_LunyScript));
			var gVars = LunyScriptEngine.Instance.GlobalVariables;

			// 5 frames
			SimulateFrames(10);

			Assert.That(gVars["TimerFired"].AsBoolean(), Is.EqualTo(true));
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
