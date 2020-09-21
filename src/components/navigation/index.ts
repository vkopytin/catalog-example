import { ofType } from 'redux-observable';
import { merge, of } from 'rxjs';
import { map, switchMap, withLatestFrom } from 'rxjs/operators';
import { selectors as apiSelectors, types as apiTypes } from '../api';


const navigationState = {
    hash: window.location.hash.replace(/^[#/]/, ''),
    manufacturers: [],
    tab: 'catalog'
};

export const types = {
    LOCATION_HASH_CHANGE: 'LOCATION_HASH_CHANGE',
    NAVIGATION_LOAD_MANUFACTURERS: 'NAVIGATION_LOAD_MANUFACTURERS',
    EXPAND_MANUFACTURER: 'EXPAND_MANUFACTURER',
    NAVIGATION_TAB: 'NAVIGATION_TAB'
};

export const actions = {
    updateLoactionHash(hash) {
        return {
            type: types.LOCATION_HASH_CHANGE,
            payload: { hash }
        };
    },
    updateNavigationManufacturers(manufacturers) {
        return {
            type: types.NAVIGATION_LOAD_MANUFACTURERS,
            payload: { manufacturers }
        };
    },
    expandManufacturer(expandedManufacturer) {
        return {
            type: types.EXPAND_MANUFACTURER,
            payload: {
                expandedManufacturer
            }
        };
    },
    updateNavigationTab(tab) {
        return {
            type: types.NAVIGATION_TAB,
            payload: { tab }
        };
    }
}

export const reducer = (state = navigationState, { type, payload }) => {
    switch (type) {
        case types.LOCATION_HASH_CHANGE:
        case types.NAVIGATION_LOAD_MANUFACTURERS:
        case types.EXPAND_MANUFACTURER:
        case types.NAVIGATION_TAB:
            return {
                ...state,
                ...payload
            };
        default:
            return state;
    }
};

export const selectors = {
    navigationHash({ navigation }) {
        const { hash } = navigation;
        return hash;
    },
    navigationManufacturers({ navigation }) {
        const { manufacturers } = navigation;
        return manufacturers;
    },
    navigationExpandedManufacturer({ navigation }) {
        const { expandedManufacturer } = navigation;
        return expandedManufacturer;
    },
    navigationTab({ navigation }) {
        const { tab } = navigation;
        return tab;
    }
};

export const epick = (action$, state$): any=> {
    return merge(
        action$.pipe(
            ofType(apiTypes.LIST_MANUFACTURERS_SUCCESS),
            withLatestFrom(state$.pipe(map(apiSelectors.apiManufacturers))),
            map(([, manufacturers]: any[]) => actions.updateNavigationManufacturers(manufacturers))
        ),
        action$.pipe(
            ofType(types.EXPAND_MANUFACTURER),
            withLatestFrom(state$.pipe(map(selectors.navigationExpandedManufacturer))),
            switchMap(([, name]: any) => {
                if (name) {
                    window.location.hash = name;
                    return of(
                        actions.updateLoactionHash(name),
                        actions.updateNavigationTab('catalog')
                    );
                }
                return of(
                    actions.updateLoactionHash(''),
                    actions.updateNavigationTab('catalog')
                );
            })
        ),
    );
};
