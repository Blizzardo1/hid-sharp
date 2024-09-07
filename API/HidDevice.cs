using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.InteropServices;

namespace hid_sharp.API;

[StructLayout(LayoutKind.Sequential)]
public struct HidDevice {
    /// <summary>
    /// Handle to the actual device — libusb_device_handle* device_handle
    /// </summary>
    public nint DeviceHandle;

    /// <summary>
    /// USB Configuration Number of the device — int config_number
    /// </summary>
    public int ConfigNumber;

    /// <summary>
    /// The interface number of the HID — int interface;
    /// </summary>
    public int Interface;

    /// <summary>
    /// Report Descriptor Size — uint16_t report_descriptor_size
    /// </summary>
    public ushort ReportDescriptorSize;

    /// <summary>
    /// Endpoint Information — int input_endpoint
    /// </summary>    
    public int InputEndpoint;

    /// <summary>
    /// Endpoint Information — int output_endpoint
    /// </summary>
    public int OutputEndpoint;

    /// <summary>
    /// Endpoint Information — int input_ep_max_packet_size
    /// </summary>
    public int InputEpMaxPacketSize;

    /// <summary>
    /// Manufacture Index — int manufacturer_index
    /// </summary>
    public int ManufacturerIndex;

    /// <summary>
    /// Product Index — int product_index
    /// </summary>
    public int ProductIndex;

    /// <summary>
    /// Serial Index — int serial_index
    /// </summary>
    public int SerialIndex;

    /// <summary>
    /// Device Information — hid_device_info* device_info
    /// </summary>
    public nint DeviceInfo;

    /// <summary>
    /// Whether blocking reads are used — int blocking
    /// </summary>
    [MarshalAs(UnmanagedType.I4)]
    public int Blocking;

    /// <summary>
    /// Read Thread Objects — hidapi_thread_state thread_state
    /// </summary>
    public ThreadState ThreadState;

    /// <summary>
    /// Read Thread Objects — int shutdown_thread
    /// </summary>
    public int ShutdownThread;

    /// <summary>
    /// Read Thread Objects — int transfer_loop_finished
    /// </summary>
    public int TransferLoopFinished;

    /// <summary>
    /// Read Thread Objects — libusb_transfer* transfer
    /// </summary>
    public nint Transfer;

    /// <summary>
    /// List of received input reports — input_report* input_reports
    /// </summary>
    public nint InputReports;

    /// <summary>
    /// Is the Kernel driver detached? — int kernel_driver_detached
    /// </summary>
    public int IsDriverDetached;

    public HidDeviceInfo GetDeviceInfo() {
        if (DeviceInfo == nint.Zero) return default;
        return (HidDeviceInfo)Marshal.PtrToStructure(DeviceInfo, typeof(HidDeviceInfo))!;
    }

    public override bool Equals([NotNullWhen(true)] object? obj) => base.Equals(obj);

    public static bool operator ==(HidDevice a, HidDevice b) {
        return a.DeviceHandle == b.DeviceHandle;
    }

    public static bool operator !=(HidDevice a, HidDevice b) {
        return a.DeviceHandle != b.DeviceHandle;
    }

    public static explicit operator HidDevice(nint ptr) => Marshal.PtrToStructure<HidDevice>(ptr)!;

    public static explicit operator nint(HidDevice device) {
        nint ptr = nint.Zero;
        try {
            ptr = HID.ConnectedDevices.FirstOrDefault(x => x.Device == device).DeviceHandle;
        } catch (Exception) {
            // default struct is already initialized, meaning ptr should still be zero.
            // exception is just incase it doesn't work out the way I want to.
        }
        return ptr;
    }
}
