using Luny;
using Luny.ContractTest;
using Luny.Engine.Bridge.Enums;
using LunyScript.Activation;
using NUnit.Framework;
using System;

namespace LunyScript.Test.Coroutines
{
	public sealed class Coroutine_UsedAsBlock_Throws_LunyScript : Script
	{
		public override void Build(ScriptContext context)
		{
			var counter = Coroutine("throws")
				.OnFrameUpdate(GVar("Counter").Add(1))
				.Build();

			On.Ready(counter); // throws
		}
	}

	public sealed class Coroutine_OnUpdate_RunsEveryFrame_LunyScript : Script
	{
		public override void Build(ScriptContext context)
		{
			var counter = Coroutine("counter")
				.OnFrameUpdate(GVar("Counter").Add(1))
				.Build();
		}
	}

	public sealed class Coroutine_OnHeartbeat_RunsEveryStep_LunyScript : Script
	{
		public override void Build(ScriptContext context)
		{
			var counter = Coroutine(nameof(Coroutine_OnHeartbeat_RunsEveryStep_LunyScript))
				.OnHeartbeat(GVar("Counter").Add(1))
				.Build();
		}
	}

	public sealed class Coroutine_Duration_FiresElapsed_LunyScript : Script
	{
		public override void Build(ScriptContext context)
		{
			var co = Coroutine("timed")
				.For(2)
				.Seconds()
				.OnFrameUpdate(GVar("Ticks").Inc())
				.Elapsed(GVar("Elapsed").Set(true));
		}
	}

	public sealed class Coroutine_Duration_DoesNotFireEarly_LunyScript : Script
	{
		public override void Build(ScriptContext context)
		{
			var co = Coroutine("timed")
				.For(5)
				.Seconds()
				.Elapsed(GVar("Elapsed").Set(true));
		}
	}

	public sealed class Coroutine_Started_FiresOnStart_LunyScript : Script
	{
		public override void Build(ScriptContext context)
		{
			var co = Coroutine(nameof(Coroutine_Started_FiresOnStart_LunyScript))
				.For(10)
				.Seconds()
				.Started(GVar("Started").Set(true))
				.Build();
		}
	}

	public sealed class Coroutine_Stopped_FiresOnStop_LunyScript : Script
	{
		public override void Build(ScriptContext context)
		{
			var co = Coroutine("controlled")
				.For(10)
				.Seconds()
				.Stopped(GVar("Stopped").Set(true))
				.Build();

			// immediately stop
			On.Ready(co.Stop());
		}
	}

	public sealed class Coroutine_Paused_FiresOnPause_LunyScript : Script
	{
		public override void Build(ScriptContext context)
		{
			var co = Coroutine("controlled")
				.For(10)
				.Seconds()
				.Paused(GVar("Paused").Set(true))
				.Build();

			// immediately pause
			On.Ready(co.Pause());
		}
	}

	public sealed class Coroutine_Resumed_FiresOnResume_LunyScript : Script
	{
		public override void Build(ScriptContext context)
		{
			var co = Coroutine("controlled")
				.For(10)
				.Seconds()
				.Resumed(GVar("Resumed").Set(true))
				.Build();

			// pause, then resume
			On.Ready(co.Pause(), co.Resume());
		}
	}

	public abstract class CoroutineBaseTests : ContractTestBase
	{
		[Test]
		public void Coroutine_UsedAsBlock_Throws()
		{
			LunyEngine.Instance.Object.CreateEmpty(nameof(Coroutine_UsedAsBlock_Throws_LunyScript));
			Assert.Throws<NotImplementedException>(() => SimulateFrames(1));
		}

		[Test]
		public void Coroutine_OnUpdate_RunsEveryFrame()
		{
			LunyEngine.Instance.Object.CreateEmpty(nameof(Coroutine_OnUpdate_RunsEveryFrame_LunyScript));
			var gVars = ScriptEngine.Instance.GlobalVariables;

			SimulateFrames(5);

			// Should have run 5 times (once per frame after start)
			Assert.That(gVars["Counter"].AsInt32(), Is.GreaterThanOrEqualTo(4));
		}

		[Test]
		public void Coroutine_OnHeartbeat_RunsEveryStep()
		{
			LunyEngine.Instance.Object.CreateEmpty(nameof(Coroutine_OnHeartbeat_RunsEveryStep_LunyScript));
			var gVars = ScriptEngine.Instance.GlobalVariables;

			SimulateFrames(5);

			// Should have run multiple times (heartbeat runs per frame in tests)
			Assert.That(gVars["Counter"].AsInt32(), Is.GreaterThanOrEqualTo(4));
		}

		[Test]
		public void Coroutine_Duration_FiresElapsed()
		{
			LunyEngine.Instance.Object.CreateEmpty(nameof(Coroutine_Duration_FiresElapsed_LunyScript));
			var gVars = ScriptEngine.Instance.GlobalVariables;

			// 2 seconds at 60fps = 120 frames + buffer
			SimulateFrames(125);

			Assert.That(gVars["Ticks"].AsInt32(), Is.EqualTo(120));
			Assert.That(gVars["Elapsed"].AsBoolean(), Is.True);
		}

		[Test]
		public void Coroutine_Duration_DoesNotFireEarly()
		{
			LunyEngine.Instance.Object.CreateEmpty(nameof(Coroutine_Duration_DoesNotFireEarly_LunyScript));
			var gVars = ScriptEngine.Instance.GlobalVariables;

			// Only 2 seconds at 60fps = 120 frames, duration is 5 seconds
			SimulateFrames(120);

			Assert.That(gVars["Elapsed"].AsBoolean(), Is.False);
		}

		[Test]
		public void Coroutine_Started_FiresOnStart()
		{
			LunyEngine.Instance.Object.CreateEmpty(nameof(Coroutine_Started_FiresOnStart_LunyScript));
			var gVars = ScriptEngine.Instance.GlobalVariables;

			SimulateFrames(2);

			Assert.That(gVars["Started"].AsBoolean(), Is.True);
		}

		[Test]
		public void Coroutine_Stopped_FiresOnStop()
		{
			LunyEngine.Instance.Object.CreateEmpty(nameof(Coroutine_Stopped_FiresOnStop_LunyScript));
			var gVars = ScriptEngine.Instance.GlobalVariables;

			SimulateFrames(2);

			Assert.That(gVars["Stopped"].AsBoolean(), Is.True);
		}

		[Test]
		public void Coroutine_Paused_FiresOnPause()
		{
			LunyEngine.Instance.Object.CreateEmpty(nameof(Coroutine_Paused_FiresOnPause_LunyScript));
			var gVars = ScriptEngine.Instance.GlobalVariables;

			SimulateFrames(2);

			Assert.That(gVars["Paused"].AsBoolean(), Is.True);
		}

		[Test]
		public void Coroutine_Resumed_FiresOnResume()
		{
			LunyEngine.Instance.Object.CreateEmpty(nameof(Coroutine_Resumed_FiresOnResume_LunyScript));
			var gVars = ScriptEngine.Instance.GlobalVariables;

			SimulateFrames(2);

			Assert.That(gVars["Resumed"].AsBoolean(), Is.True);
		}
	}

	[TestFixture]
	public sealed class CoroutineBaseGodotTests : CoroutineBaseTests
	{
		protected override NativeEngine Engine => NativeEngine.Godot;
	}

	[TestFixture]
	public sealed class CoroutineBaseUnityTests : CoroutineBaseTests
	{
		protected override NativeEngine Engine => NativeEngine.Unity;
	}
}
