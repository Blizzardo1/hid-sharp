using System.Runtime.InteropServices;

namespace hid_sharp;

[StructLayout(LayoutKind.Sequential)]
public struct HidDeviceInfo {
    public string? Path;                // char *
    public ushort VendorId;             // unsigned short
    public ushort ProductId;            // unsigned short
    public string? SerialNumber;        // wchar_t *
    public ushort ReleaseNumber;        // unsigned short
    public string? ManufacturerString;  // wchar_t *
    public string? ProductString;       // wchar_t *
    public ushort UsagePage;            // unsigned short
    public ushort Usage;                // unsigned short
    public int InterfaceNumber;         // int

    // Pointer to hid_device_info
    // public HidDeviceInfo Next;       // We need to make an IList<HidDeviceInfo> to enumerate through devices.
    public nint Next;         // Unless otherwise specified, this is a pointer and/or dummy
    public HidBusType BusType;          // hid_bus_type
    public override string ToString() {
        return $"{Path} - {VendorId:X4}:{ProductId:X4} - {ManufacturerString} - {ProductString} ({SerialNumber})";
    }

    public static explicit operator HidDeviceInfo?(nint ptr) {
        return Marshal.PtrToStructure<HidDeviceInfo>(ptr);
    }

    public static explicit operator nint(HidDeviceInfo hdi) {
        nint ptr = new();
        Marshal.StructureToPtr(hdi, ptr, false);
        return ptr;
    }
}
