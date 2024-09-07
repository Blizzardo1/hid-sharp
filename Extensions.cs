using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hid_sharp.API;
internal static class Extensions {
    public static bool IsEmpty(this string str) => string.IsNullOrEmpty(str) || string.IsNullOrWhiteSpace(str);

    public static string ToHex(this byte[] array) => BitConverter.ToString(array).Replace("-", ",");
}