using Luny;
using Luny.ContractTest;
using Luny.Engine.Bridge.Enums;
using NUnit.Framework;

namespace LunyScript.Test.Scripts
{
	public sealed class DestroyObject_OnCreated_LunyScript : Script
	{
		public override void Build(ScriptContext context) => On.Created(Object.Destroy());
	}

	public sealed class DestroyObject_OnEnabled_LunyScript : Script
	{
		public override void Build(ScriptContext context) => On.Enabled(Object.Destroy());
	}

	public sealed class DestroyObject_OnReady_LunyScript : Script
	{
		public override void Build(ScriptContext context) => On.Ready(Object.Destroy());
	}

	public sealed class DestroyObject_OnHeartbeat_LunyScript : Script
	{
		public override void Build(ScriptContext context) => On.Heartbeat(Object.Destroy());
	}

	public sealed class DestroyObject_OnFrameUpdate_LunyScript : Script
	{
		public override void Build(ScriptContext context) => On.FrameUpdate(Object.Destroy());
	}

	public sealed class DestroyObject_OnAfterFrameUpdate_LunyScript : Script
	{
		public override void Build(ScriptContext context) => On.AfterFrameUpdate(Object.Destroy());
	}

	public sealed class DestroyObject_OnDisabled_LunyScript : Script
	{
		public override void Build(ScriptContext context)
		{
			On.Created(Object.Disable());
			On.Disabled(Object.Destroy());
		}
	}

	public sealed class DestroyObject_OnDestroyed_LunyScript : Script
	{
		public override void Build(ScriptContext context)
		{
			On.Enabled(Object.Destroy());
			On.Destroyed(Object.Destroy());
		}
	}

	public sealed class SelfDisabled_DoesNotRunUpdates_LunyScript : Script
	{
		public override void Build(ScriptContext context)
		{
			On.Ready(Object.Disable());
			On.Heartbeat(GVar("Steps").Set(true));
			On.FrameUpdate(GVar("Updates").Set(true));
			On.AfterFrameUpdate(GVar("LateUpdates").Set(true));
		}
	}

	public sealed class SelfEnabled_RunsUpdates_LunyScript : Script
	{
		public override void Build(ScriptContext context)
		{
			On.Ready(Object.Disable(), Object.Enable());
			On.Heartbeat(GVar("Steps").Set(true));
			//On.Heartbeat(GVar("Steps") = (true));
			On.FrameUpdate(GVar("Updates").Set(true));
			On.AfterFrameUpdate(GVar("LateUpdates").Set(true));
		}
	}

	public abstract class ScriptLifecycleTests : ContractTestBase
	{
		[Test] public void DestroyObject_OnCreated_DoesNotThrow()
		{
			LunyEngine.Instance.Object.CreateEmpty(nameof(DestroyObject_OnCreated_LunyScript));

			Assert.DoesNotThrow(() => SimulateFrames(3));
		}

		[Test] public void DestroyObject_OnEnabled_DoesNotThrow()
		{
			LunyEngine.Instance.Object.CreateEmpty(nameof(DestroyObject_OnEnabled_LunyScript));

			Assert.DoesNotThrow(() => SimulateFrames(3));
		}

		[Test] public void DestroyObject_OnReady_DoesNotThrow()
		{
			LunyEngine.Instance.Object.CreateEmpty(nameof(DestroyObject_OnReady_LunyScript));

			Assert.DoesNotThrow(() => SimulateFrames(3));
		}

		[Test] public void DestroyObject_OnHeartbeat_DoesNotThrow()
		{
			LunyEngine.Instance.Object.CreateEmpty(nameof(DestroyObject_OnHeartbeat_LunyScript));

			Assert.DoesNotThrow(() => SimulateFrames(3));
		}

		[Test] public void DestroyObject_OnFrameUpdate_DoesNotThrow()
		{
			LunyEngine.Instance.Object.CreateEmpty(nameof(DestroyObject_OnFrameUpdate_LunyScript));

			Assert.DoesNotThrow(() => SimulateFrames(3));
		}

		[Test] public void DestroyObject_OnAfterFrameUpdate_DoesNotThrow()
		{
			LunyEngine.Instance.Object.CreateEmpty(nameof(DestroyObject_OnAfterFrameUpdate_LunyScript));

			Assert.DoesNotThrow(() => SimulateFrames(3));
		}

		[Test] public void DestroyObject_OnDisabled_DoesNotThrow()
		{
			LunyEngine.Instance.Object.CreateEmpty(nameof(DestroyObject_OnDisabled_LunyScript));

			Assert.DoesNotThrow(() => SimulateFrames(3));
		}

		[Test] public void DestroyObject_OnDestroyed_DoesNotThrow()
		{
			LunyEngine.Instance.Object.CreateEmpty(nameof(DestroyObject_OnDestroyed_LunyScript));

			Assert.DoesNotThrow(() => SimulateFrames(3));
		}

		[Test]
		public void SelfDisabled_DoesNotRunUpdates()
		{
			LunyEngine.Instance.Object.CreateEmpty(nameof(SelfDisabled_DoesNotRunUpdates_LunyScript));
			var gVars = ScriptEngine.Instance.GlobalVariables;

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
			var gVars = ScriptEngine.Instance.GlobalVariables;

			SimulateFrames(3);

			// not using Is.True: NUnit calls false.Equals({Variable}) which will always be false
			Assert.That(gVars["Steps"], Is.EqualTo((Variable)true));
			Assert.That(gVars["Updates"], Is.EqualTo((Variable)true));
			Assert.That(gVars["LateUpdates"], Is.EqualTo((Variable)true));
		}
	}

	[TestFixture]
	public sealed class ScriptLifecycleGodotTests : ScriptLifecycleTests
	{
		protected override NativeEngine Engine => NativeEngine.Godot;
	}

	[TestFixture]
	public sealed class ScriptLifecycleUnityTests : ScriptLifecycleTests
	{
		protected override NativeEngine Engine => NativeEngine.Unity;
	}
}
