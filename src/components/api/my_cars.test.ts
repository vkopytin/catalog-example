import { MyCars } from './my_cars';


// Storage Mock
function storageMock() {
    var storage = {};

    return {
        setItem: function (key, value) {
            storage[key] = value || '';
        },
        getItem: function (key) {
            return key in storage ? storage[key] : null;
        },
        removeItem: function (key) {
            delete storage[key];
        },
        get length() {
            return Object.keys(storage).length;
        },
        key: function (i) {
            var keys = Object.keys(storage);
            return keys[i] || null;
        }
    };
}

describe('checking my cars storage', () => {
    it('add item', async () => {
        const myCars = new MyCars(storageMock());

        const item = { stockNumber: 123 };
        await myCars.add(item);
        const items = await myCars.list();
        expect(items).toEqual({ '123': item });
    });

    it('remove item', async () => {
        const myCars = new MyCars(storageMock());

        const item = { stockNumber: 123 };
        await myCars.add(item);
        const items = await myCars.list();
        await myCars.remove(123);
        const items2 = await myCars.list();

        expect(items).toEqual({ '123': item });
        expect(items2).toEqual({});
    });

    it('check liked', async () => {
        const myCars = new MyCars(storageMock());

        const item = { stockNumber: 123 };
        const item2 = { stockNumber: 456 };
        await myCars.add(item);
        await myCars.add(item2);
        const items = await myCars.list();
        await myCars.remove(123);
        const items2 = await myCars.list();

        expect(items).toEqual({ '123': item, '456': item2 });
        expect(items2).toEqual({ '456': item2 });
        const liked = await myCars.checkLiked(['123', '456', '789']);
        expect(liked).toEqual([false, true, false]);
    });
});
