using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.IO;
using ModulePR;

namespace ConsoleApp2
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Write("Введи количество методов: ");
            int count = Int32.Parse(Console.ReadLine());
            //int count = 4;
            MethodN[] M = new MethodN[count];
            MPR mprm = new MPR();
            AnalysisResult Arezult = new AnalysisResult();
            int Tcal;
            int Trab;
            Random randObj = new Random();
            using (StreamWriter sw = File.AppendText("Text.txt"))
            {
                sw.WriteLine(DateTime.Now + " Начали работу! Ввели: " + count + " методов");
            }
            for (int i = 0; i < count; i++)
            {
                Console.Write("Введи через пробел Время калибровки и Время работы для метода " + i + ": ");
                var text = Console.ReadLine();
                string[] texts = text.Split(' ');
                Tcal = Int32.Parse(texts[0]);
                Trab = Int32.Parse(texts[1]);
                //Tcal = randObj.Next(1, 5);
                //Trab = randObj.Next(1, 5);
                M[i] = new MethodN(Tcal, Trab, i);
                using (StreamWriter sw = File.AppendText("Text.txt"))
                {
                    sw.WriteLine("Метод "+i+" Калибровка = "+Tcal+" Результат = "+Trab);
                }
            }

            while (true)
            {
                var R1 = new DetectionResult();
                Console.Clear();
                bool flag = false; 
                for (int i = 0; i < count; i++)
                {
                    R1 = M[i].Work();
                    if (R1.CalibON == true)
                    {
                        using (StreamWriter sw = File.AppendText("Text.txt"))
                        {
                            sw.WriteLine("Калибровка метода "+i+" выполняется");
                        }
                        //Console.WriteLine(i + " Calibration");
                    }
                    else if (R1.TimeofRezult != true)
                    {
                        flag = true;
                        mprm.mprAdd(R1, Arezult);
                        using (StreamWriter sw = File.AppendText("Text.txt"))
                        {
                            sw.WriteLine(DateTime.Now +" Метод " + R1.NumberofMethod+" Сработал "+"Время дедеткт.: "+R1.TimeofDetect+" Вероятность "+R1.Veroyat+" Коорд. "+ R1.Coord+" Расход "+ R1.Rashod);
                        }
                        Console.WriteLine("Метод " + R1.NumberofMethod + " Сработал " + "Время дедеткт.: " + R1.TimeofDetect + " Вероятность " + R1.Veroyat + " Коорд. " + R1.Coord + " Расход " + R1.Rashod);
                    }
                    else
                    {
                        //Console.WriteLine("Zdem");
                    }
                }
                if (flag) Arezult.PrintRez();
                Thread.Sleep(1000);
            }
        }
    }
    public class DetectionResult
    {
        public bool CalibON;
        public bool TimeofRezult;
        public int NumberofMethod;
        public DateTime TimeofDetect;
        public bool Veroyat;
        public int Coord;
        public int Rashod;
    }

    public class AnalysisResult
    {
        public DetectionResult[] Rezults = new DetectionResult[5];
        public void PrintRez() {
            if ((Rezults[0]) == null) { return; }
                Console.WriteLine("Следующие методы показали приближенное значение своей работы:");
            using (StreamWriter sw = File.AppendText("Text.txt"))
            {
                sw.WriteLine("=====================Следующие методы покали приближенное значение своей работы======================");
                for (int i = 0; i < Rezults.Length; i++)
                {
                    if ((Rezults[i]) != null) {
                        Console.WriteLine("Метод № " + Rezults[i].NumberofMethod + " Нашел утечку за " + Rezults[i].TimeofDetect + " На координате: " + Rezults[i].Coord + " При расходе: " + Rezults[i].Rashod + "   ");
                        sw.WriteLine(" Метод " + Rezults[i].NumberofMethod + " Нашел утечку за " + Rezults[i].TimeofDetect + " Коорд. " + Rezults[i].Coord + " Расход " + Rezults[i].Rashod);
                    }
                }
                sw.WriteLine("=====================================================================================================");
            }
        }
    }

    public class MethodN
    {
        private int Tcalc;
        private int Tresn;
        private int Tresninit;
        private int MethodNum;
        static Random randObj = new Random();

        public MethodN(int Tcal, int Tres, int Metnum)
        { // Construct
            Tcalc = Tcal;
            Tresninit = Tresn = Tres;
            MethodNum = Metnum;
            //Console.WriteLine(MethodNum + " Init With Tcal=" + Tcalc + " Tres=" + Tresn);
        }
        public bool CheckCal()
        { // Proverka na istechenie vremeni kalibrovki
            if ((Tcalc) != 0) { Tcalc--; return false; };
            return true;
        }
        public bool RandomBool()
        {
            if (randObj.Next(0, 2) == 1) return true;
            return false;
        }
        public DetectionResult Work()
        {
            if (CheckCal() == false)
            {
                return new DetectionResult { CalibON = true }; // Kalibrovka
            }
            Tresn--;
            if (Tresn == 0)
            {// worked
                Tresn = Tresninit;
                //Console.WriteLine("Est rezult!");
                return new DetectionResult
                {
                    CalibON = false,
                    NumberofMethod = MethodNum,
                    TimeofDetect = DateTime.Now,
                    Veroyat = RandomBool(),
                    Coord = randObj.Next(0, 1000),
                    Rashod = randObj.Next(0, 100)
                };
            }
            return new DetectionResult { TimeofRezult = true };
        }
    }
    public class MPR
    {
        public List<DetectionResult> All = new List<DetectionResult>();

        public void mprAdd(DetectionResult a, AnalysisResult Arezult)
        {
            if (a.Veroyat == true)
            { // Анализируем только методы с вероятностью true
                All.Add(a);
                All.Sort((x, y) => x.Coord.CompareTo(y.Coord));

                int[] Coord = new int[All.Count];
                for (int i = 0; i < All.Count; i++)
                { // массив координат отсортированный
                    Coord[i] = All[i].Coord;
                }
                int index = 0;
                index = FindDT.FindMinDt(Coord);
                
                Arezult.Rezults[0] = new DetectionResult();
                Arezult.Rezults[0].NumberofMethod = All[index].NumberofMethod;
                Arezult.Rezults[0].TimeofDetect = All[index].TimeofDetect;
                Arezult.Rezults[0].Rashod = All[index].Rashod;
                Arezult.Rezults[0].Coord = All[index].Coord;

                if (All.Count > 1)
                {
                    Arezult.Rezults[1] = new DetectionResult();
                    Arezult.Rezults[1].NumberofMethod = All[index + 1].NumberofMethod;
                    Arezult.Rezults[1].TimeofDetect = All[index + 1].TimeofDetect;
                    Arezult.Rezults[1].Rashod = All[index + 1].Rashod;
                    Arezult.Rezults[1].Coord = All[index + 1].Coord;
                }
            }
        }

    }

}