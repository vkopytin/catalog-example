import { actions, types, epick } from './';
import { ActionsObservable, StateObservable } from 'redux-observable';
import { toArray } from 'rxjs/operators';
import { of } from 'rxjs';


describe('home epic', () => {
    it('dispatches actions to change app loading', (done) => {
        const action$ = ActionsObservable.of({
            type: types.APP_LOAD_FINISH
        });
        const state$ = of({});
        epick(action$, state$)
            .pipe(toArray())
            .subscribe((outputActions) => {
                expect(outputActions).toEqual([
                    actions.homeAppLoading(false)
                ]);
                done();
            });
    });
});
