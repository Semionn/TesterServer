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
        [DataMember]
        public List<double> rList;
        [DataMember]
        public List<double> wList;
        [DataMember]
        public List<double> pList;
        [DataMember]
        public List<double> qList;
        [DataMember]
        public List<double> pqList;
        [DataMember]
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

        public IRTTable()
        {
        }

        public void AddTestPassage(TestPassage testPassage)
        {
            if (table == null)
                table = new List<IRTRow>();
            taskCount = testPassage.Answers.Count;

            if (rList == null | rList != null & rList.Count==0)
            {                
                rList = new List<double>(new double[taskCount]);
                wList = new List<double>(new double[taskCount]);
                pList = new List<double>(new double[taskCount]);
                qList = new List<double>(new double[taskCount]);
                pqList = new List<double>(new double[taskCount]);
                qpList = new List<double>(new double[taskCount]);
                betaList = new List<double>(new double[taskCount]);
            }
            table.Add(new IRTRow(this, testPassage));

            SetList(ref wList, (a, i) => table.Count - rList[i]);
            SetList(ref pList, (a, i) => rList[i] / table.Count);
            SetList(ref qList, (a, i) => wList[i] / table.Count);
            SetList(ref pqList, (a, i) => pList[i] * qList[i]);
            SetList(ref qpList, (a, i) => qList[i] / pList[i]);
            SetList(ref betaList, (a, i) => Math.Log(qpList[i]));

            Aj = GetRyj().Select(a => a / Math.Sqrt(1 - a * a)).ToList();
            Predict();
        }

        public void DelTestPassage(List<int> delRows)
        {
            if (table == null)
            {
                table = new List<IRTRow>();
                return;
            }

            for (int i = delRows.Count - 1; i >= 0; i--)
            {
                table.RemoveAt(i);
            }
            if (table.Count == 0)
                return;

            SetList(ref wList, (a, i) => table.Count - rList[i]);
            SetList(ref pList, (a, i) => rList[i] / table.Count);
            SetList(ref qList, (a, i) => wList[i] / table.Count);
            SetList(ref pqList, (a, i) => pList[i] * qList[i]);
            SetList(ref qpList, (a, i) => qList[i] / pList[i]);
            SetList(ref betaList, (a, i) => Math.Log(qpList[i]));

            Aj = GetRyj().Select(a => a / Math.Sqrt(1 - a * a)).ToList();
            Predict();
        }

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
            for (int i = 0; i < table.Count; i++)
            {
                list.Add(table[i].val[j]);
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
            predictTable = new List<List<double>>();
            for (int i = 0; i < table.Count; i++)
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

            double[] Y = new double[table.Count];
            for (int i = 0; i < table.Count; i++)
            {
                Y[i] = table[i].Y;
            }
            for (int i = 0; i < taskCount; i++)
            {
                double[] x = new double[table.Count];
                for (int j = 0; j < table.Count; j++)
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

        public static double GetTeta(TestPassage testPassage, Test test)
        {
            var irt = test.irt;
            return iterative2(-20, 20, irt.betaList, irt.Aj, testPassage.Answers.Select((a,i) => testPassage.AnswerRight(i)?1.0:0).ToList(), testPassage.Answers.Count);
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


    }

    [DataContract]
    public class IRTRow
    {
        private Test test;
        private IRTTable mainTable;
        [DataMember]
        public double[] val;
        [DataMember]
        public double sum;

        public double Y;
        public double p;
        public double q;
        public double pq;
        [DataMember]
        public double teta;

        [DataMember]
        public int userID;

        public IRTRow(IRTTable table,TestPassage testPassage)
        {
            test = testPassage.Test;
            val = new double[table.taskCount];
            this.mainTable = table;
            userID = testPassage.User.ID;

            sum = 0;
            for (int j = 0; j < table.taskCount; j++)
            {
                val[j] = testPassage.AnswerRight(j) ? 1 : 0;
                sum += val[j];
                table.rList[j] += val[j];
            }
            CalcKoeff();
        }

        public IRTRow()
        {
            userID = -1;
        }

        public void CalcKoeff()
        {
            Y = val.Count(a => a > 0);
            p = Y / mainTable.taskCount;
            q = (mainTable.taskCount - Y) / mainTable.taskCount;
            pq = p / q;
            teta = Math.Log(pq);
        }

        public void SetMainTable(IRTTable table)
        {
            mainTable = table;
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