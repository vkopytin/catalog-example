import * as _ from 'lodash';
import { ofType } from 'redux-observable';
import { merge, of } from 'rxjs';
import { debounceTime, map, switchMap, withLatestFrom } from 'rxjs/operators';
import { actions as apiActions, selectors as apiSelectors, types as apiTypes } from '../api';
import { actions as apiFActions, selectors as apiFSelectors, types as apiFTypes } from '../api/my_cars';
import { selectors as favoritesSelectors } from '../favorites';
import { selectors as navigationSelectors, types as navigationTypes } from '../navigation';


const catalogState = {
    colors: [] as string[],
    selectedColor: false,
    cars: [],
    totalCarsCount: 0,
    totalPageCount: 0,
    sort: '',
    display: 'list'
} as {
    colors: string[];
    selectedColor: boolean;
    selectedCar?: any;
    cars: any[];
    totalCarsCount: number;
    totalPageCount: number;
    sort: string;
    display: string;
};

export const types = {
    CATALOG_REFRESH: 'CATALOG_REFRESH',
    UPDATE_COLORS: 'UPDATE_COLORS',
    UPDATE_SELECTED_COLOR: 'UPDATE_SELECTED_COLOR',
    UPDATE_SELECTED_PAGE: 'UPDATE_SELECTED_PAGE',
    CATALOG_CARS_RESULT: 'CATALOG_CARS_RESULT',
    UPDATE_SELECTED_CAR: 'UPDATE_SELECTED_CAR',
    CATALOG_MODELS: 'CATALOG_MODELS',
    CATALOG_SORT: 'CATALOG_SORT',
    CATALOG_DISPLAY: 'CATALOG_DISPLAY'
};

export const actions = {
    catalogRefresh() {
        return {
            type: types.CATALOG_REFRESH
        };
    },
    updateSelectedColor(selectedColor) {
        return {
            type: types.UPDATE_SELECTED_COLOR,
            payload: {
                selectedColor
            }
        };
    },
    updateSelectedPage(selectedPage) {
        return {
            type: types.UPDATE_SELECTED_PAGE,
            payload: {
                selectedPage
            }
        };
    },
    updateSelectedCar(selectedCar) {
        return {
            type: types.UPDATE_SELECTED_CAR,
            payload: {
                selectedCar
            }
        };
    },
    updateCatalogColors(colors) {
        return {
            type: types.UPDATE_COLORS,
            payload: {
                colors
            }
        };
    },
    updateCatalogCars({
        cars,
        totalCarsCount,
        totalPageCount
    }) {
        return {
            type: types.CATALOG_CARS_RESULT,
            payload: {
                cars,
                totalCarsCount,
                totalPageCount
            }
        };
    },
    updateCatalogModels(models) {
        return {
            type: types.CATALOG_MODELS,
            payload: { models }
        };
    },
    updateCatalogSort(sort: 'asc' | 'desc') {
        return {
            type: types.CATALOG_SORT,
            payload: { sort }
        };
    },
    updateCatalogDisplay(display) {
        return {
            type: types.CATALOG_DISPLAY,
            payload: { display }
        };
    }
};

export const reducer = (state = catalogState, { type, payload }) => {
    switch (type) {
        case types.UPDATE_COLORS:
        case types.UPDATE_SELECTED_COLOR:
        case types.UPDATE_SELECTED_PAGE:
        case types.CATALOG_CARS_RESULT:
        case types.UPDATE_SELECTED_CAR:
        case types.CATALOG_MODELS:
        case types.CATALOG_SORT:
        case types.CATALOG_DISPLAY:
            return {
                ...state,
                ...payload
            };
        default:
            return state;
    }
};

export const selectors = {
    catalogColors({ catalog }) {
        const { colors } = catalog;
        return colors;
    },
    catalogModels({ catalog }) {
        const { models } = catalog;
        return models;
    },
    catalogSelectedColor({ catalog }) {
        const { selectedColor } = catalog;
        return selectedColor;
    },
    catalogSelectedProps({ catalog, navigation }) {
        const {
            selectedColor,
            selectedPage,
            sort
        } = catalog;
        const {
            hash
        } = navigation;
        return {
            ...((!selectedColor || selectedColor === 'none') ? {} : { color: selectedColor }),
            ...(selectedPage ? { page: selectedPage } : {}),
            ...(hash ? { manufacturer: hash } : {}),
            ...(sort ? { sort } : {})
        };
    },
    catalogSelectedPage({ catalog }) {
        const { selectedPage } = catalog;
        return selectedPage;
    },
    catalogSelectedCar({ catalog }) {
        const { selectedCar } = catalog;
        return selectedCar;
    },
    catalogTotalPages({ catalog }) {
        const { totalPageCount } = catalog;
        return totalPageCount;
    },
    catalogListCars({ catalog }) {
        const { cars } = catalog;
        return cars;
    },
    catalogSort({ catalog }) {
        const { sort } = catalog;
        return sort;
    },
    catalogDisplay({ catalog }) {
        const { display } = catalog;
        return display;
    }
};

export const epick = (action$, state$): any => {
    return merge(
        of(actions.catalogRefresh()),
        action$.pipe(
            ofType(apiTypes.LIST_COLORS_SUCCESS),
            withLatestFrom(state$.pipe(map(apiSelectors.apiColors))),
            map(([, colors]: any[]) => actions.updateCatalogColors(colors))
        ),
        action$.pipe(
            ofType(
                types.CATALOG_REFRESH,
                types.UPDATE_SELECTED_COLOR,
                navigationTypes.LOCATION_HASH_CHANGE
            ),
            map(() => actions.updateSelectedPage(1))
        ),
        action$.pipe(
            ofType(apiTypes.LIST_MANUFACTURERS_SUCCESS, navigationTypes.LOCATION_HASH_CHANGE),
            debounceTime(100),
            withLatestFrom(
                state$.pipe(map(apiSelectors.apiManufacturers)),
                state$.pipe(map(navigationSelectors.navigationHash))
            ),
            map(([, manufacturers, manufacturer]: any[]) => actions.updateCatalogModels(manufacturers ? manufacturers[manufacturer] : null))
        ),
        action$.pipe(
            ofType(
                types.CATALOG_REFRESH,
                types.UPDATE_SELECTED_COLOR,
                types.CATALOG_SORT,
                navigationTypes.LOCATION_HASH_CHANGE,
                types.UPDATE_SELECTED_PAGE
            ),
            debounceTime(100),
            withLatestFrom(state$.pipe(map(selectors.catalogSelectedProps))),
            map(([, params]: any[]) => apiActions.listCars(params))
        ),
        action$.pipe(
            ofType(apiTypes.LIST_CARS_SUCCESS, apiFTypes.API_FAVORITES_CHECK_LIKED_SUCCESS),
            withLatestFrom(
                state$.pipe(map(apiSelectors.apiCars)),
                state$.pipe(map(apiFSelectors.apiFavoritesLiked))
            ),
            map(([, { cars, totalCarsCount, totalPageCount }, liked]: any[]) => actions.updateCatalogCars({
                totalCarsCount,
                totalPageCount,
                cars: cars.map((car) => ({...car, ...liked[car.stockNumber]}))
            }))
        ),
        action$.pipe(
            ofType(apiTypes.LIST_CARS_SUCCESS, apiFTypes.API_FAVORITES_LIKE_SUCCESS, apiFTypes.API_FAVORITES_UNLIKE_SUCCESS),
            withLatestFrom(state$.pipe(map(selectors.catalogListCars))),
            map(([, cars]: any[]) => apiFActions.apiFavoritesCheckLiked(_.map(cars, ({stockNumber}) => stockNumber)))
        ),
        action$.pipe(
            ofType(types.UPDATE_SELECTED_CAR),
            withLatestFrom(state$.pipe(map(selectors.catalogSelectedCar))),
            map(([, { stockNumber }]: any) => apiActions.fetchCar(stockNumber))
        ),
        action$.pipe(
            ofType(apiFTypes.API_FAVORITES_LIKE_SUCCESS),
            withLatestFrom(
                state$.pipe(map(selectors.catalogSelectedCar)),
                state$.pipe(map(favoritesSelectors.favoriteMarked))
            ),
            switchMap(([, selectedCar, likedCar]: any[]) => {
                if (selectedCar?.stockNumber === likedCar?.stockNumber) {
                    return of(actions.updateSelectedCar({
                        ...selectedCar,
                        isLiked: true
                    }));
                }
                return of();
            })
        ),
        action$.pipe(
            ofType(apiFTypes.API_FAVORITES_UNLIKE_SUCCESS),
            withLatestFrom(
                state$.pipe(map(selectors.catalogSelectedCar)),
                state$.pipe(map(favoritesSelectors.favoritesUnmarked))
            ),
            switchMap(([, selectedCar, unlikedCarId]: any[]) => {
                if (selectedCar?.stockNumber === unlikedCarId) {
                    return of(actions.updateSelectedCar({
                        ...selectedCar,
                        isLiked: false
                    }));
                }
                return of();
            })
        )
    );
};

export { Catalog } from './catalog';
