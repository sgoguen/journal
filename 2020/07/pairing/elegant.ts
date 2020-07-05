
export function pair(x: bigint, y: bigint): bigint {
    const mx = (x >= y) ? x : y;
    if (mx !== x)
        return y * y + x;
    return (x * x) + x + y;
}

export function unpair(z: bigint): [bigint, bigint] {
    let fz = Number(z);
    let r = BigInt(Math.floor(Math.sqrt(fz)));
    let rl = z - (r * r);
    if (rl < r)
        return [rl, r];
    return [r, rl - r];
}
