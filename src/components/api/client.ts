export interface ICar {
    stockNumber: number;
    color?: string;
    fuelType?: string;
    manufacturerName?: string;
    mileage?: { number: number; unit: string; };
    pictureUrl?: string;
    isLiked?: boolean;
};

const query = params => Object.keys(params)
    .map(k => encodeURIComponent(k) + '=' + encodeURIComponent(params[k]))
    .join('&');
         
const DOMAIN = 'https://auto1-mock-server.herokuapp.com';
const ENDPOINTS = {
    cars: (params = {}, stockNumber = null) => {
        const q = query(params);
        const url = [] as string[];

        if (stockNumber) {
            url.push(`${DOMAIN}/api/cars/${stockNumber}`);
        } else {
            url.push(`${DOMAIN}/api/cars`);
        }

        q && url.push(q);

        return url.join('?');
    },
    colors: () => `${DOMAIN}/api/colors`,
    manufacturers: () => `${DOMAIN}/api/manufacturers`
};

export async function fetchCar(stockNumber) {
    const req = await fetch(ENDPOINTS.cars({}, stockNumber));
    return await req.json() as ICar;
}

export async function listCars(params: { manufacturer?: string; color?: string; sort?: 'asc' | 'des'; page?: number; }) {
    const req = await fetch(ENDPOINTS.cars(params));
    return await req.json() as {
        cars: Array<ICar>;
        totalCarsCount: number;
        totalPageCount: number;
    };
}

export async function listColors() {
    const req = await fetch(ENDPOINTS.colors());
    return await req.json() as {
        colors: string[];
    };
}

export async function listManufacturers() {
    const req = await fetch(ENDPOINTS.manufacturers());
    return await req.json() as {
        manufacturers: Array<{ name: string; }>;
    };
}
