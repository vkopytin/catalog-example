import { actions, types, epick, reducer } from './';
import { actions as apiFActions, types as apiFTypes } from '../api/my_cars';
import { actions as navigationActions, types as navigationFTypes } from '../navigation';
import { ActionsObservable, StateObservable } from 'redux-observable';
import { toArray } from 'rxjs/operators';
import { Observable, of } from 'rxjs';


describe('favorites epic', () => {
    it('dispatches action to like the car', (done) => {
        const car = { stockNumber: 123 };
        const a = actions.updateFavoritesLike(car);
        const state = {
            favorites: reducer({
                list: [],
                suggestions: []
            }, a),
            apiFavorites: {
                list: [],
                car: car
            }
        };
        const action$ = ActionsObservable.of(a);
        const state$ = of(state);
        epick(action$, state$)
            .pipe(toArray())
            .subscribe((outputActions) => {
                outputActions[0] = outputActions[0](a => a);
                expect(outputActions).toEqual([
                    apiFActions.apiFavoritesLike(car)(a => a)
                ]);
                done();
            });
    });

    it('dispatches action to unlike the car', (done) => {
        const car = { stockNumber: 123 };
        const a = actions.updateFavoritesUnlike(car);
        const state = {
            favorites: reducer({
                list: [],
                suggestions: []
            }, a),
            apiFavorites: {
                list: [],
                car: car
            }
        };
        const action$ = ActionsObservable.of(a);
        const state$ = of(state);
        epick(action$, state$)
            .pipe(toArray())
            .subscribe((outputActions) => {
                outputActions[0] = outputActions[0](a => a);
                expect(outputActions).toEqual([
                    apiFActions.apiFavoritesUnlike(car)(a => a)
                ]);
                done();
            });
    });

    it('dispatches action to load favorites', (done) => {
        const a = navigationActions.updateNavigationTab('test');
        const state = {
            favorites: reducer({
                list: [],
                suggestions: []
            }, a),
            apiFavorites: {}
        };
        const action$ = ActionsObservable.of(a);
        const state$ = of(state);
        epick(action$, state$)
            .pipe(toArray())
            .subscribe((outputActions) => {
                outputActions[0] = outputActions[0](a => a);
                expect(outputActions).toEqual([
                    apiFActions.apiFavoritesList()(a => a)
                ]);
                done();
            });
    });

    it('dispatches action to update favorites suggestions', (done) => {
        const a = apiFActions.apiFavoritesListSuccess({});
        const state = {
            favorites: reducer({
                list: [],
                suggestions: []
            }, a),
            apiFavorites: {
                list: {}
            }
        };
        const action$ = ActionsObservable.of(a);
        const state$ = of(state);
        epick(action$, state$)
            .pipe(toArray())
            .subscribe((outputActions) => {
                expect(outputActions).toEqual([
                    actions.updateFavoritesList({}),
                    actions.updateFavoritesSuggestions([])
                ]);
                done();
            });
    });

    it('dispatches action to refresh favorites when liked', (done) => {
        const car = { stockNumber: 1 };
        const a = apiFActions.apiFavoritesLikeSuccess(car);
        const state = {
            favorites: reducer({
                list: {
                    '1': car
                },
                suggestions: []
            }, a),
            apiFavorites: {
                list: {}
            }
        };
        const action$ = ActionsObservable.of(a);
        const state$ = of(state);
        epick(action$, state$)
            .pipe(toArray())
            .subscribe((outputActions) => {
                expect(outputActions).toEqual([
                    actions.updateFavoritesList({
                        '1': {
                            ...car,
                            isLiked: true
                        }
                    })
                ]);
                done();
            });
    });

    it('dispatches action to refresh favorites when unliked', (done) => {
        const car = { stockNumber: 1 };
        const a = apiFActions.apiFavoritesUnlikeSuccess(1);
        const state = {
            favorites: reducer({
                list: {
                    '1': car
                },
                suggestions: []
            }, a),
            apiFavorites: {
                list: {}
            }
        };
        const action$ = ActionsObservable.of(a);
        const state$ = of(state);
        epick(action$, state$)
            .pipe(toArray())
            .subscribe((outputActions) => {
                expect(outputActions).toEqual([
                    actions.updateFavoritesList({
                        '1': {
                            ...car,
                            isLiked: false
                        }
                    })
                ]);
                done();
            });
    });

    it('dispatches action when filter by suggestion', (done) => {
        const car = { stockNumber: 1 };
        const a = actions.updateFavoritesSelectedSuggestion('test');
        const state = {
            favorites: reducer({
                list: {
                    '1': car
                },
                suggestions: ['test']
            }, a),
            apiFavorites: {
                list: {}
            }
        };
        const action$ = ActionsObservable.of(a);
        const state$ = of(state);
        epick(action$, state$)
            .pipe(toArray())
            .subscribe((outputActions) => {
                expect(outputActions).toEqual([
                    actions.updateFavoritesList({})
                ]);
                done();
            });
    });
});
