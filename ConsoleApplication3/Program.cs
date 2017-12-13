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
            MethodN[] M = new MethodN[count]; // Класс метода
            MPR mprm = new MPR(); // Класс модуля принятия решения
            AnalysisResult Arezult = new AnalysisResult(); // Класс распечатки результата
            int Tcal;
            int Trab;
            Random randObj = new Random();
            using (StreamWriter sw = File.AppendText("Text.txt")) // Запись в файл лога
            {
                sw.WriteLine(DateTime.Now + " Начали работу! Ввели: " + count + " методов");
            }
            for (int i = 0; i < count; i++) // Инициализация каждого метода
            {
                Console.Write("Введи через пробел Время калибровки и Время работы для метода " + i + ": ");
                var text = Console.ReadLine();
                string[] texts = text.Split(' ');
                Tcal = Int32.Parse(texts[0]);
                Trab = Int32.Parse(texts[1]);
                M[i] = new MethodN(Tcal, Trab, i);
                using (StreamWriter sw = File.AppendText("Text.txt"))
                {
                    sw.WriteLine("Метод "+i+" Калибровка = "+Tcal+" Результат = "+Trab);
                }
            }

            while (true)
            {
                var R1 = new DetectionResult(); // Класс результата работы метода
                Console.Clear();
                bool flag = false; // Флаг результата исполнения какого-то метода.(Калибровка прошла)
                for (int i = 0; i < count; i++)
                {
                    R1 = M[i].Work();
                    if (R1.CalibON == true) // Если калибровка выполняется то ничего не делаем
                    {
                        using (StreamWriter sw = File.AppendText("Text.txt"))
                        {
                            sw.WriteLine("Калибровка метода "+i+" выполняется");
                        }
                    }
                    else if (R1.TimeofRezult != true) // Подошло время исполнения метода (1 раз в N сек)
                    {
                        flag = true;
                        mprm.mprAdd(R1, Arezult); // Результат работы метода сохраняем
                        using (StreamWriter sw = File.AppendText("Text.txt"))
                        {
                            sw.WriteLine(DateTime.Now +" Метод " + R1.NumberofMethod+" Сработал "+"Время дедеткт.: "+R1.TimeofDetect+" Вероятность "+R1.Veroyat+" Коорд. "+ R1.Coord+" Расход "+ R1.Rashod);
                        }
                        Console.WriteLine("Мет. " + R1.NumberofMethod + " Сработал " + "Время дед.: " + R1.TimeofDetect + " Вер-ть " + R1.Veroyat + " Коорд. " + R1.Coord + " Расход " + R1.Rashod);
                    }
                    else
                    {
                        //Console.WriteLine("Zdem");
                    }
                }
                if (flag) Arezult.PrintRez(); // Если что-то сохраняли, метод сработал, то проанализируем результат
                Thread.Sleep(1000); // Пауза 1 сек
            }
        }
    }
    public class DetectionResult
    {
        public bool CalibON;            // Калибровка  
        public bool TimeofRezult;       // Есть данные работы метода
        public int NumberofMethod;      // Номер метода
        public DateTime TimeofDetect;   // Время дедект-я
        public double Veroyat;          // Вероятность дедект-я
        public int Coord;               // Координата
        public int Rashod;              // Расход
    }

    public class AnalysisResult // Распечатаем результат работы методов
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

        public MethodN(int Tcal, int Tres, int Metnum) // Инициализируем метод
        {
            Tcalc = Tcal;
            Tresninit = Tresn = Tres;
            MethodNum = Metnum;
            //Console.WriteLine(MethodNum + " Init With Tcal=" + Tcalc + " Tres=" + Tresn);
        }
        public bool CheckCal() // Проверка на истечение времени калибровки
        {
            if ((Tcalc) != 0) { Tcalc--; return false; };
            return true;
        }
        public DetectionResult Work()
        {
            if (CheckCal() == false)
            {
                return new DetectionResult { CalibON = true }; // Если еще выполняется калибровка, вернем флаг состояния, ничего не выдаем
            }
            Tresn--;
            if (Tresn == 0) // Время выдачи результата подошло
            {// worked
                Tresn = Tresninit; // Запишем время выдачи результата метода
                return new DetectionResult  // Заполним случайными значениями
                {
                    CalibON = false,
                    NumberofMethod = MethodNum,
                    TimeofDetect = DateTime.Now,
                    Veroyat = Math.Round(randObj.NextDouble(), 2),
                    Coord = randObj.Next(0, 1000),
                    Rashod = randObj.Next(0, 100)
                };
            }
            return new DetectionResult { TimeofRezult = true }; // Время не подошло, ничего не выдаем
        }
    }
    public class MPR
    {
        public List<DetectionResult> All = new List<DetectionResult>();

        public void mprAdd(DetectionResult a, AnalysisResult Arezult)
        {
            if (a.Veroyat >=0.50)// Рассматриваем к работе методы с вероятностью обнаружения > 50%
            { 
                All.Add(a); // Добавим в список результат работы метода
                All.Sort((x, y) => x.Coord.CompareTo(y.Coord)); // Сортировка результатов по координатам

                int[] Coord = new int[All.Count]; 
                for (int i = 0; i < All.Count; i++) // Массив координат, вытащим из списка координаты методов
                { // массив координат отсортированный
                    Coord[i] = All[i].Coord;
                }
                int index = 0;
                index = FindDT.FindMinDt(Coord); // Найдем минимальную разницу между 2-мя методами по координатам
                                                 // Считаем что 2 метода не могли ошибиться
                Arezult.Rezults[0] = new DetectionResult();
                Arezult.Rezults[0].NumberofMethod = All[index].NumberofMethod;
                Arezult.Rezults[0].TimeofDetect = All[index].TimeofDetect;
                Arezult.Rezults[0].Rashod = All[index].Rashod;
                Arezult.Rezults[0].Coord = All[index].Coord;

                if (All.Count > 1) // После первого исполнения тут будет только 1 результат работы метода, потому суда не зайдем
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