#!/usr/bin/env python3

pawns = [
    0,  0,  0,  0,  0,  0,  0,  0,
    50, 50, 50, 50, 50, 50, 50, 50,
    10, 10, 20, 30, 30, 20, 10, 10,
    5,  5, 10, 25, 25, 10,  5,  5,
    0,  0,  0, 20, 20,  0,  0,  0,
    5, -5,-10,  0,  0,-10, -5,  5,
    5, 10, 10,-20,-20, 10, 10,  5,
    0,  0,  0,  0,  0,  0,  0,  0
]

COMPRESS_BITS = 4
COMPRESS_MAX = (2 ^ COMPRESS_BITS) - 1
INT_BITS = 64

def num_to_range(num, inMin, inMax, outMin, outMax):
    return int(outMin + (float(num - inMin) / float(inMax - inMin) * (outMax - outMin)))


def clamp(array):
    clamped = [0] * len(array)
    minA = min(array)
    maxA = max(array)
    for index, number in enumerate(array):
        clamped[index] = num_to_range(number, minA, maxA, 0, COMPRESS_MAX)
    return clamped


def encode(array):
    out = [0] * COMPRESS_BITS
    for index in range(0, INT_BITS, COMPRESS_BITS):
        for offset in range(COMPRESS_BITS):
            out[offset] |= array[index + offset] << index

    return out


def decode(encoded):
    decoded = [0] * INT_BITS
    for index in range(0, INT_BITS, COMPRESS_BITS):
        mask = 0xF << index
        for offset in range(COMPRESS_BITS):
            decoded[index + offset] = (encoded[offset] & mask) >> index

    return decoded


clamped = clamp(pawns)
print(f"clamped: {clamped}")
out = encode(clamped)
print(f"out: {out}")

decoded = decode(out)
assert(clamped == decoded)
