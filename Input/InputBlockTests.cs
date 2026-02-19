using Luny;
using Luny.ContractTest;
using Luny.Engine.Bridge;
using Luny.Unity.Engine.Services;
using LunyScript.Blocks;
using NUnit.Framework;

namespace LunyScript.Test.Input
{
	public sealed class InputAxisTestScript : Script
	{
		public override void Build(ScriptContext context) => On.FrameUpdate(
			GVar["btn_value"].Set(Input.Axis("Fire").Value)
		);
	}

	[TestFixture]
	public sealed class InputDirectionBlockTests : ContractTestBase
	{
		protected override NativeEngine Engine => NativeEngine.Unity;

		private UnityInputService InputService => (UnityInputService)LunyEngine.Instance.Input;

		[Test]
		public void Directional_Returns_Zero_When_No_Input()
		{
			var block = InputDirectionBlock.Create("Move");
			var value = block.GetValue(null);

			Assert.That(value.Type, Is.EqualTo(Variable.ValueType.Vector2));
			Assert.That(value.AsVector2(), Is.EqualTo(LunyVector2.Zero));
		}

		[Test]
		public void Directional_Returns_Value_After_Simulate()
		{
			var expected = new LunyVector2(0.7f, -0.3f);
			InputService.SimulateDirectionalInput("Move", expected);

			var block = InputDirectionBlock.Create("Move");
			var value = block.GetValue(null);

			Assert.That(value.AsVector2(), Is.EqualTo(expected));
		}

		[Test]
		public void Directional_GetValueGeneric_Returns_LunyVector2()
		{
			var expected = new LunyVector2(1f, 0f);
			InputService.SimulateDirectionalInput("Move", expected);

			var block = InputDirectionBlock.Create("Move");
			var vec = block.GetValue<LunyVector2>(null);

			Assert.That(vec, Is.EqualTo(expected));
		}

		[Test]
		public void Directional_NotCleared_Across_Frames()
		{
			var expected = new LunyVector2(0.5f, 0.5f);
			InputService.SimulateDirectionalInput("Look", expected);

			SimulateFrames(1);

			var block = InputDirectionBlock.Create("Look");
			Assert.That(block.GetValue<LunyVector2>(null), Is.EqualTo(expected));
		}
	}

	[TestFixture]
	public sealed class InputButtonIsPressedBlockTests : ContractTestBase
	{
		protected override NativeEngine Engine => NativeEngine.Unity;

		private UnityInputService InputService => (UnityInputService)LunyEngine.Instance.Input;

		[Test]
		public void IsPressed_False_When_No_Input()
		{
			var block = InputButtonIsPressedBlock.Create("Fire");
			Assert.That(block.Evaluate(null), Is.False);
		}

		[Test]
		public void IsPressed_True_While_Held()
		{
			InputService.SimulateButtonInput("Fire", true);

			var block = InputButtonIsPressedBlock.Create("Fire");
			Assert.That(block.Evaluate(null), Is.True);
		}

		[Test]
		public void IsPressed_False_After_Release()
		{
			InputService.SimulateButtonInput("Fire", true);
			InputService.SimulateButtonInput("Fire", false);

			var block = InputButtonIsPressedBlock.Create("Fire");
			Assert.That(block.Evaluate(null), Is.False);
		}
	}

	[TestFixture]
	public sealed class InputButtonIsJustPressedBlockTests : ContractTestBase
	{
		protected override NativeEngine Engine => NativeEngine.Unity;

		private UnityInputService InputService => (UnityInputService)LunyEngine.Instance.Input;

		[Test]
		public void IsJustPressed_False_When_No_Input()
		{
			var block = InputButtonIsJustPressedBlock.Create("Jump");
			Assert.That(block.Evaluate(null), Is.False);
		}

		[Test]
		public void IsJustPressed_True_On_Press_Frame()
		{
			InputService.SimulateButtonInput("Jump", true);

			var block = InputButtonIsJustPressedBlock.Create("Jump");
			Assert.That(block.Evaluate(null), Is.True);
		}

		[Test]
		public void IsJustPressed_False_After_PreUpdate()
		{
			InputService.SimulateButtonInput("Jump", true);
			SimulateFrames(1);

			var block = InputButtonIsJustPressedBlock.Create("Jump");
			Assert.That(block.Evaluate(null), Is.False);
		}
	}

	[TestFixture]
	public sealed class InputAxisValueBlockTests : ContractTestBase
	{
		protected override NativeEngine Engine => NativeEngine.Unity;

		private UnityInputService InputService => (UnityInputService)LunyEngine.Instance.Input;

		[Test]
		public void ButtonValue_Zero_When_No_Input()
		{
			var block = InputAxisValueBlock.Create("Trigger");
			Assert.That(block.GetValue(null).AsDouble(), Is.EqualTo(0.0));
		}

		[Test]
		public void AxisValue_Returns_Analog()
		{
			InputService.SimulateAxisInput("Trigger", 0.75f);

			var block = InputAxisValueBlock.Create("Trigger");
			Assert.That(block.GetValue(null).AsDouble(), Is.EqualTo(0.75).Within(0.01));
		}

		[Test]
		public void AxisValue_In_Script_Via_GVar()
		{
			LunyEngine.Instance.Object.CreateEmpty(nameof(InputAxisTestScript));

			SimulateFrames(1);

			InputService.SimulateAxisInput("Fire", 0.6f);

			SimulateFrames(1);

			var gVars = ScriptEngine.Instance.GlobalVariables;
			Assert.That(gVars["btn_value"].AsDouble(), Is.EqualTo(0.6).Within(0.01));
		}

		[Test]
		public void ButtonValue_In_Script_Zero_After_Release()
		{
			LunyEngine.Instance.Object.CreateEmpty(nameof(InputAxisTestScript));
			InputService.SimulateButtonInput("Fire", true, 0.8f);
			SimulateFrames(1);

			InputService.SimulateButtonInput("Fire", false);
			SimulateFrames(1);

			var gVars = ScriptEngine.Instance.GlobalVariables;
			Assert.That(gVars["btn_value"].AsDouble(), Is.EqualTo(0.0));
		}
	}
}
