export type IntegerField = "integer";
export type TextField = "text";
export type FieldType = IntegerField | TextField;
export interface Fields {
  [key: string]: FieldType;
}

export type JsType<T> = T extends Fields ? ({ [P in keyof T]: JsType<T[P]> })
  : T extends IntegerField ? number
  : T extends TextField ? string
  : never;

export function defineTable<F extends Fields>(
  table: { name: string; fields: F },
) {
  type JsRecord = JsType<F>;
  type K = keyof JsRecord;

  const fields = Object.entries(table.fields).map(([name, field]) => ({
    name: name as K,
    field,
  }));

  return {
    table,
    create(o: JsRecord) {
      return o;
    },
    sql: {
      createTable() {
        const fieldDefs = fields.map((name, field) => `${name} ${field}`)
          .join(", ");
        return `create table ${table.name} (${fields});`;
      },
      insert(o: JsRecord) {
        const fieldNames = fields.map((f) => f.name).join(", ");
        const values = fields.map((f) => `${encode_sql(o[f.name])}`).join(", ");
        return `insert into ${table.name} (${fieldNames}) values (${values});`;
      },
    },
  };
}

function encode_sql(value: JsType<FieldType>) {
  switch (typeof value) {
    case "boolean":
      return value ? "1" : "0";
    case "number":
      return value.toString();
    case "string":
      return `'${value.replace("\'", "\'\'")}'`;
    case "undefined":
      return "null";
    default:
  }
}
