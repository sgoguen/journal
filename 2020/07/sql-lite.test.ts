import { assertEquals } from "https://deno.land/std/testing/asserts.ts";
import { defineTable } from "./data-map.ts";

const Person = defineTable({
  name: "people",
  fields: {
    name: "text",
    age: "integer",
  },
});

const bob = Person.create({
  name: "Bob Jones",
  age: 21,
});

Deno.test("SQL", () => {
  //   const sql = Person.sql.insert(bob);
  const sql = Person.sql.insert({ name: "Bob", age: 12 });
  console.log({ bob, sql });
  assertEquals(2 + 2, 4);
});
