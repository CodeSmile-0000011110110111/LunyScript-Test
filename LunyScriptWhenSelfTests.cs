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

	public sealed class LunyScriptWhenSelfTests
	{
		// This test class is now redundant as logic moved to ContractTests.
	}
}
