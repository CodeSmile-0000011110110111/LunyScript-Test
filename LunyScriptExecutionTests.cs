using NUnit.Framework;
using System;

namespace LunyScript.Test
{
	public sealed class LunyScriptExecutionTestScript : LunyScript
	{
		public static Boolean DidRunBuild { get; private set; }
		public override void Build() => DidRunBuild = true;
	}

	[TestFixture]
	public sealed class LunyScriptExecutionTests : LunyScriptTestBase
	{
		[Test]
		public void LunyScript_RunsBuildMethod()
		{
			var adapter = CreateEngineMockAdapter();
			RegisterMockScript(typeof(LunyScriptExecutionTestScript));

			Assert.That(LunyScriptExecutionTestScript.DidRunBuild, Is.False);
			adapter.Run();
			Assert.That(LunyScriptExecutionTestScript.DidRunBuild, Is.True);
		}
	}
}
