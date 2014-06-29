using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WcfService1
{
    public class IRTTable
    {
        public List<IRTRow> table;
        public List<TestPassage> rows;
        public List<double> rList;
        public List<double> wList;
        public List<double> pList;
        public List<double> qList;
        public List<double> pqList;
        public List<double> qpList;
        public List<double> betaList;
        public List<double> Aj;
        public int taskCount = 0;
        public List<List<double>> predictTable = new List<List<double>>();

        public IRTTable(List<TestPassage> _columns)
        {
            rows = _columns;
            table = new List<IRTRow>();
            taskCount = rows[0].Answers.Count;
            rList = new List<double>(new double[taskCount]);
            wList = new List<double>(new double[taskCount]);
            pList = new List<double>(new double[taskCount]);
            qList = new List<double>(new double[taskCount]);
            pqList = new List<double>(new double[taskCount]);
            qpList = new List<double>(new double[taskCount]);
            betaList = new List<double>(new double[taskCount]);

            InitTable();

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
                    predictTable[i].Add(PredictFormula(table[i].teta, betaList[j], Aj[j], 0));
                }
            }
        }

        public double PredictFormula(double teta, double beta, double aj, double c)
        {
            return c + (1 - c) / (1 + Math.Exp(-aj * (teta - beta)));
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

    public class IRTRow
    {
        public Test test;
        public IRTTable mainTable;
        public double[] val;
        public double sum;

        public double Y;
        public double p;
        public double q;
        public double pq;
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