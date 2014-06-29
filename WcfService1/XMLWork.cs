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
        public static List<User> ReadXMLUsers(string filename)
        {
            XmlSerializer ser = new XmlSerializer(typeof(List<User>));
            FileStream fs = new FileStream(filename, FileMode.Open);
            List<User> users = (List<User>)ser.Deserialize(fs);
            fs.Close();
            return users;
        }

        public static void WriteXMLUsers(string filename, List<User> users)
        {
            XmlSerializer ser = new XmlSerializer(typeof(List<User>));
            TextWriter writer = new StreamWriter(filename);
            ser.Serialize(writer, users);
            writer.Close();
        }

        public static void WriteXMLTestTheme(List<TestsTheme> themes)
        {
            string path = "Тесты";
            Directory.CreateDirectory(path);
            for (int i = 0; i < themes.Count; i++)
            {
                for (int j = 0; j < themes[i].Tests.Count; j++)
                {
                    string s = path + "\\" + themes[i].Name;
                    Directory.CreateDirectory(s);
                    WriteXMLTest(s + "\\" + themes[i].Tests[j].Name + ".xml", themes[i].Tests[j]);
                }
            }
        }

        public static void WriteXMLTest(string filename, Test test)
        {
            XmlSerializer ser = new XmlSerializer(typeof(Test));
            TextWriter writer = new StreamWriter(filename);
            ser.Serialize(writer, test);
            writer.Close();
        }

        public static List<TestsTheme> ReadXMLTestTheme()
        {
            List<TestsTheme> res = new List<TestsTheme>();

            string path = "Тесты";
            var themeDirs = Directory.GetDirectories(path);
            for (int i = 0; i < themeDirs.Count(); i++)
            {
                res.Add(new TestsTheme()
                {
                    Name = themeDirs[i].Substring(path.Length + 1),
                    Tests = new List<Test>()
                });

                var testDirs = Directory.GetFiles(themeDirs[i]);
                for (int j = 0; j < testDirs.Count(); j++)
                {
                    res[i].Tests.Add(ReadXMLTest(testDirs[j]));
                }
            }
            return res;
        }

        public static Test ReadXMLTest(string filename)
        {
            XmlSerializer ser = new XmlSerializer(typeof(Test));
            FileStream fs = new FileStream(filename, FileMode.Open);
            Test test = (Test)ser.Deserialize(fs);
            fs.Close();
            return test;
        }
    }
}