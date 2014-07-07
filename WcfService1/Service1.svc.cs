using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.ServiceModel.Web;
using System.Text;
using System.IO;

namespace WcfService1
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service1" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select Service1.svc or Service1.svc.cs at the Solution Explorer and start debugging.
    public class Service1 : IService1
    {
        public Service1()
        {
            if (tests == null) 
                tests = XMLWork.ReadXMLTestTheme();

            /*tests[0].Tests[0].Tasks = new List<Task>();
            for (int i = 0; i < 50; i++)
            {
                tests[0].Tests[0].Tasks.Add(new Task() 
                { 
                    Answers = new List<string>(){"1","2","3","4"},
                    Difficult = GaussDistr(),
                    RightAnswer = (int)(rnd()*4),
                    Text = "none"
                });
            }*/

            if (studentGroups == null)
                studentGroups = XMLWork.ReadXMLUserGroup();
            
            /*
            students = new List<User>();
            for (int i = 0; i < 50; i++)
            {
                students.Add(new User() { Name = "Unknown_" + i.ToString() });
            } 
            SetStudentKnowledge();

            foreach (User user in students)
            {
                testPassage.Add(PassTest(user, tests[0].Tests[0]));
            }

            irt = new IRTTable(testPassage);
            PrintIntoFileExcel(irt);
            PrintPredictFileExcel(irt);*/

        }
        
        public bool Login(string username, string pass)
        {
            var user = FindUser(username);
            if (user == null)
                return false;
            if (user.CheckPass(pass))
            {
                instances.Add(this, username);
                return true;
            }
            return false;
        }

        public User FindUser(string name)
        {
            for (int j = 0; j < studentGroups.Count; j++)
            {
                for (int i = 0; i < studentGroups[j].Users.Count; i++)
                {
                    if (studentGroups[j].Users[i].Name == name)
                    {
                        return studentGroups[j].Users[i];
                    }
                }
            }
            return null;
        }

        public static Dictionary<Service1, string> instances = new Dictionary<Service1, string>();
        public static List<TestsTheme> tests = null;
        public static List<UserGroup> studentGroups = null;
        public static List<TestPassage> testPassage = new List<TestPassage>();
        public static List<IRTTable> irt;
 
        public List<TestsTheme> GetAllTests()
        {
            var dc = ServiceSecurityContext.Current;
            return new List<TestsTheme>(tests); //.Where(message => message.Receiver == dc.PrimaryIdentity.Name || message.Sender == dc.PrimaryIdentity.Name)
        }

        public void SendTestResult(TestPassage message)
        {
            testPassage.Add(message);

            if (testPassage.Count >= 2)
            {
                //irt = new IRTTable(testPassage);
                //PrintIntoFile(irt);
            }
        }

        public void PrintIntoFile(IRTTable irt)
        {
            StreamWriter sw = new StreamWriter("irt.txt");

            string s = "№  ";
            for (int i = 0; i < irt.taskCount; i++)
            {
                s += "x" + i.ToString() + "  ";
            }
            s += "Teta";
            sw.WriteLine(s);

            for (int i = 0; i < testPassage.Count; i++)
            {
                s = string.Format("{0,2} ", i);
                for (int j = 0; j < irt.taskCount; j++)
                {
                    s += irt.table[i][j].ToString() + "   ";
                }
                s += irt.table[i].p.ToString() + " ";
                s += irt.table[i].q.ToString() + " ";
                s += string.Format("{0:f2} ", irt.table[i].pq);
                s += string.Format("{0:f2} ", irt.table[i].teta) + " ";
                sw.WriteLine(s);
            }
            s = "w  ";
            for (int i = 0; i < irt.taskCount; i++)
            {
                s += string.Format("{0:f1} ", irt.wList[i]);
            }
            sw.WriteLine(s);
            s = "p  ";
            for (int i = 0; i < irt.taskCount; i++)
            {
                s += string.Format("{0:f1} ", irt.pList[i]);
            }
            sw.WriteLine(s);
            s = "b  ";
            for (int i = 0; i < irt.taskCount; i++)
            {
                s += string.Format("{0:f1} ", irt.betaList[i]);
            }
            sw.WriteLine(s);

            sw.Close();
        }
        public void PrintIntoFileExcel(IRTTable irt)
        {
            StreamWriter sw = new StreamWriter("irt.csv");

            string s = "№;";
            for (int i = 0; i < irt.taskCount; i++)
            {
                s += "x" + i.ToString() + ";";
            }
            s += "Teta";
            sw.WriteLine(s);

            for (int i = 0; i < testPassage.Count; i++)
            {
                s = string.Format("{0};", i);
                for (int j = 0; j < irt.taskCount; j++)
                {
                    s += irt.table[i][j].ToString() + ";";
                }
                s += irt.table[i].p.ToString() + ";";
                s += irt.table[i].q.ToString() + ";";
                s += string.Format("{0:f2};", irt.table[i].pq);
                s += string.Format("{0:f2};", irt.table[i].teta) + ";";
                sw.WriteLine(s);
            }
            s = "w;";
            for (int i = 0; i < irt.taskCount; i++)
            {
                s += string.Format("{0:f1};", irt.wList[i]);
            }
            sw.WriteLine(s);
            s = "p;";
            for (int i = 0; i < irt.taskCount; i++)
            {
                s += string.Format("{0:f1};", irt.pList[i]);
            }
            sw.WriteLine(s);
            s = "b;";
            for (int i = 0; i < irt.taskCount; i++)
            {
                s += string.Format("{0:f1};", irt.betaList[i]);
            }
            sw.WriteLine(s);

            sw.Close();
        }

        public void PrintPredictFileExcel(IRTTable irt)
        {
            StreamWriter sw = new StreamWriter("irt2.csv");

            string s = "№;";
            for (int i = 0; i < irt.taskCount; i++)
            {
                s += "x" + i.ToString() + ";";
            }
            s += "Teta";
            sw.WriteLine(s);
            irt.table.Sort((a, b) => a.teta == b.teta ? 0 : a.teta > b.teta ? 1 : -1);

            for (int i = 0; i < testPassage.Count; i++)
            {
                s = string.Format("{0};", i);
                for (int j = 0; j < irt.taskCount; j++)
                {
                    //s += string.Format("{0};", irt.predictTable[i][j]);
                    s += string.Format("{0:f2};", GetTeta2(irt.betaList, irt.Aj, irt.table[i].val.ToList(), j));
                }
                s += irt.table[i].p.ToString() + ";";
                s += irt.table[i].q.ToString() + ";";
                s += string.Format("{0:f2};", irt.table[i].pq);
                s += string.Format("{0:f2};", irt.table[i].teta);
                //s += string.Format("{0:f2};", GetTeta(irt.betaList, irt.Aj, irt.table[i].val.ToList()));
                s += string.Format("{0:f2};", GetTeta2(irt.betaList, irt.Aj, irt.table[i].val.ToList(), irt.taskCount));
                sw.WriteLine(s);
            }
            s = "w;";
            for (int i = 0; i < irt.taskCount; i++)
            {
                s += string.Format("{0:f1};", irt.wList[i]);
            }
            sw.WriteLine(s);
            s = "Aj;";
            for (int i = 0; i < irt.taskCount; i++)
            {
                s += string.Format("{0:f2};", irt.Aj[i]);
            }
            sw.WriteLine(s);
            s = "beta;";
            for (int i = 0; i < irt.taskCount; i++)
            {
                s += string.Format("{0:f2};", irt.betaList[i]);
            }
            sw.WriteLine(s);

            sw.Close();
        }

        public void SetStudentKnowledge()
        {
            for (int j = 0; j < studentGroups.Count; j++)
            {
                List<double> l = new List<double>();
                for (int i = 0; i < studentGroups[j].Users.Count; i++)
                {
                    l.Add(GaussDistr());
                }
                l.Sort();
                for (int i = 0; i < studentGroups[j].Users.Count; i++)
                {
                    studentGroups[j].Users[i].knowledge = l[i];
                }
            }
        }

        public TestPassage PassTest(User user, Test test)
        {
            var result = new TestPassage();
            result.User = user;
            result.Test = test;
            result.Answers = new List<int>();
            for (int i = 0; i < test.Tasks.Count; i++)
            {
                int right = test.Tasks[i].RightAnswer;
                double taskP = 0.25+(1 - test.Tasks[i].Difficult);
                if (rnd() < 0.25+user.knowledge * (taskP)*0.75)
                {
                    result.Answers.Add(right);
                }
                else
                {
                    if (right > 0)
                        result.Answers.Add(right - 1);
                    else
                        result.Answers.Add(right + 1);
                }
            }
            return result;
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
                    double r = 0.5 + (v2 * Math.Sqrt((-2.0) * Math.Log(s) / s)) / 5;
                    if (r < 0)
                        r = 0;
                    if (r > 1)
                        r = 1;
                    return r;
                }
            }
        }

        static Random rand = new Random();
        private static double rnd()
        {
            return rand.NextDouble();
        }

        static double FuncDeriv(List<double> beta, List<double> aj, List<double> right, double teta)
        {
            double c = 0.25;
            double res = 0;
            for (int i = 0; i < beta.Count; i++)
            {
                double ch = aj[i] * (1 - c) * (Math.Exp(aj[i] * (beta[i] + teta)));
                double zn = Math.Pow(c * (Math.Exp(aj[i] * (beta[i] - teta)) + 1), 2);
                if (right[i] > 0 ? true : false)
                    res += ch / zn;
                else
                    res -= ch / zn;
            }
            return res;
        }

        static double FuncDeriv2(List<double> beta, List<double> aj, List<double> right, double teta)
        {
            double c = 0.25;
            double res = 0;
            double ch = 0;
            for (int i = 0; i < beta.Count; i++)
            {
                ch += Math.Pow(IRTTable.PredictFormula(teta, beta[i], aj[i], c) - right[i], 2);
            }
            res = Math.Sqrt(ch);
            return res;
        }


        static double FuncResult(List<double> beta, List<double> aj, List<double> right, double teta, int k)
        {
            double c = 0.25;
            double res = 1;
            for (int i = 0; i < k; i++)//beta.count
            {
                if (right[i] > 0)
                    res *= IRTTable.PredictFormula(teta, beta[i], aj[i], c);
                else
                    res *= 1 - IRTTable.PredictFormula(teta, beta[i], aj[i], c);
            }
            return res;
        }

        static double Binary(double a, double b, List<double> beta, List<double> aj, List<double> right)
        {
            double x = (a + b) / 2;
            while (Math.Abs(FuncDeriv2(beta, aj, right, x)) > 0.01)
            {
                if (FuncDeriv2(beta, aj, right, x) < 0)
                    b = x;
                else
                    a = x;
                x = (a + b) / 2;
            }
            return x;
        }

        static double iterative(double a, double b, List<double> beta, List<double> aj, List<double> right)
        {
            double x = (a + b) / 2;
            double step = 0.01;
            int k = 1;
            double res = FuncDeriv2(beta, aj, right, x);
            if (res < FuncDeriv2(beta, aj, right, x + step))
                k = -1;
            while (true)
            {
                x += k * step;
                double temp = FuncDeriv2(beta, aj, right, x);
                if (temp > res)
                    break;
                res = temp;
                if (Math.Abs(x) >= 20)
                    break;
            }
            return x;
        }

        static double iterative2(double a, double b, List<double> beta, List<double> aj, List<double> right, int count)
        {
            double x = (a + b) / 2;
            double step = 0.01;
            int k = 1;
            double res = FuncResult(beta, aj, right, x, count);
            if (res > FuncResult(beta, aj, right, x + step, count))
                k = -1;
            while (true)
            {
                x += k * step;
                double temp = FuncResult(beta, aj, right, x, count);
                if (temp < res)
                    break;
                res = temp;
                if (Math.Abs(x) >= 20)
                    break;
            }
            return x;
        }

        public static double GetTeta(List<double> beta, List<double> aj, List<double> right)
        {
            return iterative(-10, 10, beta, aj, right);
        }
        public static double GetTeta2(List<double> beta, List<double> aj, List<double> right, int count)
        {
            return iterative2(-10, 10, beta, aj, right, count);
        }

        public List<UserGroup> GetAllUsers()
        {
            return studentGroups;
        }

        public IRTTable GetIRT(Test test)
        {
            return test.GetIRT();
        }
    }

}
