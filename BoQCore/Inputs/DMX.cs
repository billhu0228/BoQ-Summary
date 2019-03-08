using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BoQCore
{

    public struct Point
    {
        public double ZH,H;        
    }

    public class DMX
    {
        public string Name
        {
            set;get;
        }

        readonly public List<Point> PTList;
                

        public DMX(string name, byte[] inputs)
        {
            Name = name;
            PTList = new List<Point>();

            var ff = inputs;
            Decoder d = Encoding.UTF8.GetDecoder();
            int charSize = d.GetCharCount(ff, 0, ff.Length);
            char[] chs = new char[charSize];
            d.GetChars(ff, 0, ff.Length, chs, 0);
            string s = new string(chs);
            var ss = s.Split('\n');

            string line;
            string[] xx;
            foreach (string item in ss)
            {
                Point pt = new Point();
                if (item.StartsWith("//"))
                {
                    continue;
                }

                try
                {
                    line = item.TrimEnd('\r');
                    xx = Regex.Split(line, @"\s+");
                    xx[0]=xx[0].Split('_')[0];
                    pt.ZH = double.Parse(xx[0]);
                    pt.H = double.Parse(xx[1]);
                    PTList.Add(pt);
                }
                catch (Exception)
                {
                    throw;
                }

            }
            PTList.Sort((x, y) => x.ZH.CompareTo(y.ZH));
        }

        public double GetBG(double pk)
        {
            if (pk<PTList.First().ZH||pk>PTList.Last().ZH)
            {
                throw new ArgumentOutOfRangeException("里程不在设计范围内");

            }
            else if (PTList.Exists(x=>x.ZH==pk))
            {
                return PTList.Find(x => x.ZH == pk).H;
            }
            else
            {
                var xs = from pt in PTList select pt.ZH;
                List<double >tmp=xs.ToList();
                tmp.Add(pk);
                tmp.Sort();
                int i0 = tmp.IndexOf(pk);
                Point t1 = PTList[i0 - 1];
                Point t2 = PTList[i0];
                return t1.H + (t2.H - t1.H) / (t2.ZH - t1.ZH) * (pk - t1.ZH);
            }
        }


    }
}
