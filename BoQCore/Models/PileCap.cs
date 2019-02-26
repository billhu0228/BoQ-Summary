﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace BoQCore
{
    public class PileCap:BasicModel//桩承台统计
    {
        public double LongDim//顺桥向长度
        {
            set;get;
        }
        public double TransDim//横桥向长度
        {
            set;get;
        }
        public double Height//承台高度
        {
            set;get;
        }
        public double Rho//承台配筋率
        {
            set;get;
        }
        double Vol1//承台体积
        {
            get
            {
                return LongDim * TransDim * Height;
            }
        }
        double Vol2//垫层体积
        {
            get
            {
                return (LongDim+2*0.1) * (TransDim+2*0.1) * 0.1;
            }
        }
        /// <summary>
        /// 构造函数
        /// </summary>
        public PileCap(double longDim,double transDim,double height,double r)
        {
            LongDim=longDim;
            TransDim=transDim;
            Height=height;
            Rho = r;
        }

        public PileCap()
        {
        }

        /// <summary>
        /// 输出
        /// </summary>
        /// <param name="dt"></param>
        public override void WriteData(ref DataTable dt,string br, int times = 1)
        {
            Globals.Write(ref dt, br, "承台", "", "", "", "", 1, 1, 1,1);

            //Globals.Write(ref dt, br,"桩承台", "","","混凝土","",LongDim,Vol1, xmh_zj);
            //Globals.Write(ref dt, br,"承台垫层", "","","混凝土","",LongDim,Vol2, xmh_zj);
            //Globals.Write(ref dt, br, "桩承台", "", "", "钢筋", "", Rho* Vol1,LongDim, xmh_zjrebar);
            
        }
    }
}
