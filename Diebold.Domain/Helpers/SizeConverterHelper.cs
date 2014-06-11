using System;

namespace Diebold.Domain.Helpers
{
    public class SizeConverterHelper
    {
        public static string ConvertFormat(Int64 source)
        {
            const int byteConversion = 1024;
            var bytes = Convert.ToDouble(source);

            if (bytes >= Math.Pow(byteConversion, 3))
                return string.Concat(Math.Round(bytes / Math.Pow(byteConversion, 3), 2), " GB");

            if (bytes >= Math.Pow(byteConversion, 2))
                return string.Concat(Math.Round(bytes / Math.Pow(byteConversion, 2), 2), " MB");

            if (bytes >= byteConversion)
                return string.Concat(Math.Round(bytes / byteConversion, 2), " KB");

            return string.Concat(bytes, " Bytes");
        }
    }
}
