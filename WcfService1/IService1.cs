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
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IService1" in both code and config file together.
    [ServiceContract]
    public interface IService1
    {
        [OperationContract]
        List<TestsTheme> GetAllTests();
        
        [OperationContract]
        List<UserGroup> GetAllUsers();

        [OperationContract]
        void SendTestResult(TestPassage message);

        [OperationContract]
        bool Login(string username, string pass);

        [OperationContract]
        IRTTable GetIRT(Test test);
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
            if (i < 0 || i > answers.Count)
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

    // Use a data contract as illustrated in the sample below to add composite types to service operations.
    [DataContract]
    public class CompositeType
    {
        private bool boolValue = true;
        private string stringValue = "Hello ";

        [DataMember]
        public bool BoolValue
        {
            get { return boolValue; }
            set { boolValue = value; }
        }

        [DataMember]
        public string StringValue
        {
            get { return stringValue; }
            set { stringValue = value; }
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
        private string name = "";
        private List<Task> tasks = new List<Task>();
        private IRTTable irt = null;

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
        
        public void SetIRT(IRTTable value)
        {
            irt = value;
        }

        public IRTTable GetIRT()
        {
            return irt;
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