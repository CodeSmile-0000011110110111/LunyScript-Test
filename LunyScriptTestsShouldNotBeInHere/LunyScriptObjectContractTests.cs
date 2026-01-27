using Luny.Attributes;
using NUnit.Framework;

namespace Luny.ContractTest.LunyScriptTestsShouldNotBeInHere
{
	public abstract class LunyScriptObjectContractTests : ContractTestBase
	{
		protected void RegisterMockScript(System.Type scriptType)
		{
			LunyEngine.Instance.Object.CreateEmpty(scriptType.Name);
		}

		[Test]
		public void CreateObject_CreatesNativeObjects()
		{
			// Scripts are activated on Scene Load (which happens during Startup in our adapters)
			// We must ensure the engine is fully started before we can expect scripts to run.
			// ContractTestBase.Setup calls InitializeEngine, but doesn't necessarily start it.
			
			// We need to create the object with the same name as the script.
			// The LunyScriptRunner will find it during OnSceneLoaded.
			LunyEngine.Instance.Object.CreateEmpty(nameof(LunyScript_Test_CreateObjects));

			SimulateFrames(3);

			// Verify native objects exist
			AssertNativeObjectExists("empty");
			AssertNativeObjectExists("cube");
			AssertNativeObjectExists("sphere");
			AssertNativeObjectExists("capsule");
			AssertNativeObjectExists("cylinder");
			AssertNativeObjectExists("plane");
			AssertNativeObjectExists("quad");
		}

		protected abstract void AssertNativeObjectExists(string name);
	}

	[LunyTestable]
	public sealed class LunyScript_Test_CreateObjects : LunyScript.LunyScript
	{
		public override void Build()
		{
			When.Self.Ready(Object.Create("empty"));
			When.Self.Ready(Object.CreateCube("cube"));
			When.Self.Ready(Object.CreateSphere("sphere"));
			When.Self.Ready(Object.CreateCapsule("capsule"));
			When.Self.Ready(Object.CreateCylinder("cylinder"));
			When.Self.Ready(Object.CreatePlane("plane"));
			When.Self.Ready(Object.CreateQuad("quad"));
		}
	}
}
