using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyShop.Common
{
    public interface IDbConnectionFactory
    {
        IDbConnection CetDbConnection(DbConnectionType connectionType);
    }
}
