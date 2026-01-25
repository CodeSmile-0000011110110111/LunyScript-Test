using NUnit.Framework;
using System;

namespace LunyScript.Test
{
	public sealed class LunyScriptBuilderTestScript : LunyScript
	{
		public static Boolean DidRunBuild { get; private set; }
		public override void Build() => DidRunBuild = true;
	}

	[TestFixture]
	public sealed class LunyScriptBuilderTests : LunyScriptTestBase
	{
		[Test]
		public void LunyScript_RunsBuildMethod()
		{
			var adapter = CreateEngineMockAdapter();
			RegisterMockScript(typeof(LunyScriptBuilderTestScript));

			Assert.That(LunyScriptBuilderTestScript.DidRunBuild, Is.False);
			adapter.RunAllFrames();
			Assert.That(LunyScriptBuilderTestScript.DidRunBuild, Is.True);
		}
	}
}
