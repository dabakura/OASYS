using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OASYS
{
    public class Interfaz
    {
       public string ROL(int ID)
        {
            if (ID == 1)
            {
                return "~/Views/Shared/_Layout_Admin.cshtml";
            }

            if (ID == 2)
            {
               return "~/Views/Shared/_Layout_Colab.cshtml";
            }

            if (ID == 3)
            {
                return "~/Views / Shared / _Layout_Prof.cshtml";
            }

            if (ID == 4)
            {
                return "~/Views/Shared/_Layout_Estud.cshtml";
                
            }

            return "";
            
        }
    }
}