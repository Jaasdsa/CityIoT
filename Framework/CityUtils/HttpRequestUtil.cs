using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CityUtils
{
    public enum ContentType
    {
        Json,
        XWwwFromUrlencoded,
    }

    public class HttpRequestUtil
    {
        private int defaultTimeout = 15000; //超时时间100秒
        private string contentType = "application/json;charset=UTF-8";//链接类型  ;"application /json;charset=UTF-8"   "application/x-www-form-urlencoded;charset=UTF-8"

        public HttpRequestUtil()
        {

        }
        public HttpRequestUtil(int defaultTimeout)
        {
            this.defaultTimeout = defaultTimeout;
        }
        public HttpRequestUtil(ContentType contentType)
        {
            switch (contentType)
            {
                case ContentType.Json:
                    this.contentType = "application/json;charset=UTF-8";
                    break;
                case ContentType.XWwwFromUrlencoded:
                    this.contentType = "application/x-www-form-urlencoded;charset=UTF-8";
                    break;
            }
        }
        public HttpRequestUtil(int defaultTimeout, ContentType contentType)
        {
            this.defaultTimeout = defaultTimeout;
            switch (contentType)
            {
                case ContentType.Json:
                    this.contentType = "application/json;charset=UTF-8";
                    break;
                case ContentType.XWwwFromUrlencoded:
                    this.contentType = "application/x-www-form-urlencoded;charset=UTF-8";
                    break;
            }
        }

        /// <summary>
        /// 发送同步http Get请求
        /// </summary>
        /// <param name="url"></param>
        /// <param name="AsyncCallBack">成功回调</param>
        /// <param name="RequestErrorCallBack">失败回调</param>
        /// <param name="ExcuteErrorCallBack">执行回调方法失败回调</param>
        public void CreateSyncGetHttpRequest(string url, Action<string> AsyncCallBack, Action<string> RequestErrorCallBack, Action<string> ExcuteErrorCallBack)
        {
            HttpWebRequest request = CreateGetHttpWebRequest(url);
            ExcuteHttpRequest(request, AsyncCallBack, RequestErrorCallBack, ExcuteErrorCallBack);
        }
        /// <summary>
        /// 发送异步http Get请求
        /// </summary>
        /// <param name="url"></param>
        /// <param name="AsyncCallBack">成功回调</param>
        /// <param name="RequestErrorCallBack">失败回调</param>
        public void CreateAsyncGetHttpRequest(string url, Action<string> AsyncCallBack,Action<string> RequestErrorCallBack, Action<string> ExcuteErrorCallBack)
        {
            //HttpWebRequest request = CreateGetHttpWebRequest(url);
            //CreatAsyncHttpRequest(request, AsyncCallBack, ErrorCallBack);
            Action<string , Action<string>, Action<string>, Action<string>> action= CreateSyncGetHttpRequest;
            action.BeginInvoke(url, AsyncCallBack, RequestErrorCallBack, ExcuteErrorCallBack, null, null);
        }

        /// <summary>
        /// 发送同步http post请求
        /// </summary>
        /// <param name="url">地址</param>
        /// <param name="parameters">任意参数类型</param>
        /// <param name="AsyncCallBack">成功回调</param>
        /// <param name="RequestErrorCallBack">失败回调</param>
        /// <param name="ExcuteErrorCallBack">执行回调方法失败回调</param>
        public void CreateSyncPostHttpRequest(string url, object postObj, Action<string> AsyncCallBack, Action<string> RequestErrorCallBack, Action<string> ExcuteErrorCallBack)
        {
            HttpWebRequest request = CreatePostHttpWebRequest(url, postObj);
            ExcuteHttpRequest(request, AsyncCallBack, RequestErrorCallBack, ExcuteErrorCallBack);
        }
        public void CreateSyncPostHttpRequest(string url, IDictionary<string, string> parameters,Action<string> AsyncCallBack, Action<string> RequestErrorCallBack, Action<string> ExcuteErrorCallBack)
        {
            HttpWebRequest request = CreatePostHttpWebRequest(url, parameters);
            ExcuteHttpRequest(request, AsyncCallBack, RequestErrorCallBack, ExcuteErrorCallBack);
        }
        /// <summary>
        /// 发送异步步http post请求
        /// </summary>
        /// <param name="url">地址</param>
        /// <param name="postObj">任意参数类型</param>
        /// <param name="AsyncCallBack">成功回调</param>
        /// <param name="RequestErrorCallBack">失败回调</param>
        /// <param name="ExcuteErrorCallBack">执行回调方法失败回调</param>
        /// <param name="ExcuteErrorCallBack">执行回调方法失败回调</param>
        public void CreateAsyncPostHttpRequest(string url, object postObj, Action<string> AsyncCallBack, Action<string> RequestErrorCallBack, Action<string> ExcuteErrorCallBack)
        {
            //HttpWebRequest request = CreatePostHttpWebRequest(url, postObj);
            //CreatAsyncHttpRequest(request, AsyncCallBack, ErrorCallBack);
            Action<string, object, Action<string>, Action<string>, Action<string>> action = CreateSyncPostHttpRequest;
            action.BeginInvoke(url, postObj, AsyncCallBack, RequestErrorCallBack, ExcuteErrorCallBack, null, null);
        }
        public void CreateAsyncPostHttpRequest(string url, IDictionary<string, string> parameters, Action<string> AsyncCallBack, Action<string> RequestErrorCallBack, Action<string> ExcuteErrorCallBack)
        {
            Action<string, IDictionary<string, string>, Action<string>, Action<string>, Action<string>> action = CreateSyncPostHttpRequest;
            action.BeginInvoke(url, parameters, AsyncCallBack, RequestErrorCallBack, ExcuteErrorCallBack, null, null);
        }
        /// <summary>
        /// 创建请求对象
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        private HttpWebRequest CreateGetHttpWebRequest(string url)
        {
            var getRequest = HttpWebRequest.Create(url) as HttpWebRequest;
            getRequest.Method = "GET";
            getRequest.Timeout = defaultTimeout;
            getRequest.ContentType = contentType;
            getRequest.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
            return getRequest;
        }
        private HttpWebRequest CreatePostHttpWebRequest(string url, object postObj)
        {
            var postRequest = HttpWebRequest.Create(url) as HttpWebRequest;
            postRequest.KeepAlive = false;
            postRequest.Timeout = defaultTimeout;
            postRequest.Method = "POST";
            postRequest.ContentType = contentType;
            if (postObj == null)
                return postRequest;
            string postData = JsonConvert.SerializeObject(postObj);
            postRequest.ContentLength = postData.Length;
            postRequest.AllowWriteStreamBuffering = false;
            StreamWriter writer = new StreamWriter(postRequest.GetRequestStream(), Encoding.ASCII);
            writer.Write(postData);
            writer.Flush();
            return postRequest;
        }
        private HttpWebRequest CreatePostHttpWebRequest(string url, IDictionary<string, string> parameters)
        {
            var request = HttpWebRequest.Create(url) as HttpWebRequest;
            request.KeepAlive = false;
            request.Timeout = defaultTimeout;
            request.Method = "POST";
            request.ContentType = contentType;

            //发送POST数据  
            if (!(parameters == null || parameters.Count == 0))
            {
                StringBuilder buffer = new StringBuilder();
                int i = 0;
                foreach (string key in parameters.Keys)
                {
                    if (i > 0)
                    {
                        buffer.AppendFormat("&{0}={1}", key, parameters[key]);
                    }
                    else
                    {
                        buffer.AppendFormat("{0}={1}", key, parameters[key]);
                        i++;
                    }
                }
                //byte[] data = Encoding.ASCII.GetBytes(buffer.ToString());
                 byte[] data = Encoding.UTF8.GetBytes(buffer.ToString()); // 注意编码格式
                using (Stream stream = request.GetRequestStream())
                {
                    stream.Write(data, 0, data.Length);
                }
            }
            return request;
        }
        /// <summary>
        /// 执行请求
        /// </summary>
        /// <param name="request"></param>
        /// <param name="AsyncCallBack"></param>
        /// <param name="RequestErrorCallBack"></param>
        private void ExcuteAsyncHttpRequest(HttpWebRequest request, Action<string> AsyncCallBack, Action<string> RequestErrorCallBack)
        {
            bool isTimeout = false;
            try
            {
                IAsyncResult result = (IAsyncResult)request.BeginGetResponse((r) => {
                    try
                    {
                        HttpWebResponse response = (HttpWebResponse)request.EndGetResponse(r);
                        AsyncCallBack(GetResponseString(response));
                    }
                    catch (Exception e)
                    {
                        string errMsg = e.Message;
                        if (isTimeout)
                            errMsg = "请求超时";
                        if (RequestErrorCallBack != null)
                            RequestErrorCallBack(errMsg);
                    }
                }, null);
                
                // 超时回调
                ThreadPool.RegisterWaitForSingleObject(result.AsyncWaitHandle, new WaitOrTimerCallback((state, timedOut) =>
                {
                    if (timedOut)
                    {
                        HttpWebRequest httpWebRequest = state as HttpWebRequest;
                        if (httpWebRequest != null)
                        {
                            isTimeout = true;
                            httpWebRequest.Abort();
                        }
                    }
                }), request, defaultTimeout, true);
            }
            catch (Exception e)
            {
                string errMsg = e.Message;
                if (RequestErrorCallBack != null)
                    RequestErrorCallBack(errMsg);
            }
        }
        public void ExcuteHttpRequest(HttpWebRequest request, Action<string> AsyncCallBack, Action<string> RequestErrorCallBack,Action<string> ExcuteErrorCallBack)
        {
            try
            {
                HttpWebResponse r = request.GetResponse() as HttpWebResponse;
                string data = GetResponseString(r);
                try
                {
                    AsyncCallBack(data);
                }
                catch (Exception e)
                {
                    ExcuteErrorCallBack(e.Message);
                }
            }
            catch (Exception e)
            {
                try
                {
                    RequestErrorCallBack(e.Message);
                }
                catch (Exception ex)
                {
                    ExcuteErrorCallBack(ex.Message);
                }
            }
        }

        /// <summary>
        /// 从HttpWebResponse对象中提取响应的数据转换为字符串
        /// </summary>
        /// <param name="webresponse"></param>
        /// <returns></returns>
        private string GetResponseString(HttpWebResponse webresponse)
        {
            if (webresponse == null)
                return null;
            using (Stream s = webresponse.GetResponseStream())
            {
                StreamReader reader = new StreamReader(s, Encoding.UTF8);
                return reader.ReadToEnd();
            }
        }

    }

    //____________________**************使用案例——————****************


    //static void Main(string[] args)
    //{

    //    string url = @"http://172.16.10.33/CityInterface/rest/services/CityIoTServiceManager.svc/CaseManage/Test?test={TEST}";
    //    string urlPost = @"http://172.16.10.33/CityInterface/rest/services/CityIoTServiceManager.svc/CaseManage/TestPost";
    //    string pandaURL = @"https://new.s-water.cn/App/GetAccessToken";
    //    Person person = new Person()
    //    {
    //        name = "adsadsa",
    //        age = 12
    //    };

    //    IDictionary<string, string> parameters = new Dictionary<string, string>();
    //    parameters.Add("AppKey", "34h3rj3ri3jrt5y778934t5yfg3333h4h");
    //    parameters.Add("appSecret", "45tnn5juyojgn3rn3fnn3t5j4to3fn6y64p3");

    //    HttpRequestUtil httpRequestUtil = new HttpRequestUtil(ContentType.XWwwFromUrlencoded);
    //    httpRequestUtil.CreateAsyncPostHttpRequest(pandaURL, parameters, new Action<string>((data) =>
    //    {
    //        Console.WriteLine("POST 异步成功回调:" + data);
    //        Convert.ToInt16("aaaa");
    //    }), new Action<string>((data) =>
    //    {
    //        Console.WriteLine("POST 异步失败回调:" + data);
    //        Convert.ToInt16("aaaa");
    //    }), new Action<string>((data) =>
    //    {
    //        Console.WriteLine("POST 回调执行失败回调:" + data);
    //    }));

    //    httpRequestUtil = new HttpRequestUtil(2000);
    //    httpRequestUtil.CreateAsyncGetHttpRequest(url, new Action<string>((data) =>
    //    {
    //        Console.WriteLine("GET 异步成功回调:" + data);
    //    }), new Action<string>((data) =>
    //    {
    //        Console.WriteLine("GET 异步失败回调:" + data);
    //    }), null);


    //    httpRequestUtil.CreateAsyncPostHttpRequest(urlPost, person, new Action<string>((data) =>
    //    {
    //        Console.WriteLine("POST 异步成功回调:" + data);
    //    }), new Action<string>((data) =>
    //    {
    //        Console.WriteLine("POST 异步失败回调:" + data);
    //    }), null);

    //    Console.WriteLine("成功实现了异步");
    //    Console.ReadKey();

    //    // Test_core();
    //}
}
