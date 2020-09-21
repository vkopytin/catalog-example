import * as React from 'react';
import { mount } from 'enzyme';
import { actions, reducer, selectors } from './';
import { CatalogElement } from './catalog';
import ListItem from '@material-ui/core/ListItem';


describe('./catalog', () => {

    it('catalog view', () => {
        const props = {
            classes: {},
            colors: ['test'],
            selectedColor: 'test',
            updateSelectedColor: () => { },
            updateSelectedCar: () => { },
            catalogSelectedPage: 1,
            catalogTotalPages: 10,
            updateSelectedPage: () => { },
            selectedCar: {
                stockNumber: 'test',
                name: 'test',
                pictureUrl: 'test'
            },
            manufacturers: [{ name: 'test'}],
            expandedManufacturer: 'test',
            expandManufacturer: 'test',
            hash: 'test',
            cars: [{
                stockNumber: 'test',
                name: 'test',
                pictureUrl: 'test'
            }],
            sort: 'asc',
            catalogDisplay: 'list',
            width: 6
        };
        const tree = mount(
            <CatalogElement {...props} />
        );
        const items = tree.find(ListItem);
        expect(items.length).toEqual(1);
    });

    it('reduser - catalog cars', () => {
        const catalog = reducer({
            colors: [],
            selectedColor: false,
            cars: [],
            totalCarsCount: 0,
            totalPageCount: 0,
            display: 'list',
            sort: 'asc'
        }, actions.updateCatalogCars({ cars: ['test'], totalCarsCount: 1, totalPageCount: 1 }));
        expect(selectors.catalogListCars({ catalog })).toEqual(['test']);
        expect(selectors.catalogTotalPages({ catalog })).toEqual(1);
    });

    it('reduser - catalog colors', () => {
        const catalog = reducer({
            colors: [],
            selectedColor: false,
            cars: [],
            totalCarsCount: 0,
            totalPageCount: 0,
            display: 'list',
            sort: 'asc'
        }, actions.updateCatalogColors(['test']));
        expect(selectors.catalogColors({ catalog })).toEqual(['test']);
    });

    it('reduser - catalog models', () => {
        const catalog = reducer({
            colors: [],
            selectedColor: false,
            cars: [],
            totalCarsCount: 0,
            totalPageCount: 0,
            display: 'list',
            sort: 'asc'
        }, actions.updateCatalogModels(['test']));
        expect(selectors.catalogModels({ catalog })).toEqual(['test']);
    });

    it('reduser - catalog selected color', () => {
        const catalog = reducer({
            colors: [],
            selectedColor: false,
            cars: [],
            totalCarsCount: 0,
            totalPageCount: 0,
            display: 'list',
            sort: 'asc'
        }, actions.updateSelectedColor('test'));
        expect(selectors.catalogSelectedColor({ catalog })).toEqual('test');
    });

    it('reduser - catalog selected page', () => {
        const catalog = reducer({
            colors: [],
            selectedColor: false,
            cars: [],
            totalCarsCount: 0,
            totalPageCount: 0,
            display: 'list',
            sort: 'asc'
        }, actions.updateSelectedPage(1));
        expect(selectors.catalogSelectedPage({ catalog })).toEqual(1);
    });

    it('reduser - catalog selected car', () => {
        const car = { name: 'test' };
        const catalog = reducer({
            colors: [],
            selectedColor: false,
            cars: [],
            totalCarsCount: 0,
            totalPageCount: 0,
            display: 'list',
            sort: 'asc'
        }, actions.updateSelectedCar(car));
        expect(selectors.catalogSelectedCar({ catalog })).toEqual(car);
    });
});
