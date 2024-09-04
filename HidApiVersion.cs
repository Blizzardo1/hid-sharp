﻿namespace hid_sharp;

public struct HidApiVersion {
    public int Major;
    public int Minor;
    public int Patch;

    public override string ToString() => $"{Major}.{Minor}.{Patch}";
}
