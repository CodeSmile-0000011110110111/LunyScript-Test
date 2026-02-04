using Luny;
using Luny.ContractTest;
using Luny.Engine.Bridge.Enums;
using LunyScript.Execution;
using NUnit.Framework;

namespace LunyScript.Test
{
	public sealed class VariableTestScript : LunyScript
	{
		public override void Build()
		{
			var hp = Var("hp");
			var score = GVar("score");
			var isDead = Var("isDead");
			var count = Var("count");
			var falseFlag = Var("falseFlag");

			var testOp = Var("testOp");
			var testOp2 = Var("testOp2");

			When.Self.Ready(
				// Test Set Literals
				hp.Set(100),
				score.Set(0),
				isDead.Set(false),

				// Test Arithmetic Literals
				hp.Sub(20), // 80
				score.Add(10), // 10

				// Test Arithmetic with ScriptVars (Operators)
				hp.Set(hp - 10), // 70
				score.Set(score + 5), // 15

				// Test Toggle
				isDead.Toggle(), // true

				// Test Conditions
				If(hp.IsGreaterThan(50)).Then(Var("status").Set("healthy")),
				If(isDead.IsTrue()).Then(GVar("lastEvent").Set("died")),

				// Test Operators & Condition implementation
				If(hp > 50).Then(Var("status_op").Set("healthy")),
				If(isDead).Then(GVar("lastEvent_op").Set("died")),
				If(score >= 15).Then(Var("score_ok").Set(true)),

				// Test Multiplex arithmetic
				hp.Set(hp * 2), // 140
				hp.Set(hp / 2), // 70
				If(count + 10 - 10 < 10).Then(count.Set(count - 10 + 11)),
				hp.Set((hp + 10) / 2),
				hp.Set((hp + 10) * 2 - 5), // (40 + 10) * 2 - 5 = 95
				If(!falseFlag).Then(falseFlag.Set(hp)),

				// Test Increment/Decrement operators
				testOp.Set(10),
				testOp.Set(testOp++), // remains 10 (postfix => testOp incremented after Set())
				If(testOp == 11).Then(testOp2.Set(++testOp)) // becomes 12 (prefix => increment before Set())
			);
		}
	}

	[TestFixture]
	public sealed class ScriptVariableTests : ContractTestBase
	{
		protected override NativeEngine Engine => NativeEngine.Unity;

		[SetUp]
		public void Setup() => LunyScriptEngine.Instance?.GlobalVars.RemoveAll();

		[Test]
		public void Variable_API_Works()
		{
			var obj = LunyEngine.Instance.Object.CreateEmpty(nameof(VariableTestScript));
			var context = (LunyScriptContext)LunyScriptEngine.Instance.GetScriptContext(obj.NativeObjectID);
			var Vars = context.LocalVariables;
			var GVars = LunyScriptEngine.Instance.GlobalVars;

			SimulateFrames(1);

			LunyLogger.LogInfo($"Final testOp: {Vars["testOp"]} ({Vars["testOp"].Value})");
			Assert.That(Vars["hp"], Is.EqualTo((Variable)95));
			Assert.That(GVars["score"], Is.EqualTo((Variable)15));
			Assert.That(Vars["isDead"], Is.EqualTo((Variable)true));
			Assert.That(Vars["status"], Is.EqualTo((Variable)"healthy"));
			Assert.That(GVars["lastEvent"], Is.EqualTo((Variable)"died"));
			LunyLogger.LogInfo($"---------> {Vars["falseFlag"]} ({Vars["falseFlag"].Value})");
			Assert.That(Vars["falseFlag"], Is.EqualTo((Variable)95));

			// Op variants
			Assert.That(Vars["status_op"], Is.EqualTo((Variable)"healthy"));
			Assert.That(GVars["lastEvent_op"], Is.EqualTo((Variable)"died"));
			Assert.That(Vars["score_ok"], Is.EqualTo((Variable)true));

			// Increment/Decrement
			Assert.That(Vars["testOp"], Is.EqualTo((Variable)10));
			Assert.That(Vars["testOp2"], Is.EqualTo((Variable)12));
		}

		[Test]
		public void Variable_Handle_Is_Persistent()
		{
			// Verify that ScriptVar holds a persistent reference even if dictionary entry is removed/re-added
			var table = new Table();
			var handle1 = table.GetHandle("test");
			handle1.Value = 1;

			table.ResetValue("test");
			Assert.That(handle1.Value.Type, Is.EqualTo(Variable.ValueType.Null));

			var handle2 = table.GetHandle("test");
			Assert.That(handle2, Is.SameAs(handle1));
		}
	}
}
