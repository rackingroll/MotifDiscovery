using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MotifDiscovery
{
    class Program
    {
        static void Main(string[] args)
        {
            #region DataReading
            // Read the Local data source File, format csv
            StreamReader reader = new StreamReader(@"D:\v-chenluo\Data.csv");
            String value = reader.ReadLine();
            value = reader.ReadLine() + ",";

            List<double> SeriesOne = new List<double>();
            List<double> SeriesTwo = new List<double>();


            while (value != null)
            {
                value += ",";
                PerformenceCounter perf = new PerformenceCounter(value);
                SeriesOne.Add(double.Parse(perf.getAttributeValue(7)));
                SeriesTwo.Add(double.Parse(perf.getAttributeValue(11)));

                value = reader.ReadLine();
            }
            #endregion

            // Testing Code, the motif is the result motif
            SamplingDiscover MotifDiscover = new SamplingDiscover();
            List<double> motif = MotifDiscover.Sampling(SeriesOne, 100, 10, 5, 0.05, 10);

            Console.ReadLine();
        }
    }
}
