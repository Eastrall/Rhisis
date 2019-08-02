using Rhisis.Core.External;
using System;

namespace Rhisis.Core.Helpers
{
    /// <summary>
    /// Random helper class that uses the MerseenTwister algorithm to generate numbers.
    /// </summary>
    public static class RandomHelper
    {
        private static object _syncLock = new object();
        private static uint _id;

        /// <summary>
        /// Do a random between integers
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public static int Random(int min, int max)
        {
            return (int)(min + Math.Floor(MersenneTwister.NextDouble() * (max - min + 1)));
        }

        /// <summary>
        /// Do a random between floats
        /// </summary>
        /// <param name="f1"></param>
        /// <param name="f2"></param>
        /// <returns></returns>
        public static float FloatRandom(float f1, float f2)
        {
            return (f2 - f1) * (float)MersenneTwister.NextDouble() + f1;
        }

        /// <summary>
        /// Do a random between long values
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public static long LongRandom(long min, long max)
        {
            return (long)(min + Math.Floor(MersenneTwister.NextDouble() * (max - min + 1)));
        }

        /// <summary>
        /// Generates a session key.
        /// </summary>
        /// <returns></returns>
        public static uint GenerateSessionKey()
        {
            return (uint)(new Random().Next(0, int.MaxValue));
        }

        /// <summary>
        /// Generates a unique id. 
        /// </summary>
        /// <returns></returns>
        public static uint GenerateUniqueId()
        {
            lock (_syncLock)
            {
                _id++;
            }

            return _id;
        }
    }
}
