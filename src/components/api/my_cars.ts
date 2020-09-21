import * as _ from 'lodash';
import { merge } from 'rxjs';
import { ICar } from './client';


export function asyncQueue(concurrency = 1) {
    let running = 0;
    const taskQueue: any[] = [];

    const runTask = (task) => {
        const done = () => {
            running--;
            if (taskQueue.length > 0) {
                runTask(taskQueue.shift());
            }
        };
        running++;
        try {
            task(done);
        } catch (ex) {
            setTimeout(() => { throw ex; });
            done();
        }
    };

    const enqueueTask = task => taskQueue.push(task);

    return {
        push: task => running < concurrency ? runTask(task) : enqueueTask(task)
    };
}

const lock = asyncQueue(1);

const STORAGE_NAME = 'favorites';

export class MyCars {
    static create() {
        return new MyCars();
    }
    constructor(public storage = localStorage as any) {

    }
    checkLiked(stockNumbers: string[]) {
        return new Promise<boolean[]>(async (resolve, reject) => {
            try {
                const cars = await this.list();
                const keys = Object.keys(cars);
                resolve(_.reduce(stockNumbers, (res, stockNumber) => [...res, keys.indexOf('' + stockNumber) !== -1], [] as boolean[]));
            } catch (ex) {
                reject(ex);
            }
        });
    }
    add(car: ICar) {
        return new Promise((resolve, reject) => {
            lock.push(done => {
                try {
                    const myCars = JSON.parse(this.storage.getItem(STORAGE_NAME) || '{}');
                    myCars[car?.stockNumber] = car;
                    this.storage.setItem(STORAGE_NAME, JSON.stringify(myCars));
                } catch (ex) {
                    reject(ex);
                } finally {
                    done();
                    resolve();
                }
            });
        });
    }
    remove(stockNumber) {
        return new Promise((resolve, reject) => {
            lock.push(done => {
                try {
                    const myCars = JSON.parse(this.storage.getItem(STORAGE_NAME) || '{}');
                    delete myCars[stockNumber];
                    this.storage.setItem(STORAGE_NAME, JSON.stringify(myCars));
                } catch (ex) {
                    reject(ex);
                } finally {
                    done();
                    resolve();
                }
            });
        });
    }
    list() {
        return new Promise<{ [key: string]: ICar }>((resolve, reject) => {
            lock.push(done => {
                let myCars = {};
                try {
                    myCars = JSON.parse(this.storage.getItem(STORAGE_NAME) || '{}');
                } catch (ex) {
                    reject(ex);
                } finally {
                    done();
                    resolve(myCars);
                }
            });
        });
    }
};

const myCars = MyCars.create();

const apiFavoritesState = {
    list: {},
    liked: {}
} as {
    list: { [key: string]: ICar },
    liked: { [key: string]: { isLiked: boolean; } }
};

export const types = {
    API_FAVORITES_CHECK_LIKED: 'API_FAVORITES_CHECK_LIKED',
    API_FAVORITES_CHECK_LIKED_SUCCESS: 'API_FAVORITES_CHECK_LIKED',
    API_FAVORITES_CHECK_LIKED_ERROR: 'API_FAVORITES_CHECK_LIKED',
    API_FAVORITES_LIST: 'API_FAVORITES_LIST',
    API_FAVORITES_LIST_SUCCESS: 'API_FAVORITES_LIST_SUCCESS',
    API_FAVORITES_LIST_ERROR: 'API_FAVORITES_LIST_ERROR',
    API_FAVORITES_LIKE: 'API_FAVORITES_LIKE',
    API_FAVORITES_LIKE_SUCCESS: 'API_FAVORITES_LIKE_SUCCESS',
    API_FAVORITES_LIKE_ERROR: 'API_FAVORITES_LIKE_ERROR',
    API_FAVORITES_UNLIKE: 'API_FAVORITES_UNLIKE',
    API_FAVORITES_UNLIKE_SUCCESS: 'API_FAVORITES_UNLIKE_SUCCESS',
    API_FAVORITES_UNLIKE_ERROR: 'API_FAVORITES_UNLIKE_ERROR'
};

export const actions = {
    apiFavoritesCheckLiked(stockNumbers: string[]) {
        return dispatch => {
            (async () => {
                let result = [] as boolean[];
                try {
                    result = await myCars.checkLiked(stockNumbers);
                } catch (ex) {
                    return dispatch(actions.apiFavoritesCheckLikedError(ex));
                }
                return dispatch(actions.apiFavoritesCheckLikedSuccess(stockNumbers.reduce((res, sn, index) => ({
                    ...res,
                    [sn]: { isLiked: result[index] }
                }), {})));
            })();
            return dispatch({
                type: types.API_FAVORITES_CHECK_LIKED,
                payload: { isLoading: true }
            });
        };
    },
    apiFavoritesCheckLikedSuccess(liked) {
        return {
            type: types.API_FAVORITES_CHECK_LIKED_SUCCESS,
            payload: { liked, isLoading: false }
        };
    },
    apiFavoritesCheckLikedError(error) {
        return {
            type: types.API_FAVORITES_CHECK_LIKED_ERROR,
            payload: { error, isLoading: false }
        };
    },
    apiFavoritesList() {
        return dispatch => {
            (async () => {
                let result = {};
                try {
                    result = await myCars.list();
                } catch (ex) {
                    return dispatch(actions.apiFavoritesListError(ex));
                }
                return dispatch(actions.apiFavoritesListSuccess(result));
            })();
            return dispatch({
                type: types.API_FAVORITES_LIST,
                payload: { isLoading: true }
            });
        };
    },
    apiFavoritesListSuccess(list) {
        return {
            type: types.API_FAVORITES_LIST_SUCCESS,
            payload: { list, isLoading: false }
        };
    },
    apiFavoritesListError(error) {
        return {
            type: types.API_FAVORITES_LIST_ERROR,
            payload: { error, isLoading: false }
        };
    },
    apiFavoritesLike(car) {
        return dispatch => {
            (async () => {
                try {
                    await myCars.add({
                        ...car,
                        isLiked: true
                    });
                } catch (ex) {
                    return dispatch(actions.apiFavoritesLikeError(ex));
                }
                return dispatch(actions.apiFavoritesLikeSuccess({
                    ...car,
                    isLiked: true
                }));
            })();
            return dispatch({
                type: types.API_FAVORITES_LIKE,
                payload: { isLoading: true }
            });
        };
    },
    apiFavoritesLikeSuccess(car) {
        return {
            type: types.API_FAVORITES_LIKE_SUCCESS,
            payload: { car, isLoading: false }
        };
    },
    apiFavoritesLikeError(error) {
        return {
            type: types.API_FAVORITES_LIKE_SUCCESS,
            payload: { error, isLoading: false }
        };
    },
    apiFavoritesUnlike(stockNumber) {
        return dispatch => {
            (async () => {
                try {
                    await myCars.remove(stockNumber);
                } catch (ex) {
                    dispatch(actions.apiFavoritesUnlikeError(ex))
                }
                return dispatch(actions.apiFavoritesUnlikeSuccess(stockNumber));
            })();
            return dispatch({
                type: types.API_FAVORITES_UNLIKE,
                payload: { isLoading: true }
            });
        };
    },
    apiFavoritesUnlikeSuccess(stockNumber) {
        return {
            type: types.API_FAVORITES_UNLIKE_SUCCESS,
            payload: { stockNumber, isLoading: false }
        }
    },
    apiFavoritesUnlikeError(error) {
        return {
            type: types.API_FAVORITES_UNLIKE_SUCCESS,
            payload: { error, isLoading: false }
        }
    }
};

export const reducer = (state = apiFavoritesState, { type, payload }) => {
    switch (type) {
        case types.API_FAVORITES_CHECK_LIKED_SUCCESS:
            return {
                ...state,
                liked: {
                    ...state.liked,
                    ...payload.liked,
                    updated_at: +new Date()
                }
            };
        case types.API_FAVORITES_LIKE:
        case types.API_FAVORITES_LIKE_SUCCESS:
        case types.API_FAVORITES_LIKE_ERROR:
        case types.API_FAVORITES_UNLIKE:
        case types.API_FAVORITES_UNLIKE_SUCCESS:
        case types.API_FAVORITES_UNLIKE_ERROR:
        case types.API_FAVORITES_CHECK_LIKED:
        case types.API_FAVORITES_CHECK_LIKED_ERROR:
        case types.API_FAVORITES_LIST:
        case types.API_FAVORITES_LIST_SUCCESS:
        case types.API_FAVORITES_LIST_ERROR:
            return {
                ...state,
                ...payload
            };
        default:
            return state;
    }
};

export const selectors = {
    apiFavoritesList({ apiFavorites }) {
        const { list } = apiFavorites as typeof apiFavoritesState;
        return list;
    },
    apiFavoritesLiked({ apiFavorites }) {
        const { liked } = apiFavorites as typeof apiFavoritesState;
        return liked;
    }
};

export const epick = (action$, state$): any => {
    return merge(

    );
};
