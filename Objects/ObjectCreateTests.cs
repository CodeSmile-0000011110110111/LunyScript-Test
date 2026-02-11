using Godot;
using Luny;
using Luny.ContractTest;
using Luny.Engine.Bridge.Enums;
using LunyScript.Activation;
using NUnit.Framework;
using System;
using System.Linq;
using UnityEngine;
using UnityObject = UnityEngine.Object;

namespace LunyScript.Test.Objects
{
	public sealed class LunyScript_Test_CreateObjects : LunyScript
	{
		public override void Build(ScriptBuildContext context)
		{
			On.Ready(Object.Create("empty"));
			On.Ready(Object.CreateCube("cube"));
			On.Ready(Object.CreateSphere("sphere"));
			On.Ready(Object.CreateCapsule("capsule"));
			On.Ready(Object.CreateCylinder("cylinder"));
			On.Ready(Object.CreatePlane("plane"));
			On.Ready(Object.CreateQuad("quad"));
		}
	}

	public abstract class ObjectCreateTests : ContractTestBase
	{
		[Test]
		public void CreateObject_CreatesNativeObjects()
		{
			// We need to create the object with the same name as the script.
			// The LunyScriptRunner will find it during OnSceneLoaded.
			LunyEngine.Instance.Object.CreateEmpty(nameof(LunyScript_Test_CreateObjects));

			SimulateFrame();

			// Verify native objects exist
			AssertNativeObjectExists("empty");
			AssertNativeObjectExists("cube");
			AssertNativeObjectExists("sphere");
			AssertNativeObjectExists("capsule");
			AssertNativeObjectExists("cylinder");
			AssertNativeObjectExists("plane");
			AssertNativeObjectExists("quad");
		}

		protected abstract void AssertNativeObjectExists(String name);
	}

	[TestFixture]
	public sealed class GodotObjectCreateTests : ObjectCreateTests
	{
		protected override NativeEngine Engine => NativeEngine.Godot;

		protected override void AssertNativeObjectExists(String name)
		{
			var node = GodotObject._allObjects.OfType<Node>().FirstOrDefault(n => n.Name == name);
			Assert.That(node, Is.Not.Null, $"Native Godot Node '{name}' should exist.");
		}
	}

	[TestFixture]
	public sealed class UnityObjectCreateTests : ObjectCreateTests
	{
		protected override NativeEngine Engine => NativeEngine.Unity;

		protected override void AssertNativeObjectExists(String name)
		{
			var go = UnityObject._allObjects.OfType<GameObject>().FirstOrDefault(g => g.name == name);
			Assert.That(go, Is.Not.Null, $"Native Unity GameObject '{name}' should exist.");
		}
	}
}
