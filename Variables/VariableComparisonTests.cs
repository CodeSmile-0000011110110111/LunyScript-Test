using Luny;
using Luny.ContractTest;
using Luny.Engine.Bridge;
using LunyScript.Activation;
using NUnit.Framework;

namespace LunyScript.Test.Variables
{
	public sealed class ComparisonOperationsTestScript : Script
	{
		public override void Build(ScriptContext context)
		{
			var a = GVar["a"];
			var b = GVar["b"];
			var c = GVar["c"];
			var isTrue = GVar["isTrue"];
			var isFalse = GVar["isFalse"];

 		On.Ready(
				a.Set(10),
				b.Set(20),
				c.Set(10),
				isTrue.Set(true),
				isFalse.Set(false),

				// Variable-to-variable operators
				If(a == c).Then(GVar["o_eq"].Set(true)),
				If(a != b).Then(GVar["o_neq"].Set(true)),
				If(b > a).Then(GVar["o_gt"].Set(true)),
				If(b >= a).Then(GVar["o_ge1"].Set(true)),
				If(a >= c).Then(GVar["o_ge2"].Set(true)),
				If(a < b).Then(GVar["o_lt"].Set(true)),
				If(a <= b).Then(GVar["o_le1"].Set(true)),
				If(a <= c).Then(GVar["o_le2"].Set(true)),
				If(isTrue).Then(GVar["o_true"].Set(true)),
				If(!isFalse).Then(GVar["o_not_false"].Set(true)),

				// Variable-to-literal operators
				If(a == 10).Then(GVar["lv_eq"].Set(true)),
				If(a != 20).Then(GVar["lv_neq"].Set(true)),
				If(b > 10).Then(GVar["lv_gt"].Set(true)),
				If(b >= 20).Then(GVar["lv_ge1"].Set(true)),
				If(a >= 10).Then(GVar["lv_ge2"].Set(true)),
				If(a < 20).Then(GVar["lv_lt"].Set(true)),
				If(a <= 20).Then(GVar["lv_le1"].Set(true)),
				If(a <= 10).Then(GVar["lv_le2"].Set(true))
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
			var gVars = ScriptEngine.Instance.GlobalVariables;

			SimulateFrames(1);

			// Variable-to-variable operators
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
