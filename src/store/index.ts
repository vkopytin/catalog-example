import { applyMiddleware, combineReducers, compose, createStore } from 'redux';
import thunk from 'redux-thunk';
import { reducers, rootEpic } from '../components';
import { createEpicMiddleware } from 'redux-observable';


const epicMiddleware = createEpicMiddleware();

export function configureStore(initialState = {}) {
    const middleware = [
        thunk,
        epicMiddleware
    ];
    const rootReducer = combineReducers({
        ...reducers
    });
    const enhancers = [] as any[];
    const windowIfDefined = typeof window === 'undefined' ? null : window as any;
    if (windowIfDefined && windowIfDefined.__REDUX_DEVTOOLS_EXTENSION__) {
        enhancers.push(windowIfDefined.__REDUX_DEVTOOLS_EXTENSION__());
    }

    const store = createStore(
        rootReducer,
        initialState,
        compose(applyMiddleware(...middleware), ...enhancers)
    );

    epicMiddleware.run(rootEpic);

    return store;
}
