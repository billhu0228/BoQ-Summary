using BoQCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Configuration
{
    public abstract class BasicConfig
    {
        public string Name;
        public DataTable Record;
        public Bridge curBridge;
        public SupStructure curSupStr;
        public Pier curPier;
        public GZX BridgeList;
        public DMX Dmx;
        public SQX Sjx;
               

        public BasicConfig()
        {
            Record = new DataTable();
            Record.Columns.Add("bridge", typeof(string));
            Record.Columns.Add("class", typeof(string));
            Record.Columns.Add("loc", typeof(string));
            Record.Columns.Add("detial", typeof(string));
            Record.Columns.Add("name", typeof(string));
            Record.Columns.Add("spec", typeof(string));
            Record.Columns.Add("quantity1", typeof(double));
            Record.Columns.Add("quantity2", typeof(double));
            Record.Columns.Add("xmh1", typeof(string));
            Record.Columns.Add("xmh2", typeof(string));

        }

        // =====================================================================
        public abstract void Run();
        public abstract void Anouncement();
        // =====================================================================
        public abstract void GetSupStr(out SupStructure curSupStr, double L, double w0, Globals.BeamType curBT);
        public abstract void GetPier(out Pier curPier,ref SupStructure curBT, double Lp);
        public abstract void GetAbutment(out Abutment curAbut, ref SupStructure curSupStr, double h0, double w0);
        public abstract void GetPile(out Pile ret,ref PileCap pc,double Lz);
        public abstract void GetPileCap(out PileCap ret,ref SupStructure curBT,ref Pier curPier);
        public abstract void GetCapBeam(out CapBeam ret, double bridgeWidth);

        // =====================================================================
        


        /// <summary>
        /// 桩长算法
        /// </summary>
        /// <param name="pk">里程</param>
        /// <returns></returns>
        public abstract double GetZLength(double pk);

        /// <summary>
        /// 桩基互通算法
        /// </summary>
        /// <param name="pk">里程</param>
        /// <returns></returns>
        public abstract double GetZHTLength(double pk);


        /// <summary>
        /// 桥宽算法
        /// </summary>
        /// <param name="pk">里程</param>
        /// <returns></returns>
        public abstract double GetBridgeWidth(double pk,bool isleft);

        /// <summary>
        /// T梁片数算法
        /// </summary>
        /// <param name="w0">桥宽</param>
        /// <param name="refBT">结构类型</param>
        /// <returns></returns>
        public abstract int GetTBeamNum(double w0, Globals.BeamType refBT);

        /// <summary>
        /// 主流程
        /// </summary>
        /// <param name="curBridge">当前桥梁对象</param>
        public abstract void GenStrList(ref Bridge curBridge);

        // =====================================================================
    }
}
