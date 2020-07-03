
export interface Fn<T, R> { (source: T): R; }

export interface OperatorFunction<T, R> extends Fn<AsyncIterable<T>, AsyncIterable<R>> { }


export function compose<A>(): Fn<A, A>;
export function compose<A, B>(a: Fn<A, B>): Fn<A, B>;
export function compose<A, B, C>(a: Fn<A, B>, b: Fn<B, C>): Fn<A, C>;
export function compose<A, B, C, D>(a: Fn<A, B>, b: Fn<B, C>, c: Fn<C, D>): Fn<A, D>;
export function compose<A, B, C, D, E>(a: Fn<A, B>, b: Fn<B, C>, c: Fn<C, D>, d: Fn<D, E>): Fn<A, E>;
export function compose<A, B, C, D, E, F>(a: Fn<A, B>, b: Fn<B, C>, c: Fn<C, D>, d: Fn<D, E>, e: Fn<E, F>): Fn<A, F>;
export function compose<A, B, C, D, E, F, G>(a: Fn<A, B>, b: Fn<B, C>, c: Fn<C, D>, d: Fn<D, E>, e: Fn<E, F>, f: Fn<F, G>): Fn<A, G>;

export function compose(...functions: Fn<any, any>[]): Fn<any, any> {

    if (!functions || !functions.length) {
        return x => x;
    }
    let r: (Fn<any, any> | null) = null;
    for (const f of functions) {
        const c: (Fn<any, any> | null) = r;
        if (!!c) {
            r = (x => f(c(x)))
        } else {
            r = f;
        }
    }
    if (!r) {
        throw new Error('No functions were provided');
    }
    return r;
}




export async function* create<T>(source: AsyncIterable<T>): AsyncIterable<T> {
    return source;
}

export function map<T, U>(f: (x: T) => U) {
    return async function* (source: AsyncIterable<T>): AsyncIterable<U> {
        for await (const item of source) {
            yield f(item);
        }
    }
}

export function flatMap<T, U>(f: (x: T) => AsyncIterable<U>) {
    return async function* (source: AsyncIterable<T>): AsyncIterable<U> {
        for await (const item of source) {
            const innerSource = f(item);
            for await (const item2 of innerSource) {
                yield item2;
            }
        }
    }
}

export function flatMap2<T, U, V>(f: (x: T) => AsyncIterable<U>, project: ((x: T, y: U) => V)) {
    return async function* (source: AsyncIterable<T>): AsyncIterable<V> {
        for await (const item of source) {
            const innerSource = f(item);
            for await (const item2 of innerSource) {
                yield project(item, item2);
            }
        }
    }
}

export function filter<T>(test: (x: T) => boolean) {
    return async function* (source: AsyncIterable<T>): AsyncIterable<T> {
        for await (const item of source) {
            if (test(item)) {
                yield item;
            }
        }
    }
}

export function scan<T, U>(defaultValue: U, fold: (current: T, last: U) => U) {
    return async function* (source: AsyncIterable<T>): AsyncIterable<U> {
        let currentValue = defaultValue;
        yield currentValue;
        for await (const item of source) {
            let newValue = fold(item, currentValue);
            currentValue = newValue;
            yield newValue;
        }
    }
}

export function take<T>(count: number) {
    return async function* (source: AsyncIterable<T>): AsyncIterable<T> {
        for await (const item of source) {
            if (count <= 0) { return; }
            yield item;
            count--;
        }
    }
}

export function tap<T>(f: (input: T) => void) {
    return async function* (source: AsyncIterable<T>): AsyncIterable<T> {
        for await (const item of source) {
            f(item);
            yield item;
        }
    }
}

export async function* range(begin: number, end: number): AsyncIterable<number> {
    for (let i = begin; i <= end; i++) {
        yield i;
    }
}

export async function sum(source: AsyncIterable<number>): Promise<number> {
    let acc = 0;
    for await (const item of source) {
        acc += item;
    }
    return acc;
}


export async function* fromArray<T>(source: T[]): AsyncIterable<T> {
    for await (const item of source) {
        yield item;
    }
}

export async function toArray<T>(source: AsyncIterable<T>): Promise<T[]> {
    const result = [];
    for await (const item of source) {
        result.push(item);
    }
    return result;
}

export async function first<T>(source: AsyncIterable<T>, defaultValue: T | undefined = undefined): Promise<T | undefined> {
    let result = defaultValue;
    for await (const item of source) {
        return item;
    }
    return defaultValue;
}


export async function last<T>(source: AsyncIterable<T>, defaultValue: T | undefined = undefined): Promise<T | undefined> {
    let result = defaultValue;
    for await (const item of source) {
        result = item;
    }
    return result;
}

