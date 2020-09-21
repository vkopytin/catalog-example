import { actions, types, epick, reducer } from './';
import { actions as apiActions, types as apiTypes } from '../api';
import { ActionsObservable, StateObservable } from 'redux-observable';
import { toArray } from 'rxjs/operators';
import { Observable, of } from 'rxjs';


describe('navigation epic', () => {
    it('dispatches action to update loaded manufacturers', (done) => {
        const a = apiActions.listManufacturersSuccess(['test']);
        const state = {
            navigation: reducer({
                hash: 'test',
                manufacturers: [],
                tab: 'test'
            }, a),
            api: {
                manufacturers: {
                    manufacturers: ['test']
                }
            }
        };
        const action$ = ActionsObservable.of(a);
        const state$ = of(state);
        epick(action$, state$)
            .pipe(toArray())
            .subscribe((outputActions) => {
                expect(outputActions).toEqual([
                    actions.updateNavigationManufacturers(['test'])
                ]);
                done();
            });
    });

    it('dispatches action when click on manufacterer', (done) => {
        const a = actions.expandManufacturer('test');
        const state = {
            navigation: reducer({
                hash: 'test',
                manufacturers: [],
                tab: 'test'
            }, a),
            api: {
                manufacturers: {}
            }
        };
        const action$ = ActionsObservable.of(a);
        const state$ = of(state);
        epick(action$, state$)
            .pipe(toArray())
            .subscribe((outputActions) => {
                expect(outputActions).toEqual([
                    actions.updateLoactionHash('test'),
                    actions.updateNavigationTab('catalog')
                ]);
                done();
            });
    });
});
