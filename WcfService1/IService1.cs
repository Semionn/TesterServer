using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Security.Cryptography;

namespace WcfService1
{
    [ServiceContract]
    public interface IService1
    {
        [OperationContract]
        List<TestsTheme> GetAllTests();

        [OperationContract]
        List<string> GetThemesNames();

        [OperationContract]
        int GetTestsCount(int themeNum);
        
        [OperationContract]
        Test GetTest(int themeNum, int testID);

        [OperationContract]
        List<UserGroup> GetAllUsers();

        [OperationContract]
        List<User> GetAdmins();

        [OperationContract]
        void SendTestResult(TestPassage message);

        [OperationContract]
        bool Login(string username, string pass);

        [OperationContract]
        bool LoginAdmin(string username, string pass);

        [OperationContract]
        IRTTable GetIRT(Test test);

        [OperationContract]
        bool AddTestTheme(string testThemeName);

        [OperationContract]
        bool AddTest(string testThemeName, Test test);

        [OperationContract]
        bool AddGroup(string groupName);

        [OperationContract]
        bool AddStudent(string groupName, string studentName);

        [OperationContract]
        Task GetEmptyTask();

        [OperationContract]
        bool SaveTest(Test test);

        [OperationContract]
        bool SaveAll();

        [OperationContract]
        bool Rollback();

        [OperationContract]
        Test TestCopy(Test test);

        [OperationContract]
        Task GetTaskAdaptive(TestPassage testPassage, Test test, out double teta);

        [OperationContract]
        bool RemoveTest(int testID);

        [OperationContract]
        bool RemoveTestPassage(int testID, List<int> delRows);

        [OperationContract]
        bool CheckIRTAdaptive(Test test);
    }

    [DataContract]
    public class TestPassage
    {
        private User user;
        private Test test;
        private List<int> answers = new List<int>();
        private List<int> timeAnswer = new List<int>();
        private DateTime timeBegin = DateTime.Now, timeEnd;
        private int lastTask = 0;

        public int markAccurate = 0;
        public int mark = 0;

        public bool AnswerRight(int i)
        {
            if (i < 0 || i > answers.Count-1)
                return false;
            return Answers[i] == Test.Tasks[i].RightAnswer;
        }

        [DataMember]
        public User User
        {
            get { return user; }
            set { user = value; }
        }

        [DataMember]
        public Test Test
        {
            get { return test; }
            set { test = value; }
        }

        [DataMember]
        public List<int> Answers
        {
            get { return answers; }
            set { answers = value; }
        }

        [DataMember]
        public List<int> TimeAnswer
        {
            get { return timeAnswer; }
            set { timeAnswer = value; }
        }

        [DataMember]
        public DateTime TimeBegin
        {
            get { return timeBegin; }
            set { timeBegin = value; }
        }

        [DataMember]
        public DateTime TimeEnd
        {
            get { return timeEnd; }
            set { timeEnd = value; }
        }

        [DataMember]
        public int LastTask
        {
            get { return lastTask; }
            set { lastTask = value; }
        }

        public int GetRightAnswCount()
        {
            return Test.Tasks.Where((a, i) => a.RightAnswer == Answers[i]).ToList().Count;
        }

        public void GetMark()
        {
            for (int i = 0; i < Answers.Count; i++)
            {

            }
        }
    }
    
    [DataContract]
    public class User
    {
        private string name = "User";
        public string passwordHash = "81DC9BDB52D04DC20036DBD8313ED055";

        private List<TestPassage> completedTests;
        public double knowledge = 0.5;
        [DataMember]
        public int ID = -1;

        [DataMember]
        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public bool CheckPass(string pass)
        {
            return passwordHash == GetMD5Hash(pass);
        }

        public void SetPass(string pass)
        {
            passwordHash = GetMD5Hash(pass);
        }

        private string GetMD5Hash(string s)
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] checkSum = md5.ComputeHash(Encoding.UTF8.GetBytes(s));
            string result = BitConverter.ToString(checkSum).Replace("-", String.Empty);
            return result;
        }

    }

    [DataContract]
    public class UserGroup
    {
        private string name = "Group";
        private List<User> users;

        [DataMember]
        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        [DataMember]
        public List<User> Users
        {
            get { return users; }
            set { users = value; }
        }

    }

    [DataContract]
    public class TestsTheme
    {
        private string name = "";
        private List<Test> tests = new List<Test>();

        [DataMember]
        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        [DataMember]
        public List<Test> Tests
        {
            get { return tests; }
            set { tests = value; }
        }
    }

    [DataContract]
    public class Test
    {
        [DataMember]
        public IRTTable irt = new IRTTable();
        [DataMember]
        public int ID;
        private string name = "";
        private List<Task> tasks = new List<Task>();
        private int score;
        private int timeLimitMinutes = 20;

        [DataMember]
        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        [DataMember]
        public List<Task> Tasks
        {
            get { return tasks; }
            set { tasks = value; }
        }

        [DataMember]
        public int Score
        {
            get { return score; }
            set { score = value; }
        }

        [DataMember]
        public int TimeLimit
        {
            get { return timeLimitMinutes; }
            set { timeLimitMinutes = value; }
        }
    }

    [DataContract]
    public class Task
    {
        private string text = "";
        private List<string> answers = new List<string>();
        private int rightAnswer = 0;
        private double difficult = 0;

        [DataMember]
        public string Text
        {
            get { return text; }
            set { text = value; }
        }

        [DataMember]
        public List<string> Answers
        {
            get { return answers; }
            set { answers = value; }
        }

        [DataMember]
        public int RightAnswer
        {
            get { return rightAnswer; }
            set { rightAnswer = value; }
        }

        [DataMember]
        public double Difficult
        {
            get { return difficult; }
            set { difficult = value; }
        }
    }
}