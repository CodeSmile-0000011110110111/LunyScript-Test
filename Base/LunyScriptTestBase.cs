using Luny;
using Luny.Test;
using System;

namespace LunyScript.Test
{
	public abstract class LunyScriptTestBase : LunyTestBase
	{
		protected static void RegisterMockScript(Type scriptType)
		{
			var nativeObject = new MockNativeObject(scriptType.Name);
			var sceneService = (MockSceneService)LunyEngine.Instance.Scene;
			sceneService.AddNativeObject(nativeObject);
		}
	}
}
