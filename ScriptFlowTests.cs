using Luny;
using Luny.ContractTest;
using Luny.Engine.Bridge.Enums;
using NUnit.Framework;

namespace LunyScript.Test
{
	#region Scripts
	public sealed class IfBranchingScript : LunyScript
	{
		public override void Build()
		{
			var condition = GVar("Condition");
			var result = GVar("Result");

			When.Self.Ready(
				If(condition == 1)
					.Then(result.Set("Branch 1"))
					.ElseIf(condition == 2)
					.Then(result.Set("Branch 2"))
					.Else(result.Set("Branch Else"))
			);
		}
	}

	public sealed class WhileLoopScript : LunyScript
	{
		public override void Build()
		{
			var counter = GVar("Counter");

			When.Self.Ready(
				counter.Set(0),
				While(counter < 5)
					.Do(counter.Inc())
			);
		}
	}

	public sealed class ForLoopScript : LunyScript
	{
		public override void Build()
		{
			var sum = GVar("Sum");

			When.Self.Ready(
				sum.Set(0),
				For(3).Do(sum.Add(Loop.Counter))
			);
		}
	}

	public sealed class ForLoopReverseScript : LunyScript
	{
		public override void Build()
		{
			var sum = GVar("Sum");

			When.Self.Ready(
				sum.Set("START"),
				For(3, -1).Do(sum.Add(Loop.Counter))
			);
		}
	}

	public sealed class NestedForLoopScript : LunyScript
	{
		public override void Build()
		{
			var outer = GVar("Outer");
			var inner = GVar("Inner");

			When.Self.Ready(
				outer.Set(0),
				inner.Set(0),
				For(2)
					.Do(
						outer.Inc(),
						For(3).Do(inner.Inc())
					)
			);
		}
	}
	#endregion

	public abstract class ScriptFlowTests : ContractTestBase
	{
		[Test]
		public void If_Branching_Works()
		{
			var gVars = LunyScriptEngine.Instance.GlobalVariables;

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
			var gVars = LunyScriptEngine.Instance.GlobalVariables;

			SimulateFrames(3);

			Assert.That(gVars["Counter"], Is.EqualTo((Variable)5));
		}

		[Test]
		public void For_Loop_Works()
		{
			LunyEngine.Instance.Object.CreateEmpty(nameof(ForLoopScript));
			var gVars = LunyScriptEngine.Instance.GlobalVariables;

			SimulateFrames(3);

			// 1 + 2 + 3 = 6
			Assert.That(gVars["Sum"], Is.EqualTo((Variable)6));
		}

		[Test]
		public void For_Loop_Reverse_Works()
		{
			LunyEngine.Instance.Object.CreateEmpty(nameof(ForLoopReverseScript));
			var gVars = LunyScriptEngine.Instance.GlobalVariables;

			SimulateFrames(3);

			Assert.That(gVars["Sum"], Is.EqualTo((Variable)6));
		}

		[Test]
		public void Nested_For_Loop_Works()
		{
			LunyEngine.Instance.Object.CreateEmpty(nameof(NestedForLoopScript));
			var gVars = LunyScriptEngine.Instance.GlobalVariables;

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
