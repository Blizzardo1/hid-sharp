using System.Runtime.InteropServices;
using NLog;

namespace hid_sharp;

public static class HID {
    // Remember that HID_API_EXPORT is __declspec(dllexport) in Windows

    public const int Major = 0;
    public const int Minor = 15;
    public const int Patch = 0;

    // Eventually we will detect an OS to load the correct binary
    private const string Dll = "hidapi.dll";
    private const CallingConvention convention = CallingConvention.Cdecl;

    private static ILogger _log;

    static HID()
    {
        _log = LogManager.GetLogger("HID");
    }

    public static int MakeVersion(int major, int minor, int patch) => ((major) << 24) | ((minor) << 8) | (patch);

    [DllImport(Dll, CallingConvention = convention, EntryPoint = "hid_init")]
    private static extern int hid_init();
    
    [DllImport(Dll, CallingConvention = convention, EntryPoint = "hid_exit")]
    private static extern int hid_exit();

    [DllImport(Dll, CallingConvention = convention, EntryPoint = "hid_enumerate")]
    private static extern nint hid_enumerate(ushort vendorId, ushort productId);

    [DllImport(Dll, CallingConvention = convention, EntryPoint = "hid_free_enumeration")]
    private static extern void hid_free_enumeration(nint devices);

    [DllImport(Dll, CallingConvention = convention, EntryPoint = "hid_open")]
    private static extern nint hid_open(ushort vendorId, ushort productId, string serial_number = "");

    [DllImport(Dll, CallingConvention = convention, EntryPoint ="hid_open_path")]
    private static extern nint hid_open_path(string path);

    [DllImport(Dll, CallingConvention = convention, EntryPoint = "hid_write")]
    private static extern int hid_write(ref HidDevice dev, byte[] data, int length);

    [DllImport(Dll, CallingConvention = convention, EntryPoint = "hid_read_timeout")]
    private static extern int hid_read_timeout(ref HidDevice dev, [MarshalAs(UnmanagedType.LPArray)] byte[] data, int length, int milliseconds);

    [DllImport(Dll, CallingConvention = convention, EntryPoint = "hid_read")]
    private static extern int hid_read(ref HidDevice dev,[MarshalAs(UnmanagedType.LPArray)] byte[] data, int length);

    [DllImport(Dll, CallingConvention = convention, EntryPoint = "hid_set_nonblocking")]
    private static extern int hid_set_nonblocking(ref HidDevice dev, int nonblock);

    [DllImport(Dll, CallingConvention = convention, EntryPoint = "hid_send_feature")]
    private static extern int hid_send_feature_report(ref HidDevice dev, [MarshalAs(UnmanagedType.LPArray)] byte[] data, int length);

    [DllImport(Dll, CallingConvention = convention, EntryPoint = "hid_get_feature")]
    private static extern int hid_get_feature_report(ref HidDevice dev, [MarshalAs(UnmanagedType.LPArray)] byte[] data, int length);

    [DllImport(Dll, CallingConvention = convention, EntryPoint = "hid_send_output_report")]
    private static extern int hid_send_output_report(ref HidDevice dev, [MarshalAs(UnmanagedType.LPArray)] byte[] data, int length);

    [DllImport(Dll, CallingConvention = convention, EntryPoint = "hid_get_input_report")]
    private static extern int hid_get_input_report(ref HidDevice dev, [MarshalAs(UnmanagedType.LPArray)] byte[] data, int length);

    [DllImport(Dll, CallingConvention = convention, EntryPoint = "hid_close")]
    private static extern void hid_close(ref HidDevice dev);

    [DllImport(Dll, CallingConvention = convention, EntryPoint = "hid_get_manufacturer_string")]
    private static extern int hid_get_manufacturer_string(ref HidDevice dev, string @string, int maxlen);

    [DllImport(Dll, CallingConvention = convention, EntryPoint = "hid_get_product_string")]
    private static extern int hid_get_product_string(ref HidDevice dev, string @string, int maxlen);

    [DllImport(Dll, CallingConvention = convention, EntryPoint = "hid_get_serial_number")]
    private static extern int hid_get_serial_number_string(ref HidDevice dev, string @string, int maxlen);

    [DllImport(Dll, CallingConvention = convention, EntryPoint = "hid_get_device_info")]
    private static extern nint hid_get_device_info(ref HidDevice dev);

    [DllImport(Dll, CallingConvention = convention, EntryPoint = "hid_get_indexed_string")]
    private static extern int hid_get_indexed_string(ref HidDevice dev, int string_index, string @string, int maxlen);

    [DllImport(Dll, CallingConvention = convention, EntryPoint = "hid_get_report_descriptor")]
    private static extern int hid_get_report_descriptor(ref HidDevice dev, [MarshalAs(UnmanagedType.LPArray)] byte[] buf, int buf_size);

    [DllImport(Dll, CallingConvention = convention, EntryPoint = "hid_error")]
    private static extern string hid_error(ref HidDevice dev);

    [DllImport(Dll, CallingConvention = convention, EntryPoint = "hid_version")]
    private static extern nint hid_version();

    [DllImport(Dll, CallingConvention = convention, EntryPoint = "hid_version_str")]
    private static extern string hid_version_str();

    /// <summary>
    /// Initialize the HIDAPI library.
    /// 
    /// 
    /// This function initializes the HIDAPI library. Calling it is not
    /// strictly necessary, as it will be called automatically by
    /// hid_enumerate() and any of the hid_open_*() functions if it is
    /// needed.  This function should be called at the beginning of
    /// execution however, if there is a chance of HIDAPI handles
    /// being opened by different threads simultaneously.
    /// </summary>
    /// <returns>
    ///     This function returns true on success and false on error.
    ///     Call <see cref="Error(ref HidDevice?)"/> to get the failure reason.
    /// </returns>
    public static bool Initialize()
    {
        bool init = hid_init() == 0;

        if (init) _log.Info("HIDAPI initialized successfully");
        else _log.Error("HIDAPI failed to initialize");
        
        return init;
    }



    /// <summary>
    /// Finalize the HIDAPI library.
    /// This function frees all of the static data associated with
    /// HIDAPI.It should be called at the end of execution to avoid
    /// memory leaks.
    /// </summary>
    /// <returns>
    ///     This function returns true on success and false on error.
    /// </returns>
    public static bool Exit()
    {
        bool exit = hid_exit() == 0;
        if (exit) _log.Info("HIDAPI finalized successfully");
        else _log.Error("HIDAPI failed to finalize");
        return exit;
    }

    /// <summary>
    ///     Enumerate the HID Devices.
    ///     This function returns a linked list of all the HID devices
    ///     attached to the system which match <paramref name="vendorId"/> and <paramref name="productId" />.
    ///     If <paramref name="vendorId"/> is set to 0 then any vendor matches.
    ///     If <paramref name="productId"/> is set to 0 then any product matches.
    ///     If <paramref name="vendorId"/> and <paramref name="productId"/> are both set to 0, then
    ///     all HID devices will be returned
    /// </summary>
    /// <remarks>
    ///     The returned value by this function must be freed by calling <see cref="FreeEnumeration(List{HidDeviceInfo})" />,
    ///     when not needed anymore.
    ///     <para>This method is intentionally private as it's being handled by <see cref="hid_free_enumeration(nint)"/></para>
    /// </remarks>
    /// <param name="vendorId">The Vendor ID (VID) of the types of device to open.</param>
    /// <param name="productId">The Product ID (PID) of the types of device to open.</param>
    /// <returns>
    ///     This function returns a pointer to a linked list of type
    ///     <see cref="HidDeviceInfo"/>, containing information about the HID devices
    ///     attached to the system,
    ///     or NULL in the case of failure or if no HID devices present in the system.
    ///     Call <see cref="Error(ref HidDevice?)"/> to get the failure reason.
    /// </returns>
    public static List<HidDeviceInfo>? Enumerate(ushort vendorId = 0, ushort productId = 0)
    {
        List<HidDeviceInfo> devices = [];
        _log.Info("Initialized devices list");
        nint ptr = hid_enumerate(vendorId, productId);
        _log.Trace($"Device PTR: {ptr:X8}");
        nint currPtr = ptr;
        
        while(currPtr != nint.Zero)
        {
            HidDeviceInfo hdi = Marshal.PtrToStructure<HidDeviceInfo>(currPtr);
            devices.Add(hdi);
            _log.Trace($"Device: {hdi} added");
            currPtr = hdi.Next;
        }

        hid_free_enumeration(ptr);
        _log.Info("Enumeration freed PTR");
        return devices;
    }

    /// <summary>
    /// Free an enumeration Linked List
    /// </summary>
    /// <remarks>
    /// This function frees a linked list created by <see cref="Enumerate(ushort, ushort)"/>
    /// </remarks>
    /// <param name="devices">Pointer to a list of struct_device returned from <see cref="Enumerate(ushort, ushort)"/></param>
    private static void FreeEnumeration(List<HidDeviceInfo> devices)
    {
        // We are doing this in the wrapped function. Ignore this, mmk?
    }

    /// <summary>
    /// Open a HID device using a Vendor ID (VID), Product ID (PID) and optionally a serial number.
    /// </summary>
    /// <remarks>
    ///     If <paramref name="serialNumber"/> is NULL, the first device with the
    ///     specified VID and PID is opened.
    ///     The returned object must be freed by calling <see cref="Close(ref HidDevice)"/>
    ///     when not needed anymore.
    /// </remarks>
    /// <param name="vendorId">The Vendor ID (VID) of the device to open.</param>
    /// <param name="productId">The Product ID (PID) of the device to open.</param>
    /// <param name="serialNumber">The Serial Number of the device to open (Optionally NULL)</param>
    /// <returns>
    ///     This function returns a pointer to an <see cref="HidDevice"/> object on
    ///		success or NULL on failure.
    ///     Call <see cref="Error(ref HidDevice?)"/> to get the failure reason.
    /// </returns>
    public static HidDevice? Open(ushort vendorId, ushort productId, [MarshalAs(UnmanagedType.BStr)] string serialNumber = "") {
        
        nint ptr = hid_open(vendorId, productId, serialNumber);
        _log.Info($"Device Opened by (VendorId,ProductId,SerialNumber)({vendorId}, {productId}, {serialNumber}) with PTR {ptr:X8}");

        if(ptr == nint.Zero)
        {
            return null;
        }

        try
        {
            HidDevice? dev = Marshal.PtrToStructure<HidDevice>(ptr);
            if (dev is null) throw new ContextMarshalException("Unable to open Device");
            return (HidDevice)dev;
        }
        catch (Exception ex)
        {
            _log.Fatal($"Failed to translate PTR {ptr:X8} to Struct {typeof(HidDevice).FullName}\nException: {ex}");
            return null;
        }
        
    }


    /// <summary>
    /// Open a HID device by its path name.
    /// </summary>
    /// <remarks>
    ///	    The path name be determined by calling <see cref="Enumerate(ushort, ushort)"/>, or a
    ///	    platform-specific path name can be used (eg: /dev/hidraw0 on Linux).
    ///	    
    ///     The returned object must be freed by calling hid_close(),
    ///	    when not needed anymore.
    ///	</remarks>
    ///	
    ///	<param name="path"> The path name of the device to open </param>
    ///	<returns>
    ///		This function returns a pointer to a #hid_device object on
    ///		success or NULL on failure.
    ///		Call <see cref="Error(ref HidDevice?)"/> to get the failure reason.
    /// </returns>
    public static HidDevice? OpenPath(string path)
    {
        nint ptr = hid_open_path(path);
        _log.Info($"Device Opened by Path ({path}) with PTR {ptr:X8}");

        if (ptr == nint.Zero)
        {
            return null;
        }

        try
        {
            HidDevice? dev = Marshal.PtrToStructure<HidDevice>(ptr);
            if (dev is null) throw new ContextMarshalException("Unable to open Device");
            return (HidDevice)dev;
        }
        catch (Exception ex)
        {
            _log.Fatal($"Failed to translate PTR {ptr:X8} to Struct {typeof(HidDevice).FullName}\nException: {ex}");
            return null;
        }
    }


    /// <summary>
    /// Write an Output report to a HID device.
    /// </summary>
    /// <remarks>
    ///     The first byte of <paramref name="data"/> must contain the Report ID.
    ///     For devices which only support a single report, this must be set
    ///     to 0x0. The remaining bytes contain the report data.
    ///     Since the Report ID is mandatory, calls to <see cref="Write(ref HidDevice, byte[], int)"/> will always 
    ///     contain one more byte than the report contains.For example,
    ///     if hid report is 16 bytes long, 17 bytes must be passed to
    ///     <see cref="Write(ref HidDevice, byte[], int)"/>, the Report ID(or 0x0, for devices with a
    ///     single report), followed by the report data(16 bytes). In
    ///     this example, the length passed in would be 17.
    ///     <see cref="Write(ref HidDevice, byte[], int)"/> will send the data on the first interrupt 
    ///     endpoint, if one exists. If it does not the behavior is as 
    /// </remarks>
    /// <seealso cref="SendOutputReport(ref HidDevice, byte[], int)"/>
    /// <param name="dev"> A device handle returned from <see cref="Open(ushort, ushort, string)"/></param>
    /// <param name="data"> The data to send, including the report number as the first byte. </param>
    /// <param name="length"> The length in bytes of the data to send.</param>
    /// 
    /// 
    /// <returns>
    ///     This function returns the actual number of bytes written and on error.
    ///     Call <see cref="Error(ref HidDevice)"/> to get the failure reason.
    /// </returns>
    public static int Write(ref HidDevice dev, byte[] data, int length)
    {
        _log.Info($"Writing {data.Length} bytes to PTR {dev.DeviceHandle:X8}");
        int result = hid_write(ref dev, data, length);
        _log.Info($"Wrote {result} bytes");
        return result;
    }

    /// <summary>
    /// Read an Input report from a HID device with timeout.
    /// </summary>
    /// 
    /// <remarks>
    ///     Input reports are to the host through the INTERRUPT IN endpoint.
    ///     The first byte will contain the Report number if the device uses numbered reports.
    /// </remarks>
    /// <param name="dev"> A device handle returned from <see cref="OpenPath(string)"/>.</param>
    /// <param name="data"> A buffer to put the read data into. </param>
    /// <param name="length"> The number of bytes to read. For devices with multiple reports,
    ///     make sure to read an extra byte for the report number.</param>
    /// <param name="milliseconds"> The timeout in milliseconds or -1 for blocking wait.</param>
    ///
    ///
    /// <returns>
    ///     This function returns the actual number of bytes read and -1 on error.
    ///     Call <see cref="Error(ref HidDevice)"/> to get the failure reason.
    ///     If no packet was available to be read within
    ///     the timeout period, this function returns 0.
    ///	</returns>
    public static byte[] ReadTimeout(ref HidDevice dev, int length, int milliseconds)
    {
        _log.Info($"Waiting {milliseconds}ms to read data with length {length}");
        byte[] data = new byte[length];
        int result = hid_read_timeout(ref dev, data, length, milliseconds);
        _log.Info($"Read {result} bytes");
        return data;
    }

    /// <summary>
    /// Read an Input report from a HID device.
    /// </summary>
    /// <remarks>
    /// 	Input reports are returned to the host through the INTERRUPT IN endpoint.
    /// 	The first byte will	contain the Report number if the device uses numbered reports.
    /// </remarks>
    /// <param name="dev">A device handle returned from <see cref="Open(ushort, ushort, string)"/>.</param>
    /// <param name="data"> A buffer to put the read data into.</param>
    /// <param name="length"> The number of bytes to read. For devices with
    /// 		multiple reports, make sure to read an extra byte for
    /// 		the report number. </param>
    /// <returns>
    ///     This function returns the actual number of bytes read and -1 on error.
    ///     Call <see cref="Error(ref HidDevice)"/> to get the failure reason.
    ///     If no packet was available to be read and
    ///     the handle is in non-blocking mode, this function returns 0.
    /// </returns>
    public static byte[] Read(ref HidDevice dev, byte[] data, int length)
    {
        // Converting to a returnable byte sequence;
        return hid_read(ref dev, data, length);
    }

    /// <summary>
    /// Set the device handle to be non-blocking.
    /// </summary>
    /// <remarks>
    ///     In non-blocking mode calls to <see cref="Read(ref HidDevice, byte[], int)"/> will return
    ///     immediately with a value of 0 if there is no data to be
    ///     read. In blocking mode, <see cref="Read(ref HidDevice, byte[], int)"/> will wait(block) until
    ///     there is data to read before returning.
    ///     Nonblocking can be turned on and off at any time.
    /// </remarks>
    /// <param name="dev">A device handle returned from <see cref="Open(ushort, ushort, string)"/>.</param>
    /// <param name="nonBlock"> Enable or not the nonblocking reads <see cref="true"/> to enable nonblocking, <see cref="false"/> to disable nonblocking.</param>
    /// <returns>
    ///     This function returns 0 on success and -1 on error.
    ///     Call <see cref="Error(HidDevice)"/> to get the failure reason.
    /// </returns>
    public static int SetNonBlocking(ref HidDevice dev, bool nonBlock) =>
        // Translate the boolean into an int for the hid_set_nonblocking function.
        hid_set_nonblocking(ref dev, nonBlock ? 1 : 0);

    /// <summary>
    /// Send a Feature report to the device.
    /// </summary>
    /// <remarks>
    ///     Feature reports are sent over the Control endpoint as a
    ///     Set_Report transfer. The first byte of <paramref name="data"/> must
    ///     contain the Report ID. For devices which only support 
    ///     single report, this must be set to 0x0. The remaining bytes
    ///     contain the report data.
    ///     <para> Since the Report ID is mandatory, calls to <see cref="SendFeatureReport(ref HidDevice, byte[], int)"/> will always contain 
    ///     one more byte than the report contains.
    ///     </para>
    ///     <para>For example, if a report is 16 bytes long, 17 bytes must be passed to
    ///     <see cref="SendFeatureReport(ref HidDevice, byte[], int)"/>: the Report ID(or 0x0, devices which do not use numbered reports),
    ///     followed by the report data(16 bytes). In this example, the length in would be 17.
    ///     </para>
    /// </remarks>
    /// <param name="dev"> A device handle returned from <see cref="Open(ushort, ushort, string)"/>.</param>
    /// <param name="data"> The data to send, including the report number as the first byte.</param>
    /// <param name="length"> The length in bytes of the data to send, including the report number.</param>
    /// <returns>
    ///     This function returns the actual number of bytes written and -1 on error
    ///     Call <see cref="Error(ref HidDevice)"/> to get the failure reason.
    /// </returns>
    public static int SendFeatureReport(ref HidDevice dev, byte[] data, int length) => hid_send_feature_report(ref dev, data, length);

    /// <summary>
    /// Get a feature report from a HID device.
    /// </summary>
    /// <remarks>
    ///     Set the first byte of <paramref name="data"/> to the Report ID of the report to be read.
    ///     Make sure to allow space for this extra byte in <paramref name="data"/>.
    ///     Upon return, the first byte will still contain the Report ID,
    ///     and the report data will start in <paramref name="data"/>[1].
    /// </remarks>
    /// <param name="dev"> A device handle returned from <see cref="Open(ushort, ushort, string)"/>. </param>
    /// <param name="data">
    ///     A buffer to put the read data into, including the Report ID.
    ///     Set the first byte of <paramref name="data"/> to the Report ID of the report to be read,
    ///     or set it to zero if your device does not use numbered reports.
    /// </param> 
    /// <param name="length">
    ///     The number of bytes to read, including an extra byte for the report ID.
    ///     The buffer can be longer than the actual report.
    /// </param>
    /// <returns>
    ///     This function returns the number of bytes read plus
    ///     one for the report ID (which is still in the first byte), or -1 on error.
    ///     Call <see cref="Error(HidDevice)"/> to get the failure reason.
    /// </returns>
    public static int GetFeatureReport(ref HidDevice dev, byte[] data, int length) => hid_get_feature_report(ref dev, data, length);

    /// <summary>
    /// Send a Output report to the device.
    /// </summary>
    /// <remarks>
    ///     <para>Since version 0.15.0, <see cref="HidApiVersion"/> >= HID_API_MAKE_VERSION(0, 15, 0)
    ///     Output reports are sent over the Control endpoint as a Set_Report transfer.
    ///     The first byte of <paramref name="data"/> must contain the Report ID.
    ///     For devices which only support single report, this must be set to 0x0.
    ///     The remaining bytes contain the report data.
    ///     </para>
    ///     <para>
    ///     Since the Report ID is mandatory, calls to <see cref="SendOutputReport(ref HidDevice, byte[], int)"/> will always contain one
    ///     more byte than the report contains.
    ///     For example, if a hid report is 16 bytes long, 17 bytes must be passed to
    ///     <see cref="SendOutputReport(ref HidDevice)"/>: the Report ID(or 0x0, for devices which do not use numbered reports),
    ///     followed by the report data(16 bytes).</para>
    ///     In this example, the length passed in would be 17.
    ///     This function sets the return value of <see cref="Error(ref HidDevice)"/>.
    ///     See <see cref="Write(ref HidDevice, byte[], int)"/>
    /// </remarks>
    /// <param name="dev">A device handle returned from <see cref="Open(ushort, ushort, string)"/>.</param>
    /// <param name="data">The data to send, including the report number as the first byte.</param>
    /// <param name="length">The length in bytes of the data to send, including the report number.</param>
    /// <returns>
    ///     This function returns the actual number of bytes written and -1 on error.
    /// </returns>
    public static int SendOutputReport(ref HidDevice dev, byte[] data, int length) => hid_send_output_report(ref dev, data, length);

    /// <summary>
    /// Get a input report from a HID device.
    /// </summary>
    /// <remarks>
    /// 	Since version 0.10.0, <see cref="HidApiVersion"/> >= HID_API_MAKE_VERSION(0, 10, 0)
    ///     Set the first byte of <paramref name="data"/> to the Report ID of the
    ///     report to be read. Make sure to allow space for this extra byte in <paramref name="data"/>.
    ///     Upon return, the first byte will still contain the Report ID,
    ///     and the report data will start in <paramref name="data"/>[1].
    /// </remarks>
    ///     <param name="dev">A device handle returned from <see cref="Open(ushort, ushort, string)"/>.</param>
    ///     <param name="data"> A buffer to put the read data into, including the Report ID.
    ///     Set the first byte of <paramref name="data"/> to the Report ID of the report to be read,
    ///     or set it to zero if your device does not use numbered reports.
    /// 	</param>
    ///     <param name="length"> The number of bytes to read, including an extra byte for the report ID.
    ///     The buffer can be longer than the actual report.
    ///     </param>
    /// 
    /// <returns>
    ///     This function returns the number of bytes read plus one
    ///     for the report ID (which is still in the first byte), or -1 on error.
    ///     Call <see cref="Error(ref HidDevice)"/> to get the failure reason.
    /// </returns>

    public static int GetInputReport(ref HidDevice dev, byte[] data, int length) => hid_get_input_report(ref dev, data, length);

    /// <summary>
    /// Close a HID device.
    /// </summary>
    /// <param name="dev"> A device handle returned from <see cref="Open(ushort, ushort, string)"/></param>
    public static void Close(ref HidDevice dev) => hid_close(ref dev);

    /// <summary>
    /// Get The Manufacturer String from a HID device.
    /// </summary>
    /// <param name="dev"> A device handle returned from <see cref="Open(ushort, ushort, string)"/>.</param>
    /// <param name="string"> A wide string buffer to put the data into.</param>
    /// <param name="maxlen"> The length of the buffer in multiples of wchar_t.</param>
    /// <returns>
    ///     This function returns true on success and false on error.
    ///     Call <see cref="Error(ref HidDevice)"/> to get the failure reason.
    /// </returns>
    public static bool GetManufacturerString(ref HidDevice dev, string @string, int maxlen) => hid_get_manufacturer_string(ref dev, @string, maxlen) == 0;

    /// <summary>
    /// Get The Product String from a HID device.
    /// </summary>
    /// <param name="dev"> A device handle returned from <see cref="Open(ushort, ushort, string)"/>.</param>
    /// <param name="string"> A wide string buffer to put the data into.</param>
    /// <param name="maxlen"> The length of the buffer in multiples of wchar_t.</param>
    /// <returns>
    ///     This function returns true on success and false on error.
    ///     Call <see cref="Error(ref HidDevice)"/> to get the failure reason.
    /// </returns>

    public static bool GetProductString(ref HidDevice dev, string @string, int maxlen) => hid_get_product_string(ref dev, @string, maxlen) == 0;

    /// <summary>
    /// Get The Serial Number String from a HID device.
    /// </summary>
    /// <param name="dev">A device handle returned from <see cref="Open(ushort, ushort, string)"/>.</param>
    ///	<param name="string"> A wide string buffer to put the data into.</param>
    /// <param name="maxlen"> The length of the buffer in multiples of wchar_t.</param>
    /// <returns>
    ///     This function returns true on success and false on error.
    ///     Call <see cref="Error(ref HidDevice)"/> to get the failure reason.
    ///	</returns>

    public static bool GetSerialNumberString(ref HidDevice dev, string @string, int maxlen) => hid_get_serial_number_string(ref dev, @string, maxlen) == 0;

    /// <summary>
    /// Get The <see cref="HidDeviceInfo"/> from a HID device.
    /// </summary>
    /// <remarks>
    ///     Since version 0.13.0, <see cref="HidApiVersion"/> >= HID_API_MAKE_VERSION(0, 13, 0)
    ///     The returned object is owned by the <paramref name="dev"/>, and SHOULD NOT be freed by the user.
    /// </remarks>
    /// <param name="dev">A device handle returned from <see cref="Open(ushort, ushort, string)"/>.</param>
    /// 
    /// <returns>
    ///     This function returns <see cref="HidDeviceInfo"/>
    /// 	for this <paramref name="dev" />, or NULL in the case of failure.
    ///     Call <see cref="Error(ref HidDevice)"/> to get the failure reason.
    ///     This struct is valid until the device is closed with <see cref="Close(ref HidDevice)"/>.
    /// </returns>
    public static HidDeviceInfo? GetDeviceInfo(ref HidDevice dev) => hid_get_device_info(ref dev);

    /// <summary>
    /// Get a string from a HID device, based on its string index.
    /// </summary>
    /// <param name="dev">A device handle returned from <see cref="Open(ushort, ushort, string)"/>.</param>
    /// <param name="string_index">The index of the string to get.</param>
    /// <param name="string">A wide string buffer to put the data into.</param>
    /// <param name="maxlen">The length of the buffer in multiples of wchar_t.</param>
    /// 
    /// <returns>
    ///     This function returns true on success and false on error.
    ///     Call <see cref="Error(ref HidDevice)"/> to get the failure reason.
    /// </returns>
    public static bool GetIndexedString(ref HidDevice dev, int string_index, string @string, int maxlen) => hid_get_indexed_string(ref dev, string_index, @string, maxlen) == 0;

    /// <summary>
    /// Get a report descriptor from a HID device.
    /// </summary>
    /// <remarks>
    ///     Since version 0.14.0, <see cref="HidApiVersion"/> >= HID_API_MAKE_VERSION(0, 14, 0)
    ///     User has to provide a preallocated buffer where descriptor will be copied to.
    ///     The recommended size for preallocated buffer is @ref HID_API_MAX_REPORT_DESCRIPTOR_SIZE bytes.
    /// </remarks>
    /// <param name="dev">A device handle returned from <see cref="Open(ushort, ushort, string)"/>.</param>
    /// <param name="buf">The buffer to copy descriptor into.</param>
    /// <param name="bufSize">The size of the buffer in bytes.</param>
    /// <returns>
    ///     This function returns non-negative number of bytes actually copied, or -1 on error.
    /// </returns>

    public static int GetReportDescriptor(ref HidDevice dev, byte[] buf, int bufSize) => hid_get_report_descriptor(ref dev, buf, bufSize);

    /// <summary>
    /// Get a string describing the last error which occurred.
    /// </summary>
    /// <remarks>
    ///     This function is intended for logging/debugging purposes.
    ///     This function guarantees to never return NULL.
    ///     If there was no error in the last function call — the returned string clearly indicates that.
    ///     Any HIDAPI function that can explicitly indicate an execution
    ///     failure (e.g.by an error code, or by returning NULL) - may set the error string, to be returned by this function.
    ///     Strings returned from <see cref="Error(ref HidDevice)"/> must not be freed by the user, i.e. owned by HIDAPI library.
    ///     Device-specific error string may remain allocated at most until <see cref="Close(ref HidDevice)"/> is called.
    ///     Global error string may remain allocated at most until <see cref="Exit"/> is called.
    /// </remarks>
    /// <param name="dev"> A device handle returned from <see cref="Open(ushort, ushort, string)"/>,
    ///     or NULL to get the last non-device-specific error (e.g. for errors in <see cref="Open(ushort, ushort, string)"/> or <see cref="Enumerate(ushort, ushort)"/>).
    ///     </param>
    /// <returns>
    ///     A string describing the last error(if any).
    /// </returns>

    public static string Error(ref HidDevice dev) => hid_error(ref dev);

    /// <summary>
    /// Get a runtime version of the library.
    /// </summary>
    /// <remarks>
    /// This function is thread-safe.
    /// </remarks>
    /// <returns>
    ///     Pointer to statically allocated struct, that contains version.
    /// </returns>

    public static HidApiVersion Version() => hid_version();

    /// <summary>
    /// Get a runtime version string of the library.
    /// </summary>
    /// <remarks>
    ///     This function is thread-safe
    /// </remarks
    /// <returns>
    ///     Pointer to statically allocated string, that contains version string.
    /// </returns>

    public static string VersionStr() => hid_version_str();
}
