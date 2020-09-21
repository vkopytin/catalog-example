import { useCallback } from 'react';
import { connect } from 'react-redux';
import { bindActionCreators } from 'redux';
import { actions, selectors } from '../';
import { useEventListener } from '../../hooks/use_event_listener';


const mapStateToProps = (state, props) => {
    return {
        hash: selectors.navigationHash(state),
        ...props
    };
};

const mapDispatchToProps = (dispatch, props) => {
    return {
        ...props,
        ...bindActionCreators(actions, dispatch)
    };
}

const LocationElement = ({
    hash,
    updateLoactionHash
}) => {
    // Event handler utilizing useCallback ...
    // ... so that reference never changes.
    const handler = useCallback(
        (evnt) => updateLoactionHash(window.location.hash.replace(/^[#/]/, ''))
        ,
        [updateLoactionHash]
    );

    // Add event listener using our hook
    useEventListener('hashchange', handler);

    return null;
}

export const LocationView = connect(mapStateToProps, mapDispatchToProps)(LocationElement);
