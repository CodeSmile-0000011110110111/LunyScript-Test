using Luny;
using Luny.ContractTest;
using Luny.Engine.Bridge.Enums;
using LunyScript.Blocks;
using LunyScript.Exceptions;
using NUnit.Framework;
using System;

namespace LunyScript.Test.Variables
{
	public sealed class VariableReadOnlyTests
	{
		[Test]
		public void Modification_On_Constant_Throws()
		{
			var constant = ConstantVariableBlock.Create(5);
			Assert.Throws<LunyScriptVariableException>(() => constant.Set(10));
		}

		[Test]
		public void Modification_On_LoopCounter_Throws()
		{
			var counter = LoopCounterVariableBlock.Instance;
			Assert.Throws<LunyScriptVariableException>(() => counter.Set(10));
		}

		[Test]
		public void Modification_On_Pure_Expression_Throws()
		{
			var expr = ConstantVariableBlock.Create(10) + ConstantVariableBlock.Create(20);
			Assert.Throws<LunyScriptVariableException>(() => expr.Set(100));
		}

		[Test]
		public void Modification_On_Named_Constant_Handle_Throws()
		{
			var table = new Table();
			var handle = table.DefineConstant("PI", 3.14159);
			var block = TableVariableBlock.Create(handle);

			Assert.Throws<LunyScriptVariableException>(() => block.Set(4));
		}
	}

	public sealed class NamedConstantTestScript : LunyScript
	{
		public override void Build()
		{
			var pi = Const("PI", 3.14159);
			var gVar = GVar("result");

			On.Ready(
				gVar.Set(pi)
			);
		}
	}

	[TestFixture]
	public sealed class NamedConstantTests : ContractTestBase
	{
		protected override NativeEngine Engine => NativeEngine.Unity;

		[Test]
		public void Named_Constant_Value_Is_Accessible()
		{
			LunyEngine.Instance.Object.CreateEmpty(nameof(NamedConstantTestScript));
			var gVars = LunyScriptEngine.Instance.GlobalVariables;

			SimulateFrames(1);

			Assert.That((Double)gVars["result"], Is.EqualTo(3.14159).Within(0.00001));
		}

		[Test]
		public void Named_Constant_Modification_In_Script_Throws()
		{
			var table = new Table();
			var handle = table.DefineConstant("PI", 3.14159);
			var block = TableVariableBlock.Create(handle);

			Assert.Throws<LunyScriptVariableException>(() => block.Set(4));
			Assert.Throws<LunyScriptVariableException>(() => block.Inc());
			Assert.Throws<LunyScriptVariableException>(() => block.Add(1));
		}
	}
}
