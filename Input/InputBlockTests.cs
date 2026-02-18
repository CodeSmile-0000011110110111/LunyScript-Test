using Luny;
using Luny.ContractTest;
using Luny.Engine.Bridge;
using Luny.Unity.Engine.Services;
using LunyScript.Blocks;
using NUnit.Framework;
using System;

namespace LunyScript.Test.Input
{
	public sealed class InputButtonValueTestScript : Script
	{
		public override void Build(ScriptContext context)
		{
			On.FrameUpdate(
				GVar["btn_value"].Set(Input.Button("Fire").Value)
			);
		}
	}

	[TestFixture]
	public sealed class InputAxisBlockTests : ContractTestBase
	{
		protected override NativeEngine Engine => NativeEngine.Unity;

		private UnityInputService InputService => (UnityInputService)LunyEngine.Instance.Input;

		[Test]
		public void Axis_Returns_Zero_When_No_Input()
		{
			var block = InputAxisBlock.Create("Move");
			var value = block.GetValue(null);

			Assert.That(value.Type, Is.EqualTo(Variable.ValueType.Vector2));
			Assert.That(value.AsVector2(), Is.EqualTo(LunyVector2.Zero));
		}

		[Test]
		public void Axis_Returns_Value_After_Simulate()
		{
			var expected = new LunyVector2(0.7f, -0.3f);
			InputService.SimulateAxisInput("Move", expected);

			var block = InputAxisBlock.Create("Move");
			var value = block.GetValue(null);

			Assert.That(value.AsVector2(), Is.EqualTo(expected));
		}

		[Test]
		public void Axis_GetValueGeneric_Returns_LunyVector2()
		{
			var expected = new LunyVector2(1f, 0f);
			InputService.SimulateAxisInput("Move", expected);

			var block = InputAxisBlock.Create("Move");
			var vec = block.GetValue<LunyVector2>(null);

			Assert.That(vec, Is.EqualTo(expected));
		}

		[Test]
		public void Axis_Persists_Across_Frames()
		{
			var expected = new LunyVector2(0.5f, 0.5f);
			InputService.SimulateAxisInput("Look", expected);

			SimulateFrames(3);

			var block = InputAxisBlock.Create("Look");
			Assert.That(block.GetValue<LunyVector2>(null), Is.EqualTo(expected));
		}
	}

	[TestFixture]
	public sealed class InputIsPressedBlockTests : ContractTestBase
	{
		protected override NativeEngine Engine => NativeEngine.Unity;

		private UnityInputService InputService => (UnityInputService)LunyEngine.Instance.Input;

		[Test]
		public void IsPressed_False_When_No_Input()
		{
			var block = InputIsPressedBlock.Create("Fire");
			Assert.That(block.Evaluate(null), Is.False);
		}

		[Test]
		public void IsPressed_True_While_Held()
		{
			InputService.SimulateButtonInput("Fire", true);

			var block = InputIsPressedBlock.Create("Fire");
			Assert.That(block.Evaluate(null), Is.True);
		}

		[Test]
		public void IsPressed_False_After_Release()
		{
			InputService.SimulateButtonInput("Fire", true);
			InputService.SimulateButtonInput("Fire", false);

			var block = InputIsPressedBlock.Create("Fire");
			Assert.That(block.Evaluate(null), Is.False);
		}
	}

	[TestFixture]
	public sealed class InputIsJustPressedBlockTests : ContractTestBase
	{
		protected override NativeEngine Engine => NativeEngine.Unity;

		private UnityInputService InputService => (UnityInputService)LunyEngine.Instance.Input;

		[Test]
		public void IsJustPressed_False_When_No_Input()
		{
			var block = InputIsJustPressedBlock.Create("Jump");
			Assert.That(block.Evaluate(null), Is.False);
		}

		[Test]
		public void IsJustPressed_True_On_Press_Frame()
		{
			InputService.SimulateButtonInput("Jump", true);

			var block = InputIsJustPressedBlock.Create("Jump");
			Assert.That(block.Evaluate(null), Is.True);
		}

		[Test]
		public void IsJustPressed_False_After_PreUpdate()
		{
			InputService.SimulateButtonInput("Jump", true);
			SimulateFrames(1);

			var block = InputIsJustPressedBlock.Create("Jump");
			Assert.That(block.Evaluate(null), Is.False);
		}
	}

	[TestFixture]
	public sealed class InputButtonValueBlockTests : ContractTestBase
	{
		protected override NativeEngine Engine => NativeEngine.Unity;

		private UnityInputService InputService => (UnityInputService)LunyEngine.Instance.Input;

		[Test]
		public void ButtonValue_Zero_When_No_Input()
		{
			var block = InputButtonValueBlock.Create("Trigger");
			Assert.That(block.GetValue(null).AsDouble(), Is.EqualTo(0.0));
		}

		[Test]
		public void ButtonValue_Returns_Analog()
		{
			InputService.SimulateButtonInput("Trigger", true, 0.75f);

			var block = InputButtonValueBlock.Create("Trigger");
			Assert.That(block.GetValue(null).AsDouble(), Is.EqualTo(0.75).Within(0.01));
		}

		[Test]
		public void ButtonValue_In_Script_Via_GVar()
		{
			LunyEngine.Instance.Object.CreateEmpty(nameof(InputButtonValueTestScript));
			InputService.SimulateButtonInput("Fire", true, 0.6f);

			SimulateFrames(1);

			var gVars = ScriptEngine.Instance.GlobalVariables;
			Assert.That(gVars["btn_value"].AsDouble(), Is.EqualTo(0.6).Within(0.01));
		}

		[Test]
		public void ButtonValue_In_Script_Zero_After_Release()
		{
			LunyEngine.Instance.Object.CreateEmpty(nameof(InputButtonValueTestScript));
			InputService.SimulateButtonInput("Fire", true, 0.8f);
			SimulateFrames(1);

			InputService.SimulateButtonInput("Fire", false);
			SimulateFrames(1);

			var gVars = ScriptEngine.Instance.GlobalVariables;
			Assert.That(gVars["btn_value"].AsDouble(), Is.EqualTo(0.0));
		}
	}
}
