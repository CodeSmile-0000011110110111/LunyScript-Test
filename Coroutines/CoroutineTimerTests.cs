using Luny;
using Luny.ContractTest;
using Luny.Engine.Bridge.Enums;
using NUnit.Framework;

namespace LunyScript.Test.Coroutines
{
	public sealed class Timer_AutoStarts_LunyScript : LunyScript
	{
		public override void Build(ScriptBuildContext context)
		{
			// It should not be necessary to call timer.Start()
			var timer = Timer("test").In(50).Milliseconds().Do(GVar("TimerFired").Set(true));
		}
	}

	public sealed class Timer_StartsStopped_StartsLater_LunyScript : LunyScript
	{
		public override void Build(ScriptBuildContext context)
		{
			var timer = Timer("test")
				.In(10)
				.Milliseconds()
				.Do(GVar("TimerFired").Set(true),
					GVar("TimerFired_Seconds").Set(GVar("Time.ElapsedSeconds")));

			// This should start the coroutine as stopped
			timer.Stop();

			// timer should complete within >= 50+10 ms (0.06 seconds)
			var startLater = Timer("start").In(50).Milliseconds().Do(timer.Start());
		}
	}

	public sealed class Timer_StartsPaused_ResumeLater_LunyScript : LunyScript
	{
		public override void Build(ScriptBuildContext context)
		{
			var timer = Timer("TIMER_StartsPaused_ResumeLater_LunyScript")
				.In(50)
				.Milliseconds()
				.Do(GVar("TimerFired").Set(true),
					GVar("TimerFired_Seconds").Set(GVar("Time.ElapsedSeconds")));

			// This should start the coroutine as paused
			On.Ready(timer.Pause());

			// timer should complete after >= 70+50 ms (0.12 seconds)
			var resumeLater = Timer("RESUME_StartsPaused_ResumeLater_LunyScript").In(70).Milliseconds().Do(timer.Resume());
		}
	}

	public sealed class Timer_PausedLater_ResumeLater_LunyScript : LunyScript
	{
		public override void Build(ScriptBuildContext context)
		{
			var timer = Timer("test")
				.In(40)
				.Milliseconds()
				.Do(GVar("TimerFired").Set(true),
					GVar("TimerFired_Seconds").Set(GVar("Time.ElapsedSeconds")));

			// This should pause about quarter-way
			var pauseLater = Timer("pause").In(15).Milliseconds().Do(timer.Pause());

			// resume remaining 25 ms after 30 ms to complete after >= 30+25 ms (0.055 seconds)
			var resumeLater = Timer("resume").In(30).Milliseconds().Do(timer.Resume());
		}
	}

	public sealed class Timer_LowerTimeScale_TakesLonger_LunyScript : LunyScript
	{
		public override void Build(ScriptBuildContext context)
		{
			// this should complete after >= 200 ms
			var timer = Timer("test").In(100).Milliseconds().Do(GVar("TimerFired").Set(true));
			On.Ready(timer.TimeScale(0.5));
		}
	}

	public sealed class Timer_FiresAfterDuration_LunyScript : LunyScript
	{
		public override void Build(ScriptBuildContext context)
		{
			// Timer fires after 0.1 seconds
			var timer = Timer("test").In(100).Milliseconds().Do(GVar("TimerFired").Set(true));
			On.Ready(timer.Start());
		}
	}

	public sealed class Timer_DoesNotFireBeforeDuration_LunyScript : LunyScript
	{
		public override void Build(ScriptBuildContext context)
		{
			// Timer fires after 1 second
			var timer = Timer("test").In(1).Seconds().Do(GVar("TimerFired").Set(true));
			On.Ready(timer.Start());
		}
	}

	public sealed class Timer_CanBeStopped_LunyScript : LunyScript
	{
		public override void Build(ScriptBuildContext context)
		{
			var timer = Timer("test").In(100).Milliseconds().Do(GVar("TimerFired").Set(true));
			On.Ready(timer.Start());
			On.FrameUpdate(timer.Stop()); // Stop immediately on first update
		}
	}

	public sealed class Timer_CanBePaused_LunyScript : LunyScript
	{
		public override void Build(ScriptBuildContext context)
		{
			var timer = Timer("test").In(100).Milliseconds().Do(GVar("TimerFired").Set(true));
			On.Ready(timer.Start());
			On.FrameUpdate(timer.Pause()); // Pause immediately - timer should never fire
		}
	}

	public sealed class Timer_Repeating_FiresMultipleTimes_LunyScript : LunyScript
	{
		public override void Build(ScriptBuildContext context)
		{
			// Timer fires every 100ms
			var timer = Timer("test").Every(100).Milliseconds().Do(GVar("Counter").Inc());
			On.Ready(timer.Start());
		}
	}

	public abstract class CoroutineTimerTests : ContractTestBase
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
		public void Timer_AutoStarts()
		{
			LunyEngine.Instance.Object.CreateEmpty(nameof(Timer_AutoStarts_LunyScript));
			var gVars = LunyScriptEngine.Instance.GlobalVariables;

			// 50ms = 3 frames at 60fps.
			SimulateFrames(10);

			Assert.That(gVars["TimerFired"].AsBoolean(), Is.EqualTo(true));
		}

		[Test]
		public void Timer_StartsStopped_StartsLater()
		{
			LunyEngine.Instance.Object.CreateEmpty(nameof(Timer_StartsStopped_StartsLater_LunyScript));
			var gVars = LunyScriptEngine.Instance.GlobalVariables;

			// Timer fires after 50ms (start) + 10ms (test) = 60ms
			SimulateFrames(10);

			Assert.That(gVars["TimerFired"].AsBoolean(), Is.EqualTo(true));
			Assert.That(gVars["TimerFired_Seconds"].AsDouble(), Is.GreaterThanOrEqualTo(0.06));
		}

		[Test]
		public void Timer_StartsPaused_ResumeLater()
		{
			LunyEngine.Instance.Object.CreateEmpty(nameof(Timer_StartsPaused_ResumeLater_LunyScript));
			var gVars = LunyScriptEngine.Instance.GlobalVariables;

			// Timer fires after 70ms (resume) + 50ms (test) = 120ms
			SimulateFrames(10);

			Assert.That(gVars["TimerFired"].AsBoolean(), Is.EqualTo(true));
			Assert.That(gVars["TimerFired_Seconds"].AsDouble(), Is.GreaterThanOrEqualTo(0.12));
		}

		[Test]
		public void Timer_PausedLater_ResumeLater()
		{
			LunyEngine.Instance.Object.CreateEmpty(nameof(Timer_PausedLater_ResumeLater_LunyScript));
			var gVars = LunyScriptEngine.Instance.GlobalVariables;

			// Timer fires after 30ms (resume) + 25ms (remaining) = 55ms
			SimulateFrames(10);

			Assert.That(gVars["TimerFired"].AsBoolean(), Is.EqualTo(true));
			Assert.That(gVars["TimerFired_Seconds"].AsDouble(), Is.GreaterThanOrEqualTo(0.055));
		}

		[Test]
		public void Timer_LowerTimeScale_TakesLonger()
		{
			LunyEngine.Instance.Object.CreateEmpty(nameof(Timer_LowerTimeScale_TakesLonger_LunyScript));
			var gVars = LunyScriptEngine.Instance.GlobalVariables;

			// 100ms at 0.5 scale = 200ms. 10 frames = 166ms.
			SimulateFrames(10);
			Assert.That(gVars["TimerFired"].AsBoolean(), Is.EqualTo(false));

			// Another 5 frames = 15 frames = 250ms.
			SimulateFrames(5);
			Assert.That(gVars["TimerFired"].AsBoolean(), Is.EqualTo(true));
		}
	}

	[TestFixture]
	public sealed class GodotCoroutineTimerTests : CoroutineTimerTests
	{
		protected override NativeEngine Engine => NativeEngine.Godot;
	}

	[TestFixture]
	public sealed class UnityCoroutineTimerTests : CoroutineTimerTests
	{
		protected override NativeEngine Engine => NativeEngine.Unity;
	}
}
