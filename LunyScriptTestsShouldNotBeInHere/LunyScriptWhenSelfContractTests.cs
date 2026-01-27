using LunyScript;
using NUnit.Framework;
using System;

namespace Luny.ContractTest.LunyScriptTestsShouldNotBeInHere
{
	public sealed class LunyScript_Test_WhenDisabledDoesNotRunSelfUpdates : LunyScript.LunyScript
	{
		public override void Build()
		{
			When.Self.Ready(Object.Disable());
			When.Self.Steps(Method.Run(() => GlobalVars[nameof(When.Self.Steps)] = true));
			When.Self.Updates(Method.Run(() => GlobalVars[nameof(When.Self.Updates)] = true));
			When.Self.LateUpdates(Method.Run(() => GlobalVars[nameof(When.Self.LateUpdates)] = true));
		}
	}

	public sealed class LunyScript_Test_WhenEnabledRunsSelfUpdates : LunyScript.LunyScript
	{
		public override void Build()
		{
			When.Self.Ready(Object.Disable(), Object.Enable());
			When.Self.Steps(Method.Run(() => GlobalVars[nameof(When.Self.Steps)] = true));
			When.Self.Updates(Method.Run(() => GlobalVars[nameof(When.Self.Updates)] = true));
			When.Self.LateUpdates(Method.Run(() => GlobalVars[nameof(When.Self.LateUpdates)] = true));
		}
	}

	public abstract class LunyScriptWhenSelfContractTests : ContractTestBase
	{
		protected void RegisterMockScript(Type scriptType) => LunyEngine.Instance.Object.CreateEmpty(scriptType.Name);

		[Test]
		public void SelfDisabled_DoesNotRunUpdates()
		{
			LunyEngine.Instance.Object.CreateEmpty(nameof(LunyScript_Test_WhenDisabledDoesNotRunSelfUpdates));
			var gVars = LunyScriptEngine.Instance.GlobalVars;

			SimulateFrames(3);

			// not using Is.False: NUnit calls false.Equals({Variable}) which will always be false
			Assert.That(gVars[nameof(LunyScript.LunyScript.When.Self.Steps)], Is.EqualTo(false));
			Assert.That(gVars[nameof(LunyScript.LunyScript.When.Self.Updates)], Is.EqualTo(false));
			Assert.That(gVars[nameof(LunyScript.LunyScript.When.Self.LateUpdates)], Is.EqualTo(false));
		}

		[Test]
		public void SelfEnabled_RunsUpdates()
		{
			LunyEngine.Instance.Object.CreateEmpty(nameof(LunyScript_Test_WhenEnabledRunsSelfUpdates));
			var gVars = LunyScriptEngine.Instance.GlobalVars;

			SimulateFrames(3);

			// not using Is.True: NUnit calls false.Equals({Variable}) which will always be false
			Assert.That(gVars[nameof(LunyScript.LunyScript.When.Self.Steps)], Is.EqualTo(true));
			Assert.That(gVars[nameof(LunyScript.LunyScript.When.Self.Updates)], Is.EqualTo(true));
			Assert.That(gVars[nameof(LunyScript.LunyScript.When.Self.LateUpdates)], Is.EqualTo(true));
		}
	}
}
