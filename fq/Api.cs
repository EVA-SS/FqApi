namespace fq
{
    public static class Api
    {
        #region 目录配置

        public static string path_clash = "";
        public static string path_fq = "";

        public static void SetPath(string path)
        {
            path_clash = path + "clash.txt";
            path_fq = path + "fq\\";
            if (!Directory.Exists(path_fq)) Directory.CreateDirectory(path_fq);
        }

        #endregion

        #region Json

        public static string? ToJson(this object obj)
        {
            if (obj == null)
            {
                return null;
            }
            return Newtonsoft.Json.JsonConvert.SerializeObject(obj);
        }
        public static T? ToJson<T>(this string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                try
                {
                    return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(value);
                }
                catch
                {
                }
            }
            return default;
        }

        #endregion

        public static int ToInt(this object value, int deVal = 0)
        {
            if (value == null || value is DBNull)
                return deVal;
            else
            {
                if (int.TryParse(value.ToString(), out int _value)) return _value;
                else return deVal;
            }
        }
    }
}
