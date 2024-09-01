namespace hid_sharp;

public enum HidBusType {
    /// <summary>
    /// Unknown bus type
    /// </summary>
    ApiBusUnknown = 0x00,

    /// <summary>
    /// USB Bus
    /// </summary>
    ApiBusUsb = 0x01,

    /// <summary>
    /// Bluetooth or Bluetooth LE Bus
    /// </summary>
    /// <remarks>
    ///  Specifications:
    ///     https://www.bluetooth.com/specifications/specs/human-interface-device-profile-1-1-1/
    ///     https://www.bluetooth.com/specifications/specs/hid-service-1-0/
    ///     https://www.bluetooth.com/specifications/specs/hid-over-gatt-profile-1-0/ 
    /// </remarks>
    ApiBusBluetooth = 0x02,

    /// <summary>
    /// I2C Bus
    /// </summary>
    /// <remarks>
    /// Specifications:
    ///     https://docs.microsoft.com/previous-versions/windows/hardware/design/dn642101(v=vs.85)
    /// </remarks>
    ApiBusI2C = 0x03,

    /// <summary>
    /// SPI Bus
    /// </summary>
    /// <remarks>
    /// Specifications:
    ///     https://www.microsoft.com/download/details.aspx?id=103325
    /// </remarks>
    ApiBusSpi = 0x04
}