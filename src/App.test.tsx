import React from 'react';
import { render } from '@testing-library/react';
import { App } from './App';
import configureMockStore from 'redux-mock-store';
import { Provider } from 'react-redux';


const mockStore = configureMockStore();
const store = mockStore({
  home: {
    appLoading: true
  },
  navigation: {
    hash: 'test',
    manufacturers: []
  },
  catalog: {
    colors: [],
    cars: []
  }
});

describe('<AppElement/>', () => {
  test('renders loading overlay link (loading overlay)', () => {
    const { getByText } = render(<Provider store={store}><App appLoading={true} /></Provider>);
    const linkElement = getByText(/loading overlay/i);
    expect(linkElement).toBeInTheDocument();
  });

  test('doesn\'t render loading overlay link (no loading overlay)', () => {
    const { queryByText } = render(<Provider store={store}><App appLoading={false} /></Provider>);
    const linkElement = queryByText(/loading overlay/i);
    expect(linkElement).not.toBeInTheDocument();
  });
});
