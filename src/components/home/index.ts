import { merge, of } from 'rxjs';


const homeState = {
    appLoading: true
};

export const types = {
    APP_LOAD_FINISH: 'APP_LOAD_FINISH'
};

export const actions = {
    homeAppLoading(appLoading) {
        return {
            type: types.APP_LOAD_FINISH,
            payload: { appLoading }
        };
    }
}

export const reducer = (state = homeState, { type, payload }) => {
    switch (type) {
        case types.APP_LOAD_FINISH:
            return {
                ...state,
                ...payload
            };
        default:
            return state;
    }
};

export const selectors = {
    homeAppLoading({ home }) {
        const { appLoading } = home;
        return appLoading;
    }
};

export const epick = (action$, state$): any=> {
    return merge(
        of(actions.homeAppLoading(false))
    );
};
