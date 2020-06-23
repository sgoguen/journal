import { assertEquals } from "https://deno.land/std/testing/asserts.ts";
import { compose, range, map, sum, filter, toArray, scan, fromArray, first, last, tap, take } from './async-iterable.ts';

Deno.test('Async', () => {

    const f1 = compose(
        increment,
        (x => x.toString() + 'a'),
        (y => y.trim().toUpperCase())
    );

    const x = f1(2);

    assertEquals(x, '3A');
});

const countTo = (max: number) => range(1, max);

Deno.test('Async Battery', async () => {

    const tesfFn = compose(
        countTo,
        scan(0, (x, y) => x + y),
        map(x => x * 3),
        filter(x => x % 2 === 0),
        sum
    );

    const x = await tesfFn(100);

    // console.table(x);
    assertEquals(x, 265200);
});

Deno.test('first', async () => {

    assertEquals(await first(fromArray([22, 33]), 77), 22);

    assertEquals(await first(fromArray([]), 77), 77);
    assertEquals(await first(fromArray([])), undefined);

});

Deno.test('take', async () => {

    const countUpTo5 = compose(countTo, take(5), last);
    assertEquals(await countUpTo5(5), 5);
    // assertEquals(await countUpTo5(6), 5);

});

Deno.test('last', async () => {

    assertEquals(await last(fromArray([22, 33]), 77), 33);

    assertEquals(await last(fromArray([]), 77), 77);
    assertEquals(await last(fromArray([])), undefined);

});

Deno.test('tap', async () => {

    let x = 0;
    const check = compose(
        countTo,
        take(5),
        tap((i: number) => {
            x = i;
        }),
    );

    for await (const item of check(20)) {
        
    }

    assertEquals(x, 5);

});


function increment(x: number) { return x + 1; }
