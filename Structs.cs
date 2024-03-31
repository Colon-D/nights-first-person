using Reloaded.Hooks.Definitions;
using Reloaded.Hooks.Definitions.X86;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using static nights.test.firstperson.structs.JointArray;
using static nights.test.firstperson.structs.PlayerSub;

namespace nights.test.firstperson.structs;

[StructLayout(LayoutKind.Sequential)]
public struct Vec3 {
	public float X { get; set; }
	public float Y { get; set; }
	public float Z { get; set; }
}

[StructLayout(LayoutKind.Sequential)]
public struct Rot3 {
	public short X { get; set; }
	public short Y { get; set; }
	public short Z { get; set; }
}

[StructLayout(LayoutKind.Explicit)]
public unsafe struct PlayerSub {
	[StructLayout(LayoutKind.Explicit)]
	private unsafe struct PlayerSubVFTable {
		[FieldOffset(0x0)]
		public long Dtor;
		[FieldOffset(0x8)]
		public long Update;
		[FieldOffset(0x58)]
		public long InitMaybe;
	}

	[FieldOffset(0x0)]
	private PlayerSubVFTable* _vftable;

	[Function(CallingConventions.MicrosoftThiscall)]
	private unsafe delegate void DtorT(PlayerSub* self, char a2);
	[Function(CallingConventions.MicrosoftThiscall)]
	private unsafe delegate int UpdateT(PlayerSub* self);
	[Function(CallingConventions.MicrosoftThiscall)]
	private unsafe delegate int InitMaybeT(PlayerSub* self, int a2);

	public void Dtor() {
		Console.WriteLine("Dtor: 0x" + ((int)_vftable->Dtor).ToString("X"));
		var fn = Globals.Hooks.CreateWrapper<DtorT>(_vftable->Dtor, out _);
		fixed (PlayerSub* self = &this) {
			fn(self, (char)1);
		}
	}

	public int Update() {
		var fn = Globals.Hooks.CreateWrapper<UpdateT>(_vftable->Update, out _);
		fixed (PlayerSub* self = &this) {
			return fn(self);
		}
	}

	public int InitMaybe(int a2) {
		var fn = Globals.Hooks.CreateWrapper<InitMaybeT>(_vftable->InitMaybe, out _);
		fixed (PlayerSub* self = &this) {
			return fn(self, a2);
		}
	}

	[FieldOffset(0x84)]
	public int Unknown84;

	[FieldOffset(0x94)]
	public Vec3 Pos;
	[FieldOffset(0xA8)]
	public Vec3 RenderPos;

	[FieldOffset(0xBC)]
	public Vec3 Velocity;

	[FieldOffset(0xC8)]
	public short Pitch;

	[FieldOffset(0xCA)]
	public short Yaw;

	[FieldOffset(0xCC)]
	public short Roll;

	[FieldOffset(0xEC)]
	public Player* Player;

	[FieldOffset(0xF0)]
	public PlayerSubType Type;

	[FieldOffset(0xF4)]
	public int State;

	[FieldOffset(0x112)]
	public short SpinSpeed;

	[FieldOffset(0x114)]
	public float Speed;

	[FieldOffset(0x178)]
	public Vec3 BubbleSomething; // not velocity, idk what it actually is

	[FieldOffset(0x88)]
	public Animation* Animation;
}

[StructLayout(LayoutKind.Explicit)]
public unsafe struct Player {
	[FieldOffset(0x58)]
	public byte Unknown58;

	[FieldOffset(0x59)]
	public byte Unknown59;

	[FieldOffset(0x5A)]
	public byte WantsMovement;

	[FieldOffset(0x5C)]
	public short AngleTarget;

	[FieldOffset(0x60)]
	public PlayerSub* PlayerSub;

	// world's ugliest code, in C++/Rust this would be simple and good looking
	[FieldOffset(0x64)]
	private PlayerSub* _playerSubsBegin;
	public PlayerSub* GetPlayerSub(PlayerSubType type) {
		fixed (PlayerSub** _playerSubs = &_playerSubsBegin) {
			return _playerSubs[(int)type];
		}
	}

	[FieldOffset(0x80)]
	public int Dream;

	[FieldOffset(0x84)]
	public int Unknown84;

	[FieldOffset(0x88)]
	public int Unknown88;

	[FieldOffset(0x8C)]
	public int Unknown8C;

	[FieldOffset(0x90)]
	public int Unknown90;
}

public enum PlayerSubType {
	Nights,
	Elliot,
	Claris,
	ElliotTwinSeeds,
	ClarisTwinSeeds,
	OtherNightsWizemanFight
}

[StructLayout(LayoutKind.Explicit)]
public unsafe struct GameInput {
	[StructLayout(LayoutKind.Explicit)]
	public struct Action {
		[FieldOffset(0x0)]
		public int State;
		[FieldOffset(0x4)]
		public int Down;
		[FieldOffset(0x8)]
		public sbyte AnalogCopy;
		[FieldOffset(0x9)]
		public sbyte Analog;
		[FieldOffset(0xC)]
		public int FramesDown;
	};
	[FieldOffset(0x4)]
	private Action _actionsBegin;
	public Action* GetAction(int action) {
		fixed (Action* actions = &_actionsBegin) {
			return &actions[action];
		}
	}

	[FieldOffset(0x3D4)]
	public sbyte select;
	[FieldOffset(0x3D7)]
	public sbyte start;
	[FieldOffset(0x3D8)]
	public sbyte dpad_up;
	[FieldOffset(0x3D9)]
	public sbyte dpad_right;
	[FieldOffset(0x3DA)]
	public sbyte dpad_down;
	[FieldOffset(0x3DB)]
	public sbyte dpad_left;
	[FieldOffset(0x3DC)]
	public sbyte left_trigger;
	[FieldOffset(0x3DD)]
	public sbyte right_trigger;
	[FieldOffset(0x3DE)]
	public sbyte left_bumper;
	[FieldOffset(0x3DF)]
	public sbyte right_bumper;
	[FieldOffset(0x3E0)]
	public sbyte y;
	[FieldOffset(0x3E1)]
	public sbyte a;
	[FieldOffset(0x3E2)]
	public sbyte b;
	[FieldOffset(0x3E3)]
	public sbyte x;
	[FieldOffset(0x3E4)]
	public sbyte right_x;
	[FieldOffset(0x3E5)]
	public sbyte right_y;
	[FieldOffset(0x3E6)]
	public sbyte left_x;
	[FieldOffset(0x3E7)]
	public sbyte left_y;
}

[StructLayout(LayoutKind.Explicit)]
public unsafe struct WorldManager {
	[FieldOffset(0x50)]
	public Player* Player;
}

public unsafe struct Globals {
	public static unsafe WorldManager** WorldManager = (WorldManager**)0x24C4EC4;
	public static unsafe GameStateManager** GameStateManager = (GameStateManager**)0x24C4E94;
	public static unsafe GameInput** GameInput       = (GameInput**)0x24C4E88;
	public static unsafe uint** FramesElapsed        = (uint**)0x24C4E88;
	public static unsafe IntPtr* HWND                = (IntPtr*)0x24C44B0;

	public static IReloadedHooks Hooks;
}

[StructLayout(LayoutKind.Explicit)]
public unsafe struct Animation {
	[FieldOffset(0x18)]
	public byte Frozen;
	[FieldOffset(0x20)]
	public Motion* Motion;
	[FieldOffset(0x2C)]
	public Motion* MotionCopy;
	[FieldOffset(0x30)]
	public Vec3 Pos;
	[FieldOffset(0x3C)]
	public Rot3 Rot;
	[FieldOffset(0x58)]
	public Vec3 Scale;
}

[StructLayout(LayoutKind.Explicit)]
public unsafe struct Motion {
	[FieldOffset(0x14)]
	public int Animation;
	[FieldOffset(0x18)]
	public int Frame;
	[FieldOffset(0x116C)]
	public JointArray* JointArray;
	[FieldOffset(0x1290)]
	public int FrameAlt;
	[FieldOffset(0x1294)]
	public int Unknown1294;
	[FieldOffset(0x1298)]
	public int Unknown1298;
	[FieldOffset(0x12A8)]
	public int ThisNeedsToBe2OrAnimationsAreBrokenIDKWhy;
	[FieldOffset(0x12AC)]
	public int SpeedSometimes;
}

[StructLayout(LayoutKind.Explicit)]
public unsafe struct JointArray {
	[StructLayout(LayoutKind.Explicit)]
	private unsafe struct VFTable {
		[FieldOffset(0x0)]
		public long Dtor;
	}

	[FieldOffset(0x0)]
	private VFTable* _vftable;

	[Function(CallingConventions.MicrosoftThiscall)]
	private unsafe delegate void DtorT(JointArray* self, int a1);

	public void Dtor(int a1) {
		var fn = Globals.Hooks.CreateWrapper<DtorT>(_vftable->Dtor, out _);
		fixed (JointArray* self = &this) {
			fn(self, a1);
		}
	}
}

[StructLayout(LayoutKind.Explicit)]
public unsafe struct GameStateManager {
	[StructLayout(LayoutKind.Explicit)]
	public unsafe struct GameStateSpecial {
		[FieldOffset(0x4)]
		public byte TopDownOrSledOrRear; // depending on dream
		[FieldOffset(0x5)]
		public byte RearOrInMuseum;
	}

	[FieldOffset(0x3B8C)]
	public GameStateSpecial* Special;
}

[StructLayout(LayoutKind.Explicit)]
public unsafe struct CameraBoyGirl {
	[StructLayout(LayoutKind.Explicit)]
	private unsafe struct VFTable {
		[FieldOffset(0x0)]
		public long Dtor;
	}

	[FieldOffset(0x0)]
	private VFTable* _vftable;

	[FieldOffset(0x90)]
	public Vec3 pos;

	[FieldOffset(0x9C)]
	public Vec3 look_at;

	[FieldOffset(0xA8)]
	public Rot3 rot;
	[FieldOffset(0xBC)]
	public Rot3 rot2;

	[FieldOffset(0xAC)]
	public float dist;

	[FieldOffset(0xC8)]
	public Player* player;
};
