using Luny;
using Luny.ContractTest;
using Luny.Engine.Bridge.Enums;
using NUnit.Framework;
using System;

namespace LunyScript.Test
{
	public sealed class Timer_FiresAfterDuration_LunyScript : LunyScript
	{
		public override void Build()
		{
			// Timer fires after 0.1 seconds
			var timer = Timer("test").In(100).Milliseconds().Do(GVar("TimerFired").Set(true));
			On.Ready(timer.Start());
		}
	}

	public sealed class Timer_DoesNotFireBeforeDuration_LunyScript : LunyScript
	{
		public override void Build()
		{
			// Timer fires after 1 second
			var timer = Timer("test").In(1).Seconds().Do(GVar("TimerFired").Set(true));
			On.Ready(timer.Start());
		}
	}

	public sealed class Timer_CanBeStopped_LunyScript : LunyScript
	{
		public override void Build()
		{
			var timer = Timer("test").In(100).Milliseconds().Do(GVar("TimerFired").Set(true));
			On.Ready(timer.Start());
			On.FrameUpdate(timer.Stop()); // Stop immediately on first update
		}
	}

	public sealed class Timer_CanBePaused_LunyScript : LunyScript
	{
		public override void Build()
		{
			var timer = Timer("test").In(100).Milliseconds().Do(GVar("TimerFired").Set(true));
			On.Ready(timer.Start());
			On.FrameUpdate(timer.Pause()); // Pause immediately - timer should never fire
		}
	}

	public abstract class TimerTests : ContractTestBase
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
	}

	[TestFixture]
	public sealed class GodotTimerTests : TimerTests
	{
		protected override NativeEngine Engine => NativeEngine.Godot;
	}

	[TestFixture]
	public sealed class UnityTimerTests : TimerTests
	{
		protected override NativeEngine Engine => NativeEngine.Unity;
	}
}
