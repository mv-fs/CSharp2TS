// Auto-generated from TestClassMethodReturnTypes.cs

import TestEnum from './TestEnum';
import TestClass2 from './TestClass2';
import GenericClass1 from './GenericClass1';

interface TestClassMethodReturnTypes {
  id: number;
  getInt(): number;
  getNullableInt(): number | null;
  getDouble(): number;
  getString(): string;
  getNullableString(): string | null;
  getBool(): boolean;
  getIntArray(): number[];
  getDoubleArray(): number[];
  getStringArray(): string[];
  getBoolArray(): boolean[];
  getEnum(): TestEnum;
  getNullableEnum(): TestEnum | null;
  getClass2(): TestClass2;
  getGenericClass(): GenericClass1<TestClass2>;
  getDictionary(): { [key: string]: string };
  getFile(): File;
  getFileCollection(): File[];
  getJson(): unknown;
  getDateTime(): string;
  getVoid(): void;
  getTask(): void;
  getTaskInt(): number;
  getEnumerableInt(): number[];
}

export default TestClassMethodReturnTypes;
