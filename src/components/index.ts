import { combineEpics } from 'redux-observable';
import {
    actions as homeActions,
    types as homeTypes,
    reducer as homeReducer,
    selectors as homeSelectors,
    epick as homeEpick
} from './home';
import {
    actions as navigationActions,
    types as navigationTypes,
    reducer as navigationReducer,
    selectors as navigationSelectors,
    epick as navigationEpick
} from './navigation';
import {
    actions as catalogActions,
    types as catalogTypes,
    reducer as catalogReducer,
    selectors as catalogSelectors,
    epick as catalogEpick
} from './catalog';
import {
    actions as favoritesActions,
    types as favoritesTypes,
    reducer as favoritesReducer,
    selectors as favoritesSelectors,
    epick as favoritesEpick
} from './favorites';
import {
    actions as apiActions,
    types as apiTypes,
    reducer as apiReducer,
    selectors as apiSelectors,
    epick as apiEpic
} from './api';
import {
    actions as apiFavoritesActions,
    types as apiFavoritesTypes,
    reducer as apiFavoritesReducer,
    selectors as apiFavoritesSelectors,
    epick as apiFavoritesEpic
} from './api/my_cars';

export const actions = {
    ...homeActions,
    ...navigationActions,
    ...catalogActions,
    ...favoritesActions,
    ...apiActions,
    ...apiFavoritesActions
};

export const types = {
    ...homeTypes,
    ...navigationTypes,
    ...catalogTypes,
    ...favoritesTypes,
    ...apiTypes,
    ...apiFavoritesTypes
};

export const selectors = {
    ...homeSelectors,
    ...navigationSelectors,
    ...catalogSelectors,
    ...favoritesSelectors,
    ...apiSelectors,
    ...apiFavoritesSelectors
};

export const reducers = {
    home: homeReducer,
    navigation: navigationReducer,
    catalog: catalogReducer,
    favorites: favoritesReducer,
    api: apiReducer,
    apiFavorites: apiFavoritesReducer
};

export const rootEpic = combineEpics(
    homeEpick,
    navigationEpick,
    catalogEpick,
    favoritesEpick,
    apiEpic,
    apiFavoritesEpic
);

export { LocationView } from './navigation/location';
export { DrawerLayout } from './navigation/drawer_layout';
export { MainContainer } from './navigation/container';
export { Favorites } from './favorites';
export { Catalog } from './catalog';
