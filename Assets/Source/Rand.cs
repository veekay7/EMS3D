using UnityEngine;

public static class Rand
{
    /// <summary>
    /// Generate random numbers with the Linear Congruential Generator
    /// References: https://en.wikipedia.org/wiki/Linear_congruential_generator
    /// https://stackoverflow.com/questions/686353/random-float-number-generation
    /// </summary>
    public static class LCRNG
    {
        private const long a = 1103515245;  // Multiplier
        private const long c = 12345;       // Increment
        private static long m_seed = 100;

        public static void Init(long seed = 100)
        {
            m_seed = seed;
        }

        private static void Reseed()
        {
            // Basic idea: seed = (a * seed + c) mod m
            m_seed = (a * m_seed + c) % GetRandMax();
        }

        private static long GetRandMax()
        {
            // Modulus m (which is also the maximum possible random value).
            return (long)Mathf.Pow(2.0f, 31.0f);
        }

        public static int GetRange(int lower, int upper)
        {
            Reseed();
            int result = lower + (int)m_seed / ((int)GetRandMax() / (upper - lower));

            return result;
        }

        /// <summary>
        /// Returns a value between lower and upper.
        /// </summary>
        /// <param name="lower"></param>
        /// <param name="upper"></param>
        /// <returns></returns>
        public static float GetRange(float lower, float upper)
        {
            Reseed();
            float result = lower + m_seed / ((float)GetRandMax() / (upper - lower));

            return result;
        }

        /// <summary>
        /// Returns a random value between 0 and 1.
        /// </summary>
        /// <returns></returns>
        public static float GetRange()
        {
            // To get a value between 0 and 1.
            Reseed();
            float randomValue = m_seed / (float)GetRandMax();

            return randomValue;
        }
    }

    /// <summary>
    /// Mersenne Twister Random Number Generator
    /// </summary>
    public class MersenneTwister
    {
        // Period parameters
        private const int N = 624;

        private const int M = 397;
        private const ulong MATRIX_A = 0x9908b0dfUL;    // Constant vector a.
        private const ulong UPPER_MASK = 0x80000000UL;  // Most significant w-r bits.
        private const ulong LOWER_MASK = 0x7fffffffUL;  // Least significant r bits.
        private ulong[] mt = new ulong[N];      // The array for the state vector.
        private int mti = N + 1;                // mti==N+1 means mt[N] is not initialized.

        /// <summary>
        /// Constructor
        /// </summary>
        public MersenneTwister()
        {
            // Set default seeds.
            InitGenRandArray(new ulong[] { 0x123, 0x234, 0x345, 0x456 });
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="s"></param>
        public MersenneTwister(ulong s)
        {
            InitGenRand(s);
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="init_key"></param>
        public MersenneTwister(ulong[] init_key)
        {
            InitGenRandArray(init_key);
        }

        /// <summary>
        /// Initializes mt[N] with a seed.
        /// </summary>
        /// <param name="s"></param>
        public void InitGenRand(ulong s)
        {
            mt[0] = s & 0xffffffffUL;
            for (mti = 1; mti < N; mti++)
            {
                mt[mti] = (1812433253UL * (mt[mti - 1] ^ (mt[mti - 1] >> 30)) + (ulong)mti);
                /* See Knuth TAOCP Vol2. 3rd Ed. P.106 for multiplier. */
                /* In the previous versions, MSBs of the seed affect   */
                /* only MSBs of the array mt[].                        */
                /* 2002/01/09 modified by Makoto Matsumoto             */
                mt[mti] &= 0xffffffffUL;
                /* for >32 bit machines */
            }
        }

        /// <summary>
        /// Initialize by an array with array-length.
        /// init_key is the array for initializing keys.
        /// init_key.Length is its length.
        /// </summary>
        /// <param name="init_key"></param>
        public void InitGenRandArray(ulong[] init_key)
        {
            InitGenRand(19650218UL);
            int i = 1;
            int j = 0;
            int k = (N > init_key.Length ? N : init_key.Length);
            for (; k != 0; k--)
            {
                mt[i] = (mt[i] ^ ((mt[i - 1] ^ (mt[i - 1] >> 30)) * 1664525UL)) + init_key[j] + (ulong)j; /* non linear */
                mt[i] &= 0xffffffffUL; /* for WORDSIZE > 32 machines */
                i++; j++;
                if (i >= N)
                {
                    mt[0] = mt[N - 1]; i = 1;
                }

                if (j >= init_key.Length)
                {
                    j = 0;
                }
            }

            for (k = N - 1; k != 0; k--)
            {
                mt[i] = (mt[i] ^ ((mt[i - 1] ^ (mt[i - 1] >> 30)) * 1566083941UL)) - (ulong)i; // non linear
                mt[i] &= 0xffffffffUL; // for WORDSIZE > 32 machines
                i++;
                if (i >= N)
                {
                    mt[0] = mt[N - 1];
                    i = 1;
                }
            }

            mt[0] = 0x80000000UL; // MSB is 1; assuring non-zero initial array.
        }

        /// <summary>
        /// Generates a random number on [0,0xffffffff]-interval.
        /// </summary>
        /// <returns></returns>
        public ulong GenRand_UInt32()
        {
            ulong[] mag01 = new ulong[] { 0x0UL, MATRIX_A };
            ulong y = 0;

            // mag01[x] = x * MATRIX_A  for x=0,1
            if (mti >= N)
            {
                // generate N words at one time
                int kk;
                if (mti == N + 1)
                {
                    // If init_genrand() has not been called,
                    InitGenRand(5489UL);   // a default initial seed is used
                }
                for (kk = 0; kk < N - M; kk++)
                {
                    y = (mt[kk] & UPPER_MASK) | (mt[kk + 1] & LOWER_MASK);
                    mt[kk] = mt[kk + M] ^ (y >> 1) ^ mag01[y & 0x1UL];
                }
                for (; kk < N - 1; kk++)
                {
                    y = (mt[kk] & UPPER_MASK) | (mt[kk + 1] & LOWER_MASK);
                    mt[kk] = mt[kk + (M - N)] ^ (y >> 1) ^ mag01[y & 0x1UL];
                }
                y = (mt[N - 1] & UPPER_MASK) | (mt[0] & LOWER_MASK);
                mt[N - 1] = mt[M - 1] ^ (y >> 1) ^ mag01[y & 0x1UL];
                mti = 0;
            }

            y = mt[mti++];
            // Tempering
            y ^= (y >> 11);
            y ^= (y << 7) & 0x9d2c5680UL;
            y ^= (y << 15) & 0xefc60000UL;
            y ^= (y >> 18);
            return y;
        }

        /// <summary>
        /// Generates a random floating point number on [0,1]
        /// </summary>
        /// <returns></returns>
        public double GenRandNormalized1()
        {
            return GenRand_UInt32() * (1.0 / 4294967295.0); // divided by 2^32-1
        }

        /// <summary>
        /// Generates a random floating point number on [0,1)
        /// </summary>
        /// <returns></returns>
        public double GenRandNormalized2()
        {
            return GenRand_UInt32() * (1.0 / 4294967296.0); // divided by 2^32
        }

        /// <summary>
        /// Generates a random integer number from 0 to N-1
        /// </summary>
        /// <param name="iN"></param>
        /// <returns></returns>
        public int GenRandInt(int iN)
        {
            return (int)(GenRand_UInt32() * (iN / 4294967296.0));
        }
    }
}