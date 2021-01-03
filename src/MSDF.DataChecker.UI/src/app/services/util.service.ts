import { environment } from './../../environments/environment';

export class UtilService {
  static tokenInformation: any = null;
  static communityUrlUser: any = null;

  static apiUrl: any = () => {
    const url = environment.apiUrl;
    return url;
  }

  static communityUrl: any = () => {
    if (
      UtilService.communityUrlUser == null ||
      UtilService.communityUrlUser === environment.communityUrl
    ) {
      return environment.communityUrl;
    }

    return UtilService.communityUrlUser;
  }

  static currentUserId: any = () => '661BFE54-ED59-4D59-B771-005F25F8356D';

  static compareObj: any = (obj1: any, obj2: any) => diff(obj1, obj2);
}

const diff = (
  obj1: { [x: string]: any; hasOwnProperty: (arg0: string) => any },
  obj2: { [x: string]: any; hasOwnProperty: (arg0: string) => any }
) => {
  // Make sure an object to compare is provided
  if (!obj2 || Object.prototype.toString.call(obj2) !== '[object Object]') {
    return obj1;
  }

  //
  // Variables
  //
  const diffs: any = {};
  let key;

  //
  // Methods
  //
  const arraysMatch = (arr1: any[], arr2: any[]): boolean => {
    // Check if the arrays are the same length
    if (arr1.length !== arr2.length) {
      return false;
    }

    // Check if all items exist and are in the same order
    for (let i = 0; i < arr1.length; i++) {
      if (arr1[i] !== arr2[i]) {
        return false;
      }
    }

    // Otherwise, return true
    return true;
  };

  const compare = (item1: any[], item2: any[], k: string) => {
    // Get the object type
    const type1 = Object.prototype.toString.call(item1);
    const type2 = Object.prototype.toString.call(item2);

    // If type2 is undefined it has been removed
    if (type2 === '[object Undefined]') {
      diffs[k] = null;
      return;
    }

    // If items are different types
    if (type1 !== type2) {
      diffs[k] = item2;
      return;
    }

    // If an object, compare recursively
    if (type1 === '[object Object]') {
      const objDiff = diff(item1, item2);
      if (Object.keys(objDiff).length > 1) {
        diffs[k] = objDiff;
      }
      return;
    }

    // If an array, compare
    if (type1 === '[object Array]') {
      if (!arraysMatch(item1, item2)) {
        diffs[k] = item2;
      }
      return;
    }

    // Else if it's a function, convert to a string and compare
    // Otherwise, just compare
    if (type1 === '[object Function]') {
      if (item1.toString() !== item2.toString()) {
        diffs[k] = item2;
      }
    } else {
      if (item1 !== item2) {
        diffs[k] = item2;
      }
    }
  };

  //
  // Compare our objects
  //
  // Loop through the first object
  for (key in obj1) {
    if (obj1.hasOwnProperty(key)) {
      compare(obj1[key], obj2[key], key);
    }
  }

  // Loop through the second object and find missing items
  for (key in obj2) {
    if (obj2.hasOwnProperty(key)) {
      if (!obj1[key] && obj1[key] !== obj2[key]) {
        diffs[key] = obj2[key];
      }
    }
  }

  // Return the object of differences
  return diffs;
};
