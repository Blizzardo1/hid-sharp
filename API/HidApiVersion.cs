using System.Runtime.InteropServices;

namespace hid_sharp.API;

public struct HidApiVersion {
    public int Major;
    public int Minor;
    public int Patch;

    // This is removed unless it's absolutely needed. Recall HID.Version().
    // public static explicit operator nint(HidApiVersion hav) {
    //     return ptr;
    // }

    public static explicit operator HidApiVersion(nint ptr) {
        return Marshal.PtrToStructure<HidApiVersion>(ptr);
    }

    public override string ToString() => $"{Major}.{Minor}.{Patch}";
}
