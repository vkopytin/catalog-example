import { actions, reducer, selectors } from './';


describe('./catalog', () => {
    it('reduser - home appLoading', () => {
        const home = reducer({
            appLoading: true,
        }, actions.homeAppLoading(false));
        const appLoading = selectors.homeAppLoading({ home });
        expect(appLoading).toBeFalsy();
    });
});
