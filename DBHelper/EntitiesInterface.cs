using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EntityModel;

namespace DBHelper
{
    public interface EntitiesInterface
    {
        BusinessEntities GetEntityByConn(EntitiesConnection.entityName connName);//根据连接字符串的名称返回对应的数据库实体模型

        BusinessEntities GetEntityByLevel(int level);//根据级别返回对应的数据库实体模型

    }
}
