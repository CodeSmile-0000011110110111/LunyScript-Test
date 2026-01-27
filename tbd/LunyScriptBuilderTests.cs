using NUnit.Framework;
using System;

namespace LunyScript.Test
{
	public sealed class LunyScriptBuilderTestScript : LunyScript
	{
		public static Boolean DidRunBuild { get; private set; }
		public override void Build() => DidRunBuild = true;
	}

	public sealed class LunyScriptBuilderTests
	{
		// This test class is now redundant as logic moved to ContractTests.
	}
}
