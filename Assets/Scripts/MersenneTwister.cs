using System;

public class MersenneTwister
{
    private const int N = 624;
    private const int M = 397;
    private const uint MATRIX_A = 0x9908B0DFU; // constant vector a
    private const uint UPPER_MASK = 0x80000000U; // most significant w-r bits
    private const uint LOWER_MASK = 0x7FFFFFFFU; // least significant r bits

    private uint[] mt = new uint[N];
    private int mti = N + 1;

    public MersenneTwister(uint seed)
    {
        InitGenrand(seed);
    }

    private void InitGenrand(uint s)
    {
        mt[0] = s;
        for (mti = 1; mti < N; mti++)
        {
            mt[mti] = (1812433253U * (mt[mti - 1] ^ (mt[mti - 1] >> 30)) + (uint)mti);
        }
    }

    private void GenerateNumbers()
    {
        for (int i = 0; i < N - M; i++)
        {
            uint y = (mt[i] & UPPER_MASK) | (mt[i + 1] & LOWER_MASK);
            mt[i] = mt[i + M] ^ (y >> 1) ^ ((y & 1) * MATRIX_A);
        }
        for (int i = N - M; i < N - 1; i++)
        {
            uint y = (mt[i] & UPPER_MASK) | (mt[i + 1] & LOWER_MASK);
            mt[i] = mt[i + (M - N)] ^ (y >> 1) ^ ((y & 1) * MATRIX_A);
        }
        uint y0 = (mt[N - 1] & UPPER_MASK) | (mt[0] & LOWER_MASK);
        mt[N - 1] = mt[M - 1] ^ (y0 >> 1) ^ ((y0 & 1) * MATRIX_A);
    }

    public uint GenrandInt32()
    {
        if (mti >= N)
        {
            GenerateNumbers();
            mti = 0;
        }

        uint y = mt[mti++];
        y ^= (y >> 11);
        y ^= (y << 7) & 0x9D2C5680U;
        y ^= (y << 15) & 0xEFC60000U;
        y ^= (y >> 18);
        return y;
    }

    public int Next(int max)
    {
        return (int)(GenrandInt32() % (uint)max);
    }
}
