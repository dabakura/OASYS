using OASYS.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OASYS
{
    public class Permisos
    {
        public void close()
        {
            LoginController a = new LoginController();
            a.Close();
        }
    }
}