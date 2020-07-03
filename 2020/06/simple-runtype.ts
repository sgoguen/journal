import {
  String,
  Record,
  Static,
} from "https://raw.githubusercontent.com/sgoguen/runtypes/deno/src/index.ts";

// export interface Runtype<A = unknown> {
//   /* @internal */ _falseWitness: A;
// }

// export type Static<A extends Runtype> = A["_falseWitness"];

// export function create<A extends Runtype>(
//   A: any,
// ): A {
//   return A;
// }

// /////////////////////////////////////////////

// export interface Text extends Runtype<string> {
//   tag: "string";
// }

// export const Text = create<Text>(
//   { tag: "string" },
// );

// /////////////////////////////////////////////

// type RecordStaticType<
//   O extends { [_: string]: Runtype }
// > = {
//   [K in keyof O]: Static<O[K]>
// };

// export interface Record<O extends { [_: string]: Runtype }>
//   extends Runtype<RecordStaticType<O>> {
//   tag: "record";
//   fields: O;
// }

// export function Record<O extends { [_: string]: Runtype }>(
//   fields: O,
// ): Record<O> {
//   return create({
//     tag: 'record',
//     fields: fields,
//   });
// }

const Name = Record({
  first: String,
  last: String,
});

const Person = Record({
  name: Name.Or(String),
  title: String,
});

// // type Name = Static<typeof Name>;
type Person = Static<typeof Person>;

function greet(input: any) {
  const person = Person.check(input);
  if (typeof person === "string") {
    return person;
  }
  const { name, title } = person;

  if (typeof name === "string") return name;
  const { first, last } = name;
  return `${title} ${first} ${last}`;
}

console.log(greet({ name: "Yo", title: "Hey" }));
