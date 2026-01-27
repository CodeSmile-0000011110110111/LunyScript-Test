using Luny;
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

	public sealed class LunyScriptObjectTests
	{
		// This test class is now redundant as logic moved to ContractTests.
		// Keeping it empty or removing it after Tier 4.
	}
}
