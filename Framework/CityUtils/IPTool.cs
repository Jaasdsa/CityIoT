using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CityUtils
{
    public static class IPTool
    {
        // 检测IP  
        public static bool PingIP(string strIP)
        {
            if (!IsValidIP(strIP))
            {
                return false;
            }
            for (int i = 0; i < 3; i++)
            {
                // 只检测三次
                System.Net.NetworkInformation.Ping psender = new System.Net.NetworkInformation.Ping();
                System.Net.NetworkInformation.PingReply prep = psender.Send(strIP, 500, Encoding.Default.GetBytes("afdafdadfsdacareqretrqtqeqrq8899tu"));
                if (prep.Status == System.Net.NetworkInformation.IPStatus.Success)
                {
                    return true;
                }
            }
            return false;
        }
        // 验证IP  
        public static bool IsValidIP(string ip)
        {
            if (System.Text.RegularExpressions.Regex.IsMatch(ip, "[0-9]{1,3}\\.[0-9]{1,3}\\.[0-9]{1,3}\\.[0-9]{1,3}"))
            {
                string[] ips = ip.Split('.');
                if (ips.Length == 4 || ips.Length == 6)
                {
                    if (System.Int32.Parse(ips[0]) < 256 && System.Int32.Parse(ips[1]) < 256 & System.Int32.Parse(ips[2]) < 256 & System.Int32.Parse(ips[3]) < 256)
                        return true;
                    else
                        return false;
                }
                else
                {
                    return false;
                }
            }
            else
                return false;
        }
        /// <summary>
        /// 是否空闲端口
        /// </summary>
        /// <param name="port"></param>
        /// <returns></returns>
        public static bool IsValidPort(int port)
        {
            System.Net.NetworkInformation.IPGlobalProperties iproperties = System.Net.NetworkInformation.IPGlobalProperties.GetIPGlobalProperties();
            System.Net.IPEndPoint[] ipEndPoints = iproperties.GetActiveTcpListeners();
            foreach (var item in ipEndPoints)
            {
                if (item.Port == port)
                {
                    return false;
                }
            }
            return true;
        }
    }
}
