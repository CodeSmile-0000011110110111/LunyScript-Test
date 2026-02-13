using Godot;
using Luny;
using Luny.ContractTest;
using Luny.Engine.Bridge.Enums;
using LunyScript.Api;
using LunyScript.Activation;
using NUnit.Framework;
using System;
using System.Linq;
using UnityEngine;
using UnityObject = UnityEngine.Object;

namespace LunyScript.Test.Objects
{
	public sealed class LunyScript_Test_CreateObjects : Script
	{
		public override void Build(ScriptContext context)
		{
			On.Ready(Object.Create("empty").Do());
			On.Ready(Object.Create("cube").AsCube().Do());
			On.Ready(Object.Create("sphere").AsSphere().Do());
			On.Ready(Object.Create("capsule").AsCapsule().Do());
			On.Ready(Object.Create("cylinder").AsCylinder().Do());
			On.Ready(Object.Create("plane").AsPlane().Do());
			On.Ready(Object.Create("quad").AsQuad().Do());
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
	public sealed class ObjectCreateGodotTests : ObjectCreateTests
	{
		protected override NativeEngine Engine => NativeEngine.Godot;

		protected override void AssertNativeObjectExists(String name)
		{
			var node = GodotObject._allObjects.OfType<Node>().FirstOrDefault(n => n.Name == name);
			Assert.That(node, Is.Not.Null, $"Native Godot Node '{name}' should exist.");
		}
	}

	[TestFixture]
	public sealed class ObjectCreateUnityTests : ObjectCreateTests
	{
		protected override NativeEngine Engine => NativeEngine.Unity;

		protected override void AssertNativeObjectExists(String name)
		{
			var go = UnityObject._allObjects.OfType<GameObject>().FirstOrDefault(g => g.name == name);
			Assert.That(go, Is.Not.Null, $"Native Unity GameObject '{name}' should exist.");
		}
	}
}
