export function pair(k1: bigint, k2: bigint): bigint {
  return (((k1 + k2) * (k1 + k2 + 1n) / 2n) + k2);
}

export function unpair(z: bigint): [bigint, bigint] {
  const w = (sqrt(8n * z + 1n) - 1n) / 2n;
  const t = (w * w + w) / 2n;
  const y = z - t;
  const x = w - y;
  return [x, y];
}

function sqrt(value: bigint): bigint {
  if (value < 0n) {
    throw "square root of negative numbers is not supported";
  }

  if (value < 2n) {
    return value;
  }

  function newtonIteration(n: bigint, x0: bigint): bigint {
    const x1 = (BigInt((n / x0)) + x0) >> 1n;
    if (x0 === x1 || x0 === (x1 - 1n)) {
      return x0;
    }
    return newtonIteration(n, x1);
  }

  return newtonIteration(value, 1n);
}
