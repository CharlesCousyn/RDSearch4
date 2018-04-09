using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CrawlerOrphanet.tools
{
    public class TimeLeft
    {
        static private TimeLeft instance;

        private Mutex m;

        private Mutex m2;
        public int operationsToDo { get; set; }

        public int operationsDone { get; set; }

        public double averageSecondsToMakeAnOperation{get; set;}

        private TimeLeft()
        {
            this.operationsToDo = 0;
            this.operationsDone = 0;
            this.averageSecondsToMakeAnOperation = 0;
            this.m = new Mutex();
            this.m2 = new Mutex();
        }
        //Singleton code
        public static TimeLeft Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new TimeLeft();
                }
                return instance;
            }
        }

        public void CalcAndShowTimeLeft()
        {
            this.m2.WaitOne();
            Console.Out.Flush();
            double secondsLeft = (operationsToDo - operationsDone) * averageSecondsToMakeAnOperation;
            double progress = (double)((double)operationsDone/(double)operationsToDo) * 100.0;

            //Show time left
            TimeSpan time = TimeSpan.FromSeconds(secondsLeft);
            string str = time.ToString(@"hh\:mm\:ss\:fff");
            Console.Write("Estimated time left: "+str+", Progress: "+ Math.Round(progress, 2)+"%               " );

            Console.SetCursorPosition(0, Console.CursorTop);
            this.m2.ReleaseMutex();
        }

        public void CalcAndShowTimeLeft(int batchNum, int numberOfBatchs)
        {
            this.m2.WaitOne();
            Console.Out.Flush();
            double secondsLeft = (operationsToDo - operationsDone) * averageSecondsToMakeAnOperation;
            double progress = (double)((double)operationsDone / (double)operationsToDo) * 100.0;

            //Show time left
            TimeSpan time = TimeSpan.FromSeconds(secondsLeft);
            string str = time.ToString(@"hh\:mm\:ss\:fff");
            Console.Write("Batch "+ batchNum+"/"+ numberOfBatchs+": Estimated time left: " + str + ", Progress: " + Math.Round(progress, 2) + "%                  ");

            Console.SetCursorPosition(0, Console.CursorTop);
            this.m2.ReleaseMutex();
        }

        public void IncrementOfXOperations(double seconds, int numberOfOperations)
        {
            this. m.WaitOne();
            this.operationsDone += numberOfOperations;
            this.averageSecondsToMakeAnOperation = (double)((double)(this.operationsDone - numberOfOperations) *this.averageSecondsToMakeAnOperation + seconds) / (double) this.operationsDone;
            this.m.ReleaseMutex();
        }

        public void Reset()
        {
            this.operationsToDo = 0;
            this.operationsDone = 0;
            this.averageSecondsToMakeAnOperation = 0;
        }
    }
}
