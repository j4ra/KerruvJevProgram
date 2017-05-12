using System;
using System.IO;
using System.Collections.Generic;

//TOMM¥ €A$H  -  Winaloto
namespace chuj
{
    class Program
    {
        static void Main(string[] args)
        {
            string dataPath;

            Console.WriteLine("Where's the motherfuckin' data??: ");
            dataPath = Console.ReadLine();

            while(!File.Exists(dataPath))
            {
                Console.WriteLine("Are you fuckin' wit me m8?!?\nThere's no file in {0}, you dipshit!\nNow tell me where is the data file?:", dataPath);
                dataPath = Console.ReadLine();
            }

            DataProcessor dp = new DataProcessor(dataPath);

            switch(dp.Run())
            {
                case 0:
                    break;
                case 1:
                    Console.WriteLine("You stupid idiot, the data file contains invalid data!!!\nIt's time to end this stupid game!");
                    break;
            }

            Console.WriteLine("The end is near\nPress any key to abandon program");
            Console.ReadKey(true);
        }
    }

    class DataProcessor
    {
        string dataFilePath;
        List<VoltageData> voltageData;
        List<OutputSingleData> outputData;
        
        public DataProcessor(String dataPath)
        {
            dataFilePath = dataPath;
            voltageData = new List<VoltageData>();
            outputData = new List<OutputSingleData>();
        }

        public int Run()
        {
            if(!LoadData())
            {
                return 1;
            }
            Console.WriteLine("Now little shit tell me do you want to know what was in the data file?");
            if(Console.ReadKey(true).Key == ConsoleKey.Y)
            {
                PrintData();
            }
            Questioning();

            Console.WriteLine("Tell me, motherfucker, do you want to see your data?");
            if (Console.ReadKey(true).Key == ConsoleKey.Y)
            {
                PrintData();
            }

            DoStat();
            SaveData();


            return 0;
        }

        void SaveData()
        {
            Console.WriteLine("Hey stupid little shit, where do you want your processed data to be saved?!");
            string outPath = Console.ReadLine();
            while(File.Exists(outPath))
            {
                Console.WriteLine("I am going to overwrite your stupid file {0}, what do you think?", outPath);
                if (Console.ReadKey(true).Key == ConsoleKey.Y)
                {
                    break;
                }
                else
                {
                    outPath = Console.ReadLine();
                }
            }

            StreamWriter sw = new StreamWriter(outPath);
            foreach(var sd in outputData)
            {
                sw.WriteLine("{0}\t{1}", sd.voltage, sd.intensity);
            }
            sw.Close();
        }

        void Questioning()
        {
            Console.WriteLine("Now you, sucha waste of semen, will tell me what were the voltages!");
            int i = 1;
            foreach (var vd in voltageData)
            {
                Console.WriteLine("What was voltage during measurement number {0}, you cock sucking idiot?!?", i);
                string answer = Console.ReadLine();
                double v = 0.0;
                while (!double.TryParse(answer, out v)) 
                {
                    Console.WriteLine("STOP Fucking with me! you little fucker\nEnter some nonretarded number, you moron!");
                    answer = Console.ReadLine();
                }
                Console.WriteLine("You entered: {0}", v);
                vd.Voltage = v;
                i++;
            }
        }

        void DoStat()
        {
            foreach (var vd in voltageData)
            {
                double avgInt = 0.0;
                for (int i = 10; i > 0; i--)
                {
                    avgInt += vd.Data[vd.Data.Count - i].intensity;
                }
                avgInt /= 10.0;
                OutputSingleData osd = new OutputSingleData();
                osd.intensity = avgInt;
                osd.voltage = vd.Voltage;
                outputData.Add(osd);
            }
        }

        void PrintData()
        {
            foreach(var vd in voltageData)
            {
                foreach(var sd in vd.Data)
                {
                    Console.WriteLine(sd.number + " " + sd.time + " " + sd.intensity + " " + vd.Voltage);
                }
            }
        }

        bool LoadData()
        {
            int i = 0;
            StreamReader sr = new StreamReader(dataFilePath);
            VoltageData current = new VoltageData();
            while(!sr.EndOfStream)
            {
                string line = sr.ReadLine();
                if(line=="")
                {
                    if(i>0)
                    {
                        voltageData.Add(current);
                    }
                    current = new VoltageData();
                }
                else
                {
                    SingleData sd = new SingleData();
                    line = line.Trim(' ');
                    string[] split = line.Split('\t');
                    if(split.Length != 3)
                    {
                        Console.WriteLine("Wrong amount of numbers!");
                        sr.Close();
                        return false;
                    }
                    try
                    {
                        sd.number = int.Parse(split[0].Trim(' '));
                        sd.time = double.Parse(split[1].Trim(' '), System.Globalization.NumberFormatInfo.InvariantInfo);
                        sd.intensity = double.Parse(split[2].Trim(' '), System.Globalization.NumberFormatInfo.InvariantInfo);
                    }
                    catch(Exception e)
                    {
                        Console.WriteLine(split[0]);
                        Console.WriteLine(split[1]);
                        Console.WriteLine(split[2]);
                        Console.WriteLine(e.Message);
                        sr.Close();
                        return false;
                    }
                    current.Data.Add(sd);
                }
                i++;
            }
            voltageData.Add(current);
            sr.Close();
            return true;
        }
    }

    struct SingleData
    {
        public int number;
        public double time;
        public double intensity;
    }

    class VoltageData
    {
        List<SingleData> _voltageData;
        double voltage = -1;


        public List<SingleData> Data { get { return _voltageData; } set { _voltageData = value; } }
        public double Voltage { get { return voltage; } set { voltage = value; } }

        public VoltageData()
        {
            _voltageData = new List<SingleData>();
        }
    }

    struct OutputSingleData
    {
        public double voltage;
        public double intensity;
    }
}
