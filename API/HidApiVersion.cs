using System.Runtime.InteropServices;

namespace hid_sharp.API;

public struct HidApiVersion {
    public int Major;
    public int Minor;
    public int Patch;

    public static explicit operator nint(HidApiVersion hav) {
        nint ptr = Marshal.AllocHGlobal(Marshal.SizeOf(hav));
        Marshal.StructureToPtr(hav, ptr, false);
        return ptr;
    }

    public static explicit operator HidApiVersion(nint ptr) {
        return Marshal.PtrToStructure<HidApiVersion>(ptr);
    }

    public override string ToString() => $"{Major}.{Minor}.{Patch}";
}
