using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BoQCore;
using Configuration;


namespace BoQApplication
{
    class Program
    {
        static void Main()
        {
            // 准备输入数据
            SQX LK_SQX = new SQX("LK", InputDatas.LKSQX);
            DMX LK_DMX = new DMX("LK", InputDatas.LKDMX);
            GZX LK_GZX = new GZX("LK", InputDatas.LKBOB);

            
            // 调用配置器
            //Configer Bill = new Configer("E763Config");
            Configer Bill = new Configer("E763ConfigV2");
            Bill.ConfigInstance.BridgeList = LK_GZX;
            Bill.ConfigInstance.Dmx = LK_DMX;
            Bill.ConfigInstance.Sjx = LK_SQX;
            //Bill.ConfigInstance.Record = AllRecord;
            Bill.ConfigInstance.Run();

            Bill.ConfigInstance.Record.DataTableToCSV("TestA");





            // 完成



            Console.WriteLine("按任意键继续..");
            Console.ReadKey();
        }






    }
}
