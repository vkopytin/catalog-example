import React from 'react';
import './App.css';
import { Loading, MainApp } from './controls';
import { connect } from 'react-redux';
import { bindActionCreators } from 'redux';
import { actions, selectors } from './components';


export const AppElement = ({
  appLoading
}) => {
  return (
    <div className="App">
      {appLoading && <Loading />}
      {appLoading || <MainApp />}
    </div>
  );
}

const mapStateToProps = (state, props) => {
  return {
    ...props,
    appLoading: selectors.homeAppLoading(state)
  };
}

const mapDispatchToProps = (dispatch, props) => {
  return {
    ...props,
    ...bindActionCreators(actions, dispatch)
  };
}

const App = connect(mapStateToProps, mapDispatchToProps)(AppElement);

export { App };
