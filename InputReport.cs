using System.Runtime.InteropServices;

namespace hid_sharp;

[StructLayout(LayoutKind.Sequential)]
public struct InputReport
{
    /// <summary>
    /// Data — uint8_t* data
    /// </summary>
    public nint Data;

    /// <summary>
    /// Data Length — size_t len
    /// </summary>
    public int Length;

    /// <summary>
    /// Next Input Report — input_report* next
    /// </summary>
    public nint Next;


    public static explicit operator InputReport(nint ptr) {
        return Marshal.PtrToStructure<InputReport>(ptr);
    }

    public static explicit operator nint(InputReport report) {
        nint ptr = Marshal.AllocHGlobal(Marshal.SizeOf<InputReport>())!;
        Marshal.StructureToPtr(report, ptr, false);
        return ptr;
    }
}
