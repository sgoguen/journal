import { assertEquals } from "https://deno.land/std/testing/asserts.ts";

Deno.test("Smoothing should...", () => {
  const v1 = [1, 4, 8, 4, 3, 2];
  const smoothed = series(v1).smooth(0.75).values;
  //   console.log({ smoothed });
  assertEquals(v1.length, smoothed.length);
  assertEquals(smoothed, [1, 1.75, 5, 7, 3.75, 2.75]);
});

Deno.test("Thinning should remove elements", () => {
  const v1 = gen(1, 10, 0.1);
  const thinned = series(v1).thin(3, 0.75).values;
  console.log({ v1: v1.length, thinned: thinned.length });
});

function series(values: number[]) {
  return {
    values,
    smooth(factor: number) {
      return series(values.map(smoothAt(factor)));
    },
    thin(size: number, smooth: number) {
      return series(values.filter(thinAt(size, smooth)));
    },
  };

  function smoothAt(factor: number) {
    return function (x: number, i: number) {
      if (i === 0) return x;
      const inv = 1 - factor;
      const prev = values[i - 1];
      return factor * prev + inv * x;
    };
  }

  function thinAt(size: number, smooth: number) {
    const s = smoothAt(smooth);
    return function (x: number, i: number) {
      if (i === 0) return true;
      const prev = values[i - 1];
      const diff = Math.abs(s(x, i) - prev - 1);
      console.log({ size, diff });
      return diff > size;
    };
  }
}

function gen(start: number, end: number, incr: number) {
  return Array.from(generate());

  function* generate() {
    let i = start;
    while (i <= end) {
      yield i;
      i += incr;
    }
  }
}
