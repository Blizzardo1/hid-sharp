using System.Runtime.InteropServices;

namespace hid_sharp;

[StructLayout(LayoutKind.Sequential)]
public struct HidDeviceInfo {
    /// <summary>
    /// Device Path — char* path
    /// </summary>
    public string Path;

    /// <summary>
    /// Vendor ID — unsigned short vendor_id
    /// </summary>
    public ushort VendorId;

    /// <summary>
    /// Product ID — unsigned short product_id
    /// </summary>
    public ushort ProductId;

    /// <summary>
    /// Serial Number — wchar_t* serial_number
    /// </summary>
    public string SerialNumber;

    /// <summary>
    /// Release Number — unsigned short release_number
    /// </summary>
    public ushort ReleaseNumber;

    /// <summary>
    /// Manufacturer String — wchar_t* manufacturer_string
    /// </summary>
    public string ManufacturerString;

    /// <summary>
    /// Product String — wchar_t* product_string
    /// </summary>
    public string ProductString;

    /// <summary>
    /// Usage Page — unsigned short usage_page
    /// </summary>
    public ushort UsagePage;

    /// <summary>
    /// Usage — unsigned short usage
    /// </summary>
    public ushort Usage;

    /// <summary>
    /// Interface Number — int interface_number
    /// </summary>
    public int InterfaceNumber;

    /// <summary>
    /// Next Device Info — hid_device_info* next
    /// </summary>
    public nint Next;

    /// <summary>
    /// Bus Type — hid_bus_type bus_type
    /// </summary>
    public HidBusType BusType;


    public override string ToString() {
        return $"{Path} - {VendorId:X4}:{ProductId:X4} - {ManufacturerString} - {ProductString} ({SerialNumber})";
    }

    public static explicit operator HidDeviceInfo(nint ptr) {
        return Marshal.PtrToStructure<HidDeviceInfo>(ptr);
    }

    public static explicit operator nint(HidDeviceInfo hdi) {
        nint ptr = Marshal.AllocHGlobal(Marshal.SizeOf<HidDeviceInfo>())!;
        Marshal.StructureToPtr(hdi, ptr, false);
        return ptr;
    }
}
