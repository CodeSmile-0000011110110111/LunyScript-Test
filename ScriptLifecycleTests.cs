using Luny;
using Luny.ContractTest;
using Luny.Engine.Bridge.Enums;
using NUnit.Framework;

namespace LunyScript.Test
{
	public sealed class SelfDisabled_DoesNotRunUpdates_LunyScript : LunyScript
	{
		public override void Build()
		{
			On.Ready(Object.Disable());
			On.Heartbeat(GVar("Steps").Set(true));
			On.FrameUpdate(GVar("Updates").Set(true));
			On.FrameEnd(GVar("LateUpdates").Set(true));
		}
	}

	public sealed class SelfEnabled_RunsUpdates_LunyScript : LunyScript
	{
		public override void Build()
		{
			On.Ready(Object.Disable(), Object.Enable());
			On.Heartbeat(GVar("Steps").Set(true));
			On.FrameUpdate(GVar("Updates").Set(true));
			On.FrameEnd(GVar("LateUpdates").Set(true));
		}
	}

	public abstract class ScriptLifecycleTests : ContractTestBase
	{
		[Test]
		public void SelfDisabled_DoesNotRunUpdates()
		{
			LunyEngine.Instance.Object.CreateEmpty(nameof(SelfDisabled_DoesNotRunUpdates_LunyScript));
			var gVars = LunyScriptEngine.Instance.GlobalVariables;

			SimulateFrames(3);

			// not using Is.False: NUnit calls false.Equals({Variable}) which will always be false
			Assert.That(gVars["Steps"].AsBoolean(), Is.EqualTo(false));
			Assert.That(gVars["Updates"].AsBoolean(), Is.EqualTo(false));
			Assert.That(gVars["LateUpdates"].AsBoolean(), Is.EqualTo(false));
		}

		[Test]
		public void SelfEnabled_RunsUpdates()
		{
			LunyEngine.Instance.Object.CreateEmpty(nameof(SelfEnabled_RunsUpdates_LunyScript));
			var gVars = LunyScriptEngine.Instance.GlobalVariables;

			SimulateFrames(3);

			// not using Is.True: NUnit calls false.Equals({Variable}) which will always be false
			Assert.That(gVars["Steps"], Is.EqualTo((Variable)true));
			Assert.That(gVars["Updates"], Is.EqualTo((Variable)true));
			Assert.That(gVars["LateUpdates"], Is.EqualTo((Variable)true));
		}
	}

	[TestFixture]
	public sealed class GodotScriptLifecycleTests : ScriptLifecycleTests
	{
		protected override NativeEngine Engine => NativeEngine.Godot;
	}

	[TestFixture]
	public sealed class UnityScriptLifecycleTests : ScriptLifecycleTests
	{
		protected override NativeEngine Engine => NativeEngine.Unity;
	}
}
