using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoQCore
{
    public class TBeam : SupStructure
    {
        public double AcPrecast { get; private set; }
        public double AcInplace { get; private set; }

        /// <summary>
        /// T梁构造函数
        /// </summary>
        /// <param name="h">梁高</param>
        /// <param name="acprecast">预制断面面积</param>
        /// <param name="acinplace">现浇面积</param>
        /// <param name="l">梁长</param>
        /// <param name="rhoRebar">含筋率</param>
        /// <param name="rhoPreRebar">含预应力筋率</param>
        public TBeam(double h, double acprecast, double acinplace, double l, double rhoRebar, double rhoPreRebar):
            base(h,l,acprecast+acinplace,rhoRebar,rhoPreRebar)
        {            
            AcPrecast = acprecast;
            AcInplace = acinplace;
            if (l==25)
            {
                curBeamType = Globals.BeamType.T25;
            }
            else if(l==35)
            {
                curBeamType = Globals.BeamType.T35;
            }
            else
            {
                curBeamType = Globals.BeamType.None;
            }            
        }


        public override void WriteData(ref DataTable dt, string br,int time=1)
        {
            for (int i = 0; i < time; i++)
            {
                Globals.Write(ref dt, br, "T梁", string.Format("{0:D2}", i + 1), curBeamType.ToString(), "混凝土", "预制", AcPrecast*L, 1, 1, 1);
                Globals.Write(ref dt, br, "T梁", string.Format("{0:D2}", i + 1), curBeamType.ToString(), "混凝土", "现浇", AcInplace*L, 1, 1, 1);
                Globals.Write(ref dt, br, "T梁", string.Format("{0:D2}", i + 1), curBeamType.ToString(), "钢筋", "", Vc * RhoRebar, 1, 1, 1);
                Globals.Write(ref dt, br, "T梁", string.Format("{0:D2}", i + 1), curBeamType.ToString(), "预应力钢束", "", Vc * RhoPreRebar, 1, 1, 1);
            }            
        }
    }
}
