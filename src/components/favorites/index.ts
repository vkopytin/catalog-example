import Fuse from 'fuse.js';
import * as _ from 'lodash';
import { ofType } from 'redux-observable';
import { merge, of } from 'rxjs';
import { map, switchMap, withLatestFrom } from 'rxjs/operators';
import { ICar } from '../api';
import { actions as apiFActions, selectors as apiFSelectors, types as apiFTypes } from '../api/my_cars';
import { types as navigationTypes } from '../navigation';


const favoritesState = {
    list: {},
    suggestions: [] as any[]
};

export const types = {
    FEVORITES_REFRESH: 'FEVORITES_REFRESH',
    FAVORITES_LIKE: 'FAVORITES_LIKE',
    FAVORITES_UNLIKE: 'FAVORITES_UNLIKE',
    FAVORITES_LIST: 'FAVORITES_LIST',
    FAVORITES_SUGGESTIONS: 'FAVORITES_SUGGESTIONS',
    FAVORITES_SELECTED_SUGGESTION: 'FAVORITES_SELECTED_SUGGESTION'
};

export const actions = {
    favoritesRefresh() {
        return {
            type: types.FEVORITES_REFRESH
        };
    },
    updateFavoritesLike(car: ICar) {
        return {
            type: types.FAVORITES_LIKE,
            payload: { car }
        };
    },
    updateFavoritesUnlike({ stockNumber }) {
        return {
            type: types.FAVORITES_UNLIKE,
            payload: {
                stockNumber
            }
        };
    },
    updateFavoritesList(list: {[key: string]: ICar}) {
        return {
            type: types.FAVORITES_LIST,
            payload: {
                list
            }
        };
    },
    updateFavoritesSuggestions(suggestions) {
        return {
            type: types.FAVORITES_SUGGESTIONS,
            payload: {
                suggestions
            }
        };
    },
    updateFavoritesSelectedSuggestion(selectedSuggestion) {
        return {
            type: types.FAVORITES_SELECTED_SUGGESTION,
            payload: {
                selectedSuggestion
            }
        };
    }
};

export const reducer = (state = favoritesState, { type, payload }) => {
    switch (type) {
        case types.FEVORITES_REFRESH:
        case types.FAVORITES_LIKE:
        case types.FAVORITES_UNLIKE:
        case types.FAVORITES_LIST:
        case types.FAVORITES_SUGGESTIONS:
        case types.FAVORITES_SELECTED_SUGGESTION:
            return {
                ...state,
                ...payload
            };
        default:
            return state;
    }
};

export const selectors = {
    favoritesList({ favorites }) {
        const { list } = favorites;
        return Object.values(list).map((item: any) => ({ isLiked: true, ...item }));
    },
    favoritesHashed({ favorites }) {
        const { list } = favorites;
        return list;
    },
    favoriteMarked({ favorites }) {
        const { car } = favorites;
        return car;
    },
    favoritesUnmarked({ favorites }) {
        const { stockNumber } = favorites;
        return stockNumber;
    },
    favoritesSuggestions({ favorites }) {
        const { suggestions } = favorites;
        return suggestions;
    },
    favoritesSelectedSuggestion({ favorites }) {
        const { selectedSuggestion } = favorites;
        return selectedSuggestion;
    }
};

export const epick = (action$, state$): any => {
    return merge(
        action$.pipe(
            ofType(types.FAVORITES_LIKE),
            withLatestFrom(state$.pipe(map(selectors.favoriteMarked))),
            map(([, car]: any[]) => apiFActions.apiFavoritesLike(car))
        ),
        action$.pipe(
            ofType(types.FAVORITES_UNLIKE),
            withLatestFrom(state$.pipe(map(selectors.favoritesUnmarked))),
            map(([, stockNumber]: any[]) => apiFActions.apiFavoritesUnlike(stockNumber))
        ),
        action$.pipe(
            ofType(navigationTypes.NAVIGATION_TAB),
            map(() => apiFActions.apiFavoritesList())
        ),
        action$.pipe(
            ofType(apiFTypes.API_FAVORITES_LIST_SUCCESS),
            withLatestFrom(state$.pipe(map(apiFSelectors.apiFavoritesList))),
            switchMap(([, favoritesList]: any[]) => {
                return of(
                    actions.updateFavoritesList(favoritesList),
                    actions.updateFavoritesSuggestions(
                        _.uniq(Object.values<ICar>(favoritesList).reduce((res: any[], item) => {
                            return [
                                ...res,
                                item.stockNumber,
                                item.color,
                                item.fuelType,
                                item.manufacturerName,
                                item.stockNumber,
                                //item.pictureUrl,
                                item.mileage?.number,
                                item.mileage?.unit
                            ];
                        }, []) as any[]).sort().reverse()
                    )
                );
            })
        ),
        action$.pipe(
            ofType(apiFTypes.API_FAVORITES_LIKE_SUCCESS),
            withLatestFrom(
                state$.pipe(map(selectors.favoritesHashed))
            ),
            switchMap(([{ payload: { car: liked } }, favoritesList]: any[]) => {
                return of(actions.updateFavoritesList({
                    ...favoritesList,
                    [liked.stockNumber]: {
                        ...liked,
                        isLiked: true
                    }
                }));
            })
        ),
        action$.pipe(
            ofType(apiFTypes.API_FAVORITES_UNLIKE_SUCCESS),
            withLatestFrom(
                state$.pipe(map(selectors.favoritesHashed))
            ),
            switchMap(([{ payload: { stockNumber } }, favoritesList]: any[]) => {
                return of(actions.updateFavoritesList({
                    ...favoritesList,
                    [stockNumber]: {
                        ...favoritesList[stockNumber],
                        isLiked: false
                    }
                }));
            })
        ),
        action$.pipe(
            ofType(types.FAVORITES_SELECTED_SUGGESTION),
            withLatestFrom(
                state$.pipe(map(apiFSelectors.apiFavoritesList)),
                state$.pipe(map(selectors.favoritesSelectedSuggestion))
            ),
            map(([, favoritesList, selectedSuggestion]: any[]) => {
                if (!selectedSuggestion) {
                    return actions.updateFavoritesList(favoritesList);
                }
                const keys = Object.values<ICar>(favoritesList),
                    fuse = new Fuse(keys, {
                        threshold: 0.6,
                        keys: ['stockNumber',
                            'color',
                            'fuelType',
                            'manufacturerName',
                            'stockNumber',
                            'mileage.number',
                            'mileage.unit']
                    }),
                    result = fuse.search(selectedSuggestion);

                return actions.updateFavoritesList(result.reduce((res, i) => ({
                    ...res,
                    [i.item.stockNumber]: i.item
                }), {}));
            })
        )
    );
};

export { Favorites } from './favorites';
