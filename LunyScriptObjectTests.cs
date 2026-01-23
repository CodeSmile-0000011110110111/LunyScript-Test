using Luny;
using Luny.Test;
using NUnit.Framework;

namespace LunyScript.Test
{
	public sealed class LunyScriptObjectTestScript : LunyScript
	{
		public override void Build()
		{
			When.Object.Ready(Object.CreateEmpty("empty"));
			When.Object.Ready(Object.CreateCube("cube"));
			When.Object.Ready(Object.CreateSphere("sphere"));
		}
	}

	[TestFixture]
	public sealed class LunyScriptObjectTests : LunyTestBase
	{
		[Test]
		public void CreateObject_CallsServiceMethods()
		{
			LunyLogger.EnableInternalLogging = true;
			var adapter = CreateEngineMockAdapter();

			var nativeObject = new MockNativeObject(nameof(LunyScriptObjectTestScript));
			var testObject = new MockLunyObject(nativeObject);
			var sceneService = (MockSceneService)LunyEngine.Instance.Scene;
			sceneService.AddSceneObject(testObject);

			var objectService = LunyEngine.Instance.Object as MockObjectService;
			Assert.That(objectService, Is.Not.Null, "MockObjectService should have been discovered and registered.");

			adapter.Run();

			Assert.That(objectService.Log, Contains.Item("CreateEmpty(empty)"));
			Assert.That(objectService.Log, Contains.Item("CreatePrimitive(Cube,cube)"));
			Assert.That(objectService.Log, Contains.Item("CreatePrimitive(Sphere,sphere)"));
		}
	}
}
