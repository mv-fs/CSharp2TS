// Auto-generated from TestClassWithDefault.cs

interface TestClassWithDefault {
  guidProperty: string;
  regularGuidProperty: string;
}

export default TestClassWithDefault;

export class TestClassWithDefaultStub implements TestClassWithDefault {
  guidProperty: string = '018b1a05-8f0f-477d-8d95-c7effcde2eeb';
  regularGuidProperty: string = '';

  constructor(data?: Partial<TestClassWithDefault>) {
    if (data) {
      Object.assign(this, data);
    }
  }
}
