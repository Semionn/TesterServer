using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.ServiceModel.Security;
using System.Text;
using System.Threading.Tasks;
using WcfService1;
using System.IO;
using System.Xml.Serialization;

namespace WcfHost1
{
    class Program
    {
        static void Main(string[] args)
        {            
            /*List<User> students = new List<User>();
            students.Add(new User() { Name = "Петя" });
            students.Add(new User() { Name = "Вася" });
            students.Add(new User() { Name = "Маша" });
            WriteXMLUsers("students.xml", students);*/

            /*List<TestsTheme> test = new List<TestsTheme>();
            test.Add(new TestsTheme()
                    {
                        Name = "Общая биология",
                        Tests = new List<Test>()
                    });
            test[0].Tests.Add(new Test() 
                    {
                        Name = "Ознакомительный тест", 
                        Tasks = new List<WcfService1.Task>()
                    });
            test[0].Tests.Add(new Test()
                    {
                        Name = "Тест модуля 1",
                        Tasks = new List<WcfService1.Task>()
                    });

            test[0].Tests[0].Tasks.Add(new WcfService1.Task() 
                                    { 
                                        Text = "Простой вопрос", 
                                        Answers = new List<string>() { "Вариант 1", "Вариант 2", "Вариант 3" }, 
                                        Difficult = 0.25, 
                                        RightAnswer = 1
                                    });
            test[0].Tests[0].Tasks.Add(new WcfService1.Task()
                                    {
                                        Text = "Средний вопрос",
                                        Answers = new List<string>() { "Вариант 1", "Вариант 2", "Вариант 3" },
                                        Difficult = 0.5,
                                        RightAnswer = 0
                                    });
            test[0].Tests[1].Tasks.Add(new WcfService1.Task()
                                    {
                                        Text = "Сложный вопрос",
                                        Answers = new List<string>() { "Вариант 1", "Вариант 2", "Вариант 3", "Вариант 4" },
                                        Difficult = 1,
                                        RightAnswer = 3
                                    });
            test[0].Tests[1].Tasks.Add(new WcfService1.Task()
                                    {
                                        Text = "Простой вопрос",
                                        Answers = new List<string>() { "Вариант 1", "Вариант 2" },
                                        Difficult = 0.1,
                                        RightAnswer = 1
                                    });

            XMLWork.WriteXMLTestTheme(test);*/
            /*Console.WriteLine(Service1.GetTeta(
                new List<double>() 
                    {-1,-0.8,-0.8,-0.8,-0.8,-0.8,-0.7,-0.6,-0.6,-0.6,-0.5,-0.4,-0.4,-0.4,-0.3,-0.3,-0.3,-0.2,-0.2,-0.2,-0.2,-0.1,-0.1,0,0,1,0.1,0.2,0.2,0.2,0.2,0.2,0.3,0.3,0.3,0.4,0.4,0.4,0.4,0.4,0.4,0.4,0.5,0.5,0.7,0.8,0.8,0.8,0.8,1.3,1.5 }, 
                new List<double>()
                    { }, 
                new List<int>() 
                    { }));*/
            using (var host = new ServiceHost(typeof (Service1)))
            {
                host.Open();
                Console.WriteLine("The service is ready at {0}", host.Description.Endpoints[0].Address);
                Console.ReadLine();
                host.Close();
            }
        }

        public static double Pirson(double[] x, double[] y)
        {
            double Xsr = x.Average();
            double Ysr = y.Average();
            double ch = 0;
            for (int i = 0; i < x.Length; i++)
            {
                ch += (x[i] - Xsr) * (y[i] - Ysr);
            }
            double zn = x.Select(a => (a - Xsr) * (a - Xsr)).Sum() * y.Select(a => (a - Ysr) * (a - Ysr)).Sum();
            zn = Math.Sqrt(zn);
            double res = ch/zn;
            return res;
        }

        private static double GaussDistr()
        {
            double v1, v2, s;
            while (true)
            {
                v1 = 2.0 * rnd() - 1.0;
                v2 = 2.0 * rnd() - 1.0;
                s = v1 * v1 + v2 * v2;
                if ((s <= 1.0) && (s > 0.0))
                {
                    double r = 0.5+(v2 * Math.Sqrt((-2.0) * Math.Log(s) / s))/5;
                    if (r < 0)
                        r = 0;
                    if (r > 1)
                        r = 1;
                    return r;
                }
            }
        }

        private static int rand_puass(double mu)
        {
            double time;
            short count;
            if (mu <= 0.0) 
                return (0);
            time = 0.0;
            count = 0;
            while (true)
            {
                time -= Math.Log(rnd()) / mu;
                if (time < 1.0)
                    count++;
                else
                    break;
            }
            return (count);
        }

        static Random rand = new Random();
        private static double rnd()
        {
            return rand.NextDouble();
        }

    }
}
