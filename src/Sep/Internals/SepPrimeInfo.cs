﻿using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace nietras.SeparatedValues;

// Stores info about primes, including the magic number and shift amount needed
// to implement a divide without using the divide instruction
readonly record struct SepPrimeInfo(uint Prime, uint Magic, int Shift)
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal uint GetIndexForHash(uint hash) => MagicNumberRem(hash);

    // To implement magic-number divide with a 32-bit magic number,
    // multiply by the magic number, take the top 64 bits, and shift that
    // by the amount given in the table.
    // https://github.com/dotnet/runtime/blob/300013b6dffdfeb1864ad5a6a929c75f9278fe50/src/coreclr/inc/simplerhash.inl#L7C1-L27C2
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal readonly uint MagicNumberRem(uint numerator)
    {
        var div = MagicNumberDivide(numerator);
        var result = numerator - (div * Prime);
        Debug.Assert(result == numerator % Prime);
        return result;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal readonly uint MagicNumberDivide(uint numerator)
    {
        ulong num = numerator;
        ulong mag = Magic;
        var product = (num * mag) >> (32 + Shift);
        return (uint)product;
    }
}
