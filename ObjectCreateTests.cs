using Godot;
using Luny;
using Luny.ContractTest;
using Luny.Engine;
using NUnit.Framework;
using System;
using System.Linq;
using UnityEngine;
using UnityObject = UnityEngine.Object;

namespace LunyScript.Test
{
	public sealed class LunyScript_Test_CreateObjects : LunyScript
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
