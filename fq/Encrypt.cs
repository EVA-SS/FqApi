using System.Text;


namespace fq
{
    /// <summary>
    /// 常用的加密解密算法
    /// </summary>
    public static class Security
    {
        #region Base64加密解密

        /// <summary>
        /// Base64加密
        /// </summary>
        /// <param name="str">需要加密的字符串</param>
        /// <returns>加密后的数据</returns>
        public static string EncryptBase64(this string str, bool noNull = true)
        {
            if (str == null) { return null; }
            try
            {
                byte[] encbuff = Encoding.UTF8.GetBytes(str);
                return Convert.ToBase64String(encbuff);
            }
            catch
            {
                return noNull ? str : null;
            }
        }
        /// <summary>
        /// Base64加密
        /// </summary>
        /// <param name="buff">需要加密的字节</param>
        /// <returns>加密后的数据</returns>
        public static byte[] EncryptBase64(this byte[] buff, bool noNull = true)
        {
            if (buff == null) { return null; }
            try
            {
                return Encoding.UTF8.GetBytes(Convert.ToBase64String(buff));
            }
            catch
            {
                return noNull ? buff : null;
            }
        }

        /// <summary>
        /// Base64解密
        /// </summary>
        /// <param name="str">需要解密的字符串</param>
        /// <returns>解密后的数据</returns>
        public static string DecryptBase64(this string str, bool noNull = true)
        {
            if (str == null) { return null; }
            try
            {
                byte[] decbuff = Convert.FromBase64String(str);
                return Encoding.UTF8.GetString(decbuff);
            }
            catch
            {
                return noNull ? str : null;
            }
        }

        /// <summary>
        /// Base64解密
        /// </summary>
        /// <param name="buff">需要解密的字节</param>
        /// <returns>解密后的数据</returns>
        public static byte[] DecryptBase64(this byte[] buff, bool noNull = true)
        {
            if (buff == null) { return null; }
            try
            {
                return Convert.FromBase64String(Encoding.UTF8.GetString(buff));
            }
            catch
            {
                return noNull ? buff : null;
            }
        }

        #endregion
    }
}
