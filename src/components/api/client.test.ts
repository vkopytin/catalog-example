import * as client from './client';


global.fetch = jest.fn(() =>
  Promise.resolve({
    json: () => Promise.resolve({ rates: { CAD: 1.42 } }),
  })
);

beforeEach(() => {
  fetch.mockClear();
});

describe('api', () => {

    it('action fetch car', async () => {
        const res = await client.listCars({ manufacturer: 'test', color: 'test', page: 1, sort: 'asc' });

        expect(fetch).toBeCalledWith('https://auto1-mock-server.herokuapp.com/api/cars?manufacturer=test&color=test&page=1&sort=asc');
    });
});
