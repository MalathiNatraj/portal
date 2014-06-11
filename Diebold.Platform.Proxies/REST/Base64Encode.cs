using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Diebold.Platform.Proxies.REST
{
    class Base64Encode
    {
        /// <summary>
        /// Method can be use to encode the given decoded string value
        /// </summary>
        /// <param name="strDecodeValue"></param>
        /// <returns></returns>
        public static String Encode64(String StringToEncode)
        {
            byte[] bytDecodes = System.Text.ASCIIEncoding.ASCII.GetBytes(StringToEncode);
            string strEncode = Convert.ToBase64String(bytDecodes);
            return strEncode;
        }

        /// <summary>
        /// Method can be use to decode the given string
        /// </summary>
        /// <param name="strEncodeValue"></param>
        /// <returns></returns>
        public static string Decode64(string strEncodeValue)
        {
            byte[] bytEncodes = Convert.FromBase64String(strEncodeValue);
            string strDecode = System.Text.ASCIIEncoding.ASCII.GetString(bytEncodes);
            return strDecode;
        }
    }
}
