import { merge, of } from 'rxjs';
import { fetchCar, listCars, listColors, listManufacturers } from './client';


const apiState = {
    manufacturers: {
        manufacturers: []
    },
    colors: {
        colors: []
    },
    cars: {},
    totalCarsCount: 0,
    totalPageCount: 0,
    carsById: []
};

export const types = {
    FETCH_CAR: 'FETCH_CAR',
    FETCH_CAR_SUCCESS: 'FETCH_CAR_SUCCESS',
    FETCH_CAR_ERROR: 'FETCH_CAR_ERROR',
    LIST_CARS: 'LIST_CARS',
    LIST_CARS_SUCCESS: 'LIST_CARS_SUCCESS',
    LIST_CARS_ERROR: 'LIST_CARS_ERROR',
    LIST_COLORS: 'LIST_COLORS',
    LIST_COLORS_SUCCESS: 'LIST_COLORS_SUCCESS',
    LIST_COLORS_ERROR: 'LIST_COLORS_ERROR',
    LIST_MANUFACTURERS: 'LIST_MANUFACTURERS',
    LIST_MANUFACTURERS_SUCCESS: 'LIST_MANUFACTURERS_SUCCESS',
    LIST_MANUFACTURERS_ERROR: 'LIST_MANUFACTURERS_ERROR'
};

export const actions = {
    fetchCar(stockNumber) {
        return dispatch => {
            (async () => {
                try {
                    const result = await fetchCar(stockNumber);
                    return dispatch(actions.fetchCarSuccess(result));
                } catch (ex) {
                    return dispatch(actions.fetchCarError(ex));
                }
            })();
            return dispatch({
                type: types.FETCH_CAR,
                payload: {
                    isLoading: true
                }
            });
        }
    },
    fetchCarSuccess(car) {
        return {
            type: types.FETCH_CAR_SUCCESS,
            payload: { car, isLoading: false }
        };
    },
    fetchCarError(error) {
        return {
            type: types.FETCH_CAR_ERROR,
            payload: { error, isLoading: false }
        }
    },
    listCars(params: { manufacturer?: string; color?: string; sort?: 'asc' | 'des'; page?: number; }) {
        return dispatch => {
            (async () => {
                try {
                    const result = await listCars(params);
                    return dispatch(actions.listCarsSuccess(result));
                } catch (ex) {
                    return dispatch(actions.listCarsError(ex));
                }
            })();
            return dispatch({
                type: types.LIST_CARS,
                payload: {
                    isLoading: true
                }
            });
        }
    },
    listCarsSuccess({ cars, totalCarsCount, totalPageCount}) {
        const items = cars.reduce((res, car) => ({ ...res, [car.stockNumber]: car }), {});
        const byId = cars.map(({ stockNumber }) => stockNumber);
        return {
            type: types.LIST_CARS_SUCCESS,
            payload: {
                cars: items,
                totalCarsCount,
                totalPageCount,
                byId,
                isLoading: false
            }
        };
    },
    listCarsError(error) {
        return {
            type: types.LIST_CARS_ERROR,
            payload: { error, isLoading: false }
        }
    },
    listColors() {
        return dispatch => {
            (async () => {
                try {
                    const result = await listColors();
                    return dispatch(actions.listColorsSuccess(result));
                } catch (ex) {
                    return dispatch(actions.listColorsError(ex));
                }
            })();
            return dispatch({
                type: types.LIST_COLORS,
                payload: {
                    isLoading: true
                }
            });
        }
    },
    listColorsSuccess(colors) {
        return {
            type: types.LIST_COLORS_SUCCESS,
            payload: { colors, isLoading: false }
        };
    },
    listColorsError(error) {
        return {
            type: types.LIST_COLORS_ERROR,
            payload: { error, isLoading: false }
        }
    },
    listManufacturers() {
        return dispatch => {
            (async () => {
                try {
                    const result = await listManufacturers();
                    return dispatch(actions.listManufacturersSuccess(result));
                } catch (ex) {
                    return dispatch(actions.listManufacturersError(ex));
                }
            })();
            return dispatch({
                type: types.LIST_MANUFACTURERS,
                payload: {
                    isLoading: true
                }
            });
        }
    },
    listManufacturersSuccess(manufacturers) {
        return {
            type: types.LIST_MANUFACTURERS_SUCCESS,
            payload: { manufacturers, isLoading: false }
        };
    },
    listManufacturersError(error) {
        return {
            type: types.LIST_MANUFACTURERS_ERROR,
            payload: { error, isLoading: false }
        }
    }
}

export const reducer = (state = apiState, { type, payload }) => {
    switch (type) {
        case types.LIST_CARS_SUCCESS:
            return {
                ...state,
                cars: {
                    ...state.cars,
                    ...payload.cars,
                    updated_at: +new Date()
                },
                carsById: payload.byId,
                totalCarsCount: payload.totalCarsCount,
                totalPageCount: payload.totalPageCount
            };
        case types.FETCH_CAR:
        case types.FETCH_CAR_SUCCESS:
        case types.FETCH_CAR_ERROR:
        case types.LIST_CARS:
        case types.LIST_CARS_ERROR:
        case types.LIST_COLORS:
        case types.LIST_COLORS_SUCCESS:
        case types.LIST_COLORS_ERROR:
        case types.LIST_MANUFACTURERS:
        case types.LIST_MANUFACTURERS_SUCCESS:
        case types.LIST_MANUFACTURERS_ERROR:
            return {
                ...state,
                ...payload
            };
        default:
            return state;
    }
};

export const selectors = {
    apiCar({ api }) {
        const { car } = api;
        return car.car;
    },
    apiCars({ api }) {
        const { cars, carsById, totalCarsCount, totalPageCount } = api;
        return {
            cars: carsById.map(id => cars[id]),
            totalCarsCount,
            totalPageCount
        };
    },
    apiColors({ api }) {
        const { colors } = api;
        return colors.colors;
    },
    apiManufacturers({ api }) {
        const { manufacturers } = api;
        return manufacturers.manufacturers;
    }
};

export const epick = (action$, state$): any=> {
    return merge(
        of(
            actions.listColors(),
            actions.listManufacturers()
        )
    );
};

export type { ICar } from './client';
