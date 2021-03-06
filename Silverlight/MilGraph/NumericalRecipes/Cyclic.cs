// <copyright file="Cyclic.cs" company="Numerical Recipes">
// Copyright © Numerical Recipes.</copyright>
// <email>ov-p@yandex.ru</email>
// <summary>Chapter 2.7 Sparse Linear Systems.
// Solution of the cyclic set of linear equations.</summary>

namespace MilSym.MilGraph.NumericalRecipes
{
    using System;

    /// <summary>
    /// Solves the cyclic set of linear equations.
    /// </summary>
    /// <remarks>
    /// The cyclic set of equations have the form
    /// ---------------------------
    /// b0 c0  0 · · · · · · β
    /// ·  a1 b1 c1 · · · · · ·
    /// ·  ·  ·  · · · · · · · · 
    /// ·  ·  ·  a[n−2] b[n−2] c[n−2]
    /// α  ·  ·  · · 0  a[n-1] b[n-1]
    /// ---------------------------
    /// This is a tri-diagonal system, except for the matrix elements α and β in the corners.
    /// </remarks>
    public static class Cyclic
    {
        /// <summary>
        /// Solves the cyclic set of linear equations. 
        /// </summary>
        /// <remarks>
        /// All vectors have size of n although some elements are not used.
        /// The input is not modified.
        /// </remarks>
        /// <param name="a">Lower diagonal vector of size n; a[0] not used.</param>
        /// <param name="b">Main diagonal vector of size n.</param>
        /// <param name="c">Upper diagonal vector of size n; c[n-1] not used.</param>
        /// <param name="alpha">Bottom-left corner value.</param>
        /// <param name="beta">Top-right corner value.</param>
        /// <param name="rhs">Right hand side vector.</param>
        /// <returns>The solution vector of size n.</returns>
        public static double[] Solve(double[] a, double[] b, double[] c, double alpha, double beta, double[] rhs)
        {
            // a, b, c and rhs vectors must have the same size.
            if (a.Length != b.Length || c.Length != b.Length || rhs.Length != b.Length)
            {
                throw new ArgumentException("Diagonal and rhs vectors must have the same size.");
            }

            int n = b.Length;
            if (n <= 2)
            {
                throw new ArgumentException("n too small in Cyclic; must be greater than 2.");
            }

            double gamma = -b[0]; // Avoid subtraction error in forming bb[0].
            
            // Set up the diagonal of the modified tridiagonal system.
            var bb = new double[n];
            bb[0] = b[0] - gamma;
            bb[n - 1] = b[n - 1] - ((alpha * beta) / gamma);
            for (int i = 1; i < n - 1; ++i)
            {
                bb[i] = b[i];
            }

            // Solve A · x = rhs.
            var solution = Tridiagonal.Solve(a, bb, c, rhs);
            var x = new double[n];
            for (int k = 0; k < n; ++k)
            {
                x[k] = solution[k];
            }

            // Set up the vector u.
            var u = new double[n];
            u[0] = gamma;
            u[n - 1] = alpha;
            for (int i = 1; i < n - 1; ++i)
            {
                u[i] = 0.0;
            }

            // Solve A · z = u.
            solution = Tridiagonal.Solve(a, bb, c, u);
            var z = new double[n];
            for (int k = 0; k < n; ++k)
            {
                z[k] = solution[k];
            }

            // Form v · x/(1 + v · z).
            double fact = (x[0] + ((beta * x[n - 1]) / gamma))
                  / (1.0 + z[0] + ((beta * z[n - 1]) / gamma));

            // Now get the solution vector x.
            for (int i = 0; i < n; ++i)
            {
                x[i] -= fact * z[i];
            }

            return x;
        }
    }
}
