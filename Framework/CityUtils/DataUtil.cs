using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CityUtils 
{
    /// <summary>
    /// 工具类
    /// </summary>
    public class DataUtil
    {
        /// <summary>
        /// 把数据库的日期格式化为 yyyy-MM-dd HH:mm:ss
        /// </summary>
        /// <param name="tableCell"></param>
        /// <returns></returns>
        public static string ToDateString(object dataTableCell)
        {
            string dateString = "";
            try
            {
                dateString = Convert.ToDateTime(dataTableCell).ToString("yyyy-MM-dd HH:mm:ss");
            }
            catch
            {
                dateString = "1900-01-01 00:00:00";
            }
            return (dateString == "1900-01-01 00:00:00") ? "" : dateString;
        }
        public static DateTime ToDateTime(string dateTimeStr)
        {
          return  Convert.ToDateTime(ToDateString(dateTimeStr));
        }

        /// <summary>
        /// 把数据库的日期格式化 yyyy-MM-dd HH:mm:ss/yyyy-MM-dd
        /// </summary>
        /// <param name="dataTableCell"></param>
        /// <param name="format"></param>
        /// <returns></returns>
        public static string ToDateString(object dataTableCell, string format)
        {
            string dateString = "";
            try
            {
                dateString = Convert.ToDateTime(dataTableCell).ToString(format);
            }
            catch
            {
                dateString = dataTableCell.ToString();
            }
            return (dateString == "1900-01-01 00:00:00") ? "" : dateString;
        }

        /// <summary>
        /// 转换成int类型
        /// </summary>
        /// <param name="tableCell"></param>
        /// <returns></returns>
        public static int ToInt(object dataTableCell)
        {
            int response = 0;
            int.TryParse(Convert.ToString(dataTableCell), out response);
            return response;
        }
        /// <summary>
        ///  转换为16位符号整数
        /// </summary>
        /// <param name="dataTableCell"></param>
        /// <returns></returns>
        public static ushort ToUshort(int dataTableCell)
        {
            ushort response = 0;
            try
            {
                return Convert.ToUInt16(dataTableCell);
            }
            catch { return response; }
        }

        /// <summary>
        /// 转换成单精度
        /// </summary>
        /// <param name="tableCell"></param>
        public static float ToFloat(object dataTableCell)
        {
            float response = 0;
            float.TryParse(Convert.ToString(dataTableCell), out response);
            return response;
        }

        /// <summary>
        /// 转换成双精度
        /// </summary>
        /// <param name="dataTableCell"></param>
        /// <returns></returns>
        public static double ToDouble(object dataTableCell)
        {
            double response = 0;
            //dataTableCell.ToString()修改为Convert.ToString(dataTableCell)解决dataTableCell为null报错问题
            double.TryParse(Convert.ToString(dataTableCell), out response);
            return response;
        }

        /// <summary>
        /// 转换成字符串
        /// </summary>
        /// <param name="dataTableCell"></param>
        /// <returns></returns>
        public static string ToString(object dataTableCell)
        {
            if (dataTableCell == null)
            {
                return "";
            }
            return dataTableCell.ToString();
        }


        /// <summary>
        /// 获取两个时间点的时间差
        /// </summary>
        /// <param name="DateTime1"></param>
        /// <param name="beginDateTime"></param>
        /// <returns></returns>
        public static string DateDiff(DateTime endDateTime, DateTime beginDateTime)
        {
            string dateDiff = null;
            TimeSpan ts = endDateTime.Subtract(beginDateTime).Duration();
            dateDiff = ts.Days.ToString() + "天" + ts.Hours.ToString() + "小时" + ts.Minutes.ToString() + "分钟" + ts.Seconds.ToString() + "秒";
            return dateDiff;
        }

        private static readonly DateTime UTC_BASE = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        private static readonly long UTC_BASE_TICKS = UTC_BASE.Ticks;
        private static readonly long TIMEZONE_BEIJING = 8 * 60 * 60 * 1000;

        /// <summary>
        /// 从UTC long值转换为DateTime值；
        /// </summary>
        /// <param name="time">long值时间</param>
        /// <returns></returns>
        public static DateTime ConvertIntDateTime(string timeStamp)
        {
            long nTime = Convert.ToInt64(timeStamp);
            return new DateTime((nTime + TIMEZONE_BEIJING) * TimeSpan.TicksPerMillisecond + UTC_BASE_TICKS);
        }

        /// <summary>
        /// 转换为UTC，Long时间值
        /// </summary>
        /// <param name="time">时间</param>
        /// <returns>Long值，距离1970，1，1的毫秒数</returns>
        public static long ConvertDateTimeInt(DateTime time)
        {
            return (time.ToUniversalTime().Ticks - UTC_BASE_TICKS) / TimeSpan.TicksPerMillisecond;
        }

        /// <summary>
        /// 获取当前时间
        /// </summary>
        /// <returns></returns>
        public static string GetNowTime()
        {
            string currentTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            return currentTime;
        }

        /// <summary>
        /// 根据两个日期 计算其中去除周末的时长 返回天数
        /// </summary>
        /// <param name="dateFrom"></param>
        /// <param name="dateTo"></param>
        /// <returns></returns>
        public static int CalcWorkDays(string dateFrom, string dateTo)
        {
            DateTime start = Convert.ToDateTime(dateFrom);
            DateTime end = Convert.ToDateTime(dateTo);
            TimeSpan span = end - start;

            int AllDays = Convert.ToInt32(span.TotalDays) + 1;//差距的所有天数
            int totleWeek = AllDays / 7;//差别多少周
            int yuDay = AllDays % 7; //除了整个星期的天数
            int lastDay = 0;
            if (yuDay == 0) //正好整个周
            {
                lastDay = AllDays - (totleWeek * 2);
            }
            else
            {
                int weekDay = 0;
                int endWeekDay = 0; //多余的天数有几天是周六或者周日
                switch (start.DayOfWeek)
                {
                    case DayOfWeek.Monday:
                        weekDay = 1;
                        break;
                    case DayOfWeek.Tuesday:
                        weekDay = 2;
                        break;
                    case DayOfWeek.Wednesday:
                        weekDay = 3;
                        break;
                    case DayOfWeek.Thursday:
                        weekDay = 4;
                        break;
                    case DayOfWeek.Friday:
                        weekDay = 5;
                        break;
                    case DayOfWeek.Saturday:
                        weekDay = 6;
                        break;
                    case DayOfWeek.Sunday:
                        weekDay = 7;
                        break;
                }
                if ((weekDay == 6 && yuDay >= 2) || (weekDay == 7 && yuDay >= 1) || (weekDay == 5 && yuDay >= 3) || (weekDay == 4 && yuDay >= 4) || (weekDay == 3 && yuDay >= 5) || (weekDay == 2 && yuDay >= 6) || (weekDay == 1 && yuDay >= 7))
                {
                    endWeekDay = 2;
                }
                if ((weekDay == 6 && yuDay < 1) || (weekDay == 7 && yuDay < 5) || (weekDay == 5 && yuDay < 2) || (weekDay == 4 && yuDay < 3) || (weekDay == 3 && yuDay < 4) || (weekDay == 2 && yuDay < 5) || (weekDay == 1 && yuDay < 6))
                {
                    endWeekDay = 1;
                }
                lastDay = AllDays - (totleWeek * 2) - endWeekDay;
            }
            return lastDay;
        }

        public static double ToDoubleByScale(double data,double scale)
        {
            string srtScale = scale.ToString();
            if (srtScale.IndexOf('.') == -1)
            {
                //倍率为整数
                return data * scale;
            }
            int len = scale.ToString().Length;
            int index = len - scale.ToString().IndexOf('.') - 1;
            return Math.Round(data * scale, index);
        }
        private static int maxScale = 10;
        public static bool CheckScale(double scale)
        {
            if (scale == 0)
                return false;
            //int  scaleInt = 1;
            // if (scale <= 0)
            //     return false;
            // int i = 0;
            // while (i < maxScale)
            // {
            //     if (scale * scaleInt == 1)
            //     {
            //         return true;
            //     }                   
            //     scaleInt = scaleInt * 10;
            //     i++;
            // }
            // if (i > 9)
            // {
            //     return false;
            // }
            // return false;
            return IgnoreScale();
        }
        public static bool IgnoreScale()
        {
            return true;
        }
        public static ushort GetIntScale(double scale)
        {
            ushort r = 1;
            int scaleInt = 1;
            if (scale <= 0)
                return r;
            int i = 0;
            while (i < 10)
            {
                if (scale * scaleInt == 1)
                {
                    r = Convert.ToUInt16(scaleInt);
                    return r;
                }
                scaleInt = scaleInt * 10;
                i++;
            }
            if (i > 9)
            {
                return r;
            }
            return r;
        }
    }
}
