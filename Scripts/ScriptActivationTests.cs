using Luny;
using Luny.ContractTest;
using Luny.Engine.Bridge.Enums;
using NUnit.Framework;
using System;

namespace LunyScript.Test.Scripts
{
	public sealed class Object_SpawnLater_RunsScript_LunyScript : Script
	{
		public override void Build(ScriptContext context)
		{
			var counter = Counter("spawn").In(10).Frames().Do(Object.Create(nameof(CountEvents_LunyScript)));
		}
	}

	public sealed class CountEvents_LunyScript : Script
	{
		public override void Build(ScriptContext context)
		{
			LunyLogger.LogInfo("-------------------------------------------------------------------------");
			LunyLogger.LogInfo($" ==> {nameof(CountEvents_LunyScript)}.Build() running ...");
			LunyLogger.LogInfo("-------------------------------------------------------------------------");

			On.Created(GVar("Spawned_CreatedCount").Inc());
			On.Enabled(GVar("Spawned_EnabledCount").Inc());
			On.Ready(GVar("Spawned_ReadyCount").Inc());
			On.Heartbeat(GVar("Spawned_HeartbeatCount").Inc());
			On.FrameUpdate(GVar("Spawned_UpdateCount").Inc());
			On.AfterFrameUpdate(GVar("Spawned_LateUpdateCount").Inc(), Object.Destroy());
			On.Disabled(GVar("Spawned_DisabledCount").Inc());
			On.Destroyed(GVar("Spawned_DestroyedCount").Inc());

			Counter("Spawned_CounterCoroutine").In(0).Frames().Do(Debug.LogInfo("COUNTER => Immediate Destroy"), GVar("Spawned_CounterCoroutineCount").Inc());
		}
	}

	public abstract class ScriptActivationTests : ContractTestBase
	{
		[Test]
		public void CountEvents_SpawnTwoObjectsWithSameName_CountsAllEventsTwice()
		{
			var gVars = ScriptEngine.Instance.GlobalVariables;

			var luny = LunyEngine.Instance;
			SimulateFrames(5);
			luny.Object.CreateEmpty(nameof(CountEvents_LunyScript));
			luny.Object.CreateEmpty(nameof(CountEvents_LunyScript));
			SimulateFrames(5);
			luny.Object.CreateEmpty(nameof(CountEvents_LunyScript));
			luny.Object.CreateEmpty(nameof(CountEvents_LunyScript));
			SimulateFrames(5);
			luny.Object.CreateEmpty(nameof(CountEvents_LunyScript));
			luny.Object.CreateEmpty(nameof(CountEvents_LunyScript));
			SimulateFrames(5);

			AssertAllEventCountersEqual(gVars, 6);
		}

		[Test]
		public void CountEvents_SpawnObjectMultipleTimes_CountsAllEvents()
		{
			var gVars = ScriptEngine.Instance.GlobalVariables;

			var luny = LunyEngine.Instance;
			SimulateFrames(5);
			luny.Object.CreateEmpty(nameof(CountEvents_LunyScript));
			SimulateFrames(5);
			luny.Object.CreateEmpty(nameof(CountEvents_LunyScript));
			SimulateFrames(5);
			luny.Object.CreateEmpty(nameof(CountEvents_LunyScript));
			SimulateFrames(5);

			AssertAllEventCountersEqual(gVars, 3);
		}

		[Test]
		public void CountEvents_SpawnObjectLater_CountsAllEvents()
		{
			var gVars = ScriptEngine.Instance.GlobalVariables;

			SimulateFrames(5);
			LunyEngine.Instance.Object.CreateEmpty(nameof(CountEvents_LunyScript));
			SimulateFrames(5);

			AssertAllEventCountersEqual(gVars, 1);
		}

		[Test]
		public void CountEvents_SpawnObjectBeforeSimulate_CountsAllEvents()
		{
			var gVars = ScriptEngine.Instance.GlobalVariables;
			AssertAllEventCountersEqual(gVars, 0);

			LunyEngine.Instance.Object.CreateEmpty(nameof(CountEvents_LunyScript));

			Assert.That(gVars["Spawned_CounterCoroutineCount"].AsInt32(), Is.EqualTo(0));
			Assert.That(gVars["Spawned_CreatedCount"].AsInt32(), Is.EqualTo(1)); // runs upon Create
			Assert.That(gVars["Spawned_EnabledCount"].AsInt32(), Is.EqualTo(1)); // runs upon Create
			Assert.That(gVars["Spawned_ReadyCount"].AsInt32(), Is.EqualTo(0));
			Assert.That(gVars["Spawned_HeartbeatCount"].AsInt32(), Is.EqualTo(0));
			Assert.That(gVars["Spawned_UpdateCount"].AsInt32(), Is.EqualTo(0));
			Assert.That(gVars["Spawned_LateUpdateCount"].AsInt32(), Is.EqualTo(0));
			Assert.That(gVars["Spawned_DisabledCount"].AsInt32(), Is.EqualTo(0));
			Assert.That(gVars["Spawned_DestroyedCount"].AsInt32(), Is.EqualTo(0));

			SimulateFrames(5);

			AssertAllEventCountersEqual(gVars, 1);
		}

		private void AssertAllEventCountersEqual(ITable gVars, Int32 expectedCount)
		{
			Assert.That(gVars["Spawned_CounterCoroutineCount"].AsInt32(), Is.EqualTo(expectedCount), "CounterCoroutine");
			Assert.That(gVars["Spawned_CreatedCount"].AsInt32(), Is.EqualTo(expectedCount), LunyObjectEvent.OnCreated.ToString());
			Assert.That(gVars["Spawned_EnabledCount"].AsInt32(), Is.EqualTo(expectedCount), LunyObjectEvent.OnEnabled.ToString());
			Assert.That(gVars["Spawned_ReadyCount"].AsInt32(), Is.EqualTo(expectedCount), LunyObjectEvent.OnReady.ToString());
			Assert.That(gVars["Spawned_HeartbeatCount"].AsInt32(), Is.EqualTo(expectedCount),  LunyObjectEvent.OnHeartbeat.ToString());
			Assert.That(gVars["Spawned_UpdateCount"].AsInt32(), Is.EqualTo(expectedCount), LunyObjectEvent.OnFrameUpdate.ToString());
			Assert.That(gVars["Spawned_LateUpdateCount"].AsInt32(), Is.EqualTo(expectedCount),  LunyObjectEvent.OnFrameLateUpdate.ToString());
			Assert.That(gVars["Spawned_DisabledCount"].AsInt32(), Is.EqualTo(expectedCount), LunyObjectEvent.OnDisabled.ToString());
			Assert.That(gVars["Spawned_DestroyedCount"].AsInt32(), Is.EqualTo(expectedCount), LunyObjectEvent.OnDestroyed.ToString());
		}
	}

	[TestFixture]
	public sealed class ScriptActivationGodotTests : ScriptActivationTests
	{
		protected override NativeEngine Engine => NativeEngine.Godot;
	}

	[TestFixture]
	public sealed class ScriptActivationUnityTests : ScriptActivationTests
	{
		protected override NativeEngine Engine => NativeEngine.Unity;
	}
}
