using Luny;
using Luny.ContractTest;
using Luny.Engine.Bridge;
using LunyScript.Activation;
using NUnit.Framework;
using System;

namespace LunyScript.Test.Variables
{
	public sealed class ArithmeticOperationsTestScript : Script
	{
		public override void Build(ScriptContext context)
		{
			// Constants
			var three = Define("three", 3);
			var four = Define("four", 4);
			var five = Define("five", 5);
			var ten = Define("ten", 10);

			// Method variants
			var v_set = GVar["v_set"];
			var v_add = GVar["v_add"];
			var v_sub = GVar["v_sub"];
			var v_mul = GVar["v_mul"];
			var v_div = GVar["v_div"];
			var v_inc = GVar["v_inc"];
			var v_dec = GVar["v_dec"];
			var v_tog = GVar["v_tog"];

			// Operator variants
			var o_add = GVar["o_add"];
			var o_sub = GVar["o_sub"];
			var o_mul = GVar["o_mul"];
			var o_div = GVar["o_div"];
			var o_inc_pre = GVar["o_inc_pre"];
			var o_inc_post = GVar["o_inc_post"];
			var o_dec_pre = GVar["o_dec_pre"];
			var o_dec_post = GVar["o_dec_post"];

			// Inter-variable variants
			var v_add_v = GVar["v_add_v"];
			var v_sub_v = GVar["v_sub_v"];
			var v_mul_v = GVar["v_mul_v"];
			var v_div_v = GVar["v_div_v"];
			var v_complex = GVar["v_complex"];
			var v_complex2 = GVar["v_complex2"];
			var v_lit_left = GVar["v_lit_left"];
			var v_lit_right = GVar["v_lit_right"];

			// VariableBlock variants (explicit method calls)
			var m_add_b = GVar["m_add_b"];
			var m_sub_b = GVar["m_sub_b"];
			var m_mul_b = GVar["m_mul_b"];
			var m_div_b = GVar["m_div_b"];

			On.Ready(
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
				v_add_v.Set(five + ten), // 15
				v_sub_v.Set(ten - five), // 5
				v_mul_v.Set(five * ten), // 50
				v_div_v.Set(ten / five), // 2
				v_complex.Set(five * ten / four - three), // 9.5
				v_complex2.Set(five * (ten / four - three)), // -2.5
				(10 + v_lit_left).Set(100),
				(v_lit_right + 20).Set(200),

				// Explicitly call Add/Sub/Mul/Div(VariableBlock)
				m_add_b.Set(10), m_add_b.Add(ten), // 20
				m_sub_b.Set(200), m_sub_b.Sub(five), // 195 (200 - 5)
				m_mul_b.Set(10), m_mul_b.Mul(ten), // 100
				m_div_b.Set(10), m_div_b.Div(five) // 2 (10 / 5)
			);

 		var v_upd_inc = GVar["v_upd_inc"];
			var v_upd_tog = GVar["v_upd_tog"];
			GVar["v_upd_inc"] = 0;
			GVar["v_upd_tog"] = false;

			On.FrameUpdate(
				v_upd_inc.Inc(),
				v_upd_tog.Toggle()
			);
		}
	}

	[TestFixture]
	public sealed class VariableArithmeticTests : ContractTestBase
	{
		protected override NativeEngine Engine => NativeEngine.Unity;

		[Test]
		public void Arithmetic_Operations_Work()
		{
			LunyEngine.Instance.Object.CreateEmpty(nameof(ArithmeticOperationsTestScript));
			var gVars = ScriptEngine.Instance.GlobalVariables;

			var frameCount = 3;
			SimulateFrames(frameCount);

			// Method variants
			Assert.That(gVars["v_set"], Is.EqualTo((Variable)100));
			Assert.That(gVars["v_add"], Is.EqualTo((Variable)15));
			Assert.That(gVars["v_sub"], Is.EqualTo((Variable)5));
			Assert.That(gVars["v_mul"], Is.EqualTo((Variable)50));
			Assert.That(gVars["v_div"], Is.EqualTo((Variable)5));
			Assert.That(gVars["v_inc"], Is.EqualTo((Variable)11));
			Assert.That(gVars["v_dec"], Is.EqualTo((Variable)9));
			Assert.That(gVars["v_tog"].AsBoolean(), Is.True);

			// Operator variants
			Assert.That(gVars["o_add"], Is.EqualTo((Variable)15));
			Assert.That(gVars["o_sub"], Is.EqualTo((Variable)5));
			Assert.That(gVars["o_mul"], Is.EqualTo((Variable)50));
			Assert.That(gVars["o_div"], Is.EqualTo((Variable)5));
			Assert.That(gVars["o_inc_pre"], Is.EqualTo((Variable)11));
			Assert.That(gVars["o_inc_post"], Is.EqualTo((Variable)10));
			Assert.That(gVars["o_dec_pre"], Is.EqualTo((Variable)9));
			Assert.That(gVars["o_dec_post"], Is.EqualTo((Variable)10));

			// Inter-variable variants
			Assert.That(gVars["v_add_v"], Is.EqualTo((Variable)15));
			Assert.That(gVars["v_sub_v"], Is.EqualTo((Variable)5));
			Assert.That(gVars["v_mul_v"], Is.EqualTo((Variable)50));
			Assert.That(gVars["v_div_v"], Is.EqualTo((Variable)2));
			Assert.That(gVars["v_complex"], Is.EqualTo(9.5));
			Assert.That(gVars["v_complex2"], Is.EqualTo(-2.5));

			// VariableBlock variants
			Assert.That(gVars["m_add_b"], Is.EqualTo((Variable)20));
			Assert.That(gVars["m_sub_b"], Is.EqualTo((Variable)195));
			Assert.That(gVars["m_mul_b"], Is.EqualTo((Variable)100));
			Assert.That(gVars["m_div_b"], Is.EqualTo((Variable)2));

			// Literals in expressions targeting the variable
			Assert.That(gVars["v_lit_left"], Is.EqualTo((Variable)100));
			Assert.That(gVars["v_lit_right"], Is.EqualTo((Variable)200));

			// Update increment test
			Assert.That(gVars["v_upd_inc"], Is.EqualTo((Variable)frameCount));
			Assert.That((Boolean)gVars["v_upd_tog"], Is.EqualTo((Variable)true));
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
