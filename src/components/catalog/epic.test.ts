import { ActionsObservable } from 'redux-observable';
import { of } from 'rxjs';
import { toArray } from 'rxjs/operators';
import { actions as apiActions } from '../api';
import { actions as apiFActions } from '../api/my_cars';
import { actions as navigationActions } from '../navigation';
import { actions, epick, reducer } from './';


describe('favorites epic', () => {
    it('dispatches actions when clors are loaded', (done) => {
        const a = apiActions.listColorsSuccess(['test']);
        const state = {
            navigation: {
                hash: ''
            },
            catalog: reducer({
                cars: [],
                colors: [],
                display: 'list',
                selectedColor: false,
                sort: '',
                totalCarsCount: 0,
                totalPageCount: 0
            }, a),
            api: {
                colors: { colors: ['test'] },
                manufacturers: {},
                carsById: []
            },
            favorites: {
                car: {}
            },
            apiFavorites: {
            }
        };
        const action$ = ActionsObservable.of(a);
        const state$ = of(state);
        epick(action$, state$)
            .pipe(toArray())
            .subscribe((outputActions) => {
                expect(outputActions).toEqual([
                    actions.catalogRefresh(),
                    actions.updateCatalogColors(['test'])
                ]);
                done();
            });
    });

    it('dispatches actions when color is selected', (done) => {
        const a = actions.updateSelectedColor('');
        const state = {
            navigation: {
                hash: ''
            },
            catalog: reducer({
                cars: [],
                colors: [],
                display: 'list',
                selectedColor: false,
                sort: '',
                totalCarsCount: 0,
                totalPageCount: 0
            }, a as any),
            api: {
                colors: { colors: ['test'] },
                manufacturers: {},
                carsById: []
            },
            favorites: {
                car: {}
            },
            apiFavorites: {
            }
        };
        const action$ = ActionsObservable.of(a);
        const state$ = of(state);
        epick(action$, state$)
            .pipe(toArray())
            .subscribe((outputActions) => {
                outputActions[2] = outputActions[2](a => a);
                expect(outputActions).toEqual([
                    actions.catalogRefresh(),
                    actions.updateSelectedPage(1),
                    apiActions.listCars({})(a => a)
                ]);
                done();
            });
    });

    it('dispatches actions when location hash is changed', (done) => {
        const a = navigationActions.updateLoactionHash('test');
        const state = {
            navigation: {
                hash: ''
            },
            catalog: reducer({
                cars: [],
                colors: [],
                display: 'list',
                selectedColor: false,
                sort: '',
                totalCarsCount: 0,
                totalPageCount: 0
            }, a as any),
            api: {
                colors: { colors: ['test'] },
                manufacturers: {},
                carsById: []
            },
            favorites: {
                car: {}
            },
            apiFavorites: {
            }
        };
        const action$ = ActionsObservable.of(a);
        const state$ = of(state);
        epick(action$, state$)
            .pipe(toArray())
            .subscribe((outputActions) => {
                outputActions = outputActions.map(output => typeof output === 'function' ? output(a => a) : output);
                expect(outputActions).toEqual([
                    actions.catalogRefresh(),
                    actions.updateSelectedPage(1),
                    actions.updateCatalogModels(null),
                    apiActions.listCars({})(a => a)
                ]);
                done();
            });
    });

    it('dispatches actions when car item is selected', (done) => {
        const car = { stockNumber: 123 };
        const a = actions.updateSelectedCar(car);
        const state = {
            navigation: {
                hash: ''
            },
            catalog: reducer({
                cars: [],
                colors: [],
                display: 'list',
                selectedColor: false,
                sort: '',
                totalCarsCount: 0,
                totalPageCount: 0
            }, a as any),
            api: {
                colors: { colors: ['test'] },
                manufacturers: {},
                carsById: []
            },
            favorites: {
                car: {}
            },
            apiFavorites: {
            }
        };
        const action$ = ActionsObservable.of(a);
        const state$ = of(state);
        epick(action$, state$)
            .pipe(toArray())
            .subscribe((outputActions) => {
                outputActions = outputActions.map(output => typeof output === 'function' ? output(a => a) : output);
                expect(outputActions).toEqual([
                    actions.catalogRefresh(),
                    apiActions.fetchCar(car.stockNumber)(a=>a)
                ]);
                done();
            });
    });

    it('dispatches actions when car like is saved on backend', (done) => {
        const car = { stockNumber: 123 };
        const a = apiFActions.apiFavoritesLikeSuccess(car);
        const state = {
            navigation: {
                hash: ''
            },
            catalog: reducer({
                cars: [],
                colors: [],
                display: 'list',
                selectedColor: false,
                selectedCar: car,
                sort: '',
                totalCarsCount: 0,
                totalPageCount: 0
            }, a as any),
            api: {
                colors: { colors: ['test'] },
                manufacturers: {},
                carsById: []
            },
            favorites: {
                car: car
            },
            apiFavorites: {
            }
        };
        const action$ = ActionsObservable.of(a);
        const state$ = of(state);
        epick(action$, state$)
            .pipe(toArray())
            .subscribe((outputActions) => {
                outputActions = outputActions.map(output => typeof output === 'function' ? output(a => a) : output);
                expect(outputActions).toEqual([
                    actions.catalogRefresh(),
                    apiFActions.apiFavoritesCheckLiked(['123'])(a => a),
                    actions.updateSelectedCar({
                        ...car,
                        isLiked: true
                    })
                ]);
                done();
            });
    });


    it('dispatches actions when car unlike is saved on backend', (done) => {
        const car = { stockNumber: 123 };
        const a = apiFActions.apiFavoritesUnlikeSuccess(123);
        const state = {
            navigation: {
                hash: ''
            },
            catalog: reducer({
                cars: [],
                colors: [],
                display: 'list',
                selectedColor: false,
                selectedCar: car,
                sort: '',
                totalCarsCount: 0,
                totalPageCount: 0
            }, a as any),
            api: {
                colors: { colors: ['test'] },
                manufacturers: {},
                carsById: []
            },
            favorites: {
                stockNumber: 123
            },
            apiFavorites: {
            }
        };
        const action$ = ActionsObservable.of(a);
        const state$ = of(state);
        epick(action$, state$)
            .pipe(toArray())
            .subscribe((outputActions) => {
                outputActions = outputActions.map(output => typeof output === 'function' ? output(a => a) : output);
                expect(outputActions).toEqual([
                    actions.catalogRefresh(),
                    apiFActions.apiFavoritesCheckLiked(['123'])(a => a),
                    actions.updateSelectedCar({
                        ...car,
                        isLiked: false
                    })
                ]);
                done();
            });
    });
});
