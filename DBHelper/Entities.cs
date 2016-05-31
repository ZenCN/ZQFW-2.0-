using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EntityModel;
using System.Configuration;
/*----------------------------------------------------------------
    // 版本说明：
    // 版本号：
    // 文件名：GetEntity.cs
    // 文件功能描述：获取对应单位的数据库实体模型 字典库直接实例化FXDICTEntities
    // 创建标识： 刘志 2013-6-28
    // 修改标识： 
    // 修改描述：
//-------------------------------------------------------------*/

namespace DBHelper
{
    public class Entities:EntitiesInterface
    {

        #region 获取除  字典库  以外的数据实体
        /// <summary>
        /// 根据连接字符串的名称返回对应单位的数据库实体模型
        /// </summary>
        /// <param name="connName">传入的枚举类型的连接字符串名称</param>
        /// <returns>对应业务库实体模型</returns>
        EntitiesConnection getConn = new EntitiesConnection();
        public BusinessEntities GetEntityByConn( EntitiesConnection.entityName connName)
        {
            string connStr = ConfigurationManager.ConnectionStrings[connName.ToString()].ConnectionString;
            BusinessEntities entity = new BusinessEntities(connStr);
            return entity;
        }

        /// <summary>
        /// 根据连接字符串的名称返回对应单位的数据库实体模型(重载)(这样不好，可能输入的连接字符串名称不存在)
        /// </summary>
        /// <param name="connName"></param>
        /// <returns></returns>
        //public BusinessEntities GetEntityByConn(string connName)
        //{
        //    if (connName.Trim().Length > 0)
        //    {
        //        string connStr = ConfigurationManager.ConnectionStrings[connName.ToString()].ConnectionString;
        //        BusinessEntities entity = new BusinessEntities(connStr);
        //        return entity;
        //    }
        //    else
        //    {
        //        BusinessEntities entityCloud = new BusinessEntities();
        //        return entityCloud;
        //    }
        //}

        /// <summary>
        /// 根据级别返回对应单位的数据库实体模型
        /// </summary>
        /// <param name="level">当前使用单位的单位级别</param>
        /// <returns>对应业务库实体模型</returns>
        public BusinessEntities GetEntityByLevel(int level) 
        {
            string connName = getConn.getConnectionNameByLevel(level);
            string connStr = ConfigurationManager.ConnectionStrings[connName.ToString()].ConnectionString;
            //BusinessEntities entity = new BusinessEntities();//此方法实例化的对象默认是云库的,需要使用他的重载
            BusinessEntities entity = new BusinessEntities(connStr);
            return entity;
        }
        #endregion


        #region 获取持久化的实体（包含字典库）

        /// <summary>
        /// 根据级别获取持久化的数据实体
        /// </summary>
        /// <param name="level">单位级别</param>
        /// <returns>object类型的模型</returns>
        public object GetPersistenceEntityByLevel(int level)
        {
            EntitiesConnection.entityName name = new EntitiesConnection.entityName();
            switch (level)
            {
                case 5://查询乡镇
                    name = EntitiesConnection.entityName.FXTWNEntities;
                    break;
                case 4://查询县级
                    name = EntitiesConnection.entityName.FXCNTEntities;
                    break;
                case 3://查询市级
                    name = EntitiesConnection.entityName.FXCTYEntities;
                    break;
                case 2://查询省级
                    name = EntitiesConnection.entityName.FXPRVEntities;
                    break;
                case 0:
                    name = EntitiesConnection.entityName.FXCLDEntities;
                    break;
                default:
                    //字典库
                    name = EntitiesConnection.entityName.FXDICTEntities;
                    break;
            }
            //Persistence_old persistence = new Persistence_old();
            object entity =new Persistence().GetPersistenceEntity(name);
            return entity;
        }

        /// <summary>
        /// 根据枚举的实体名称，获取持久化的数据实体
        /// </summary>
        /// <param name="entityName">枚举类型的数据库实体名称</param>
        /// <returns>object类型的模型</returns>
        public object GetPersistenceEntityByEntityName(EntitiesConnection.entityName entityName)
        {
            //Persistence_old persistence = new Persistence_old();
            object entity = new Persistence().GetPersistenceEntity(entityName);
            return entity;
        }

        #endregion
    }
}
