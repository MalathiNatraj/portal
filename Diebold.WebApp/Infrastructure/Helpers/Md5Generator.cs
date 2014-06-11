using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Security.Cryptography;
using System.IO;
using System;
using System.Text;

namespace Diebold.WebApp.Infrastructure.Helpers
{
    public class Md5Generator
    {
        public static Dictionary<String, String> md5Array = new Dictionary<String, String>();
        public static String path;
        
        public static Dictionary<String, String> getMd5Array()
        {
            return md5Array;
        }

        public static void generateMD5(String filePath) {
            using (var md5 = MD5.Create())
            {
                using (var stream = File.OpenRead(path + filePath))
                {
                    var byteData = md5.ComputeHash(stream);

                    md5Array.Add(filePath, BitConverter.ToString(byteData).Replace("-", ""));
                }
            }
        }

        public static String getMD5Code(String filePath)
        {
            if (md5Array.ContainsKey(filePath))
            {
                return md5Array[filePath];
            }
            else
            {
                generateMD5(filePath);
                return md5Array[filePath];
            }
        }
    }
}