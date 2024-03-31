using Reloaded.Mod.Interfaces;
using nights.test.firstperson.Template;
using nights.test.firstperson.Configuration;
using Reloaded.Hooks.Definitions;
using CallingConventions = Reloaded.Hooks.Definitions.X86.CallingConventions;
using Reloaded.Hooks.Definitions.X86;
using System.Runtime.InteropServices;
using static Reloaded.Hooks.Definitions.X86.FunctionAttribute;
using nights.test.firstperson.structs;
using Reloaded.Memory.Interop;
using Reloaded.Hooks.Definitions.Enums;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace nights.test.firstperson;

/// <summary>
/// Your mod logic goes here.
/// </summary>
public class Mod : ModBase // <= Do not Remove.
{
	/// <summary>
	/// Provides access to the mod loader API.
	/// </summary>
	private readonly IModLoader _modLoader;

	/// <summary>
	/// Provides access to the Reloaded.Hooks API.
	/// </summary>
	/// <remarks>This is null if you remove dependency on Reloaded.SharedLib.Hooks in your mod.</remarks>
	private readonly IReloadedHooks _hooks;

	/// <summary>
	/// Provides access to the Reloaded logger.
	/// </summary>
	private readonly ILogger _logger;

	/// <summary>
	/// Entry point into the mod, instance that created this class.
	/// </summary>
	private readonly IMod _owner;

	/// <summary>
	/// Provides access to this mod's configuration.
	/// </summary>
	private Config _configuration;

	/// <summary>
	/// The configuration of the currently executing mod.
	/// </summary>
	private readonly IModConfig _modConfig;

	public Mod(ModContext context) {
		_modLoader = context.ModLoader;
		_hooks = context.Hooks;
		_logger = context.Logger;
		_owner = context.Owner;
		_configuration = context.Configuration;
		_modConfig = context.ModConfig;

		Globals.Hooks = _hooks;

		unsafe {
			CameraBoyGirlUpdateHook =
				_hooks.CreateHook<CameraBoyGirlUpdate>(CameraBoyGirlUpdateImpl, 0x42A240).Activate();
			CameraStageBeginUpdateHook =
				_hooks.CreateHook<CameraBoyGirlUpdate>(CameraBoyGirlUpdateImpl, 0x42ABB0).Activate();
			
			CameraNightsUpdateHook =
				_hooks.CreateHook<CameraBoyGirlUpdate>(CameraNightsUpdateImpl, 0x42A150).Activate();
			CameraCourseClearUpdateHook =
				_hooks.CreateHook<CameraBoyGirlUpdate>(CameraNightsUpdateImpl, 0x42A390).Activate();
			CameraCourseClear4UpdateHook =
				_hooks.CreateHook<CameraBoyGirlUpdate>(CameraNightsUpdateImpl, 0x42A680).Activate();
			CameraBossUpdateHook =
				_hooks.CreateHook<CameraBoyGirlUpdate>(CameraNightsUpdateImpl, 0x42CEE0).Activate();
			CameraNightsFromUpUpdateHook =
				_hooks.CreateHook<CameraBoyGirlUpdate>(CameraNightsUpdateImpl, 0x42B140).Activate();
			CameraNightsFromRearUpdateHook =
				_hooks.CreateHook<CameraBoyGirlUpdate>(CameraNightsUpdateImpl, 0x42B3D0).Activate();
			CameraInDoorUpdateHook =
				_hooks.CreateHook<CameraBoyGirlUpdate>(CameraNightsUpdateImpl, 0x42C810).Activate();
			CameraBossGulpoUpdateHook =
				_hooks.CreateHook<CameraBoyGirlUpdate>(CameraNightsUpdateImpl, 0x42D5A0).Activate();
			CameraSt6TowerUpdateHook =
				_hooks.CreateHook<CameraBoyGirlUpdate>(CameraNightsUpdateImpl, 0x42BCB0).Activate();
			CameraSt7EndUpdateHook =
				_hooks.CreateHook<CameraBoyGirlUpdate>(CameraNightsUpdateImpl, 0x42C2D0).Activate();

			CameraSt6CatapultUpdateHook =
				_hooks.CreateHook<CameraBoyGirlUpdate>(CameraCatapultUpdateImpl, 0x42BA80).Activate();

			MoveActionToAngleHook =
				_hooks.CreateHook<MoveActionToAngle>(MoveActionToAngleImpl, 0x4A2120).Activate();

			CamUp = new Pinnable<Vec3>(new Vec3 {
				X = 0f,
				Y = 1f,
				Z = 0f
			});
			string[] UpVectorAsm = {
				$"use32",
				$"MOV ecx, {(UInt32)CamUp.Pointer}"
			};
			UpVectorHook = _hooks.CreateAsmHook(
				UpVectorAsm, 0x428857, AsmHookBehaviour.ExecuteFirst
			).Activate();
		}
	}

	static public Pinnable<Vec3> CamUp;
	public IAsmHook UpVectorHook;

	static float Lerp(float a, float b, float t) {
		return a + t * (b - a);
	}

	static float InverseLerp(float a, float b, float value) {
		return (value - a) / (b - a);
	}

	public static float ApplyDeadzone(float axis, float deadzone) {
		if (Math.Abs(axis) < deadzone) {
			return 0f;
		} else {
			float sign = Math.Sign(axis);
			return sign * Lerp(0f, 1f, InverseLerp(deadzone, 1f, Math.Abs(axis)));
		}
	}

	public static float ShortToRad(short value) {
		return value * MathF.PI / short.MaxValue;
	}

	public static short RadToShort(float value) {
		return (short)(value * short.MaxValue / MathF.PI);
	}

	[DllImport("d3dx9_41.dll")]
	public unsafe static extern void* D3DXMatrixLookAtRH(void* pOut, Vec3* pEye, Vec3* pAt, Vec3* pUp);

	[Function(CallingConventions.MicrosoftThiscall)]
	public unsafe delegate PlayerSubType CameraBoyGirlUpdate(CameraBoyGirl* thisPtr, int a2);
	public IHook<CameraBoyGirlUpdate> CameraBoyGirlUpdateHook;
	public IHook<CameraBoyGirlUpdate> CameraStageBeginUpdateHook;
	public IHook<CameraBoyGirlUpdate> CameraCourseClearUpdateHook;
	public IHook<CameraBoyGirlUpdate> CameraCourseClear4UpdateHook;
	public IHook<CameraBoyGirlUpdate> CameraBossUpdateHook;
	public IHook<CameraBoyGirlUpdate> CameraNightsFromUpUpdateHook;
	public IHook<CameraBoyGirlUpdate> CameraNightsFromRearUpdateHook;
	public IHook<CameraBoyGirlUpdate> CameraInDoorUpdateHook;
	public IHook<CameraBoyGirlUpdate> CameraBossGulpoUpdateHook;
	public IHook<CameraBoyGirlUpdate> CameraNightsUpdateHook;
	public IHook<CameraBoyGirlUpdate> CameraSt6CatapultUpdateHook;
	public IHook<CameraBoyGirlUpdate> CameraSt6TowerUpdateHook;
	public IHook<CameraBoyGirlUpdate> CameraSt7EndUpdateHook;
	public unsafe PlayerSubType CameraBoyGirlUpdateImpl(CameraBoyGirl* cam, int a2) {
		// make player "invisible" (scale = 0)
		// ideally this shouldn't be done every frame but fuck it
		cam->player->PlayerSub->Animation->Scale =
			new Vec3 { X = 0f, Y = 0f, Z = 0f };

		// move camera to player's eyes
		cam->pos = cam->player->PlayerSub->Pos;
		cam->pos.Y += 1.7f; // move up to Claris' Eye Level (use Blender on model to find this)

		// rotate camera with right stick
		var right_x = (float)(*Globals.GameInput)->right_x / sbyte.MaxValue;
		right_x = ApplyDeadzone(right_x, 0.1f);
		var right_y = (float)(*Globals.GameInput)->right_y / sbyte.MaxValue;
		right_y = ApplyDeadzone(right_y, 0.1f);

		const float sensitivity = 500f;

		cam->rot.Y -= (short)(sensitivity * right_x);
		var flip = _configuration.InvertYAxis ? -1 : 1;
		cam->rot.X += (short)(sensitivity * right_y * flip);

		// rotate camera with mouse (disabled as too little time)

		//// hide cursor
		//ShowCursor(0);

		//const float mouse_sensitivity = 12f;

		//var foreground = GetForegroundWindow();
		//GetWindowRect(foreground, out Rect window_rect);

		//var MouseOrigin = new structs.Point {
		//	X = (window_rect.Left + window_rect.Right) / 2,
		//	Y = (window_rect.Bottom + window_rect.Top) / 2
		//};

		//GetCursorPos(out structs.Point mouse_pos);
		//// if controlling feels jank, lower mouse refresh rate
		//SetCursorPos(MouseOrigin.X, MouseOrigin.Y);

		//var delta_mouse_pos = new structs.Point {
		//	X = mouse_pos.X - MouseOrigin.X,
		//	Y = mouse_pos.Y - MouseOrigin.Y
		//};

		//cam->rot.Y -= (short)(mouse_sensitivity * delta_mouse_pos.X);
		//cam->rot.X += (short)(mouse_sensitivity * delta_mouse_pos.Y);

		//// clip cursor to middle of window
		//var clip_rect = new Rect {
		//	Left = MouseOrigin.X,
		//	Top = MouseOrigin.Y,
		//	Right = MouseOrigin.X,
		//	Bottom = MouseOrigin.Y
		//};

		// clamp pitch
		cam->rot.X = Math.Clamp(
			cam->rot.X,
			(short)(-short.MaxValue / 2 + 255),
			(short)(short.MaxValue / 2 - 255)
		);

		CamUp.Value.X = 0f;
		CamUp.Value.Y = 1f;
		CamUp.Value.Z = 0f;

		// update look at vector
		UpdateLookAt(cam);

		return cam->player->PlayerSub->Type;
	}

	Vec3 _prevPos = new Vec3();
	public unsafe PlayerSubType CameraNightsUpdateImpl(CameraBoyGirl* cam, int a2) {
		Console.WriteLine("cam: 0x" + ((int)cam).ToString("X"));

		// make player "invisible" (scale = 0)
		// ideally this shouldn't be done every frame but fuck it
		cam->player->PlayerSub->Animation->Scale =
			new Vec3 { X = 0f, Y = 0f, Z = 0f };

		// move camera to origin
		cam->pos = cam->player->PlayerSub->Pos;

		cam->rot.Y = cam->player->PlayerSub->Yaw;
		cam->rot.X = (short)-cam->player->PlayerSub->Pitch;
		cam->rot.Z = cam->player->PlayerSub->Roll;

		// gulpo 0x29 state (going through bubble)
		if (cam->player->PlayerSub->State == 0x29) {
			var vel = new Vec3 {
				X = cam->pos.X - _prevPos.X,
				Y = cam->pos.Y - _prevPos.Y,
				Z = cam->pos.Z - _prevPos.Z
			};
			if (vel.X != 0f || vel.Y != 0f || vel.Z != 0f) {
				cam->rot = ToEulerAngles(vel);
			}
		}
		// stick canyon elevator state
		if (cam->player->PlayerSub->State == 0x31) {
			cam->pos.Y += 1.7f; // because visitor - not nights (eyes)
			cam->rot.X = 0;
			cam->rot.Y = short.MinValue / 2;
			cam->rot.Z = 0;
		}
		_prevPos = cam->pos;

		// update look at vector
		UpdateLookAt(cam);

		// set up vector to be perpendicular to look at vector
		UpdateUp(cam);

		return cam->player->PlayerSub->Type;
	}

	public unsafe PlayerSubType CameraCatapultUpdateImpl(CameraBoyGirl* cam, int a2) {
		// move camera to player's eyes
		cam->pos = cam->player->PlayerSub->Pos;

		var vel = new Vec3 {
			X = cam->pos.X - _prevPos.X,
			Y = cam->pos.Y - _prevPos.Y,
			Z = cam->pos.Z - _prevPos.Z
		};
		if (vel.X != 0f || vel.Y != 0f || vel.Z != 0f) {
			cam->rot = ToEulerAngles(vel);
		}

		_prevPos = cam->pos;

		// update look at vector
		UpdateLookAt(cam);

		// set up vector to be perpendicular to look at vector
		UpdateUp(cam);

		return cam->player->PlayerSub->Type;
	}

	public unsafe void UpdateUp(CameraBoyGirl* cam) {
		var yaw = ShortToRad(cam->rot.Y);
		var pitch = -ShortToRad(cam->rot.X) + MathF.PI / 2f;
		var roll = ShortToRad(cam->rot.Z);
		CamUp.Value.X = MathF.Sin(yaw) * MathF.Cos(pitch) * MathF.Cos(roll) - MathF.Cos(yaw) * MathF.Sin(roll);
		CamUp.Value.Y = MathF.Sin(pitch) * MathF.Cos(roll);
		CamUp.Value.Z = MathF.Cos(yaw) * MathF.Cos(pitch) * MathF.Cos(roll) + MathF.Sin(yaw) * MathF.Sin(roll);
	}

	public unsafe void UpdateLookAt(CameraBoyGirl* cam) {
		var yaw = ShortToRad(cam->rot.Y);
		var pitch = -ShortToRad(cam->rot.X);
		cam->look_at.X = MathF.Sin(yaw) * MathF.Cos(pitch);
		cam->look_at.Y = MathF.Sin(pitch);
		cam->look_at.Z = MathF.Cos(yaw) * MathF.Cos(pitch);
	}

	public static Rot3 ToEulerAngles(Vec3 direction) {
		var yaw = RadToShort(MathF.Atan2(direction.X, direction.Z));
		var pitch = RadToShort(MathF.Asin(-direction.Y));
		return new Rot3 { X = pitch, Y = yaw, Z = 0 };
	}

	[Function(new[] { Register.esi }, Register.eax, StackCleanup.Caller)]
	public unsafe delegate void MoveActionToAngle(Player* player);
	public IHook<MoveActionToAngle> MoveActionToAngleHook;
	public unsafe void MoveActionToAngleImpl(Player* player) {
		// todo: make dpad work.

		if (
			player->PlayerSub->Type != PlayerSubType.Nights
			&& player->PlayerSub->Type != PlayerSubType.ClarisTwinSeeds
			&& player->PlayerSub->Type != PlayerSubType.ElliotTwinSeeds
		) {
			MoveActionToAngleHook.OriginalFunction(player);
			return;
		}

		var left_y = (float)(*Globals.GameInput)->left_y / sbyte.MaxValue;
		left_y = ApplyDeadzone(left_y, 0.1f);

		player->WantsMovement = (byte)((left_y < 0f) ? 1 : 0);

		// wahh! this is a mess :(
		if ((*Globals.GameStateManager)->Special != null) {
			if ((*Globals.GameStateManager)->Special->TopDownOrSledOrRear == 0x01) {
				if (player->Dream == 7 || player->Dream == 9 || player->Dream == 11) { // frozen bell (sled) or soft museum (cannon) or stick canyon (catapult/elevator)
					if (player->PlayerSub->Type == PlayerSubType.Nights) {
						MoveActionToAngleHook.OriginalFunction(player);
						return;
					}
				} else {
					// top down
					var right_x = (float)(*Globals.GameInput)->right_x / sbyte.MaxValue;
					right_x = ApplyDeadzone(right_x, 0.1f);

					var offset = player->Dream == 5 ? short.MinValue : short.MinValue / 2; // Mystic Forest : Splash Garden

					player->AngleTarget = (short)(player->PlayerSub->Yaw + offset - right_x * player->PlayerSub->SpinSpeed);
					if (right_x != 0f) {
						player->WantsMovement = 1;
					}
					return;
				}
			} else if ((*Globals.GameStateManager)->Special->RearOrInMuseum == 0x01) {
				if (player->Dream == 3) { // Splash Garden - rear view, else in Soft Museum's Museum
					MoveActionToAngleHook.OriginalFunction(player);
					return;
				}
			}
		}

		var right_y = (float)(*Globals.GameInput)->right_y / sbyte.MaxValue;
		right_y = ApplyDeadzone(right_y, 0.1f);
		var right_x2 = (float)(*Globals.GameInput)->right_x / sbyte.MaxValue;
		right_x2 = ApplyDeadzone(right_x2, 0.1f);
		var roll_angle = ShortToRad(player->PlayerSub->Roll);
		var roll_x = MathF.Sin(roll_angle);
		var roll_y = MathF.Cos(roll_angle);
		var dot = right_x2 * roll_x + right_y * roll_y;

		var flip = player->Dream == 0x13; // flip in elliots xmas stage
		var flip_sign = flip ? -1 : 1; // todo: config
		if (_configuration.InvertYAxis) {
			flip_sign = -flip_sign;
		}
		player->AngleTarget = (short)(player->PlayerSub->Pitch + short.MinValue / 2 + dot * player->PlayerSub->SpinSpeed * -1f * flip_sign);

		if (dot != 0f) {
			player->WantsMovement = 1;
		}
	}

	#region Standard Overrides
	public override void ConfigurationUpdated(Config configuration)
	{
		// Apply settings from configuration.
		// ... your code here.
		_configuration = configuration;
		_logger.WriteLine($"[{_modConfig.ModId}] Config Updated: Applying");
	}
	#endregion

	#region For Exports, Serialization etc.
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
	public Mod() { }
#pragma warning restore CS8618
	#endregion
}
