using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace cnnnet.Lib.Utils
{
    public static class Contract
    {
        public static void Requires<TException>(bool check, string message = null) 
            where TException : Exception, new()
        {
            if (check == false)
            {
                throw new TException();
            }
        }
    }
}
