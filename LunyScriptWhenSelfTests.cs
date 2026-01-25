using NUnit.Framework;

namespace LunyScript.Test
{
	/*
	public sealed class WhenDisabledRunsEveryUpdates : LunyScript
	{
		public override void Build()
		{
			When.Self.Ready(Object.Disable());
			Every.Step(Method.Run(() => GlobalVariables[nameof(When.Self.Steps)] = true));
			Every.Update(Method.Run(() => GlobalVariables[nameof(When.Self.Updates)] = true));
			Every.LateUpdate(Method.Run(() => GlobalVariables[nameof(When.Self.LateUpdates)] = true));
		}
	}
	*/
	public sealed class WhenDisabledDoesNotRunSelfUpdates : LunyScript
	{
		public override void Build()
		{
			When.Self.Ready(Object.Disable());
			When.Self.Steps(Method.Run(() => GlobalVars[nameof(When.Self.Steps)] = true));
			When.Self.Updates(Method.Run(() => GlobalVars[nameof(When.Self.Updates)] = true));
			When.Self.LateUpdates(Method.Run(() => GlobalVars[nameof(When.Self.LateUpdates)] = true));
		}
	}

	public sealed class WhenEnabledRunsSelfUpdates : LunyScript
	{
		public override void Build()
		{
			When.Self.Ready(Object.Disable(), Object.Enable());
			When.Self.Steps(Method.Run(() => GlobalVars[nameof(When.Self.Steps)] = true));
			When.Self.Updates(Method.Run(() => GlobalVars[nameof(When.Self.Updates)] = true));
			When.Self.LateUpdates(Method.Run(() => GlobalVars[nameof(When.Self.LateUpdates)] = true));
		}
	}

	[TestFixture]
	public sealed class LunyScriptWhenSelfTests : LunyScriptTestBase
	{
		[Test]
		public void SelfDisabled_DoesNotRunUpdates()
		{
			var adapter = CreateEngineMockAdapter();
			var self = RegisterMockScript(typeof(WhenDisabledDoesNotRunSelfUpdates));
			var gVars = LunyScriptEngine.Instance.GlobalVars;

			adapter.RunAllFrames();

			Assert.That(gVars[nameof(LunyScript.When.Self.Steps)] == false);
			Assert.That(gVars[nameof(LunyScript.When.Self.Updates)] == false);
			Assert.That(gVars[nameof(LunyScript.When.Self.LateUpdates)] == false);
		}

		[Test]
		public void SelfEnabled_RunsUpdates()
		{
			var adapter = CreateEngineMockAdapter();
			var self = RegisterMockScript(typeof(WhenEnabledRunsSelfUpdates));
			var gVars = LunyScriptEngine.Instance.GlobalVars;

			adapter.RunAllFrames();

			Assert.That(gVars[nameof(LunyScript.When.Self.Steps)] == true);
			Assert.That(gVars[nameof(LunyScript.When.Self.Updates)] == true);
			Assert.That(gVars[nameof(LunyScript.When.Self.LateUpdates)] == true);
		}
	}
}
