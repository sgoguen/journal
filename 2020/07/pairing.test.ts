import { assertEquals } from "https://deno.land/std/testing/asserts.ts";

function pair(k1: bigint, k2: bigint): bigint {
  return (((k1 + k2) * (k1 + k2 + 1n) / 2n) + k2);
}

function unpair(z: bigint): [bigint, bigint] {
  const w = (sqrt(8n * z + 1n) - 1n) / 2n;
  const t = (w * w + w) / 2n;
  const y = z - t;
  const x = w - y;
  return [x, y];
}

Deno.test("Pairing", () => {
  const results = Array.from(getPairs());
  console.log('Pairing')
  console.table(results);

  function* getPairs() {
    for (let i = 0n; i <= 200n; i++) {
      const [x, y] = unpair(i);
      const ip = pair(x, y);
      yield { i, ip, x, y };
      assertEquals(ip, i);
    }
  }
});

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
