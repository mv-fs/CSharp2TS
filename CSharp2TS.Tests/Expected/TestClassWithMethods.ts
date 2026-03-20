// Auto-generated from TestClassWithMethods.cs

import TestClass2 from './TestClass2';

interface TestClassWithMethods {
  value: number;
  build(input: TestClass2 | null): TestClass2;
  reset(): void;
}

export default TestClassWithMethods;

export class TestClassWithMethodsStub implements TestClassWithMethods {
  value: number = 0;

  build(input: TestClass2 | null): TestClass2 {
    throw new Error('Method not implemented.');
  }

  reset(): void {
    throw new Error('Method not implemented.');
  }

  constructor(data?: Partial<TestClassWithMethods>) {
    if (data) {
      Object.assign(this, data);
    }
  }
}
