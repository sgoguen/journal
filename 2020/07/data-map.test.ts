import { assertEquals } from "https://deno.land/std/testing/asserts.ts";
import { defineTable } from "./data-map.ts";

const Person = defineTable({
  name: "people",
  fields: {
    name: "text",
    age: "integer",
  },
});

Deno.test("SQL", () => {
  const bob = { name: "Bob", age: 12 };
  const sql = Person.sql.insert(bob);
  console.log({ bob, sql });
  assertEquals(2 + 2, 4);
});
