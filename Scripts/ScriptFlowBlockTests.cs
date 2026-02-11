using Luny;
using Luny.Engine.Bridge;
using LunyScript.Blocks;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace LunyScript.Test.Scripts
{
	[TestFixture]
	public sealed class ScriptFlowBlockTests
	{
		[Test]
		public void TestAndBlock()
		{
			var condTrue = new MockCondition(true);
			var condFalse = new MockCondition(false);

			Assert.That(AndBlock.Create(condTrue, condTrue).Evaluate(null), Is.True);
			Assert.That(AndBlock.Create(condTrue, condFalse).Evaluate(null), Is.False);
			Assert.That(AndBlock.Create().Evaluate(null), Is.True);
		}

		[Test]
		public void TestOrBlock()
		{
			var condTrue = new MockCondition(true);
			var condFalse = new MockCondition(false);

			Assert.That(OrBlock.Create(condTrue, condFalse).Evaluate(null), Is.True);
			Assert.That(OrBlock.Create(condFalse, condFalse).Evaluate(null), Is.False);
			Assert.That(OrBlock.Create().Evaluate(null), Is.False);
		}

		[Test]
		public void TestNotBlock()
		{
			var condTrue = new MockCondition(true);
			var condFalse = new MockCondition(false);

			Assert.That(NotBlock.Create(condTrue).Evaluate(null), Is.False);
			Assert.That(NotBlock.Create(condFalse).Evaluate(null), Is.True);
		}

		[Test]
		public void TestIfBlock()
		{
			var executed = false;
			var cond = new MockCondition(true);
			var action = new MockAction(() => executed = true);

			var ifBlock = IfBlock.Create(new List<(IScriptConditionBlock[], IScriptActionBlock[])>
			{
				(new[] { cond }, new[] { action }),
			}, null);

			ifBlock.Execute(null);
			Assert.That(executed, Is.True);
		}

		[Test]
		public void TestIfElseIfElseBlock()
		{
			var executedBranch = 0;
			var condFalse = new MockCondition(false);
			var condTrue = new MockCondition(true);
			var action1 = new MockAction(() => executedBranch = 1);
			var action2 = new MockAction(() => executedBranch = 2);
			var action3 = new MockAction(() => executedBranch = 3);

			// Test ElseIf branch
			var ifBlock1 = IfBlock.Create(new List<(IScriptConditionBlock[], IScriptActionBlock[])>
			{
				(new[] { condFalse }, new[] { action1 }),
				(new[] { condTrue }, new[] { action2 }),
			}, new[] { action3 });

			ifBlock1.Execute(null);
			Assert.That(executedBranch, Is.EqualTo(2));

			// Test Else branch
			executedBranch = 0;
			var ifBlock2 = IfBlock.Create(new List<(IScriptConditionBlock[], IScriptActionBlock[])>
			{
				(new[] { condFalse }, new[] { action1 }),
				(new[] { condFalse }, new[] { action2 }),
			}, new[] { action3 });

			ifBlock2.Execute(null);
			Assert.That(executedBranch, Is.EqualTo(3));
		}

		[Test]
		public void TestIfBlockNullConditions()
		{
			var executed = false;
			var action = new MockAction(() => executed = true);

			// null conditions should evaluate to true
			var ifBlock = IfBlock.Create(new List<(IScriptConditionBlock[], IScriptActionBlock[])>
			{
				(null, new[] { action }),
			}, null);

			ifBlock.Execute(null);
			Assert.That(executed, Is.True);
		}

		[Test]
		public void TestIfBlockEmptyConditions()
		{
			var executed = false;
			var action = new MockAction(() => executed = true);

			// empty conditions should evaluate to true
			var ifBlock = IfBlock.Create(new List<(IScriptConditionBlock[], IScriptActionBlock[])>
			{
				(Array.Empty<IScriptConditionBlock>(), new[] { action }),
			}, null);

			ifBlock.Execute(null);
			Assert.That(executed, Is.True);
		}

		[Test]
		public void TestForBlock()
		{
			var counts = new List<Int32>();
			var action = new MockActionWithContext(ctx => counts.Add(ctx.LoopStack.Peek()));

			var forBlock = ForBlock.Create(3, 1, new[] { action });
			var context = new MockRuntimeContext();

			forBlock.Execute(context);
			Assert.That(counts, Is.EqualTo(new[] { 1, 2, 3 }));
		}

		[Test]
		public void TestForBlockReverse()
		{
			var counts = new List<Int32>();
			var action = new MockActionWithContext(ctx => counts.Add(ctx.LoopStack.Peek()));

			var forBlock = ForBlock.Create(3, -1, new[] { action });
			var context = new MockRuntimeContext();

			forBlock.Execute(context);
			Assert.That(counts, Is.EqualTo(new[] { 3, 2, 1 }));
		}

		[Test]
		public void TestLoopCounterValue()
		{
			var context = new MockRuntimeContext();
			context.LoopStack.Push(42);

			var value = LoopCounterVariableBlock.Instance.GetValue(context);
			Assert.That(value.AsInt32(), Is.EqualTo(42));
		}

		private class MockCondition : IScriptConditionBlock
		{
			private readonly Boolean _result;
			public MockCondition(Boolean result) => _result = result;
			public Boolean Evaluate(IScriptRuntimeContext runtimeContext) => _result;
		}

		private class MockAction : IScriptActionBlock
		{
			private readonly Action _action;
			public MockAction(Action action) => _action = action;
			public void Execute(IScriptRuntimeContext runtimeContext) => _action();
		}

		private class MockActionWithContext : IScriptActionBlock
		{
			private readonly Action<IScriptRuntimeContext> _action;
			public MockActionWithContext(Action<IScriptRuntimeContext> action) => _action = action;
			public void Execute(IScriptRuntimeContext runtimeContext) => _action(runtimeContext);
		}

		private class MockRuntimeContext : IScriptRuntimeContext
		{
			public ScriptDefID ScriptDefId => default;
			public Type ScriptType => null;
			public ILunyObject LunyObject => null;
			public ITable GlobalVariables => null;
			public ITable LocalVariables => null;
			public Stack<Int32> LoopStack { get; } = new();
			public Int32 LoopCount => LoopStack.Count > 0 ? LoopStack.Peek() : 0;
		}
	}
}
