using Luny;
using Luny.ContractTest;
using Luny.Engine.Bridge.Enums;
using LunyScript.Execution;
using NUnit.Framework;

namespace LunyScript.Test
{
	public sealed class ArithmeticOperationsTestScript : LunyScript
	{
		public override void Build()
		{
			// Method variants
			var v_set = Var("v_set");
			var v_add = Var("v_add");
			var v_sub = Var("v_sub");
			var v_mul = Var("v_mul");
			var v_div = Var("v_div");
			var v_inc = Var("v_inc");
			var v_dec = Var("v_dec");
			var v_tog = Var("v_tog");

			// Operator variants
			var o_add = Var("o_add");
			var o_sub = Var("o_sub");
			var o_mul = Var("o_mul");
			var o_div = Var("o_div");
			var o_inc_pre = Var("o_inc_pre");
			var o_inc_post = Var("o_inc_post");
			var o_dec_pre = Var("o_dec_pre");
			var o_dec_post = Var("o_dec_post");

			// Inter-variable variants
			var five = Var("five");
			var ten = Var("ten");
			var four = Var("four");
			var three = Var("three");
			var v_add_v = Var("v_add_v");
			var v_sub_v = Var("v_sub_v");
			var v_mul_v = Var("v_mul_v");
			var v_div_v = Var("v_div_v");
			var v_complex = Var("v_complex");
			var v_complex2 = Var("v_complex2");

			When.Self.Ready(
				v_set.Set(100),
				v_add.Set(10), v_add.Add(5), // 15
				v_sub.Set(10), v_sub.Sub(5), // 5
				v_mul.Set(10), v_mul.Mul(5), // 50
				v_div.Set(10), v_div.Div(2), // 5
				v_inc.Set(10), v_inc.Inc(), // 11
				v_dec.Set(10), v_dec.Dec(), // 9
				v_tog.Set(false), v_tog.Toggle(), // true

				o_add.Set(10), o_add.Set(o_add + 5), // 15
				o_sub.Set(10), o_sub.Set(o_sub - 5), // 5
				o_mul.Set(10), o_mul.Set(o_mul * 5), // 50
				o_div.Set(10), o_div.Set(o_div / 2), // 5
				o_inc_pre.Set(10), o_inc_pre.Set(++o_inc_pre), // 11
				o_inc_post.Set(10), o_inc_post.Set(o_inc_post++), // 10 (postfix)
				o_dec_pre.Set(10), o_dec_pre.Set(--o_dec_pre), // 9
				o_dec_post.Set(10), o_dec_post.Set(o_dec_post--), // 10 (postfix)

				five.Set(5),
				ten.Set(10),
				four.Set(4),
				three.Set(3),
				v_add_v.Set(five + ten), // 15
				v_sub_v.Set(ten - five), // 5
				v_mul_v.Set(five * ten), // 50
				v_div_v.Set(ten / five), // 2
				v_complex.Set(five * ten / four - three), // 9.5
				v_complex2.Set(five * ((ten / four) - three)) // -2.5
			);
		}
	}

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
	public sealed class ScriptVariableTests : ContractTestBase
	{
		protected override NativeEngine Engine => NativeEngine.Unity;

		[SetUp]
		public void Setup() => LunyScriptEngine.Instance?.GlobalVars.RemoveAll();

		[Test]
		public void Arithmetic_Operations_Work()
		{
			var obj = LunyEngine.Instance.Object.CreateEmpty(nameof(ArithmeticOperationsTestScript));
			var context = (LunyScriptContext)LunyScriptEngine.Instance.GetScriptContext(obj.NativeObjectID);
			var Vars = context.LocalVariables;

			SimulateFrames(1);

			// Method variants
			Assert.That(Vars["v_set"], Is.EqualTo((Variable)100));
			Assert.That(Vars["v_add"], Is.EqualTo((Variable)15));
			Assert.That(Vars["v_sub"], Is.EqualTo((Variable)5));
			Assert.That(Vars["v_mul"], Is.EqualTo((Variable)50));
			Assert.That(Vars["v_div"], Is.EqualTo((Variable)5));
			Assert.That(Vars["v_inc"], Is.EqualTo((Variable)11));
			Assert.That(Vars["v_dec"], Is.EqualTo((Variable)9));
			Assert.That(Vars["v_tog"].AsBoolean(), Is.True);

			// Operator variants
			Assert.That(Vars["o_add"], Is.EqualTo((Variable)15));
			Assert.That(Vars["o_sub"], Is.EqualTo((Variable)5));
			Assert.That(Vars["o_mul"], Is.EqualTo((Variable)50));
			Assert.That(Vars["o_div"], Is.EqualTo((Variable)5));
			Assert.That(Vars["o_inc_pre"], Is.EqualTo((Variable)11));
			Assert.That(Vars["o_inc_post"], Is.EqualTo((Variable)10));
			Assert.That(Vars["o_dec_pre"], Is.EqualTo((Variable)9));
			Assert.That(Vars["o_dec_post"], Is.EqualTo((Variable)10));

			// Inter-variable variants
			Assert.That(Vars["v_add_v"], Is.EqualTo((Variable)15));
			Assert.That(Vars["v_sub_v"], Is.EqualTo((Variable)5));
			Assert.That(Vars["v_mul_v"], Is.EqualTo((Variable)50));
			Assert.That(Vars["v_div_v"], Is.EqualTo((Variable)2));
			Assert.That(Vars["v_complex"], Is.EqualTo((Variable)9.5));
			Assert.That(Vars["v_complex2"], Is.EqualTo((Variable)(-2.5)));

			int i = (int)-2.5f;
			Variable v = (Variable)-2.5f;
		}

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
