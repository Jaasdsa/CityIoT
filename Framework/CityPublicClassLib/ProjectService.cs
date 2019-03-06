using CityPublicClassLib;
using CityUtils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CityPublicClassLib 
{
    public class Interface
    {
        /// <summary>
        /// 拿到配置的dll的实例，dll需要重写下面CaseManagerInjection的方法，用于框架的接入
        /// </summary>
        /// <param name="dllPath"></param>
        /// <returns></returns>
        public static CaseManagerInjection GetInjection(string dllPath)
        {
            Assembly assembly;
            bool USE_THE_RIGHT_WAY = true;
            if (USE_THE_RIGHT_WAY)
            {
                assembly = Assembly.Load(Path.GetFileNameWithoutExtension(XMLHelper.GetRootFilePath(dllPath)));
            }
            else
            {
                // 直接加载文件会导致 "LoadNeither" 错误 (与当前运行环境不在一个上下文, 目前受影响的是XML序列化会报类型转换错误)
                assembly = Assembly.LoadFile(Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory + "bin\\" + dllPath));
            }

            foreach (Type type in assembly.GetTypes())
            {
                if (type.IsSubclassOf(typeof(CaseManagerInjection)))
                {
                    return (CaseManagerInjection)Activator.CreateInstance(type);
                }
            }
            throw new NotImplementedException("接口DLL中没有派生自 CaseManagerInjection 的对象。");
        }
    }
    public class CaseManagerInjection : MarshalByRefObject, ISonService, IServiceWorker
    {

        public virtual bool IsRuning { get; set; }

        public virtual void Start(out string errMsg)
        {
            errMsg = "";
        }
        public virtual void Stop()
        {

        }

        public virtual void ReceiveCommand(RequestCommand command)
        {

        }

        #region Start()实例
        //// 项目服务管理器 采用配置的dll执行项目
        //if (caseManagerInjection != null)
        //    caseManagerInjection.Stop();
        //caseManagerInjection = Interface.GetInjection(Config.dllName);
        //caseManagerInjection.Start(out errMsg);
        //if (caseManagerInjection.IsRuning)
        //    TraceManagerForWeb.AppendDebug("成功加载并打开:" + Config.dllName); 
        //else
        //{
        //    TraceManagerForWeb.AppendErrMsg("加载失败" + Config.dllName + errMsg);
        //    Stop();
        //    return;
        //}
        #endregion

        #region Stop()实例
        //try
        //{
        //    // 项目服务管理器 采用配置的dll执行项目
        //    if (caseManagerInjection != null)
        //    {
        //        caseManagerInjection.Stop();
        //        if (!caseManagerInjection.IsRuning)
        //        {
        //            TraceManagerForWeb.AppendDebug("成功停止" + Config.dllName);
        //            this.caseManagerInjection = null;
        //        }
        //        else
        //            TraceManagerForWeb.AppendErrMsg("停止失败" + Config.dllName);
        //    }
        //}
        //catch (Exception e)
        //{
        //    LogManager.AppendErrMsg(ServerTypeName.Dispatch, "项目动态库停止失败:" + Config.dllName +"--"+ e.Message);
        //}
        #endregion

    }
}
