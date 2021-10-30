using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NLog;

namespace LoggerNS
{
    public class Logger
    {
        private static NLog.Logger logger = LogManager.GetCurrentClassLogger();

        public static void WarnInfoLogger(int number)
        {
            logger.Warn("There is no info about " + number.ToString() + " packet");
        }

        public static void ErrorOpenFileLogger(string fileName)
        {
            logger.Error(new Exception(), "Can not open file " + fileName);
        }
    }
}
