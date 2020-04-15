using System;
using System.Collections.Generic;
using System.Text;

namespace Andrew_Roe_s_Jarvis.Classes
{
    public class GeneralFunctions
    {
        public static bool CheckUlong(object unknownType)
        {
            try
            {
                Convert.ToUInt64(unknownType);
                return true;
            }

            catch (FormatException)
            {
                return false;
            }
        }
    }
}
