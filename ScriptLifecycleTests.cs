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
			When.Self.Ready(Object.Disable());
			When.Self.Steps(Method.Run(() => GlobalVars[nameof(When.Self.Steps)] = true));
			When.Self.Updates(Method.Run(() => GlobalVars[nameof(When.Self.Updates)] = true));
			When.Self.LateUpdates(Method.Run(() => GlobalVars[nameof(When.Self.LateUpdates)] = true));
		}
	}

	public sealed class SelfEnabled_RunsUpdates_LunyScript : LunyScript
	{
		public override void Build()
		{
			When.Self.Ready(Object.Disable(), Object.Enable());
			When.Self.Steps(Method.Run(() => GlobalVars[nameof(When.Self.Steps)] = true));
			When.Self.Updates(Method.Run(() => GlobalVars[nameof(When.Self.Updates)] = true));
			When.Self.LateUpdates(Method.Run(() => GlobalVars[nameof(When.Self.LateUpdates)] = true));
		}
	}

	public abstract class ScriptLifecycleTests : ContractTestBase
	{
		[Test]
		public void SelfDisabled_DoesNotRunUpdates()
		{
			LunyEngine.Instance.Object.CreateEmpty(nameof(SelfDisabled_DoesNotRunUpdates_LunyScript));
			var gVars = LunyScriptEngine.Instance.GlobalVars;

			SimulateFrames(3);

			// not using Is.False: NUnit calls false.Equals({Variable}) which will always be false
			Assert.That(gVars[nameof(LunyScript.When.Self.Steps)].AsBoolean(), Is.EqualTo(false));
			Assert.That(gVars[nameof(LunyScript.When.Self.Updates)].AsBoolean(), Is.EqualTo(false));
			Assert.That(gVars[nameof(LunyScript.When.Self.LateUpdates)].AsBoolean(), Is.EqualTo(false));
		}

		[Test]
		public void SelfEnabled_RunsUpdates()
		{
			LunyEngine.Instance.Object.CreateEmpty(nameof(SelfEnabled_RunsUpdates_LunyScript));
			var gVars = LunyScriptEngine.Instance.GlobalVars;

			SimulateFrames(3);

			// not using Is.True: NUnit calls false.Equals({Variable}) which will always be false
			Assert.That(gVars[nameof(LunyScript.When.Self.Steps)], Is.EqualTo((Variable)true));
			Assert.That(gVars[nameof(LunyScript.When.Self.Updates)], Is.EqualTo((Variable)true));
			Assert.That(gVars[nameof(LunyScript.When.Self.LateUpdates)], Is.EqualTo((Variable)true));
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
