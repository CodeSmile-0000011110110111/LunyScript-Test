using LunyScript.Blocks;
using LunyScript.Exceptions;
using NUnit.Framework;

namespace LunyScript.Test
{
	public sealed class VariableReadOnlyModificationTests
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
	}
}
