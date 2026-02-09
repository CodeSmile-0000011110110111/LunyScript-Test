using Luny;
using Luny.ContractTest;
using Luny.Engine.Bridge.Enums;
using NUnit.Framework;

namespace LunyScript.Test.Variables
{
	public sealed class ComparisonOperationsTestScript : LunyScript
	{
		public override void Build()
		{
			var a = GVar("a");
			var b = GVar("b");
			var c = GVar("c");
			var isTrue = GVar("isTrue");
			var isFalse = GVar("isFalse");

			On.Ready(
				a.Set(10),
				b.Set(20),
				c.Set(10),
				isTrue.Set(true),
				isFalse.Set(false),

				// Method variants
				If(a.IsEqualTo(c)).Then(GVar("m_eq").Set(true)),
				If(a.IsNotEqualTo(b)).Then(GVar("m_neq").Set(true)),
				If(b.IsGreaterThan(a)).Then(GVar("m_gt").Set(true)),
				If(b.IsAtLeast(a)).Then(GVar("m_ge1").Set(true)),
				If(a.IsAtLeast(c)).Then(GVar("m_ge2").Set(true)),
				If(a.IsLessThan(b)).Then(GVar("m_lt").Set(true)),
				If(a.IsAtMost(b)).Then(GVar("m_le1").Set(true)),
				If(a.IsAtMost(c)).Then(GVar("m_le2").Set(true)),
				If(isTrue.IsTrue()).Then(GVar("m_true").Set(true)),
				If(isFalse.IsFalse()).Then(GVar("m_false").Set(true)),

				// Operator variants
				If(a == c).Then(GVar("o_eq").Set(true)),
				If(a != b).Then(GVar("o_neq").Set(true)),
				If(b > a).Then(GVar("o_gt").Set(true)),
				If(b >= a).Then(GVar("o_ge1").Set(true)),
				If(a >= c).Then(GVar("o_ge2").Set(true)),
				If(a < b).Then(GVar("o_lt").Set(true)),
				If(a <= b).Then(GVar("o_le1").Set(true)),
				If(a <= c).Then(GVar("o_le2").Set(true)),
				If(isTrue).Then(GVar("o_true").Set(true)),
				If(!isFalse).Then(GVar("o_not_false").Set(true)),

				// Is* with Variable literals
				If(a.IsEqualTo(10)).Then(GVar("lv_eq").Set(true)),
				If(a.IsNotEqualTo(20)).Then(GVar("lv_neq").Set(true)),
				If(b.IsGreaterThan(10)).Then(GVar("lv_gt").Set(true)),
				If(b.IsAtLeast(20)).Then(GVar("lv_ge1").Set(true)),
				If(a.IsAtLeast(10)).Then(GVar("lv_ge2").Set(true)),
				If(a.IsLessThan(20)).Then(GVar("lv_lt").Set(true)),
				If(a.IsAtMost(20)).Then(GVar("lv_le1").Set(true)),
				If(a.IsAtMost(10)).Then(GVar("lv_le2").Set(true))
			);
		}
	}

	[TestFixture]
	public sealed class VariableComparisonTests : ContractTestBase
	{
		protected override NativeEngine Engine => NativeEngine.Unity;

		[Test]
		public void Comparison_Operations_Work()
		{
			LunyEngine.Instance.Object.CreateEmpty(nameof(ComparisonOperationsTestScript));
			var gVars = LunyScriptEngine.Instance.GlobalVariables;

			SimulateFrames(1);

			// Method variants
			Assert.That(gVars["m_eq"].AsBoolean(), Is.True, "m_eq");
			Assert.That(gVars["m_neq"].AsBoolean(), Is.True, "m_neq");
			Assert.That(gVars["m_gt"].AsBoolean(), Is.True, "m_gt");
			Assert.That(gVars["m_ge1"].AsBoolean(), Is.True, "m_ge1");
			Assert.That(gVars["m_ge2"].AsBoolean(), Is.True, "m_ge2");
			Assert.That(gVars["m_lt"].AsBoolean(), Is.True, "m_lt");
			Assert.That(gVars["m_le1"].AsBoolean(), Is.True, "m_le1");
			Assert.That(gVars["m_le2"].AsBoolean(), Is.True, "m_le2");
			Assert.That(gVars["m_true"].AsBoolean(), Is.True, "m_true");
			Assert.That(gVars["m_false"].AsBoolean(), Is.True, "m_false");

			// Operator variants
			Assert.That(gVars["o_eq"].AsBoolean(), Is.True, "o_eq");
			Assert.That(gVars["o_neq"].AsBoolean(), Is.True, "o_neq");
			Assert.That(gVars["o_gt"].AsBoolean(), Is.True, "o_gt");
			Assert.That(gVars["o_ge1"].AsBoolean(), Is.True, "o_ge1");
			Assert.That(gVars["o_ge2"].AsBoolean(), Is.True, "o_ge2");
			Assert.That(gVars["o_lt"].AsBoolean(), Is.True, "o_lt");
			Assert.That(gVars["o_le1"].AsBoolean(), Is.True, "o_le1");
			Assert.That(gVars["o_le2"].AsBoolean(), Is.True, "o_le2");
			Assert.That(gVars["o_true"].AsBoolean(), Is.True, "o_true");
			Assert.That(gVars["o_not_false"].AsBoolean(), Is.True, "o_not_false");

			// Literal Variable comparison variants
			Assert.That(gVars["lv_eq"].AsBoolean(), Is.True, "lv_eq");
			Assert.That(gVars["lv_neq"].AsBoolean(), Is.True, "lv_neq");
			Assert.That(gVars["lv_gt"].AsBoolean(), Is.True, "lv_gt");
			Assert.That(gVars["lv_ge1"].AsBoolean(), Is.True, "lv_ge1");
			Assert.That(gVars["lv_ge2"].AsBoolean(), Is.True, "lv_ge2");
			Assert.That(gVars["lv_lt"].AsBoolean(), Is.True, "lv_lt");
			Assert.That(gVars["lv_le1"].AsBoolean(), Is.True, "lv_le1");
			Assert.That(gVars["lv_le2"].AsBoolean(), Is.True, "lv_le2");
		}
	}
}
