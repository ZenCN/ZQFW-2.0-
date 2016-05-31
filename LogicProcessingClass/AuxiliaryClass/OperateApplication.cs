using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
/*----------------------------------------------------------------
    // 版本说明：
    // 版本号：
    // 文件名：OperateApplication.cs
    // 文件功能描述：操作Application缓存类
    // 创建标识：
    // 修改标识：
    // 修改描述：
//-------------------------------------------------------------*/


namespace LogicProcessingClass.AuxiliaryClass
{
    public class OperateApplication
    {
        public static HttpApplicationState httpapplication = HttpContext.Current.Application;

        /// <summary>
        /// 根据名字获得Application
        /// </summary>
        /// <param name="applicationName">apppliaction名字</param>
        /// <returns>一个Object的application对象</returns>
        public Object GetApplication(string applicationName)
        {
            Object obj = httpapplication[applicationName];
            return obj;
        }
        /// <summary>
        /// 把数据存入到缓存
        /// </summary>
        /// <param name="applicationName">apppliaction名字</param>
        public void SetApplication(string applicationName, object obj)
        {
            httpapplication[applicationName] = obj;
        }

        /// <summary>
        /// 判断这个applicationName在Application集合当中有没有

        /// </summary>
        /// <param name="applicationName">apppliaction名字</param>
        /// <returns>true(有) or false(没有)</returns>
        public bool JudgeAppcalitionName(string applicationName)
        {
            bool flag = false;
            if (httpapplication[applicationName] != null)
            {
                flag = true;
                return flag;
            }
            return flag;
        }
    }
}
