// Copyright (c) 2025 Alexander Sinapov | Simeon Petkov
// All rights reserved.
// This code is proprietary and confidential.  
// Unauthorized copying, modification, distribution, or use is strictly prohibited.

using System;
using System.Numerics;

namespace Aesclea_Back_End_.AIModel.NeuralODE
{
    /// <summary>
    /// Fast Fourier Transform implementation for attention optimization
    /// y = F⁻¹(F(K) ⋅ F(V))
    /// </summary>
    public static class FFT
    {
        /// <summary>
        /// Compute FFT using Cooley-Tukey algorithm
        /// </summary>
        public static Complex[] Forward(Complex[] x)
        {
            int n = x.Length;
            
            // Base case
            if (n <= 1) return x;
            
            // Ensure power of 2
            if ((n & (n - 1)) != 0)
            {
                // Pad to next power of 2
                int nextPow2 = 1;
                while (nextPow2 < n) nextPow2 <<= 1;
                Array.Resize(ref x, nextPow2);
                n = nextPow2;
            }
            
            return FFTRecursive(x);
        }

        /// <summary>
        /// Compute inverse FFT
        /// </summary>
        public static Complex[] Inverse(Complex[] X)
        {
            int n = X.Length;
            
            // Conjugate
            Complex[] conjugated = new Complex[n];
            for (int i = 0; i < n; i++)
                conjugated[i] = Complex.Conjugate(X[i]);
            
            // Forward FFT
            Complex[] result = Forward(conjugated);
            
            // Conjugate and scale
            for (int i = 0; i < n; i++)
                result[i] = Complex.Conjugate(result[i]) / n;
            
            return result;
        }

        private static Complex[] FFTRecursive(Complex[] x)
        {
            int n = x.Length;
            if (n <= 1) return x;
            
            // Divide
            Complex[] even = new Complex[n / 2];
            Complex[] odd = new Complex[n / 2];
            
            for (int i = 0; i < n / 2; i++)
            {
                even[i] = x[2 * i];
                odd[i] = x[2 * i + 1];
            }
            
            // Conquer
            Complex[] evenFFT = FFTRecursive(even);
            Complex[] oddFFT = FFTRecursive(odd);
            
            // Combine
            Complex[] result = new Complex[n];
            for (int k = 0; k < n / 2; k++)
            {
                double angle = -2 * Math.PI * k / n;
                Complex twiddle = new Complex(Math.Cos(angle), Math.Sin(angle));
                Complex t = twiddle * oddFFT[k];
                
                result[k] = evenFFT[k] + t;
                result[k + n / 2] = evenFFT[k] - t;
            }
            
            return result;
        }

        /// <summary>
        /// Convert real array to complex array
        /// </summary>
        public static Complex[] RealToComplex(double[] real)
        {
            Complex[] complex = new Complex[real.Length];
            for (int i = 0; i < real.Length; i++)
                complex[i] = new Complex(real[i], 0);
            return complex;
        }

        /// <summary>
        /// Extract real part from complex array
        /// </summary>
        public static double[] ComplexToReal(Complex[] complex)
        {
            double[] real = new double[complex.Length];
            for (int i = 0; i < complex.Length; i++)
                real[i] = complex[i].Real;
            return real;
        }

        /// <summary>
        /// FFT-based convolution (much faster than direct integration for long sequences)
        /// </summary>
        public static double[] Convolve(double[] signal, double[] kernel)
        {
            int n = signal.Length + kernel.Length - 1;
            
            // Pad to power of 2
            int paddedSize = 1;
            while (paddedSize < n) paddedSize <<= 1;
            
            // Pad arrays
            Array.Resize(ref signal, paddedSize);
            Array.Resize(ref kernel, paddedSize);
            
            // Convert to complex
            Complex[] signalComplex = RealToComplex(signal);
            Complex[] kernelComplex = RealToComplex(kernel);
            
            // FFT both
            Complex[] signalFFT = Forward(signalComplex);
            Complex[] kernelFFT = Forward(kernelComplex);
            
            // Pointwise multiply
            Complex[] productFFT = new Complex[paddedSize];
            for (int i = 0; i < paddedSize; i++)
                productFFT[i] = signalFFT[i] * kernelFFT[i];
            
            // Inverse FFT
            Complex[] result = Inverse(productFFT);
            
            return ComplexToReal(result);
        }
    }

    /// <summary>
    /// Integral attention operator with FFT optimization
    /// y(t) = ∫K(t,s)V(s)ds
    /// </summary>
    public class IntegralAttention
    {
        public IAttentionKernel Kernel { get; set; }
        public bool UseFFT { get; set; } = true;

        public IntegralAttention(IAttentionKernel kernel, bool useFFT = true)
        {
            Kernel = kernel;
            UseFFT = useFFT;
        }

        /// <summary>
        /// Compute attention output: y(t) = ∫K(t,s)V(s)ds
        /// </summary>
        public double[] ComputeAttention(double[] values, double[] timePoints, double queryTime)
        {
            int dim = values.Length / timePoints.Length;
            double[] output = new double[dim];
            
            if (UseFFT && timePoints.Length > 64)
            {
                return ComputeAttentionFFT(values, timePoints, queryTime, dim);
            }
            else
            {
                return ComputeAttentionDirect(values, timePoints, queryTime, dim);
            }
        }

        /// <summary>
        /// Direct integration (accurate for small sequences)
        /// </summary>
        private double[] ComputeAttentionDirect(double[] values, double[] timePoints, double queryTime, int dim)
        {
            double[] output = new double[dim];
            int numTimePoints = timePoints.Length;
            
            for (int t = 0; t < numTimePoints; t++)
            {
                double kernelValue = Kernel.Compute(queryTime, timePoints[t]);
                
                for (int d = 0; d < dim; d++)
                {
                    output[d] += kernelValue * values[t * dim + d];
                }
            }
            
            // Normalize (approximate integral)
            double dt = numTimePoints > 1 ? (timePoints[numTimePoints - 1] - timePoints[0]) / (numTimePoints - 1) : 1.0;
            for (int d = 0; d < dim; d++)
            {
                output[d] *= dt;
            }
            
            return output;
        }

        /// <summary>
        /// FFT-based attention (efficient for long sequences)
        /// </summary>
        private double[] ComputeAttentionFFT(double[] values, double[] timePoints, double queryTime, int dim)
        {
            int numTimePoints = timePoints.Length;
            
            // Build kernel vector
            double[] kernelVec = new double[numTimePoints];
            for (int t = 0; t < numTimePoints; t++)
            {
                kernelVec[t] = Kernel.Compute(queryTime, timePoints[t]);
            }
            
            // Convolve each dimension
            double[] output = new double[dim];
            for (int d = 0; d < dim; d++)
            {
                double[] valueDim = new double[numTimePoints];
                for (int t = 0; t < numTimePoints; t++)
                {
                    valueDim[t] = values[t * dim + d];
                }
                
                double[] convolved = FFT.Convolve(valueDim, kernelVec);
                output[d] = convolved[numTimePoints / 2]; // Take center point
            }
            
            return output;
        }

        /// <summary>
        /// Compute attention for full sequence
        /// </summary>
        public double[][] ComputeAttentionSequence(double[] values, double[] timePoints, int dim)
        {
            int numTimePoints = timePoints.Length;
            double[][] outputs = new double[numTimePoints][];
            
            for (int t = 0; t < numTimePoints; t++)
            {
                outputs[t] = ComputeAttention(values, timePoints, timePoints[t]);
            }
            
            return outputs;
        }
    }
}
