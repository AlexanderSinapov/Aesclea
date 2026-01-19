// Copyright (c) 2025 Alexander Sinapov | Simeon Petkov
// All rights reserved.
// This code is proprietary and confidential.  
// Unauthorized copying, modification, distribution, or use is strictly prohibited.

using System;

namespace Aesclea_Back_End_.AIModel.NeuralODE
{
    /// <summary>
    /// Solves Ordinary Differential Equations using various numerical methods
    /// Primary method: RK4 (4th order Runge-Kutta) for accuracy
    /// dx/dt = f(x(t), t; θ)
    /// </summary>
    public class ODESolver
    {
        public delegate double[] ODEFunction(double[] state, double t);

        /// <summary>
        /// Solve ODE using 4th order Runge-Kutta method
        /// </summary>
        public static double[] RK4Step(double[] x, double t, double dt, ODEFunction f)
        {
            // k1 = f(x, t)
            double[] k1 = f(x, t);
            
            // k2 = f(x + dt/2 * k1, t + dt/2)
            double[] x_k2 = VectorAdd(x, VectorScale(k1, dt / 2.0));
            double[] k2 = f(x_k2, t + dt / 2.0);
            
            // k3 = f(x + dt/2 * k2, t + dt/2)
            double[] x_k3 = VectorAdd(x, VectorScale(k2, dt / 2.0));
            double[] k3 = f(x_k3, t + dt / 2.0);
            
            // k4 = f(x + dt * k3, t + dt)
            double[] x_k4 = VectorAdd(x, VectorScale(k3, dt));
            double[] k4 = f(x_k4, t + dt);
            
            // x_next = x + dt/6 * (k1 + 2*k2 + 2*k3 + k4)
            double[] result = new double[x.Length];
            for (int i = 0; i < x.Length; i++)
            {
                result[i] = x[i] + (dt / 6.0) * (k1[i] + 2 * k2[i] + 2 * k3[i] + k4[i]);
            }
            
            return result;
        }

        /// <summary>
        /// Adaptive step size RK4 with error estimation (Runge-Kutta-Fehlberg)
        /// </summary>
        public static (double[] nextState, double suggestedDt) RK4Adaptive(
            double[] x, double t, double dt, ODEFunction f, double tolerance = 1e-6)
        {
            // Full step
            double[] x_full = RK4Step(x, t, dt, f);
            
            // Two half steps
            double[] x_half1 = RK4Step(x, t, dt / 2.0, f);
            double[] x_half2 = RK4Step(x_half1, t + dt / 2.0, dt / 2.0, f);
            
            // Error estimate
            double error = 0.0;
            for (int i = 0; i < x.Length; i++)
            {
                double diff = Math.Abs(x_half2[i] - x_full[i]);
                error = Math.Max(error, diff);
            }
            
            // Adjust step size based on error
            double factor = Math.Pow(tolerance / (error + 1e-10), 0.2);
            double newDt = dt * Math.Min(2.0, Math.Max(0.5, 0.9 * factor));
            
            return (x_half2, newDt); // Use more accurate two-step solution
        }

        /// <summary>
        /// Solve ODE from t0 to T with fixed step size
        /// </summary>
        public static double[] Solve(double[] x0, double t0, double T, double dt, ODEFunction f)
        {
            double[] x = (double[])x0.Clone();
            double t = t0;
            
            while (t < T)
            {
                double actualDt = Math.Min(dt, T - t);
                x = RK4Step(x, t, actualDt, f);
                t += actualDt;
            }
            
            return x;
        }

        /// <summary>
        /// Solve ODE with adaptive step size
        /// </summary>
        public static double[] SolveAdaptive(double[] x0, double t0, double T, double dtInitial, 
            ODEFunction f, double tolerance = 1e-6)
        {
            double[] x = (double[])x0.Clone();
            double t = t0;
            double dt = dtInitial;
            
            while (t < T)
            {
                var (nextState, suggestedDt) = RK4Adaptive(x, t, dt, f, tolerance);
                
                // Ensure we don't overshoot
                if (t + dt > T)
                {
                    dt = T - t;
                    x = RK4Step(x, t, dt, f);
                    break;
                }
                
                x = nextState;
                t += dt;
                dt = Math.Min(suggestedDt, T - t);
            }
            
            return x;
        }

        // Helper methods for vector operations
        private static double[] VectorAdd(double[] a, double[] b)
        {
            double[] result = new double[a.Length];
            for (int i = 0; i < a.Length; i++)
                result[i] = a[i] + b[i];
            return result;
        }

        private static double[] VectorScale(double[] v, double scalar)
        {
            double[] result = new double[v.Length];
            for (int i = 0; i < v.Length; i++)
                result[i] = v[i] * scalar;
            return result;
        }
    }
}
