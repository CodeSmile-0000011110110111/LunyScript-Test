using Luny;
using Luny.ContractTest;
using System;

namespace LunyScript.Test
{
	public abstract class LunyScriptTestBase : ContractTestBase
	{
		protected void RegisterMockScript(Type scriptType)
		{
			// The LunyScriptRunner will automatically discover all classes inheriting from LunyScript
			// We just need to create an object with the same name as the script class
			// to trigger activation in LunyScriptRunner.OnSceneLoaded.
			
			var engine = LunyEngine.Instance;
			engine.Object.CreateEmpty(scriptType.Name);
			
			// If the engine is already started, we might need to manually trigger scene load 
			// if we want immediate activation, or just let the next tick handle it if 
			// the adapter triggers it.
		}
	}
}
