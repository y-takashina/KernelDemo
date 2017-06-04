using System;
using System.Collections.Generic;
using System.Linq;
using Accord.Math;

namespace KernelDemo
{
    public static class Kernel
    {
        public static double PorynomialKernel(double x1, double x2) => Math.Pow(x1 * x2 + 1, 3);
        public static double LinearKernel(double x1, double x2) => x1 * x2;
        public static double SigmoidKernel(double x1, double x2) => 1.0 / (1 + Math.Exp(-1.0 * x1 * x2));
        public static double GaussianKernel(double x1, double x2) => Math.Exp(-0.5 / 0.25 * (x1 - x2) * (x1 - x2));


        public static double[,] CalcCMat(IEnumerable<double> data, Func<double, double, double> kernel, double alpha, double beta)
        {
            var gramMatrix = data.Select(x1 => data.Select(x2 => kernel(x1, x2)).ToArray()).ToArray().ToMatrix();
            return gramMatrix.Divide(alpha).Add(Matrix.Identity(data.Count()).Divide(beta));
        }

        public static double[] CalcCVec(IEnumerable<double> data, double newX, Func<double, double, double> kernel, double alpha)
        {
            return data.Select(x => kernel(x, newX)).ToArray().Divide(alpha);
        }

        public static double CalcCSca(double newX, Func<double, double, double> kernel, double alpha, double beta)
        {
            return kernel(newX, newX) / alpha + 1.0 / beta;
        }

        public static (double mean, double deviation) Predict((double x, double y)[] observations, double newX, Func<double, double, double> kernel, double alpha, double beta)
        {
            var x = observations.Select(t => t.x).ToArray();
            var y = observations.Select(t => t.y).ToArray();
            var cMat = CalcCMat(x, kernel, alpha, beta);
            var cVec = CalcCVec(x, newX, kernel, alpha);
            var cSca = CalcCSca(newX, kernel, alpha, beta);
            var cMatInv = cMat.Inverse();
            var mean = cVec.Dot(cMatInv).Dot(y);
            var dev = cSca - cVec.Dot(cMatInv).Dot(cVec);
            return (mean, dev);
        }
    }
}