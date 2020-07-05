import { assertEquals } from "https://deno.land/std/testing/asserts.ts";
import { pair, unpair } from "./cantor.ts";

Deno.test("Cantor Pairing", () => {
  const results = Array.from(getPairs());

  function* getPairs() {
    for (let i = 0n; i <= 200n; i++) {
      const [x, y] = unpair(i);
      const ip = pair(x, y);
      yield { i, ip, x, y };
      assertEquals(ip, i);
    }
  }
});
