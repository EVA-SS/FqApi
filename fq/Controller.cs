using Microsoft.AspNetCore.Mvc;

namespace fq
{
    [Produces("text/plain")]
    [Route("[controller]/[action]")]
    [ApiController]
    public class Controller : ControllerBase
    {
        #region GET: fq

        [HttpGet]
        public string? fq()
        {
            return Data();
        }

        static string? Data()
        {
            //数据来源
            //https://github.com/freefq/free
            if (System.IO.File.Exists(Api.path_fq))
            {
                var t = System.IO.File.GetCreationTime(Api.path_fq);
                var elapsedTicks = DateTime.Now.Ticks - t.Ticks;
                var elapsedSpan = new TimeSpan(elapsedTicks);
                if (elapsedSpan.TotalHours > 12)
                {
                    var data = HttpLib.Http.Get("https://raw.kgithub.com/freefq/free/master/v2").request().Result.Data;
                    if (data != null) { System.IO.File.WriteAllText(Api.path_fq, data); return data; }
                }
                return System.IO.File.ReadAllText(Api.path_fq);
            }
            else
            {
                var data = HttpLib.Http.Get("https://raw.kgithub.com/freefq/free/master/v2").request().Result.Data;
                if (data != null) { System.IO.File.WriteAllText(Api.path_fq, data); return data; }
            }
            return null;
        }

        #endregion

        #region GET: sub

        [HttpGet]
        public object sub()
        {
            var fq_data = Data();
            if (fq_data != null)
            {
                var temp = System.IO.File.ReadAllLines(Api.path_clash).ToList();
                var old = fq_data.DecryptBase64();
                List<string> proxies = new List<string>(), ids = new List<string>();
                var cf_id = new List<string>();
                foreach (var item in old.Split('\n'))
                {
                    if (!string.IsNullOrEmpty(item))
                    {
                        int index = item.IndexOf("://");
                        if (index > -1)
                        {
                            string agree = item.Substring(0, index), config_str = item.Substring(index + 3).DecryptBase64();
                            if (agree == "vmess")
                            {
                                var config = config_str.ToJson<Vmess>();
                                if (config != null && config.port != 0)
                                {
                                    var it = new Proxies(ref cf_id, config);
                                    proxies.Add("  - " + it.ToString());
                                    ids.Add("      - " + it.ID());
                                }
                            }
                            else if (agree == "trojan")
                            {
                                var arr = config_str.Split('@', '#');
                                var name = Uri.UnescapeDataString(arr[2]);
                                var it = new Proxies(ref cf_id, agree, name, arr[1], arr[0]);
                                proxies.Add("  - " + it.ToString());
                                ids.Add("      - " + it.ID());
                            }
                            else if (agree == "ss")
                            {
                                var arr = config_str.Split('@', '#');
                                var config_pass_str = arr[0].DecryptBase64().Split(':');
                                var it = new Proxies(ref cf_id, agree, Uri.UnescapeDataString(arr[2]), arr[1], config_pass_str[0], config_pass_str[1]);
                                proxies.Add("  - " + it.ToString());
                                ids.Add("      - " + it.ID());
                            }
                        }
                    }
                }
                int rindex = temp.IndexOf("{{proxies}}");
                temp.RemoveAt(rindex);
                temp.InsertRange(rindex, proxies);

                int dindex = temp.IndexOf("{{ids}}");
                while (dindex > -1)
                {
                    temp.RemoveAt(dindex);
                    temp.InsertRange(dindex, ids);
                    dindex = temp.IndexOf("{{ids}}");
                }
                return string.Join('\n', temp);
            }
            else { return NotFound("下载资源失败"); }
        }

        public static string ClearID(string key)
        {
            var _key = key.Replace("github.com/freefq - ", "").Trim();
            if (_key.Contains(" "))
            {
                var i = _key.LastIndexOf(" ");
                if (_key.Substring(i).Trim().ToInt(-1) > -1)
                    return _key.Substring(0, i).Trim();
            }
            return _key;
        }
        public static string? GetID(string key)
        {
            //http://www.fhdq.net/emoji/14.html
            if (key.Contains("香港"))
                return "🇭🇰";
            else if (key.Contains("澳门"))
                return "🇲🇴";
            else if (key.Contains("荷兰"))
                return "🇳🇱";
            else if (key.Contains("英国"))
                return "🇬🇧";
            else if (key.Contains("法国"))
                return "🇫🇷";
            else if (key.Contains("美国"))
                return "🇺🇸";
            else if (key.Contains("俄罗斯"))
                return "🇷🇺";
            else if (key.Contains("澳大利亚"))
                return "🇦🇺";
            else if (key.Contains("加拿大"))
                return "🇨🇦";
            else if (key.Contains("中国"))
                return "🇨🇳";
            else if (key.Contains("印度"))
                return "🇮🇳";
            else if (key.Contains("日本"))
                return "🇯🇵";
            else if (key.Contains("韩国"))
                return "🇰🇷";
            else if (key.Contains("新加坡"))
                return "🇸🇬";
            else if (key.Contains("泰国"))
                return "🇹🇭";
            else if (key.Contains("越南"))
                return "🇻🇳";
            else if (key.Contains("欧盟"))
                return "🇪🇺";
            else if (key.Contains("摩尔多瓦"))
                return "🇲🇩";
            return null;
        }

        public class Proxies
        {
            public Proxies(ref List<string> cf, Vmess data)
            {
                type = "vmess";
                name = ClearID(data.ps);
                id = GetID(name);
                if (cf.Contains(name))
                {
                    int num = 1;
                    string name_temp = name + " " + num;
                    while (cf.Contains(name_temp))
                    {
                        num++;
                        name_temp = name + " " + num;
                    }
                    name = name_temp;
                }
                cf.Add(name);
                server = data.add;
                port = data.port;
                cipher = "auto";
                if (data.net == "ws") ws_opts = new WsOpts(data);
                if (data.tls == "tls") tls = true;
                else tls = false;

                if (!string.IsNullOrEmpty(data.sni)) servername = data.sni;

                uuid = data.id;
                alterId = data.aid;
                network = data.net;
            }
            public Proxies(ref List<string> cf, string _type, string _name, string _server, string _password)
            {
                type = _type;
                name = ClearID(_name);
                id = GetID(name);
                if (cf.Contains(name))
                {
                    int num = 1;
                    string name_temp = name + " " + num;
                    while (cf.Contains(name_temp))
                    {
                        num++;
                        name_temp = name + " " + num;
                    }
                    name = name_temp;
                }
                cf.Add(name);
                var ipport = _server.Split(':');
                server = ipport[0];
                port = ipport[1].ToInt();
                password = _password;
            }
            public Proxies(ref List<string> cf, string _type, string _name, string _server, string _cipher, string _password)
            {
                type = _type;
                name = ClearID(_name);
                id = GetID(name);
                if (cf.Contains(name))
                {
                    int num = 1;
                    string name_temp = name + " " + num;
                    while (cf.Contains(name_temp))
                    {
                        num++;
                        name_temp = name + " " + num;
                    }
                    name = name_temp;
                }
                cf.Add(name);
                var ipport = _server.Split(':');
                server = ipport[0];
                port = ipport[1].ToInt();
                cipher = _cipher;
                password = _password;
            }
            public string? id { get; set; }
            public string name { get; set; }
            public string server { get; set; }
            public int port { get; set; }
            public string type { get; set; }
            public string? uuid { get; set; }
            public string? alterId { get; set; }
            public string cipher { get; set; }
            public bool? tls { get; set; }
            public string? password { get; set; }
            public string? servername { get; set; }
            public string? network { get; set; }
            public WsOpts? ws_opts { get; set; }

            public string ID()
            {
                return id == null ? name : id + " " + name;
            }
            public override string ToString()
            {
                var arr = new List<string> {
                    id==null?"name: "+name:"name: "+id+" "+name,
                    "server: "+server,
                    "port: "+port,
                    "type: "+type,
                };
                if (uuid != null) arr.Add("uuid: " + uuid);
                if (alterId != null) arr.Add("alterId: " + alterId);
                if (cipher != null) arr.Add("cipher: " + cipher);
                if (password != null) arr.Add("password: " + password);
                if (tls.HasValue) arr.Add("tls: " + (tls.Value ? "true" : "false"));

                if (servername != null) arr.Add("servername: " + servername);
                if (network != null) arr.Add("network: " + network);
                if (ws_opts != null) arr.Add("ws-opts: " + ws_opts.ToString());
                return "{" + string.Join(", ", arr) + "}";
            }
        }

        public class WsOpts
        {
            public WsOpts(Vmess data)
            {
                headers_Host = data.add;
                path = "/";
                if (!string.IsNullOrEmpty(data.host)) headers_Host = data.host;
                if (!string.IsNullOrEmpty(data.path) && (data.path != "/speedtest" && data.path != "/nguevws" && data.path != "/vmess"))
                    path = data.path;
            }
            public WsOpts() { }
            public string path { get; set; }
            public string headers_Host { get; set; }

            public override string ToString()
            {
                if (path.Contains("?") || path.Contains("=")) return "{path: \"" + path + "\", headers: {Host: " + headers_Host + "}}";
                return "{path: " + path + ", headers: {Host: " + headers_Host + "}}";
            }
        }

        public class Vmess
        {
            public string v { get; set; }
            public string ps { get; set; }
            public string add { get; set; }
            public int port { get; set; }
            public string id { get; set; }
            public string aid { get; set; }
            public string net { get; set; }
            public string type { get; set; }
            //public string security { get; set; }
            public string host { get; set; }
            public string path { get; set; }
            public string tls { get; set; }
            public string sni { get; set; }
        }

        #endregion
    }
}
