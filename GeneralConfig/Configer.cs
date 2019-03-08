using System;
using System.Data;
using System.IO;
using System.Reflection;
using BoQCore;

namespace Configuration
{    

    public class Configer
    {

        public BasicConfig ConfigInstance;


        public Configer(string ss)
        {
            string fullName = "Configuration." + ss;//命名空间.类型名
            object ect = Assembly.Load("Configuration").CreateInstance(fullName);
            ConfigInstance = (BasicConfig)ect;

        }
       
    }
}