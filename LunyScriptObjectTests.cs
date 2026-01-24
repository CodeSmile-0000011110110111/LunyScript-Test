using Luny;
using Luny.Test;
using NUnit.Framework;

namespace LunyScript.Test
{
	public sealed class LunyScriptObjectTestScript : LunyScript
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

	[TestFixture]
	public sealed class LunyScriptObjectTests : LunyScriptTestBase
	{
		[Test]
		public void CreateObject_CallsServiceMethods()
		{
			var adapter = CreateEngineMockAdapter();
			RegisterMockScript(typeof(LunyScriptObjectTestScript));

			var objectService = LunyEngine.Instance.Object as MockObjectService;
			Assert.That(objectService, Is.Not.Null, "MockObjectService should have been discovered and registered.");

			adapter.Run();

			Assert.That(objectService.Log, Contains.Item("CreateEmpty(empty)"));
			Assert.That(objectService.Log, Contains.Item("CreatePrimitive(Cube,cube)"));
			Assert.That(objectService.Log, Contains.Item("CreatePrimitive(Sphere,sphere)"));
			Assert.That(objectService.Log, Contains.Item("CreatePrimitive(Capsule,capsule)"));
			Assert.That(objectService.Log, Contains.Item("CreatePrimitive(Cylinder,cylinder)"));
			Assert.That(objectService.Log, Contains.Item("CreatePrimitive(Plane,plane)"));
			Assert.That(objectService.Log, Contains.Item("CreatePrimitive(Quad,quad)"));
		}
	}
}
