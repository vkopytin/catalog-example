import { Subject } from 'rxjs';
import { ActionsObservable, StateObservable, Epic } from 'redux-observable';
import { Action } from 'redux';


export const testEpic = <A extends Action<any>, S, D>(
    epic: Epic<A, A, S, D>, // A, S, D will be inferred from here
    deps: Partial<D> = ({} as unknown) as D,
    initialState = {}
) => {
    const actionSubject = new Subject<A>();
    const action$ = new ActionsObservable(actionSubject);
    const emitAction = actionSubject.next.bind(actionSubject);

    const stateSubject = new Subject<S>();
    const state$ = new StateObservable(stateSubject, initialState as S);
    const emitState = stateSubject.next.bind(stateSubject);

    const epicEmissions: A[] = [];
    const epic$ = epic(action$, state$, deps as D);
    epic$.subscribe(e => epicEmissions.push(e));

    return { emitAction, emitState, epicEmissions, epic$ };
};
