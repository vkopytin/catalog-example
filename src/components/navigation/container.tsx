import * as React from 'react';
import { connect } from 'react-redux';
import { Catalog, Favorites, selectors, LocationView } from '../';


const mapStateToProps = (state, props) => {
    return {
        navigationTab: selectors.navigationTab(state),
        ...props
    };
}

export const MainContainerElement = ({ navigationTab }) => <>
    <LocationView/>
    {navigationTab === 'catalog' && <Catalog />}
    {navigationTab === 'favorites' && <Favorites />}
</>;

export const MainContainer = connect(mapStateToProps)(MainContainerElement);
