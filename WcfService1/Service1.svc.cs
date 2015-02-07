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
    public class Service1 : IService1
    {
        public Service1()
        {
            if (testThemes == null)
                testThemes = XMLWork.ReadTestTheme();

            if (studentGroups == null)
                studentGroups = XMLWork.ReadUserGroup();

            if (admins == null)
                admins = XMLWork.ReadUsers("admins\\info.xml");
            
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

        public bool LoginAdmin(string username, string pass)
        {
            var user = admins.FirstOrDefault(a => a.Name == username);
            if (user == null)
                return false;
            if (user.CheckPass(pass))
            {
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
        public static List<TestsTheme> testThemes = null;
        public static List<UserGroup> studentGroups = null;
        public static List<User> admins = null;
        public static List<TestPassage> testPassage = new List<TestPassage>();
        public static List<IRTTable> irt = new List<IRTTable>();

        public List<TestsTheme> GetAllTests()
        {
            return testThemes;
        }

        public List<string> GetThemesNames()
        {
            return testThemes.Select(x => x.Name).ToList();
        }

        public int GetTestsCount(int themeNum)
        {
            if (themeNum > testThemes.Count - 1 || themeNum < 0)
                return 0;
            return testThemes[themeNum].Tests.Count;
        }

        public Test GetTest(int themeNum, int testID)
        {
            if (themeNum > testThemes.Count - 1 || themeNum < 0)
                return null;
            if (testID > testThemes[themeNum].Tests.Count - 1 || testID < 0)
                return null;
            return testThemes[themeNum].Tests[testID];
        }

        public void SendTestResult(TestPassage message)
        {
            testPassage.Add(message);
            if (testPassage.Count >= 1)
            {
                for (int j = 0; j < testThemes.Count; j++)
                {
                    TestsTheme t = testThemes[j];
                    for (int i = 0; i < t.Tests.Count; i++)
                    {
                        if (t.Tests[i].ID == message.Test.ID)
                        {
                            if (t.Tests[i].irt == null)
                                t.Tests[i].irt = new IRTTable();
                            t.Tests[i].irt.AddTestPassage(message);
                        }
                    }
                }
                irt.Add(new IRTTable(testPassage));
                XMLWork.WriteIRT("irt.xml", irt[0]);
                XMLWork.WriteTestTheme(testThemes);
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
                    s += string.Format("{0:f2};", IRTTable.GetTeta2(irt.betaList, irt.Aj, irt.table[i].val.ToList(), j));
                }
                s += irt.table[i].p.ToString() + ";";
                s += irt.table[i].q.ToString() + ";";
                s += string.Format("{0:f2};", irt.table[i].pq);
                s += string.Format("{0:f2};", irt.table[i].teta);
                s += string.Format("{0:f2};", IRTTable.GetTeta2(irt.betaList, irt.Aj, irt.table[i].val.ToList(), irt.taskCount));
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
                double taskP = 0.25 + (1 - test.Tasks[i].Difficult);
                if (rnd() < 0.25 + user.knowledge * (taskP) * 0.75)
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

        public List<UserGroup> GetAllUsers()
        {
            return studentGroups;
        }

        public List<User> GetAdmins()
        {
            return admins;
        }

        public IRTTable GetIRT(Test test)
        {
            return test.irt;
        }

        public bool AddTestTheme(string testThemeName)
        {
            if (testThemes == null)
            {
                testThemes = new List<TestsTheme>();
            }
            if (testThemes.FirstOrDefault(a => a.Name == testThemeName) != null)
                return false;
            testThemes.Add(new TestsTheme() { Name = testThemeName, Tests = new List<Test>() });
            return true;
        }


        public bool AddTest(string testThemeName, Test test)
        {
            TestsTheme t = testThemes.FirstOrDefault(a => a.Name == testThemeName);
            if (t == null)
                return false;
            int testID = testThemes.Max(a => a.Tests.Count > 0 ? a.Tests.Max(b => b.ID) : 0) + 1;
            test.ID = testID;
            t.Tests.Add(test);
            return true;
        }


        public Task GetEmptyTask()
        {
            var t = new Task()
            {
                Text = "",
                Answers = new List<string>()
            };

            for (int i = 0; i < 4; i++)
            {
                t.Answers.Add("Вариант " + (i + 1).ToString());
            }
            return t;
        }

        public bool SaveTest(Test test)
        {
            for (int j = 0; j < testThemes.Count; j++)
            {
                TestsTheme t = testThemes[j];
                for (int i = 0; i < t.Tests.Count; i++)
                {
                    if (t.Tests[i].ID == test.ID)
                    {
                        t.Tests[i] = test;
                        return true;
                    }
                }
            }
            return false;
        }

        public bool SaveAll()
        {
            try
            {
                XMLWork.WriteTestTheme(testThemes);
                XMLWork.WriteUserGroup(studentGroups);
                //XMLWork.WriteIRT("irt.xml", irt[0]);
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        public bool Rollback()
        {
            try
            {
                testThemes = XMLWork.ReadTestTheme();
                studentGroups = XMLWork.ReadUserGroup();
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }


        public Test TestCopy(Test test)
        {
            return test;
        }


        public Task GetTaskAdaptive(TestPassage testPassage, Test test, out double teta)
        {
            teta = 0;
            if (testPassage.Test.Tasks.Count != 0)
            {
                teta = IRTTable.GetTeta(testPassage, test);
                if (teta == 20)
                    teta = testPassage.Test.Tasks.Count;

                if (testPassage.Test.Tasks.Count >= test.Tasks.Count)
                    return null;

                if (testPassage.Test.Tasks.Count >= 20)
                    return null;
            }
            int taskNum = -1;
            var testIRT = test.irt;
            if (test.irt == null)
                return null;
            if (test.irt.betaList == null)
                return null;
            for (int i = 0; i < test.Tasks.Count; i++)
            {
                bool finded = false;
                for (int j = 0; j < testPassage.Test.Tasks.Count; j++)
                {
                    if (test.Tasks[i].Text == testPassage.Test.Tasks[j].Text)
                    {
                        finded = true;
                        break;
                    }
                }
                if (!finded)
                    if (taskNum == -1 || Math.Abs(testIRT.betaList[i] - teta) < Math.Abs(testIRT.betaList[i] - testIRT.betaList[taskNum]))
                        taskNum = i;
            }
            if (taskNum == -1)
                return null;
            return test.Tasks[taskNum];
        }


        public bool RemoveTest(int testID)
        {
            for (int j = 0; j < testThemes.Count; j++)
            {
                TestsTheme t = testThemes[j];
                for (int i = 0; i < t.Tests.Count; i++)
                {
                    if (t.Tests[i].ID == testID)
                    {
                        XMLWork.RemoveFile(XMLWork.TEST_PATH + "\\" + t.Name + "\\" + t.Tests[i].Name + ".xml");
                        t.Tests.RemoveAt(i);
                        return true;
                    }
                }
            }
            return false;
        }



        public bool AddGroup(string groupName)
        {
            try
            {
                studentGroups.Add(new UserGroup() { Name = groupName, Users = new List<User>() });
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        public bool AddStudent(string groupName, string studentName)
        {
            var t = studentGroups.FirstOrDefault(a => a.Name == groupName);
            if (t == null)
                return false;
            int studID = studentGroups.Max(b => b.Users.Count > 0 ? b.Users.Max(c => c.ID) : 0) + 1;
            try
            {
                t.Users.Add(new User() { Name = studentName, ID = studID });
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }


        public bool RemoveTestPassage(int testID, List<int> delRows)
        {
            for (int j = 0; j < testThemes.Count; j++)
            {
                TestsTheme t = testThemes[j];
                for (int i = 0; i < t.Tests.Count; i++)
                {
                    if (t.Tests[i].ID == testID)
                    {
                        t.Tests[i].irt.DelTestPassage(delRows);
                        return true;
                    }
                }
            }
            return false;
        }


        public bool CheckIRTAdaptive(Test test)
        {
            if (test.irt == null)
                test.irt = GetIRT(test);

            bool enoughTasks = true;
            bool enoughPass = true;

            if (test.irt.table.Count < (test.Tasks.Count > 25 ? 25 : test.irt.taskCount))
            {
                enoughPass = false;
            }
            else
            {
                int maxPassedTaskNum = 0;
                if (test.Tasks.Count > 25)
                {
                    for (int i = 0; i < test.irt.taskCount; i++)
                    {
                        bool b = false;
                        for (int j = 0; j < test.irt.table[0].val.Length; j++)
                        {
                            if (test.irt.table[0].val[j] == 1)
                            {
                                b = true;
                                break;
                            }
                        }

                        if (b)
                            maxPassedTaskNum = i;
                    }
                    if (maxPassedTaskNum < 25)
                        enoughTasks = false;
                }
            }

            return enoughTasks && enoughPass;
        }
    }

}
