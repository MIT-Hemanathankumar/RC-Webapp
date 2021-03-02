using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDM.Helper
{
    public class GlobalFunction
    {
        public static string GenerateToken()
        {
            return Guid.NewGuid().ToString().Replace("-", "");
        }

        public static string GenerateGuid()
        {
            return Guid.NewGuid().ToString().Replace("-", "");
        }
    }
}
