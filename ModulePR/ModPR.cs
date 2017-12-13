using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace ModulePR
{
    public class FindDT
    {
        public static int FindMinDt(int[] Mass)
        {
            Array.Sort(Mass);
            int[] DtCoord = new int[Mass.Length - 1];
            int index = 0;
            for (int i = 0; i < Mass.Count() - 1; i++)// массив разниц между координатами
            { 
                DtCoord[i] = Mass[i + 1] - Mass[i];
            }
            int maxel = int.MaxValue;
            for (int i = 0; i < DtCoord.Count(); i++) // Ищем индекс минимального значения 
            {
                if (DtCoord[i] <= maxel)
                {
                    maxel = DtCoord[i];
                    index = i;
                }
            }
            return index;
        }

    }

}

