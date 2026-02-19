using Luny;
using Luny.Engine.Bridge;
using LunyScript.Blocks;
using LunyScript.Exceptions;
using NUnit.Framework;
using System;
using System.Runtime.CompilerServices;

namespace LunyScript.Test.Variables
{
	[TestFixture]
	public sealed class ComputedVariableBlockTests
	{
		[Test]
		public void TargetHandle_Is_Null()
		{
			var block = TestComputedBlock.Create(42);
			Assert.That(block.TargetHandle, Is.Null);
		}

		[Test]
		public void Set_Throws_ReadOnly_Exception()
		{
			var block = TestComputedBlock.Create(42);
			Assert.Throws<LunyScriptVariableException>(() => block.Set(10));
		}

		[Test]
		public void Inc_Throws_ReadOnly_Exception()
		{
			var block = TestComputedBlock.Create(42);
			Assert.Throws<LunyScriptVariableException>(() => block.Inc());
		}

		[Test]
		public void Add_Throws_ReadOnly_Exception()
		{
			var block = TestComputedBlock.Create(42);
			Assert.Throws<LunyScriptVariableException>(() => block.Add(1));
		}

		[Test]
		public void GetValue_Returns_Correct_Value()
		{
			var block = TestComputedBlock.Create(42);
			Assert.That(block.GetValue(null).AsDouble(), Is.EqualTo(42.0));
		}

		[Test]
		public void GetValueGeneric_Double_Returns_Correct_Value()
		{
			var block = TestComputedBlock.Create(42);
			Assert.That(block.GetValue<Double>(null), Is.EqualTo(42.0));
		}

		[Test]
		public void GetValueGeneric_Single_Returns_Correct_Value()
		{
			var block = TestComputedBlock.Create(42);
			Assert.That(block.GetValue<Single>(null), Is.EqualTo(42f));
		}

		[Test]
		public void GetValueGeneric_Boolean_Returns_Correct_Value()
		{
			var block = TestComputedBlock.Create(true);
			Assert.That(block.GetValue<Boolean>(null), Is.True);
		}

		[Test]
		public void GetValueGeneric_Int32_Returns_Correct_Value()
		{
			var block = TestComputedBlock.Create(42);
			Assert.That(block.GetValue<Int32>(null), Is.EqualTo(42));
		}

		[Test]
		public void GetValueGeneric_Vector2_Returns_Correct_Value()
		{
			var vec = new LunyVector2(3f, 4f);
			var block = TestVector2ComputedBlock.Create(vec);
			Assert.That(block.GetValue<LunyVector2>(null), Is.EqualTo(vec));
		}

		[Test]
		public void GetValueGeneric_Vector2_Via_BaseDefault()
		{
			var vec = new LunyVector2(3f, 4f);
			var block = TestComputedBlock.Create(Variable.FromVector2(vec));
			Assert.That(block.GetValue<LunyVector2>(null), Is.EqualTo(vec));
		}

		[Test]
		public void GetValueGeneric_Vector3_Via_BaseDefault()
		{
			var vec = new LunyVector3(1f, 2f, 3f);
			var block = TestComputedBlock.Create(Variable.FromVector3(vec));
			Assert.That(block.GetValue<LunyVector3>(null), Is.EqualTo(vec));
		}

		[Test]
		public void GetValueGeneric_Unsupported_Type_Throws()
		{
			var block = TestComputedBlock.Create(42);
			Assert.Throws<LunyScriptVariableException>(() => block.GetValue<LunyQuaternion>(null));
		}

		private sealed class TestComputedBlock : ComputedVariableBlock
		{
			private readonly Variable _value;

			internal static TestComputedBlock Create(Variable value) => new(value);
			private TestComputedBlock(Variable value) => _value = value;

			internal override Variable GetValue(IScriptRuntimeContext runtimeContext) => _value;
		}

		private sealed class TestVector2ComputedBlock : ComputedVariableBlock
		{
			private readonly LunyVector2 _value;

			internal static TestVector2ComputedBlock Create(LunyVector2 value) => new(value);
			private TestVector2ComputedBlock(LunyVector2 value) => _value = value;

			internal override Variable GetValue(IScriptRuntimeContext runtimeContext) => Variable.FromVector2(_value);

			internal override T GetValue<T>(IScriptRuntimeContext runtimeContext)
			{
				if (typeof(T) == typeof(LunyVector2))
				{
					var val = _value;
					return Unsafe.As<LunyVector2, T>(ref val);
				}
				return base.GetValue<T>(runtimeContext);
			}
		}
	}
}
