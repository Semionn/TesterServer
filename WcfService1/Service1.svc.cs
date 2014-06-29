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

            tests = XMLWork.ReadXMLTestTheme();

            tests[0].Tests[0].Tasks = new List<Task>();
            for (int i = 0; i < 50; i++)
            {
                tests[0].Tests[0].Tasks.Add(new Task() 
                { 
                    Answers = new List<string>(){"1","2","3","4"},
                    Difficult = 0.25+GaussDistr()/2,
                    RightAnswer = (int)(rnd()*4),
                    Text = "none"
                });
            }

            //students = XMLWork.ReadXMLUsers("students.xml");
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
            PrintPredictFileExcel(irt);

        }
        

        public string GetData(int value)
        {
            return string.Format("You entered: {0}", value);
        }

        /*public CompositeType GetDataUsingDataContract(CompositeType composite)
        {
            if (composite == null)
            {
                throw new ArgumentNullException("composite");
            }
            if (composite.BoolValue)
            {
                composite.StringValue += "Suffix";
            }
            return composite;
        }*/

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
            for (int i = 0; i < students.Count; i++)
            {
                if (students[i].Name == name)
                {
                    return students[i];
                }
            }
            return null;
        }

        public static Dictionary<Service1, string> instances = new Dictionary<Service1, string>();
        public static List<TestsTheme> tests = new List<TestsTheme>();
        private List<User> students = new List<User>();
        private List<TestPassage> testPassage = new List<TestPassage>();
        private IRTTable irt;
 
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
                irt = new IRTTable(testPassage);
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

            for (int i = 0; i < testPassage.Count; i++)
            {
                s = string.Format("{0};", i);
                for (int j = 0; j < irt.taskCount; j++)
                {
                    s += string.Format("{0};", irt.predictTable[i][j]);
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

        public void SetStudentKnowledge()
        {
            List<double> l = new List<double>();
            for (int i = 0; i < students.Count; i++)
            {
                l.Add(GaussDistr());
            }
            l.Sort();
            for (int i = 0; i < students.Count; i++)
            {
                students[i].knowledge = l[i];
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
                if (rnd() < user.knowledge * (1 - test.Tasks[i].Difficult * test.Tasks[i].Difficult))
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

        public List<User> GetAllUsers()
        {
            return students;
        }

    }

}
