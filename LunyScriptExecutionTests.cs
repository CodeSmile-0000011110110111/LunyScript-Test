using Luny;
using Luny.Test;
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
	public sealed class LunyScriptExecutionTests : LunyTestBase
	{
		[Test]
		public void LunyScript_RunsBuildMethod()
		{
			var adapter = CreateEngineMockAdapter();

			var nativeObject = new MockNativeObject(nameof(LunyScriptExecutionTestScript));
			var mockObject = new MockLunyObject(nativeObject);
			var sceneService = (MockSceneService)LunyEngine.Instance.Scene;
			sceneService.AddSceneObject(mockObject);

			Assert.That(LunyScriptExecutionTestScript.DidRunBuild, Is.False);
			adapter.Run();
			Assert.That(LunyScriptExecutionTestScript.DidRunBuild, Is.True);
		}
	}
}
