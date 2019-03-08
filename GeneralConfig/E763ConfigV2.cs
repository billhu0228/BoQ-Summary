using BoQCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Configuration
{
    public class E763ConfigV2:BasicConfig
    {       
        

        public E763ConfigV2()
        {
            Name = "E763ConfigV2";
        }

        

        public override void Run()
        {
            Anouncement();
            // 循环读取桥涵信息表
            for (int i = 0; i < BridgeList.BRList.Count; i++)
            {
                curBridge = BridgeList.BRList[i];

                GenStrList(ref curBridge);

                Auxiliary(ref curBridge);
            }

        }


        public override void Anouncement()
        {
            Console.WriteLine("\nThis is General Configuration.");
        }

        public override void GenStrList( ref Bridge curBridge)
        {
            string LR = curBridge.ClassNum == 0 ? "LR" : curBridge.ClassNum == 5 ? "L" : "R";

            Console.WriteLine(curBridge.Name);

            double h0, pk0, w0;

            foreach (var item in LR)
            {
                bool isLeft = item == 'L' ? true : false;
                w0 = GetBridgeWidth(curBridge.ZH, isLeft);

                for (int i = 0; i < curBridge.SpanList.Count + 1; i++)
                {
                    // 当前设计墩高

                    double a = curBridge.SpanList.GetRange(0, i).Sum();
                    pk0 = curBridge.ZH - 0.5 * curBridge.Length + a;
                    h0 = Sjx.GetBG(pk0) - Dmx.GetBG(pk0);

                    // 获取结构类型
                    if (i == 0)
                    {
                        Globals.BeamType curBT = GetBeamType(i, ref curBridge);
                        GetSupStr(out curSupStr, curBridge.SpanList[i], w0, curBT);
                        int beamNum = GetTBeamNum(w0, curBT);
                        curSupStr.WriteData(ref Record, curBridge.Name, beamNum);

                        GetAbutment(out Abutment curAbut, ref curSupStr, h0, w0);
                        curAbut.WriteData(ref Record, curBridge.Name);
                    }
                    else if (i == curBridge.SpanList.Count)
                    {
                        Globals.BeamType curBT = GetBeamType(i - 1, ref curBridge);
                        GetSupStr(out curSupStr, curBridge.SpanList[i - 1], w0, curBT);

                        GetAbutment(out Abutment curAbut, ref curSupStr, h0, w0);
                        curAbut.WriteData(ref Record, curBridge.Name);
                    }
                    else
                    {
                        Globals.BeamType curBT = GetBeamType(i, ref curBridge);
                        GetSupStr(out curSupStr, curBridge.SpanList[i - 1], w0, curBT);
                        int beamNum = GetTBeamNum(w0, curBT);
                        curSupStr.WriteData(ref Record, curBridge.Name, beamNum);

                        GetPier(out curPier, ref curSupStr, h0);
                        if (curPier != null)
                        {
                            if (curPier.GetType() == typeof(SolidCirclePier))
                            {
                                curPier.WriteData(ref Record, curBridge.Name, 3);
                            }
                            else
                            {
                                curPier.WriteData(ref Record, curBridge.Name);
                            }


                            GetCapBeam(out CapBeam curCB, w0);
                            curCB.WriteData(ref Record, curBridge.Name);

                            GetPileCap(out PileCap curPC, ref curSupStr, ref curPier);
                            curPC.WriteData(ref Record, curBridge.Name);


                            GetPile(out Pile curPile, curBridge.ZH);
                            curPile.WriteData(ref Record, curBridge.Name);
                        }
                    }
                }
            }
        }



        public override void GetPile(out Pile curPile,  double cZH)
        {
            double ZL = GetZLength(cZH);

            curPile = new Pile(ZL,1.0,100);            
        }



        public override void GetPileCap(out PileCap curPC,ref SupStructure curBT, ref Pier curPier)
        {
            if (curPier.GetType()==typeof(SolidCirclePier))
            {
                curPC = new PileCap(1.8, 12.8, 1.5, 140,"PC-1");
            }
            else if (curPier.GetType() == typeof(SolidRecPier))
            {
                if (curBT.GetType()==typeof(TBeam))
                {
                    // R187
                    curPC = new PileCap(6,9.6,1.8,140,"PC-2");
                }
                else
                {
                    curPC = new PileCap(9, 9, 2.5, 140, "PC-3");                    
                }
                
            }
            else if (curPier.GetType() == typeof(HollowRecPier))
            {
                if (curBT.GetType() == typeof(TBeam))
                {
                    curPC = new PileCap(9, 9, 2.5, 140, "PC-3");
                }
                else if (curPier.L >= 40)
                {
                    curPC = new PileCap(11, 11, 3, 140, "PC-5");
                }
                else
                {
                    curPC = new PileCap(10, 10, 3, 140, "PC-4");
                }
            }
            else
            {
                curPC = null;
            }
            
        }


        /// <summary>
        /// 配置盖梁
        /// </summary>
        /// <param name="curCB">盖梁类</param>
        /// <param name="curBeam">当前主梁</param>
        /// <param name="curPier">当前桥墩</param>
        /// <param name="w0">当前桥宽</param>
        public override void GetCapBeam(out CapBeam curCB,  double w0)
        {
            double l = w0;
            double dl, dv;
            if (curPier==null||curPier.GetType()==typeof(SolidCirclePier))
            {
                dl = 1.6;
                dv = 1.7;
            }
            else
            {
                dl = curPier.DimLong + 0.4;
                if (curSupStr.curBeamType == Globals.BeamType.T25)
                {
                    dv = 2.0;
                }
                else
                {
                    dv = 2.5;
                }
            }

            curCB = new CapBeam(l, dl, dv, 180,0);
        }



        /// <summary>
        /// 配置桥墩
        /// </summary>
        /// <param name="curPier">桥墩类</param>
        /// <param name="curBt">当前主梁</param>
        /// <param name="hh">设计高差</param>
        public override void GetPier(out Pier curPier, ref SupStructure curBT, double hh)
        {
            double h0 = hh - curBT.H;
            curPier=null;

            if (h0<0)
            {
                curPier = null;
            }
            else if(h0<=10)
            {
                curPier = new SolidCirclePier(1.0,h0,180,0);
            }
            else if (h0 <= 25)
            {
                if (curBT.curBeamType>=Globals.BeamType.B50)
                {
                    curPier = new SolidRecPier(7, 2.2, h0, 160, 0);
                }
                else if (curBT.curBeamType >= Globals.BeamType.T25)
                {
                    curPier = new SolidRecPier(7, 1.8, h0, 160, 0);
                }
                else
                {
                    curPier = null;
                }
               

            }
            else if (h0 <= 40)
            {
                if (curBT.curBeamType >= Globals.BeamType.B50)
                {
                    curPier = new HollowRecPier(19.6, 10.24, 4.5, h0, 180, 0);
                }
                else if (curBT.curBeamType >= Globals.BeamType.T25)
                {
                    curPier = new HollowRecPier(17.5, 10.04, 3.5, h0, 180, 0);
                }
                else
                {
                    curPier = null;
                }

            }
            else
            {
                curPier = new HollowRecPier(21, 10.8, 4.5, h0, 180, 0);
            }            


            
        }

        /// <summary>
        ///  配置主梁
        /// </summary>
        /// <param name="obj">主梁输出</param>
        /// <param name="span">跨径</param>
        /// <param name="w0">桥宽</param>
        /// <param name="curBT">上部类型</param>
        public override void GetSupStr(out SupStructure obj,double span,double w0,Globals.BeamType curBT)
        {
            if (span==10)
            {
                obj = new SlabBeam(1, span, w0, 170, 0);
            }
            else if (span==15)
            {
                obj = new SlabBeam(1, span, w0, 170, 0);
            }
            else if (span==25)
            {
                obj = new TBeam(1.57,0.44,0.08,25,170,65);                
            }
            else if (span == 35)
            {
                if (curBT==Globals.BeamType.B60)
                {
                    double Ac;//涂装长度
                    if (w0 == 13.65)
                    {
                        Ac = 9.66;                        
                    }
                    else if (w0 == 14.65)
                    {
                        Ac = 10.0549;                        
                    }
                    else if (w0 == 17.15)
                    {
                        Ac = 11.4707;                        
                    }
                    else if (w0 == 18.15)
                    {
                        Ac = 11.8;                        
                    }
                    else
                    {
                        Ac = 0;                        
                    }
                    obj = new BoxBeam(3.6, Ac, span, 190, 40);
                }
                else
                {
                    obj = new TBeam(2.1, 0.744229, 0.072857, 35, 170.8917, 59.20414);
                }
            }
            else if (span == 50)
            {
                double Ac, Lc;//涂装长度
                if (w0 == 13.65)
                {
                    Ac = 9.66;
                    Lc = 18.3;
                }
                else if (w0 == 14.65)
                {
                    Ac = 10.0549;
                    Lc = 19.3;
                }
                else if (w0 == 17.15)
                {
                    Ac = 11.4707;
                    Lc = 22.2;
                }
                else if (w0 == 18.15)
                {
                    Ac = 11.8;
                    Lc = 23.1;
                }
                else
                {
                    Ac = 0;
                    Lc = 0;
                }
                obj = new BoxBeam(3.6, Ac, span, 190, 40);
                
            }
            else if (span == 60)
            {
                double Ac;//涂装长度
                if (w0 == 13.65)
                {
                    Ac = 9.66;                    
                }
                else if (w0 == 14.65)
                {
                    Ac = 10.0549;                    
                }
                else if (w0 == 17.15)
                {
                    Ac = 11.4707;                    
                }
                else if (w0 == 18.15)
                {
                    Ac = 11.8;                    
                }
                else
                {
                    Ac = 0;                    
                }
                obj = new BoxBeam(3.6, Ac, span, 190, 40);
            }
            else
            {
                throw new Exception("跨径无匹配上部类型.");
            }
        }

        /// <summary>
        /// 生成桥台对象
        /// </summary>
        /// <param name="curAbut">输出</param>
        /// <param name="curSupStr">当前上部结构</param>
        /// <param name="h0">真实高差（允许负值）</param>
        public override void GetAbutment(out Abutment curAbut,ref SupStructure curSupStr,double h0,double w0)
        {
            double embark = 0;            
            double bwh = curSupStr.H;
            double abwt = curSupStr.curBeamType <= Globals.BeamType.T35 ? 1.8 : 2.5;            


            if (curSupStr.H + 1.5 > h0 + 0.5)
            {
                embark = curSupStr.H + 1.5;
            }
            else
            {
                embark = h0 + 0.5;
            }
            curAbut = new Abutment(embark,w0,bwh,abwt,140,150,120,120,120);
        }

        public override double GetBridgeWidth(double pks,bool isleft=true)
        {
            if (isleft)
            {
                if (pks <= 164200)
                {
                    if (((pks >= 154621) && (pks < 156136)) || ((pks >= 162423) && (pks < 163671)))
                    {
                        return 18.15;
                    }
                    else
                    {
                        return 14.65;
                    }
                }
                else if (pks <= 222300)
                {
                    if (((pks >= 179000) && (pks < 182320)) || ((pks >= 218396) && (pks < 221001)))
                    {
                        return 17.15;
                    }
                    else
                    {
                        return 13.65;
                    }
                }
                else
                {
                    if (((pks >= 224071) && (pks < 225571)) || ((pks >= 226983) && (pks < 228408)) || ((pks >= 238381) && (pks < 240602)))
                    {
                        return 18.15;
                    }
                    else
                    {
                        return 14.65;
                    }
                }

            }
            else
            {
                if (pks <= 164200)
                {
                    if (((pks >= 151889) && (pks < 155701)) || ((pks >= 161245) && (pks < 163569)))
                    {
                        return 18.15;
                    }
                    else
                    {
                        return 14.65;
                    }
                }
                else if (pks <= 222300)
                {
                    if (((pks >= 164722) && (pks < 166558)) || ((pks >= 171812) && (pks < 174400)) || ((pks >= 176545) && (pks < 179767)) || ((pks >= 187691) && (pks < 190246.869)) || ((pks >= 191520) && (pks < 193075)) || ((pks >= 195015) && (pks < 196035)) || ((pks >= 196720) && (pks < 199628)) || ((pks >= 200710) && (pks < 206017)) || ((pks >= 207838) && (pks < 217472)))
                    {
                        return 17.15;
                    }
                    else
                    {
                        return 13.65;
                    }
                }
                else
                {
                    if (((pks >= 223516) && (pks < 225144)) || ((pks >= 226580) && (pks < 228052)) || ((pks >= 229615) && (pks < 231910)) || ((pks >= 234025) && (pks < 239482)) || ((pks >= 247966) && (pks < 252459)))
                    {
                        return 18.15;
                    }
                    else
                    {
                        return 14.65;
                    }
                }
            }
            
        }

        public override int GetTBeamNum(double w0,Globals.BeamType refBT)
        {
            int beamNum;
            
            if (refBT == Globals.BeamType.T25)
            {
                if (w0 == 13.65)
                {
                    beamNum = 8;

                }
                else if (w0 == 14.65)
                {
                    beamNum = 9;

                }
                else if (w0 == 17.15)
                {
                    beamNum = 10;

                }
                else if (w0 == 18.15)
                {
                    beamNum = 11;
                }
                else
                {
                    beamNum = 0;
                }
            }
            else if (refBT == Globals.BeamType.T35)
            {
                if (w0 == 13.65)
                {
                    beamNum = 7;

                }
                else if (w0 == 14.65)
                {
                    beamNum = 8;

                }
                else if (w0 == 17.15)
                {
                    beamNum = 9;

                }
                else if (w0 == 18.15)
                {
                    beamNum = 10;
                }
                else
                {
                    beamNum = 0;
                }
            }
            else
            {
                beamNum = 1;
            }
            return beamNum;
        }


        /// <summary>
        /// 获取第spanIndex跨主梁类型
        /// </summary>
        /// <param name="spanIndex">索引</param>
        /// <param name="curBridge">对象</param>
        /// <returns></returns>
        private Globals.BeamType GetBeamType(int spanIndex,ref Bridge curBridge)
        {
            double span = curBridge.SpanList[spanIndex];
            if (span == 10)
            {
                return Globals.BeamType.F10;
            }
            else if (span == 15)
            {
                return Globals.BeamType.F15;
            }
            else if (span == 25)
            {
                return Globals.BeamType.T25;
            }
            else if (span == 35)
            {

                if (spanIndex == 0)
                {
                    if (curBridge.SpanList.Count == 1)
                    {
                        return Globals.BeamType.T35;
                    }
                    else if (curBridge.SpanList[spanIndex + 1] == 60)
                    {
                        return Globals.BeamType.B60;
                    }
                    else
                    {
                        return Globals.BeamType.T35;
                    }
                }
                else if (spanIndex == curBridge.SpanList.Count - 1)
                {
                    if (curBridge.SpanList[spanIndex - 1] == 60)
                    {
                        return Globals.BeamType.B60;
                    }
                    else
                    {
                        return Globals.BeamType.T35;
                    }
                }
                else
                {
                    if (curBridge.SpanList[spanIndex + 1] == 60 | curBridge.SpanList[spanIndex - 1] == 60)
                    {
                        return Globals.BeamType.B60;
                    }
                    else
                    {
                        return Globals.BeamType.T35;
                    }

                }
            }
            else if (span == 50)
            {
                return Globals.BeamType.B50;
            }
            else if (span == 60)
            {
                return Globals.BeamType.B60;
            }

            return Globals.BeamType.F15;
        }


        
        public override double GetZLength(double cZH)
        {
            return 30.0;
        }

        internal virtual void Auxiliary(ref Bridge curBridge)
        {
            ;
        }

        public override double GetZHTLength(double pk)
        {
            return GetZLength(pk) * 0.05;
        }

       
    }
}
