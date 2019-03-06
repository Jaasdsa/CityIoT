using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CityUtils
{
    /// <summary>
    /// 异步执行不带参数不带返回值方法，可设置超时时间
    /// </summary>
    public class ActionTimeout
    {
        /// DEMO --示例
        //Timeout timeout = new Timeout();
        //timeout.Do = ExcuteHandle;
        //bool bo = timeout.DoWithTimeout(new TimeSpan(0, 0, 0, 10));//只等待10秒

        public delegate void DoHandler();
        private ManualResetEvent mTimeoutObject;
        //标记变量
        private bool mBoTimeout;
        public DoHandler Do;

        public ActionTimeout()
        {
            //  初始状态为 停止
            this.mTimeoutObject = new ManualResetEvent(true);
        }
        ///<summary>
        /// 指定超时时间 异步执行某个方法
        ///</summary>
        ///<returns>执行 是否超时</returns>
        public bool DoWithTimeout(TimeSpan timeSpan)
        {
            if (this.Do == null)
            {
                return false;
            }
            this.mTimeoutObject.Reset();
            this.mBoTimeout = true; //标记
            this.Do.BeginInvoke(DoAsyncCallBack, null);
            // 等待 信号Set
            if (!this.mTimeoutObject.WaitOne(timeSpan, false))
            {
                this.mBoTimeout = true;
            }
            return this.mBoTimeout;
        }
        ///<summary>
        /// 异步委托 回调函数
        ///</summary>
        ///<param name="result"></param>
        private void DoAsyncCallBack(IAsyncResult result)
        {
            try
            {
                this.Do.EndInvoke(result);
                // 指示方法的执行未超时
                this.mBoTimeout = false;
            }
            catch
            {
                //  Console.WriteLine(ex.Message);
                this.mBoTimeout = true;
            }
            finally
            {
                this.mTimeoutObject.Set();
            }
        }
    }

    /// <summary>
    /// 异步执行带参数不带返回值方法，可设置超时时间
    /// </summary>
    /// <typeparam name="T">方法的参数类型</typeparam>
    public class ActionTimeout<T>
    {
        /// DEMO --示例
        //Timeout<Command> timeout = new Timeout<Command>();
        //timeout.Do = ExcuteHandle;
        //bool bo = timeout.DoWithTimeout(command, new TimeSpan(0, 0, 0, 10));//只等待10秒

        public delegate void DoHandler(T t);
        private ManualResetEvent mTimeoutObject;
        //标记变量
        private bool mBoTimeout;
        public DoHandler Do;

        public ActionTimeout()
        {
            //  初始状态为 停止
            this.mTimeoutObject = new ManualResetEvent(true);
        }
        ///<summary>
        /// 指定超时时间 异步执行某个方法
        ///</summary>
        ///<returns>执行 是否超时</returns>
        public bool DoWithTimeout(T t, TimeSpan timeSpan)
        {
            if (this.Do == null)
            {
                return false;
            }
            this.mTimeoutObject.Reset();
            this.mBoTimeout = true; //标记
            this.Do.BeginInvoke(t, DoAsyncCallBack, null);
            // 等待 信号Set
            if (!this.mTimeoutObject.WaitOne(timeSpan, false))
            {
                this.mBoTimeout = true;
            }
            return this.mBoTimeout;
        }
        ///<summary>
        /// 异步委托 回调函数
        ///</summary>
        ///<param name="result"></param>
        private void DoAsyncCallBack(IAsyncResult result)
        {
            try
            {
                this.Do.EndInvoke(result);
                // 指示方法的执行未超时
                this.mBoTimeout = false;
            }
            catch 
            {
              //  Console.WriteLine(ex.Message);
                this.mBoTimeout = true;
            }
            finally
            {
                this.mTimeoutObject.Set();
            }
        }
    }


    /// <summary>
    /// 异步执行带参数带返回值方法，可设置超时时间
    /// </summary>
    /// <typeparam name="TParam">方法的参数类型</typeparam>
    /// <typeparam name="TResult">方法的返回值类型</typeparam>
    public class FuncTimeout<TParam,TResult>
    {
        /// DEMO --示例
        //Timeout<Command,bool> timeout = new Timeout<Command,bool>();
        //timeout.Do = ExcuteHandle;
        //bool bo = timeout.DoWithTimeout(command, new TimeSpan(0, 0, 0, 10),out bool result);//只等待10秒

        public delegate TResult DoHandler(TParam t);
        private ManualResetEvent mTimeoutObject;
        //标记变量
        private bool mBoTimeout;
        private TResult tResult;
        public DoHandler Do;

        public FuncTimeout()
        {
            //  初始状态为 停止
            this.mTimeoutObject = new ManualResetEvent(true);
        }
        ///<summary>
        /// 指定超时时间 异步执行某个方法
        ///</summary>
        ///<returns>执行 是否超时</returns>
        public bool DoWithTimeout(TParam tParam, TimeSpan timeSpan ,out TResult tResult)
        {
            if (this.Do == null)
            {
                tResult = this.tResult;
                return false;
            }
            this.mTimeoutObject.Reset();
            this.mBoTimeout = true; //标记
            this.Do.BeginInvoke(tParam, DoAsyncCallBack, null);
            // 等待 信号Set
            if (!this.mTimeoutObject.WaitOne(timeSpan, false))
            {
                this.mBoTimeout = true; 
            }
            tResult = this.tResult;
            return this.mBoTimeout;
        }
        ///<summary>
        /// 异步委托 回调函数
        ///</summary>
        ///<param name="result"></param>
        private void DoAsyncCallBack(IAsyncResult result)
        {
            try
            {
               this.tResult=  this.Do.EndInvoke(result);
                // 指示方法的执行未超时
                this.mBoTimeout = false;
            }
            catch
            {
              //  Console.WriteLine(ex.Message);
                this.mBoTimeout = true;
            }
            finally
            {
                this.mTimeoutObject.Set();
            }
        }
    }

}
