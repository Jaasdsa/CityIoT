using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CityUtils
{
    public class FileUtil
    {
        //删除指定路径的文件夹
        public static void DeleteDirectory(string directoryPath)
        {

            //删除文件
            for (int i = 0; i < Directory.GetFiles(directoryPath).ToList().Count; i++)
            {                
                 File.Delete(Directory.GetFiles(directoryPath)[i]);
            }

            //删除文件夹        
           Directory.Delete(directoryPath, true);
            
        }
    }
}
