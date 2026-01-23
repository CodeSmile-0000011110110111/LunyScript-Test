using Luny;
using Luny.Test;
using System;

namespace LunyScript.Test
{
	public abstract class LunyScriptTestBase : LunyTestBase
	{
		protected static MockLunyObject RegisterMockScript(Type scriptType)
		{
			var nativeObject = new MockNativeObject(scriptType.Name);
			var mockObject = new MockLunyObject(nativeObject);

			var sceneService = (MockSceneService)LunyEngine.Instance.Scene;
			sceneService.AddSceneObject(mockObject);

			return mockObject;
		}
	}
}
