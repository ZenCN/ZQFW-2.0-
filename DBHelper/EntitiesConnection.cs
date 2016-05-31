using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EntityModel;

namespace DBHelper
{
    public class EntitiesConnection
    {
        /// <summary>
        /// 根据级别获取连接名称（与web.config中name名称一样）
        /// </summary>
        /// <param name="level"></param>
        /// <returns></returns>
        public string getConnectionNameByLevel(int level)
        {
            string connName = "";
            switch (level)
            {
                case 5://查询乡镇
                    connName = entityName.FXTWNEntities.ToString();
                    break;
                case 4://查询县级
                    connName = entityName.FXCNTEntities.ToString();
                    break;
                case 3://查询市级
                    connName = entityName.FXCTYEntities.ToString();
                    break;
                case 2://查询省级
                    connName = entityName.FXPRVEntities.ToString();
                    break;
                case 0:
                    connName = entityName.FXCLDEntities.ToString();
                    break;
                default:
                    //字典库
                    //connName = entityName.FXDICTEntities.ToString();
                    break;
            }
            return connName;
        }

        public enum entityName
        {
            FXDICTEntities,//字典库

    
            FXTWNEntities,//乡镇库
            FXCNTEntities,//县级库
            FXCTYEntities,//市级库
            FXPRVEntities,//省级库
            FXCLDEntities//云库

        }
    }
}
