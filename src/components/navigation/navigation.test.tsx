import * as React from 'react';
import { mount } from 'enzyme';
import { actions, reducer, selectors } from './';
import { DrawerLayoutElement } from './drawer_layout';
import ListItem from '@material-ui/core/ListItem';


describe('./catalog', () => {

    it('Drawer layout view', () => {
        const props = {
            manufacturers: [{ name: 'test'}],
            children: null,
            expandedManufacturer: '',
            expandManufacturer: false,
            navigationTab: 'catalog',
            updateNavigationTab: () => { }
        };
        const tree = mount(
            <DrawerLayoutElement {...props} />
        );
        const items = tree.find(ListItem);
        expect(items.length).toEqual(2);
    });

    it('reduser - navigation hash', () => {
        const navigation = reducer({
            hash: '',
            manufacturers: [],
            tab: 'catalog'
        }, actions.updateLoactionHash('test'));
        expect(selectors.navigationHash({ navigation })).toEqual('test');
    });

    it('reduser - catalog manufacturer', () => {
        const navigation = reducer({
            hash: '',
            manufacturers: ['test'] as never[],
            tab: 'catalog'
        }, actions.expandManufacturer('test'));
        expect(selectors.navigationManufacturers({ navigation })).toEqual(['test']);
    });

    it('reduser - catalog manufacturers', () => {
        const navigation = reducer({
            hash: '',
            manufacturers: [],
            tab: 'catalog'
        }, actions.updateNavigationManufacturers(['test']));
        expect(selectors.navigationManufacturers({ navigation })).toEqual(['test']);
    });

});
