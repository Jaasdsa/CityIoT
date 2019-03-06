using Microsoft.Win32;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration.Install;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.ServiceProcess;
using System.Text;

namespace CityUtils
{
    public class ServiceToolEx
    {
        // 启动  停止 注册 卸载
        public static ServiceController InstallService(string filePath, string serviceName, string displayName, string description, ServiceStartMode startMode)
        {
            bool a = IsServiceExist(serviceName);

            string startModeText = "demand";
            switch (startMode)
            {
                case ServiceStartMode.Automatic:
                    startModeText = "auto";
                    break;
                case ServiceStartMode.Manual:
                    startModeText = "demand";
                    break;
                case ServiceStartMode.Disabled:
                    startModeText = "disabled";
                    break;
            }

            ProcessStartInfo psi = new ProcessStartInfo("sc.exe");
            psi.CreateNoWindow = true;
            psi.WindowStyle = ProcessWindowStyle.Hidden;

            psi.Arguments = string.Format("create \"{0}\" binPath= \"{1}\" start= {2}", serviceName, filePath, startModeText);
            using (Process sc = Process.Start(psi))
            {
                sc.WaitForExit();
            }

            if (!string.IsNullOrEmpty(displayName))
            {
                psi.Arguments = string.Format("config \"{0}\" DisplayName= \"{1}\"", serviceName, displayName);
                using (Process sc = Process.Start(psi))
                {
                    sc.WaitForExit();
                }
            }

            if (!string.IsNullOrEmpty(description))
            {
                psi.Arguments = string.Format("description \"{0}\" \"{1}\"", serviceName, description);
                using (Process sc = Process.Start(psi))
                {
                    sc.WaitForExit();
                }
            }

            return new ServiceController(serviceName);
        }

        public static bool UninstallService(string serviceName)
        {
            ProcessStartInfo psi = new ProcessStartInfo("sc.exe");
            psi.CreateNoWindow = true;
            psi.WindowStyle = ProcessWindowStyle.Hidden;
            psi.Arguments = string.Format("delete \"{0}\"", serviceName);

            using (Process sc = Process.Start(psi))
            {
                sc.WaitForExit();
            }

            try
            {
                Registry.LocalMachine.DeleteSubKeyTree(@"SYSTEM\CurrentControlSet\services\" + serviceName);
            }
            catch { }

            return !IsServiceExist(serviceName);
        }

        public static ServiceController StartService(string serviceName,string[] args)
        {
            try
            {
                ServiceController service = new ServiceController(serviceName);

                if (service.Status == ServiceControllerStatus.Running)
                {
                    return service;
                }
                else
                {
                    if (args == null)
                        service.Start();
                    else
                        service.Start(args);
                    service.WaitForStatus(ServiceControllerStatus.Running, TimeSpan.FromSeconds(30));
                }

                return service;
            }
            catch(Exception e)
            {
                return null;
            }
        }

        public static ServiceController StopService(string serviseName)
        {
            try
            {
                ServiceController service = new ServiceController(serviseName);
                if (service.Status == ServiceControllerStatus.Stopped)
                {
                    return service;
                }
                else
                {
                    service.Stop();
                    service.WaitForStatus(ServiceControllerStatus.Stopped, TimeSpan.FromSeconds(30));
                }

                return service;
            }
            catch
            {
                return null;
            }
        }

        // 服务是否存在
        public static bool IsServiceExist(string serviceName)
        {
            try
            {
                ServiceController[] services = ServiceController.GetServices();
                foreach (ServiceController service in services)
                {
                    if (string.Compare(service.ServiceName, serviceName, true) == 0)
                    {
                        return true;
                    }
                }
                return false;
            }
            catch
            {
                return false;
            }
        }

        public static bool IsServiceRunning(string serviceName)
        {
            ServiceController service = new ServiceController(serviceName);
            if (service.Status != ServiceControllerStatus.Stopped)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 获取服务安装路径
        /// </summary>
        /// <param name="ServiceName"></param>
        /// <returns></returns>
        public static string GetWindowsServiceInstallPath(string ServiceName)
        {
            string key = @"SYSTEM\CurrentControlSet\Services\" + ServiceName;
            string path = Registry.LocalMachine.OpenSubKey(key).GetValue("ImagePath").ToString();
            //替换掉双引号   
            path = path.Replace("\"", string.Empty);

            FileInfo fi = new FileInfo(path);
            return fi.Directory.ToString();

        }

        // 获取服务列表
        public static string[] GetLocalHostServerList()
        {
            List<string> services = new List<string>();
            try
            {
                ServiceController[] serviceControllers = ServiceController.GetServices();
                foreach (ServiceController service in serviceControllers)
                {
                    services.Add(service.ServiceName);
                }
            }
            catch { }
            return services.ToArray();
        }

        // 停止服务
        public static bool StopService(string serviceName,out string errMsg)
        {
            errMsg = "";
            try
            {
                ServiceController service = new ServiceController(serviceName);
                if (service.Status == ServiceControllerStatus.Stopped)
                    return true;
                else
                {
                    service.Stop();
                    service.WaitForStatus(ServiceControllerStatus.Stopped, TimeSpan.FromSeconds(30));
                }
                // 判断停止是否成功
                if (service.Status == ServiceControllerStatus.Stopped)
                    return true;
                else
                {
                    errMsg = serviceName + "停止失败";
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

    /// <summary>
    /// 注册服务
    /// </summary>
    public class RegisteredServer
    {
        /// <summary>
        /// 它用于保存执行提交、回滚或卸载操作所需的信息。
        /// </summary>
        IDictionary stateSaver = new Hashtable();

        private static RegisteredServer _instance = null;
        private static readonly object SynObject = new object();
        public static RegisteredServer Instance
        {
            get
            {
                // Syn operation.
                lock (SynObject)
                {
                    return _instance ?? (_instance = new RegisteredServer());
                }
            }
        }
        /// <summary>
        /// 注册服务
        /// </summary>
        /// <param name="path"></param>
        /// <param name="name"></param>
        public void Registered(string path)
        {
            try
            {
                string name = GetServiceName(path.Trim());//通过服务路径获取服务名称
                if (IsExistedService(name))//服务是否存在
                {
                    if (ServiceIsRunning(name))//服务是否运行
                    {
                        if (!StopService(name))//停止服务
                            throw new Exception("停止服务出现异常");
                    }
                    UnInstallService(path, name);//卸载服务
                }
                InstallService(stateSaver, path, name);//安装服务
                ChangeServiceStartType(2, name);//改变服务类型
                ServiceStart(name); //运行服务
            }
            catch (Exception ex)
            {
                //Log.Error(ex);
            }
        }

        /// <summary>
        /// 获取Windows服务的名称
        /// </summary>
        /// <param name="serviceFileName">文件路径</param>
        /// <returns>服务名称</returns>
        private string GetServiceName(string serviceFileName)
        {
            try
            {
                Assembly assembly = Assembly.LoadFrom(serviceFileName);
                Type[] types = assembly.GetTypes();
                foreach (Type myType in types)
                {
                    if (myType.IsClass && myType.BaseType == typeof(System.Configuration.Install.Installer))
                    {
                        FieldInfo[] fieldInfos = myType.GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Default | BindingFlags.Instance | BindingFlags.Static);
                        foreach (FieldInfo myFieldInfo in fieldInfos)
                        {
                            if (myFieldInfo.FieldType == typeof(System.ServiceProcess.ServiceInstaller))
                            {
                                ServiceInstaller serviceInstaller = (ServiceInstaller)myFieldInfo.GetValue(Activator.CreateInstance(myType));
                                return serviceInstaller.ServiceName;
                            }
                        }
                    }
                }
                return "";
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 判断服务是否存在
        /// </summary>
        /// <param name="serviceName"></param>
        /// <returns></returns>
        private bool IsExistedService(string serviceName)
        {
            ServiceController[] services = ServiceController.GetServices();
            foreach (ServiceController s in services)
            {
                if (s.ServiceName == serviceName)
                {
                    return true;
                }
            }
            return false;

        }
        /// <summary>
        /// 验证服务是否启动
        /// </summary>
        /// <returns></returns>
        private bool ServiceIsRunning(string serviceName)
        {
            ServiceController service = new ServiceController(serviceName);
            if (service.Status == ServiceControllerStatus.Running)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        /// <summary>
        /// 停止服务
        /// </summary>
        /// <param name="serviseName"></param>
        /// <returns></returns>
        private bool StopService(string serviseName)
        {
            try
            {
                ServiceController service = new ServiceController(serviseName);
                if (service.Status == ServiceControllerStatus.Stopped)
                {
                    return true;
                }
                else
                {
                    TimeSpan timeout = TimeSpan.FromMilliseconds(1000 * 10);
                    service.Stop();
                    service.WaitForStatus(ServiceControllerStatus.Stopped, timeout);
                }
            }
            catch
            {

                return false;
            }
            return true;
        }

        /// <summary>
        /// 卸载服务
        /// </summary>
        /// <param name="filepath"></param>
        private void UnInstallService(string filepath, string serviceName)
        {
            try
            {
                if (IsExistedService(serviceName))
                {
                    //UnInstall Service
                    AssemblyInstaller myAssemblyInstaller = new AssemblyInstaller();
                    myAssemblyInstaller.UseNewContext = true;
                    myAssemblyInstaller.Path = filepath;
                    myAssemblyInstaller.Uninstall(null);
                    myAssemblyInstaller.Dispose();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("unInstallServiceError/n" + ex.Message);
            }
        }

        /// <summary>
        /// 安装服务
        /// </summary>
        /// <param name="stateSaver"></param>
        /// <param name="filepath"></param>
        private void InstallService(IDictionary stateSaver, string filepath, string serviceName)
        {
            try
            {
                System.ServiceProcess.ServiceController service = new System.ServiceProcess.ServiceController(serviceName);
                if (!IsExistedService(serviceName))
                {
                    //Install Service
                    AssemblyInstaller myAssemblyInstaller = new AssemblyInstaller();
                    myAssemblyInstaller.UseNewContext = true;
                    myAssemblyInstaller.Path = filepath;
                    myAssemblyInstaller.Install(stateSaver);
                    myAssemblyInstaller.Commit(stateSaver);
                    myAssemblyInstaller.Dispose();
                    //--Start Service
                    service.Start();
                }
                else
                {
                    if (service.Status != System.ServiceProcess.ServiceControllerStatus.Running && service.Status != System.ServiceProcess.ServiceControllerStatus.StartPending)
                    {
                        service.Start();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("installServiceError/n" + ex.Message);
            }
        }
        /// <summary>
        /// 启动服务
        /// </summary>
        /// <param name="serviceName"></param>
        /// <returns></returns>
        private static bool ServiceStart(string serviceName)
        {
            try
            {
                ServiceController service = new ServiceController(serviceName);
                if (service.Status == ServiceControllerStatus.Running)
                {
                    return true;
                }
                else
                {
                    TimeSpan timeout = TimeSpan.FromMilliseconds(1000 * 10);
                    service.Start();
                    service.WaitForStatus(ServiceControllerStatus.Running, timeout);
                }
            }
            catch
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// 设置服务启动类型
        /// 2为自动 3为手动 4 为禁用
        /// </summary>
        /// <param name="startType"></param>
        /// <param name="serviceName"></param>
        /// <returns></returns>
        private static bool ChangeServiceStartType(int startType, string serviceName)
        {
            try
            {
                RegistryKey regist = Registry.LocalMachine;
                RegistryKey sysReg = regist.OpenSubKey("SYSTEM");
                RegistryKey currentControlSet = sysReg.OpenSubKey("CurrentControlSet");
                RegistryKey services = currentControlSet.OpenSubKey("Services");
                RegistryKey servicesName = services.OpenSubKey(serviceName, true);
                servicesName.SetValue("Start", startType);
            }
            catch (Exception ex)
            {

                return false;
            }
            return true;
        }

    }

    public class ServiceControl
    {
        /// <summary>
        /// 注册服务（注册完就启动，已经存在的服务直接启动。）
        /// </summary>
        /// <param name="strServiceName">服务名称</param>
        /// <param name="strServiceInstallPath">服务安装程序完整路径（.exe程序完整路径）</param>
        public void Register(string strServiceName, string strServiceInstallPath)
        {
            IDictionary mySavedState = new Hashtable();

            try
            {
                System.ServiceProcess.ServiceController service = new System.ServiceProcess.ServiceController(strServiceName);

                //服务已经存在则卸载
                if (ServiceIsExisted(strServiceName))
                {
                    //StopService(strServiceName);
                    UnInstallService(strServiceName, strServiceInstallPath);
                }
                service.Refresh();
                //注册服务
                AssemblyInstaller myAssemblyInstaller = new AssemblyInstaller();

                mySavedState.Clear();
                myAssemblyInstaller.Path = strServiceInstallPath;
                myAssemblyInstaller.UseNewContext = true;
                myAssemblyInstaller.Install(mySavedState);
                myAssemblyInstaller.Commit(mySavedState);
                myAssemblyInstaller.Dispose();

               // service.Start();
            }
            catch (Exception ex)
            {
                throw new Exception("注册服务时出错：" + ex.Message);
            }
        }

        /// <summary>
        /// 卸载服务
        /// </summary>
        /// <param name="strServiceName">服务名称</param>
        /// <param name="strServiceInstallPath">服务安装程序完整路径（.exe程序完整路径）</param>
        public void UnInstallService(string strServiceName, string strServiceInstallPath)
        {
            try
            {
                if (ServiceIsExisted(strServiceName))
                {
                    //UnInstall Service
                    AssemblyInstaller myAssemblyInstaller = new AssemblyInstaller();
                    myAssemblyInstaller.UseNewContext = true;
                    myAssemblyInstaller.Path = strServiceInstallPath;
                    myAssemblyInstaller.Uninstall(null);
                    myAssemblyInstaller.Dispose();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("卸载服务时出错：" + ex.Message);
            }
        }


        /// <summary>
        /// 判断服务是否存在
        /// </summary>
        /// <param name="serviceName">服务名</param>
        /// <returns></returns>
        public bool ServiceIsExisted(string serviceName)
        {
            ServiceController[] services = ServiceController.GetServices();
            foreach (ServiceController s in services)
            {
                if (s.ServiceName == serviceName)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 启动服务（启动存在的服务，30秒后启动失败报错）
        /// </summary>
        /// <param name="serviceName">服务名</param>
        public void StartService(string serviceName)
        {
            if (ServiceIsExisted(serviceName))
            {
                System.ServiceProcess.ServiceController service = new System.ServiceProcess.ServiceController(serviceName);
                if (service.Status != System.ServiceProcess.ServiceControllerStatus.Running && service.Status != System.ServiceProcess.ServiceControllerStatus.StartPending)
                {
                    service.Start();
                    for (int i = 0; i < 30; i++)
                    {
                        service.Refresh();
                        System.Threading.Thread.Sleep(1000);
                        if (service.Status == System.ServiceProcess.ServiceControllerStatus.Running)
                        {
                            break;
                        }
                        if (i == 29)
                        {
                            throw new Exception("服务" + serviceName + "启动失败！");
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 停止服务（停止存在的服务，30秒后停止失败报错）
        /// </summary>
        /// <param name="serviceName"></param>
        public void StopService(string serviceName)
        {
            if (ServiceIsExisted(serviceName))
            {
                System.ServiceProcess.ServiceController service = new System.ServiceProcess.ServiceController(serviceName);
                if (service.Status == System.ServiceProcess.ServiceControllerStatus.Running)
                {
                    service.Stop();
                    for (int i = 0; i < 30; i++)
                    {
                        service.Refresh();
                        System.Threading.Thread.Sleep(1000);
                        if (service.Status == System.ServiceProcess.ServiceControllerStatus.Stopped)
                        {
                            break;
                        }
                        if (i == 29)
                        {
                            throw new Exception("服务" + serviceName + "停止失败！");
                        }
                    }
                }
            }
        }
    }
}
