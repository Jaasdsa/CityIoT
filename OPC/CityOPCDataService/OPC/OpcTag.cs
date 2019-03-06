using OPCAutomation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CityOPCDataService
{
   public class OpcTag
    {
        public OpcTag(string name, int clientHandle, int serverHandle)
        {
            _name = name;
            _clientHandle = clientHandle;
            _serverHandle = serverHandle;
        }
        public override string ToString()
        {
            if (_lastError == null)
                return string.Format("{0}: {1}", _name, _value);
            else
                return string.Format("{0}: {1}", _name, _lastError);
        }
        public void Set(object value, int quality, DateTime timeStamp)
        {
            _value = value;
            _timeStamp = timeStamp;

            if (quality == 192)
                _lastError = null;
            else
                _lastError = "Bad";
        }
        
        public string Name
        {
            get { return _name; }
        }
        public object Value
        {
            get { return _value; }
            set { _value = value; }
        }
        public string Error
        {
            get { return _lastError; }
        }
        public DateTime TimeStamp
        {
            get { return _timeStamp; }
            set { _timeStamp = value; }
        }
        public bool IsKingView
        {
            get { return string.IsNullOrEmpty(_name) ? false : _name.EndsWith(".Value", StringComparison.OrdinalIgnoreCase); }
        }
        public int ServerHandle
        {
            get { return _serverHandle; }
        }
        public int ClientHandle
        {
            get { return _clientHandle; }
        }
        
        // 缓存属性, 保持线程访问安全
        private string _name;
        private int _clientHandle;
        private int _serverHandle;
        private object _value;
        private DateTime _timeStamp;
        private string _lastError;
    }
}
