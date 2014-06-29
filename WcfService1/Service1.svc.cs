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
            students = XMLWork.ReadXMLUsers("students.xml");
            tests = XMLWork.ReadXMLTestTheme();
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
                PrintIntoFile(irt);
            }
        }

        public void PrintIntoFile(IRTTable irt)
        {
            StreamWriter sw = new StreamWriter("irt.txt");

            string s = "№  ";
            for (int i = 0; i < irt.taskCount; i++)
            {
                s += "x" + i.ToString()+"  ";
            }
            s += "Teta";
            sw.WriteLine(s);

            for (int i = 0; i < testPassage.Count; i++)
            {
                s = i.ToString()+"   ";
                for (int j = 0; j < irt.taskCount; j++)
                {
                    s += irt.table[i][j].ToString() + "   ";
                }
                s += irt.table[i].p.ToString() + " ";
                s += irt.table[i].q.ToString() + " ";
                s += irt.table[i].pq.ToString() + " ";
                s += irt.table[i].teta.ToString() + " ";
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
                s += string.Format("{0:f1} ",irt.betaList[i]);
            } 
            sw.WriteLine(s);

            sw.Close();
        }

        public List<User> GetAllUsers()
        {
            return students;
        }

    }

}
