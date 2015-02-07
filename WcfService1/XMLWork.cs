using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Serialization;
using System.Xml;
using System.IO;

namespace WcfService1
{
    public static class XMLWork
    {
        public const string TEST_PATH = "Тесты";
        public const string GROUP_PATH = "Группы";
        public const string DELETED_PATH = "Удаленное";

        public static List<User> ReadUsers(string filename)
        {
            XmlSerializer ser = new XmlSerializer(typeof(List<User>));
            FileStream fs = new FileStream(filename, FileMode.Open);
            List<User> users = (List<User>)ser.Deserialize(fs);
            fs.Close();
            return users;
        }

        public static void WriteUsers(string filename, List<User> users)
        {
            XmlSerializer ser = new XmlSerializer(typeof(List<User>));
            TextWriter writer = new StreamWriter(filename);
            ser.Serialize(writer, users);
            writer.Close();
        }

        public static void WriteTestTheme(List<TestsTheme> themes)
        {
            string path = TEST_PATH;
            Directory.CreateDirectory(path);
            for (int i = 0; i < themes.Count; i++)
            {
                for (int j = 0; j < themes[i].Tests.Count; j++)
                {
                    string s = path + "\\" + themes[i].Name;
                    Directory.CreateDirectory(s);
                    WriteTest(s + "\\" + themes[i].Tests[j].Name + ".xml", themes[i].Tests[j]);
                }
            }
        }

        public static void WriteUserGroup(List<UserGroup> groups)
        {
            string path = GROUP_PATH;
            Directory.CreateDirectory(path);
            for (int i = 0; i < groups.Count; i++)
            {
                string s = path + "\\" + groups[i].Name + ".xml";
                WriteUsers(s, groups[i].Users);
            }
        }
        public static List<UserGroup> ReadUserGroup()
        {
            List<UserGroup> res = new List<UserGroup>();

            string path = GROUP_PATH;
            var groupsDirs = Directory.GetFiles(path);
            for (int i = 0; i < groupsDirs.Count(); i++)
            {
                res.Add(new UserGroup()
                {
                    Name = Path.GetFileNameWithoutExtension(groupsDirs[i]),
                    Users = new List<User>()
                });
                res[i].Users.AddRange(ReadUsers(groupsDirs[i]));
            }
            return res;
        }

        public static void WriteTest(string filename, Test test)
        {
            XmlSerializer ser = new XmlSerializer(typeof(Test));
            TextWriter writer = new StreamWriter(filename);
            ser.Serialize(writer, test);
            writer.Close();
        }

        public static List<TestsTheme> ReadTestTheme()
        {
            List<TestsTheme> res = new List<TestsTheme>();

            string path = TEST_PATH;
            var themeDirs = Directory.GetDirectories(path);
            for (int i = 0; i < themeDirs.Count(); i++)
            {
                res.Add(new TestsTheme()
                {
                    Name = Path.GetFileNameWithoutExtension(themeDirs[i]),
                    Tests = new List<Test>()
                });

                var testDirs = Directory.GetFiles(themeDirs[i]);
                for (int j = 0; j < testDirs.Count(); j++)
                {
                    res[i].Tests.Add(ReadTest(testDirs[j]));
                }
            }
            return res;
        }

        public static Test ReadTest(string filename)
        {
            XmlSerializer ser = new XmlSerializer(typeof(Test));
            FileStream fs = new FileStream(filename, FileMode.Open);
            Test test = (Test)ser.Deserialize(fs);
            fs.Close();
            return test;
        }

        public static void WriteIRT(string filename, IRTTable irt)
        {
            XmlSerializer ser = new XmlSerializer(typeof(IRTTable));
            TextWriter writer = new StreamWriter(filename);
            ser.Serialize(writer, irt);
            writer.Close();
        }

        public static IRTTable ReadIRT(string filename)
        {
            XmlSerializer ser = new XmlSerializer(typeof(IRTTable));
            FileStream fs = new FileStream(filename, FileMode.Open);
            IRTTable irt = (IRTTable)ser.Deserialize(fs);
            fs.Close();
            return irt;
        }

        public static void RemoveFile(string filename)
        {
            File.Move(filename, DELETED_PATH + "\\" + Path.GetFileName(filename));
        }
    }
}