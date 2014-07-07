using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace WcfService1
{
    [DataContract]
    public class IRTTable
    {
        [DataMember]
        public List<IRTRow> table;
        List<TestPassage> rows;
        public List<double> rList;
        public List<double> wList;
        public List<double> pList;
        public List<double> qList;
        public List<double> pqList;
        public List<double> qpList;
        [DataMember]
        public List<double> betaList;
        [DataMember]
        public List<double> Aj;
        [DataMember]
        public int taskCount = 0;
        [DataMember]
        public List<List<double>> predictTable = new List<List<double>>();

        public IRTTable(List<TestPassage> _columns)
        {
            rows = _columns;
            table = new List<IRTRow>();
            taskCount = rows[0].Answers.Count;
            rList = new List<double>(new double[taskCount]);

            InitTable();
            Reduction();

            wList = new List<double>(new double[taskCount]);
            pList = new List<double>(new double[taskCount]);
            qList = new List<double>(new double[taskCount]);
            pqList = new List<double>(new double[taskCount]);
            qpList = new List<double>(new double[taskCount]);
            betaList = new List<double>(new double[taskCount]);

            SetList(ref wList, (a, i) => rows.Count - rList[i]);
            SetList(ref pList, (a, i) => rList[i] / rows.Count);
            SetList(ref qList, (a, i) => wList[i] / rows.Count);
            SetList(ref pqList, (a, i) => pList[i] * qList[i]);
            SetList(ref qpList, (a, i) => qList[i] / pList[i]);
            SetList(ref betaList, (a, i) => Math.Log(qpList[i]));

            Aj = GetRyj().Select(a => a / Math.Sqrt(1 - a * a)).ToList();
            Predict();
        }

        public Test test { get { return rows[0].Test; } }

        public void InitTable()
        {
            for (int i = 0; i < rows.Count; i++)
            {
                table.Add(new IRTRow(this, rows[i]));
            }
        }

        public void Reduction()
        {
            for (int i = taskCount-1; i > 0; i--)
            {
                if (CheckColumn(i) != -1)
                {
                    rList.RemoveAt(i);
                    for (int z = 0; z < table.Count; z++)
                    {
                        for (int j = i; j < taskCount - 1; j++)
                        {
                            table[z].val[j] = table[z].val[j + 1];
                        }
                        Array.Resize(ref table[z].val, taskCount);
                    }
                    taskCount--;
                }
            }
        }

        public int CheckColumn(int j)
        {
            List<double> list = new List<double>();
            for (int i = 0; i < taskCount; i++)
            {
                list.Add(table[j].val[i]);
            }
            if (list.All(a => a == 0))
                return 0;
            if (list.All(a => a == 1))
                return 1;
            return -1;
        }

        public void SetList(ref List<double> list, Func<double, int, double> func)
        {
            list = list.Select(func).ToList();
        }

        public void Predict()
        {
            for (int i = 0; i < rows.Count; i++)
            {
                predictTable.Add(new List<double>());
                for (int j = 0; j < taskCount; j++)
                {
                    predictTable[i].Add(PredictFormula(table[i].teta, betaList[j], Aj[j], 0.25));
                }
            }
        }

        public static double PredictFormula(double teta, double beta, double aj, double c)
        {
            //return c + (1 - c) / (1 + Math.Exp(-aj * (teta - beta)));
            //return 1 / (1 + Math.Exp(-aj * (teta - beta)));
            return 1/(1 + Math.Exp(beta - teta));
            //return c+(1-c)/(1 + Math.Exp(beta - teta));
        }

        public List<double> GetRyj()
        {
            List<double> res = new List<double>();

            double[] Y = new double[rows.Count];
            for (int i = 0; i < rows.Count; i++)
            {
                Y[i] = table[i].Y;
            }
            for (int i = 0; i < taskCount; i++)
            {
                double[] x = new double[rows.Count];
                for (int j = 0; j < rows.Count; j++)
                {
                    x[j] = table[j].val[i];
                }

                res.Add(Pirson(x, Y));
            }
            return res;

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
            double res = ch / zn;
            return res;
        }

    }

    [DataContract]
    public class IRTRow
    {
        public Test test;
        public IRTTable mainTable;
        public double[] val;
        [DataMember]
        public double sum;

        public double Y;
        public double p;
        public double q;
        public double pq;
        [DataMember]
        public double teta;

        public IRTRow(IRTTable table,TestPassage testPassage)
        {
            test = table.test;
            val = new double[table.taskCount];
            this.mainTable = table;

            sum = 0;
            for (int j = 0; j < table.taskCount; j++)
            {
                val[j] = testPassage.AnswerRight(j) ? 1 : 0;
                sum += val[j];
                table.rList[j] += val[j];
            }
            CalcKoeff();
        }

        public void CalcKoeff()
        {
            Y = val.Count(a => a > 0);
            p = Y / mainTable.taskCount;
            q = (mainTable.taskCount - Y) / mainTable.taskCount;
            pq = p / q;
            teta = Math.Log(pq);
        }

        public double this[int i]
        {
            get 
            {
                return val[i];
            }
            set 
            {
                val[i] = value;
            }
        }
    }
}