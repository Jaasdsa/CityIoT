using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace CityUtils
{
    public class COMHelper
    {

        //  每一个COM控件都有一个全球唯一的标识，CoClass的GUID，简称CLSID；

        //  每一个COM控件注册后，都会在注册表中的 “HKEY_CLASSES_ROOT\CLSID”键下创建一个以COM控件的CLSID命名的键（带大括号），所以判断一个COM控件是否注册，可在 “HKEY_CLASSES_ROOT\CLSID”键下查看是否存在以该COM控件的CLSID命名的注册表键。代码如下：

        /// <summary>
        /// 检查指定CLSID的COM控件是否注册
        /// </summary>
        /// <param name="clsid">COM控件的CLSID，不带大括号</param>
        /// <returns>是否已经注册</returns>
        public static bool IsRegister(string clsid,out string filePath)
        {
            filePath = "";
            RegistryKey rkTest = Registry.ClassesRoot.OpenSubKey(String.Format("CLSID\\{{{0}}}\\InprocServer32", clsid));
            if (rkTest != null)
            {
                var val = rkTest.GetValue("");//获取注册表中注册的dll路径
                if (val != null)
                {
                    bool r = System.IO.File.Exists(val.ToString());
                    if (r)
                        filePath = val.ToString();
                    return r;
                }
            }
            return false;
        }

        /// <summary>
        /// 检查指定CLSID的COM控件是否注册，并返回文件信息
        /// </summary>
        /// <param name="clsid">COM控件的CLSID，不带大括号</param>
        /// <param name="file">文件信息</param>
        /// <returns>是否已经注册</returns>
        public static bool IsRegister(string clsid, out FileInfo file)
        {
            file = null;
            RegistryKey root = RegistryKey.OpenBaseKey(RegistryHive.ClassesRoot, RegistryView.Registry32);
            string cld = String.Format("\\CLSID\\{0}{1}{2}", "{", clsid, "}");
            RegistryKey comKey = root.OpenSubKey(cld);
            if (comKey == null) return false;

            RegistryKey fileKey = comKey.OpenSubKey("InprocServer32");
            if (fileKey == null) return false;
            string filename = fileKey.GetValue("").ToString();
            if (!string.IsNullOrEmpty(filename))
            {
                file = new FileInfo(filename);
            }
            return true;
        }

        /// <summary>
        /// 根据指定的文件名创建一个运行Regsvr32的Process
        /// </summary>
        /// <param name="filename">文件绝对路径</param>
        /// <param name="register">注册/反注册</param>
        /// <returns></returns>
        private static Process IsDoRegsvr32(string filename, bool register)
        {
            if (!File.Exists(filename)) { return null; }
            Process p = new Process();
            p.StartInfo.FileName = string.Format("{0}\\{1}", Environment.GetFolderPath(Environment.SpecialFolder.System),
                "regsvr32.exe");
            p.StartInfo.Arguments = string.Format("/s {0}", filename);
            if (!register)
            {
                p.StartInfo.Arguments += " /u   ";
            }
            return p;
        }

        public static bool IsRegsvr32(string dllGUID,string fileName,bool register,out string errMsg)
        {
            errMsg = "";
            try
            {
                Process p = IsDoRegsvr32(fileName, register);
                if (p.Start())
                {
                    p.WaitForExit(3000);// 只等三秒钟，防止阻塞卡死
                    try { p.Dispose(); } catch { }
                    try { p.Kill(); } catch { }
                    if (register)
                    {  // 注册
                        if (IsRegister(dllGUID,out string filePath))
                            return true;
                        else
                        {
                            errMsg = "注册OPCDAAuto.dll失败";
                            return false;
                        }
                    }
                    else
                    {
                        // 卸载
                        if (!IsRegister(dllGUID, out string filePath))
                            return true;
                        else
                        {
                            errMsg = "卸载OPCDAAuto.dll失败";
                            return false;
                        }
                    }
                }
                else
                {
                    try { p.Kill(); } catch { }
                    errMsg = "注册OPCDAAuto.dll进程打开失败";
                    return false;
                }
            }
            catch (Exception e)
            {
                errMsg = e.Message;
                return false;
            }

        }

    }
}
