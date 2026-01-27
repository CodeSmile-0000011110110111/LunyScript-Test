using Godot;
using Luny.ContractTest.LunyScriptTestsShouldNotBeInHere;
using NUnit.Framework;
using System;
using System.Linq;

namespace Luny.ContractTest
{
	[TestFixture]
	public class GodotScriptingTests : LunyScriptObjectContractTests
	{
		protected override NativeEngine Engine => NativeEngine.Godot;

		protected override void AssertNativeObjectExists(String name)
		{
			var node = GodotObject._allObjects.OfType<Node>().FirstOrDefault(n => n.Name == name);
			Assert.That(node, Is.Not.Null, $"Native Godot Node '{name}' should exist.");
		}
	}

	[TestFixture]
	public class GodotScriptWhenSelfTests : LunyScriptWhenSelfContractTests
	{
		protected override NativeEngine Engine => NativeEngine.Godot;
	}
}
