import { actions } from './';
import * as client from './client';


jest.mock('./client');

const fetchCar = client.fetchCar;
const listCars = client.listCars;
const listColors = client.listColors;
const listManufacturers = client.listManufacturers;

beforeEach(() => {
    fetchCar.mockReset();
    listCars.mockReset();
    listColors.mockReset();
    listManufacturers.mockReset();
});

describe('api', () => {

    it('action fetch car', async () => {
        const fetchedCar = { data: 'test' };
        fetchCar.mockResolvedValue(fetchedCar);
        let index = 0;
        let finish: any = () => { throw new Error('Unexpected Finish'); };
        const done = new Promise((resolve, reject) => {
            finish = resolve;
            setTimeout(() => reject('timeout'), 1000);
        });
        const dispatchMock = jest.fn(a => {
            if (index++ > 0) {
                finish();
            };
            return a;
        });

        const res = actions.fetchCar('test')(dispatchMock);
        await done;
        expect(fetchCar).toBeCalled();
        expect(dispatchMock).toBeCalledTimes(2);
        expect(dispatchMock).toHaveBeenNthCalledWith(2, actions.fetchCarSuccess(fetchedCar))
    });

    it('action list cars', async () => {
        const fetchedCars = { cars: [{ stockNumber: 'test' }], totalCarsCount: 1, totalPageCount: 1 };
        listCars.mockResolvedValue(fetchedCars);
        let index = 0;
        let finish: any = () => { throw new Error('Unexpected Finish'); };
        const done = new Promise((resolve, reject) => {
            finish = resolve;
            setTimeout(() => reject('timeout'), 1000);
        });
        const dispatchMock = jest.fn(a => {
            if (index++ > 0) {
                finish();
            };
            return a;
        });

        const res = actions.listCars({
            color: 'test',
            manufacturer: 'test',
            page: 1,
            sort: 'desc'
        })(dispatchMock);
        await done;
        expect(listCars).toBeCalled();
        expect(dispatchMock).toBeCalledTimes(2);
        expect(dispatchMock).toHaveBeenNthCalledWith(2, actions.listCarsSuccess(fetchedCars))
    });

    it('action list colors', async () => {
        const fetchedColors = ['test'];
        listColors.mockResolvedValue(fetchedColors);
        let index = 0;
        let finish: any = () => { throw new Error('Unexpected Finish'); };
        const done = new Promise((resolve, reject) => {
            finish = resolve;
            setTimeout(() => reject('timeout'), 1000);
        });
        const dispatchMock = jest.fn(a => {
            if (index++ > 0) {
                finish();
            };
            return a;
        });

        const res = actions.listColors()(dispatchMock);
        await done;
        expect(listColors).toBeCalled();
        expect(dispatchMock).toBeCalledTimes(2);
        expect(dispatchMock).toHaveBeenNthCalledWith(2, actions.listColorsSuccess(fetchedColors))
    });

    it('action list manufacturers', async () => {
        const fetchedManufacturers = ['test'];
        listManufacturers.mockResolvedValue(fetchedManufacturers);
        let index = 0;
        let finish: any = () => { throw new Error('Unexpected Finish'); };
        const done = new Promise((resolve, reject) => {
            finish = resolve;
            setTimeout(() => reject('timeout'), 1000);
        });
        const dispatchMock = jest.fn(a => {
            if (index++ > 0) {
                finish();
            };
            return a;
        });

        const res = actions.listManufacturers()(dispatchMock);
        await done;
        expect(listManufacturers).toBeCalled();
        expect(dispatchMock).toBeCalledTimes(2);
        expect(dispatchMock).toHaveBeenNthCalledWith(2, actions.listManufacturersSuccess(fetchedManufacturers))
    });
});
