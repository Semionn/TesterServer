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

            WriteXMLTestTheme(test);*/

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

    }
}
