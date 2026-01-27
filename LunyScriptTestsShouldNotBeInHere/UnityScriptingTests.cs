using Luny.ContractTest.LunyScriptTestsShouldNotBeInHere;
using NUnit.Framework;
using System;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Luny.ContractTest
{
	[TestFixture]
	public class UnityScriptingTests : LunyScriptObjectContractTests
	{
		protected override NativeEngine Engine => NativeEngine.Unity;

		protected override void AssertNativeObjectExists(String name)
		{
			var go = Object._allObjects.OfType<GameObject>().FirstOrDefault(g => g.name == name);
			Assert.That(go, Is.Not.Null, $"Native Unity GameObject '{name}' should exist.");
		}
	}

	[TestFixture]
	public class UnityScriptWhenSelfTests : LunyScriptWhenSelfContractTests
	{
		protected override NativeEngine Engine => NativeEngine.Unity;
	}
}
