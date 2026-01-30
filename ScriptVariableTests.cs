using Luny;
using Luny.ContractTest;
using Luny.Engine;
using Luny.Engine.Bridge.Enums;
using LunyScript.Execution;
using NUnit.Framework;

namespace LunyScript.Test
{
	public sealed class VariableTestScript : LunyScript
	{
		public override void Build()
		{
			var hp = Var.Get("hp");
			var score = GVar.Get("score");
			var isDead = Var.Get("isDead");

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
				If(hp.IsGreater(50)).Then(Var.Set("status", "healthy")),
				If(isDead.IsTrue()).Then(GVar.Set("lastEvent", "died")),

				// Test Multiplex arithmetic
				hp.Set(hp * 2), // 140
				hp.Set(hp / 2) // 70
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
			var gVars = LunyScriptEngine.Instance.GlobalVars;

			var obj = LunyEngine.Instance.Object.CreateEmpty(nameof(VariableTestScript));
			var context = (LunyScriptContext)LunyScriptEngine.Instance.GetScriptContext(obj.NativeObjectID);
			var lVars = context.LocalVariables;

			SimulateFrames(5);

			Assert.That(lVars["hp"], Is.EqualTo((Variable)70));
			Assert.That(gVars["score"], Is.EqualTo((Variable)15));
			Assert.That(lVars["isDead"], Is.EqualTo((Variable)true));
			Assert.That(lVars["status"], Is.EqualTo((Variable)"healthy"));
			Assert.That(gVars["lastEvent"], Is.EqualTo((Variable)"died"));
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
