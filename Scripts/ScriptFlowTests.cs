using Luny;
using Luny.ContractTest;
using Luny.Engine.Bridge.Enums;
using LunyScript.Activation;
using NUnit.Framework;

namespace LunyScript.Test.Scripts
{
	#region Scripts
	public sealed class IfBranchingScript : Script
	{
		public override void Build(ScriptContext context)
		{
			var condition = GVar("Condition");
			var result = GVar("Result");

			On.Ready(
				If(condition == 1)
					.Then(result.Set("Branch 1"))
					.ElseIf(condition == 2)
					.Then(result.Set("Branch 2"))
					.Else(result.Set("Branch Else"))
			);
		}
	}

	public sealed class WhileLoopScript : Script
	{
		public override void Build(ScriptContext context)
		{
			var counter = GVar("Counter");

			On.Ready(
				counter.Set(0),
				While(counter < 5)
					.Do(counter.Inc())
			);
		}
	}

	public sealed class ForLoopScript : Script
	{
		public override void Build(ScriptContext context)
		{
			var sum = GVar("Sum");

			On.Ready(
				sum.Set(0),
				For(3).Do(sum.Add(Loop.Counter))
			);
		}
	}

	public sealed class ForLoopReverseScript : Script
	{
		public override void Build(ScriptContext context)
		{
			var sum = GVar("Sum");

			On.Ready(
				sum.Set("START"),
				For(3, -1).Do(sum.Add(Loop.Counter))
			);
		}
	}

	public sealed class NestedForLoopScript : Script
	{
		public override void Build(ScriptContext context)
		{
			var outer = GVar("Outer");
			var inner = GVar("Inner");

			On.Ready(
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
			var gVars = ScriptEngine.Instance.GlobalVariables;

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
			var gVars = ScriptEngine.Instance.GlobalVariables;

			SimulateFrames(3);

			Assert.That(gVars["Counter"], Is.EqualTo((Variable)5));
		}

		[Test]
		public void For_Loop_Works()
		{
			LunyEngine.Instance.Object.CreateEmpty(nameof(ForLoopScript));
			var gVars = ScriptEngine.Instance.GlobalVariables;

			SimulateFrames(3);

			// 1 + 2 + 3 = 6
			Assert.That(gVars["Sum"], Is.EqualTo((Variable)6));
		}

		[Test]
		public void For_Loop_Reverse_Works()
		{
			LunyEngine.Instance.Object.CreateEmpty(nameof(ForLoopReverseScript));
			var gVars = ScriptEngine.Instance.GlobalVariables;

			SimulateFrames(3);

			Assert.That(gVars["Sum"], Is.EqualTo((Variable)6));
		}

		[Test]
		public void Nested_For_Loop_Works()
		{
			LunyEngine.Instance.Object.CreateEmpty(nameof(NestedForLoopScript));
			var gVars = ScriptEngine.Instance.GlobalVariables;

			SimulateFrames(3);

			Assert.That(gVars["Outer"], Is.EqualTo((Variable)2));
			Assert.That(gVars["Inner"], Is.EqualTo((Variable)6));
		}
	}

	[TestFixture]
	public sealed class ScriptFlowGodotTests : ScriptFlowTests
	{
		protected override NativeEngine Engine => NativeEngine.Godot;
	}

	[TestFixture]
	public sealed class ScriptFlowUnityTests : ScriptFlowTests
	{
		protected override NativeEngine Engine => NativeEngine.Unity;
	}
}
