using Luny;
using Luny.ContractTest;
using Luny.Engine.Bridge.Enums;
using NUnit.Framework;

namespace LunyScript.Test
{
	#region Scripts
	public sealed class IfBranchingScript : LunyScript
	{
		public override void Build() => When.Self.Ready(
			If(Method.IsTrue(() => GlobalVars["Condition"] == 1))
				.Then(Method.Run(() => GlobalVars["Result"] = "Branch 1"))
				.ElseIf(Method.IsTrue(() => GlobalVars["Condition"] == 2))
				.Then(Method.Run(() => GlobalVars["Result"] = "Branch 2"))
				.Else(Method.Run(() => GlobalVars["Result"] = "Branch Else"))
		);
	}

	public sealed class WhileLoopScript : LunyScript
	{
		public override void Build() => When.Self.Ready(
			Method.Run(() => GlobalVars["Counter"] = 0),
			While(Method.IsTrue(() => GlobalVars["Counter"] < 5))
				.Do(Method.Run(() => GlobalVars["Counter"] = GlobalVars["Counter"] + 1))
		);
	}

	public sealed class ForLoopScript : LunyScript
	{
		public override void Build() => When.Self.Ready(
			Method.Run(() => GlobalVars["Sum"] = 0),
			For(3).Do(Method.Run(ctx => GlobalVars["Sum"] = GlobalVars["Sum"] + ctx.LoopCount))
		);
	}

	public sealed class ForLoopReverseScript : LunyScript
	{
		public override void Build() => When.Self.Ready(
			Method.Run(() => GlobalVars["Sum"] = "START"),
			For(3, -1).Do(Method.Run(ctx => GlobalVars["Sum"] = GlobalVars["Sum"] + ctx.LoopCount))
		);
	}

	public sealed class NestedForLoopScript : LunyScript
	{
		public override void Build() => When.Self.Ready(
			Method.Run(() => GlobalVars["Outer"] = 0),
			Method.Run(() => GlobalVars["Inner"] = 0),
			For(2)
				.Do(Method.Run(() => GlobalVars["Outer"] = GlobalVars["Outer"] + 1),
					For(3)
						.Do(Method.Run(() => GlobalVars["Inner"] = GlobalVars["Inner"] + 1))));
	}
	#endregion

	public abstract class ScriptFlowTests : ContractTestBase
	{
		[SetUp]
		public void SetupFlowTests() => LunyScriptEngine.Instance?.GlobalVars.RemoveAll();

		[Test]
		public void If_Branching_Works()
		{
			var gVars = LunyScriptEngine.Instance.GlobalVars;

			// Branch 1
			gVars.RemoveAll();
			gVars["Condition"] = 1;
			LunyEngine.Instance.Object.CreateEmpty(nameof(IfBranchingScript));
			SimulateFrames(3);
			Assert.That(gVars["Result"], Is.EqualTo((Variable)"Branch 1"));

			// Branch 2
			gVars.RemoveAll();
			gVars["Condition"] = 2;
			LunyEngine.Instance.Object.CreateEmpty(nameof(IfBranchingScript));
			SimulateFrames(3);
			Assert.That(gVars["Result"], Is.EqualTo((Variable)"Branch 2"));

			// Branch Else
			gVars.RemoveAll();
			gVars["Condition"] = 3;
			LunyEngine.Instance.Object.CreateEmpty(nameof(IfBranchingScript));
			SimulateFrames(3);
			Assert.That(gVars["Result"], Is.EqualTo((Variable)"Branch Else"));
		}

		[Test]
		public void While_Loop_Works()
		{
			LunyEngine.Instance.Object.CreateEmpty(nameof(WhileLoopScript));
			var gVars = LunyScriptEngine.Instance.GlobalVars;

			SimulateFrames(3);

			Assert.That(gVars["Counter"], Is.EqualTo((Variable)5));
		}

		[Test]
		public void For_Loop_Works()
		{
			LunyEngine.Instance.Object.CreateEmpty(nameof(ForLoopScript));
			var gVars = LunyScriptEngine.Instance.GlobalVars;

			SimulateFrames(3);

			// 1 + 2 + 3 = 6
			Assert.That(gVars["Sum"], Is.EqualTo((Variable)6));
		}

		[Test]
		public void For_Loop_Reverse_Works()
		{
			LunyEngine.Instance.Object.CreateEmpty(nameof(ForLoopReverseScript));
			var gVars = LunyScriptEngine.Instance.GlobalVars;

			SimulateFrames(3);

			Assert.That(gVars["Sum"], Is.EqualTo((Variable)6));
		}

		[Test]
		public void Nested_For_Loop_Works()
		{
			LunyEngine.Instance.Object.CreateEmpty(nameof(NestedForLoopScript));
			var gVars = LunyScriptEngine.Instance.GlobalVars;

			SimulateFrames(3);

			Assert.That(gVars["Outer"], Is.EqualTo((Variable)2));
			Assert.That(gVars["Inner"], Is.EqualTo((Variable)6));
		}
	}

	[TestFixture]
	public sealed class GodotScriptFlowTests : ScriptFlowTests
	{
		protected override NativeEngine Engine => NativeEngine.Godot;
	}

	[TestFixture]
	public sealed class UnityScriptFlowTests : ScriptFlowTests
	{
		protected override NativeEngine Engine => NativeEngine.Unity;
	}
}
