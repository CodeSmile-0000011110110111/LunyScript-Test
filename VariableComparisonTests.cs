using Luny;
using Luny.ContractTest;
using Luny.Engine.Bridge.Enums;
using LunyScript.Execution;
using NUnit.Framework;

namespace LunyScript.Test
{
	public sealed class ComparisonOperationsTestScript : LunyScript
	{
		public override void Build()
		{
			var a = Var("a");
			var b = Var("b");
			var c = Var("c");
			var isTrue = Var("isTrue");
			var isFalse = Var("isFalse");

			When.Self.Ready(
				a.Set(10),
				b.Set(20),
				c.Set(10),
				isTrue.Set(true),
				isFalse.Set(false),

				// Method variants
				If(a.IsEqualTo(c)).Then(Var("m_eq").Set(true)),
				If(a.IsNotEqualTo(b)).Then(Var("m_neq").Set(true)),
				If(b.IsGreaterThan(a)).Then(Var("m_gt").Set(true)),
				If(b.IsAtLeast(a)).Then(Var("m_ge1").Set(true)),
				If(a.IsAtLeast(c)).Then(Var("m_ge2").Set(true)),
				If(a.IsLessThan(b)).Then(Var("m_lt").Set(true)),
				If(a.IsAtMost(b)).Then(Var("m_le1").Set(true)),
				If(a.IsAtMost(c)).Then(Var("m_le2").Set(true)),
				If(isTrue.IsTrue()).Then(Var("m_true").Set(true)),
				If(isFalse.IsFalse()).Then(Var("m_false").Set(true)),

				// Operator variants
				If(a == c).Then(Var("o_eq").Set(true)),
				If(a != b).Then(Var("o_neq").Set(true)),
				If(b > a).Then(Var("o_gt").Set(true)),
				If(b >= a).Then(Var("o_ge1").Set(true)),
				If(a >= c).Then(Var("o_ge2").Set(true)),
				If(a < b).Then(Var("o_lt").Set(true)),
				If(a <= b).Then(Var("o_le1").Set(true)),
				If(a <= c).Then(Var("o_le2").Set(true)),
				If(isTrue).Then(Var("o_true").Set(true)),
				If(!isFalse).Then(Var("o_not_false").Set(true))
			);
		}
	}

	[TestFixture]
	public sealed class VariableComparisonTests : ContractTestBase
	{
		protected override NativeEngine Engine => NativeEngine.Unity;

		[SetUp]
		public void Setup() => LunyScriptEngine.Instance?.GlobalVars.RemoveAll();

		[Test]
		public void Comparison_Operations_Work()
		{
			var obj = LunyEngine.Instance.Object.CreateEmpty(nameof(ComparisonOperationsTestScript));
			var context = (LunyScriptContext)LunyScriptEngine.Instance.GetScriptContext(obj.NativeObjectID);
			var Vars = context.LocalVariables;

			SimulateFrames(1);

			// Method variants
			Assert.That(Vars["m_eq"].AsBoolean(), Is.True, "m_eq");
			Assert.That(Vars["m_neq"].AsBoolean(), Is.True, "m_neq");
			Assert.That(Vars["m_gt"].AsBoolean(), Is.True, "m_gt");
			Assert.That(Vars["m_ge1"].AsBoolean(), Is.True, "m_ge1");
			Assert.That(Vars["m_ge2"].AsBoolean(), Is.True, "m_ge2");
			Assert.That(Vars["m_lt"].AsBoolean(), Is.True, "m_lt");
			Assert.That(Vars["m_le1"].AsBoolean(), Is.True, "m_le1");
			Assert.That(Vars["m_le2"].AsBoolean(), Is.True, "m_le2");
			Assert.That(Vars["m_true"].AsBoolean(), Is.True, "m_true");
			Assert.That(Vars["m_false"].AsBoolean(), Is.True, "m_false");

			// Operator variants
			Assert.That(Vars["o_eq"].AsBoolean(), Is.True, "o_eq");
			Assert.That(Vars["o_neq"].AsBoolean(), Is.True, "o_neq");
			Assert.That(Vars["o_gt"].AsBoolean(), Is.True, "o_gt");
			Assert.That(Vars["o_ge1"].AsBoolean(), Is.True, "o_ge1");
			Assert.That(Vars["o_ge2"].AsBoolean(), Is.True, "o_ge2");
			Assert.That(Vars["o_lt"].AsBoolean(), Is.True, "o_lt");
			Assert.That(Vars["o_le1"].AsBoolean(), Is.True, "o_le1");
			Assert.That(Vars["o_le2"].AsBoolean(), Is.True, "o_le2");
			Assert.That(Vars["o_true"].AsBoolean(), Is.True, "o_true");
			Assert.That(Vars["o_not_false"].AsBoolean(), Is.True, "o_not_false");
		}
	}
}
