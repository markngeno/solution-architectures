import { createStore } from "redux";

import reducer from "./reducer";

/**
 * Logs all actions and states after they are dispatched.
 */
/*
const logger = store => next => action => {
  console.group(action.type);
  console.info('dispatching', action);
  let result = next(action);
  console.log('next state', store.getState());
  console.groupEnd();
  return result;
};
*/

const store = createStore(reducer, {});

export default store;
