using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace AutomaticUpdate.ViewModel
{
    class Forecast
    {
        //data 有二列，第一列表示 x 值，第二列表示 y 值
        // 一元线性回归分析预测 , 返回相关系数
        public static double LinearRegression(double[,] data, out double A, out double B)
        {
            double[,] derivedData = new double[data.GetLength(0) + 1, data.GetLength(1) + 3];
            for (int i = 0; i < data.GetLength(0); i++)
            {
                derivedData[i, 0] = data[i, 0]; //X 
                derivedData[i, 1] = data[i, 1]; //Y 
                derivedData[i, 2] = data[i, 0] * data[i, 1]; //XY 
                derivedData[i, 3] = data[i, 0] * data[i, 0]; //XX 
                derivedData[i, 4] = data[i, 1] * data[i, 1]; //YY 
                derivedData[derivedData.GetLength(0) - 1, 0] += derivedData[i, 0];  //X 的累加
                derivedData[derivedData.GetLength(0) - 1, 1] += derivedData[i, 1];  //Y 的累加
                derivedData[derivedData.GetLength(0) - 1, 2] += derivedData[i, 2];  //XY 的累加
                derivedData[derivedData.GetLength(0) - 1, 3] += derivedData[i, 3];  //XX 的累加
                derivedData[derivedData.GetLength(0) - 1, 4] += derivedData[i, 4];  //YY 的累加  
            }
            double xba = derivedData[derivedData.GetLength(0) - 1, 0] / data.GetLength(0);
            double yba = derivedData[derivedData.GetLength(0) - 1, 1] / data.GetLength(0);
            double Lxx = derivedData[derivedData.GetLength(0) - 1, 3] - Math.Pow(derivedData[derivedData.GetLength(0) - 1, 0], 2) / data.GetLength(0);
            double Lyy = derivedData[derivedData.GetLength(0) - 1, 4] - Math.Pow(derivedData[derivedData.GetLength(0) - 1, 1], 2) / data.GetLength(0);
            double Lxy = derivedData[derivedData.GetLength(0) - 1, 2] - derivedData[derivedData.GetLength(0) - 1, 0] * derivedData[derivedData.GetLength(0) - 1, 1] / data.GetLength(0);
            double b = Lxy / Lxx;
            double a = yba - b * xba;
            A = a;
            B = b;
            return Lxy / Math.Sqrt(Lxx * Lyy);
        }
        public static double LinearRegression(double[,] data, double x, out double r)
        {
            r = LinearRegression(data, out double a, out double b);
            return a + b * x;
        }
        //幂回归分析预测
        public static double PowerRegression(double[,] data, out double A, out double B)
        {
            double[,] derivedData = (double[,])data.Clone();
            for (int i = 0; i < derivedData.GetLength(0); i++)
            {
                for (int j = 0; j < derivedData.GetLength(1); j++)
                {
                    derivedData[i, j] = Math.Log(derivedData[i, j]);
                }
            }
            double r;
            r = LinearRegression(derivedData, out double a, out double b);
            A = Math.Exp(a);
            B = b;
            return r;
        }
        public static double PowerRegression(double[,] data, double x, out double r)
        {
            r = PowerRegression(data, out double a, out double b);
            return a * Math.Pow(x, b);
        }
        //灰色模型法
        public static double GM(double[] data, int x, out double p, out double c)
        {
            int rowNum = data.GetLength(0);
            double[] x0 = new double[rowNum];
            double[] x1 = new double[rowNum];
            x0[0] = data[0];
            x1[0] = data[0];
            for (int i = 1; i < rowNum; i++)
            {
                x0[i] = data[i];
                x1[i] = x1[i - 1] + x0[i];
            }
            double[,] derivedData = new double[rowNum - 1, 2];
            for (int i = 0; i < rowNum - 1; i++)
            {
                derivedData[i, 0] = -0.5 * (x1[i] + x1[i + 1]);
                derivedData[i, 1] = x0[i + 1];
            }
            double a;
            double u;
            LinearRegression(derivedData, out u, out a);
            double result;
            result = (x0[0] - u / a) * (1 - Math.Exp(a)) * Math.Exp(-a * (x - 1));
            // 模型精度检验
            double[] q = new double[rowNum];
            double[] epsilon = new double[rowNum];
            double epsilonSum = 0;
            for (int i = 0; i < rowNum; i++)
            {
                double x0i = x0[i];
                double xxi = (x0[0] - u / a) * (1 - Math.Exp(a)) * Math.Exp(-a * i);
                q[i] = x0i - xxi;
                epsilon[i] = q[i] / x0i;
                epsilonSum += Math.Abs(epsilon[i]);
            }
            double epsilonAverage = epsilonSum / rowNum;
            p = 1 - epsilonAverage;
            // 后验差比值
            double x0Average;
            double qAverage;
            double x0Sum = 0;
            double qSum = 0;
            for (int i = 0; i < rowNum; i++)
            {
                x0Sum += x0[i];
                qSum += q[i];
            }

            x0Average = x0Sum / rowNum;
            qAverage = qSum / rowNum;
            double s1;
            double s2;
            double s1Sum = 0;
            double s2Sum = 0;
            for (int i = 0; i < rowNum; i++)
            {
                s1Sum += Math.Pow(x0[i] - x0Average, 2);
                s2Sum += Math.Pow(q[i] - qAverage, 2);
            }
            s1 = s1Sum / rowNum;
            s2 = s2Sum / rowNum;
            c = s2 / s1;
            return result;
        }
        //加权移动平均法
        public static double WeightedMovingAverage(List<double> data, int x)
        {
            int count = data.Count;
            double s = 0.0;
            for (int i = 0; i < x; i++)
            {
                s = 0.0;
                for (int j = 0; j < count; j++)
                {
                    s += (j + 1) * data[j];
                }
                s /= (count * (count + 1) / 2);
                for (int j = 0; j < count - 1; j++)
                {
                    data[j] = data[j + 1];
                }
                data[count - 1] = s;
            }
            return s;
        }
    }
}
